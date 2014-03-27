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

using System.IO;
using System.Net;
using System;
using System.Text;

namespace Pidgeon
{
    public static partial class Core
    {
        /// <summary>
        /// Connect to XMPP
        /// </summary>
        /// <param name="server">Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="password">Password</param>
        /// <param name="secured">SSL</param>
        /// <returns>false</returns>
        public static bool ConnectXmpp(string server, int port, string password, bool secured = false)
        {
            Protocols.ProtocolXmpp IM = new Protocols.ProtocolXmpp();
            //IM.Open();
            return false;
        }

        /// <summary>
        /// Connect to dcc
        /// </summary>
        /// <param name="server">Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="password">Password</param>
        /// <param name="secured">SSL</param>
        /// <param name="dcc">Connection parameter</param>
        /// <param name="remote">Remote ID</param>
        /// <param name="Listening">Whether DCC should be listening or connect to remote</param>
        /// <param name="UserName">User name of user you connect to</param>
        /// <returns>false</returns>
        public static bool ConnectDcc(string server, int port, string password, Protocols.ProtocolDCC.DCC dcc, bool Listening, string UserName, bool secured = false, string remote = null)
        {
            Protocols.ProtocolDCC DC = new Protocols.ProtocolDCC();
            DC.Server = server;
            DC.Port = port;
            DC.SSL = secured;
            DC.ListenerMode = Listening;
            DC.Dcc = dcc;
            if (dcc == Protocols.ProtocolDCC.DCC.Chat && secured)
            {
                DC.Dcc = Protocols.ProtocolDCC.DCC.SecureChat;
            }
            DC.UserName = UserName;
            DC.Open();
            Connections.Add(DC);
            return false;
        }

        /// <summary>
        /// Connect to quassel
        /// </summary>
        /// <param name="server">Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="password">Password</param>
        /// <param name="secured">SSL</param>
        /// <returns>false</returns>
        public static bool ConnectQl(string server, int port, string password = "xx", bool secured = false)
        {
            Protocols.ProtocolQuassel _quassel = new Protocols.ProtocolQuassel();
            _quassel.Port = port;
            _quassel.Password = password;
            _quassel.Server = server;
            _quassel.SSL = secured;
            _quassel.Open();
            Connections.Add(_quassel);
            return false;
        }

        /// <summary>
        /// Connect to pidgeon server
        /// </summary>
        /// <param name="server">Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="password">Password</param>
        /// <param name="secured">SSL</param>
        /// <returns>Protocol object</returns>
        public static Protocols.Services.ProtocolSv ConnectPS(string server, int port = 8222, string password = "xx", bool secured = false)
        {
            Protocols.Services.ProtocolSv protocol = new Protocols.Services.ProtocolSv();
            protocol.Server = server;
            protocol.nick = Configuration.UserData.nick;
            protocol.Port = port;
            protocol.SSL = secured;
            protocol.password = password;
            Connections.Add(protocol);
            protocol.Open();
            return protocol;
        }

        /// <summary>
        /// Connect to IRC
        /// </summary>
        /// <param name="server">Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="password">Password</param>
        /// <param name="secured">SSL</param>
        /// <returns>Protocol object</returns>
        public static Protocols.ProtocolIrc ConnectIRC(string server, int port = 6667, string password = "", bool secured = false)
        {
            Protocols.ProtocolIrc protocol = new Protocols.ProtocolIrc();
            Connections.Add(protocol);
            protocol.Server = server;
            protocol.Port = port;
            protocol.Password = password;
            protocol.SSL = secured;
            Network network = new Network (server, protocol);
            protocol.IRCNetwork = (libirc.Network)network;
            SelectedNetwork = network;
            protocol.Open();
            return protocol;
        }
    }
}
