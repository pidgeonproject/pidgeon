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
        public string TopicUser;
        public int TopicDate;
        private Network._window Chat;
        public bool Redraw;
        public bool ok;
            
        public class ChannelMode : Protocol.Mode
        { 
            
        }

        public bool containUser(string user)
        {
            foreach (var name in UserList)
            {
                if (name.Nick == user)
                {
                    return true;
                }
            }
            return false;
        }

        public void redrawUsers()
        {
            if (Core._KernelThread == System.Threading.Thread.CurrentThread)
            {
                retrieveWindow();
                List<string> owners = new List<string>();
                List<string> admins = new List<string>();
                List<string> oper = new List<string>();
                List<string> halfop = new List<string>();
                List<string> vs = new List<string>();
                List<string> users = new List<string>();
                if (Chat != null)
                {
                    Chat.userlist.Items.Clear();
                    foreach (var nick in UserList)
                    {
                        switch (nick.Nick[0])
                        {
                            case '~':
                                owners.Add(nick.Nick);
                                continue;
                            case '&':
                                admins.Add(nick.Nick);
                                continue;
                            case '@':
                                oper.Add(nick.Nick);
                                continue;
                            case '%':
                                halfop.Add(nick.Nick);
                                continue;
                            case '+':
                                vs.Add(nick.Nick);
                                continue;
                        }
                        users.Add(nick.Nick);
                    }
                    owners.Sort();
                    admins.Sort();
                    halfop.Sort();
                    oper.Sort();
                    vs.Sort();
                    users.Sort();
                    foreach (string user in owners)
                    {
                        Chat.userlist.Items.Add(user);
                    }
                    foreach (string user in admins)
                    {
                        Chat.userlist.Items.Add(user);
                    }
                    foreach (string user in oper)
                    {
                        Chat.userlist.Items.Add(user);
                    }
                    foreach (string user in halfop)
                    {
                        Chat.userlist.Items.Add(user);
                    }
                    foreach (string user in vs)
                    {
                        Chat.userlist.Items.Add(user);
                    }

                    foreach (string user in users)
                    {
                        Chat.userlist.Items.Add(user);
                    }
                }
                return;
            }
            Redraw = true;
            return;
        }

        public Network._window retrieveWindow()
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
