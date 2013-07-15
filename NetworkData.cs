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

namespace Client
{
    /// <summary>
    /// Network
    /// </summary>
    public class NetworkData
    {
        /// <summary>
        /// Information about a single network in a network list
        /// </summary>
        public class NetworkInfo
        {
            /// <summary>
            /// Name of the network
            /// </summary>
            public string Name = null;
            /// <summary>
            /// Server
            /// </summary>
            public string Server = null;
            /// <summary>
            /// Network is using ssl
            /// </summary>
            public bool SSL = false;
            /// <summary>
            /// Type
            /// </summary>
            public ProtocolType protocolType = ProtocolType.IRC;

            /// <summary>
            /// Constructor for XML
            /// </summary>
            public NetworkInfo()
            {
                // this is here for xml parser
                // keep this
            }
        }

        /// <summary>
        /// List of all loaded networks
        /// </summary>
        public static List<NetworkInfo> Networks = new List<NetworkInfo>();

        /// <summary>
        /// Protocol type
        /// </summary>
        public enum ProtocolType
        {
            /// <summary>
            /// IRC
            /// </summary>
            IRC,
            /// <summary>
            /// DCC
            /// </summary>
            DCC,
            /// <summary>
            /// Services
            /// </summary>
            Services,
            /// <summary>
            /// Quassel
            /// </summary>
            Quassel,
            /// <summary>
            /// XMPP
            /// </summary>
            XMPP,
        }
    }
}
