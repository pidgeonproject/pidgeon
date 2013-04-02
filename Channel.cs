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
    public class UserData
    {
        public Gtk.TreeIter iter;
        public string username;
        public string hn;
        
        public UserData(string Name, Gtk.TreeIter Node, string HostName)
        {
            iter = Node;
            username = Name;
            hn = HostName;
        }
    }
    
    public class ChannelParameterMode
    {
        public string Target = null;
        public string Time = null;
        public string User = null;
    }

    [Serializable]
    public class Invite : ChannelParameterMode
    {
        public Invite()
        {
            // This empty constructor is here so that we can serialize this
        }

        public Invite(string user, string target, string time)
        {

        }
    }

    [Serializable]
    public class Except : ChannelParameterMode
    {
        public Except()
        {
            // This empty constructor is here so that we can serialize this
        }
    }

    [Serializable]
    public class SimpleBan : ChannelParameterMode
    {
        public SimpleBan()
        {
            // This empty constructor is here so that we can serialize this
        }

        public SimpleBan(string user, string target, string time)
        {
            Target = target;
            User = user;
            Time = time;
        }
    }

    [Serializable]
    public class Channel
    {
        /// <summary>
        /// Name of a channel including the special prefix
        /// </summary>
        public string Name;
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
        /// Topic
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
        public List<Except> Exceptions = null;
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
        /// Whether window needs to be redraw
        /// </summary>
        public bool Redraw = false;
        /// <summary>
        /// If true the window is considered usable, in case it's false, the window is flagged as parted channel
        /// </summary>
        public bool ChannelWork = false;
        /// <summary>
        /// Tree node
        /// </summary>
        public Gtk.TreeIter TreeNode;
        /// <summary>
        /// If this is true user list was changed and needs to be refreshed but it can't be refreshed as it's waiting for some lock
        /// </summary>
        public bool UserListRefreshWait = false;
        /// <summary>
        /// Text displayed in the list menu
        /// </summary>
        public string MenuData = null;

        /// <summary>
        /// Renew the bans
        /// </summary>
        public void ReloadBans()
        {
            parsing_bans = true;
            Bans.Clear();
            _Network.Transfer("MODE +b " + Name);
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

        ~Channel()
        {
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor called for channel: " + Name);
                Core.DebugLog("Released: " + System.Runtime.InteropServices.Marshal.SizeOf(this).ToString() + " bytes of memory");
            }
        }

        /// <summary>
        /// Recreate information in side menu
        /// </summary>
        public void UpdateInfo()
        {
            string text = "";
            string trimmed = Topic;
            if (trimmed.Length > 160)
            {
                if (trimmed.Contains(" "))
                {
                    int space = 0;
                    space = 160 + trimmed.Substring(160).IndexOf(" ");
                    trimmed = trimmed.Substring(0, space) + "\n" + trimmed.Substring(space);
                }
            }
            if (!ChannelWork)
            {
                text = "[PARTED CHAN] ";

            }
            text += Name + " " + UserList.Count + " users, mode: " + ChannelMode.ToString() + "\n" + "Topic: " + trimmed + "\nLast activity: " + DateTime.Now.ToString();
            MenuData = Core.normalizeHtml(text);
        }

        /// <summary>
        /// Return true if channel contains the given user name
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool containsUser(string user)
        {
            if (userFromName(user) != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if a channel is matching ban (exact, not a mask)
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public bool containsBan(string host)
        {
            lock (Bans)
            {
                foreach (var name in Bans)
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
        /// Thread safe, redraw all users in the user list in window, if exist
        /// </summary>
        public void redrawUsers()
        {
            try
            {
                if (Core._KernelThread == System.Threading.Thread.CurrentThread)
                {
                    Redraw = false;
                    retrieveWindow();
                    List<User> owners = new List<User>();
                    List<User> admins = new List<User>();
                    List<User> oper = new List<User>();
                    List<User> halfop = new List<User>();
                    List<User> vs = new List<User>();
                    List<User> users = new List<User>();
                    bool Inserted;
                    Core._Main.UpdateStatus();
                    if (Chat != null && Chat.isInitialised)
                    {
                        if (Chat.Locked)
                        {
                            Redraw = true;
                            Graphics.PidgeonList.Updated = true;
                            UserListRefreshWait = true;
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
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ToString());
                            user.Status = User.ChannelStatus.Owner;
                        }

                        foreach (User user in admins)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ToString());
                            user.Status = User.ChannelStatus.Admin;
                        }
                        foreach (User user in oper)
                        {
                             Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ToString());
                            user.Status = User.ChannelStatus.Op;
                        }
                        foreach (User user in halfop)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ToString());
                            user.Status = User.ChannelStatus.Halfop;
                        }
                        foreach (User user in vs)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ToString());
                            user.Status = User.ChannelStatus.Voice;
                        }

                        foreach (User user in users)
                        {
                            Chat.UserList.AppendValues(uchr(user) + user.Nick, user, user.ToString());
                            user.Status = User.ChannelStatus.Regular;
                        }
                    }
                    return;
                }

                Redraw = true;
                Graphics.PidgeonList.Updated = true;
                return;
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }
        /// <summary>
		/// Destroy this class, be careful, it can't be used in any way after you
		/// call this
		/// </summary>
		public void Destroy()
		{
			lock (UserList)
			{
				UserList.Clear();
			}
			
			Chat = null;
			ChannelWork = false;
			_Network = null;
			
			lock (Exceptions)
			{
				Exceptions.Clear();
			}
			
			lock (Bans)
			{
				Bans.Clear();
			}
		}
		
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

        public void InsertBan(string ban, string user)
        {

        }

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
        /// Return user object if specified user exist
        /// </summary>
        /// <param name="name">User name</param>
        /// <returns></returns>
        public User userFromName(string name)
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
        public Graphics.Window retrieveWindow()
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
                foreach (var curr in _Network._Protocol.Windows)
                {
                    if (curr.Key == _Network.window + Name)
                    {
                        this.Chat = curr.Value;
                        return curr.Value;
                    }
                }
            }
            return Chat;
        }
    }
}
