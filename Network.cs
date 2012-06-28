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
using System.Linq;
using System.Text;

namespace Client
{
    public class Network
    {
        public bool Connected;
        public string server;
        public Protocol.UserMode usermode = new Protocol.UserMode();
        public string username;
        public List<Channel> Channels = new List<Channel>();
        public string nickname;
        public string ident;
        public string quit;
        public Protocol _protocol;

        public class _window
        {
            public Scrollback scrollback;
            public System.Windows.Forms.ListView userlist;
            public bool Making = true;
            public TextBox textbox;
            public string name;
            public bool writable;
        }

        private _window Current;
        public Dictionary<string, _window> windows = new Dictionary<string, _window>();

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

        public void CreateChat(string _name, bool _Focus, bool _writable = false)
        {
            Main._WindowRequest request = new Main._WindowRequest();
            request.owner = this;
            request.name = _name;
            request.writable = _writable;
            request._Focus = _Focus;
            Core._Main.W.Add(request);
        }

        public void _Chat(string _name, bool _writable)
        {
            _window Chat = new _window();
            Chat.writable = _writable;
            windows.Add(_name, Chat);
            Chat.scrollback = Core._Main.CreateS();
            Chat.textbox = Core._Main.CreateText();
            Chat.userlist = Core._Main.CreateList();
            Chat.Making = false;
            Chat.name = _name;
        }

        public int Join(string channel)
        {
            Channel _channel = new Channel();
            _channel.Name = channel;
            _channel._Network = this;
            Channels.Add(_channel);
            Core._Main.channelList1.insertChannel(_channel);
            CreateChat(channel, true, true);
            return 0;
        }

        public bool ShowChat(string name)
        {
            if (windows.ContainsKey(name))
            {
                Core._Main.listView.Visible = false;
                Core._Main.MessageLine.Visible = false;
                Core._Main.Scrollback.Visible = false;
                if (Core._Main.Chat != null)
                {
                    Core._Main.Chat.scrollback.Visible = false;
                    Core._Main.Chat.userlist.Visible = false;
                    Core._Main.Chat.textbox.Visible = false;
                }
                if (Current != null)
                {
                    Current.userlist.Visible = false;
                    Current.scrollback.Visible = false;
                    Current.textbox.Visible = false;
                }
                Core._Main._Scrollback = windows[name].scrollback;
                Core._Main.Chat = windows[name];
                Current = windows[name];
                windows[name].textbox.Visible = true;
                windows[name].textbox.Focus();
                windows[name].scrollback.Visible = true;
                windows[name].userlist.Visible = true;
                Core._Main.Reload();
            }
            return true;
        }

        public Network(string Server)
        {
            server = Server;
            quit = Configuration.quit;
            nickname = Configuration.nick;
            Core._Main.channelList1.insertNetwork(this);
            CreateChat("!system", true);
            username = Configuration.user;
            ident = Configuration.ident;
        }
    }
}
