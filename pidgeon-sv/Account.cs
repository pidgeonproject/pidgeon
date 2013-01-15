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
    public class Account
    {
        public List<ProtocolIrc> networks = new List<ProtocolIrc>();
        public List<ProtocolMain> ClientsOK = new List<ProtocolMain>();
        public List<ProtocolMain> Clients = new List<ProtocolMain>();
        public string username = "";
        public string password = "";
        public string nickname = "PidgeonUser";
        public string ident = "pidgeon";
        public string realname = "http://pidgeonclient.org";
        public List<Network> ConnectedNetworks = new List<Network>();
        
        public Account(string user, string pw)
        {
            username = user;
            password = pw;
        }

        public Network retrieveServer(string name)
        {
            lock (ConnectedNetworks)
            {
                foreach (Network servername in ConnectedNetworks)
                {
                    if (servername.server == name)
                    {
                        return servername;
                    }
                }
            }
            return null;
        }

        public bool Deliver(ProtocolMain.Datagram text)
        {
            lock (Clients)
            {
                try
                {
                    if (Clients.Count == 0)
                    {
                        return false;
                    }
                    foreach (ProtocolMain ab in Clients)
                    {
                        ab.Deliver(text);
                    }
                    foreach (ProtocolMain i in ClientsOK)
                    {
                        Clients.Remove(i);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error 10");
                }
            }
            return true;
        }

        public bool ConnectIRC(string network, int port = 6667)
        {
            try
            {
                ProtocolIrc server = new ProtocolIrc();
                Network networkid = new Network(network, server);
                networkid.nickname = nickname;
                networkid.ident = ident;
                networkid.username = realname;
                networkid.quit = "http://pidgeonclient.org";
                lock (ConnectedNetworks)
                {
                    ConnectedNetworks.Add(networkid);
                }
                server.Server = network;
                server.Port = port;
                server._server = networkid;
                server.owner = this;
                lock (networks)
                {
                    networks.Add(server);
                }
                server.Open();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
        }

        public bool containsNetwork(string network)
        {
            lock (ConnectedNetworks)
            {
                foreach (Network curr in ConnectedNetworks)
                {
                    if (network == curr.server)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
