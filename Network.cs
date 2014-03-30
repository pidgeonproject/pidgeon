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
using System.Linq;
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
                SystemWindow = WindowsManager.CreateChat("!" + ServerName, false, this, false, "!" +
                                                         RandomuQID + ServerName, false, true, this);
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

        public Graphics.Window GetPrivateUserWindow(string nickname)
        {
            lock (this.PrivateWins)
            {
                User user = this.GetUser(nickname);
                if (user == null)
                {
                    // we didn't have a conversion so far, let's make a new window for this user
                    user = new User(nickname, this);
                    Graphics.Window window = WindowsManager.CreateChat(nickname, false, this, false, nickname, false, true, this);
                    this.PrivateWins.Add(user, window);
                    window.IsPrivMsg = true;
                    Core.SystemForm.ChannelList.insertUser(user);
                    return window;
                }
                else
                {
                    return this.PrivateWins[user];
                }
            }
        }
  
        public void RemoveUserWindow(User user)
        {
            Graphics.Window window = null;
            lock (this.PrivateWins)
            {
                if (this.PrivateWins.ContainsKey(user))
                {
                    window = this.PrivateWins[user];
                    this.PrivateWins.Remove(user);
                } else
                {
                    return;
                }
            }
            if (WindowsManager.CurrentWindow == window)
            {
                Core.SystemForm.SwitchRoot();
            }
            WindowsManager.UnregisterWindow(user.Nick, this);
            window._Destroy ();
        }
           
        
        public User GetUser(string nickname)
        {
            nickname = nickname.ToLower();
            lock (this.PrivateWins)
            {
                foreach (User user in PrivateWins.Keys)
                {
                    if (user.Nick.ToLower() == nickname)
                    {
                        return user;
                    }
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
        [Obsolete ("Use GetPrivateUserWindow instead")]
        public User Private(string user)
        {
            lock (this.PrivateWins)
            {
                this.GetPrivateUserWindow(user);
                return GetUser(user);
            }
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

        public override void __evt_PRIVMSG(libirc.Network.NetworkPRIVMSGEventArgs args)
        {
            if (String.IsNullOrEmpty(args.ChannelName))
            {
                libirc.User user = new libirc.User(args.Source);
                Graphics.Window window = this.GetPrivateUserWindow(user.Nick);
                if (args.IsAct)
                {
                    window.scrollback.InsertText(args.Message, ContentLine.MessageStyle.Action, true, args.Date, false);
                } else
                {
                    window.scrollback.InsertText(args.Message, ContentLine.MessageStyle.Message, true, args.Date, false);
                }
            }
            else
            {
                Channel channel = this.GetChannel(args.ChannelName);
                if (args.SourceUser == null)
                {
                    args.SourceUser = new libirc.User(args.Source);
                }
                if (channel != null)
                {
                    Graphics.Window window = channel.RetrieveWindow();
                    if (window != null)
                    {
                        if (!args.IsAct)
                        {
                            window.scrollback.InsertText(Protocol.PRIVMSG(args.SourceUser.Nick, args.Message),
                                                         ContentLine.MessageStyle.Message, true,
                                                         args.Date, false);
                        } else
                        {
                            window.scrollback.InsertText(Configuration.CurrentSkin.Message2 + args.SourceUser.Nick + " " +
                                                         args.Message, ContentLine.MessageStyle.Action,
                                                         true, args.Date, false);
                        }
                    } else
                    {
                        this.SystemWindow.scrollback.InsertText("Message to channel you aren't in (" + args.ChannelName +
                                                                ")" + args.Message, ContentLine.MessageStyle.Message,
                                                                true, args.Date, false);
                    }
                } else
                {
                    this.SystemWindow.scrollback.InsertText("Message to channel you aren't in (" + args.ChannelName +
                                                                ")" + args.Message, ContentLine.MessageStyle.Message,
                                                                true, args.Date, false);
                }
            }
        }
        
        public override void __evt_NOTICE (libirc.Network.NetworkNOTICEEventArgs args)
        {
            Graphics.Window window = null;
            if (args.SourceUser == null)
            {
                args.SourceUser = new libirc.User(args.Source);
            }
            if (args.ParameterLine.StartsWith(this.ChannelPrefix))
            {
                Channel channel = this.GetChannel(args.ParameterLine);
                if (channel != null)
                {
                    window = channel.RetrieveWindow();
                }
            }
            if (window == null) window = this.SystemWindow;
            window.scrollback.InsertText("[" + args.SourceUser.Nick + "] " + args.Message,ContentLine.MessageStyle.Message, true,
                                         args.Date, false);
        }
        
        public override void __evt_ChannelInfo (libirc.Network.NetworkChannelDataEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null && args.Parameters.Count > 1)
            {
                Graphics.Window window = channel.RetrieveWindow();
                window.scrollback.InsertText("Mode: " + args.Parameters[2], ContentLine.MessageStyle.Channel, true, args.Date, false);
                channel.UpdateInfo();
            }
        }
        
        public override void __evt_ParseUser (libirc.Network.NetworkParseUserEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                User user = channel.UserFromName(args.User.Nick);
                if (user != null)
                {
                    user.Nick = args.User.Nick;
                    user.Ident = args.User.Ident;
                    user.Host = args.User.Host;
                    user.LastAwayCheck = DateTime.Now;
                    user.RealName = args.RealName;
                    user.Away = args.IsAway;
                } else
                {
                    user.Away = args.IsAway;
                    user.RealName = args.RealName;
                    user = new User(args.User, this);
                    channel.InsertUser(user);
                }
            }
            if (args.Channel != null && args.Channel.IsParsingWhoData && !Configuration.Kernel.HidingParsed)
            {
                this.SystemWindow.scrollback.InsertText(args.ServerLine, ContentLine.MessageStyle.System, true, args.Date, false);
            }
        }
        
        public override void __evt_ChannelUserList (libirc.Network.ChannelUserListEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (args.Channel != null && channel != null)
            {
                channel.ImportData(args.Channel);
                channel.RedrawUsers();
            }
        }
        
        public override void __evt_FinishChannelParseUser (libirc.Network.NetworkChannelDataEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            channel.UpdateInfo();
            channel.RedrawUsers();
        }
        
        public override void __evt_KICK (libirc.Network.NetworkKickEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                Graphics.Window window = channel.RetrieveWindow();
                if (window != null)
                {
                    window.scrollback.InsertText(messages.get("userkick", Core.SelectedLanguage,
                                                 new List<string> { args.Source, args.Target, args.Message }),
                                                 ContentLine.MessageStyle.Join, true, args.Date, false);
                    channel.RemoveUser(args.Target);
                }
            }
        }
        
        public override void __evt_PART (libirc.Network.NetworkChannelEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                Graphics.Window w = channel.RetrieveWindow();
                User user = channel.UserFromName(args.SourceInfo.Nick);
                if (user != null)
                {
                    if ( w != null)
                    {
                        w.scrollback.InsertText(messages.get("window-p1", Core.SelectedLanguage,
                               new List<string> { "%L%" + user.Nick + "%/L%!%D%" + user.Ident + "%/D%@%H%" + user.Host + "%/H%" }),
                               ContentLine.MessageStyle.Part, true, args.Date, false);
                    }
                    channel.RemoveUser(user);
                    channel.RedrawUsers();
                    channel.UpdateInfo();
                }
            }
        }
        
        public override void __evt_JOIN (libirc.Network.NetworkChannelEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                Graphics.Window w = channel.RetrieveWindow();
                User user = new User(args.Source, this);
                if (w != null)
                {
                    w.scrollback.InsertText(messages.get("join", Core.SelectedLanguage,
                               new List<string> { "%L%" + user.Nick + "%/L%!%D%" + user.Ident + "%/D%@%H%" + user.Host + "%/H%" }),
                               ContentLine.MessageStyle.Join, true, args.Date, false);
                }
                channel.InsertUser(user);
                channel.RedrawUsers();
                channel.UpdateInfo();
            }
        }
        
        public override void __evt_NICK (libirc.Network.NetworkNICKEventArgs args)
        {
            lock (this.Channels)
            {
                foreach (Channel channel in this.Channels.Values)
                {
                    User user = channel.UserFromName(args.SourceInfo.Nick);
                    if (user != null)
                    {
                        Graphics.Window window = channel.RetrieveWindow();
                        if (window != null)
                        {
                            window.scrollback.InsertText(messages.get ("protocol-nick", Core.SelectedLanguage,
                                                           new List<string> { args.OldNick, args.NewNick }),
                                                           Pidgeon.ContentLine.MessageStyle.Channel,
                                                           !channel.TemporarilyHidden, args.Date, false);
                        }
                        user.Nick = args.NewNick;
                        channel.RedrawUsers();
                    }
                }
            }
        }
        
        public override void __evt_MODE (libirc.Network.NetworkMODEEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                foreach (libirc.SimpleMode m in args.FormattedMode.getMode)
                {
                    if (this.CUModes.Contains(m.Mode) && m.ContainsParameter)
                    {
                        User flagged_user = channel.UserFromName(m.Parameter);
                        if (flagged_user != null)
                        {
                            flagged_user.ChannelMode.ChangeMode("+" + m.Mode);
                            flagged_user.ResetMode();
                        }
                    }

                    if (m.ContainsParameter)
                    {
                        switch (m.Mode.ToString())
                        {
                            case "b":
                                if (channel.Bans == null)
                                {
                                    channel.Bans = new List<libirc.SimpleBan>();
                                }
                                lock (channel.Bans)
                                {
                                    channel.Bans.Add(new libirc.SimpleBan(args.Source, m.Parameter, ""));
                                }
                                break;
                        }
                    }
                }

                foreach (libirc.SimpleMode m in args.FormattedMode.getRemovingMode)
                {
                    if (this.CUModes.Contains(m.Mode) && m.ContainsParameter)
                    {
                        User flagged_user = channel.UserFromName(m.Parameter);
                        if (flagged_user != null)
                        {
                            flagged_user.ChannelMode.ChangeMode("-" + m.Mode);
                            flagged_user.ResetMode();
                        }
                    }

                    if (m.ContainsParameter)
                    {
                        switch (m.Mode.ToString())
                        {
                            case "b":
                                if (channel.Bans == null)
                                {
                                    channel.Bans = new List<libirc.SimpleBan>();
                                }
                                channel.RemoveBan(m.Parameter);
                                break;
                        }
                    }
                }
            }
        }
        
        public override void __evt_QUIT (libirc.Network.NetworkGenericDataEventArgs args)
        {
            lock (this.Channels)
            {
                foreach (Channel channel in this.Channels.Values)
                {
                    User user_ = channel.UserFromName(args.SourceInfo.Nick);
                    Graphics.Window window_ = channel.RetrieveWindow();
                    if (window_ != null && user_ != null)
                    {
                        window_.scrollback.InsertText(messages.get("protocol-quit", Core.SelectedLanguage,
                                                      new List<string> { "%L%" + args.SourceInfo.Nick + "%/L%!%D%" +
                                                      args.SourceInfo.Ident + "%/D%@%H%" + args.SourceInfo.Host + "%/H%", args.Message }),
                                                      ContentLine.MessageStyle.Join, true, args.Date, false);
                    }
                    channel.RemoveUser(args.SourceInfo.Nick);
                    channel.UpdateInfo();
                    channel.RedrawUsers();
                }
            }
        }
        
        public override void __evt_INFO (libirc.Network.NetworkGenericDataEventArgs args)
        {
            string parameter_line = args.ParameterLine;
            if (parameter_line.Contains("PREFIX=("))
            {
                string cmodes = parameter_line.Substring(parameter_line.IndexOf("PREFIX=(", StringComparison.Ordinal) + 8);
                cmodes = cmodes.Substring(0, cmodes.IndexOf(")", StringComparison.Ordinal));
                lock (this.CUModes)
                {
                    this.CUModes.Clear();
                    this.CUModes.AddRange(cmodes.ToArray<char>());
                }
                cmodes = parameter_line.Substring(parameter_line.IndexOf("PREFIX=(", StringComparison.Ordinal) + 8);
                cmodes = cmodes.Substring(cmodes.IndexOf(")", StringComparison.Ordinal) + 1, this.CUModes.Count);

                this.UChars.Clear();
                this.UChars.AddRange(cmodes.ToArray<char>());
            }
            if (parameter_line.Contains("CHANMODES="))
            {
                string xmodes = parameter_line.Substring(parameter_line.IndexOf("CHANMODES=", StringComparison.Ordinal) + 11);
                xmodes = xmodes.Substring(0, xmodes.IndexOf(" ", StringComparison.Ordinal));
                string[] _mode = xmodes.Split(',');
                this.ParsedInfo = true;
                if (_mode.Length == 4)
                {
                    this.PModes.Clear();
                    this.CModes.Clear();
                    this.XModes.Clear();
                    this.SModes.Clear();
                    this.PModes.AddRange(_mode[0].ToArray<char>());
                    this.XModes.AddRange(_mode[1].ToArray<char>());
                    this.SModes.AddRange(_mode[2].ToArray<char>());
                    this.CModes.AddRange(_mode[3].ToArray<char>());
                }
            }
        }
    }
}
