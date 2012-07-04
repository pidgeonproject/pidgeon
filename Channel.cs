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
using System.Linq;
using System.Text;

namespace Client
{
    public class Channel
    {
        public string Name;
        public Network _Network;
        public List<User> UserList = new List<User>();
        public string Topic;
        public bool dispose = false;
        public string TopicUser;
        public int TopicDate;
        public List<string> Invites;
        public List<string> Bl;
        public List<string> Exceptions;
        private Window Chat;
        public Protocol.Mode _mode = new Protocol.Mode();
        public bool Redraw;
        public bool ok;

        public Channel()
        {
            ok = true;
            Topic = "";
            TopicUser = "";
            Chat = null;
        }

        public class ChannelMode : Protocol.Mode
        { 
            
        }

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

        public void redrawUsers()
        {
            try
            {
                if (Core._KernelThread == System.Threading.Thread.CurrentThread)
                {
                    Redraw = false;
                    System.Windows.Forms.ListView listView = null;
                    retrieveWindow();
                    List<string> owners = new List<string>();
                    List<string> admins = new List<string>();
                    List<string> oper = new List<string>();
                    List<string> halfop = new List<string>();
                    List<string> vs = new List<string>();
                    List<string> users = new List<string>();
                    bool Inserted;
                    Core._Main.UpdateStatus();
                    if (Chat != null)
                    {
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
                            return;
                        }
                        lock (UserList)
                        {
                            foreach (var nick in UserList)
                            {
                                Inserted = false;
                                lock (_Network._protocol.CUModes)
                                {
                                    foreach (char mode in _Network._protocol.CUModes)
                                    {
                                        int _m;
                                        if (nick.ChannelMode._Mode.Contains(mode.ToString()))
                                        {
                                            _m = _Network._protocol.CUModes.IndexOf(mode);
                                            if (_Network._protocol.UChars.Count >= _m)
                                            {
                                                switch (mode)
                                                {
                                                    case 'q':
                                                        owners.Add(_Network._protocol.UChars[_m].ToString() + nick.Nick);
                                                        Inserted = true;
                                                        break;
                                                    case 'a':
                                                        admins.Add(_Network._protocol.UChars[_m].ToString() + nick.Nick);
                                                        Inserted = true;
                                                        break;
                                                    case 'o':
                                                        Inserted = true;
                                                        oper.Add(_Network._protocol.UChars[_m].ToString() + nick.Nick);
                                                        break;
                                                    case 'h':
                                                        Inserted = true;
                                                        halfop.Add(_Network._protocol.UChars[_m].ToString() + nick.Nick);
                                                        break;
                                                    case 'v':
                                                        vs.Add(_Network._protocol.UChars[_m].ToString() + nick.Nick);
                                                        Inserted = true;
                                                        break;
                                                }
                                                if (!Inserted)
                                                {
                                                    users.Add(_Network._protocol.UChars[_m].ToString() + nick.Nick);
                                                    Inserted = true;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                                if (Inserted == true)
                                {
                                    continue;
                                }
                                users.Add(nick.Nick);
                            }
                        }
                        
                        listView.Items.Clear();

                        owners.Sort();
                        admins.Sort();
                        halfop.Sort();
                        oper.Sort();
                        vs.Sort();
                        users.Sort();

                        foreach (string user in owners)
                        {
                            listView.Items.Add(user);
                        }
                        foreach (string user in admins)
                        {
                            listView.Items.Add(user);
                        }
                        foreach (string user in oper)
                        {
                            listView.Items.Add(user);
                        }
                        foreach (string user in halfop)
                        {
                            listView.Items.Add(user);
                        }
                        foreach (string user in vs)
                        {
                            listView.Items.Add(user);
                        }

                        foreach (string user in users)
                        {
                            listView.Items.Add(user);
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

        public Window retrieveWindow()
        {
            if (Chat == null)
            {
                foreach (var curr in _Network.windows)
                {
                    if (curr.Key == Name)
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
