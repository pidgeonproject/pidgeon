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

// Documentation
///////////////////////
// This file contains a default class for protocols which all the other classes are inherited from
// some functions are returning integer, which should be 0 on success and 2 by default
// which means that the function was never overriden, so that a function working with that can catch it

using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Pidgeon
{
    /// <summary>
    /// Connection
    /// </summary>
    [Serializable]
    public class Protocol : libirc.Protocol
    {
        /// <summary>
        /// Parse a command
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public virtual bool Command(string cm, Network network = null)
        {
            Syslog.DebugLog("Command(string cm, Network network = null) is not implemented");
            return false;
        }

        public virtual Result ParseInput(string Input)
        {
            return Result.NotImplemented;
        }

        /// <summary>
        /// Format a message to given style selected by skin
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="text">Text</param>
        /// <returns></returns>
        public static string PRIVMSG(string user, string text)
        {
            return Configuration.Scrollback.format_nick.Replace("$1", "%USER%" + user + "%/USER%") + EncodeText(text);
        }

        /// <summary>
        /// Return back the system chars to previous look
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DecodeText(string text)
        {
            return text.Replace("%####%", "%");
        }
        
        /// <summary>
        /// Escape system char
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Escaped text</returns>
        public static string EncodeText(string text)
        {
            return text.Replace("%", "%####%");
        }

        public virtual int Message(string text, string to, Network network, libirc.Defs.Priority _priority = libirc.Defs.Priority.Normal, bool pmsg = false)
        {
            return 0;
        }
    }
}
