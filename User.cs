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
using System.Text;

namespace Client
{
    /// <summary>
    /// User, Every user on irc has instance of this class for every channel they are in
    /// </summary>
    [Serializable]
    public class User : IComparable
    {
        /// <summary>
        /// Host name
        /// </summary>
        public string Host = null;
        /// <summary>
        /// Network
        /// </summary>
        [NonSerialized]
        public Network _Network = null;
        /// <summary>
        /// Identifier
        /// </summary>
        public string Ident = null;
        /// <summary>
        /// Channel mode
        /// </summary>
        public NetworkMode ChannelMode = new NetworkMode();
        /// <summary>
        /// Status
        /// </summary>
        public ChannelStatus Status = ChannelStatus.Regular;
        /// <summary>
        /// Nick
        /// </summary>
        public string Nick = null;
        /// <summary>
        /// Name
        /// </summary>
        public string RealName = null;
        /// <summary>
        /// Server
        /// </summary>
        public string Server = null;
        /// <summary>
        /// Away message
        /// </summary>
        public string AwayMessage = null;
        /// <summary>
        /// User away
        /// </summary>
        public bool Away = false;
        /// <summary>
        /// Check
        /// </summary>
        public DateTime LastAwayCheck;
        /// <summary>
        /// Primary chan
        /// </summary>
        public Channel Channel = null;
        /// <summary>
        /// Return true if user is owner of a channel
        /// </summary>
        public bool IsOwner
        {
            get
            {
                if (ChannelMode._Mode.Contains("q"))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Return true if user is admin of a channel
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                if (ChannelMode._Mode.Contains("a"))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Return true if user is op of a channel
        /// </summary>
        public bool IsOp
        {
            get
            {
                if (ChannelMode._Mode.Contains("o"))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Return true if user is half_operator of a channel
        /// </summary>
        public bool IsHalfop
        {
            get
            {
                if (ChannelMode._Mode.Contains("h"))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Return true if user is voiced in this channel
        /// </summary>
        public bool IsVoiced
        {
            get
            {
                if (ChannelMode._Mode.Contains("v"))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Away time
        /// </summary>
        public DateTime AwayTime;
        private bool destroyed = false;
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
        /// This return true if we are looking at current user
        /// </summary>
        public bool IsPidgeon
        {
            get
            {
                return (Nick.ToLower() == _Network.Nickname.ToLower());
            }
        }

        /// <summary>
        /// Get a list of all channels this user is in
        /// </summary>
        public List<Channel> ChannelList
        {
            get
            {
                List<Channel> List = new List<Channel>();
                if (_Network == null)
                {
                    return null;
                }
                lock (_Network.Channels)
                {
                    foreach (Channel xx in _Network.Channels)
                    {
                        if (xx.ContainsUser(Nick))
                        {
                            List.Add(xx);
                        }
                    }
                }
                return List;
            }
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">user!ident@hostname</param>
        /// <param name="network">Network this class belongs to</param>
        public User(string user, Network network)
        {
            if (!user.Contains("@") || !user.Contains("!"))
            {
                Core.DebugLog("Unable to create user from " + user);
                return;
            }
            string name = user.Substring(0, user.IndexOf("!"));
            string ident = user.Substring(user.IndexOf("!") + 1);
            string host = ident.Substring(ident.IndexOf("@") + 1);
            ident = ident.Substring(0, ident.IndexOf("@"));
            MakeUser(name, host, network, ident);
            Server = network.ServerName;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="nick"></param>
        /// <param name="host"></param>
        /// <param name="network"></param>
        /// <param name="ident"></param>
        public User(string nick, string host, Network network, string ident)
        {
            MakeUser(nick, host, network, ident);
            Server = network.ServerName;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="nick"></param>
        /// <param name="host"></param>
        /// <param name="network"></param>
        /// <param name="ident"></param>
        /// <param name="server"></param>
        public User(string nick, string host, Network network, string ident, string server)
        {
            MakeUser(nick, host, network, ident);
            Server = server;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="nick">Nick</param>
        /// <param name="host">Host</param>
        /// <param name="network">Network</param>
        /// <param name="ident">Ident</param>
        /// <param name="server">Server</param>
        /// <param name="channel">Channel</param>
        public User(string nick, string host, Network network, string ident, string server, Channel channel)
        {
            this.Server = server;
            this.Channel = channel;
            MakeUser(nick, host, network, ident);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~User()
        {
            // remove reference to network from channel mode that is no longer going to be accessible so that GC can remove it
            ChannelMode.network = null;
        }

        /// <summary>
        /// Change a user level according to symbol
        /// </summary>
        /// <param name="symbol"></param>
        public void SymbolMode(char symbol)
        {
            if (_Network == null)
            {
                return;
            }

            if (symbol == '\0')
            {
                return;
            }

            if (_Network.UChars.Contains(symbol))
            {
                char mode = _Network.CUModes[_Network.UChars.IndexOf(symbol)];
                ChannelMode.ChangeMode("+" + mode.ToString());
            }
        }
        
        private void MakeUser(string nick, string host, Network network, string ident)
        {
            _Network = network;
            if (nick != "")
            {
                char prefix = nick[0];
                if (network.UChars.Contains(prefix))
                {
                    SymbolMode(prefix);
                    nick = nick.Substring(1);
                }
            }
            Nick = nick;
            Ident = ident;
            Host = host;
        }

        /// <summary>
        /// Destroy
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }
            Channel = null;
            Core.SystemForm.ChannelList.RemoveUser(this);
            _Network = null;
            destroyed = true;
        }

        /// <summary>
        /// Converts a user object to string
        /// </summary>
        /// <returns>[nick!ident@host]</returns>
        public override string ToString()
        {
            return Nick + "!" + Ident + "@" + Host;
        }

        /// <summary>
        /// Generate full string
        /// </summary>
        /// <returns></returns>
        public string ConvertToInfoString()
        {
            if (RealName != "" && RealName != null)
            {
                return RealName + "\n" + ToString();
            }
            return ToString();
        }

        /// <summary>
        /// Internal function
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is User)
            {
                return this.Nick.CompareTo((obj as User).Nick);
            }
            return 0;
        }

        /// <summary>
        /// Channel status
        /// </summary>
        public enum ChannelStatus
        { 
            /// <summary>
            /// Owner
            /// </summary>
            Owner,
            /// <summary>
            /// Admin
            /// </summary>
            Admin,
            /// <summary>
            /// Operator
            /// </summary>
            Op,
            /// <summary>
            /// Halfop
            /// </summary>
            Halfop,
            /// <summary>
            /// Voice
            /// </summary>
            Voice,
            /// <summary>
            /// Normal user
            /// </summary>
            Regular,
        }
    }
}
