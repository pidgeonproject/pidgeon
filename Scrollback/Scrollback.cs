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
        private static List<Scrollback> ScrollbackList = new List<Scrollback>();
        /// <summary>
        /// Maximal size of text
        /// </summary>
        private int scrollback_max = Configuration.scrollback_plimit;
        /// <summary>
        /// List of all lines in this textbox
        /// </summary>
        private List<ContentLine> ContentLines = new List<ContentLine>();
        public TextBox Last = null;
        /// <summary>
        /// Owner window
        /// </summary>
        public Window owner = null;
        private List<ContentLine> UndrawnLines = new List<ContentLine>();
        private string Link = "";
        public bool simple = false;
        private DateTime lastDate;
        private bool ScrollingEnabled = true;
        private bool ReloadWaiting = false;

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
            lock (ScrollbackList)
            {
                if (ScrollbackList.Contains(this))
                {
                    ScrollbackList.Remove(this);
                }
            }
            base.Dispose(disposing);
        }

        public bool WindowVisible()
        {
            if (this.Visible == false)
            {
                return false;
            }
            if (owner != null)
            {
                if (owner.Visible != true)
                {
                    return false;
                }
            }
            return true;
        }

        public void Switch(bool advanced)
        {
            if (advanced)
            {
                if (simple)
                {
                    simple = false;
                    RT.Visible = true;
                    simpleview.Visible = false;
                    Recreate(true);
                    return;
                }
                return;
            }
            simple = true;
            simpleview.Visible = true;
            RT.Visible = false;
            Reload();
        }

        /// <summary>
        /// Creates a new scrollback instance
        /// </summary>
        public Scrollback()
        {
            lock (ScrollbackList)
            {
                ScrollbackList.Add(this);
            }
            this.BackColor = Configuration.CurrentSkin.backgroundcolor;

            ReloadWaiting = true;
        }

        public void Create()
        {
            InitializeComponent();

            simpleview.Visible = false;
        }

        public string validpath(string text)
        {
            return text.Replace("?", "1_").Replace("|", "2_").Replace(":", "3_").Replace("\\", "4_");
        }

        public bool Find(string text)
        {
            if (simple)
            {
                if (!simpleview.Text.Contains(text))
                {
                    return false;
                }
                simpleview.SelectionStart = simpleview.Text.IndexOf(text);
                simpleview.SelectionLength = text.Length;
                simpleview.ScrollToCaret();
                return true;
            }
            return false;
        }

        public string _getFileName()
        {
            string name = Configuration.logs_dir + Path.DirectorySeparatorChar + owner._Network.server + Path.DirectorySeparatorChar + owner.name + Path.DirectorySeparatorChar + DateTime.Now.ToString(Configuration.logs_name).Replace("$1", validpath(owner.name));
            return name;
        }

        private void Scrollback_Load(object sender, EventArgs e)
        {
            try
            {
                RT.Spacing = Configuration.CurrentSkin.SBABOX_sp;
                RT.BackColor = Configuration.CurrentSkin.backgroundcolor;
                RT.Font = Configuration.CurrentSkin.SBABOX;
                simpleview.BackColor = Configuration.CurrentSkin.backgroundcolor;
                simpleview.ForeColor = Configuration.CurrentSkin.fontcolor;
                RT.scrollback = this;
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
                List<string> values = new List<string>();
                foreach (ContentLine _line in ContentLines)
                {
                    values.Add(Configuration.format_date.Replace("$1", _line.time.ToString(Configuration.timestamp_mask)) + Core.RemoveSpecial(_line.text));
                }
                simpleview.Lines = values.ToArray<string>();
                simpleview.SelectionStart = simpleview.Text.Length;
                simpleview.ScrollToCaret();
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
        /// Redraw a text
        /// </summary>
        /// <param name="enforce">Deprecated</param>
        private void Recreate(bool enforce = false)
        {
            if (!simple)
            {
                RT.RedrawText();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                timer2.Enabled = false;
                if (RT != null && WindowVisible())
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
                                RT.RedrawText();
                                if (ScrollingEnabled)
                                {
                                    RT.ScrollToBottom();
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
                timer2.Enabled = true;
            }
            catch (Exception fail)
            {
                timer2.Enabled = true;
                Core.handleException(fail);
            }
        }
    }
}