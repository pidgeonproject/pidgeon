/***************************************************************************
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) version 3.                                           *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.         *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Client
{
    [Serializable]
    [System.ComponentModel.ToolboxItem(true)]
    public partial class Scrollback : Gtk.Bin
    {
        /// <summary>
        /// Maximal size of text
        /// </summary>
        private int scrollback_max = Configuration.Scrollback.scrollback_plimit;
        /// <summary>
        /// List of all lines in this textbox
        /// </summary>
        private List<ContentLine> ContentLines = new List<ContentLine>();
        /// <summary>
        /// Owner window
        /// </summary>
        public Graphics.Window owner = null;
        private List<ContentLine> UndrawnLines = new List<ContentLine>();
        private string Link = "";
        public bool simple = true;
        private DateTime lastDate;
        private bool ScrollingEnabled = true;
        private bool ReloadWaiting = false;
        private bool Changed = false;
        public bool isMicro = false;
        private string LogfilePath = null;
        public bool SortNeeded = false;
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        public global::Gtk.TextView simpleview;
        private GLib.TimeoutHandler timer2;
        public RichTBox RT = null;
        public string SelectedLink = null;
        private int ScrollTime = 0;
        public bool Scrolling = true;
        private bool destroyed = false;
        private bool running = false;

        /// <summary>
        /// This will return true in case object was requested to be disposed
        /// you should never work with objects that return true here
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return destroyed;
            }
        }

        new public List<ContentLine> Data
        {
            get
            {
                List<ContentLine> data = new List<ContentLine>();
                lock (ContentLines)
                {
                    data.AddRange(ContentLines);
                }
                return data;
            }
        }

        public int Lines
        {
            get
            {
                return ContentLines.Count;
            }
        }

        public bool WindowVisible()
        {
            if (owner != null)
            {
                if (owner != Core._Main.Chat)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a new scrollback instance
        /// </summary>
        public Scrollback()
        {
            ReloadWaiting = true;
        }

        public Scrollback(Graphics.Window _ParentWindow)
        {
            this.owner = _ParentWindow;
            ReloadWaiting = true;
        }

        ~Scrollback()
        {
            if (Configuration.Kernel.Debugging)
            {
                if (owner != null)
                {
                    Core.DebugLog("Destructor called for scrollback " + owner.name);
                }
                else
                {
                    Core.DebugLog("Destructor called for scrollback of no name");
                }
                //Core.DebugLog("Released: " + Core.GetSizeOfObject(this).ToString() + " bytes of memory");
            }
        }

        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            global::Stetic.BinContainer.Attach(this);
            this.Name = "Client.ScrollbackWidget";
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            this.simpleview = new global::Gtk.TextView();
            this.simpleview.CanFocus = true;
            this.simpleview.WrapMode = Gtk.WrapMode.Word;
            this.Visible = true;
            this.simpleview.Name = "simpleview";
            this.simpleview.Editable = false;
            this.simpleview.DoubleBuffered = true;
            this.GtkScrolledWindow.Add(this.simpleview);
            this.RT = new RichTBox();
            if (!isMicro)
            {
                this.simpleview.PopulatePopup += new Gtk.PopulatePopupHandler(CreateMenu_simple);
                this.RT.textView.PopulatePopup += new Gtk.PopulatePopupHandler(CreateMenu_rt);
                this.RT.textView.ButtonPressEvent += new Gtk.ButtonPressEventHandler(Click);
            }
            timer2 = new GLib.TimeoutHandler(timer2_Tick);
            simpleview.ModifyBase(Gtk.StateType.Normal, Core.fromColor(Configuration.CurrentSkin.backgroundcolor));
            simpleview.ModifyText(Gtk.StateType.Normal, Core.fromColor(Configuration.CurrentSkin.colordefault));
            GLib.Timeout.Add(200, timer2);
            this.Add(this.GtkScrolledWindow);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }

            this.Hide();
        }

        private void simpleviewFinished(object o, EventArgs r)
        {
            try
            {
                if (ScrollingEnabled)
                {
                    simpleview.ScrollToIter(simpleview.Buffer.GetIterAtLine(ContentLines.Count), 0, true, 0, 0);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Change the text to specified list
        /// </summary>
        /// <param name="text"></param>
        public void SetText(List<ContentLine> text)
        {
            lock (UndrawnLines)
            {
                UndrawnLines.Clear();
                lock (ContentLines)
                {
                    ContentLines.Clear();
                    ContentLines.AddRange(text);
                }
            }
            ReloadWaiting = true;
            Changed = true;
        }

        /// <summary>
        /// This function switch between rendering types
        /// </summary>
        /// <param name="advanced"></param>
        public void Switch(bool advanced)
        {
            if (!Configuration.Memory.EnableSimpleViewCache)
            {
                simpleview.Buffer.Text = "";
            }
            if (advanced)
            {
                if (simple)
                {
                    simple = false;
                    if (this.Child != null)
                    {
                        this.Remove(this.Child);
                    }
                    this.Add(RT);
                    toggleAdvancedLayoutToolStripMenuItem.Checked = true;
                    toggleSimpleLayoutToolStripMenuItem.Checked = false;
                    return;
                }
                Reload(true, true);
                return;
            }
            toggleAdvancedLayoutToolStripMenuItem.Checked = false;
            toggleSimpleLayoutToolStripMenuItem.Checked = true;
            simple = true;
            if (this.Child != null)
            {
                this.Remove(this.Child);
            }
            this.Add(GtkScrolledWindow);

            this.ShowAll();

            Reload(true, true);
        }

        public void Create()
        {
            Build();
            RT.scrollback = this;
            scrollToolStripMenuItem.Checked = true;
            HideLn();
            lastDate = DateTime.MinValue;
            if (owner != null && owner.isPM)
            {
                listAllChannelsToolStripMenuItem.Visible = true;
            }
            Switch(true);
        }

        public void ReloadSimple()
        {
            lock (ContentLines)
            {
                StringBuilder everything = new StringBuilder("");
                foreach (ContentLine _line in ContentLines)
                {
                    everything.Append(Configuration.Scrollback.format_date.Replace("$1", _line.time.ToString(Configuration.Scrollback.timestamp_mask)) + Core.RemoveSpecial(_line.text) + Environment.NewLine);
                }
                simpleview.Buffer.Text = "";
                simpleview.Buffer.InsertAtCursor(everything.ToString());
                if (ScrollingEnabled)
                {
                    simpleview.ScrollToIter(simpleview.Buffer.GetIterAtLine(ContentLines.Count), 0, true, 0, 0);
                }
            }
        }

        public void HideLn()
        {
            mode1b2ToolStripMenuItem.Visible = false;
            toolStripMenuItem1.Visible = false;
            openLinkInBrowserToolStripMenuItem.Visible = false;
            joinToolStripMenuItem.Visible = false;
            mode1e2ToolStripMenuItem.Visible = false;
            mode1I2ToolStripMenuItem.Visible = false;
            mode1q2ToolStripMenuItem.Visible = false;
        }
        
        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void _Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }
            destroyed = true;
            lock (UndrawnLines)
            {
                UndrawnLines.Clear();
            }
            lock (ContentLines)
            {
                ContentLines.Clear();
            }
            RT = null;
            owner = null;
        }
        
        public bool IncreaseOffset()
        {
            if (scrollback_max < ContentLines.Count)
            {
                scrollback_max += Configuration.Scrollback.DynamicSize;
                Changed = true;
                Reload();
                return true;
            }
            return false;
        }

        private void ResetScrolling()
        {
            ScrollTime = 0;
            Scrolling = true;
        }

        public bool RestoreOffset()
        {
            if (scrollback_max != Configuration.Scrollback.scrollback_plimit)
            {
                scrollback_max = Configuration.Scrollback.scrollback_plimit;
                Changed = true;
                Reload();
                return true;
            }
            return false;
        }

        public bool timer2_Tick()
        {
            try
            {
                if (running)
                {
                    return true;
                }
                running = true;
                if (IsDestroyed)
                {
                    // the window was destroyed so we can't work with it
                    return false;
                }
                if (WindowVisible())
                {
                    if (ScrollingEnabled && Scrolling)
                    {
                        if (ScrollTime < 10)
                        {
                            ScrollTime++;
                            if (simple)
                            {
                                lock (simpleview.Buffer.Text)
                                {
                                    simpleview.ScrollToIter(simpleview.Buffer.GetIterAtLine(ContentLines.Count), 0, true, 0, 0);
                                }
                            }
                            if (!simple)
                            {
                                RT.ScrollToBottom();
                            }
                        }
                        else
                        {
                            Scrolling = false;
                        }
                    }
                    lock (UndrawnLines)
                    {
                        if (!ReloadWaiting)
                        {
                            if (UndrawnLines.Count > 0)
                            {
                                // if there is too many lines in buffer we write only last
                                if (UndrawnLines.Count > scrollback_max && !simple)
                                {
                                    lock (ContentLines)
                                    {
                                        ContentLines.Clear();
                                    }
                                    int from = UndrawnLines.Count - scrollback_max - 1;
                                    while (from < UndrawnLines.Count)
                                    {
                                        InsertLineToText(UndrawnLines[from], false);
                                        from++;
                                    }
                                    SortNeeded = true;
                                    ReloadWaiting = true;
                                    Changed = true;
                                } else
                                {
                                    foreach (ContentLine curr in UndrawnLines)
                                    {
                                        InsertLineToText(curr, false);
                                    }
                                }
                            }
                        }
                        UndrawnLines.Clear();
                    }
                    if (ReloadWaiting)
                    {
                        if (Reload())
                        {
                            ReloadWaiting = false;
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            running = false;
            return true;
        }
    }
}
