using System;
using System.Collections.Generic;
using System.Text;

namespace pidgeon_sv
{
    class Account
    {
        public List<ProtocolIrc> networks = new List<ProtocolIrc>();
        public List<ProtocolMain> Clients = new List<ProtocolMain>();
        public string username = "";
        public string password = "";
        public List<Network> ConnectedNetworks = new List<Network>();
        
        public Account(string user, string pw)
        {
            username = user;
            password = pw;
        }

        public void Deliver(ProtocolMain.Datagram text)
        {
            foreach (ProtocolMain ab in Clients)
            {
                ab.Deliver(text);
            }
        }

        public bool ConnectIRC(string network, int port = 6667)
        {
            ProtocolIrc server = new ProtocolIrc();
            server.Server = network;
            server.Port = port;
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
