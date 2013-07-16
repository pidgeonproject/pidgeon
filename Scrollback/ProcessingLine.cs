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
using System.Drawing;
using System.Text;
using System.Threading;
using System.Linq;
using System.IO;
using System.Data;

namespace Client
{
    public partial class Scrollback
    {
        /// <summary>
        /// This is a line for logs
        /// </summary>
        class LI
        {
            /// <summary>
            /// Text to be written to log file
            /// </summary>
            public string text;
            /// <summary>
            /// Style
            /// </summary>
            public ContentLine.MessageStyle style;
            /// <summary>
            /// Time when the log was supposed to be written
            /// </summary>
            public DateTime date;
        }

        /// <summary>
        /// This is a container for logs that couldn't be written yet, it exist because the window can be used before it's fully
        /// created and in that case the first logs are not logged but dropped, so in that case we hold them in here
        /// </summary>
        private List<LI> logs = new List<LI>();
        private void InsertLineToText(ContentLine line, bool Draw = true)
        {
            if (Configuration.Memory.EnableSimpleViewCache && simple)
            {
                if (!ScrollingEnabled)
                {
                    Changed = true;
                    return;
                }
                Gtk.TextIter iter = simpleview.Buffer.EndIter;
                simpleview.Buffer.Insert(ref iter, Configuration.Scrollback.format_date.Replace("$1",
                                          line.time.ToString(Configuration.Scrollback.timestamp_mask)) +
                                          Core.RemoveSpecial(line.text) + Environment.NewLine);
                if (ScrollingEnabled)
                {
                    simpleview.ScrollToIter(simpleview.Buffer.GetIterAtLine(ContentLines.Count), 0, true, 0, 0);
                }
                Changed = false;
                return;
            }
            lock (ContentLines)
            {
                if (ContentLines.Count > scrollback_max)
                {
                    RT.RemoveLine(0);
                }
            }
            RT.InsertLine(CreateLine(line));

            Changed = false;
        }

        private Client.RichTBox.ContentText CreateText(ContentLine line, string text)
        {
            Color color = Configuration.CurrentSkin.joincolor;
            if (line != null)
            {
                switch (line.style)
                {
                    case Client.ContentLine.MessageStyle.Action:
                        color = Configuration.CurrentSkin.miscelancscolor;
                        break;
                    case Client.ContentLine.MessageStyle.Kick:
                        color = Configuration.CurrentSkin.kickcolor;
                        break;
                    case Client.ContentLine.MessageStyle.System:
                        color = Configuration.CurrentSkin.miscelancscolor;
                        break;
                    case Client.ContentLine.MessageStyle.Channel:
                        color = Configuration.CurrentSkin.colortalk;
                        break;
                    case Client.ContentLine.MessageStyle.User:
                        color = Configuration.CurrentSkin.changenickcolor;
                        break;
                    case Client.ContentLine.MessageStyle.Join:
                    case Client.ContentLine.MessageStyle.Part:
                        color = Configuration.CurrentSkin.joincolor;
                        break;
                }

                if (line.notice)
                {
                    color = Configuration.CurrentSkin.highlightcolor;
                }
            }

            return new Client.RichTBox.ContentText(text, color);
        }

        private Client.RichTBox.Line CreateLine(ContentLine Line)
        {
            Color color = Configuration.CurrentSkin.fontcolor;
            switch (Line.style)
            {
                case Client.ContentLine.MessageStyle.Action:
                    color = Configuration.CurrentSkin.miscelancscolor;
                    break;
                case Client.ContentLine.MessageStyle.Kick:
                    color = Configuration.CurrentSkin.kickcolor;
                    break;
                case Client.ContentLine.MessageStyle.System:
                    color = Configuration.CurrentSkin.miscelancscolor;
                    break;
                case Client.ContentLine.MessageStyle.Channel:
                    color = Configuration.CurrentSkin.colortalk;
                    break;
                case Client.ContentLine.MessageStyle.User:
                    color = Configuration.CurrentSkin.changenickcolor;
                    break;
                case Client.ContentLine.MessageStyle.Join:
                case Client.ContentLine.MessageStyle.Part:
                    color = Configuration.CurrentSkin.joincolor;
                    break;
            }

            if (Line.notice)
            {
                color = Configuration.CurrentSkin.highlightcolor;
            }

            string stamp = "";
            if (Configuration.Scrollback.chat_timestamp)
            {
                stamp = Configuration.Scrollback.format_date.Replace("$1", Line.time.ToString(Configuration.Scrollback.timestamp_mask));
            }

            Client.RichTBox.Line text = Parser.FormatLine(Line.text, RT, color);
            Client.RichTBox.ContentText content = new Client.RichTBox.ContentText(stamp, color);
            Client.RichTBox.Line line = new Client.RichTBox.Line(RT);
            line.insertData(content);
            line.Merge(text);
            return line;
        }

        /// <summary>
        /// Reload all text
        /// </summary>
        /// <param name="enforce"></param>
        /// <returns></returns>
        public bool Reload(bool enforce = false)
        {
            if (IsDestroyed)
            {
                return false;
            }

            if (!enforce && !Changed)
            {
                return false;
            }

            Changed = false;

            Flush();

            lock (UndrawnLines)
            {
                UndrawnLines.Clear();
            }

            lock (UndrawnTextParts)
            {
                UndrawnTextParts.Clear();
            }

            if (owner == null || (owner != null && WindowVisible()) || !Configuration.Scrollback.DynamicReload)
            {
                if (Configuration.Memory.MaximumChannelBufferSize != 0)
                {
                    lock (ContentLines)
                    {
                        if (Configuration.Memory.MaximumChannelBufferSize <= ContentLines.Count)
                        {
                            ContentLines.Sort();
                            SortNeeded = false;
                            while (Configuration.Memory.MaximumChannelBufferSize <= ContentLines.Count)
                            {
                                ContentLines.RemoveAt(0);
                            }
                        }
                    }
                }
                if (SortNeeded)
                {
                    lock (ContentLines)
                    {
                        ContentLines.Sort();
                        SortNeeded = false;
                    }
                }

                if (simple && Configuration.Memory.EnableSimpleViewCache)
                {
                    ReloadSimple();
                    return true;
                }

                int min = 0;

                lock (ContentLines)
                {
                    if (scrollback_max != 0 && scrollback_max < ContentLines.Count)
                    {
                        min = ContentLines.Count - scrollback_max;
                    }

                    RT.RemoveText();

                    if (ContentLines.Count > 0)
                    {
                        int max = ContentLines.Count;
                        int current = min;
                        lastDate = ContentLines[max - 1].time;
                        while (current < max)
                        {
                            max--;
                            RT.InsertLineToStart(CreateLine(ContentLines[max]), false);
                        }
                    }
                }

                RT.RedrawText();
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

        private bool RequireReload(DateTime date)
        {
            if (date < lastDate)
            {
                return true;
            }
            return false;
        }

        private void InsertPartToText(string text)
        {
            if (Configuration.Memory.EnableSimpleViewCache && simple)
            {
                if (!ScrollingEnabled)
                {
                    Changed = true;
                    return;
                }
                Gtk.TextIter iter = simpleview.Buffer.EndIter;
                simpleview.Buffer.Insert(ref iter, Core.RemoveSpecial(text));
                if (ScrollingEnabled)
                {
                    simpleview.ScrollToIter(simpleview.Buffer.GetIterAtLine(ContentLines.Count), 0, true, 0, 0);
                }
                Changed = false;
                return;
            }
            RT.InsertPart(CreateText(EndingLine, text));
            Changed = false;
        }

        private void InsertPartToText(ContentLine line)
        {
            if (Configuration.Memory.EnableSimpleViewCache && simple)
            {
                if (!ScrollingEnabled)
                {
                    Changed = true;
                    return;
                }
                Gtk.TextIter iter = simpleview.Buffer.EndIter;
                simpleview.Buffer.Insert(ref iter, Configuration.Scrollback.format_date.Replace("$1",
                                          line.time.ToString(Configuration.Scrollback.timestamp_mask)) +
                                          Core.RemoveSpecial(line.text));
                if (ScrollingEnabled)
                {
                    simpleview.ScrollToIter(simpleview.Buffer.GetIterAtLine(ContentLines.Count), 0, true, 0, 0);
                }
                Changed = false;
                return;
            }
            RT.InsertPart(CreateLine(line));

            Changed = false;
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
        public bool InsertText(string text, Client.ContentLine.MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false)
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
        public bool InsertTextAndIgnoreUpdate(string text, Client.ContentLine.MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false)
        {
            return insertText(text, InputStyle, WriteLog, Date, SuppressPing, true);
        }

        private void Flush()
        {
            if (!IsEmtpy)
            {
                if (EndingLine != null)
                {
                    lock (ContentLines)
                    {
                        ContentLines.Add(EndingLine);
                    }
                }

                lock (UndrawnTextParts)
                {
                    UndrawnTextParts.Add(new TextPart(Environment.NewLine, ContentLine.MessageStyle.Join, DateTime.Now));
                }

                EndingLine = null;
            }
        }

        /// <summary>
        /// Insert part
        /// </summary>
        /// <param name="text">Part of text to insert</param>
        /// <param name="InputStyle">Style</param>
        /// <param name="WriteLog">Write this line to a log</param>
        /// <param name="Date">Date</param>
        /// <param name="SuppressPing">Suppress ping</param>
        /// <param name="IgnoreUpdate"></param>
        /// <returns></returns>
        [Experimental]
        public void InsertPart(string text, Client.ContentLine.MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false, bool IgnoreUpdate = false)
        {
            DateTime time = DateTime.Now;

            if (Date != 0)
            {
                time = DateTime.FromBinary(Date);
            }

            if (EndingLine == null)
            {
                EndingLine = new ContentLine(InputStyle, text, time, false);

                if (Thread.CurrentThread == Core._KernelThread && (!Configuration.Scrollback.DynamicReload || WindowVisible()))
                {
                    if (lastDate > time)
                    {
                        Flush();
                        SortNeeded = true;
                        Reload(true);
                    }
                    else
                    {
                        InsertPartToText(EndingLine);
                    }
                }
                else
                {
                    lock (UndrawnTextParts)
                    {
                        UndrawnTextParts.Add(new TextPart(Configuration.Scrollback.format_date.Replace("$1",
                                          time.ToString(Configuration.Scrollback.timestamp_mask)) + text, InputStyle, time));
                    }
                }
            }
            else
            {
                lock (EndingLine)
                {
                    EndingLine.text += text;
                }

                if (Thread.CurrentThread == Core._KernelThread && (!Configuration.Scrollback.DynamicReload || WindowVisible()))
                {
                    InsertPartToText(text);
                }
                else
                {
                    lock (UndrawnTextParts)
                    {
                        UndrawnTextParts.Add(new TextPart(text, InputStyle, time));
                    }
                }
            }
        }

        /// <summary>
        /// Insert a text to scrollback list
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="InputStyle">Style</param>
        /// <param name="WriteLog">Write to a log</param>
        /// <param name="Date">Date</param>
        /// <param name="SuppressPing">Suppress highlight</param>
        /// <param name="IgnoreUpdate"></param>
        /// <returns></returns>
        private bool insertText(string text, Client.ContentLine.MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false, bool IgnoreUpdate = false)
        {
            // we need to finish the previous partial line
            if (!IsEmtpy)
            {
                Flush();
            }
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
                Core.SystemForm.micro.scrollback_mc.InsertText("{" + owner.WindowName + "} " + text, InputStyle, false, Date);
            }

            bool Matched = false;

            if (!SuppressPing)
            {
                Matched = Match(text);
            }

            if (Matched && owner != null && owner.Highlights)
            {
                if (Hooks._Scrollback.NotificationDisplay(text, InputStyle, ref WriteLog, Date, ref SuppressPing))
                {
                    Core.DisplayNote(text, owner.WindowName);
                }
            }

            if (!IgnoreUpdate && owner != null && owner != Core.SystemForm.Chat && owner._Network != null && owner._Network._Protocol != null && !owner._Network._Protocol.SuppressChanges)
            {
                switch (InputStyle)
                {
                    case ContentLine.MessageStyle.Kick:
                    case ContentLine.MessageStyle.System:
                        owner.MenuColor = Configuration.CurrentSkin.highlightcolor;
                        break;
                    case ContentLine.MessageStyle.Message:
                        if (owner.MenuColor != Configuration.CurrentSkin.highlightcolor)
                        {
                            owner.MenuColor = Configuration.CurrentSkin.colortalk;
                        }
                        break;
                    case ContentLine.MessageStyle.Action:
                        if (owner.MenuColor != Configuration.CurrentSkin.colortalk && owner.MenuColor != Configuration.CurrentSkin.highlightcolor)
                        {
                            owner.MenuColor = Configuration.CurrentSkin.miscelancscolor;
                        }
                        break;
                    case ContentLine.MessageStyle.Part:
                    case ContentLine.MessageStyle.Channel:
                    case ContentLine.MessageStyle.User:
                    case ContentLine.MessageStyle.Join:
                        if (owner.MenuColor != Configuration.CurrentSkin.highlightcolor && owner.MenuColor != Configuration.CurrentSkin.miscelancscolor && owner.MenuColor != Configuration.CurrentSkin.colortalk)
                        {
                            owner.MenuColor = Configuration.CurrentSkin.joincolor;
                        }
                        break;
                }

                Graphics.PidgeonList.Updated = true;

                if (Matched)
                {
                    owner.MenuColor = Configuration.CurrentSkin.highlightcolor;
                }
            }

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

            if (WriteLog == true)
            {
                if (owner != null && owner._Network != null)
                {
                    if (logs.Count > 0)
                    {
                        lock (logs)
                        {
                            foreach (LI item in logs)
                            {
                                LineLogs.Log(item.text, item.style, owner, LogfilePath, item.date);
                            }
                            logs.Clear();
                        }
                    }
                    LineLogs.Log(text, InputStyle, owner, LogfilePath, time);
                }
                else
                {
                    LI item = new LI();
                    item.text = text;
                    item.style = InputStyle;
                    item.date = time;
                    lock (logs)
                    {
                        if (logs.Count > 100)
                        {
                            Core.DebugLog("Buffer overflow: more than 100 items waiting to be written to log: auto flushing");
                            logs.Clear();
                        }
                        logs.Add(item);
                    }
                }
            }

            Changed = true;

            if (Thread.CurrentThread == Core._KernelThread && (!Configuration.Scrollback.DynamicReload || WindowVisible()))
            {
                if (!RequireReload(time))
                {
                    InsertLineToText(line);
                    lastDate = time;
                }
                else
                {
                    SortNeeded = true;
                    Reload(true);
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
                    SortNeeded = true;
                    ReloadWaiting = true;
                }
            }
            return false;
        }
    }
}
