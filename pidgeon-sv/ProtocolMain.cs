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
using System.IO;
using System.Xml;
using System.Text;

namespace pidgeon_sv
{
    public class ProtocolMain
    {
        public class Datagram
        {
            public Datagram(string Name, string Text = "")
            {
                _Datagram = Name;
                if (Text != "")
                {
                    _InnerText = System.Web.HttpUtility.HtmlEncode(Text);
                }
                _InnerText = Text;
            }
            public string _InnerText;
            public string _Datagram;
            public Dictionary<string, string> Parameters = new Dictionary<string,string>();
        }

        /// <summary>
        /// Pointer to client
        /// </summary>
        public Connection client;

        /// <summary>
        /// networks
        /// </summary>
        public List<Network> _in = new List<Network>();

        public static bool Valid(string datagram)
        {
            if (datagram == null)
            {
                return false;
            }
            if (datagram == "")
            {
                return false;
            }
            if (datagram.StartsWith("<") && datagram.EndsWith(">"))
            {
                return true;
            }
            return false;
        }



        public void parseCommand(string data)
        {
            
            XmlDocument datagram = new XmlDocument();
            datagram.LoadXml(data);
            foreach (System.Xml.XmlNode curr in datagram.ChildNodes)
            {
                parseXml(curr);
            }
        }

        public void Exit()
        {
            if (client.account != null)
            {
                client.account.Clients.Remove(this);
            }
        }

        public void parseXml(XmlNode node)
        {
            Datagram response;
            if (client.status == Connection.Status.WaitingPW)
            {
                switch(node.Name.ToUpper())
                {
                    case "CHANNELINFO":
                    case "RAW":
                    case "GLOBALNICK":
                    case "GLOBALIDENT":
                    case "MESSAGE":
                    case "CONNECT":
                    case "NICK":
                    case "JOIN":
                    case "PART":
                    case "KICK":
                    case "NETWORKLIST":
                        response = new Datagram(node.Name.ToUpper(), "PERMISSIONDENY");
                        Deliver(response);
                        return;
                }
            }

            switch (node.Name.ToUpper())
            { 
                case "STATUS":
                    string info = client.status.ToString();
                    response = new Datagram("STATUS", info);
                    Deliver(response);
                    break;
                case "NETWORKINFO":
                    Network network2 = client.account.retrieveServer(node.InnerText);
                    if (network2 == null)
                    {
                        response = new Datagram("NETWORKINFO", "UNKNOWN");
                        response.Parameters.Add("network", node.InnerText);
                        Deliver(response);
                        return;
                    }
                    if (network2.Connected == false)
                    {
                        response = new Datagram("NETWORKINFO", "OFFLINE");
                        response.Parameters.Add("network", node.InnerText);
                        Deliver(response);
                        return;
                    }
                    response = new Datagram("NETWORKINFO", "ONLINE");
                    response.Parameters.Add("network", node.InnerText);
                    Deliver(response);
                    return;
                case "RAW":
                    if (node.Attributes.Count > 0)
                    {
                        string server = node.Attributes[0].Value;
                        string priority = "Normal";
                        ProtocolIrc.Priority Priority = ProtocolIrc.Priority.Normal;
                        if (node.Attributes.Count > 1)
                        {
                            priority = node.Attributes[1].Value;
                            switch (priority)
                            {
                                case "High":
                                    Priority = ProtocolIrc.Priority.High;
                                    break;
                                case "Low":
                                    Priority = ProtocolIrc.Priority.Low;
                                    break;
                            }
                        }
                        if (client.account.containsNetwork(server))
                        {
                            Network network = client.account.retrieveServer(server);
                            network._protocol.Transfer(node.InnerText, Priority);
                        }
                    }
                    break;
                case "CHANNELINFO":
                    
                    return;
                case "NETWORKLIST":
                    string networks = "";
                    foreach (Network current_net in client.account.ConnectedNetworks)
                    {
                        networks += current_net.server + "|";
                    }
                    response = new Datagram("NETWORKLIST", networks);
                    Deliver(response);
                    return;
                case "LOAD":
                    response = new Datagram("LOAD", "Pidgeon service version 1.0.0.0 supported mode=ns");
                    Deliver(response);
                    return;
                case "CONNECT":
                    if (client.account.containsNetwork(node.InnerText))
                    { 
                        response = new Datagram("CONNECT", "CONNECTED");
                        response.Parameters.Add("network", node.InnerText);
                        Deliver(response);
                        return;
                    }
                    int port = 6667;
                    if (node.Attributes.Count > 0)
                    {
                        port = int.Parse(node.Attributes[0].Value);
                    }
                    client.account.ConnectIRC(node.InnerText, port);
                    Console.WriteLine("Connecting to " + node.InnerText);
                    response = new Datagram("CONNECT", "OK");
                    response.Parameters.Add("network", node.InnerText);
                    Deliver(response);
                    break;
                case "GLOBALIDENT":
                    client.account.ident = node.InnerText;
                    Deliver(new Datagram("GLOBALIDENT", node.InnerText));
                    break;
                case "GLOBALNICK":
                    if (node.InnerText == "")
                    {
                        Deliver(new Datagram("GLOBALNICK", client.account.nickname));
                        break;
                    }
                    Deliver(new Datagram("GLOBALNICK", node.InnerText));
                    client.account.nickname = node.InnerText;
                    Core.SaveData();
                    break;
                case "AUTH":
                    string username = node.Attributes[0].Value;
                    string pw = node.Attributes[1].Value;
                    foreach (Account curr_user in Core._accounts)
                    {
                        if (curr_user.username == username)
                        {
                            if (curr_user.password == pw)
                            {
                                client.account = curr_user;
                                client.account.Clients.Add(this);
                                response = new Datagram("AUTH", "OK");
                                client.status = Connection.Status.Connected;
                                Deliver(response);
                                return;
                            }
                        }
                    }
                    response = new Datagram("AUTH", "INVALID");
                    Deliver(response);
                    return;
            }
        }

        public ProtocolMain(Connection t)
        {
            client = t;
        }

        public void Deliver(Datagram message)
        {
            string text = "";
            string dl = "";

            foreach (KeyValuePair<string, string> curr in message.Parameters)
            {
                dl += " "   + curr.Key + "=\"" + System.Web.HttpUtility.HtmlEncode(curr.Value) + "\"";  
            }

            text = "<S" + message._Datagram + dl + ">" + System.Web.HttpUtility.HtmlEncode(message._InnerText) + "</S" + message._Datagram + ">";


            Send(text);
        }

        public bool Send(string text)
        {
            try
            {
                client._w.WriteLine(text);
                client._w.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace + ex.Message);
            }
            return true;
        }
    }
}
