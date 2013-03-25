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
        private string LogfilePath = null;
        public bool SortNeeded = false;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		public global::Gtk.TextView simpleview;
		private GLib.TimeoutHandler timer2;

        public List<ContentLine> Data
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

        public enum MessageStyle
        {
            System,
            Message,
            Action,
            User,
            Channel,
            Kick,
            Join,
            Part,
        }

        public int Lines
        {
            get
            {
                return ContentLines.Count;
            }
        }

        [Serializable]
        public class ContentLine : IComparable
        {
            public DateTime time;
            public string text;
            public bool notice = false;
            public MessageStyle style;
            public ContentLine()
            {

            }

            public ContentLine(MessageStyle _style, string Text, DateTime when, bool _notice)
            {
                style = _style;
                time = when;
                text = Text;
                notice = _notice;
            }

            public int CompareTo(object obj)
            {
                if (obj is ContentLine)
                {
                    return this.time.CompareTo((obj as ContentLine).time);
                }
                return 0;
            }
        }

        public bool WindowVisible()
        {
            if (owner != null)
            {
                if (owner.Visible != true)
                {
                    return false;
                }
            }
            return true;
        }

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Client.ScrollbackWidget";
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			this.simpleview = new global::Gtk.TextView ();
			this.simpleview.CanFocus = true;
            this.simpleview.WrapMode = Gtk.WrapMode.Word;
            this.Visible = true;
			this.simpleview.Name = "simpleview";
			this.simpleview.Editable = false;
			this.GtkScrolledWindow.Add (this.simpleview);
			timer2 = new GLib.TimeoutHandler(timer2_Tick);
			GLib.Timeout.Add(200, timer2);
			this.Add (this.GtkScrolledWindow);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
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
            if (advanced)
            {
                if (simple)
                {
                    simple = false;
                    //RT.Visible = true;
                    //simpleview.Visible = false;
                    //toggleAdvancedLayoutToolStripMenuItem.Checked = true;
                    //toggleSimpleLayoutToolStripMenuItem.Checked = false;
                    //RT.RedrawText();
                    return;
                }
                Reload(true, true);
                return;
            }
            //toggleAdvancedLayoutToolStripMenuItem.Checked = false;
            //toggleSimpleLayoutToolStripMenuItem.Checked = true;
            simple = true;
            //simpleview.Visible = true;
            //RT.Visible = false;
            Reload(true, true);
        }

        /// <summary>
        /// Creates a new scrollback instance
        /// </summary>
        public Scrollback()
        {
            //this.BackColor = Configuration.CurrentSkin.backgroundcolor;
			Build ();
            ReloadWaiting = true;
        }

        public Scrollback(Graphics.Window _ParentWindow)
        {
            //this.BackColor = Configuration.CurrentSkin.backgroundcolor;
            this.owner = _ParentWindow;
            Build();

            ReloadWaiting = true;
        }

        public void Create()
        {
            //toggleAdvancedLayoutToolStripMenuItem.Checked = true;
        }

        private void Scrollback_Load(object sender, EventArgs e)
        {
            try
            {
                //RT.Spacing = Configuration.CurrentSkin.SBABOX_sp;
                //RT.BackColor = Configuration.CurrentSkin.backgroundcolor;
                //RT.Font = Configuration.CurrentSkin.SBABOX;
                //simpleview.BackColor = Configuration.CurrentSkin.backgroundcolor;
                //simpleview.ForeColor = Configuration.CurrentSkin.fontcolor;
                //RT.scrollback = this;
                HideLn();
                lastDate = DateTime.MinValue;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
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
				simpleview.Buffer.Text = everything.ToString();
                if (ScrollingEnabled)
                {
					Gtk.TextIter iter = simpleview.Buffer.GetIterAtLine (simpleview.Buffer.LineCount);
					simpleview.ScrollToIter(iter, 0, false, 0, 0);
                }
            }
        }

        public void HideLn()
        {
            //mode1b2ToolStripMenuItem.Visible = false;
            //toolStripMenuItem1.Visible = false;
            //openLinkInBrowserToolStripMenuItem.Visible = false;
            //joinToolStripMenuItem.Visible = false;
            //mode1e2ToolStripMenuItem.Visible = false;
            //mode1I2ToolStripMenuItem.Visible = false;
            //mode1q2ToolStripMenuItem.Visible = false;
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
                if (WindowVisible())
                {
                    lock (UndrawnLines)
                    {
                        if (!ReloadWaiting)
                        {
                            if (UndrawnLines.Count > 0)
                            {
                                foreach (ContentLine curr in UndrawnLines)
                                {
                                    InsertLineToText(curr, false);
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
			return true;
        }
    }
}
