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

namespace Client
{
    /// <summary>
    /// Channel object
    ///
    /// Every channel that is open in pidgeon except for list should be made of this class, it is serializable because at some
    /// point we want to allow it to be copied to various domains
    /// </summary>
    [Serializable]
    public class Channel
    {
        /// <summary>
        /// Name of a channel including the special prefix, if it's unknown this variable is null
        /// </summary>
        public string Name = null;
        /// <summary>
        /// Network the channel belongs to
        /// </summary>
        [NonSerialized]
        public Network _Network = null;
        /// <summary>
        /// List of all users in current channel
        /// </summary>
        [NonSerialized]
        public List<User> UserList = new List<User>();
        /// <summary>
        /// Topic, if it's unknown this variable is null
        /// </summary>
        public string Topic = null;
        /// <summary>
        /// Whether channel is in proccess of dispose
        /// </summary>
        public bool dispose = false;
        /// <summary>
        /// User who set a topic
        /// </summary>
        public string TopicUser = "<Unknown user>";
        /// <summary>
        /// Date when a topic was set
        /// </summary>
        public int TopicDate = 0;
        /// <summary>
        /// Invites
        /// </summary>
        public List<Invite> Invites = null;
        /// <summary>
        /// List of bans set
        /// </summary>
        public List<SimpleBan> Bans = null;
        /// <summary>
        /// Exception list 
        /// </summary>
        public List<ChannelBanException> Exceptions = null;
        /// <summary>
        /// Window this channel is rendered to, if any
        /// </summary>
        [NonSerialized]
        private Graphics.Window Chat = null;
        /// <summary>
        /// If channel output is temporarily hidden
        /// </summary>
        public bool temporary_hide = false;
        /// <summary>
        /// If true the channel is processing the /who data
        /// </summary>
        public bool parsing_who = false;
        /// <summary>
        /// If true the channel is processing ban data
        /// </summary>
        public bool parsing_bans = false;
        /// <summary>
        /// If true the channel is processing exception data
        /// </summary>
        public bool parsing_xe = false;
        /// <summary>
        /// If true the channel is processing whois data
        /// </summary>
        public bool parsing_wh = false;
        /// <summary>
        /// Channel mode
        /// </summary>
        [NonSerialized]
        public NetworkMode ChannelMode = null;
        /// <summary>
        /// Whether window needs to be redrawn
        /// </summary>
        public bool Redraw = false;
        /// <summary>
        /// If true the window is considered usable, in case it's false, the window is flagged as parted channel
        /// </summary>
        public bool ChannelWork = false;
        /// <summary>
        /// If this is true user list was changed and needs to be refreshed but it can't be refreshed as it's waiting for some lock
        /// </summary>
        public bool UserListRefreshWait = false;
        /// <summary>
        /// Text displayed in the list menu
        /// </summary>
        public string MenuData = null;
        /// <summary>
        /// Whether part from this channel was requested
        /// </summary>
        public bool partRequested = false;
        /// <summary>
        /// If this is false the channel is not being used / you aren't in it or you can't access it
        /// </summary>
        public bool IsAlive
        {
            get
            {
                if (!ChannelWork)
                {
                    return false;
                }
                if (IsDestroyed)
                {
                    return false;
                }
                if (_Network != null)
                {
                    return _Network.IsConnected;
                }
                return false;
            }
        }
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
        /// Constructor (simple)
        /// </summary>
        public Channel()
        {
            ChannelWork = true;
            ChannelMode = new NetworkMode(NetworkMode.ModeType.Channel, _Network);
            Topic = "";
            TopicUser = "";
            Chat = null;
        }

        /// <summary>
        /// Constructor (normal)
        /// </summary>
        public Channel(Network network)
        {
            _Network = network;
            ChannelWork = true;
            ChannelMode = new NetworkMode(NetworkMode.ModeType.Channel, _Network);
            Topic = "";
            TopicUser = "";
            Chat = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Channel()
        {
            if (!destroyed)
            {
                Destroy();
            }
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor called for channel: " + Name);
                //Core.DebugLog("Released: " + Core.GetSizeOfObject(this).ToString() + " bytes of memory");
            }
        }

        /// <summary>
        /// Renew bans
        /// </summary>
        public void ReloadBans()
        {
            parsing_bans = true;
            if (Bans == null)
            {
                Bans = new List<SimpleBan>();
            }
            else
            {
                lock (Bans)
                {
                    Bans.Clear();
                }
            }
            _Network.Transfer("MODE " + Name + " +b");
        }

        /// <summary>
        /// Recreate information in side menu
        /// </summary>
        public void UpdateInfo()
        {
            if (IsDestroyed)
            {
                return;
            }
            string text = "";
            string trimmed = Topic;
            if (trimmed.Length > 160)
            {
                if (trimmed.Contains(" "))
                {
                    int space = 0;
                    space = 160 + trimmed.Substring(160).IndexOf(" ");
                    trimmed = trimmed.Substring(0, space) + Environment.NewLine + trimmed.Substring(space);
                }
            }
            if (!IsAlive)
            {
                text = "[PARTED CHAN] ";
            }
            text += Name + " " + UserList.Count + " users, mode: " + ChannelMode.ToString() +
                Environment.NewLine + "Topic: " + trimmed + Environment.NewLine + "Last activity: " + DateTime.Now.ToString();
            MenuData = Core.NormalizeHtml(Core.RemoveSpecial(text));
        }

        /// <summary>
        /// Return true if channel contains the given user name
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool ContainsUser(string user)
        {
            if (UserFromName(user) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if channel contains the given user name
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Obsolete]
        public bool containsUser(string user)
        {
            return ContainsUser(user);
        }

        /// <summary>
        /// Part this channel
        /// </summary>
        public void Part()
        {
            if (IsAlive && _Network != null)
            {
                if (!Hooks._Network.BeforePart(_Network, this))
                {
                    return;
                }
                _Network.Part(this);
                partRequested = true;
            }
            else
            {
                Core.SystemForm.Chat.scrollback.InsertText("This channel isn't working", ContentLine.MessageStyle.System, false);
            }
        }

        /// <summary>
        /// Return true if a channel is matching ban (exact, not a mask)
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public bool ContainsBan(string host)
        {
            lock (Bans)
            {
                foreach (SimpleBan name in Bans)
                {
                    if (name.Target == host)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if a channel is matching ban (exact, not a mask)
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        [Obsolete]
        public bool containsBan(string host)
        {
            return ContainsBan(host);
        }

        /// <summary>
        /// Thread safe, redraw all users in the user list in window, if exist
        /// </summary>
        public void RedrawUsers()
        {
            try
            {
                Core.Profiler profiler = null;
                if (Configuration.Kernel.Profiler)
                {
                    profiler = new Core.Profiler("Channel.redrawUsers()");
                }
                if (IsDestroyed)
                {
                    return;
                }
                if (Core._KernelThread == System.Threading.Thread.CurrentThread)
                {
                    Redraw = false;
                    RetrieveWindow();
                    List<User> owners = new List<User>();
                    List<User> admins = new List<User>();
                    List<User> oper = new List<User>();
                    List<User> halfop = new List<User>();
                    List<User> vs = new List<User>();
                    List<User> users = new List<User>();
                    bool Inserted;
                    Core.SystemForm.UpdateStatus();
                    if (Chat != null && Chat.isInitialised)
                    {
                        if (Chat.IsDestroyed)
                        {
                            if (Configuration.Kernel.Profiler)
                            {
                                profiler.Done();
                            }
                            return;
                        }
                        if (Chat.Locked)
                        {
                            Redraw = true;
                            Graphics.PidgeonList.Updated = true;
                            UserListRefreshWait = true;
                            if (Configuration.Kernel.Profiler)
                            {
                                profiler.Done();
                            }
                            return;
                        }
                        lock (UserList)
                        {
                            foreach (var nick in UserList)
                            {
                                Inserted = false;
                                if (nick.ChannelMode._Mode.Count > 0)
                                {
                                    lock (_Network.CUModes)
                                    {
                                        foreach (char mode in _Network.CUModes)
                                        {
                                            if (nick.ChannelMode._Mode.Contains(mode.ToString()))
                                            {
                                                switch (mode)
                                                {
                                                    case 'q':
                                                        owners.Add(nick);
                                                        Inserted = true;
                                                        break;
                                                    case 'a':
                                                        admins.Add(nick);
                                                        Inserted = true;
                                                        break;
                                                    case 'o':
                                                        Inserted = true;
                                                        oper.Add(nick);
                                                        break;
                                                    case 'h':
                                                        Inserted = true;
                                                        halfop.Add(nick);
                                                        break;
                                                    case 'v':
                                                        vs.Add(nick);
                                                        Inserted = true;
                                                        break;
                                                }
                                                if (!Inserted)
                                                {
                                                    users.Add(nick);
                                                    Inserted = true;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (Inserted == true)
                                {
                                    continue;
                                }
                                users.Add(nick);
                            }
                        }

                        Chat.UserList.Clear();
                        owners.Sort();
                        admins.Sort();
                        halfop.Sort();
                        oper.Sort();
                        vs.Sort();
                        users.Sort();

                        foreach (User user in owners)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ConvertToInfoString());
                            user.Status = User.ChannelStatus.Owner;
                        }

                        foreach (User user in admins)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ConvertToInfoString());
                            user.Status = User.ChannelStatus.Admin;
                        }

                        foreach (User user in oper)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ConvertToInfoString());
                            user.Status = User.ChannelStatus.Op;
                        }

                        foreach (User user in halfop)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ConvertToInfoString());
                            user.Status = User.ChannelStatus.Halfop;
                        }

                        foreach (User user in vs)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ConvertToInfoString());
                            user.Status = User.ChannelStatus.Voice;
                        }

                        foreach (User user in users)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ConvertToInfoString());
                            user.Status = User.ChannelStatus.Regular;
                        }
                    }
                    if (Configuration.Kernel.Profiler)
                    {
                        profiler.Done();
                    }
                    return;
                }

                Redraw = true;
                Graphics.PidgeonList.Updated = true;
                if (Configuration.Kernel.Profiler)
                {
                    profiler.Done();
                }
                return;
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        /// <summary>
        /// Thread safe, redraw all users in the user list in window, if exist
        /// </summary>
        [Obsolete]
        public void redrawUsers()
        {
            RedrawUsers();
        }


        /// <summary>
        /// Destroy this class, be careful, it can't be used in any way after you
        /// call this
        /// </summary>
        public void Destroy()
        {
            if (IsDestroyed)
            {
                // prevent this from being called multiple times
                return;
            }

            destroyed = true;

            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destroying channel " + Name);
            }

            Core.SystemForm.ChannelList.RemoveChannel(this);

            lock (UserList)
            {
                UserList.Clear();
            }

            Chat = null;
            ChannelWork = false;
            _Network = null;

            if (Invites != null)
            {
                lock (Invites)
                {
                    Invites.Clear();
                }
            }

            if (Exceptions != null)
            {
                lock (Exceptions)
                {
                    Exceptions.Clear();
                }
            }

            if (Bans != null)
            {
                lock (Bans)
                {
                    Bans.Clear();
                }
            }
        }

        /// <summary>
        /// This function returns a special user mode for a user that should be in user list (for example % for halfop or @ for operator)
        /// </summary>
        /// <param name="nick"></param>
        /// <returns></returns>
        private string uchr(User nick)
        {
            if (nick.ChannelMode._Mode.Count < 1)
            {
                return "";
            }
            lock (_Network.CUModes)
            {
                foreach (char mode in _Network.CUModes)
                {
                    int _m;
                    if (nick.ChannelMode._Mode.Contains(mode.ToString()))
                    {
                        _m = _Network.CUModes.IndexOf(mode);
                        if (_Network.UChars.Count >= _m)
                        {
                            return _Network.UChars[_m].ToString();
                        }
                        break;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Insert ban to a ban list, this will not set a ban to channel, this will only set it into memory of pidgeon
        /// </summary>
        /// <param name="ban">Host</param>
        /// <param name="user">User who set</param>
        /// <param name="time">Time when it was set</param>
        public void InsertBan(string ban, string user, string time = "0")
        {
            SimpleBan br = new SimpleBan(user, ban, time);
            lock (Bans)
            {
                Bans.Add(br);
            }
        }

        /// <summary>
        /// Removes a ban where target is matching "ban" this needs to be perfect match (a != A and x* != xX) you can't use mask
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public bool RemoveBan(string ban)
        {
            SimpleBan br = null;
            lock (Bans)
            {
                foreach (SimpleBan xx in Bans)
                {
                    if (xx.Target == ban)
                    {
                        br = xx;
                        break;
                    }
                }

                if (br != null)
                {
                    Bans.Remove(br);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return user object if specified user exist - replaced UserFromName(string name)
        /// </summary>
        /// <param name="name">User name</param>
        /// <returns></returns>
        [Obsolete]
        public User userFromName(string name)
        {
            return UserFromName(name);
        }

        /// <summary>
        /// Return user object if specified user exist
        /// </summary>
        /// <param name="name">User name</param>
        /// <returns></returns>
        public User UserFromName(string name)
        {
            foreach (User item in UserList)
            {
                if (name.ToLower() == item.Nick.ToLower())
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve window
        /// </summary>
        /// <returns></returns>
        public Graphics.Window RetrieveWindow()
        {
            if (Chat == null)
            {
                if (_Network == null)
                {
                    throw new Exception("Network is NULL for " + Name);
                }
                if (_Network._Protocol == null)
                {
                    throw new Exception("Protocol is NULL for " + _Network.ServerName);
                }
                lock (_Network._Protocol.Windows)
                {
                    foreach (var curr in _Network._Protocol.Windows)
                    {
                        if (curr.Key == _Network.SystemWindowID + Name)
                        {
                            this.Chat = curr.Value;
                            return curr.Value;
                        }
                    }
                }
            }
            return Chat;
        }

        /// <summary>
        /// Retrieve window - replaced by RetrieveWindow()
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public Graphics.Window retrieveWindow()
        {
            return RetrieveWindow();
        }
    }
}
