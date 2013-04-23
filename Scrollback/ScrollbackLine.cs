using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
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
