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

namespace pidgeon_sv
{
    public class ChannelParameterMode
    {
        public string _Target = "";
        public string Time;
        public string _User;
    }
    public class Invite : ChannelParameterMode
    {
        public Invite(string user, string target, string time)
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
        public string Name;
        public Network _Network;
        public List<User> UserList = new List<User>();
        public string Topic;
        public bool dispose = false;
        public string TopicUser;
        public int TopicDate;
        public List<Invite> Invites;
        public List<SimpleBan> Bl;
        public List<Except> Exceptions;
        public bool parsing_who = false;
        public bool parsing_xb = false;
        public bool parsing_xe = false;
        public bool parsing_wh = false;
        public Protocol.Mode _mode = new Protocol.Mode();
        public bool Redraw;
        public bool ok;

        public Channel()
        {
            ok = true;
            lock (_control)
            {
                _control.Add(this);
            }
            Topic = "";
            TopicUser = "";
        }

        public void Dispose()
        {
            lock (_control)
            {
                if (_control.Contains(this))
                {
                    _control.Remove(this);
                }
            }
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


        public void redrawUsers()
        {
            try
            {
                    List<User> owners = new List<User>();
                    List<User> admins = new List<User>();
                    List<User> oper = new List<User>();
                    List<User> halfop = new List<User>();
                    List<User> vs = new List<User>();
                    List<User> users = new List<User>();
                    bool Inserted;
                        lock (UserList)
                        {
                            foreach (var nick in UserList)
                            {
                                Inserted = false;
                                if (nick.ChannelMode._Mode.Count > 0)
                                {
                                    lock (_Network._protocol.CUModes)
                                    {
                                        foreach (char mode in _Network._protocol.CUModes)
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


                        owners.Sort();
                        admins.Sort();
                        halfop.Sort();
                        oper.Sort();
                        vs.Sort();
                        users.Sort();

                        int i = 0;

                return;
            }
            catch (Exception f)
            {
                //Core.handleException(f);
            }
        }

        public string uchr(User nick)
        {
            if (nick.ChannelMode._Mode.Count < 1)
            {
                return "";
            }
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
                            return _Network._protocol.UChars[_m].ToString();
                        }
                        break;
                    }
                }
            }
            return "";
        }

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
    }
}
