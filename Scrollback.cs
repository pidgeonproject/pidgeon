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
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Forms;



namespace Client
{
    public partial class Scrollback : UserControl
    {
        private bool initializationComplete;
        private bool isDisposing;
        private BufferedGraphicsContext backbufferContext;
        private BufferedGraphics backbufferGraphics;
        private Graphics drawingGraphics;
        private List<ContentLine> Line = new List<ContentLine>();
        public TextBox Last;
        public static List<Scrollback> _control = new List<Scrollback>();
        public Window owner;
        public TreeNode ln;
        public bool show = true;
        public bool px1 = true;
        public bool px2 = false;

        public class ContentLine : IComparable
        {
            public ContentLine(MessageStyle _style, string Text, DateTime when)
            {
                style = _style;
                time = when;
                text = Text;
            }
            public int CompareTo(object obj)
            {
                if (obj is ContentLine)
                {
                    return this.time.CompareTo((obj as ContentLine).time);
                }
                return 0;
            }
            public DateTime time;
            public string text;
            public MessageStyle style;
        }
        private bool db = false;
        public bool Modified;


        protected override void Dispose(bool disposing)
        {
            isDisposing = true;
            lock (_control)
            {
                if (_control.Contains(this))
                {
                    _control.Remove(this);
                }
            }
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                // We must dispose of backbufferGraphics before we dispose of backbufferContext or we will get an exception.
                if (backbufferGraphics != null)
                    backbufferGraphics.Dispose();
                if (backbufferContext != null)
                    backbufferContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private void RecreateBuffers()
        {
            // Check initialization has completed so we know backbufferContext has been assigned.
            // Check that we aren't disposing or this could be invalid.
            if (!initializationComplete || isDisposing)
                return;

            // We recreate the buffer with a width and height of the control. The "+ 1" 
            // guarantees we never have a buffer with a width or height of 0. 
            backbufferContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

            // Dispose of old backbufferGraphics (if one has been created already)
            if (backbufferGraphics != null)
                backbufferGraphics.Dispose();

            // Create new backbufferGrpahics that matches the current size of buffer.
            backbufferGraphics = backbufferContext.Allocate(this.CreateGraphics(),
                new Rectangle(0, 0, Math.Max(this.Width, 1), Math.Max(this.Height, 1)));

            // Assign the Graphics object on backbufferGraphics to "drawingGraphics" for easy reference elsewhere.
            drawingGraphics = backbufferGraphics.Graphics;

            // This is a good place to assign drawingGraphics.SmoothingMode if you want a better anti-aliasing technique.

            // Invalidate the control so a repaint gets called somewhere down the line.
            this.Invalidate();
        }

        private void Redraw()
        {
            // In this Redraw method, we simply make the control fade from black to white on a timer.
            // But, you can put whatever you want here and detach the timer. The trick is just making
            // sure redraw gets called when appropriate and only when appropriate. Examples would include
            // when you resize, when underlying data is changed, when any of the draqwing properties are changed
            // (like BackColor, Font, ForeColor, etc.)
            if (drawingGraphics == null)
                return;


            // Force the control to both invalidate and update. 
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // If we've initialized the backbuffer properly, render it on the control. 
            // Otherwise, do just the standard control paint.
            if (!isDisposing && backbufferGraphics != null)
                backbufferGraphics.Render(e.Graphics);
        }

        public void create()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Assign our buffer context.
            backbufferContext = BufferedGraphicsManager.Current;
            initializationComplete = true;

            RecreateBuffers();

            Redraw();
            Scrollback_Load(null, null);
        }

        public void ResizeWebs(object sender, EventArgs e)
        {
            Data.Top = 0;
            Data.Left = 0;
            Data.Width = pwx1.Width - 2;
            Data.Height = pwx1.Height;
            webBrowser1.Top = 0;
            webBrowser1.Left = 0;
            webBrowser1.Height = pwx2.Height;
            webBrowser1.Width = pwx2.Width - 2;
        }

        public Scrollback()
        {
            lock (_control)
            {
                _control.Add(this);
            }
            this.BackColor = Configuration.CurrentSkin.backgroundcolor;
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

        public bool InsertText(string text, MessageStyle input_style, bool lg = true, string dt = null)
        {
            if (owner != Core._Main.Chat && ln != null)
            {
                switch (input_style)
                {
                    case MessageStyle.Kick:
                    case MessageStyle.System:
                        ln.ForeColor = Configuration.CurrentSkin.highlightcolor;
                        break;
                    case MessageStyle.Action:
                    case MessageStyle.Message:
                        if (ln.ForeColor != Configuration.CurrentSkin.highlightcolor)
                        {
                            ln.ForeColor = Configuration.CurrentSkin.colortalk;
                        }
                        break;
                    case MessageStyle.Part:
                    case MessageStyle.Channel:
                    case MessageStyle.User:
                    case MessageStyle.Join:
                        if (ln.ForeColor != Configuration.CurrentSkin.highlightcolor && ln.ForeColor != Configuration.CurrentSkin.colortalk)
                        {
                            ln.ForeColor = Configuration.CurrentSkin.joincolor;
                        }
                        break;
                    
                }
            }
            DateTime time = DateTime.Now;
            if (dt != null)
            {
                time = DateTime.Parse(dt);
            }
            lock(Line)
            {
                Line.Add(new ContentLine(input_style, text, time));
                if (dt != null)
                {
                    Line.Sort();
                }
            }
            Modified = true;
            if (lg == true && owner != null && owner._Network != null)
            {
                if (!Directory.Exists(Configuration.logs_dir))
                {
                    Directory.CreateDirectory(Configuration.logs_dir);
                }
                if (!Directory.Exists(Configuration.logs_dir + Path.DirectorySeparatorChar + owner._Network.server))
                {
                    System.IO.Directory.CreateDirectory(Configuration.logs_dir + Path.DirectorySeparatorChar + owner._Network.server);
                }
                if (!Directory.Exists(Configuration.logs_dir + Path.DirectorySeparatorChar + owner._Network.server + Path.DirectorySeparatorChar + owner.name))
                {
                    Directory.CreateDirectory(Configuration.logs_dir + Path.DirectorySeparatorChar + owner._Network.server + Path.DirectorySeparatorChar + owner.name);
                }
                if (Configuration.logs_txt)
                {
                    string stamp = "";
                    if (Configuration.chat_timestamp)
                    {
                        stamp = Configuration.format_date.Replace("$1", DateTime.Now.ToString(Configuration.timestamp_mask));
                    }
                    System.IO.File.AppendAllText(_getFileName() + ".txt", stamp + text + "\n");
                }
                if (Configuration.logs_html)
                {
                    string stamp = "";
                    if (Configuration.chat_timestamp)
                    {
                        stamp = Configuration.format_date.Replace("$1", DateTime.Now.ToString(Configuration.timestamp_mask));
                    }
                    System.IO.File.AppendAllText( _getFileName() + ".html", "<font size=\"" + Configuration.CurrentSkin.fontsize.ToString() + "px\" face=" + Configuration.CurrentSkin.localfont + ">" + stamp  +  System.Web.HttpUtility.HtmlEncode(text) + "</font><br>\n" );
                }
                if (Configuration.logs_xml)
                {
                    System.IO.File.AppendAllText(_getFileName() + ".xml", "<line time=\"" + DateTime.Now.ToBinary().ToString() + "\" style=\"" + input_style.ToString() + "\">" + System.Web.HttpUtility.HtmlEncode(text) + "</line>\n");
                }
            }
            return false;
        }

        public string _getFileName()
        {
            string name = Configuration.logs_dir + Path.DirectorySeparatorChar + owner._Network.server + Path.DirectorySeparatorChar + owner.name + Path.DirectorySeparatorChar + DateTime.Now.ToString(Configuration.logs_name).Replace("$1", owner.name);
            return name;
        }

        private void Scrollback_Load(object sender, EventArgs e)
        {
            //Reload(true);
            Data.DocumentText = "<html><head> </head><body onLoad=\"scroll()\" STYLE=\"background-color: " + Configuration.CurrentSkin.backgroundcolor.Name + "\">";
            webBrowser1.DocumentText  = "<html><head> </head><body onLoad=\"scroll()\" STYLE=\"background-color: " + Configuration.CurrentSkin.backgroundcolor.Name + "\">";
            pwx2.Dock = DockStyle.Fill;
            pwx1.BringToFront();
            pwx1.Dock = DockStyle.Fill;
            this.ResumeLayout();
        }

        public void Reload(bool fast = false)
        {
            Modified = false;
            string text = "<html><head><script type=\"text/javascript\">function scroll() {window.scrollBy(0," + Line.Count.ToString() + "00);} </script> </head><body onLoad=\"scroll()\" STYLE=\"background-color: " + Configuration.CurrentSkin.backgroundcolor.Name + "\">";
            lock (Line)
            {
                foreach (ContentLine _c in Line)
                {
                    string color = Configuration.CurrentSkin.fontcolor.Name;
                    switch (_c.style)
                    { 
                        case MessageStyle.Action:
                            color = Configuration.CurrentSkin.miscelancscolor.Name;
                            break;
                        case MessageStyle.Kick:
                            color = Configuration.CurrentSkin.kickcolor.Name;
                            break;
                        case MessageStyle.System:
                            color = Configuration.CurrentSkin.miscelancscolor.Name;
                            break;
                        case MessageStyle.Channel:
                            color = Configuration.CurrentSkin.colortalk.Name;
                            break;
                        case MessageStyle.User:
                            color = Configuration.CurrentSkin.changenickcolor.Name;
                            break;
                        case MessageStyle.Join:
                        case MessageStyle.Part:
                            color = Configuration.CurrentSkin.joincolor.Name;
                            break;
                    }
                    string stamp = "";
                    if (Configuration.chat_timestamp)
                    {
                        stamp = Configuration.format_date.Replace("$1", _c.time.ToString(Configuration.timestamp_mask));
                    }
                    text += "<font size=\"" + Configuration.CurrentSkin.fontsize.ToString() + "px\" color=\"" + color + "\" face=" + Configuration.CurrentSkin.localfont + ">" + stamp  +  System.Web.HttpUtility.HtmlEncode(_c.text) + "</font><br>";
                }
            }
            text += "</body>" + "</html>";
                if (px2)
                {
                    Data.DocumentText = text;
                }
                else
                {
                    webBrowser1.DocumentText = text;
                }
            db = true;
            if (fast)
            {
                Recreate(true);
            }
        }

        private void refresh_Tick(object sender, EventArgs e)
        {
            if (Modified)
            {
                Reload();
            }
            Recreate();
            
        }

        private void Recreate(bool enforce = false)
        {
            if (db)
            {
                if (px1)
                {
                    if (enforce || (!webBrowser1.IsBusy && webBrowser1.ReadyState == WebBrowserReadyState.Complete))
                    {
                        db = false;
                        pwx2.BringToFront();
                        px1 = false;
                        px2 = true;
                    }
                }
                else
                {
                    if (enforce || (Data.ReadyState == WebBrowserReadyState.Complete && !Data.IsBusy))
                    {
                        db = false;
                        pwx1.BringToFront();
                        px2 = false;
                        px1 = true;
                    }
                }
            }
        }


        private void channelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Core.network != null)
            {
                if (Core.network.RenderedChannel != null)
                {
                    Channel_Info info = new Channel_Info();
                    info.channel = Core.network.RenderedChannel;
                    info.Show();
                }
            }
        }

        private void mrhToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void scrollToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload(true);
        }

    }
}
