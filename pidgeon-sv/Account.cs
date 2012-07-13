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
            foreach (Network servername in ConnectedNetworks)
            {
                if (servername.server == name)
                {
                    return servername;
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
            ProtocolIrc server = new ProtocolIrc();
            Network networkid = new Network(network, server);
            networkid.nickname = nickname;
            networkid.ident = ident;
            networkid.username = realname;
            networkid.quit = "http://pidgeonclient.org";
            ConnectedNetworks.Add(networkid);
            server.Server = network;
            server.Port = port;
            server._server = networkid;
            server.owner = this;
            networks.Add(server);
            server.Open();
            return true;
        }

        public bool containsNetwork(string network)
        {
            foreach (Network curr in ConnectedNetworks)
            {
                if (network == curr.server)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
