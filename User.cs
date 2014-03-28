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

namespace Pidgeon
{
    /// <summary>
    /// User, Every user on irc has instance of this class for every channel they are in
    /// </summary>
    [Serializable]
    public class User : libirc.User
    {
        /// <summary>
        /// Network this user belongs to
        /// </summary>
        [NonSerialized]
        public new Network _Network = null;

        public User(string nick, string host, Network network, string ident)
            : base(nick, host, (libirc.Network)network, ident)
        {
            this._Network = network;
        }

        public User(string user, Network network) : base(user, (libirc.Network)network)
        {
            this._Network = network;
        }
    }
}
