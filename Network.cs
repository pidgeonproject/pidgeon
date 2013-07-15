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

namespace Client
{
    /// <summary>
    /// Instance of irc network, this class is typically handled by protocols
    /// </summary>
    public class Network
    {
        /// <summary>
        /// Highlight
        /// </summary>
        public class Highlighter
        {
            /// <summary>
            /// Is simple
            /// </summary>
            public bool simple;
            /// <summary>
            /// Text
            /// </summary>
            public string text;
            /// <summary>
            /// Enabled
            /// </summary>
            public bool enabled;
            /// <summary>
            /// Expression
            /// </summary>
            public System.Text.RegularExpressions.Regex regex = null;

            /// <summary>
            /// Creates a new
            /// </summary>
            public Highlighter()
            {
                simple = true;
                enabled = false;
                text = Configuration.UserData.user;
            }
        }

        /// <summary>
        /// Information about the channel for list
        /// 
        /// This is not a class for channels, only the list
        /// </summary>
        [Serializable]
        public class ChannelData
        {
            /// <summary>
            /// Name
            /// </summary>
            public string ChannelName = null;
            /// <summary>
            /// Number of users
            /// </summary>
            public uint UserCount = 0;
            /// <summary>
            /// Topic of a channel
            /// </summary>
            public string ChannelTopic = null;

            /// <summary>
            /// Creates a new instance
            /// </summary>
            /// <param name="Users">Number of users</param>
            /// <param name="Name"></param>
            /// <param name="Topic"></param>
            public ChannelData(uint Users, string Name, string Topic)
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
        /// Message that is shown to users when you are away
        /// </summary>
        public string AwayMessage = null;
        /// <summary>
        /// User modes, these are modes that are applied on network, not channel (invisible, oper)
        /// </summary>
        public List<char> UModes = new List<char> { 'i', 'w', 'o', 'Q', 'r', 'A' };
        /// <summary>
        /// Channel user symbols (oper and such)
        /// </summary>
        public List<char> UChars = new List<char> { '~', '&', '@', '%', '+' };
        /// <summary>
        /// Channel user modes (voiced, op)
        /// </summary>
        public List<char> CUModes = new List<char> { 'q', 'a', 'o', 'h', 'v' };
        /// <summary>
        /// Channel modes (moderated, topic)
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
        public bool ParsedInfo = false;
        /// <summary>
        /// Symbol prefix of channels
        /// </summary>
        public string channel_prefix = "#";
        /// <summary>
        /// List of private message windows
        /// </summary>
        public List<User> PrivateChat = new List<User>();
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
        public bool IsSecure = false;
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
        /// If the system already attempted to change the nick
        /// </summary>
        public bool usingNick2 = false;
        /// <summary>
        /// Pointer to channel window
        /// </summary>
        private Forms.Channels wChannelList = null;
        /// <summary>
        /// Private windows
        /// </summary>
        public Dictionary<User, Graphics.Window> PrivateWins = new Dictionary<User, Graphics.Window>();
        /// <summary>
        /// Whether user is away
        /// </summary>
        public bool IsAway = false;
        /// <summary>
        /// Whether this network is fully loaded
        /// </summary>
        public bool isLoaded = false;
        /// <summary>
        /// Version of ircd running on this network
        /// </summary>
        public string IrcdVersion = null;
        private bool Connected = false;
        /// <summary>
        /// Specifies if you are connected to network
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return Connected;
            }
        }
        private bool isDestroyed = false;
        /// <summary>
        /// This will return true in case object was requested to be disposed
        /// you should never work with objects that return true here
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return isDestroyed;
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
        /// Creates a new network, requires name and protocol type
        /// </summary>
        /// <param name="Server">Server name</param>
        /// <param name="protocol">Protocol that own this instance</param>
        public Network(string Server, Protocol protocol)
        {
            try
            {
                randomuqid = Core.RetrieveRandom();
                lock (Descriptions)
                {
                    Descriptions.Clear();
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
                _Protocol = protocol;
                ServerName = Server;
                Quit = Configuration.UserData.quit;
                Nickname = Configuration.UserData.nick;

                UserName = Configuration.UserData.user;
                Ident = Configuration.UserData.ident;
                if (protocol.GetType() == typeof(ProtocolSv))
                {
                    SystemWindow = protocol.CreateChat("!" + ServerName, false, this, false, "!" + randomuqid + ServerName, false, true);
                    Core.SystemForm.ChannelList.InsertNetwork(this, (ProtocolSv)protocol);
                }
                else
                {
                    SystemWindow = protocol.CreateChat("!system", true, this, false, null, false, true);
                    Core.SystemForm.ChannelList.InsertNetwork(this);
                }
                Hooks._Network.CreatingNetwork(this);
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
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

        /// <summary>
        /// Insert a description to list
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="description"></param>
        public void InsertSafeDescription(char mode, string description)
        {
            lock (Descriptions)
            {
                if (Descriptions.ContainsKey(mode))
                {
                    Descriptions.Remove(mode);
                }

                Descriptions.Add(mode, description);
            }
        }

        /// <summary>
        /// Display server channel list
        /// </summary>
        public void DisplayChannelWindow()
        {
            try
            {
                if (wChannelList == null)
                {
                    wChannelList = new Forms.Channels(this);
                }

                wChannelList.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// This will toggle the connection flag to true
        /// </summary>
        public void flagConnection()
        {
            Connected = true;
        }

        /// <summary>
        /// This will mark the network as disconnected
        /// </summary>
        public void flagDisconnect()
        {
            Connected = false;
            lock (Channels)
            {
                foreach (Channel xx in Channels)
                {
                    // we need to change the icon to gray in side list
                    Graphics.Window cw = xx.RetrieveWindow();
                    if (cw != null)
                    {
                        cw.needIcon = true;
                    }
                    xx.UpdateInfo();
                }
            }

            lock (PrivateWins)
            {
                foreach (Graphics.Window uw in PrivateWins.Values)
                {
                    uw.needIcon = true;
                }
            }

            Graphics.PidgeonList.Updated = true;

            SystemWindow.needIcon = true;
        }

        /// <summary>
        /// Removes the channel symbols (like @ or ~) from user nick
        /// </summary>
        /// <returns>
        /// The username without char
        /// </returns>
        /// <param name='username'>
        /// Username
        /// </param>
        public string RemoveCharFromUser(string username)
        {
            foreach (char xx in UChars)
            {
                if (username.Contains(xx.ToString()))
                {
                    username = username.Replace(xx.ToString(), "");
                }
            }

            return username;
        }

        /// <summary>
        /// Retrieve information about given channel from cache of channel list
        /// </summary>
        /// <param name="channel">Channel that is about to be resolved</param>
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

        /// <summary>
        /// Part a given channel
        /// </summary>
        /// <param name="ChannelName">Channel name</param>
        public void Part(string ChannelName)
        {
            _Protocol.Part(ChannelName, this);
        }

        /// <summary>
        /// Part
        /// </summary>
        /// <param name="channel"></param>
        public void Part(Channel channel)
        {
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
            Core.SystemForm.ChannelList.insertUser(referenced_user);
            PrivateWins.Add(referenced_user, _Protocol.CreateChat(user, Configuration.UserData.SwitchWindowOnJoin, this, true, null, false, true));
            PrivateWins[referenced_user].isPM = true;
            if (Configuration.UserData.SwitchWindowOnJoin)
            {
                Core.SystemForm.ChannelList.ReselectWindow(PrivateWins[referenced_user]);
            }
            return referenced_user;
        }

        /// <summary>
        /// Reconnect a disconnected network
        /// </summary>
        public void Reconnect()
        {
            if (IsDestroyed)
            {
                return;
            }
            if (IsConnected)
            {
                SystemWindow.scrollback.InsertText(messages.Localize("[[network-reconnect-error-1]]"), ContentLine.MessageStyle.System);
                return;
            }

            _Protocol.ReconnectNetwork(this);
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="channel">Channel name which is supposed to be joined</param>
        /// <returns></returns>
        public void Join(string channel)
        {
            if (Hooks._Network.BeforeJoin(this, channel))
            {
                Transfer("JOIN " + channel, Configuration.Priority.Normal);
            }
        }

        /// <summary>
        /// Create a new instance of channel window
        /// </summary>
        /// <param name="channel">Channel</param>
        /// <param name="nf">Don't focus this new window</param>
        /// <returns>Instance of channel object</returns>
        public Channel Channel(string channel, bool nf = false)
        {
            Channel previous = getChannel(channel);
            if (previous == null)
            {
                Channel _channel = new Channel(this);
                RenderedChannel = _channel;
                _channel.Name = channel;
                Channels.Add(_channel);
                Core.SystemForm.ChannelList.InsertChannel(_channel);
                Graphics.Window window = _Protocol.CreateChat(channel, !nf, this, true);
                window.isChannel = true;
                if (!nf)
                {
                    Core.SystemForm.ChannelList.ReselectWindow(window);
                }
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
        /// <param name="key">Mode</param>
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

            Core.DebugLog("Destroying network " + ServerName);

            isDestroyed = true;

            lock (ChannelList)
            {
                ChannelList.Clear();
            }

            if (wChannelList != null)
            {
                wChannelList.Hide();
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
                foreach (Graphics.Window cw in PrivateWins.Values)
                {
                    cw._Destroy();
                }
                PrivateWins.Clear();
            }

            _Protocol = null;
            SystemWindow = null;

            lock (Descriptions)
            {
                Descriptions.Clear();
            }

            Core.SystemForm.ChannelList.RemoveServer(this);
        }

        /// <summary>
        /// Register info for channel info
        /// </summary>
        /// <param name="key">Mode</param>
        /// <param name="text">Text</param>
        /// <returns>true on success, false if this info already exist</returns>
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
            lock (this)
            {
                if (ParentSv != null)
                {
                    ParentSv.Disconnect(this);
                }
                else if (_Protocol.GetType() == typeof(ProtocolIrc))
                {
                    if (!IsConnected)
                    {
                        SystemWindow.scrollback.InsertText("You need to be connected in order to disconnect", ContentLine.MessageStyle.System);
                        return;
                    }
                    _Protocol.Disconnect();
                }
                else
                {
                    Transfer("QUIT :" + Quit);
                }
                flagDisconnect();
            }
        }
    }
}
