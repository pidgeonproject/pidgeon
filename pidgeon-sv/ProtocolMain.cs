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
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="Name">Name of a datagram</param>
            /// <param name="Text">Value</param>
            public Datagram(string Name, string Text = "")
            {
                _Datagram = Name;
                _InnerText = Text;
            }

            public string ToDocumentXmlText()
            {
                XmlDocument datagram = new XmlDocument();
                XmlNode b1 = datagram.CreateElement(_Datagram.ToUpper());
                foreach (KeyValuePair<string, string> curr in Parameters)
                {
                    XmlAttribute b2 = datagram.CreateAttribute(curr.Key);
                    b2.Value = curr.Value;
                    b1.Attributes.Append(b2);
                }
                b1.InnerText = this._InnerText;
                datagram.AppendChild(b1);
                return datagram.InnerXml;
            }

            public string _InnerText;
            public string _Datagram;
            public Dictionary<string, string> Parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Pointer to client
        /// </summary>
        public Connection client;

        public bool Connected = false;

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
            Connected = false;
            if (client.account != null)
            {
                lock (client.account.Clients)
                {
                    if (!client.account.ClientsOK.Contains(this))
                    {
                        lock (client.account.ClientsOK)
                        {
                            client.account.ClientsOK.Add(this);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Process the request
        /// </summary>
        /// <param name="node"></param>
        public void parseXml(XmlNode node)
        {
            Datagram response = null;
            if (client.status == Connection.Status.WaitingPW)
            {
                switch (node.Name.ToUpper())
                {
                    case "CHANNELINFO":
                    case "RAW":
                    case "GLOBALNICK":
                    case "GLOBALIDENT":
                    case "MESSAGE":
                    case "CONNECT":
                    case "NICK":
                    case "NETWORKINFO":
                    case "JOIN":
                    case "PART":
                    case "BACKLOGSV":
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
                        int depth = 0;
                        if (node.Attributes.Count > 2)
                        {
                            depth = int.Parse(node.Attributes[2].Value);
                        }
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
                            if (depth > 0)
                            {

                            }
                            network._protocol.Transfer(node.InnerText, Priority);
                            if (node.InnerText.StartsWith("PRIVMSG"))
                            {
                                ProtocolIrc.MessageOrigin xx = new ProtocolIrc.MessageOrigin();
                                xx.text = node.InnerText;
                                xx.time = DateTime.Now;
                            }

                        }
                    }
                    break;
                case "NICK":
                    if (node.Attributes.Count > 0)
                    {
                        Network b008 = client.account.retrieveServer(node.Attributes[0].Value);
                        if (b008 != null)
                        {
                            response = new Datagram("NICK", b008.nickname);
                            response.Parameters.Add("network", b008.server);
                            Deliver(response);
                            break;
                        }
                        response = new Datagram("NICK", "UNKNOWN");
                        response.Parameters.Add("network", b008.server);
                        response.Parameters.Add("failure", "failure");
                        Deliver(response);
                    }
                    break;
                case "CHANNELINFO":
                    Network b002 = client.account.retrieveServer(node.Attributes[0].Value);
                    switch (node.InnerText)
                    {
                        case "LIST":
                            if (b002 == null)
                            {
                                Deliver(new Datagram("CHANNELINFO", "EMPTY"));
                                return;
                            }
                            string list = "";
                            lock (b002.Channels)
                            {
                                foreach (Channel curr in b002.Channels)
                                {
                                    list += curr.Name + "!";
                                }
                            }
                            response = new Datagram("CHANNELINFO", "");
                            response.Parameters.Add("network", node.Attributes[0].Value);
                            response.Parameters.Add("channels", list);
                            Deliver(response);
                            break;
                        case "INFO":
                            if (b002 == null)
                            {
                                Deliver(new Datagram("CHANNELINFO", "EMPTY"));
                                return;
                            }
                            if (node.Attributes.Count > 1)
                            {
                                Channel channel = b002.getChannel(node.Attributes[1].Value);
                                if (channel == null)
                                {
                                    Console.WriteLine(b002.Channels.Count.ToString());
                                    response = new Datagram("CHANNELINFO", "EMPTY");
                                    response.Parameters.Add("network", node.Attributes[0].Value);
                                    response.Parameters.Add("channel", node.Attributes[1].Value);
                                    Deliver(response);
                                    return;

                                }
                                string userli = "";
                                foreach (User l2 in channel.UserList)
                                {
                                    userli += l2.Nick + "!" + l2.Ident + "@" + l2.Host + l2.ChannelMode.ToString() + ":";
                                }
                                response = new Datagram("CHANNELINFO", "USERLIST");
                                response.Parameters.Add("network", node.Attributes[0].Value);
                                response.Parameters.Add("channel", node.Attributes[1].Value);
                                response.Parameters.Add("ul", userli);
                                Console.WriteLine("can't find " + node.Attributes[1].Value);
                                Deliver(response);
                            }
                            break;
                    }
                    return;
                case "NETWORKLIST":
                    string networks = "";
                    lock (client.account.ConnectedNetworks)
                    {
                        foreach (Network current_net in client.account.ConnectedNetworks)
                        {
                            networks += current_net.server + "|";
                        }
                    }
                    response = new Datagram("NETWORKLIST", networks);
                    Deliver(response);
                    return;
                case "LOAD":
                    response = new Datagram("LOAD", "Pidgeon service version " + Config.version + " supported mode=ns I have " + Connection.ActiveUsers.Count.ToString() + " connections");
                    Deliver(response);
                    return;
                case "BACKLOGSV":
                    if (node.Attributes.Count > 1)
                    {
                        Network network = client.account.retrieveServer(node.Attributes[0].Value);
                        if (network != null)
                        {
                            ProtocolIrc protocol = (ProtocolIrc)network._protocol;
                            protocol.getDepth(int.Parse(node.Attributes[1].Value), this);
                        }
                    }
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
                    Core.SL(client.IP + ": Connecting to " + node.InnerText);
                    response = new Datagram("CONNECT", "OK");
                    response.Parameters.Add("network", node.InnerText);
                    Deliver(response);
                    break;
                case "GLOBALIDENT":
                    client.account.ident = node.InnerText;
                    Deliver(new Datagram("GLOBALIDENT", node.InnerText));
                    break;
                case "MESSAGE":
                    if (node.Attributes.Count > 2)
                    {
                        Network network4 = client.account.retrieveServer(node.Attributes[0].Value);
                        if (network4 != null)
                        {
                            string target = node.Attributes[2].Value;
                            string ff = node.Attributes[1].Value;
                            ProtocolIrc.Priority Priority = ProtocolIrc.Priority.Normal;
                            switch (ff)
                            {
                                case "Low":
                                    Priority = ProtocolIrc.Priority.Low;
                                    break;
                                case "High":
                                    Priority = ProtocolIrc.Priority.High;
                                    break;
                            }
                            network4._protocol.Message(node.InnerText, target, Priority);
                            ProtocolIrc prot = (ProtocolIrc)network4._protocol;
                        }
                    }
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
                    lock (Core._accounts)
                    {
                        foreach (Account curr_user in Core._accounts)
                        {
                            if (curr_user.username == username)
                            {
                                if (curr_user.password == pw)
                                {
                                    client.account = curr_user;
                                    lock (client.account.Clients)
                                    {
                                        client.account.Clients.Add(this);
                                    }
                                    Core.SL(client.IP + ": Logged in as " + client.account.username);
                                    response = new Datagram("AUTH", "OK");
                                    response.Parameters.Add("ls", "There is " + client.account.Clients.Count.ToString() + " connections logged in to this account");
                                    client.status = Connection.Status.Connected;
                                    Deliver(response);
                                    return;
                                }
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
            Connected = true;
            client = t;
        }

        public void Deliver(Datagram message)
        {
            if (!Connected)
            {
                return;
            }
            XmlDocument datagram = new XmlDocument();
            XmlNode b1 = datagram.CreateElement("S" + message._Datagram.ToUpper());
            foreach (KeyValuePair<string, string> curr in message.Parameters)
            {
                XmlAttribute b2 = datagram.CreateAttribute(curr.Key);
                b2.Value = curr.Value;
                b1.Attributes.Append(b2);
            }
            b1.InnerText = message._InnerText;
            datagram.AppendChild(b1);


            Send(datagram.InnerXml);
        }

        public bool Send(string text)
        {
            if (!Connected)
            {
                return false;
            }
            try
            {
                client._w.WriteLine(text);
                client._w.Flush();
            }
            catch (IOException)
            {
                Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace + ex.Message);
            }
            return true;
        }
    }
}
