using System;
using System.Collections.Generic;
using System.Text;

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

        public override bool Hook_Topic(Network network, string userline, Channel channel, string topic)
        {
            string old_topic = channel.Topic;
            if (old_topic == null)
            {
                // there is nothing to make a diff from
                return true;
            }

            string length_info = "No change in length";

            if (topic.Length != old_topic.Length)
            {
                int size = topic.Length - old_topic.Length;
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
            words_new.AddRange(topic.Split('.', ' ', ',', '(', ')'));
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
                        word_info += data + ", ";
                    }
                }
                if (rm.Count > 0)
                {
                    word_info += "removed: ";
                    foreach (string data in rm)
                    {
                        word_info += data + ", ";
                    }
                }
            }

            Graphics.Window window = channel.retrieveWindow();

            if (window != null)
            {
                window.scrollback.InsertText("Topic change: " + length_info + ", " + word_info, ContentLine.MessageStyle.System);
                window.scrollback.InsertText("Old topic: " + old_topic, ContentLine.MessageStyle.System);
                window.scrollback.InsertText("New topic: " + topic, ContentLine.MessageStyle.System);
            }

            return true;
        }
    }
}
