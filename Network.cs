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
        public Channel RenderedChannel = null;
        public string nickname;
        public string ident;
        public string quit;
        public Protocol _protocol;

        public string channel_prefix = "#";

        private Window Current;
        public Dictionary<string, Window> windows = new Dictionary<string, Window>();

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
            request.window = new Window();
            request._Focus = _Focus;
            request.window.name = _name;
            request.window.writable = _writable;
            windows.Add(_name, request.window);
            Core._Main.W.Add(request);
        }

        /// <summary>
        /// Initialise window - deprecated
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_writable"></param>
        /// <param name="window"></param>
        public void _Chat(string _name, bool _writable, Main._WindowRequest window)
        {
            
        }

        public int Join(string channel)
        {
            Channel _channel = new Channel();
            RenderedChannel = _channel;
            _channel.Name = channel;
            _channel._Network = this;
            Channels.Add(_channel);
            Core._Main.ChannelList.insertChannel(_channel);
            CreateChat(channel, true, true);
            return 0;
        }

        public bool ShowChat(string name)
        {
            if (windows.ContainsKey(name))
            {
                if (Core._Main.Chat != null)
                {
                    Core._Main.Chat.Visible = false;
                }
                
                Current = windows[name];
                Core._Main.Reload();
                Current.Redraw();
                Core._Main.toolStripStatusChannel.Text = name;
                Current.Visible = true;
                Current.textbox.Focus();
                Core._Main.Chat = windows[name];
                Current.Making = false;
                Core._Main.UpdateStatus();
            }
            return true;
        }

        public Network(string Server)
        {
            server = Server;
            quit = Configuration.quit;
            nickname = Configuration.nick;
            Core._Main.ChannelList.insertNetwork(this);
            CreateChat("!system", true);
            username = Configuration.user;
            ident = Configuration.ident;
        }
    }
}
