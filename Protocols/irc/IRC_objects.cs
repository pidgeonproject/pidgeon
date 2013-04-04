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
}