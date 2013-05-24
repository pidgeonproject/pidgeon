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

namespace Client
{
    public class TopicDiff : Extension
    {
        public override void Initialise()
        {
            Name = "TopicDiff";
            Version = "1.0";
            Description = "This extension let you display a diff of change to a topic";
        }

        public override bool Hook_Topic(Extension.TopicArgs _TopicArgs)
        {
            string old_topic = _TopicArgs.channel.Topic;
            if (old_topic == null)
            {
                // there is nothing to make a diff from
                return true;
            }

            string length_info = "No change in length";

            if (_TopicArgs.Topic.Length != old_topic.Length)
            {
                int size = _TopicArgs.Topic.Length - old_topic.Length;
                if (size > 0)
                {
                    length_info = "New topic has " + size.ToString() + " more characters";
                }
                else
                {
                    length_info = "New topic has " + size.ToString() + " less characters";
                }
            }

            List<string> rm = new List<string>();
            List<string> add = new List<string>();
            List<string> words_new = new List<string>();
            words_new.AddRange(_TopicArgs.Topic.Split('.', ' ', ',', '(', ')'));
            List<string> words_old = new List<string>();
            words_old.AddRange(old_topic.Split('.', ' ', ',', '(', ')'));

            foreach (string word in words_old)
            {
                if (!words_new.Contains(word))
                {
                    rm.Add(word);
                }
            }

            foreach (string word in words_new)
            {
                if (!words_old.Contains(word))
                {
                    add.Add(word);
                }
            }

            string word_info = "No words changed";

            if (add.Count > 0 || rm.Count > 0)
            {
                word_info = "";
                if (add.Count > 0)
                {
                    word_info += "added: ";
                    foreach (string data in add)
                    {
                        word_info += data + " ";
                    }
                }
                if (rm.Count > 0)
                {
                    word_info += "removed: ";
                    foreach (string data in rm)
                    {
                        word_info += data + " ";
                    }
                }
            }

            Graphics.Window window = _TopicArgs.channel.RetrieveWindow();

            if (window != null)
            {
                window.scrollback.InsertText("Topic change: " + length_info + ", " + word_info, ContentLine.MessageStyle.System, true, _TopicArgs.date);
                window.scrollback.InsertText("Old topic: " + old_topic, ContentLine.MessageStyle.System, true, _TopicArgs.date);
                window.scrollback.InsertText("New topic: " + _TopicArgs.Topic, ContentLine.MessageStyle.System, true, _TopicArgs.date);
            }

            return true;
        }
    }
}
