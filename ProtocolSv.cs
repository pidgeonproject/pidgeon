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
        public System.Threading.Thread main = null;
        public System.Threading.Thread keep = null;
        public DateTime pong = DateTime.Now;

        private System.Net.Sockets.NetworkStream _networkStream;
        private System.IO.StreamReader _reader = null;
        /// <summary>
        /// List of networks loaded on server
        /// </summary>
        public List<Network> NetworkList = new List<Network>();
        private System.IO.StreamWriter _writer = null;
        public string password = "";
        public List<Cache> cache = new List<Cache>();
        private Status ConnectionStatus = Status.WaitingPW;



        public string nick = "";
        public bool auth = false;
        public class Cache
        {
            public int size = 0;
            public int current = 0;
        }

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

        public enum Status
        {
            WaitingPW,
            Connected,
        }

        public void _Ping()
        {
            try
            {
                while (true)
                {
                    Deliver(new Datagram("PING"));
                    System.Threading.Thread.Sleep(480000);
                }
            }
            catch (Exception)
            {
                Core.DebugLog("ProtocolSv: Exception in Ping()");
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
                return true;
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
            return false;
        }

        public void Start()
        {
            Core._Main.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }),
                Scrollback.MessageStyle.System);
            try
            {
                _networkStream = new System.Net.Sockets.TcpClient(Server, Port).GetStream();

                _writer = new System.IO.StreamWriter(_networkStream);
                _reader = new System.IO.StreamReader(_networkStream, Encoding.UTF8);

                Connected = true;

                Deliver(new Datagram("PING"));
                Deliver(new Datagram("LOAD"));
                

                Datagram login = new Datagram("AUTH", "");
                login.Parameters.Add("user", nick);
                login.Parameters.Add("pw", password);
                Deliver(login);
                Deliver(new Datagram("GLOBALNICK"));
                Deliver(new Datagram("NETWORKLIST"));
                Deliver(new Datagram("STATUS"));


                keep = new System.Threading.Thread(_Ping);
                Core.SystemThreads.Add(keep);
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
                    Core.trafficscanner.insert(Server, " >> " + text);
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
            catch (System.IO.IOException fail)
            {
                if (Connected)
                {
                    Core._Main.Chat.scrollback.InsertText("Quit: " + fail.Message, Scrollback.MessageStyle.System);
                }
            }
            catch (System.Threading.ThreadAbortException) {
                if (Core.IgnoreErrors)
                {
                    return;
                }
            }
            catch (Exception fail)
            {
                if (Connected)
                {
                    Core.handleException(fail);
                }
                Core.killThread(System.Threading.Thread.CurrentThread);
            }
        }

        private Network retrieveNetwork(string server)
        {
            foreach (Network i in NetworkList)
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

        public void ProccessDG(string text, long time, bool backlog, Network server, string name2, string number)
        {
            try
            {
                if (server == null)
                {
                    server = new Network(name2, this);
                    NetworkList.Add(server);
                    cache.Add(new Cache());
                    server.nickname = nick;
                    server.Connected = true;
                }
                if (backlog)
                {
                    Core._Main.Status("Retrieving backlog from " + name2 + ", got " + number + " packets from total of " + cache[NetworkList.IndexOf(server)].size.ToString() + " datagrams");
                    if ((cache[NetworkList.IndexOf(server)].size - 2) < int.Parse(number))
                    {
                        Core._Main.Status("");
                        foreach (Channel i in server.Channels)
                        {
                            i.temporary_hide = false;
                            i.parsing_xe = false;
                            i.parsing_xb = false;
                            i.parsing_who = false;
                        }
                    }
                    if (text.StartsWith("PRIVMSG "))
                    {
                        string channel = text.Substring(8);
                        
                        if (channel == "" || !channel.Contains(" :"))
                        {
                            Core.DebugLog("Invalid channel provided by services");
                            return;
                        }
                        string message = channel.Substring(channel.IndexOf(" :") + 2);
                        channel = channel.Substring(0, channel.IndexOf(" :"));
                        Channel item = server.getChannel(channel);
                        if (item == null)
                        {
                            Core.DebugLog("Invalid channel provided by services");
                            return;
                        }

                        Window x = item.retrieveWindow();

                        if (x == null)
                        {
                            Core.DebugLog("Invalid window for a channel");
                            return;
                        }

                        Core._Main.Chat.scrollback.InsertText(Core.network._protocol.PRIVMSG(Core.network.nickname, message), Scrollback.MessageStyle.Message, false, time, true);

                        return;
                    }
                    Core.DebugLog("Unknown message: " + text);
                    return;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
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
                        case "SMESSAGE":
                            string message_nick = curr.Attributes[0].Value;
                            string message_text = curr.InnerText;
                            string message_target = curr.Attributes[3].Value;
                            Window message_window = null;
                            Network mn = retrieveNetwork(curr.Attributes[1].Value);
                            long message_time = long.Parse(curr.Attributes[2].Value);
                            if (mn != null)
                            {
                                Channel target = mn.getChannel(message_target);
                                if (target != null)
                                {
                                    message_window = target.retrieveWindow();
                                }
                                else
                                {
                                    Core.DebugLog("There is no channel " + message_target);
                                }
                            }


                            if (message_window != null)
                            {
                                message_window.scrollback.InsertText(mn._protocol.PRIVMSG(message_nick, message_text), Scrollback.MessageStyle.Message, false, message_time, true);
                            }
                            else
                            {
                                Core.DebugLog("There is no window for " + message_target);
                            }

                            break;
                        case "SLOAD":
                            windows["!root"].scrollback.InsertText(curr.InnerText, Scrollback.MessageStyle.System, false);
                            break;
                        case "SRAW":
                            if (curr.InnerText == "PERMISSIONDENY")
                            {
                                windows["!root"].scrollback.InsertText("You can't send this command to server, because you aren't logged in",
                                    Scrollback.MessageStyle.System, false);
                                break;
                            }
                            windows["!root"].scrollback.InsertText("Server responded to SRAW with this: " + curr.InnerText,
                                Scrollback.MessageStyle.User, false);
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
                        case "SSELFDG":
                            long date2 = 0;
                            bool backlog2 = false;
                            string id2 = "";
                            foreach (XmlAttribute time in curr.Attributes)
                            {
                                if (time.Name == "time")
                                {
                                    date2 = long.Parse(time.Value);
                                }
                            }
                            foreach (XmlAttribute i in curr.Attributes)
                            {
                                if (i.Name == "buffer")
                                {
                                    id2 = i.Value;
                                    backlog2 = true;
                                }
                            }
                            string name2 = curr.Attributes[0].Value;
                            Network server = null;
                            server = retrieveNetwork(name2);
                            ProccessDG(curr.InnerText, date2, backlog2, server, name2, id2);
                            break;
                        case "SDATA":
                            long date = 0;
                            bool backlog = false;
                            string id = "";
                            foreach (XmlAttribute time in curr.Attributes)
                            {
                                if (time.Name == "time")
                                {
                                    if (!long.TryParse(time.Value, out date))
                                    {
                                        Core.DebugLog("Warning: " + time.Value + " is not a correct time");
                                    }
                                }
                            }
                            foreach (XmlAttribute i in curr.Attributes)
                            {
                                if (i.Name == "buffer")
                                {
                                    id = i.Value;
                                    backlog = true;
                                }
                            }
                            string name = curr.Attributes[0].Value;
                            Network server4 = null;
                            server4 = retrieveNetwork(name);
                            if (server4 == null)
                            {
                                server4 = new Network(name, this);
                                NetworkList.Add(server4);
                                cache.Add(new Cache());
                                server4.nickname = nick;
                                server4.Connected = true;
                            }
                            if (backlog)
                            {
                                if (Core._Main.toolStripProgressBar1.Visible == false)
                                {
                                    Core._Main.toolStripProgressBar1.Value = int.Parse(id);
                                    Core._Main.toolStripProgressBar1.Visible = true;
                                    Core._Main.toolStripProgressBar1.Maximum = cache[NetworkList.IndexOf(server4)].size;
                                }
                                Core._Main.toolStripProgressBar1.Value = int.Parse(id);
                                Core._Main.Status("Retrieving backlog from " + name + ", got " + id + " packets from total of " + cache[NetworkList.IndexOf(server4)].size.ToString() + " datagrams");
                                if ((cache[NetworkList.IndexOf(server4)].size - 2) < int.Parse(id))
                                {
                                    Core._Main.Status("");
                                    Core._Main.toolStripProgressBar1.Visible = false;
                                    Core._Main.toolStripProgressBar1.Value = 0;
                                    foreach (Channel i in server4.Channels)
                                    {
                                        i.temporary_hide = false;
                                        i.parsing_xe = false;
                                        i.parsing_xb = false;
                                        i.parsing_who = false;
                                    }
                                }
                                ProcessorIRC processor = new ProcessorIRC(server4, this, curr.InnerText, server4.server, server4.sw, ref pong, date, false);
                                processor.Result();
                                break;
                            }
                            ProcessorIRC processor2 = new ProcessorIRC(server4, this, curr.InnerText, server4.server, server4.sw, ref pong, date);
                            processor2.Result();
                            break;
                        case "SNICK":
                            Network sv = retrieveNetwork(curr.Attributes[0].Value);
                            if (sv != null)
                            {
                                sv.nickname = curr.InnerText;
                                windows["!" + sv.window].scrollback.InsertText("Your nick was changed to " + curr.InnerText, 
                                    Scrollback.MessageStyle.User, true);
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
                                    cache.Add(new Cache());
                                    NetworkList.Add(_network);
                                    continue;
                            }
                            break;
                        case "SGLOBALIDENT":
                            windows["!root"].scrollback.InsertText(messages.get("pidgeon.globalident", Core.SelectedLanguage,
                                new List<string> { curr.InnerText }), Scrollback.MessageStyle.User, true);
                            break;
                        case "SBACKLOG":
                            network = curr.Attributes[0].Value;
                            server = retrieveNetwork(network);
                            if (server != null)
                            {
                                cache[NetworkList.IndexOf(server)].size = int.Parse(curr.InnerText);
                                foreach (Channel i in server.Channels)
                                {
                                    i.parsing_who = true;
                                    i.parsing_xb = true;
                                    i.temporary_hide = true;
                                }
                            }
                            break;
                        case "SGLOBALNICK":
                            nick = curr.InnerText;
                            windows["!root"].scrollback.InsertText(messages.get("pidgeon.globalnick", Core.SelectedLanguage,
                                new List<string> { curr.InnerText }), Scrollback.MessageStyle.User, true);
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
                            foreach (Network s2 in NetworkList)
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
                                        cache.Add(new Cache());
                                        NetworkList.Add(nw);
                                        Datagram response = new Datagram("CHANNELINFO");
                                        response._InnerText = "LIST";
                                        response.Parameters.Add("network", i);
                                        Deliver(response);
                                        Datagram request = new Datagram("BACKLOGSV");
                                        request.Parameters.Add("network", i);
                                        request.Parameters.Add("size", Configuration.Depth.ToString());
                                        Deliver(request);
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
                                                    if (nw.getChannel(channel) == null)
                                                    {
                                                        nw.Join(channel, true);
                                                    }
                                                    if (!Configuration.Retrieve_Sv)
                                                    {
                                                        SendData(nw.server, "TOPIC " + channel, Configuration.Priority.Normal);
                                                        SendData(nw.server, "MODE " + channel, Configuration.Priority.Low);
                                                    }
                                                    Datagram response2 = new Datagram("CHANNELINFO", "INFO");
                                                    response2.Parameters.Add("network", curr.Attributes[0].Value);
                                                    response2.Parameters.Add("channel", channel);
                                                    Deliver(response2);
                                                }
                                            }
                                            System.Threading.Thread.Sleep(800);
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

                                            if (user.Contains("!") && user.Contains("@"))
                                            {
                                                string us = "";
                                                string ident;
                                                us = user.Substring(0, user.IndexOf("!"));
                                                if (channel.containsUser(us))
                                                {
                                                    continue;
                                                }
                                                ident = user.Substring(user.IndexOf("!") + 1);
                                                if (ident.StartsWith("@"))
                                                {
                                                    ident = "";
                                                }
                                                else
                                                {
                                                    if (ident.Contains("@"))
                                                    {
                                                        ident = ident.Substring(0, ident.IndexOf("@"));
                                                    }
                                                }
                                                string host = user.Substring(user.IndexOf("@") + 1);
                                                if (host.StartsWith("+"))
                                                {
                                                    host = "";
                                                }
                                                else
                                                {
                                                    if (host.Contains("+"))
                                                    {
                                                        host = host.Substring(0, host.IndexOf("+"));
                                                    }
                                                }
                                                User f2 = new User(us, host, nw, ident);
                                                if (user.Contains("+") && !user.StartsWith("+"))
                                                {
                                                    f2.ChannelMode.mode(user.Substring(user.IndexOf("+")));
                                                }
                                                channel.UserList.Add(f2);
                                            }

                                        }
                                        channel.redrawUsers();
                                        channel.UpdateInfo();
                                    }
                                }
                            }
                            break;
                        case "SAUTH":
                            if (curr.InnerText == "INVALID")
                            {
                                windows["!root"].scrollback.InsertText("You have supplied wrong password, connection closed",
                                    Scrollback.MessageStyle.System, false);
                                Connected = false;
                                Exit();
                            }
                            if (curr.InnerText == "OK")
                            {
                                ConnectionStatus = Status.Connected;
                                windows["!root"].scrollback.InsertText("You are now logged in to pidgeon bnc", Scrollback.MessageStyle.System, false);
                                windows["!root"].scrollback.InsertText(curr.Attributes[0].Value, Scrollback.MessageStyle.System);
                            }
                            break;
                    }
                }
            }
            catch (System.Xml.XmlException) { }
            catch (Exception f)
            {
                Core.handleException(f);
            }
            return true;
        }

        public override void Exit()
        {
            Connected = false;
            lock (NetworkList)
            {
                foreach (Network network in NetworkList)
                {
                    network.Connected = false;
                    if (Core.network == network)
                    {
                        Core.network = null;
                    }
                }
                NetworkList.Clear();
            }
            if (keep != null)
            {
                keep.Abort();
                Core.killThread(keep);
            }
            if (_writer != null)  _writer.Close();
            if (_reader != null)  _reader.Close();
            base.Exit();
        }

        public void Deliver(Datagram message)
        {
            Send(message.ToDocumentXmlText());
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
            Core._Main.Chat.scrollback.InsertText(">>>>>>" + Core.network.nickname + " " + text, Scrollback.MessageStyle.Action);
            Transfer("PRIVMSG " + to + " :" + delimiter.ToString() + "ACTION " + text + delimiter.ToString(), _priority);
            return 0;
        }

        public override int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            if (!pmsg)
            {
                //Core._Main.Chat.scrollback.InsertText(Core.network._protocol.PRIVMSG(Core.network.nickname, text),
                //    Scrollback.MessageStyle.Message, true, 0, true);
            }
            //Transfer("PRIVMSG " + to + " :" + text, _priority);
            Datagram message = new Datagram("MESSAGE", text);
            if (Core.network != null && NetworkList.Contains(Core.network))
            {
                message.Parameters.Add("network", Core.network.server);
                message.Parameters.Add("priority", _priority.ToString());
                message.Parameters.Add("to", to);
                Deliver(message);
            }
            else
            {
                Core.DebugLog("Invalid network");
            }
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
            if (Connected)
            {
                try
                {
                    _writer.WriteLine(text);
                    Core.trafficscanner.insert(Server, " << " + text);
                    _writer.Flush();
                }
                catch (System.IO.IOException er)
                {
                    windows["!root"].scrollback.InsertText(er.Message, Scrollback.MessageStyle.User);
                    Exit();
                }
                catch (Exception f)
                {
                    if (Connected)
                    {
                        Core.handleException(f);
                    }
                }

            }
        }

        public override void Join(string name, Network network = null)
        {
            Transfer("JOIN " + name);
        }

        public override void Transfer(string text, Configuration.Priority Pr = Configuration.Priority.Normal, Network network = null)
        {
            if (network == null)
            {
                if (Core.network != null && NetworkList.Contains(Core.network))
                {
                    Datagram blah = new Datagram("RAW", text);
                    blah.Parameters.Add("network", Core.network.server);
                    Deliver(blah);
                    return;
                }
            }
            else
            {
                if (NetworkList.Contains(network))
                {
                    Datagram blah = new Datagram("RAW", text);
                    blah.Parameters.Add("network", network.server);
                    Deliver(blah);
                }
                else
                {
                    Core.DebugLog("Network is not a part of this services connection " + network.server);
                }
            }
        }
    }
}
