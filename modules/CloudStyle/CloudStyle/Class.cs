using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class RestrictedModule : Extension
    {
        public override void Initialise()
        {
            Name = "CloudStyle";
            Description = "This plugin changed the channel messages to look like in irc cloud";
            Version = "1.0";
            base.Initialise();
        }

        private void Message(Graphics.Window window, string text)
        {
            if (window == null)
            {
                return;
            }

            if (window.scrollback.IsEmtpy)
            {
                window.scrollback.InsertPart(text, ContentLine.MessageStyle.Join, false);
            }
            else
            {
                window.scrollback.InsertPart(", " + text, ContentLine.MessageStyle.Join, false);
            }
        }

        public override bool Hook_UserJoin(Network network, User user, Channel channel)
        {
            Message(channel.retrieveWindow(), user.Nick + " joined");
            return false;
        }

        public override bool Hook_UserPart(Network network, User user, Channel channel, string message)
        {
            Message(channel.retrieveWindow(), user.Nick + " parted");
            return false;
        }

        public override bool Hook_UserQuit(Network network, User user, string message, Graphics.Window window)
        {
            Message(window, user.Nick + " joined");
            return false;
        }
    }
}
