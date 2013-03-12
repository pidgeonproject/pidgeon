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
using System.Linq;


namespace Client
{
    public partial class Scrollback : UserControl
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
        public Window owner = null;
        private List<ContentLine> UndrawnLines = new List<ContentLine>();
        private string Link = "";
        public bool simple = false;
        private DateTime lastDate;
        private bool ScrollingEnabled = true;
        private bool ReloadWaiting = false;
        private bool Changed = false;
        private string LogfilePath = null;
        public List<ContentLine> Data
        {
            get
            {
                List<ContentLine> data = new List<ContentLine>();
                lock (ContentLines)
                {
                    data.AddRange(ContentLines);
                }
                lock (UndrawnLines)
                {
                    data.AddRange(UndrawnLines);
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

        protected override void Dispose(bool disposing)
        {
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
                    RT.Visible = true;
                    simpleview.Visible = false;
                    toggleAdvancedLayoutToolStripMenuItem.Checked = true;
                    toggleSimpleLayoutToolStripMenuItem.Checked = false;
                    Recreate(true);
                    return;
                }
                Reload(true, true);
                return;
            }
            toggleAdvancedLayoutToolStripMenuItem.Checked = false;
            toggleSimpleLayoutToolStripMenuItem.Checked = true;
            simple = true;
            simpleview.Visible = true;
            RT.Visible = false;
            Reload(true, true);
        }

        /// <summary>
        /// Creates a new scrollback instance
        /// </summary>
        public Scrollback()
        {
            this.BackColor = Configuration.CurrentSkin.backgroundcolor;

            ReloadWaiting = true;
        }

        public Scrollback(Window _ParentWindow)
        {
            this.BackColor = Configuration.CurrentSkin.backgroundcolor;
            this.owner = _ParentWindow;

            ReloadWaiting = true;
        }

        public void Create()
        {
            InitializeComponent();
            simpleview.Visible = false;
            toggleAdvancedLayoutToolStripMenuItem.Checked = true;
        }

        /// <summary>
        /// Return a file path without special symbols not supported in windows or linux
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string validpath()
        {
            if (LogfilePath != null)
            {
                return LogfilePath;
            }
            if (owner == null)
            {
                throw new Exception("You can't enable logging for a window that has no parent");
            }
            LogfilePath = owner.name.Replace("?", "1_").Replace("|", "2_").Replace(":", "3_").Replace("\\", "4_").Replace("/", "5_").Replace("*", "6_");
            return LogfilePath;
        }

        public string _getFileName()
        {
            string name = Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + owner._Network.server + Path.DirectorySeparatorChar + owner.name + Path.DirectorySeparatorChar + DateTime.Now.ToString(Configuration.Logs.logs_name).Replace("$1", validpath());
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
                simpleview.Text = "";
                StringBuilder everything = new StringBuilder("");
                foreach (ContentLine _line in ContentLines)
                {
                    everything.Append(Configuration.Scrollback.format_date.Replace("$1", _line.time.ToString(Configuration.Scrollback.timestamp_mask)) + Core.RemoveSpecial(_line.text) + Environment.NewLine);
                }
                simpleview.AppendText(everything.ToString());
                if (ScrollingEnabled)
                {
                    simpleview.ScrollToCaret();
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

        /// <summary>
        /// Redraw a text
        /// </summary>
        /// <param name="enforce">Deprecated</param>
        private void Recreate(bool enforce = false)
        {
            if (!simple)
            {
                //RT.RedrawText();
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
