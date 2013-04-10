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

// Documentation
///////////////////////
// This file contains a default class for protocols which all the other classes are inherited from
// some functions are returning integer, which should be 0 on success and 2 by default
// which means that the function was never overriden, so that a function working with that can catch it

using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Client
{
    [Serializable]
    public class Protocol
    {
        /// <summary>
        /// Character which is separating the special commands (such as CTCP part)
        /// </summary>
        public char delimiter = (char)001;
        /// <summary>
        /// Displayed window
        /// </summary>
        public Graphics.Window Current = null;
        /// <summary>
        /// Windows
        /// </summary>
        public Dictionary<string, Graphics.Window> Windows = new Dictionary<string, Graphics.Window>();
        /// <summary>
        /// Whether this server is connected or not
        /// </summary>
        protected bool Connected = false;
        /// <summary>
        /// If changes to windows should be suppressed (no color changes on new messages)
        /// </summary>
        public bool SuppressChanges = false;
        /// <summary>
        /// Password for server
        /// </summary>
        public string Password = null;
        /// <summary>
        /// Server
        /// </summary>
        public string Server = null;
        /// <summary>
        /// Port
        /// </summary>
        public int Port = 6667;
        /// <summary>
        /// Ssl
        /// </summary>
        public bool SSL = false;
        private bool destroyed = false;
        /// <summary>
        /// This is a time when this connection was open
        /// </summary>
        protected DateTime _time;
        
        /// <summary>
        /// Time since you connected to this protocol
        /// </summary>
        public TimeSpan ConnectionTime
        {
            get
            {
                return DateTime.Now - _time;
            }
        }

        public bool IsConnected
        {
            get
            {
                return Connected;
            }
        }

        /// <summary>
        /// This will return true in case object was requested to be disposed
        /// you should never work with objects that return true here
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return destroyed;
            }
        }

        /// <summary>
        /// Root window
        /// </summary>
        public Graphics.Window SystemWindow
        {
            get
            {
                lock (Windows)
                {
                    if (Windows.ContainsKey("!system"))
                    {
                        return Windows["!system"];
                    }
                }
                return null;
            }
        }

        public Protocol()
        {
            _time = DateTime.Now;
        }

        ~Protocol()
        {
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor called for connection to: " + Server);
                //Core.DebugLog("Released: " + Core.GetSizeOfObject(this).ToString() + " bytes of memory");
            }
        }

        /// <summary>
        /// Create window
        /// </summary>
        /// <param name="name">Identifier of window, should be unique on network level, otherwise you won't be able to locate it</param>
        /// <param name="focus">Whether new window should be immediately focused</param>
        /// <param name="network">Network the window belongs to</param>
        /// <param name="writable">If true user will be able to send text in window</param>
        /// <param name="channelw">If true a window will be flagged as channel</param>
        public virtual Graphics.Window CreateChat(string name, bool focus, Network network, bool writable = false, bool channelw = false, string id = null)
        {
            Forms.Main._WindowRequest request = new Forms.Main._WindowRequest();
            if (id == null)
            {
                id = name;
            }
            request.owner = this;
            request.name = name;
            request.writable = writable;
            request.window = new Graphics.Window();
            request.focus = focus;
            request.window._Network = network;
            request.window.name = name;
            request.window.writable = writable;

            if (network != null && !name.Contains("!"))
            { 
                Windows.Add(network.SystemWindowID + id, request.window);
            }
            else
            {
                Windows.Add(id, request.window);
            }
            
            if (channelw == true)
            {
                request.window.isChannel = true;
            }
            lock (Core._Main.WindowRequests)
            {
                // Create a request to create this window
                Core._Main.WindowRequests.Add(request);
                Graphics.PidgeonList.Updated = true;
            }
            return request.window;
        }

        /// <summary>
        /// Request window to be shown
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool ShowChat(string name)
        {
            if (Windows.ContainsKey(name))
            {
                Current = Windows[name];
                Core._Main.SwitchWindow(Current);
                Current.Redraw();
                if (Current.isChannel)
                {
                    if (Core.network != null)
                    {
                        Core.network.RenderedChannel = Core.network.getChannel(Current.name);
                    }
                }
                Core._Main.setChannel(name);
                if (Current.Making == false)
                {
                    Current.textbox.setFocus();
                }
                Core._Main.Chat = Windows[name];
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
            return Configuration.Scrollback.format_nick.Replace("$1", "%USER%" + user + "%/USER%") + encode_text(text);
        }
        
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
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
            return 2;
        }

        /// <summary>
        /// Send a message to server (deprecated)
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="to">User or a channel (needs to be prefixed with #)</param>
        /// <param name="_priority">Priority</param>
        /// <returns></returns>
        public virtual int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            return 2;
        }

        /// <summary>
        /// Send a message to server
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="to">User or a channel (needs to be prefixed with #)</param>
        /// <param name="network"></param>
        /// <param name="_priority"></param>
        /// <param name="pmsg"></param>
        /// <returns></returns>
        public virtual int Message(string text, string to, Network network, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            return 2;
        }

        /// <summary>
        /// Change nick
        /// </summary>
        /// <param name="_Nick"></param>
        /// <returns></returns>
        public virtual int requestNick(string _Nick, Network network = null)
        {
            return 2;
        }

        /// <summary>
        /// Parse a command
        /// </summary>
        /// <param name="cm"></param>
        /// <returns></returns>
        public virtual bool Command(string cm, Network network = null)
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
        /// This will disconnect the protocol but leave it in memory
        /// </summary>
        /// <returns></returns>
        public virtual bool Disconnect()
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

        public void ClearWins()
        {
            lock (Windows)
            {
                foreach (Graphics.Window xx in Windows.Values)
                {
                    xx._Destroy();
                }
            }
            Current = null;
            Windows.Clear();
        }

        /// <summary>
        /// Disconnect server
        /// </summary>
        public virtual void Exit() 
        {
            Core._Main.rootToolStripMenuItem_Click(null, null);
            if (SystemWindow != null)
            {
                if (!Windows.ContainsValue(SystemWindow))
                {
                    SystemWindow._Destroy();
                }
            }
            ClearWins();
            // we removed lot of memory now, let's clean it
            System.GC.Collect();
            Core._Main.setChannel("");
            Core._Main.Status("Disconnected from " + Server);
            Core._Main.DisplayingProgress = false;
            Core._Main.setText("");
            lock (Core.Connections)
            {
                if (Core.Connections.Contains(this) && !Core.IgnoreErrors)
                {
                    Core.Connections.Remove(this);
                }
            }
        }  

        public virtual bool Open()
        {
            return false;
        }
    }
}
