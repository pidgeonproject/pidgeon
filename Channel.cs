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
        public System.Windows.Forms.TreeNode TreeNode = null;
        /// <summary>
        /// If this is true user list was changed and needs to be refreshed but it can't be refreshed as it's waiting for some lock
        /// </summary>
        public bool UserListRefreshWait = false;

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

        /// <summary>
        /// Recreate information in side menu
        /// </summary>
        public void UpdateInfo()
        {
            if (TreeNode != null)
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
                TreeNode.ToolTipText = text;
            }
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
                    System.Windows.Forms.ListView listView = null;
                    retrieveWindow();
                    List<User> owners = new List<User>();
                    List<User> admins = new List<User>();
                    List<User> oper = new List<User>();
                    List<User> halfop = new List<User>();
                    List<User> vs = new List<User>();
                    List<User> users = new List<User>();
                    bool Inserted;
                    Core._Main.UpdateStatus();
                    if (Chat != null)
                    {
                        if (Chat.Locked)
                        {
                            Redraw = true;
                            PidgeonList.Updated = true;
                            UserListRefreshWait = true;
                            return;
                        }
                        if (Chat.listView.Visible)
                        {
                            listView = Chat.listViewd;
                        }
                        if (Chat.listViewd.Visible)
                        {
                            listView = Chat.listView;
                        }
                        if (listView == null)
                        {
                            Chat.listView.Visible = true;
                            Redraw = true;
                            PidgeonList.Updated = true;
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

                        listView.Items.Clear();

                        owners.Sort();
                        admins.Sort();
                        halfop.Sort();
                        oper.Sort();
                        vs.Sort();
                        users.Sort();

                        int i = 0;

                        foreach (User user in owners)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            listView.Items[i].ForeColor = Configuration.CurrentSkin.colorq;
                            i++;
                        }
                        foreach (User user in admins)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            listView.Items[i].ForeColor = Configuration.CurrentSkin.colora;
                            i++;
                        }
                        foreach (User user in oper)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            listView.Items[i].ForeColor = Configuration.CurrentSkin.coloro;
                            i++;
                        }
                        foreach (User user in halfop)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            listView.Items[i].ForeColor = Configuration.CurrentSkin.colorh;
                            i++;
                        }
                        foreach (User user in vs)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            listView.Items[i].ForeColor = Configuration.CurrentSkin.colorv;
                            i++;
                        }

                        foreach (User user in users)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            listView.Items[i].ForeColor = Configuration.CurrentSkin.colordefault;
                            i++;
                        }
                        if (Chat.listViewd.Visible == true)
                        {
                            Chat.listViewd.Visible = false;
                            Chat.listView.Visible = true;
                        }
                        else
                        {
                            Chat.listView.Visible = false;
                            Chat.listViewd.Visible = true;
                        }
                    }
                    return;
                }

                Redraw = true;
                PidgeonList.Updated = true;
                return;
            }
            catch (Exception f)
            {
                Core.handleException(f);
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
