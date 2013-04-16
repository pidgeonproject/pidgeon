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
    public class NetworkData
    {
        public class NetworkInfo
        {
            public string Name = null;
            public string Server = null;
            public bool SSL = false;
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

        public static List<NetworkInfo> Networks = new List<NetworkInfo>();

        public enum ProtocolType
        {
            IRC,
            Services,
            Quassel,
            XMPP,
        }
    }
}
