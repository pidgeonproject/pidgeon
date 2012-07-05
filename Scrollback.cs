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
using System.Data;
using System.Drawing;
using System.Windows.Forms;



namespace Client
{
    public partial class Scrollback : UserControl
    {
        private List<ContentLine> Line = new List<ContentLine>();
        public TextBox Last;
        public Window owner;
        public TreeNode ln;
        public class ContentLine
        {
            public ContentLine(MessageStyle _style, string Text)
            {
                style = _style;
                time = DateTime.Now;
                text = Text;
            }
            public DateTime time;
            public string text;
            public MessageStyle style;
        }
        private bool db = false;
        public bool Modified;

        public void create()
        {
            InitializeComponent();
            Data.Visible = false;
        }

        public void Recreate(object sender, EventArgs e)
        {

        }


        public Scrollback()
        {
            
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

        public bool InsertText(string text, MessageStyle _style, bool lg = true)
        {
            if (owner != Core._Main.Chat && ln != null)
            {
                switch (_style)
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
            lock(Line)
            {
                Line.Add(new ContentLine(_style, text));
            }
            if (lg == true)
            {
                
            }
            Modified = true;
            return false;
        }

        private void Scrollback_Load(object sender, EventArgs e)
        {
            Reload();
            Recreate(null, null);
        }

        public void Reload()
        {
            Modified = false;
            string text = "<html><head><script type=\"text/javascript\">function scroll() {window.scrollBy(0,100000);} </script> </head><body onLoad=\"scroll()\" STYLE=\"background-color: " + Configuration.CurrentSkin.backgroundcolor.Name + "\">";
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
            if (webBrowser1.Visible)
            {
                Data.DocumentText = text;
            }
            else
            {
                webBrowser1.DocumentText = text;
            }
            db = true;
        }

        private void refresh_Tick(object sender, EventArgs e)
        {
            if (db)
            {
                if (webBrowser1.Visible)
                {
                    if (Data.ReadyState == WebBrowserReadyState.Complete)
                    {
                        db = false;
                        Data.Visible = true;
                        webBrowser1.Visible = false;
                    }
                }
                else
                {
                    if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
                    {
                        db = false;
                        webBrowser1.Visible = true;
                        Data.Visible = false;
                    }
                }
            }
            if (Modified)
            {
                Reload();
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

    }
}
