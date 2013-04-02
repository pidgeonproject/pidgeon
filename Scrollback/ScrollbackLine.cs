using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
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
    }
}
