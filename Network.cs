﻿/***************************************************************************
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
using System.Threading;
using System.Text;

namespace Client
{
    public class Network
    {
        public class Highlighter
        {
            public bool simple;
            public string text;
            public bool enabled;
            public System.Text.RegularExpressions.Regex regex = null;

            public Highlighter()
            {
                simple = true;
                enabled = false;
                text = Configuration.UserData.user;
            }
        }

        [Serializable]
        public class ChannelData
        {
            public string ChannelName = null;
            public int UserCount = 0;
            public string ChannelTopic = null;

            public ChannelData(int Users, string Name, string Topic)
            {
                ChannelTopic = Topic;
                UserCount = Users;
                ChannelName = Name;
            }

            /// <summary>
            /// This constructor needs to exist for xml deserialization don't remove it
            /// </summary>
            public ChannelData()
            {

            }
        }

        /// <summary>
        /// User modes
        /// </summary>
        public List<char> UModes = new List<char> { 'i', 'w', 'o', 'Q', 'r', 'A' };
        /// <summary>
        /// Channel user symbols (oper and such)
        /// </summary>
        public List<char> UChars = new List<char> { '~', '&', '@', '%', '+' };
        /// <summary>
        /// Channel user modes
        /// </summary>
        public List<char> CUModes = new List<char> { 'q', 'a', 'o', 'h', 'v' };
        /// <summary>
        /// Channel modes
        /// </summary>
        public List<char> CModes = new List<char> { 'n', 'r', 't', 'm' };
        /// <summary>
        /// Special channel modes with parameter as a string
        /// </summary>
        public List<char> SModes = new List<char> { 'k', 'L' };
        /// <summary>
        /// Special channel modes with parameter as a number
        /// </summary>
        public List<char> XModes = new List<char> { 'l' };
        /// <summary>
        /// Special channel user modes with parameters as a string
        /// </summary>
        public List<char> PModes = new List<char> { 'b', 'I', 'e' };
        /// <summary>
        /// Descriptions for channel and user modes
        /// </summary>
        public Dictionary<char, string> Descriptions = new Dictionary<char, string>();
        /// <summary>
        /// Check if the info is parsed
        /// </summary>
        public bool parsed_info = false;
        /// <summary>
        /// Symbol prefix of channels
        /// </summary>
        public string channel_prefix = "#";
        /// <summary>
        /// Specifies if you are connected to network
        /// </summary>
        public bool Connected = false;
        /// <summary>
        /// List of private message windows
        /// </summary>
        public List<User> PrivateChat= new List<User>();
        /// <summary>
        /// System window
        /// </summary>
        public Graphics.Window SystemWindow = null;
        /// <summary>
        /// Host name of server
        /// </summary>
        public string ServerName = null;
        /// <summary>
        /// User mode of current user
        /// </summary>
        public NetworkMode usermode = new NetworkMode();
        /// <summary>
        /// User name (real name)
        /// </summary>
        public string UserName = null;
        /// <summary>
        /// Randomly generated ID for this network to make it unique in case some other network would share the name
        /// </summary>
        public string randomuqid = null;
        /// <summary>
        /// List of all channels on network
        /// </summary>
        public List<Channel> Channels = new List<Channel>();
        /// <summary>
        /// Currently rendered channel on main window
        /// </summary>
        public Channel RenderedChannel = null;
        /// <summary>
        /// Nickname of this user
        /// </summary>
        public string Nickname = null;
        /// <summary>
        /// Identification of user
        /// </summary>
        public string Ident = "pidgeon";
        /// <summary>
        /// Quit message
        /// </summary>
        public string Quit = null;
        /// <summary>
        /// Protocol
        /// </summary>
        public Protocol _Protocol = null;
        /// <summary>
        /// Specifies whether this network is using SSL connection
        /// </summary>
        public bool isSecure = false;
        /// <summary>
        /// Parent service
        /// </summary>
        public ProtocolSv ParentSv = null;
        /// <summary>
        /// List of channels
        /// </summary>
        public List<ChannelData> ChannelList = new List<ChannelData>();
        /// <summary>
        /// If true, the channel data will be suppressed in system window
        /// </summary>
        public bool SuppressData = false;
        /// <summary>
        /// This is true when network is just parsing the list of all channels
        /// </summary>
        public bool DownloadingList = false;
        /// <summary>
        /// Pointer to channel window
        /// </summary>
        private Forms.Channels wChannelList = null;
        /// <summary>
        /// Private windows
        /// </summary>
        public Dictionary<User, Graphics.Window> PrivateWins = new Dictionary<User, Graphics.Window>();
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
        /// Window ID of this network system window
        /// </summary>
        public string SystemWindowID
        {
            get
            {
                if (_Protocol.GetType() != typeof(ProtocolSv))
                {
                    return "system";
                }
                else
                {
                    return randomuqid + ServerName;
                }
            }
        }
        
        /// <summary>
        /// Removes the char from user.
        /// </summary>
        /// <returns>
        /// The username without char.
        /// </returns>
        /// <param name='username'>
        /// Username.
        /// </param>
        public string RemoveCharFromUser(string username)
        {
            foreach (char xx in UChars)
            {
                if (username.Contains(xx.ToString ()))
                {
                    username = username.Replace (xx.ToString(), "");
                }
            }
            
            return username;
        }
        
        public void DisplayChannelWindow()
        {
            try
            {
                if (wChannelList == null)
                {
                    wChannelList = new Forms.Channels();
                    wChannelList.network = this;
                    wChannelList.Init();
                }

                wChannelList.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Retrieve information about given channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public ChannelData ContainsChannel(string channel)
        {
            lock (ChannelList)
            {
                foreach (ChannelData data in ChannelList)
                {
                    if (channel.ToLower() == data.ChannelName.ToLower())
                    {
                        return data;
                    }
                }
            }
            return null;
        }

        ~Network()
        {
            if (!IsDestroyed)
            {
                Destroy();
            }
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor called for network: " + ServerName);
                //Core.DebugLog("Released: " + System.Runtime.InteropServices.Marshal.SizeOf(this).ToString() + " bytes of memory");
            }
        }

        public void Part(string channel_name)
        {
            _Protocol.Part(channel_name, this);
        }

        public void Part(Channel channel)
        {
            channel.ChannelWork = false;
            _Protocol.Part(channel.Name, this);
        }

        /// <summary>
        /// UNIX time to DateTime
        /// </summary>
        /// <param name="time">timestamp</param>
        /// <returns></returns>
        public static DateTime convertUNIX(string time)
        {
            double unixtimestmp = 0;
            if (!double.TryParse(time, out unixtimestmp))
            {
                unixtimestmp = 0;
            }
            return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixtimestmp);
        }

        /// <summary>
        /// Retrieve channel
        /// </summary>
        /// <param name="name">String</param>
        /// <returns></returns>
        public Channel getChannel(string name)
        {
            foreach (Channel chan in Channels)
            {
                if (chan.Name == name)
                {
                    return chan;
                }
            }
            return null;
        }

        /// <summary>
        /// Create private message to user
        /// </summary>
        /// <param name="user">User name</param>
        public User Private(string user)
        {
            User referenced_user = new User(user, "", this, "");
            PrivateChat.Add(referenced_user);
            Core._Main.ChannelList.insertUser(referenced_user);
            PrivateWins.Add(referenced_user, _Protocol.CreateChat(user, true, this, true));
            PrivateWins[referenced_user].isPM = true;
            return referenced_user;
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="channel">Channel name which is supposed to be joined</param>
        /// <param name="nf">Whether newly created window should be displayed over existing windows</param>
        /// <returns></returns>
        public void Join(string channel)
        {
            Transfer("JOIN " + channel, Configuration.Priority.Normal);
        }

        /// <summary>
        /// Create a new instance of channel window
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="nf"></param>
        /// <returns></returns>
        public Channel WindowCreateNewJoin(string channel, bool nf = false)
        {
            Channel previous = getChannel(channel);
            if (previous == null)
            {
                Channel _channel = new Channel(this);
                RenderedChannel = _channel;
                _channel.Name = channel;
                Channels.Add(_channel);
                Core._Main.ChannelList.insertChannel(_channel);
                _Protocol.CreateChat(channel, !nf, this, true, true);
                return _channel;
            }
            else
            {
                return previous;
            }
        }

        /// <summary>
        /// Send a message to network
        /// </summary>
        /// <param name="text">Text of message</param>
        /// <param name="to">Sending to</param>
        /// <param name="_priority">Priority</param>
        /// <param name="pmsg">If this is private message (so it needs to be handled in a different way)</param>
        public void Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            _Protocol.Message(text, to, this, _priority, pmsg);
        }
        
        /// <summary>
        /// Unregister info for user and channel modes
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool UnregisterInfo(char key)
        {
            lock (Descriptions)
            {
                if (Descriptions.ContainsKey(key))
                {
                    Descriptions.Remove(key);
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Destroy this class, be careful, it can't be used in any way after you
        /// call this
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
            {
                // avoid calling this function multiple times, otherwise it could crash
                Core.DebugLog("Destroy() called multiple times on " + ServerName);
                return;
            }

            destroyed = true;

            lock (ChannelList)
            {
                ChannelList.Clear();
            }

            if (wChannelList != null)
            {
                wChannelList.Dispose();
                wChannelList = null;
            }

            lock (PrivateChat)
            {
                // release all windows
                foreach (User user in PrivateChat)
                {
                    user.Destroy();
                }
                PrivateChat.Clear();
            }
            
            lock (Channels)
            {
                foreach (Channel xx in Channels)
                {
                    xx.Destroy();
                }
                
                Channels.Clear();
            }
            
            lock (PrivateWins)
            {
                PrivateWins.Clear();
            }
            
            _Protocol = null;
            SystemWindow = null;
            
            lock (Descriptions)
            {
                Descriptions.Clear();
            }
        }
        
        /// <summary>
        /// Register info for channel info
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool RegisterInfo(char key, string text)
        { 
            lock (Descriptions)
            {
                if (Descriptions.ContainsKey(key))
                {
                    return false;
                }
                Descriptions.Add(key, text);
                return true;
            }
        }

        /// <summary>
        /// Transfer data to this network server
        /// </summary>
        /// <param name="data"></param>
        /// <param name="_priority"></param>
        public void Transfer(string data, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            if (data != "")
            {
                _Protocol.Transfer(data, _priority, this);
            }
        }

        /// <summary>
        /// Disconnect you from network
        /// </summary>
        public void Disconnect()
        {
            if (ParentSv != null)
            {
                ParentSv.Disconnect(this);
            }
            else if (_Protocol.GetType() == typeof(ProtocolIrc))
            {
                _Protocol.Exit();
            }
            else
            {
                Transfer("QUIT :" + Quit);
            }
        }

        /// <summary>
        /// Create a new network, requires name and protocol type
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="protocol"></param>
        public Network(string Server, Protocol protocol)
        {
            try
            {
                randomuqid = Core.retrieveRandom();
                lock (Descriptions)
                {
                    if (Descriptions.Count < 1)
                    {
                        Descriptions.Add('n', "no /knock is allowed on channel");
                        Descriptions.Add('r', "registered channel");
                        Descriptions.Add('m', "talking is restricted");
                        Descriptions.Add('i', "users need to be invited to join");
                        Descriptions.Add('s', "channel is secret (doesn't appear on list)");
                        Descriptions.Add('p', "channel is private");
                        Descriptions.Add('A', "admins only");
                        Descriptions.Add('O', "opers chan");
                        Descriptions.Add('t', "topic changes can be done only by operators");
                    }
                }
                _Protocol = protocol;
                ServerName = Server;
                Quit = Configuration.UserData.quit;
                Nickname = Configuration.UserData.nick;

                UserName = Configuration.UserData.user;
                Ident = Configuration.UserData.ident;
                if (protocol.GetType() == typeof(ProtocolSv))
                {
                    protocol.CreateChat("!" + ServerName, false, this, false, false, "!" + randomuqid + ServerName);
                    SystemWindow = protocol.Windows["!" + randomuqid + ServerName];
                    Core._Main.ChannelList.insertNetwork(this, (ProtocolSv)protocol);
                }
                else
                {
                    protocol.CreateChat("!system", true, this);
                    SystemWindow = protocol.Windows["!system"];
                    Core._Main.ChannelList.insertNetwork(this);
                }
                Hooks._Network.CreatingNetwork(this);
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
        }
    }
}