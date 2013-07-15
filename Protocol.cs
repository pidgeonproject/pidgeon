//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

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
    /// <summary>
    /// This is lowest level of protocol interface
    /// 
    /// Every protocol is inherited from this class. Protocols are handling connections to various servers using own protocols.
    /// </summary>
    [Serializable]
    public class IProtocol
    {
        /// <summary>
        /// Displayed window
        /// </summary>
        public Graphics.Window Current = null;
        /// <summary>
        /// Windows
        /// 
        /// This is a list of all windows that are associated with this protocol
        /// </summary>
        public Dictionary<string, Graphics.Window> Windows = new Dictionary<string, Graphics.Window>();
        /// <summary>
        /// Whether this server is connected or not
        /// </summary>
        protected bool Connected = false;
        /// <summary>
        /// Whether is destroyed
        /// </summary>
        protected bool destroyed = false;
        /// <summary>
        /// This is a time when this connection was open
        /// </summary>
        protected DateTime _time;
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
        /// Whether the connection is being encrypted or not
        /// </summary>
        public bool SSL = false;
        /// <summary>
        /// Encoding
        /// </summary>
        public Encoding NetworkEncoding = Configuration.irc.NetworkEncoding;
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

        /// <summary>
        /// Whether it is working
        /// </summary>
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
        public virtual Graphics.Window SystemWindow
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

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public IProtocol()
        {
            _time = DateTime.Now;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~IProtocol()
        {
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor called for connection to: " + Server);
            }
        }

        /// <summary>
        /// This function get an input from user, if it return false, it is handled by core
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual bool ParseInput(string input)
        {
            return false;
        }

        /// <summary>
        /// Request window to be shown
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool ShowChat(string name)
        {
            lock (Windows)
            {
                if (Windows.ContainsKey(name))
                {
                    Current = Windows[name];
                    Core.SystemForm.SwitchWindow(Current);
                    Current.Redraw();
                    if (Current.isChannel)
                    {
                        if (Core.SelectedNetwork != null)
                        {
                            Core.SelectedNetwork.RenderedChannel = Core.SelectedNetwork.getChannel(Current.WindowName);
                        }
                    }
                    Core.SystemForm.setChannel(name);
                    if (Current.Making == false)
                    {
                        Current.textbox.setFocus();
                    }
                    Core.SystemForm.Chat = Windows[name];
                    Current.Making = false;
                    Core.SystemForm.UpdateStatus();
                }
            }
            return true;
        }

        private void ClearWins()
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
            try
            {
                Core.SystemForm.SwitchRoot();
                if (SystemWindow != null)
                {
                    if (!Windows.ContainsValue(SystemWindow))
                    {
                        SystemWindow._Destroy();
                    }
                }
                ClearWins();
                Core.SystemForm.setChannel("");
                Core.SystemForm.Status("Disconnected from " + Server);
                Core.SystemForm.DisplayingProgress = false;
                Core.SystemForm.setText("");
            }
            finally
            {
                lock (Core.Connections)
                {
                    if (Core.Connections.Contains(this) && !Core.IgnoreErrors)
                    {
                        Core.Connections.Remove(this);
                    }
                }
                // we removed lot of memory now, let's clean it
                System.GC.Collect();
            }
        }

        /// <summary>
        /// This will connect this protocol
        /// </summary>
        /// <returns></returns>
        public virtual bool Open()
        {
            Core.DebugLog("Open() is not implemented");
            return false;
        }

        /// <summary>
        /// Deliver raw data to server
        /// </summary>
        /// <param name="data"></param>
        /// <param name="priority"></param>
        /// <param name="network"></param>
        public virtual void Transfer(string data, Configuration.Priority priority = Configuration.Priority.Normal, Network network = null)
        {
            Core.DebugLog("Transfer(string data, Configuration.Priority _priority = Configuration.Priority.Normal, Network network = null) is not implemented");
        }

        /// <summary>
        /// This will disconnect the protocol but leave it in memory
        /// </summary>
        /// <returns></returns>
        public virtual bool Disconnect()
        {
            Core.DebugLog("Disconnect() is not implemented");
            return false;
        }

        /// <summary>
        /// Reconnect
        /// </summary>
        /// <returns></returns>
        public virtual bool Reconnect()
        {
            Core.DebugLog("Reconnect() is not implemented");
            return false;
        }

        /// <summary>
        /// Parse a command
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public virtual bool Command(string cm, Network network = null)
        {
            Core.DebugLog("Command(string cm, Network network = null) is not implemented");
            return false;
        }
    }


    /// <summary>
    /// Connection
    /// </summary>
    [Serializable]
    public class Protocol : IProtocol
    {
        /// <summary>
        /// Character which is separating the special commands (such as CTCP part)
        /// </summary>
        public char delimiter = (char)001;
        /// <summary>
        /// If changes to windows should be suppressed (no color changes on new messages)
        /// </summary>
        public bool SuppressChanges = false;

        /// <summary>
        /// Create window
        /// </summary>
        /// <param name="name">Identifier of window, should be unique on network level, otherwise you won't be able to locate it</param>
        /// <param name="focus">Whether new window should be immediately focused</param>
        /// <param name="network">Network the window belongs to</param>
        /// <param name="channelw">If true a window will be flagged as channel</param>
        /// <param name="id"></param>
        /// <param name="hasUserList"></param>
        /// <param name="hasTextBox"></param>
        /// <returns></returns>
        public virtual Graphics.Window CreateChat(string name, bool focus, Network network, bool channelw = false, string id = null, bool hasUserList = true, bool hasTextBox = true)
        {
            Forms.Main._WindowRequest request = new Forms.Main._WindowRequest();
            if (id == null)
            {
                id = name;
            }
            request.owner = this;
            request.name = name;
            request.window = new Graphics.Window();
            request.focus = focus;
            request.window._Network = network;
            request.window.WindowName = name;
            request.hasUserList = hasUserList;
            request.hasTextBox = hasTextBox;

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

            lock (Core.SystemForm.WindowRequests)
            {
                // Create a request to create this window
                Core.SystemForm.WindowRequests.Add(request);
                Graphics.PidgeonList.Updated = true;
            }
            return request.window;
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
        /// <param name="text">Text</param>
        /// <returns>Escaped text</returns>
        public static string encode_text(string text)
        {
            return text.Replace("%", "%####%");
        }

        /// <summary>
        /// Reconnect
        /// </summary>
        /// <param name="network">Network</param>
        public virtual void ReconnectNetwork(Network network)
        {
            Core.DebugLog("ReconnectNetwork(Network network) is not implemented");
        }

        /// <summary>
        /// Format a message to given style selected by skin
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="text">Text</param>
        /// <returns></returns>
        public string PRIVMSG(string user, string text)
        {
            return Configuration.Scrollback.format_nick.Replace("$1", "%USER%" + user + "%/USER%") + encode_text(text);
        }
        
        /// <summary>
        /// This will ignore all certificate issues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// /me
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public virtual int Message2(string text, string to, Configuration.Priority priority = Configuration.Priority.Normal)
        {
            Core.DebugLog("Message2(string text, string to, Configuration.Priority priority = Configuration.Priority.Normal) is not implemented");
            return 2;
        }

        /// <summary>
        /// Send a message to server (deprecated)
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="to">User or a channel (needs to be prefixed with #)</param>
        /// <param name="priority">Priority</param>
        /// <param name="pmsg">Private</param>
        /// <returns></returns>
        public virtual int Message(string text, string to, Configuration.Priority priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            Core.DebugLog("Message(string text, string to, Configuration.Priority priority = Configuration.Priority.Normal, bool pmsg = false) is not implemented");
            return 2;
        }

        /// <summary>
        /// Send a message to server
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="to">User or a channel (needs to be prefixed with #)</param>
        /// <param name="network"></param>
        /// <param name="priority"></param>
        /// <param name="pmsg"></param>
        /// <returns></returns>
        public virtual int Message(string text, string to, Network network, Configuration.Priority priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            Core.DebugLog("Message(string text, string to, Network network, Configuration.Priority priority = "
                +"Configuration.Priority.Normal, bool pmsg = false) is not implemented");
            return 2;
        }

        /// <summary>
        /// Change nick
        /// </summary>
        /// <param name="_Nick"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public virtual int requestNick(string _Nick, Network network = null)
        {
            Core.DebugLog("requestNick(string _Nick, Network network = null) is not implemented");
            return 2;
        }

        /// <summary>
        /// Write a mode
        /// </summary>
        /// <param name="_x">Mode</param>
        /// <param name="target">Channel or user</param>
        /// <param name="network">Network</param>
        public virtual void WriteMode(NetworkMode _x, string target, Network network = null)
        {
            Core.DebugLog("WriteMode(NetworkMode _x, string target, Network network = null) is not implemented");
            return;
        }

        /// <summary>
        /// /join
        /// </summary>
        /// <param name="name">Channel</param>
        /// <param name="network">Network</param>
        public virtual void Join(string name, Network network = null)
        {
            Core.DebugLog("Join() is not implemented");
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
            Core.DebugLog("Disconnect() is not implemented");
            return false;
        }

        /// <summary>
        /// /part
        /// </summary>
        /// <param name="name">Channel</param>
        /// <param name="network">Network</param>
        public virtual void Part(string name, Network network = null)
        {
            Core.DebugLog("Part(string name, Network network = null) is not implemented");
            return;
        }
    }
}
