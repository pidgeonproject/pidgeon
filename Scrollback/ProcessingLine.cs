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
    public partial class Scrollback
    {
        public void Display()
        {
            if (!this.Visible)
            {
                this.Visible = true;
            }
        }

        private void InsertLineToText(ContentLine line, bool Draw = true)
        {
            if (simple)
            {
                if (!ScrollingEnabled)
                {
                    Changed = true;
                    return;
                }
                //lock (simpleview.Lines)
                {
                    //simpleview.AppendText(Configuration.Scrollback.format_date.Replace("$1", line.time.ToString(Configuration.Scrollback.timestamp_mask)) + Core.RemoveSpecial(line.text) + Environment.NewLine);
                }
                return;
            }
            lock (ContentLines)
            {
                if (ContentLines.Count > scrollback_max)
                {
                    //RT.RemoveLine(0);
                }
            }
            //RT.InsertLine(CreateLine(line));
            if (Draw)
            {
                //RT.RedrawText();
                if (ScrollingEnabled)
                {
                    //RT.ScrollToBottom();
                }
            }

            Changed = false;
        }

        private SBABox.Line CreateLine(ContentLine Line)
        {
            ContentLine _c = Line;
            Color color = Configuration.CurrentSkin.fontcolor;
            switch (_c.style)
            {
                case MessageStyle.Action:
                    color = Configuration.CurrentSkin.miscelancscolor;
                    break;
                case MessageStyle.Kick:
                    color = Configuration.CurrentSkin.kickcolor;
                    break;
                case MessageStyle.System:
                    color = Configuration.CurrentSkin.miscelancscolor;
                    break;
                case MessageStyle.Channel:
                    color = Configuration.CurrentSkin.colortalk;
                    break;
                case MessageStyle.User:
                    color = Configuration.CurrentSkin.changenickcolor;
                    break;
                case MessageStyle.Join:
                case MessageStyle.Part:
                    color = Configuration.CurrentSkin.joincolor;
                    break;
            }
            if (_c.notice)
            {
                color = Configuration.CurrentSkin.highlightcolor;
            }

            string stamp = "";
            if (Configuration.Scrollback.chat_timestamp)
            {
                stamp = Configuration.Scrollback.format_date.Replace("$1", _c.time.ToString(Configuration.Scrollback.timestamp_mask));
            }
            //SBABox.Line text = Parser.FormatLine(_c.text, RT, color);
            //SBABox.ContentText content = new SBABox.ContentText(stamp, RT, color);
            //SBABox.Line line = new SBABox.Line(RT);
            //line.insertData(content);
            //line.Merge(text);
            //return line;
			return null;
        }

        public bool Reload(bool fast = false, bool enforce = false)
        {
            if (!enforce && !Changed)
            {
                return false;
            }

            Changed = false;

            if (owner == null || (owner != null && WindowVisible()))
            {
                if (SortNeeded)
                {
                    ContentLines.Sort();
                    SortNeeded = false;
                }

                if (simple)
                {
                    ReloadSimple();
                    return true;
                }

                int min = 0;

                if (scrollback_max != 0 && scrollback_max < ContentLines.Count)
                {
                    min = ContentLines.Count - scrollback_max;
                }

                //RT.RemoveText();

                if (ContentLines.Count > 0)
                {
                    lock (ContentLines)
                    {
                        int max = ContentLines.Count;
                        int current = min;
                        while (current < max)
                        {
                            //RT.InsertLine(CreateLine(ContentLines[current]));
                            current++;
                        }
                        lastDate = ContentLines[current - 1].time;
                    }
                }

                //RT.RedrawText();
                if (ScrollingEnabled)
                {
                    //RT.ScrollToBottom();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if this line needs to display notification window
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool Match(string text)
        {
            string matchline = "";
            foreach (Network.Highlighter item in Configuration.HighlighterList)
            {
                if (item.enabled)
                {
                    if (owner != null && owner._Network != null)
                    {
                        matchline = item.text.Replace("$nick", owner._Network.Nickname).Replace("$ident", owner._Network.Ident).Replace("$name", Configuration.UserData.user);
                    }
                    else
                    {
                        matchline = item.text.Replace("$nick", Configuration.UserData.nick).Replace("$ident", Configuration.UserData.ident).Replace("$name", Configuration.UserData.user);
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
                        matchline = item.text.Replace("$nick", System.Text.RegularExpressions.Regex.Escape(owner._Network.Nickname)).Replace("$ident",
                            System.Text.RegularExpressions.Regex.Escape(owner._Network.Ident)).Replace("$name",
                            System.Text.RegularExpressions.Regex.Escape(Configuration.UserData.user));
                    }
                    else
                    {
                        matchline = item.text.Replace("$nick", System.Text.RegularExpressions.Regex.Escape(Configuration.UserData.nick)).Replace("$ident",
                            System.Text.RegularExpressions.Regex.Escape(Configuration.UserData.ident)).Replace("$name",
                            System.Text.RegularExpressions.Regex.Escape(Configuration.UserData.user));
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

        bool RequireReload(DateTime date)
        {
            if (date < lastDate)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Insert a text to scrollback list
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="InputStyle">Style</param>
        /// <param name="WriteLog">Write to a log</param>
        /// <param name="Date">Date</param>
        /// <param name="SuppressPing">Suppress highlight</param>
        /// <returns></returns>
        public bool InsertText(string text, MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false)
        {
            return insertText(text, InputStyle, WriteLog, Date, SuppressPing, false);
        }

        /// <summary>
        /// Insert a text to scrollback list
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="InputStyle">Style</param>
        /// <param name="WriteLog">Write to a log</param>
        /// <param name="Date">Date</param>
        /// <param name="SuppressPing">Suppress highlight</param>
        /// <returns></returns>
        public bool InsertTextAndIgnoreUpdate(string text, MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false)
        {
            return insertText(text, InputStyle, WriteLog, Date, SuppressPing, true);
        }

        /// <summary>
        /// Insert a text to scrollback list
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="InputStyle">Style</param>
        /// <param name="WriteLog">Write to a log</param>
        /// <param name="Date">Date</param>
        /// <param name="SuppressPing">Suppress highlight</param>
        /// <returns></returns>
        private bool insertText(string text, MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false, bool IgnoreUpdate = false)
        {
            // in case there are multiple lines we call this function for every line
            if (text.Contains('\n'))
            {
                string[] data = text.Split('\n');
                foreach (string Next in data)
                {
                    InsertText(Next, InputStyle, WriteLog, Date, SuppressPing);
                }
                return true;
            }

            if (owner != null && owner.MicroBox)
            {
                Forms.MicroChat.mc.scrollback_mc.InsertText("{" + owner.name + "} " + text, InputStyle, false, Date);
            }

            bool Matched = false;

            if (!SuppressPing)
            {
                Matched = Match(text);
            }

            if (Matched && owner != null)
            {
                Core.DisplayNote(text, owner.name);
            }

            /* if (!IgnoreUpdate && owner != null && owner != Core._Main.Chat && owner._Protocol != null && !owner._Protocol.SuppressChanges && owner.treeNode != null)
            {
                switch (InputStyle)
                {
                    case MessageStyle.Kick:
                    case MessageStyle.System:
                        owner.treeNode.ForeColor = Configuration.CurrentSkin.highlightcolor;
                        break;
                    case MessageStyle.Message:
                        if (owner.treeNode.ForeColor != Configuration.CurrentSkin.highlightcolor)
                        {
                            owner.treeNode.ForeColor = Configuration.CurrentSkin.colortalk;
                        }
                        break;
                    case MessageStyle.Action:
                        if (owner.treeNode.ForeColor != Configuration.CurrentSkin.colortalk && owner.treeNode.ForeColor != Configuration.CurrentSkin.highlightcolor)
                        {
                            owner.treeNode.ForeColor = Configuration.CurrentSkin.miscelancscolor;
                        }
                        break;
                    case MessageStyle.Part:
                    case MessageStyle.Channel:
                    case MessageStyle.User:
                    case MessageStyle.Join:
                        if (owner.treeNode.ForeColor != Configuration.CurrentSkin.highlightcolor && owner.treeNode.ForeColor != Configuration.CurrentSkin.miscelancscolor && owner.treeNode.ForeColor != Configuration.CurrentSkin.colortalk)
                        {
                            owner.treeNode.ForeColor = Configuration.CurrentSkin.joincolor;
                        }
                        break;
                }

                if (Matched)
                {
                    owner.treeNode.ForeColor = Configuration.CurrentSkin.highlightcolor;
                }
            }
            */

            DateTime time = DateTime.Now;

            if (Date != 0)
            {
                time = DateTime.FromBinary(Date);
            }

            ContentLine line = new ContentLine(InputStyle, text, time, Matched);

            lock (ContentLines)
            {
                ContentLines.Add(line);
                if (Date != 0)
                {
                    SortNeeded = true;
                }
            }

            if (WriteLog == true && owner != null && owner._Network != null)
            {
                LineLogs.Log(text, InputStyle, owner, LogfilePath);
            }

            Changed = true;

            if (Thread.CurrentThread == Core._KernelThread && WindowVisible())
            {
                if (!RequireReload(time))
                {
                    InsertLineToText(line);
                    lastDate = time;
                }
                else
                {
                    Reload(false, true);
                }
            }
            else
            {
                if (!RequireReload(time))
                {
                    lock (UndrawnLines)
                    {
                        if (!UndrawnLines.Contains(line))
                        {
                            UndrawnLines.Add(line);
                        }
                    }
                    lastDate = time;
                }
                else
                {
                    ReloadWaiting = true;
                }
            }

            return false;
        }
    }
}
