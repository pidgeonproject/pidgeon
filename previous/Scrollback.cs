﻿/***************************************************************************
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
using System.Linq;



namespace Client
{
    public partial class Scrollback : UserControl
    {
        private bool db = false;
        public bool Modified;
        private int scrollback_max = Configuration.scrollback_plimit;
        private bool isDisposing;
        private List<ContentLine> Line = new List<ContentLine>();
        public TextBox Last;
        public static List<Scrollback> _control = new List<Scrollback>();
        public Window owner;
        public bool show = true;
        public bool px1 = true;
        public bool px2 = false;
        private string Link = "";


        public bool simple = false;

        public class ContentLine : IComparable
        {
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
            public DateTime time;
            public string text;
            public bool notice = false;
            public MessageStyle style;
        }


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
            base.Dispose(disposing);
        }

        public void create()
        {
            InitializeComponent();

            refresh.Enabled = true;
            simpleview.Visible = false;
            Scrollback_Load(null, null);
            Data.DocumentText = "<html><head> </head><body onLoad=\"scroll()\" STYLE=\"background-color: " + Configuration.CurrentSkin.backgroundcolor.Name + "\"></body></html>";
            webBrowser1.DocumentText = "<html><head> </head><body onLoad=\"scroll()\" STYLE=\"background-color: " + Configuration.CurrentSkin.backgroundcolor.Name + "\"></body></html>";

            Recreate(true);
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

        public void Switch(bool advanced)
        {
            Modified = true;
            if (advanced)
            {
                if (simple)
                {
                    simple = false;
                    pwx1.Visible = true;
                    simpleview.Visible = false;
                    pwx2.Visible = true;
                    Recreate(true);
                    return;
                }
                return;
            }
            simple = true;
            pwx1.Visible = false;
            simpleview.Visible = true;
            Recreate(true);
            pwx2.Visible = false;
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

        public bool Match(string text)
        {
            string matchline = "";
            foreach (Network.Highlighter item in Configuration.HighlighterList)
            {
                if (item.enabled)
                {
                    if (owner != null && owner._Network != null)
                    {
                        matchline = item.text.Replace("$nick", owner._Network.nickname).Replace("$ident", owner._Network.ident).Replace("$name", Configuration.user);
                    }
                    else
                    {
                        matchline = item.text.Replace("$nick", Configuration.nick).Replace("$ident", Configuration.ident).Replace("$name", Configuration.user);
                    }
                    if (item.simple)
                    {
                        if (text.Contains(matchline))
                        {
                            return true;
                        }
                        continue;
                    }
                    if (owner != null && owner._Network != null)
                    {
                        matchline = item.text.Replace("$nick", System.Text.RegularExpressions.Regex.Escape(owner._Network.nickname)).Replace("$ident",
                            System.Text.RegularExpressions.Regex.Escape(owner._Network.ident)).Replace("$name",
                            System.Text.RegularExpressions.Regex.Escape(Configuration.user));
                    }
                    else
                    {
                        matchline = item.text.Replace("$nick", System.Text.RegularExpressions.Regex.Escape(Configuration.nick)).Replace("$ident",
                            System.Text.RegularExpressions.Regex.Escape(Configuration.ident)).Replace("$name",
                            System.Text.RegularExpressions.Regex.Escape(Configuration.user));
                    }
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(matchline);
                    if (regex.IsMatch(text))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Insert a text to scrollback list
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="input_style">Style</param>
        /// <param name="lg">Write to a log</param>
        /// <param name="dt">Date</param>
        /// <param name="nh">Suppress highlight</param>
        /// <returns></returns>
        public bool InsertText(string text, MessageStyle input_style, bool lg = true, long dt = 0, bool nh = false)
        {
            if (owner != null && owner.MicroBox)
            {
                MicroChat.mc.scrollback_mc.InsertText("{" + owner.name + "} " + text, input_style, false, dt);
            }

            bool Matched = false;
            if (!nh)
            {
                Matched = Match(text);
            }

            if (Matched && owner != null)
            {
                Core.DisplayNote(text, owner.name);
            }

            if (owner != null && owner != Core._Main.Chat && owner.ln != null)
            {
                switch (input_style)
                {
                    case MessageStyle.Kick:
                    case MessageStyle.System:
                        owner.ln.ForeColor = Configuration.CurrentSkin.highlightcolor;
                        break;
                    case MessageStyle.Message:
                        if (owner.ln.ForeColor != Configuration.CurrentSkin.highlightcolor)
                        {
                            owner.ln.ForeColor = Configuration.CurrentSkin.colortalk;
                        }
                        break;
                    case MessageStyle.Action:
                        if (owner.ln.ForeColor != Configuration.CurrentSkin.colortalk && owner.ln.ForeColor != Configuration.CurrentSkin.highlightcolor)
                        {
                            owner.ln.ForeColor = Configuration.CurrentSkin.miscelancscolor;
                        }
                        break;
                    case MessageStyle.Part:
                    case MessageStyle.Channel:
                    case MessageStyle.User:
                    case MessageStyle.Join:
                        if (owner.ln.ForeColor != Configuration.CurrentSkin.highlightcolor && owner.ln.ForeColor != Configuration.CurrentSkin.miscelancscolor && owner.ln.ForeColor != Configuration.CurrentSkin.colortalk)
                        {
                            owner.ln.ForeColor = Configuration.CurrentSkin.joincolor;
                        }
                        break;

                }

                if (Matched)
                {
                    owner.ln.ForeColor = Configuration.CurrentSkin.highlightcolor;
                }
            }
            DateTime time = DateTime.Now;
            if (dt != 0)
            {
                time = DateTime.FromBinary(dt);
            }
            lock (Line)
            {
                Line.Add(new ContentLine(input_style, text, time, Matched));
                if (dt != 0)
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
                    System.IO.File.AppendAllText(_getFileName() + ".html", "<font size=\"" + Configuration.CurrentSkin.fontsize.ToString() + "px\" face=" + Configuration.CurrentSkin.localfont + ">" + stamp + System.Web.HttpUtility.HtmlEncode(text) + "</font><br>\n");
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
            simpleview.BackColor = Configuration.CurrentSkin.backgroundcolor;
            simpleview.ForeColor = Configuration.CurrentSkin.fontcolor;
            pwx2.Dock = DockStyle.Fill;
            HideLn();
            pwx1.BringToFront();
            pwx1.Dock = DockStyle.Fill;
        }

        public void Reload(bool fast = false)
        {
            if (owner == null || (owner != null && owner.Visible == true))
            {
                Modified = false;
                if (simple)
                {
                    lock (Line)
                    {
                        List<string> values = new List<string>();
                        foreach (ContentLine _line in Line)
                        {
                            values.Add(Configuration.format_date.Replace("$1", _line.time.ToString(Configuration.timestamp_mask)) + _line.text.Replace("%/L%", "").Replace("%/USER%", "").Replace("%L%", "").Replace("%USER%", ""));
                        }
                        simpleview.Lines = values.ToArray<string>();
                        simpleview.SelectionStart = simpleview.Text.Length;
                        simpleview.ScrollToCaret();
                    }
                    return;
                }
                Modified = false;
                string text = "<html><head><script type=\"text/javascript\">function scroll() {window.scrollBy(0," + Line.Count.ToString() + "00);} </script> </head><body onLoad=\"scroll()\" alink=\"" + Configuration.CurrentSkin.link.Name + "\" link=\"" + Configuration.CurrentSkin.link.Name + "\" vlink=\"" + Configuration.CurrentSkin.link.Name + "\" STYLE=\" background-color: " + Configuration.CurrentSkin.backgroundcolor.Name + "\">";
                int min = 0;
                if (scrollback_max != 0 && scrollback_max < Line.Count)
                {
                    min = Line.Count - scrollback_max;
                }
                lock (Line)
                {
                    int max = Line.Count;
                    int current = min;
                    while (current < max)
                    {
                        //foreach (ContentLine _c in Line)
                        //{
                        ContentLine _c = Line[current];
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
                        if (_c.notice)
                        {
                            color = Configuration.CurrentSkin.highlightcolor.Name;
                        }
                        current++;
                        string stamp = "";
                        if (Configuration.chat_timestamp)
                        {
                            stamp = Configuration.format_date.Replace("$1", _c.time.ToString(Configuration.timestamp_mask));
                        }
                        text += "<font style=\"" + Configuration.CurrentSkin.localfont + "\" size=\"" + Configuration.CurrentSkin.fontsize.ToString() + "px\" color=\"" + color + "\">" + stamp + Protocol.decrypt_text(Parser.link(System.Web.HttpUtility.HtmlEncode(_c.text))) + "</font><br>";
                    }
                }
                text += "</body>" + "</html>";
                if (px2)
                {
                    foreach (HtmlElement html in Data.Document.All)
                    {
                        html.MouseDown -= new HtmlElementEventHandler(Scrollback_Clicked);
                    }
                    Data.DocumentText = text;
                }
                else
                {
                    foreach (HtmlElement html in webBrowser1.Document.All)
                    {
                        html.MouseDown -= new HtmlElementEventHandler(Scrollback_Clicked);
                    }
                    webBrowser1.DocumentText = text;
                }
                db = true;
                if (fast)
                {
                    Recreate(true);
                }
            }
        }

        private void refresh_Tick(object sender, EventArgs e)
        {
            if (!scrollToolStripMenuItem.Checked)
            { return; }
            if (Modified)
            {
                Reload();
            }
            Recreate();

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

        public void Scrollback_Clicked(object sender, HtmlElementEventArgs e)
        {
            HtmlElement html = (HtmlElement)sender;
            if (e.MouseButtonsPressed == System.Windows.Forms.MouseButtons.Right)
            {
                if (html.OuterHtml.StartsWith("<A href=\"pidgeon://text"))
                {
                    ViewLn(html.InnerText, false, true);
                }
                if (html.OuterHtml.StartsWith("<A href=\"pidgeon://join"))
                {
                    ViewLn(html.InnerHtml, true, false);
                }
                if (html.OuterHtml.StartsWith("<A href=\"pidgeon://user"))
                {
                    ViewLn(html.InnerText + "!*@*", false, false);
                }
            }
        }

        public void ViewLn(string content, bool channel, bool link = true)
        {
            if (owner != null && owner.isChannel)
            {
                toolStripMenuItem1.Visible = true;
                Link = content;
                if (channel)
                {
                    mode1b2ToolStripMenuItem.Visible = false;
                    openLinkInBrowserToolStripMenuItem.Visible = false;
                    mode1e2ToolStripMenuItem.Visible = false;
                    mode1I2ToolStripMenuItem.Visible = false;
                    mode1q2ToolStripMenuItem.Visible = false;
                    joinToolStripMenuItem.Visible = true;
                    return;
                }
                mode1b2ToolStripMenuItem.Visible = true;
                joinToolStripMenuItem.Visible = false;
                toolStripMenuItem1.Visible = true;
                mode1q2ToolStripMenuItem.Text = "/mode " + owner.name + " +q " + content;
                mode1I2ToolStripMenuItem.Text = "/mode " + owner.name + " +I " + content;
                mode1e2ToolStripMenuItem.Text = "/mode " + owner.name + " +e " + content;
                mode1b2ToolStripMenuItem.Text = "/mode " + owner.name + " +b " + content;
                if (link)
                {
                    openLinkInBrowserToolStripMenuItem.Visible = true;
                }
                if (link == false)
                {
                    openLinkInBrowserToolStripMenuItem.Visible = false;
                }
                mode1e2ToolStripMenuItem.Visible = true;
                mode1I2ToolStripMenuItem.Visible = true;
                mode1q2ToolStripMenuItem.Visible = true;
            }
        }

        private void Clicked(object sender, WebBrowserNavigatingEventArgs navigating)
        {
            if (navigating.Url.ToString().Contains("https://"))
            {
                System.Diagnostics.Process.Start(navigating.Url.ToString());
                navigating.Cancel = true;
            }
            if (navigating.Url.ToString().Contains("http://"))
            {
                System.Diagnostics.Process.Start(navigating.Url.ToString());
                navigating.Cancel = true;
            }
            if (navigating.Url.ToString().Contains("pidgeon://"))
            {
                navigating.Cancel = true;
                string command = navigating.Url.ToString().Substring("pidgeon://".Length);
                if (command.EndsWith("/"))
                {
                    command = command.Substring(0, command.Length - 1);
                }
                if (command.StartsWith("user/#"))
                {
                    string nick = command.Substring(6);
                    if (owner != null && owner._Network != null)
                    {
                        if (owner.isChannel)
                        {
                            owner.textbox.richTextBox1.AppendText(nick + ": ");
                            owner.textbox.Focus();
                        }
                    }
                    return;
                }
                if (command.StartsWith("join/#"))
                {
                    Parser.parse("/join " + command.Substring(5));
                    return;
                }
                if (command.StartsWith("text/#"))
                {
                    string nick = command.Substring(6);
                    if (owner != null && owner._Network != null)
                    {
                        if (owner.isChannel)
                        {
                            owner.textbox.richTextBox1.AppendText(nick);
                            owner.textbox.Focus();
                        }
                    }
                    return;
                }
                if (command.StartsWith("join#"))
                {
                    Parser.parse("/join " + command.Substring(4));
                    return;
                }
            }
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
                        lock (webBrowser1.Document.All)
                        {
                            foreach (HtmlElement html in webBrowser1.Document.All)
                            {
                                html.MouseDown += new HtmlElementEventHandler(Scrollback_Clicked);
                            }
                        }
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
                        lock (Data.Document.All)
                        {
                            foreach (HtmlElement html in Data.Document.All)
                            {
                                html.MouseDown += new HtmlElementEventHandler(Scrollback_Clicked);
                            }
                        }
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
            if (MessageBox.Show("Do you really want to clear this window", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                lock (Line)
                {
                    Line.Clear();
                    Recreate(true);
                }
            }
        }

        private void scrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scrollToolStripMenuItem.Checked = !scrollToolStripMenuItem.Checked;
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

        private void retrieveTopicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (owner.isChannel)
            {
                owner._Protocol.Transfer("TOPIC " + owner.name);
            }
        }

        private void toggleSimpleLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Switch(false);
        }

        private void toggleAdvancedLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Switch(true);
        }

        private void mode1b2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (owner != null)
            {
                owner.textbox.richTextBox1.AppendText(mode1b2ToolStripMenuItem.Text);
            }
        }

        private void mode1q2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (owner != null)
            {
                owner.textbox.richTextBox1.AppendText(mode1q2ToolStripMenuItem.Text);
            }
        }

        private void mode1I2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (owner != null)
            {
                owner.textbox.richTextBox1.AppendText(mode1I2ToolStripMenuItem.Text);
            }
        }

        private void mode1e2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (owner != null)
            {
                owner.textbox.richTextBox1.AppendText(mode1e2ToolStripMenuItem.Text);
            }
        }

        private void openLinkInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Link.StartsWith("http"))
            {
                System.Diagnostics.Process.Start(Link);
            }
        }

        private void joinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Link.StartsWith("#"))
            {
                Parser.parse("/join " + Link);
            }
        }

    }
}