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
    public class Network : libirc.Network
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
        public Channel RenderedChannel = null;
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
        /// Descriptions for channel and user modes
        /// </summary>
        public Dictionary<char, string> Descriptions = new Dictionary<char, string>();
        /// <summary>
        /// Network cache that is used to handle the problem with users who part but we don't have
        /// them in user list yet, because that is handled by services
        /// </summary>
        private Dictionary<string, List<string>> ServiceBuffer = new Dictionary<string, List<string>>();

        public override bool IsConnected
        {
            get
            {
                return base.IsConnected;
            }
            set
            {
                base.IsConnected = value;
                this.SystemWindow.NeedsIcon = true;
                this.ServiceBuffer.Clear();
                lock (this.Channels)
                {
                    foreach (Channel channel in this.Channels.Values)
                    {
                        Graphics.Window window = channel.RetrieveWindow();
                        if (window != null)
                        {
                            window.NeedsIcon = true;
                        }
                    }
                }
                lock (this.PrivateWins)
                {
                    foreach (Graphics.Window user in this.PrivateWins.Values)
                    {
                        user.NeedsIcon = true;
                    }
                }
            }
        }
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

        private void RegisterUser(string channel, string user)
        {
            lock (ServiceBuffer)
            {
                if (!ServiceBuffer.ContainsKey(channel))
                {
                    ServiceBuffer.Add(channel, new List<string>() { user });
                    return;
                }
                if (!ServiceBuffer[channel].Contains(user))
                    ServiceBuffer[channel].Add(user);
            }
        }

        private void RemoveUserFromSB(string channel, string user)
        {
            lock (this.ServiceBuffer)
            {
                if (!this.ServiceBuffer.ContainsKey(channel))
                {
                    return;
                }
                //if (this.ServiceBuffer[channel].Contains(user))
                    this.ServiceBuffer[channel].Remove(user);
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
            Hooks._Network.CreatingNetwork(this);
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
                    Core.SystemForm.ChannelList.InsertUser(user);
                    return window;
                }
                else
                {
                    return this.PrivateWins[user];
                }
            }
        }

        /// <summary>
        /// Register info for channel info
        /// </summary>
        /// <param name="key">Mode</param>
        /// <param name="text">Text</param>
        /// <returns>true on success, false if this info already exist</returns>
        public virtual bool RegisterInfo(char key, string text)
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
        /// Insert a description to list
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="description"></param>
        public void InsertSafeDescription(char mode, string description)
        {
            lock (Descriptions)
            {
                if (Descriptions.ContainsKey(mode))
                    Descriptions[mode] = description;
                else
                    Descriptions.Add(mode, description);
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
                }
                else
                {
                    return;
                }
            }
            if (WindowsManager.CurrentWindow == window)
            {
                Core.SystemForm.SwitchRoot();
            }
            WindowsManager.UnregisterWindow(user.Nick, this);
            window._Destroy();
        }

        public override void HandleUnknownData(UnknownDataEventArgs text)
        {
            this.SystemWindow.scrollback.InsertText(text.Data, Pidgeon.ContentLine.MessageStyle.System, true, text.Date, true);
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
            if (name == null)
            {
                return null;
            }
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

        public override void Act(string text, string to, libirc.Defs.Priority _priority = libirc.Defs.Priority.Normal)
        {
            switch (_Protocol.Act(text, to, this, _priority))
            {
                case libirc.IProtocol.Result.Done:
                    Core.SystemForm.Chat.scrollback.InsertText(Configuration.CurrentSkin.Message2 + Core.SelectedNetwork.Nickname + " " + text, 
                                                               Pidgeon.ContentLine.MessageStyle.Action, true, 1, true);
                    break;
                case libirc.IProtocol.Result.Failure:
                case libirc.IProtocol.Result.NotImplemented:
                    Core.SystemForm.Chat.scrollback.InsertText("Failed to deliver: " + text, ContentLine.MessageStyle.System, false, 1, false);
                    break;
            }
        }

        /// <summary>
        /// Send a message to network
        /// </summary>
        /// <param name="text">Text of message</param>
        /// <param name="to">Sending to</param>
        /// <param name="_priority">Priority</param>
        public override void Message(string text, string to, libirc.Defs.Priority _priority = libirc.Defs.Priority.Normal)
        {
            switch (_Protocol.Message(text, to, this, _priority))
            {
                case libirc.IProtocol.Result.Done:
                    Core.SystemForm.Chat.scrollback.InsertText(Protocol.PRIVMSG(this.Nickname, text), Pidgeon.ContentLine.MessageStyle.Message, true, 1, true);
                    break;
                case libirc.IProtocol.Result.Failure:
                case libirc.IProtocol.Result.NotImplemented:
                    Core.SystemForm.Chat.scrollback.InsertText("Failed to deliver: " + text, ContentLine.MessageStyle.System, false, 1, false);
                    break;
            }
        }

        public override bool RemoveChannel(string name)
        {
            name = name.ToLower();
            lock (this.Channels)
            {
                if (this.Channels.ContainsKey(name))
                {
                    this.Channels.Remove(name);
                }
            }
            return base.RemoveChannel(name);
        }

        public new Channel MakeChannel(string channel)
        {
            lock (this.Channels)
            {
                Channel ch = GetChannel(channel);
                if (ch == null)
                {
                    // we aren't in this channel, which is expected, let's create a new window for it
                    ch = new Pidgeon.Channel(this, channel);
                    this.Channels.Add(ch.LowerName, ch);
                    lock (base.Channels)
                    {
                        if (!base.Channels.ContainsKey(ch.LowerName))
                        {
                            base.Channels.Add(ch.LowerName, (libirc.Channel)ch);
                        }
                    }
                }
                return ch;
            }
        }

        protected override void __evt_Self(NetworkSelfEventArgs args)
        {
            try
            {
                Channel channel = GetChannel(args.ChannelName);
                Graphics.Window window = null;
                if (channel != null)
                    window = channel.RetrieveWindow();
                switch (args.Type)
                {
                    case libirc.Network.EventType.Join:
                        MakeChannel(args.ChannelName);
                        break;
                    case libirc.Network.EventType.Part:
                    case libirc.Network.EventType.Kick:
                    case libirc.Network.EventType.Quit:
                        if (window != null)
                            window.NeedsIcon = true;
                        break;
                    case libirc.Network.EventType.Nick:
                        this.Nickname = args.NewNick;
                        this.SystemWindow.scrollback.InsertText(messages.get("protocolnewnick", Core.SelectedLanguage, new List<string> { args.NewNick }),
                                                                  Pidgeon.ContentLine.MessageStyle.User, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                        break;
                }
            } catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        protected override void __evt_PRIVMSG(libirc.Network.NetworkPRIVMSGEventArgs args)
        {
            try
            {
                if (String.IsNullOrEmpty(args.ChannelName))
                {
                    Graphics.Window window = this.GetPrivateUserWindow(args.SourceInfo.Nick);
                    if (args.IsAct)
                    {
                        window.scrollback.InsertText(Configuration.CurrentSkin.Message2 + args.SourceInfo.Nick + " " + args.Message,
                                                     ContentLine.MessageStyle.Action, WriteLogs(),
                                                     args.Date, IsDownloadingBouncerBacklog);
                    }
                    else
                    {
                        window.scrollback.InsertText(Protocol.PRIVMSG(args.SourceInfo.Nick, args.Message), ContentLine.MessageStyle.Message,
                                                     WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
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
                                                             ContentLine.MessageStyle.Message, WriteLogs(),
                                                             args.Date, IsDownloadingBouncerBacklog);
                            }
                            else
                            {
                                window.scrollback.InsertText(Configuration.CurrentSkin.Message2 + args.SourceUser.Nick + " " +
                                                             args.Message, ContentLine.MessageStyle.Action,
                                                             WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                            }
                        }
                        else
                        {
                            this.SystemWindow.scrollback.InsertText("Message to channel you aren't in (" + args.ChannelName +
                                                                    ") " + args.Source + ": " + args.Message, ContentLine.MessageStyle.Message,
                                                                    WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                        }
                    }
                    else
                    {
                        this.SystemWindow.scrollback.InsertText("Message to channel you aren't in (" + args.ChannelName +
                                                                    ") " + args.Source + ": " + args.Message, ContentLine.MessageStyle.Message,
                                                                    WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        protected override void __evt_NOTICE(libirc.Network.NetworkNOTICEEventArgs args)
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
            window.scrollback.InsertText("[" + args.SourceUser.Nick + "] " + args.Message, ContentLine.MessageStyle.Message, WriteLogs(),
                                         args.Date, IsDownloadingBouncerBacklog);
        }

        protected override void __evt_ChannelInfo(libirc.Network.NetworkChannelDataEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null && args.Parameters.Count > 1)
            {
                Graphics.Window window = channel.RetrieveWindow();
                channel.ChannelMode.ChangeMode(args.Parameters[2]);
                window.scrollback.InsertText("Mode: " + args.Parameters[2], ContentLine.MessageStyle.Channel, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                channel.UpdateInfo();
            }
        }

        protected override void __evt_ParseUser(libirc.Network.NetworkParseUserEventArgs args)
        {
            if (IsDownloadingBouncerBacklog)
                return;
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
                    if (args.StringMode.Length > 0)
                    {
                        user.ChannelPrefix_Char = args.StringMode[0];
                    }
                }
                else
                {
                    user = new User(args.User, this);
                    user.Away = args.IsAway;
                    user.RealName = args.RealName;
                    channel.InsertUser(user);
                    if (args.StringMode.Length > 0)
                    {
                        user.ChannelPrefix_Char = args.StringMode[0];
                    }
                }
            }
            if (args.Channel != null && args.Channel.IsParsingWhoData && !Configuration.Kernel.HidingParsed)
            {
                this.SystemWindow.scrollback.InsertText(args.ServerLine, ContentLine.MessageStyle.System, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
            }
        }

        protected override void __evt_ChannelUserList(libirc.Network.ChannelUserListEventArgs args)
        {
            if (IsDownloadingBouncerBacklog)
                return;
            Channel channel = this.GetChannel(args.ChannelName);
            if (args.Channel != null && channel != null)
            {
                channel.ImportData(args.Channel);
                channel.RedrawUsers();
            }
        }

        protected override void __evt_FinishChannelParseUser(libirc.Network.NetworkChannelDataEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                channel.UpdateInfo();
                channel.RedrawUsers();
            }
        }

        protected override void __evt_KICK(libirc.Network.NetworkKickEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                Graphics.Window window = channel.RetrieveWindow();
                if (window != null)
                {
                    window.scrollback.InsertText(messages.get("userkick", Core.SelectedLanguage,
                                                 new List<string> { args.Source, args.Target, args.Message }),
                                                 ContentLine.MessageStyle.Join, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                    if (!IsDownloadingBouncerBacklog)
                    {
                        // modify this list only if we are not downloading a backlog because in that case
                        // we are changing something what may conflict with latest data
                        channel.RemoveUser(args.Target);
                    }
                }
            }
        }

        protected override void __evt_PART(libirc.Network.NetworkChannelDataEventArgs args)
        {
            RemoveUserFromSB(args.ChannelName, args.SourceInfo.Nick);
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                Graphics.Window w = channel.RetrieveWindow();
                if (w != null)
                {
                    w.scrollback.InsertText(messages.get("window-p1", Core.SelectedLanguage, new List<string> { "%L%" + args.SourceInfo.Nick + "%/L%!%D%" + args.SourceInfo.Ident + "%/D%@%H%" + args.SourceInfo.Host + "%/H%", args.Message }),
                                            ContentLine.MessageStyle.Part, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                }
                User user = channel.UserFromName(args.SourceInfo.Nick);
                if (user != null)
                {
                    if (!IsDownloadingBouncerBacklog)
                    {
                        // modify this list only if we are not downloading a backlog because in that case
                        // we are changing something what may conflict with latest data
                        channel.RemoveUser(user);
                        channel.RedrawUsers();
                        channel.UpdateInfo();
                    }
                } else
                {

                }
            }
        }

        protected override void __evt_JOIN(libirc.Network.NetworkChannelEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            this.RegisterUser(args.ChannelName, args.SourceInfo.Nick);
            if (channel != null)
            {
                Graphics.Window w = channel.RetrieveWindow();
                User user = new User(args.Source, this);
                if (w != null)
                {
                    w.scrollback.InsertText(messages.get("join", Core.SelectedLanguage,
                               new List<string> { "%L%" + user.Nick + "%/L%!%D%" + user.Ident + "%/D%@%H%" + user.Host + "%/H%" }),
                               ContentLine.MessageStyle.Join, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                }
                if (!IsDownloadingBouncerBacklog)
                {
                    // modify this list only if we are not downloading a backlog because in that case
                    // we are changing something what may conflict with latest data
                    channel.InsertUser(user);
                    channel.RedrawUsers();
                    channel.UpdateInfo();
                }
            }
        }

        protected override void __evt_NICK(libirc.Network.NetworkNICKEventArgs args)
        {
            lock (this.Channels)
            {
                foreach (Channel channel in this.Channels.Values)
                {
                    Graphics.Window window = channel.RetrieveWindow();
                    User user = channel.UserFromName(args.OldNick);
                    bool known = user != null;
                    if (!known)
                    {
                        // let's check if they are in services buffer now
                        if (this.ServiceBuffer.ContainsKey(channel.Name) && this.ServiceBuffer[channel.Name].Contains(args.OldNick))
                        {
                            known = true;
                            this.ServiceBuffer[channel.Name].Remove(args.OldNick);
                            // insert a new nick so that we can keep a track of users that are in channel
                            this.ServiceBuffer[channel.Name].Add(args.NewNick);
                        }
                    }
                    if (window != null && known)
                    {
                        window.scrollback.InsertText(messages.get("protocol-nick", Core.SelectedLanguage, new List<string> { args.OldNick, args.NewNick }),
                                                     ContentLine.MessageStyle.Channel, !channel.TemporarilyHidden, args.Date, IsDownloadingBouncerBacklog);
                    }
                    if (!IsDownloadingBouncerBacklog)
                        channel.RedrawUsers();
                }
            }
        }

        protected override void __evt_MODE(libirc.Network.NetworkMODEEventArgs args)
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
                                    channel.Bans = new List<libirc.ChannelBan>();
                                }
                                lock (channel.Bans)
                                {
                                    channel.Bans.Add(new libirc.ChannelBan(args.Source, m.Parameter, ""));
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
                                    channel.Bans = new List<libirc.ChannelBan>();
                                }
                                channel.RemoveBan(m.Parameter);
                                break;
                        }
                    }
                }
                Graphics.Window window = channel.RetrieveWindow();
                if (window != null)
                {
                    window.scrollback.InsertText(messages.get("channel-mode", Core.SelectedLanguage, new List<string> { args.Source, args.SimpleMode }),
                                                 Pidgeon.ContentLine.MessageStyle.Action, !channel.TemporarilyHidden && WriteLogs(), args.Date,
                                                 IsDownloadingBouncerBacklog);
                }
                channel.UpdateInfo();
                channel.RedrawUsers();
            }
        }

        protected override void __evt_QUIT(libirc.Network.NetworkGenericDataEventArgs args)
        {
            lock (this.Channels)
            {
                foreach (Channel channel in this.Channels.Values)
                {
                    Graphics.Window window_ = channel.RetrieveWindow();
                    User user = channel.UserFromName(args.SourceInfo.Nick);
                    bool known = user != null;
                    if (!known)
                    {
                        // let's check if they are in services buffer now
                        if (this.ServiceBuffer.ContainsKey(channel.Name) && this.ServiceBuffer[channel.Name].Contains(args.SourceInfo.Nick))
                        {
                            known = true;
                            // we need to remove them
                            this.ServiceBuffer[channel.Name].Remove(args.SourceInfo.Nick);
                        }
                    }
                    if (window_ != null && known)
                    {
                        window_.scrollback.InsertText(messages.get("protocol-quit", Core.SelectedLanguage,
                                                      new List<string> { "%L%" + args.SourceInfo.Nick + "%/L%!%D%" +
                                                      args.SourceInfo.Ident + "%/D%@%H%" + args.SourceInfo.Host + "%/H%", args.Message }),
                                                      ContentLine.MessageStyle.Join, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                    }
                    if (!IsDownloadingBouncerBacklog && known)
                    {
                        channel.RemoveUser(args.SourceInfo.Nick);
                        channel.UpdateInfo();
                        channel.RedrawUsers();
                    }
                }
            }
        }

        protected override void __evt_TOPIC(NetworkTOPICEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                if (Hooks._Network.Topic(this, args.Source, channel, args.Topic, args.Date, true))
                {
                    Graphics.Window window = channel.RetrieveWindow();
                    if (window != null)
                    {
                        window.scrollback.InsertText(messages.get("channel-topic", Core.SelectedLanguage, new List<string> { args.Source, args.Topic }),
                                                       Pidgeon.ContentLine.MessageStyle.Channel, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                    }
                    channel.Topic = args.Topic;
                    channel.TopicDate = (int)args.TopicDate;
                    channel.TopicUser = args.Source;
                    channel.UpdateInfo();
                }
            }
        }

        protected override void __evt_TopicData(libirc.Network.NetworkTOPICEventArgs args)
        {
            Channel channel = GetChannel(args.ChannelName);
            if (channel != null)
            {
                Graphics.Window window = channel.RetrieveWindow();
                if (window != null)
                {
                    window.scrollback.InsertText("Topic: " + args.Topic, Pidgeon.ContentLine.MessageStyle.Channel, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                }
                channel.Topic = args.Topic;
                channel.UpdateInfo();
            }
        }

        protected override void __evt_TopicInfo(NetworkTOPICEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel != null)
            {
                channel.TopicDate = (int)args.TopicDate;
                channel.TopicUser = args.Source;
                Graphics.Window window = channel.RetrieveWindow();
                if (window != null)
                {
                    window.scrollback.InsertText("Topic by: " + args.Source + " date " + Core.ConvertFromUNIXToString(args.TopicDate.ToString()),
                                                 Pidgeon.ContentLine.MessageStyle.Channel, WriteLogs() && !channel.TemporarilyHidden, args.Date,
                                                 IsDownloadingBouncerBacklog);
                }
                channel.UpdateInfo();
            }
        }

        private bool WriteLogs()
        {
            if (!IsDownloadingBouncerBacklog)
            {
                return true;
            }
            else
            {
                switch (Configuration.Logs.ServicesLogs)
                {
                    case Configuration.Logs.ServiceLogs.full:
                        return true;
                    case Configuration.Logs.ServiceLogs.incremental:
                    case Configuration.Logs.ServiceLogs.none:
                        return false;
                }
            }
            return false;
        }

        protected override void __evt_CTCP(NetworkCTCPEventArgs args)
        {
            string reply = null;
            switch (args.CTCP)
            {
                case "VERSION":
                    reply = "VERSION " + Configuration.Version + " build: " + RevisionProvider.GetHash() + " http://pidgeonclient.org/wiki/";
                    if (Configuration.irc.DetailedVersion)
                    {
                        reply += " operating system " + Environment.OSVersion.ToString();
                    }
                    this.Transfer("NOTICE " + args.SourceInfo.Nick + " :" + _Protocol.Separator.ToString() + reply,
                        libirc.Defs.Priority.Low);
                    break;
                case "TIME":
                    reply = "TIME " + DateTime.Now.ToString();
                    this.Transfer("NOTICE " + args.SourceInfo.Nick + " :" + _Protocol.Separator.ToString() + reply,
                        libirc.Defs.Priority.Low);
                    break;
                case "PING":
                    if (args.Message.Length > 6)
                    {
                        string time = args.Message.Substring(6);
                        if (time.Contains(this._Protocol.Separator))
                        {
                            reply = "PING " + time;
                            time = args.Message.Substring(0, args.Message.IndexOf(this._Protocol.Separator));
                            this.Transfer("NOTICE " + args.SourceInfo.Nick + " :" + _Protocol.Separator.ToString() + reply,
                                libirc.Defs.Priority.Low);
                        }
                    }
                    break;
                case "DCC":
                    string message2 = args.Message;
                    if (message2.Length < 5 || !message2.Contains(" "))
                    {
                        Syslog.DebugLog("Malformed DCC " + message2);
                        return;
                    }
                    if (message2.EndsWith(_Protocol.Separator.ToString(), StringComparison.Ordinal))
                    {
                        message2 = message2.Substring(0, message2.Length - 1);
                    }
                    string[] list = message2.Split(' ');
                    if (list.Length < 5)
                    {
                        Syslog.DebugLog("Malformed DCC " + message2);
                        return;
                    }
                    string type = list[1];
                    int port = 0;
                    if (!int.TryParse(list[4], out port))
                    {
                        Syslog.DebugLog("Malformed DCC " + message2);
                        return;
                    }
                    if (port < 1)
                    {
                        Syslog.DebugLog("Malformed DCC " + message2);
                        return;
                    }
                    switch (type.ToLower())
                    {
                        case "send":
                            break;
                        case "chat":
                            Core.OpenDCC(list[3], port, args.SourceInfo.Nick, false, false, this);
                            return;
                        case "securechat":
                            Core.OpenDCC(list[3], port, args.SourceInfo.Nick, false, true, this);
                            return;
                    }
                    Syslog.DebugLog("Malformed DCC " + message2);
                    return;
            }
        }

        protected override void __evt_ChannelFinishBan(NetworkChannelEventArgs args)
        {
            Channel channel = this.GetChannel(args.ChannelName);
            if (channel == args.Channel)
            {
                channel.UpdateInfo();
                return;
            }
            if (channel != null && args.Channel != null)
            {
                if (channel.Bans == null)
                {
                    channel.Bans = args.Channel.Bans;
                }
                else
                {
                    channel.Bans.Clear();
                    channel.Bans.AddRange(args.Channel.Bans);
                }
                channel.UpdateInfo();
            }
        }

        protected override void __evt_WHOIS(NetworkWHOISEventArgs args)
        {
            try
            {
                if (Configuration.irc.FriendlyWhois)
                {
                    string name = args.ParameterLine.Substring(args.ParameterLine.IndexOf(" ", StringComparison.Ordinal) + 1);
                    if (args.WhoisType == NetworkWHOISEventArgs.Mode.Channels)
                    {
                        this.SystemWindow.scrollback.InsertText("WHOIS " + name + " is in channels: " + args.Message, Pidgeon.ContentLine.MessageStyle.System,
                            WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                    }
                    else if (args.WhoisType == NetworkWHOISEventArgs.Mode.Info && args.Parameters.Count > 1)
                    {
                        this.SystemWindow.scrollback.InsertText("WHOIS " + name + " " + args.Message, Pidgeon.ContentLine.MessageStyle.System,
                            WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
                    }
                    else if (args.WhoisType == NetworkWHOISEventArgs.Mode.Server)
                    {
                        if (args.Parameters.Count > 2)
                        {
                            string server = "";
                            if (!String.IsNullOrEmpty(args.Message))
                            {
                                server = " (" + args.Message + ")";
                            }
                            this.SystemWindow.scrollback.InsertText("WHOIS " + args.Parameters[1] + " is using server: " + args.Parameters[2] + server, ContentLine.MessageStyle.System, WriteLogs(),
                                            args.Date, IsDownloadingBouncerBacklog);
                        }
                    }
                    else if (args.WhoisType == NetworkWHOISEventArgs.Mode.Uptime)
                    {
                        if (args.Parameters.Count > 3)
                        {
                            DateTime date = libirc.Defs.ConvertFromUNIX(args.Parameters[3]);
                            this.SystemWindow.scrollback.InsertText("WHOIS " + args.Parameters[1] + " is idle for " + args.Parameters[2] +
                                " seconds and online since " + date.ToString() + " (" + (DateTime.Now - date).ToString() + ")", ContentLine.MessageStyle.System, WriteLogs(),
                                        args.Date, IsDownloadingBouncerBacklog);
                        }
                    }
                    else if (args.WhoisType == NetworkWHOISEventArgs.Mode.Header && args.Parameters.Count > 3)
                    {
                        this.SystemWindow.scrollback.InsertText("WHOIS for " + args.Parameters[1] + " (" + args.Parameters[1] +
                                "!" + args.Parameters[2] + "@" + args.Parameters[3] + ") " + args.Message, ContentLine.MessageStyle.System, WriteLogs(),
                                args.Date, IsDownloadingBouncerBacklog);
                    }
                    else
                    {
                        this.SystemWindow.scrollback.InsertText(args.ServerLine, ContentLine.MessageStyle.System, WriteLogs(),
                                    args.Date, IsDownloadingBouncerBacklog);
                    }
                }
                else
                {
                    this.SystemWindow.scrollback.InsertText(args.ServerLine, ContentLine.MessageStyle.System, WriteLogs(),
                                    args.Date, IsDownloadingBouncerBacklog);
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        protected override void __evt_INVITE(NetworkChannelDataEventArgs args)
        {
            this.SystemWindow.scrollback.InsertText("INVITE: " + args.Source + " invites you to join " + args.ChannelName + " (click on channel name to join)",
                Pidgeon.ContentLine.MessageStyle.System, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
            if (!Configuration.irc.IgnoreInvites)
            {
                Core.DisplayNote(args.Source + " invites you to join " + args.ChannelName, "Invitation");
            }
        }

        protected override void __evt_OnMOTD(libirc.Network.NetworkGenericDataEventArgs args)
        {
            this.SystemWindow.scrollback.InsertText("MOTD: " + args.Message, ContentLine.MessageStyle.Message, WriteLogs(), args.Date, IsDownloadingBouncerBacklog);
        }

        protected override bool __evt__IncomingData(IncomingDataEventArgs args)
        {
            switch (args.Command)
            {
                case "001":
                case "002":
                case "003":
                case "004":
                    Hooks._Network.NetworkInfo(this, args.Command, args.ParameterLine, args.Message);
                    return false;
                case "005":
                    Hooks._Network.NetworkInfo(this, args.Command, args.ParameterLine, args.Message);
                    if (!this.IsLoaded)
                    {
                        Hooks._Network.AfterConnectToNetwork(this);
                    }
                    return false;
            }
            return false;
        }
    }
}
