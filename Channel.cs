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
using System.Text;

namespace Client
{
    public class ChannelParameterMode
    {
        public string _Target = "";
        public string Time;
        public string _User;
    }
    public class Invite : ChannelParameterMode
    {
        public Invite(string user,string target, string time)
        {
        
        }
    }
    public class Except : ChannelParameterMode
    {
        public Except()
        { 
        
        }
    }
    public class SimpleBan : ChannelParameterMode
    {
        public SimpleBan(string user, string target, string time)
        {
            _Target = target;
            _User = user;
            Time = time;
        }
    }

    public class Channel
    {
        public static List<Channel> _control = new List<Channel>();
        /// <summary>
        /// Name
        /// </summary>
        public string Name;
        /// <summary>
        /// Network
        /// </summary>
        public Network _Network;
        /// <summary>
        /// List of all users in current channel
        /// </summary>
        public List<User> UserList = new List<User>();
        /// <summary>
        /// Topic
        /// </summary>
        public string Topic;
        /// <summary>
        /// Whether channel is in proccess of dispose
        /// </summary>
        public bool dispose = false;
        /// <summary>
        /// User who set a topic
        /// </summary>
        public string TopicUser;
        /// <summary>
        /// Date when a topic was set
        /// </summary>
        public int TopicDate;
        /// <summary>
        /// Invites
        /// </summary>
        public List<Invite> Invites;
        /// <summary>
        /// List of bans set
        /// </summary>
        public List<SimpleBan> Bl;
        /// <summary>
        ///
        /// </summary>
        public List<Except> Exceptions;
        private Window Chat;
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
        public bool parsing_xb = false;
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
        public Protocol.Mode _mode;
        /// <summary>
        /// Whether window needs to be redraw
        /// </summary>
        public bool Redraw;
        /// <summary>
        /// If true the window is considered usable
        /// </summary>
        public bool ok;

        /// <summary>
        /// Constructor (simple)
        /// </summary>
        public Channel()
        {
            ok = true;
            _mode = new Protocol.Mode(1, _Network);
            lock (_control)
            {
                _control.Add(this);
            }
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
            ok = true;
            _mode = new Protocol.Mode(1, _Network);
            lock (_control)
            {
                _control.Add(this);
            }
            Topic = "";
            TopicUser = "";
            Chat = null;
        }

        /// <summary>
        /// Remove the object, be sure to remove all references before, otherwise it may cause a crash
        /// </summary>
        public void Dispose()
        {
            lock (_control)
            {
                if (retrieveWindow() != null)
                {
                    retrieveWindow().Dispose();
                }
                if (_control.Contains(this))
                {
                    _control.Remove(this);
                }
            }
        }

        /// <summary>
        /// Return true if channel contains the given user name
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool containUser(string user)
        {
            lock (UserList)
            {
                foreach (var name in UserList)
                {
                    if (name.Nick == user)
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
        public bool containsBan(string host)
        {
            lock (Bl)
            {
                foreach (var name in Bl)
                {
                    if (name._Target == host)
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
                        if (Chat.locked)
                        {
                            Redraw = true;
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
                            i++;
                        }
                        foreach (User user in admins)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            i++;
                        }
                        foreach (User user in oper)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            i++;
                        }
                        foreach (User user in halfop)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            i++;
                        }
                        foreach (User user in vs)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
                            i++;
                        }

                        foreach (User user in users)
                        {
                            listView.Items.Add(uchr(user) + user.Nick);
                            listView.Items[i].ToolTipText = user.Nick + "!" + user.Ident + "@" + user.Host;
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
                return;
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        public string uchr(User nick)
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
        /// Return user object if specified user exist
        /// </summary>
        /// <param name="name">User name</param>
        /// <returns></returns>
        public User userFromName(string name)
        {
                foreach (User item in UserList)
                {
                    if (name == item.Nick)
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
        public Window retrieveWindow()
        {
            if (Chat == null)
            {
                foreach (var curr in _Network._protocol.windows)
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
