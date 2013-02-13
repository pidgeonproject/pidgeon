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
        /// <summary>
        /// Character which is separating the special commands (such as CTCP part)
        /// </summary>
        public char delimiter = (char)001;
        /// <summary>
        /// Displayed window
        /// </summary>
        public Window Current = null;
        /// <summary>
        /// Root window
        /// </summary>
        public Window SystemWindow
        {
            get
            { 
                lock (windows)
                {
                    if (windows.ContainsKey("!system"))
                    {
                        return windows["!system"];
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Windows
        /// </summary>
        public Dictionary<string, Window> windows = new Dictionary<string, Window>();
        /// <summary>
        /// Whether this network is connected or not
        /// </summary>
        public bool Connected = false;
        /// <summary>
        /// Type of protocol
        /// </summary>
        public int ProtocolType = 0;
        /// <summary>
        /// If changes to channels should be suppressed (no color changes on new messages)
        /// </summary>
        public bool SuppressChanges = false;
        /// <summary>
        /// Password for server
        /// </summary>
        public string Password = null;

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
            request.focus = focus;
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
            lock (Core._Main.WindowRequests)
            {
                // Create a request to create this window
                Core._Main.WindowRequests.Add(request);
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
                Current.scrollback.Display();
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
        public static string decode_text(string text)
        {
            return text.Replace("%####%", "%");
        }


        /// <summary>
        /// Escape system char
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string encode_text(string text)
        {
            return text.Replace("%", "%####%");
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
        public virtual void Transfer(string data, Configuration.Priority _priority = Configuration.Priority.Normal, Network network = null)
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
        public virtual void WriteMode(NetworkMode _x, string target, Network network = null)
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

        public virtual bool Open()
        {
            return false;
        }
    }
}
