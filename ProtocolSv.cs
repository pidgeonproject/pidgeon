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
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class ProtocolSv : Protocol
    {
        public System.Threading.Thread main;
        public System.Threading.Thread deliveryqueue;
        public System.Threading.Thread keep;
        public DateTime pong = DateTime.Now;

        private System.Net.Sockets.NetworkStream _network;
        private System.IO.StreamReader _reader;
        public List<Network> sl = new List<Network>();
        private System.IO.StreamWriter _writer;
        public string password = "";
        private Status ConnectionStatus = Status.WaitingPW;
        public string nick = "";
        public bool auth = false;

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
            public Dictionary<string, string> Parameters = new Dictionary<string, string>();
        }

        public enum Status
        {
            WaitingPW,
            Connected,
        }

        public void _Ping()
        {
            while (true)
            {
                Deliver(new Datagram("PING"));
                System.Threading.Thread.Sleep(480000);
            }
        }

        public override bool Command(string cm)
        {
            try
            {
                if (cm.StartsWith(" ") != true && cm.Contains(" "))
                {
                    // uppercase
                    string first_word = cm.Substring(0, cm.IndexOf(" ")).ToUpper();
                    string rest = cm.Substring(first_word.Length);
                    Transfer(first_word + rest);
                    return true;
                }
                Transfer(cm.ToUpper());
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
            return false;
        }

        public void Start()
        {
            Core._Main.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }), Scrollback.MessageStyle.System);
            try
            {
                _network = new System.Net.Sockets.TcpClient(Server, Port).GetStream();

                _writer = new System.IO.StreamWriter(_network);
                _reader = new System.IO.StreamReader(_network, Encoding.UTF8);

                Deliver(new Datagram("PING"));
                Deliver(new Datagram("LOAD"));
                Connected = true;

                Datagram login = new Datagram("AUTH", "");
                login.Parameters.Add("user", nick);
                login.Parameters.Add("pw", password);
                Deliver(login);
                //Deliver(new Datagram("NICK", nick))
                Deliver(new Datagram("GLOBALNICK"));
                Deliver(new Datagram("NETWORKLIST"));
                Deliver(new Datagram("STATUS"));


                keep = new System.Threading.Thread(_Ping);
                keep.Name = "pinger thread";
                keep.Start();

            }
            catch (Exception b)
            {
                Core._Main.Chat.scrollback.InsertText(b.Message, Scrollback.MessageStyle.System);
                return;
            }
            string text = "";
            try
            {

                while (!_reader.EndOfStream)
                {
                    text = _reader.ReadLine();
                    while (Core.blocked)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    if (Valid(text))
                    {
                        Process(text);
                        continue;
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private Network retrieveNetwork(string server)
        {
            foreach (Network i in sl)
            {
                if (i.server == server)
                {
                    return i;
                }
            }
            return null;
        }

        public void SendData(string network, string data, Configuration.Priority priority = Configuration.Priority.Normal)
        {
            Datagram line = new Datagram("RAW", data);
            string Pr = "Normal";
            switch (priority)
            { 
                case Configuration.Priority.High:
                    Pr = "High";
                    break;
                case Configuration.Priority.Low:
                    Pr = "Low";
                    break;
            }
            line.Parameters.Add("network", network);
            line.Parameters.Add("priority", Pr);
            Deliver(line);
        }

        public override void Part(string name, Network network = null)
        {
            Transfer("PART " + name);
        }

        public bool Process(string dg)
        {
            try
            {
                string network = "";
                System.Xml.XmlDocument datagram = new System.Xml.XmlDocument();
                datagram.LoadXml(dg);
                foreach (XmlNode curr in datagram.ChildNodes)
                {
                    switch (curr.Name.ToUpper())
                    {
                        case "SLOAD":
                            windows["!root"].scrollback.InsertText(curr.InnerText, Scrollback.MessageStyle.System, false);
                            break;
                        case "SRAW":
                            if (curr.InnerText == "PERMISSIONDENY")
                            {
                                windows["!root"].scrollback.InsertText("You can't send this command to server, because you aren't logged in", Scrollback.MessageStyle.System, false);
                                break;
                            }
                            windows["!root"].scrollback.InsertText("Server responded to SRAW with this: " + curr.InnerText, Scrollback.MessageStyle.User, false);
                            break;
                        case "SSTATUS":
                            switch (curr.InnerText)
                            {
                                case "Connected":
                                    ConnectionStatus = Status.Connected;
                                    break;
                                case "WaitingPW":
                                    ConnectionStatus = Status.WaitingPW;
                                    break;
                            }
                            break;
                        case "SDATA":
                            string name = curr.Attributes[0].Value;
                            Network server = null;
                            server = retrieveNetwork(name);
                            if (server == null)
                            {
                                server = new Network(name, this);
                                sl.Add(server);
                                server.nickname = nick;
                                server.Connected = true;
                            }
                            ProtocolIrc.processIRC(server, this, curr.InnerText, server.server, "!" + server.server, ref pong);
                            break;
                        case "SNICK":
                            Network sv = retrieveNetwork( curr.Attributes[0].Value );
                            if (sv != null)
                            {
                                sv.nickname = curr.InnerText;
                                windows["!" + sv.server].scrollback.InsertText("Your nick was changed to " + curr.InnerText, Scrollback.MessageStyle.User, true);
                            }
                            break;
                        case "SCONNECT":
                            network = curr.Attributes[0].Value;
                            switch (curr.InnerText)
                            {
                                case "CONNECTED":
                                    windows["!root"].scrollback.InsertText("You are already connected to " + network, Scrollback.MessageStyle.System);
                                    continue;
                                case "PROBLEM":
                                    windows["!root"].scrollback.InsertText(messages.get("service_error", Core.SelectedLanguage, new List<string> { network, curr.Attributes[1].Value }), Scrollback.MessageStyle.System, false);
                                    continue;
                                case "OK":
                                    Network _network = new Network(network, this);
                                    _network.Connected = true;
                                    _network.nickname = nick;
                                    sl.Add(_network);
                                    continue;
                            }
                            break;
                        case "SGLOBALIDENT":
                            windows["!root"].scrollback.InsertText(messages.get("pidgeon.globalident", Core.SelectedLanguage, new List<string> { curr.InnerText }), Scrollback.MessageStyle.User, true);
                            break;
                        case "SGLOBALNICK":
                            nick = curr.InnerText;
                            windows["!root"].scrollback.InsertText(messages.get("pidgeon.globalnick", Core.SelectedLanguage, new List<string> { curr.InnerText }), Scrollback.MessageStyle.User, true);
                            break;
                        case "SNETWORKINFO":
                            bool connected = false;
                            switch (curr.InnerText)
                            {
                                case "ONLINE":
                                    connected = true;
                                    break;
                                case "UNKNOWN":
                                    continue;

                                case "OFFLINE":
                                    connected = false;
                                    break;

                            }
                            foreach (Network s2 in sl)
                            {
                                if (curr.Attributes[0].Value == s2.server)
                                {
                                    s2.Connected = connected;
                                    break;
                                }
                            }
                            break;

                        case "SNETWORKLIST":
                            if (curr.InnerText != "")
                            {
                                string[] _networks = curr.InnerText.Split('|');
                                foreach (string i in _networks)
                                {
                                    if (i != "")
                                    {
                                        Deliver(new Datagram("NETWORKINFO", i));
                                        Network nw = new Network(i, this);
                                        nw.nickname = nick;
                                        sl.Add(nw);
                                        Datagram response = new Datagram("CHANNELINFO");
                                        response._InnerText = "LIST";
                                        response.Parameters.Add("network", i);
                                        Deliver(response);
                                        
                                    }
                                }
                            }
                            break;

                        case "SCHANNELINFO":
                            if (curr.InnerText == "")
                            {
                                if (curr.Attributes.Count > 1)
                                {
                                    if (curr.Attributes[1].Name == "channels")
                                    {
                                        string[] channellist = curr.Attributes[1].Value.Split('!');
                                        Network nw = retrieveNetwork(curr.Attributes[0].Value);
                                        if (nw != null)
                                        {
                                            foreach (string channel in channellist)
                                            {
                                                if (channel != "")
                                                {
                                                    nw.Join(channel);
                                                    SendData(nw.server, "TOPIC " + channel, Configuration.Priority.Normal);
                                                    SendData(nw.server, "MODE " + channel, Configuration.Priority.Low);
                                                    Datagram response2 = new Datagram("CHANNELINFO", "INFO");
                                                    response2.Parameters.Add("network", curr.Attributes[0].Value);
                                                    response2.Parameters.Add("channel", channel);
                                                    Deliver(response2);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                                if (curr.Attributes[1].Name == "channel")
                                {
                                    string[] userlist = curr.Attributes[2].Value.Split(':');
                                    Network nw = retrieveNetwork(curr.Attributes[0].Value);
                                    if (nw != null)
                                    {
                                        Channel channel = nw.getChannel(curr.Attributes[1].Value);

                                        if (channel != null)
                                        {
                                            foreach (string user in userlist)
                                            {
                                                if (user != "")
                                                {
                                                    string us = user.Substring(0, user.IndexOf("!"));
                                                    string ident = user.Substring(user.IndexOf("!") + 1);
                                                    if (ident.StartsWith("@"))
                                                    {
                                                        ident = "";
                                                    }
                                                    else
                                                    {
                                                        if (ident.Contains("@"))
                                                        {
                                                            ident = ident.Substring(0, user.IndexOf("@"));
                                                        }
                                                    }
                                                    string host = user.Substring(user.IndexOf("@") + 1);
                                                    if (host.StartsWith("+"))
                                                    {
                                                        host = "";
                                                    } else
                                                    {
                                                        host = user.Substring(0, host.IndexOf("+"));
                                                    }
                                                    User f2 = new User(us, host, nw, ident);
                                                    f2.ChannelMode.mode(user.Substring(user.IndexOf("+")));
                                                    channel.UserList.Add(f2);
                                                }
                                            }
                                            channel.redrawUsers();
                                        }
                                    }
                                }
                            break;
                        case "SAUTH":
                            if (curr.InnerText == "INVALID")
                            {
                                windows["!root"].scrollback.InsertText("You have supplied wrong password, connection closed", Scrollback.MessageStyle.System, false);
                                Exit();
                            }
                            if (curr.InnerText == "OK")
                            {
                                ConnectionStatus = Status.Connected;
                                windows["!root"].scrollback.InsertText("You are now logged in to pidgeon bnc", Scrollback.MessageStyle.System, false);
                            }
                            break;
                    }
                }
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
            return true;
        }

        public override void Exit()
        {
            Deliver(new Datagram("QUIT"));
            _writer.Close();
            _reader.Close();
        }

        public void Deliver(Datagram message)
        {
            XmlDocument datagram = new XmlDocument();
            XmlNode b1 = datagram.CreateElement(message._Datagram.ToUpper());
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

        public override bool Open()
        {
            type = 3;
            CreateChat("!root", true, null);
            main = new System.Threading.Thread(Start);
            Core._Main.ChannelList.insertSv(this);
            Core.SystemThreads.Add(main);
            main.Start();
            return true;
        }

        public override int requestNick(string _Nick)
        {
            Deliver(new Datagram("GLOBALNICK", _Nick));
            return 0;
        }

        public override int Message2(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            Transfer("PRIVMSG " + to + " :" + delimiter.ToString() + "ACTION " + text + delimiter.ToString(), _priority);
            return 0;
        }

        public override int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            Transfer("PRIVMSG " + to + " :" + text, _priority);
            return 0;
        }

        public override bool ConnectTo(string server, int port)
        {
            while (server.StartsWith(" "))
            {
                server = server.Substring(1);
            }
            if (server == "")
            {
                return false;
            }
            if (ConnectionStatus == Status.Connected)
            {
                windows["!root"].scrollback.InsertText("Connecting to " + server, Scrollback.MessageStyle.User, true);
                Datagram request = new Datagram("CONNECT", server);
                request.Parameters.Add("port", port.ToString());
                Deliver(request);
            }
            return true;
        }

        /// <summary>
        /// Check if it's a valid data
        /// </summary>
        /// <param name="datagram"></param>
        /// <returns></returns>
        public bool Valid(string datagram)
        {
            if (datagram.StartsWith("<") && datagram.EndsWith(">"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Write raw data
        /// </summary>
        /// <param name="text"></param>
        public void Send(string text)
        {
            try
            {
                _writer.WriteLine(text);
                _writer.Flush();
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        public override void Join(string name, Network network = null)
        {
            Transfer("JOIN " + name);
        }

        public override void Transfer(string text, Configuration.Priority Pr = Configuration.Priority.Normal)
        {
            if (Core.network != null && sl.Contains(Core.network))
            {
                Datagram blah = new Datagram("RAW", text);
                blah.Parameters.Add("network", Core.network.server);
                Deliver(blah);
            }
        }
    }
}
