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
    public class Network
    {
        /// <summary>
        /// User modes
        /// </summary>
        public List<char> UModes = new List<char> { 'i', 'w', 'o', 'Q', 'r', 'A' };
        /// <summary>
        /// Channel user
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

        public bool parsed_info = false;
        public string channel_prefix = "#";
        /// <summary>
        /// Specifies if you are connected to network
        /// </summary>
        public bool Connected;
        /// <summary>
        /// List of private message windows
        /// </summary>
        public List<User> PrivateChat = new List<User>();
        /// <summary>
        /// System window
        /// </summary>
        public Window system;
        /// <summary>
        /// Host name of server
        /// </summary>
        public string server;
        /// <summary>
        /// User mode of current user
        /// </summary>
        public NetworkMode usermode = new NetworkMode();
        /// <summary>
        /// User name (real name)
        /// </summary>
        public string username;
        /// <summary>
        /// Randomly generated ID for this network to make it unique in case some other network would share the name
        /// </summary>
        public string randomuqid;
        /// <summary>
        /// List of all channels on network
        /// </summary>
        public List<Channel> Channels = new List<Channel>();
        /// <summary>
        /// Currently rendered channel on main window
        /// </summary>
        public Channel RenderedChannel = null;
        public string nickname;
        public string ident;
        /// <summary>
        /// Name of system window
        /// </summary>
        //public string sw;
        public string quit;
        /// <summary>
        /// Protocol
        /// </summary>
        public Protocol _protocol;
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
        private Channels wChannelList = null;

        public void DisplayChannelWindow()
        {
            if (wChannelList == null || wChannelList.IsDisposed)
            {
                wChannelList = new Channels(this);
            }

            wChannelList.Show();
        }

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
        }

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

        public class Highlighter
        {
            public bool simple;
            public string text;
            public bool enabled;
            public System.Text.RegularExpressions.Regex regex;
            public Highlighter()
            {
                simple = true;
                enabled = false;
                text = Configuration.user;
            }
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
            foreach (Channel cu in Channels)
            {
                if (cu.Name == name)
                {
                    return cu;
                }
            }
            return null;
        }

        /// <summary>
        /// Create private message to user
        /// </summary>
        /// <param name="user">User name</param>
        public void Private(string user)
        {
            Core._Main.userToolStripMenuItem.Visible = true;
            User u = new User(user, "", this, "");
            PrivateChat.Add(u);
            Core._Main.ChannelList.insertUser(u);
            _protocol.CreateChat(user, true, this, true);
            return;
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="channel">Channel name which is supposed to be joined</param>
        /// <param name="nf">Whether newly created window should be displayed over existing windows</param>
        /// <returns></returns>
        public void Join(string channel)
        {
            _protocol.Transfer("JOIN " + channel, Configuration.Priority.Normal, this);
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
                _protocol.CreateChat(channel, !nf, this, true, true);
                return _channel;
            }
            else
            {
                return previous;
            }
        }

        /// <summary>
        /// Window ID of this network system window
        /// </summary>
        public string window
        {
            get 
              {
                  if (_protocol.ProtocolType != 3)
                  {
                      return "system";
                  }
                  else
                  {
                      return randomuqid + server;
                  }
              }
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
                _protocol.Transfer(data, _priority, this);
            }
        }

        /// <summary>
        /// Create a new network, requires name and protocol type
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="protocol"></param>
        public Network(string Server, Protocol protocol)
        {
            randomuqid = Core.retrieveRandom();
            try
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
                _protocol = protocol;
                server = Server;
                quit = Configuration.quit;
                nickname = Configuration.nick;

                username = Configuration.user;
                ident = Configuration.ident;
                if (protocol.ProtocolType == 3)
                {
                    _protocol.CreateChat("!" + server, false, this, false, false, "!" + randomuqid + server);
                    system = _protocol.windows["!" + randomuqid + server];
                    //sw = "!" + randomuqid + server;
                    Core._Main.ChannelList.insertNetwork(this, (ProtocolSv)protocol);
                }
                else
                {
                    _protocol.CreateChat("!system", true, this);
                    //sw = "!system";
                    system = _protocol.windows["!system"];
                    Core._Main.ChannelList.insertNetwork(this);
                }
                Hooks.CreatingNetwork(this);
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
        }
    }
}
