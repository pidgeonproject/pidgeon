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

        public override bool Hook_UserJoin(Network network, User user, Channel channel, bool updated)
        {
            if (!updated)
            {
                return true;
            }
            Message(channel.retrieveWindow(), user.Nick + " joined");
            return false;
        }

        public override bool Hook_UserPart(Network network, User user, Channel channel, string message, bool updated)
        {
            if (!updated)
            {
                return true;
            }
            Message(channel.retrieveWindow(), user.Nick + " parted");
            return false;
        }

        public override bool Hook_UserQuit(Network network, User user, string message, Graphics.Window window, bool updated)
        {
            if (!updated)
            {
                return true;
            }
            Message(window, user.Nick + " joined");
            return false;
        }
    }
}
