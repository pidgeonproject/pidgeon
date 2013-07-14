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

namespace Client
{
    /// <summary>
    /// TextPart
    /// </summary>
    public class TextPart
    {
        /// <summary>
        /// Text
        /// </summary>
        public string text;
        /// <summary>
        /// Date
        /// </summary>
        public DateTime date;
        /// <summary>
        /// Style
        /// </summary>
        public ContentLine.MessageStyle style;

        /// <summary>
        /// Creates a new instance of text part
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="ms"></param>
        /// <param name="time"></param>
        public TextPart(string Text, ContentLine.MessageStyle ms, DateTime time)
        {
            date = time;
            text = Text;
            style = ms;
        }
    }

    /// <summary>
    /// This is a line in scrollback
    /// </summary>
    [Serializable]
    public class ContentLine : IComparable
    {
        /// <summary>
        /// Time
        /// </summary>
        public DateTime time;
        /// <summary>
        /// Text
        /// </summary>
        public string text;
        /// <summary>
        /// Whether this message should trigger alert
        /// </summary>
        public bool notice = false;
        /// <summary>
        /// Style
        /// </summary>
        public MessageStyle style;

        /// <summary>
        /// This needs to exist for xml serialization
        /// </summary>
        public ContentLine()
        {
            text = null;
            style = MessageStyle.Channel;
        }

        /// <summary>
        /// Creates a new instance of content line
        /// </summary>
        /// <param name="_style">Style</param>
        /// <param name="Text">Value</param>
        /// <param name="when">Time</param>
        /// <param name="_notice">Whether it should trigger notification</param>
        public ContentLine(MessageStyle _style, string Text, DateTime when, bool _notice)
        {
            style = _style;
            time = when;
            text = Text;
            notice = _notice;
        }

        /// <summary>
        /// Comparer
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is ContentLine)
            {
                return this.time.CompareTo((obj as ContentLine).time);
            }
            return 0;
        }

        /// <summary>
        /// Message style
        /// </summary>
        public enum MessageStyle
        {
            /// <summary>
            /// System
            /// </summary>
            System,
            /// <summary>
            /// Message
            /// </summary>
            Message,
            /// <summary>
            /// Action
            /// </summary>
            Action,
            /// <summary>
            /// User
            /// </summary>
            User,
            /// <summary>
            /// Channel
            /// </summary>
            Channel,
            /// <summary>
            /// Kick
            /// </summary>
            Kick,
            /// <summary>
            /// Join
            /// </summary>
            Join,
            /// <summary>
            /// Part
            /// </summary>
            Part,
        }
    }
}
