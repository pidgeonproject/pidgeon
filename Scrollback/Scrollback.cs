//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Pidgeon
{
    /// <summary>
    /// Scrollback
    /// 
    /// This is a widget that is used to store all text, every irc window is using it, this is the window
    /// where chat is written to.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class Scrollback : Gtk.Bin
    {
        /// <summary>
        /// Maximal number of lines, if this number is exceeded, the text may be trimmed at some occasions
        /// 
        /// However given the fact that text is appended, this trimming may not happen very often
        /// </summary>
        private int scrollback_max = Configuration.Scrollback.scrollback_plimit;
        /// <summary>
        /// List of all lines in this textbox
        /// </summary>
        private List<ContentLine> ContentLines = new List<ContentLine>();
        /// <summary>
        /// Current line buffer
        /// 
        /// This is used when a line without line ending is being created. You can append to this buffer and once the line is finished, the whole
        /// line is then inserted instead of just a part of it.
        /// </summary>
        private ContentLine EndingLine = null;
        /// <summary>
        /// Owner window
        /// </summary>
        public Graphics.Window owner = null;
        /// <summary>
        /// Lines that are waiting to be written because they were inserted from different thread
        /// </summary>
        private List<ContentLine> UndrawnLines = new List<ContentLine>();
        private List<TextPart> UndrawnTextParts = new List<TextPart>();
        /// <summary>
        /// When you hover over a link it's added in this buffer so that you can retrieve it when you click mouse
        /// </summary>
        private string Link = "";
        /// <summary>
        /// Simple mode
        /// </summary>
        private bool simple = true;
        /// <summary>
        /// This is a newest value of time of a line of text, in case a new line of text has lower value, it needs to be inserted
        /// to middle instead of end, so that append function can't be used and whole text box needs to be redrawn
        /// </summary>
        private DateTime lastDate;
        /// <summary>
        /// If scrolling is enabled, if this is false the chat will not be scrolled when new line is inserted
        /// </summary>
        public bool ScrollingEnabled = true;
        /// <summary>
        /// This means the scrollback is waiting for reload, because there was a modification that requires it called from
        /// different thread
        /// </summary>
        private bool ReloadWaiting = false;
        /// <summary>
        /// In case that a scrollback was changed from different thread and a change was not written
        /// </summary>
        private bool Changed = false;
        /// <summary>
        /// MicroChat
        /// </summary>
        public bool isMicro = false;
        private string LogfilePath = null;
        /// <summary>
        /// Whether items in buffer needs to be sorted
        /// </summary>
        public bool SortNeeded = false;
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        /// <summary>
        /// Pointer to text box
        /// </summary>
        public global::Gtk.TextView simpleview;
        private GLib.TimeoutHandler timer2;
        /// <summary>
        /// Pointer to RT box
        /// </summary>
        public RichTBox RT = null;
        /// <summary>
        /// URL of link that is currently selected
        /// </summary>
        public string SelectedLink = null;
        private bool destroyed = false;
        /// <summary>
        /// Font used by simple view
        /// </summary>
        public Pango.FontDescription Font = new Pango.FontDescription();
        private bool running = false;
        /// <summary>
        /// If this is true scrolling will be automatically enabled after some delay
        /// </summary>
        private bool EnableScrolling = false;
        /// <summary>
        /// Return true in case that the current line is empty
        /// </summary>
        public bool IsEmtpy
        {
            get
            {
                lock(UndrawnTextParts)
                {
                    if(UndrawnTextParts.Count > 0 || EndingLine != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Simple
        /// </summary>
        public bool IsSimple
        {
            get
            {
                return simple;
            }
        }

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

        /// <summary>
        /// Content
        /// </summary>
        new public List<ContentLine> Data
        {
            get
            {
                List<ContentLine> data = new List<ContentLine>();
                lock(ContentLines)
                {
                    data.AddRange(ContentLines);
                }
                return data;
            }
        }

        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get
            {
                StringBuilder text = new StringBuilder("");
                lock(ContentLines)
                {
                    foreach(ContentLine _line in ContentLines)
                    {
                        text.Append(Configuration.Scrollback.format_date.Replace("$1", _line.time.ToString(Configuration.Scrollback.timestamp_mask)));
                        text.Append(Core.RemoveSpecial(_line.text));
                        text.Append(Environment.NewLine);
                    }
                }
                return text.ToString();
            }
        }

        /// <summary>
        /// Number of lines
        /// </summary>
        public int Lines
        {
            get
            {
                return ContentLines.Count;
            }
        }

        /// <summary>
        /// Retrieve selected text
        /// </summary>
        public string SelectedText
        {
            get
            {
                Gtk.TextIter a;
                Gtk.TextIter b;
                if(IsSimple)
                {
                    if(simpleview.Buffer.GetSelectionBounds(out a, out b))
                    {
                        return simpleview.Buffer.GetText(a, b, false);
                    }
                    return null;
                }
                if(RT.textView.Buffer.GetSelectionBounds(out a, out b))
                {
                    return RT.textView.Buffer.GetText(a, b, false);
                }
                return null;
            }
        }

        /// <summary>
        /// Creates a new scrollback instance
        /// </summary>
        public Scrollback()
        {
            ReloadWaiting = true;
        }

        /// <summary>
        /// Creates a new scrollback instance
        /// </summary>
        /// <param name="_ParentWindow"></param>
        public Scrollback(Graphics.Window _ParentWindow)
        {
            Font.Family = Configuration.CurrentSkin.LocalFont;
            Font.Size = Configuration.CurrentSkin.FontSize;
            this.owner = _ParentWindow;
            ReloadWaiting = true;
        }

#if DEBUG
        /// <summary>
        /// Destructor
        /// </summary>
        ~Scrollback()
        {
            if (Configuration.Kernel.Debugging)
            {
                if (owner != null)
                {
                    Core.DebugLog("Destructor called for scrollback " + owner.WindowName);
                }
                else
                {
                    Core.DebugLog("Destructor called for scrollback of no name");
                }
                //Core.DebugLog("Released: " + Core.GetSizeOfObject(this).ToString() + " bytes of memory");
            }
        }
#endif

        /// <summary>
        /// This will modify a size of all scrollbacks and text areas using the given value
        /// </summary>
        /// <param name="adjustment"></param>
        public void ResizeText(int adjustment)
        {
            if(Configuration.CurrentSkin.FontSize + adjustment < 1)
            {
                return;
            }

            Configuration.CurrentSkin.FontSize = Configuration.CurrentSkin.FontSize + adjustment;
            Font.Size = Configuration.CurrentSkin.FontSize;
            simpleview.ModifyFont(Font);
            RT.DefaultFont.Size = Configuration.CurrentSkin.FontSize;
            //RT.textView.ModifyFont(RT.DefaultFont);
            ScrollingEnabled = false;
            Reload(true);
            EnableScrolling = true;
        }

        /// <summary>
        /// Return true in case the window is visible
        /// </summary>
        /// <returns></returns>
        public bool WindowVisible()
        {
            if(owner != null)
            {
                if(owner != Core.SystemForm.Chat)
                {
                    return false;
                }
            }
            return true;
        }

        private void Build()
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
            this.simpleview.SizeAllocated += new Gtk.SizeAllocatedHandler(Scroll2);
            this.simpleview.Editable = false;
            this.simpleview.ModifyFont(Font);
            this.simpleview.ScrollEvent += new Gtk.ScrollEventHandler(Scroll);
            this.simpleview.DoubleBuffered = true;
            this.GtkScrolledWindow.Add(this.simpleview);
            this.RT = new RichTBox(this);
            if(!isMicro)
            {
                this.simpleview.PopulatePopup += new Gtk.PopulatePopupHandler(CreateMenu_simple);
                this.RT.textView.PopulatePopup += new Gtk.PopulatePopupHandler(CreateMenu_rt);
                this.RT.textView.ButtonPressEvent += new Gtk.ButtonPressEventHandler(Click);
                this.RT.textView.KeyPressEvent += new Gtk.KeyPressEventHandler(PressEvent);
            }
            timer2 = new GLib.TimeoutHandler(timer2_Tick);
            simpleview.ModifyBase(Gtk.StateType.Normal, Core.FromColor(Configuration.CurrentSkin.BackgroundColor));
            simpleview.ModifyText(Gtk.StateType.Normal, Core.FromColor(Configuration.CurrentSkin.ColorDefault));
            simpleview.KeyPressEvent += new Gtk.KeyPressEventHandler(PressEvent);
            GLib.Timeout.Add(200, timer2);
            this.Add(this.GtkScrolledWindow);
            if((this.Child != null))
            {
                this.Child.ShowAll();
            }
        }

        private void PressEvent(object sender, Gtk.KeyPressEventArgs e)
        {
            try
            {
                if(Forms.Main.ShortcutHandle(sender, e))
                {
                    e.RetVal = true;
                    return;
                }
            } catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        /// <summary>
        /// Change the limit of number of max lines
        /// </summary>
        public void IncreaseLimits()
        {
            if(scrollback_max < ContentLines.Count)
            {
                scrollback_max = scrollback_max + 800;
                Reload(true);
            }
        }

        /// <summary>
        /// Change the text to specified list
        /// </summary>
        /// <param name="text"></param>
        public void SetText(List<ContentLine> text)
        {
            lock(UndrawnLines)
            {
                UndrawnLines.Clear();
                lock(ContentLines)
                {
                    ContentLines.Clear();
                    ContentLines.AddRange(text);
                }
            }
            ReloadWaiting = true;
            Changed = true;
        }

        private void Scroll2(object sender, Gtk.SizeAllocatedArgs e)
        {
            if(IsDestroyed)
            {
                return;
            }
            if(ScrollingEnabled)
            {
                simpleview.ScrollToIter(simpleview.Buffer.EndIter, 0, false, 0, 0);
            }
        }

        /// <summary>
        /// This function switch between rendering types
        /// </summary>
        /// <param name="advanced"></param>
        public void Switch(bool advanced)
        {
            if(!Configuration.Memory.EnableSimpleViewCache)
            {
                simpleview.Buffer.Text = "";
            }
            if(advanced)
            {
                if(simple)
                {
                    simple = false;
                    if(this.Child != null)
                    {
                        this.Remove(this.Child);
                    }
                    this.Add(RT);
                    toggleAdvancedLayoutToolStripMenuItem.Checked = true;
                    toggleSimpleLayoutToolStripMenuItem.Checked = false;
                    Reload(true);
                }
                return;
            }
            if (!simple || !Configuration.Memory.EnableSimpleViewCache)
            {
                toggleAdvancedLayoutToolStripMenuItem.Checked = false;
                toggleSimpleLayoutToolStripMenuItem.Checked = true;
                simple = true;
                if(this.Child != null)
                {
                    this.Remove(this.Child);
                }
                this.Add(GtkScrolledWindow);
                this.ShowAll();
                Reload(true);
            }
        }

        /// <summary>
        /// Create this control, this function exist so that constructor can be called from a different thread
        /// </summary>
        public void Create()
        {
            Build();
            scrollToolStripMenuItem.Checked = true;
            HideLn();
            lastDate = DateTime.MinValue;
            Switch(true);
        }

        /// <summary>
        /// Reload simple
        /// </summary>
        public void ReloadSimple()
        {
            StringBuilder everything = new StringBuilder("");
            lock(ContentLines)
            {
                foreach(ContentLine _line in ContentLines)
                {
                    if(Configuration.Scrollback.KeepSpecialCharsSimple)
                    {
                        everything.Append(Configuration.Scrollback.format_date.Replace("$1", _line.time.ToString(Configuration.Scrollback.timestamp_mask)));
                        everything.AppendLine(_line.text);
                    } else
                    {
                        everything.Append(Configuration.Scrollback.format_date.Replace("$1", _line.time.ToString(Configuration.Scrollback.timestamp_mask)));
                        everything.Append(Core.RemoveSpecial(_line.text));
                        everything.Append(Environment.NewLine);
                    }
                }
            }
            simpleview.Buffer.Text = everything.ToString();
            if(ScrollingEnabled)
            {
                simpleview.ScrollToIter(simpleview.Buffer.GetIterAtLine(ContentLines.Count), 0, true, 0, 0);
            }
        }

        [GLib.ConnectBefore]
        private void Scroll(object sender, Gtk.ScrollEventArgs e)
        {
            try
            {
                if(Core.HoldingCtrl)
                {
                    if(e.Event.Direction == Gdk.ScrollDirection.Up)
                    {
                        ResizeText(400);
                    } else
                    {
                        ResizeText(-400);
                    }
                    e.RetVal = true;
                }
            } catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        /// <summary>
        /// Hide menu
        /// </summary>
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
            if(IsDestroyed)
            {
                return;
            }

            destroyed = true;
            lock(UndrawnLines)
            {
                UndrawnLines.Clear();
            }
            lock(ContentLines)
            {
                ContentLines.Clear();
            }
            if(simpleview != null)
            {
                simpleview.Destroy();
            }
            RT = null;
            owner = null;
            this.Destroy();
        }

        /// <summary>
        /// Draw all undrawn parts immediately and clear the array
        /// </summary>
        public void FlushParts()
        {
            if(!IsEmtpy)
            {
                lock(UndrawnTextParts)
                {
                    foreach(TextPart curr in UndrawnTextParts)
                    {
                        #pragma warning disable
                        InsertPart(curr.text, curr.style, false, curr.date.ToBinary());
                        #pragma warning enable
                    }
                    UndrawnTextParts.Clear();
                }
            }
        }

        private TextPart NewerPart(DateTime time)
        {
            lock(UndrawnTextParts)
            {
                foreach(TextPart k in UndrawnTextParts)
                {
                    if(k.date < time)
                    {
                        return k;
                    }
                }
            }
            return null;
        }

        private bool timer2_Tick()
        {
            try
            {
                if(running)
                {
                    return true;
                }
                running = true;
                if(IsDestroyed)
                {
                    // the window was destroyed so we can't work with it
                    return false;
                }
                if(WindowVisible() || !Configuration.Scrollback.DynamicReload)
                {
                    if(Font.Size != Configuration.CurrentSkin.FontSize)
                    {
                        ResizeText(0);
                    }
                    lock(UndrawnLines)
                    {
                        lock(ContentLines)
                        {
                            if(!ReloadWaiting)
                            {
                                // there are more undrawn lines than we actually want to max. have in scrollback, let's trim it all
                                if(Configuration.Memory.MaximumChannelBufferSize > 0 && Configuration.Memory.MaximumChannelBufferSize < ContentLines.Count)
                                {
                                    ContentLines.Sort();
                                    while(Configuration.Memory.MaximumChannelBufferSize < ContentLines.Count)
                                    {
                                        ContentLines.RemoveAt(0);
                                    }
                                }
                                if(Configuration.Memory.MaximumChannelBufferSize > 0 && Configuration.Memory.MaximumChannelBufferSize < UndrawnLines.Count)
                                {
                                    UndrawnLines.Sort();
                                    while(Configuration.Memory.MaximumChannelBufferSize < UndrawnLines.Count)
                                    {
                                        UndrawnLines.RemoveAt(0);
                                    }
                                }
                                lock(UndrawnTextParts)
                                {
                                    foreach(ContentLine curr in UndrawnLines)
                                    {
                                        if(!IsEmtpy)
                                        {
                                            TextPart text = NewerPart(curr.time);
                                            while(text != null)
                                            {
                                                InsertPartToText(text.text);
                                                UndrawnTextParts.Remove(text);
                                                text = NewerPart(curr.time);
                                            }
                                        }
                                        InsertLineToText(curr, false);
                                    }
                                    FlushParts();
                                }
                                UndrawnLines.Clear();
                            }
                        }
                    }
                    if(ReloadWaiting)
                    {
                        if(Reload())
                        {
                            ReloadWaiting = false;
                        }
                    }
                    if(EnableScrolling)
                    {
                        EnableScrolling = false;
                        ScrollingEnabled = true;
                    }
                }
            } catch (Exception fail)
            {
                Core.HandleException(fail);
            }
            running = false;
            return true;
        }
    }
}
