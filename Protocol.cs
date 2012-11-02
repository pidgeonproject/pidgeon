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
    public class Protocol
    {
        public char delimiter = (char)001;
        public Window Current;
        public Dictionary<string, Window> windows = new Dictionary<string, Window>();
        public bool Connected = false;
        public int type = 0;
        public string pswd;


        public class Mode
        {
            public List<string> _Mode = new List<string>();
            public Network network;
            public int ModeType = 0; //1 - channel 2 - user
            public override string ToString()
            {
                string _val = "";
                int curr = 0;
                while (curr < _Mode.Count)
                {
                    _val += _Mode[curr];
                    curr++;
                }
                return "+" + _val;
            }

            public Mode(int MT, Network _Network)
            {
                if (MT != 0)
                {
                    ModeType = MT;
                }
                network = _Network;
            }

            public Mode(string DefaultMode)
            {
                mode(DefaultMode);
            }

            public Mode()
            {
                network = null;
            }

            public bool mode(string text)
            {
                char prefix = ' ';
                foreach (char _x in text)
                {
                    if (ModeType != 0 && network != null)
                    {
                        switch (ModeType)
                        {
                            case 2:
                                if (network.CModes.Contains(_x))
                                {
                                    continue;
                                }
                                break;
                            case 1:
                                if (network.CUModes.Contains(_x) || network.PModes.Contains(_x))
                                {
                                    continue;
                                }
                                break;
                        }
                    }
                    if (_x == ' ')
                    {
                        return true;
                    }
                    if (_x == '-')
                    {
                        prefix = _x;
                        continue;
                    }
                    if (_x == '+')
                    {
                        prefix = _x;
                        continue;
                    }
                    switch (prefix)
                    { 
                        case '+':
                            if (!_Mode.Contains(_x.ToString()))
                            {
                                this._Mode.Add(_x.ToString());
                            }
                            continue;
                        case '-':
                            if (_Mode.Contains(_x.ToString()))
                            {
                                this._Mode.Remove(_x.ToString());
                            }
                            continue;
                    }continue;
                }
                return false;
            }
        }

        /// <summary>
        /// Server
        /// </summary>
        public string Server;
        /// <summary>
        /// Port
        /// </summary>
        public int Port;
        /// <summary>
        /// Ssl
        /// </summary>
        public bool SSL;

        /// <summary>
        /// Create window
        /// </summary>
        /// <param name="name">Identifier of window, should be unique on network level, otherwise you won't be able to locate it</param>
        /// <param name="focus">Whether new window should be immediately focused</param>
        /// <param name="network">Network the window belongs to</param>
        /// <param name="writable">If true user will be able to send text in window</param>
        /// <param name="channelw">If true a window will be flagged as channel</param>
        public virtual void CreateChat(string name, bool focus, Network network, bool writable = false, bool channelw = false, string id = null)
        {
            Main._WindowRequest request = new Main._WindowRequest();
            if (id == null)
            {
                id = name;
            }
            request.owner = this;
            request.name = name;
            request.writable = writable;
            request.window = new Window();
            request._Focus = focus;
            request.window._Network = network;
            request.window.name = name;
            request.window.writable = writable;

            if (network != null && !name.Contains("!"))
            { 
                windows.Add(network.window + id, request.window);
            }
            else
            {
                windows.Add(id, request.window);
            }
            
            if (channelw == true)
            {
                request.window.isChannel = true;
            }
            lock (Core._Main.W)
            {
                Core._Main.W.Add(request);
            }
        }

        /// <summary>
        /// Request window to be shown
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool ShowChat(string name)
        {
            if (windows.ContainsKey(name))
            {
                Current = windows[name];
                Current.Visible = true;
                if (Current != Core._Main.Chat)
                {
                    if (Core._Main.Chat != null)
                    {
                        Core._Main.Chat.Visible = false;
                    }
                }
                Current.Redraw();
                if (Current.isChannel)
                {
                    if (Core.network != null)
                    {
                        Core.network.RenderedChannel = Core.network.getChannel(Current.name);
                    }
                }
                Core._Main.toolStripStatusChannel.Text = name;
                if (Current.Making == false)
                {
                    Current.textbox.Focus();
                }
                Core._Main.Chat = windows[name];
                Current.BringToFront();
                Current.Making = false;
                Core._Main.UpdateStatus();
            }
            return true;
        }

        /// <summary>
        /// Return back the system chars to previous look
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string decrypt_text(string text)
        {
            return text.Replace("%#", "%");
        }


        /// <summary>
        /// Escape system char
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string encode_text(string text)
        {
            return text.Replace("%", "%#");
        }

        /// <summary>
        /// Format a message to given style selected by skin
        /// </summary>
        /// <param name="user"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public string PRIVMSG(string user, string text)
        {
            return Configuration.format_nick.Replace("$1", "%USER%" + user + "%/USER%") + encode_text(text);
        }

        /// <summary>
        /// Deliver raw data to server
        /// </summary>
        /// <param name="data"></param>
        /// <param name="_priority"></param>
        public virtual void Transfer(string data, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            
        }

        /// <summary>
        /// /me
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="_priority"></param>
        /// <returns></returns>
        public virtual int Message2(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            return 0;
        }

        /// <summary>
        /// Send a message to server
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="to">User or a channel (needs to be prefixed with #)</param>
        /// <param name="_priority"></param>
        /// <returns></returns>
        public virtual int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            return 0;
        }

        /// <summary>
        /// Change nick
        /// </summary>
        /// <param name="_Nick"></param>
        /// <returns></returns>
        public virtual int requestNick(string _Nick)
        {
            return 2;
        }

        /// <summary>
        /// Parse a command
        /// </summary>
        /// <param name="cm"></param>
        /// <returns></returns>
        public virtual bool Command(string cm)
        {
            return false;
        }

        /// <summary>
        /// Write a mode
        /// </summary>
        /// <param name="_x">Mode</param>
        /// <param name="target">Channel or user</param>
        /// <param name="network">Network</param>
        public virtual void WriteMode(Mode _x, string target, Network network = null)
        {
            return;
        }

        /// <summary>
        /// /join
        /// </summary>
        /// <param name="name">Channel</param>
        /// <param name="network">Network</param>
        public virtual void Join(string name, Network network = null)
        {
            return;
        }

        /// <summary>
        /// /connect
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="port">Port</param>
        /// <returns></returns>
        public virtual bool ConnectTo(string server, int port)
        {
            return false;
        }

        /// <summary>
        /// /part
        /// </summary>
        /// <param name="name">Channel</param>
        /// <param name="network">Network</param>
        public virtual void Part(string name, Network network = null)
        {
            return;
        }

        /// <summary>
        /// Disconnect server
        /// </summary>
        public virtual void Exit() 
        {
            if (windows.ContainsValue(Core._Main.Chat))
            {
                Core._Main.main.Visible = true;
                Core._Main.Chat.Visible = false;
                Core._Main.Chat.Dispose();
                Core._Main.Chat = Core._Main.main;
            }
            if (Core.Connections.Contains(this) && !Core.IgnoreErrors)
            {
                Core.Connections.Remove(this);
            }
        }  

        public class UserMode : Mode
        {
            
        }

        public virtual bool Open()
        {
            return false;
        }
    }
}
