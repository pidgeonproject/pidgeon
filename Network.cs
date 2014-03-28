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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Pidgeon
{
    /// <summary>
    /// Instance of irc network, this class is typically handled by protocols
    /// </summary>
    public class Network : libirc.Network, IDisposable
    {
        /// <summary>
        /// Highlight
        /// </summary>
        public class Highlighter
        {
            /// <summary>
            /// Is simple
            /// </summary>
            public bool Simple;
            /// <summary>
            /// Text
            /// </summary>
            public string Text;
            /// <summary>
            /// Enabled
            /// </summary>
            public bool Enabled;
            /// <summary>
            /// Expression
            /// </summary>
            public System.Text.RegularExpressions.Regex regex = null;

            /// <summary>
            /// Creates a new
            /// </summary>
            public Highlighter()
            {
                Simple = true;
                Enabled = false;
                Text = Configuration.UserData.user;
            }
        }

        /// <summary>
        /// System window
        /// </summary>
        public Graphics.Window SystemWindow = null;
        /// <summary>
        /// List of all channels on network
        /// </summary>
        public new Dictionary<string, Channel> Channels = new Dictionary<string, Channel>();
        /// <summary>
        /// Currently rendered channel on main window
        /// </summary>
        public new Channel RenderedChannel = null;
        /// <summary>
        /// Parent service
        /// </summary>
        public Protocols.Services.ProtocolSv ParentSv = null;
        public string RandomuQID;
        /// <summary>
        /// If true, the channel data will be suppressed in system window
        /// </summary>
        private Forms.Channels wChannelList = null;
        /// <summary>
        /// Private windows
        /// </summary>
        public Dictionary<User, Graphics.Window> PrivateWins = new Dictionary<User, Graphics.Window>();
        /// <summary>
        /// Window ID of this network system window
        /// </summary>
        public string SystemWindowID
        {
            get
            {
                if (_Protocol.GetType() != typeof(Protocols.Services.ProtocolSv))
                {
                    return "system";
                }
                else
                {
                    return RandomuQID + ServerName;
                }
            }
        }

        /// <summary>
        /// Creates a new network, requires name and protocol type
        /// </summary>
        /// <param name="Server">Server name</param>
        /// <param name="protocol">Protocol that own this instance</param>
        public Network(string Server, libirc.Protocol protocol) : base(Server, protocol)
        {
            RandomuQID = Core.RetrieveRandom();
            Quit = Configuration.UserData.quit;
            Nickname = Configuration.UserData.nick;
            UserName = Configuration.UserData.user;
            Ident = Configuration.UserData.ident;
            if (protocol.GetType() == typeof(Protocols.Services.ProtocolSv))
            {
                SystemWindow = WindowsManager.CreateChat("!" + ServerName, false, this, false, "!" + RandomuQID + ServerName, false, true, this);
                Core.SystemForm.ChannelList.InsertNetwork(this, (Protocols.Services.ProtocolSv)protocol);
            }
            else
            {
                SystemWindow = WindowsManager.CreateChat("!system", true, this, false, null, false, true, this);
                Core.SystemForm.ChannelList.InsertNetwork(this);
            }
            Hooks._Network.CreatingNetwork(this);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Network()
        {
            Dispose(true);
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor called for network: " + ServerName);
                //Core.DebugLog("Released: " + System.Runtime.InteropServices.Marshal.SizeOf(this).ToString() + " bytes of memory");
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Display server channel list
        /// </summary>
        public void DisplayChannelWindow()
        {
            if (wChannelList == null)
            {
                wChannelList = new Forms.Channels(this);
            }
            wChannelList.Show();
        }

        /// <summary>
        /// If such a user is contained in a private message list it will be returned
        /// </summary>
        /// <param name="user">User nick</param>
        /// <returns>Instance of user or null if it doesn't exist</returns>
        public User getUser(string user)
        {
            foreach (User x in PrivateChat)
            {
                if (x.Nick.ToLower() == user.ToLower())
                {
                    return x;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve channel
        /// </summary>
        /// <param name="name">String</param>
        /// <returns>Channel or null if it doesn't exist</returns>
        public new Channel GetChannel(string name)
        {
            lock (this.Channels)
            {
                Channel channel = null;
                if (this.Channels.TryGetValue(name.ToLower(), out channel))
                {
                    return channel;
                }
                return null;
            }
        }

        /// <summary>
        /// Create private message to user
        /// </summary>
        /// <param name="user">User name</param>
        public User Private(string user)
        {
            User referenced_user = new User(user, "", this, "");
            PrivateChat.Add(referenced_user);
            Core.SystemForm.ChannelList.insertUser(referenced_user);
            PrivateWins.Add(referenced_user, WindowsManager.CreateChat(user, Configuration.UserData.SwitchWindowOnJoin, this,
                                                                       true, null, false, true, this));
            PrivateWins[referenced_user].IsPrivMsg = true;
            if (Configuration.UserData.SwitchWindowOnJoin)
            {
                Core.SystemForm.ChannelList.ReselectWindow(PrivateWins[referenced_user]);
            }
            return referenced_user;
        }

        /// <summary>
        /// Send a message to network
        /// </summary>
        /// <param name="text">Text of message</param>
        /// <param name="to">Sending to</param>
        /// <param name="_priority">Priority</param>
        /// <param name="pmsg">If this is private message (so it needs to be handled in a different way)</param>
        public void Message(string text, string to, libirc.Defs.Priority _priority = libirc.Defs.Priority.Normal, bool pmsg = false)
        {
            _Protocol.Message(text, to, this, _priority);
        }

        public Channel CreateChannel(string channel)
        {
            Channel ch = GetChannel(channel);
            if(ch == null)
            {
                // we aren't in this channel, which is expected, let's create a new window for it
                ch = new Pidgeon.Channel(this, channel);
                lock(this.Channels)
                {
                    this.Channels.Add(ch.lName, ch);
                }
            }
            return ch;
        }

        public override void __evt_Self(NetworkSelfEventArgs args)
        {
            switch(args.Type)
            {
                case libirc.Network.EventType.Join:
                    CreateChannel(args.ChannelName);
                    break;
            }
        }
    }
}
