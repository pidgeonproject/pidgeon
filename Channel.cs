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

namespace Pidgeon
{
    /// <summary>
    /// Channel object
    /// </summary>
    [Serializable]
    public class Channel : libirc.Channel
    {
        /// <summary>
        /// Window this channel is rendered to
        /// </summary>
        [NonSerialized]
        private Graphics.Window Chat = null;
        /// <summary>
        /// Whether window needs to be redrawn
        /// </summary>
        public bool Redraw = false;
        /// <summary>
        /// If this is true user list was changed and needs to be refreshed but it can't be refreshed as it's waiting for some lock
        /// </summary>
        public bool UserListRefreshWait = false;
        /// <summary>
        /// Text displayed in the list menu
        /// </summary>
        public string MenuData = null;
        public new Network _Network = null;
  
        public Channel() : base()
        {
            
        }
        
        public Channel(libirc.Network network) : base(network)
        {
            
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
                    space = 160 + trimmed.Substring(160).IndexOf(" ", StringComparison.Ordinal);
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
        /// Thread safe, redraw all users in the user list in window, if exist
        /// </summary>
        public void RedrawUsers()
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
                List<libirc.User> owners = new List<libirc.User>();
                List<libirc.User> admins = new List<libirc.User>();
                List<libirc.User> oper = new List<libirc.User>();
                List<libirc.User> halfop = new List<libirc.User>();
                List<libirc.User> vs = new List<libirc.User>();
                List<libirc.User> users = new List<libirc.User>();
                bool Inserted;
                Core.SystemForm.UpdateStatus();
                if (Chat != null && Chat.IsInitialised)
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
                        foreach (libirc.User nick in UserList.Values)
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

        /// <summary>
        /// Thread safe, redraw all users in the user list in window, if exist
        /// </summary>
        [Obsolete("Use RedrawUsers(). This will be removed in pidgeon 1.6.80")]
        public void redrawUsers()
        {
            RedrawUsers();
        }

        /// <summary>
        /// Destroy this class, be careful, it can't be used in any way after you
        /// call this
        /// </summary>
        public override void Destroy()
        {
            if (IsDestroyed)
            {
                // prevent this from being called multiple times
                return;
            }
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destroying channel " + Name);
            }
            Core.SystemForm.ChannelList.RemoveChannel(this);
            Chat = null;
            base.Destroy();
        }

        /// <summary>
        /// Retrieve window
        /// </summary>
        /// <returns></returns>
        public Graphics.Window RetrieveWindow()
        {
            if (Chat == null)
            {
                if (IsDestroyed)
                {
                    return null;
                }
                if (_Network == null)
                {
                    throw new Core.PidgeonException("Network is NULL for " + Name);
                }
                if (_Network._Protocol == null)
                {
                    throw new Core.PidgeonException("Protocol is NULL for " + _Network.ServerName);
                }
                Graphics.Window wind = WindowsManager.GetWindow(_Network.SystemWindowID + Name, _Network);
                if (wind != null)
                {
                    this.Chat = wind;
                    return wind;
                }
            }
            return Chat;
        }

        /// <summary>
        /// Retrieve window - replaced by RetrieveWindow()
        /// </summary>
        /// <returns></returns>
        [Obsolete("Replaced with RetrieveWindow - will be removed in pidgeon 1.2.60")]
        public Graphics.Window retrieveWindow()
        {
            return RetrieveWindow();
        }
    }
}
