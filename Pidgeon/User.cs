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
        public new Channel Channel = null;
  
        public override string Nick
        {
            get
            {
                return this.nick;
            }
            set
            {
                if (this.Nick != value)
                {
                    if (this.Channel != null)
                    {
                        // rename the key in dictionary
                        this.Channel.RemoveUser(this);
                        // store again this exactly same user
                        this.lnick = value.ToLower();
                        this.nick = value;
                        this.Channel.InsertUser(this);
                        return;
                    }
                    this.lnick = value.ToLower();
                    this.nick = value;
                }
            }
        }
        
        public User(libirc.UserInfo user, Network network)
            : base(user, (libirc.Network)network)
        {
            this._Network = network;
        }
        
        public User(string user, Network network) : base(user, (libirc.Network)network)
        {
            this._Network = network;
        }
        
        public User(libirc.User user, Network network) : base(user.Nick, user.Host, user.Ident, (libirc.Network)network)
        {
            this.RealName = user.RealName;
            this.Away = user.Away;
            this.AwayMessage = user.AwayMessage;
            this.AwayTime = user.AwayTime;
            this.ChannelMode = user.ChannelMode;
            this.Server = user.Server;
            this._Network = network;
        }
    }
}
