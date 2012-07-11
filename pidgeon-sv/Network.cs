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

namespace pidgeon_sv
{
    public class WO
    {
        public bool ok = true;
        public string name = "";
        public bool writable = false;
    }
    public class Network
    {
        public bool Connected;
        public List<User> PrivateChat = new List<User>();
        public string server;
        public Protocol.UserMode usermode = new Protocol.UserMode();
        public string username;
        public List<Channel> Channels = new List<Channel>();
        public string nickname;
        public string ident;
        public string quit;
        public Protocol _protocol;
        public List<WO> windows = new List<WO>();



        public string channel_prefix = "#";

        public Channel getChannel(string name)
        {
            foreach (Channel cu in Channels)
            {
                if (cu.Name == name)
                {
                    return cu;
                }
            }
            return null;
        }

        public void CreateChat(string _name, bool _Focus, bool _writable = false, bool channelw = false)
        {
            WO w = new WO();
            w.name = _name;
            w.writable = _writable;
            windows.Add(w);
        }

        /// <summary>
        /// Create pm
        /// </summary>
        /// <param name="user"></param>
        public void Private(string user)
        {
            User u = new User(user, "", this, "");
            PrivateChat.Add(u);
            CreateChat(user, true, true);
            return;
        }

        public Channel Join(string channel)
        {
            Channel _channel = new Channel();
            _channel.Name = channel;
            _channel._Network = this;
            Channels.Add(_channel);
            CreateChat(channel, true, true, true);
            return _channel;
        }

        public bool ShowChat(string name)
        {
            return true;
        }

        public Network(string Server, Protocol sv)
        {
            server = Server;
            _protocol = sv;
            quit = "Pidgeon service - http://pidgeonclient.org";
            CreateChat("!system", true);
        }
    }
}
