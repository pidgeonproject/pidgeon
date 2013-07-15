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
using System.Text;

namespace Client
{
    /// <summary>
    /// This is a global interface for channel modes with parameters
    /// </summary>
    public class ChannelParameterMode
    {
        /// <summary>
        /// Target of a mode
        /// </summary>
        public string Target = null;
        /// <summary>
        /// Time when it was set
        /// </summary>
        public string Time = null;
        /// <summary>
        /// User who set the ban / invite
        /// </summary>
        public string User = null;
    }

    /// <summary>
    /// Invite
    /// </summary>
    [Serializable]
    public class Invite : ChannelParameterMode
    {
        /// <summary>
        /// Creates a new instance of invite
        /// </summary>
        public Invite()
        {
            // This empty constructor is here so that we can serialize this
        }

        /// <summary>
        /// Creates a new instance of invite
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="target">Target</param>
        /// <param name="time">Time</param>
        public Invite(string user, string target, string time)
        {
            User = user;
            Target = target;
            Time = time;
        }
    }

    /// <summary>
    /// Exception
    /// </summary>
    [Serializable]
    public class ChannelBanException : ChannelParameterMode
    {
        /// <summary>
        /// Creates a new instance of channel ban exception (xml constructor only)
        /// </summary>
        public ChannelBanException()
        {
            // This empty constructor is here so that we can serialize this
        }
    }

    /// <summary>
    /// Simplest ban
    /// </summary>
    [Serializable]
    public class SimpleBan : ChannelParameterMode
    {
        /// <summary>
        /// Creates a new instance of simple ban (xml constructor only)
        /// </summary>
        public SimpleBan()
        {
            // This empty constructor is here so that we can serialize this
        }

        /// <summary>
        /// Creates a new instance of simple ban
        /// </summary>
        /// <param name="user">Person who set a ban</param>
        /// <param name="target">Who is target</param>
        /// <param name="time">Unix date when it was set</param>
        public SimpleBan(string user, string target, string time)
        {
            Target = target;
            User = user;
            Time = time;
        }
    }
}
