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
    [Serializable]
    public class User : IComparable
    {
        /// <summary>
        /// Host name
        /// </summary>
        public string Host = null;
        [NonSerialized]
        public Network _Network = null;
        public string Ident = null;
        public NetworkMode ChannelMode = new NetworkMode();
        public ChannelStatus Status = ChannelStatus.Regular;
        public string Nick = null;
        public string RealName = null;
        public string Server = null;
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
        /// Creates a new user
        /// </summary>
        /// <param name="user">user!ident@hostname</param>
        /// <param name="network"></param>
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

        ~User()
        {
            // remove reference to network from channel mode that is no longer going to be accessible so that GC can remove it
            ChannelMode.network = null;
        }

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
                        if (xx.containsUser(Nick))
                        {
                            List.Add(xx);
                        }
                    }
                }
                return List;
            }
        }

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

        public void Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }
            destroyed = true;
            Core._Main.ChannelList.RemoveUser(this);
            _Network = null;
        }

        /// <summary>
        /// Converts a user object to string
        /// </summary>
        /// <returns>[nick!ident@host]</returns>
        public override string ToString()
        {
            return Nick + "!" + Ident + "@" + Host;
        }

        public int CompareTo(object obj)
        {
            if (obj is User)
            {
                return this.Nick.CompareTo((obj as User).Nick);
            }
            return 0;
        }

        public enum ChannelStatus
        { 
            Owner,
            Admin,
            Op,
            Halfop,
            Voice,
            Regular,
        }
    }
}
