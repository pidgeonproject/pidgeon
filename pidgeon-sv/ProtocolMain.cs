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
    class ProtocolMain
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

        public void parseXml(XmlNode node)
        {
            Datagram response;
            if (client.status == Connection.Status.WaitingPW)
            {
                switch(node.Name.ToUpper())
                {
                    case "CHANNELINFO":
                    case "RAW":
                    case "MESSAGE":
                    case "CONNECT":
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
                case "RAW":

                    break;
                case "CHANNELINFO":
                    
                    return;
                case "NETWORKLIST":
                    string networks = "";
                    foreach (Network current_net in _in)
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
                    response = new Datagram("CONNECT", "OK");
                    response.Parameters.Add("network", node.InnerText);
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
                                response = new Datagram("AUTH", "OK");
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

            text = "<S" + message._Datagram + dl + ">" + message._InnerText + "</S" + message._Datagram + ">";


            Send(text);
        }

        public bool Send(string text)
        {
            client._w.WriteLine(text);
            client._w.Flush();
            return true;
        }
    }
}
