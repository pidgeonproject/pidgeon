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
                            processIRC(curr.InnerText, server);
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
                                                        ident = ident.Substring(0, user.IndexOf("@"));
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

        public void processIRC(string text, Network network)
        {
            if (text.StartsWith(":"))
            {
                string[] data = text.Split(':');
                if (data.Length > 1)
                {
                    string command = "";
                    string parameters = "";
                    string command2 = "";
                    string source;
                    string _value;
                    source = text.Substring(1);
                    source = source.Substring(0, source.IndexOf(" "));
                    command2 = text.Substring(1);
                    command2 = command2.Substring(source.Length + 1);
                    if (command2.Contains(" :"))
                    {
                        command2 = command2.Substring(0, command2.IndexOf(" :"));
                    }
                    string[] _command = command2.Split(' ');
                    if (_command.Length > 0)
                    {
                        command = _command[0];
                    }
                    if (_command.Length > 1)
                    {
                        int curr = 1;
                        while (curr < _command.Length)
                        {
                            parameters += _command[curr] + " ";
                            curr++;
                        }
                        if (parameters.EndsWith(" "))
                        {
                            parameters = parameters.Substring(0, parameters.Length - 1);
                        }
                    }
                    _value = "";
                    if (text.Length > 3 + command2.Length + source.Length)
                    {
                        _value = text.Substring(3 + command2.Length + source.Length);
                    }
                    if (_value.StartsWith(":"))
                    {
                        _value = _value.Substring(1);
                    }

                    if (command == "PONG")
                    {
                        pong = DateTime.Now;
                        return;
                    }
                    if (data[1].Contains(" "))
                    {
                        string[] code = data[1].Split(' ');
                        switch (command)
                        {
                            case "313":
                            //whois
                            case "318":
                                break;
                            case "332":
                                if (code.Length > 3)
                                {
                                    string name = "";
                                    if (parameters.Contains("#"))
                                    {
                                        name = parameters.Substring(parameters.IndexOf("#")).Replace(" ", "");
                                    }
                                    string topic = _value;
                                    Channel channel = network.getChannel(name);
                                    if (channel != null)
                                    {
                                        Window curr = channel.retrieveWindow();
                                        if (curr != null)
                                        {
                                            while (curr.scrollback == null)
                                            {
                                                System.Threading.Thread.Sleep(100);
                                            }
                                            curr.scrollback.InsertText("Topic: " + topic, Scrollback.MessageStyle.Channel);
                                        }
                                        channel.Topic = topic;
                                        return;
                                    }
                                }
                                break;
                            case "315":
                                if (code.Length > 2)
                                {
                                    Channel channel = network.getChannel(code[3]);
                                    if (channel != null)
                                    {
                                        channel.redrawUsers();
                                    }
                                }
                                break;
                            case "333":
                                if (code.Length > 5)
                                {
                                    string name = code[3];
                                    string user = code[4];
                                    string time = code[5];
                                    Channel channel = network.getChannel(name);
                                    if (channel != null)
                                    {
                                        channel.TopicDate = int.Parse(time);
                                        channel.TopicUser = user;
                                        Window curr = channel.retrieveWindow();
                                        if (Core.windowReady(curr))
                                        {
                                            curr.scrollback.InsertText("Topic by: " + user + " date " + ProtocolIrc.convertUNIX(time), Scrollback.MessageStyle.Channel);
                                            return;
                                        }
                                    }
                                }
                                break;
                            case "352":
                                // cameron.freenode.net 352 petan2 #debian thelineva nikita.tnnet.fi kornbluth.freenode.net t0h H :0 Tommi Helineva
                                if (code.Length > 6)
                                {
                                    Channel channel = network.getChannel(code[3]);
                                    string ident = code[4];
                                    string host = code[5];
                                    string nick = code[7];
                                    string server = code[6];
                                    if (channel != null)
                                    {
                                        if (!channel.containUser(nick))
                                        {
                                            channel.UserList.Add(new User(nick, host, network, ident));
                                            channel.redrawUsers();
                                            break;
                                        }
                                        foreach (User u in channel.UserList)
                                        {
                                            if (u.Nick == nick)
                                            {
                                                u.Ident = ident;
                                                u.Host = host;

                                            }
                                        }
                                    }
                                }
                                break;

                            case "353":
                                if (code.Length > 3)
                                {
                                    string name = code[4];
                                    Channel channel = network.getChannel(name);
                                    if (channel != null)
                                    {
                                        string[] _chan = data[2].Split(' ');
                                        foreach (var user in _chan)
                                        {
                                            if (!channel.containUser(user) && user != "")
                                            {
                                                lock (channel.UserList)
                                                {
                                                    channel.UserList.Add(new User(user, "", network, ""));
                                                }
                                            }
                                        }
                                        channel.redrawUsers();
                                        return;
                                    }
                                }
                                break;
                            case "005":
                                // PREFIX=(qaohv)~&@%+ CHANMODES=beI,k
                                if (parameters.Contains("PREFIX=("))
                                {
                                    string cmodes = parameters.Substring(parameters.IndexOf("PREFIX=(") + 8);
                                    cmodes = cmodes.Substring(0, cmodes.IndexOf(")"));
                                    lock (this.CUModes)
                                    {
                                        CUModes.Clear();
                                        CUModes.AddRange(cmodes.ToArray<char>());
                                    }
                                    cmodes = parameters.Substring(parameters.IndexOf("PREFIX=(") + 8);
                                    cmodes = cmodes.Substring(cmodes.IndexOf(")") + 1, CUModes.Count);

                                    UChars.Clear();
                                    UChars.AddRange(cmodes.ToArray<char>());

                                }
                                if (parameters.Contains("CHANMODES="))
                                {
                                    string xmodes = parameters.Substring(parameters.IndexOf("CHANMODES=") + 11);
                                    xmodes = xmodes.Substring(0, xmodes.IndexOf(" "));
                                    string[] _mode = xmodes.Split(',');
                                    if (_mode.Length == 4)
                                    {
                                        PModes.Clear();
                                        CModes.Clear();
                                        XModes.Clear();
                                        SModes.Clear();
                                        PModes.AddRange(_mode[0].ToArray<char>());
                                        XModes.AddRange(_mode[1].ToArray<char>());
                                        SModes.AddRange(_mode[2].ToArray<char>());
                                        CModes.AddRange(_mode[3].ToArray<char>());
                                    }

                                }
                                break;
                            case "366":
                                return;
                            case "324":
                                if (code.Length > 3)
                                {
                                    string name = code[2];
                                    string topic = _value;
                                    Channel channel = network.getChannel(code[3]);
                                    if (channel != null)
                                    {
                                        Window curr = channel.retrieveWindow();
                                        if (curr != null)
                                        {
                                            channel._mode.mode(code[4]);
                                            curr.scrollback.InsertText("Mode: " + code[4], Scrollback.MessageStyle.Channel);
                                        }
                                        return;
                                    }
                                }
                                break;
                            //  367 petan # *!*@173.45.238.81
                            case "367":
                                if (code.Length > 6)
                                {
                                    string chan = code[3];
                                    Channel channel = network.getChannel(code[3]);
                                    if (channel != null)
                                    {
                                        if (channel.Bl == null)
                                        {
                                            channel.Bl = new List<SimpleBan>();
                                        }
                                        if (!channel.containsBan(code[4]))
                                        {
                                            channel.Bl.Add(new SimpleBan(code[5], code[4], code[6]));
                                            Core._Main.UpdateStatus();
                                        }
                                    }
                                }
                                break;
                            case "556":
                                break;
                        }
                    }
                    if (command == "INFO")
                    {
                        windows["!" + network.server].scrollback.InsertText(text.Substring(text.IndexOf("INFO") + 5), Scrollback.MessageStyle.User);
                        return;
                    }

                    if (command == "NOTICE")
                    {
                        windows["!" + network.server].scrollback.InsertText("[" + source + "] " + _value, Scrollback.MessageStyle.Message);
                        return;
                    }
                    if (source.StartsWith(network.nickname + "!"))
                    {
                        string[] _data2 = data[1].Split(' ');
                        if (_data2.Length > 2)
                        {
                            if (_data2[1].Contains("JOIN"))
                            {
                                string channel = _data2[2];
                                if (_data2[2].Contains("#") == false)
                                {
                                    channel = data[2];
                                }
                                Channel curr = network.Join(channel);
                                if (Configuration.aggressive_mode)
                                {
                                    Transfer("MODE " + channel, Configuration.Priority.Low);
                                }

                                if (Configuration.aggressive_exception)
                                {
                                    curr.parsing_xe = true;
                                    Transfer("MODE " + channel + " +e", Configuration.Priority.Low);
                                }

                                if (Configuration.aggressive_bans)
                                {
                                    curr.parsing_xb = true;
                                    Transfer("MODE " + channel + " +b", Configuration.Priority.Low);
                                }

                                if (Configuration.aggressive_invites)
                                {
                                    Transfer("MODE " + channel + " +I", Configuration.Priority.Low);
                                }

                                if (Configuration.aggressive_channel)
                                {
                                    curr.parsing_who = true;
                                    Transfer("WHO " + channel, Configuration.Priority.Low);
                                }
                                return;
                            }
                        }
                        if (_data2.Length > 2)
                        {
                            if (_data2[1].Contains("NICK"))
                            {
                                windows["!" + network.server].scrollback.InsertText(messages.get("protocolnewnick", Core.SelectedLanguage, new List<string> { _value }), Scrollback.MessageStyle.User);
                                network.nickname = _value;
                            }
                            if (_data2[1].Contains("PART"))
                            {
                                string channel = _data2[2];
                                if (_data2[2].Contains("#") == false)
                                {
                                    channel = data[2];
                                    Channel c = network.getChannel(channel);
                                    if (c != null)
                                    {
                                        Window Chat = c.retrieveWindow();
                                        if (Core.windowReady(Chat))
                                        {
                                            if (c.ok)
                                            {
                                                c.ok = false;
                                                Chat.scrollback.InsertText(messages.get("part1", Core.SelectedLanguage), Scrollback.MessageStyle.Message);
                                            }
                                            else
                                            {
                                                Chat.scrollback.InsertText(messages.get("part2", Core.SelectedLanguage), Scrollback.MessageStyle.Message);
                                            }
                                        }
                                        c.ok = false;
                                    }
                                }
                                return;
                            }
                        }
                    }

                    if (command == "PING")
                    {
                        Transfer("PONG ", Configuration.Priority.High);
                        return;
                    }

                    if (command == "NICK")
                    {
                        string nick = source.Substring(0, source.IndexOf("!"));
                        string _new = _value;
                        foreach (Channel item in network.Channels)
                        {
                            if (item.ok)
                            {
                                lock (item.UserList)
                                {
                                    foreach (User curr in item.UserList)
                                    {
                                        if (curr.Nick == nick)
                                        {
                                            curr.Nick = _new;
                                            item.redrawUsers();
                                            Window x = item.retrieveWindow();
                                            if (x != null)
                                            {
                                                x.scrollback.InsertText(messages.get("protocol-nick", Core.SelectedLanguage, new List<string> { nick, _new }), Scrollback.MessageStyle.Channel);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return;
                    }

                    if (command == "PRIVMSG")
                    {
                        string _nick;
                        string _ident;
                        string _host;
                        string chan;
                        _nick = source.Substring(0, source.IndexOf("!"));
                        _host = source.Substring(source.IndexOf("@") + 1);
                        _ident = source.Substring(source.IndexOf("!") + 1);
                        _ident = _ident.Substring(0, _ident.IndexOf("@"));
                        chan = parameters.Replace(" ", "");
                        string message = _value;
                        if (!chan.Contains(network.channel_prefix))
                        {
                            string uc;
                            if (message.StartsWith(delimiter.ToString()))
                            {
                                uc = message.Substring(1);
                                uc = uc.Substring(0, uc.IndexOf(delimiter.ToString()));
                                uc = uc.ToUpper();
                                switch (uc)
                                {
                                    case "VERSION":
                                        Transfer("NOTICE " + chan + " :" + delimiter.ToString() + "VERSION " + Configuration.Version + " http://pidgeonclient.org/wiki/", Configuration.Priority.Low);
                                        break;
                                    case "TIME":
                                        Transfer("NOTICE " + chan + " :" + delimiter.ToString() + "TIME " + DateTime.Now.ToString(), Configuration.Priority.Low);
                                        break;
                                    case "PING":
                                        break;
                                }
                                if (Configuration.DisplayCtcp)
                                {
                                    windows["!" + network.server].scrollback.InsertText("CTCP from (" + chan + ") " + message, Scrollback.MessageStyle.Message);
                                    return;
                                }
                                return;
                            }

                        }
                        User user = new User(_nick, _host, network, _ident);
                        Channel channel = null;
                        if (chan.StartsWith(network.channel_prefix))
                        {
                            channel = network.getChannel(chan);
                            if (channel != null)
                            {
                                Window window;
                                window = channel.retrieveWindow();
                                if (Core.windowReady(window))
                                {
                                    if (message.StartsWith(delimiter.ToString() + "ACTION"))
                                    {
                                        message = message.Substring("xACTION".Length);
                                        channel.retrieveWindow().scrollback.InsertText(">>>>>>" + _nick + message, Scrollback.MessageStyle.Action);
                                        return;
                                    }
                                    channel.retrieveWindow().scrollback.InsertText(PRIVMSG(user.Nick, message), Scrollback.MessageStyle.Message);
                                }
                                return;
                            }
                            return;
                        }
                        chan = source.Substring(source.IndexOf("!"));
                        if (!windows.ContainsKey(chan))
                        {
                            network.Private(chan);
                        }
                        windows[chan].scrollback.InsertText(PRIVMSG(source, message), Scrollback.MessageStyle.Channel);
                        return;
                    }

                    if (command == "TOPIC")
                    {
                        string chan = parameters;
                        chan = chan.Replace(" ", "");
                        string user = source.Substring(0, source.IndexOf("!"));
                        Channel channel = network.getChannel(chan);
                        if (channel != null)
                        {
                            Window window;
                            channel.Topic = _value;
                            window = channel.retrieveWindow();
                            if (Core.windowReady(window))
                            {
                                channel.retrieveWindow().scrollback.InsertText(messages.get("channel-topic", Core.SelectedLanguage, new List<string> { source, _value }), Scrollback.MessageStyle.Channel);
                                return;
                            }
                        }
                    }

                    if (command == "MODE")
                    {
                        if (parameters.Contains(" "))
                        {
                            string chan = parameters.Substring(0, parameters.IndexOf(" "));
                            chan = chan.Replace(" ", "");
                            string user = source;
                            if (chan.StartsWith(network.channel_prefix))
                            {
                                Channel channel = network.getChannel(chan);
                                if (channel != null)
                                {
                                    Window window;
                                    window = channel.retrieveWindow();
                                    string change = parameters.Substring(parameters.IndexOf(" "));
                                    if (Core.windowReady(window))
                                    {
                                        channel.retrieveWindow().scrollback.InsertText(messages.get("channel-mode", Core.SelectedLanguage, new List<string> { source, parameters.Substring(parameters.IndexOf(" ")) }), Scrollback.MessageStyle.Action);
                                    }

                                    while (change.StartsWith(" "))
                                    {
                                        change = change.Substring(1);
                                    }

                                    channel._mode.mode(change);

                                    while (change.EndsWith(" ") && change.Length > 1)
                                    {
                                        change = change.Substring(0, change.Length - 1);
                                    }

                                    if (change.Contains(" "))
                                    {
                                        string header = change.Substring(0, change.IndexOf(" "));
                                        List<string> parameters2 = new List<string>();
                                        parameters2.AddRange(change.Substring(change.IndexOf(" ") + 1).Split(' '));
                                        int curr = 0;

                                        char type = ' ';

                                        foreach (char m in header)
                                        {

                                            if (m == '+')
                                            {
                                                type = '+';
                                            }
                                            if (m == '-')
                                            {
                                                type = '-';
                                            }
                                            if (type == ' ')
                                            {
                                                continue;
                                            }
                                            if (CUModes.Contains(m) && curr <= parameters2.Count)
                                            {
                                                User flagged_user = channel.userFromName(parameters2[curr]);
                                                if (flagged_user != null)
                                                {
                                                    flagged_user.ChannelMode.mode(type.ToString() + m.ToString());
                                                }
                                                curr++;
                                                channel.redrawUsers();
                                            }
                                            if (parameters2.Count > curr)
                                            {
                                                switch (m.ToString())
                                                {
                                                    case "b":
                                                        if (channel.Bl == null)
                                                        {
                                                            channel.Bl = new List<SimpleBan>();
                                                        }
                                                        lock (channel.Bl)
                                                        {
                                                            if (type == '-')
                                                            {
                                                                SimpleBan br = null;
                                                                foreach (SimpleBan xx in channel.Bl)
                                                                {
                                                                    if (xx._Target == parameters2[curr])
                                                                    {
                                                                        br = xx;
                                                                        break;
                                                                    }
                                                                }
                                                                if (br != null)
                                                                {
                                                                    channel.Bl.Remove(br);
                                                                }
                                                                break;
                                                            }
                                                            channel.Bl.Add(new SimpleBan(user, parameters2[curr], ""));
                                                        }
                                                        curr++;
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                    }

                    if (command == "PART")
                    {
                        string chan = parameters;
                        chan = chan.Replace(" ", "");
                        string user = source.Substring(0, source.IndexOf("!"));
                        Channel channel = network.getChannel(chan);
                        if (channel != null)
                        {
                            Window window;
                            window = channel.retrieveWindow();
                            User delete = null;
                            if (Core.windowReady(window))
                            {
                                channel.retrieveWindow().scrollback.InsertText(messages.get("window-p1", Core.SelectedLanguage, new List<string> { source, _value }), Scrollback.MessageStyle.Part);

                                if (channel.containUser(user))
                                {
                                    lock (channel.UserList)
                                    {
                                        foreach (User _user in channel.UserList)
                                        {
                                            if (_user.Nick == user)
                                            {
                                                delete = _user;
                                                break;
                                            }
                                        }
                                    }

                                    if (delete != null)
                                    {
                                        channel.UserList.Remove(delete);
                                    }

                                    channel.redrawUsers();
                                    return;
                                }
                            }
                        }
                    }

                    if (command == "QUIT")
                    {
                        string nick = source.Substring(0, source.IndexOf("!"));
                        string _new = _value;
                        foreach (Channel item in network.Channels)
                        {
                            if (item.ok)
                            {
                                User target = null;
                                lock (item.UserList)
                                {
                                    foreach (User curr in item.UserList)
                                    {
                                        if (curr.Nick == nick)
                                        {
                                            target = curr;
                                            break;
                                        }
                                    }
                                }
                                if (target != null)
                                {
                                    Window x = item.retrieveWindow();
                                    if (x != null)
                                    {
                                        x.scrollback.InsertText(messages.get("protocol-quit", Core.SelectedLanguage, new List<string> { source, _value }), Scrollback.MessageStyle.Join);
                                    }
                                    lock (item.UserList)
                                    {
                                        item.UserList.Remove(target);
                                    }
                                    item.redrawUsers();
                                }
                            }
                        }
                        return;
                    }

                    if (command == "KICK")
                    {
                        string nick = _command[1];
                        string _new = _value;

                        //continue;
                    }

                    if (command == "JOIN")
                    {
                        string chan = parameters;
                        chan = chan.Replace(" ", "");
                        string user = source.Substring(0, source.IndexOf("!"));
                        string _ident;
                        string _host;
                        _host = source.Substring(source.IndexOf("@") + 1);
                        _ident = source.Substring(source.IndexOf("!") + 1);
                        _ident = _ident.Substring(0, _ident.IndexOf("@"));
                        Channel channel = network.getChannel(chan);
                        if (channel != null)
                        {
                            Window window;
                            window = channel.retrieveWindow();
                            if (Core.windowReady(window))
                            {
                                channel.retrieveWindow().scrollback.InsertText(messages.get("join", Core.SelectedLanguage, new List<string> { source }), Scrollback.MessageStyle.Join);

                                if (!channel.containUser(user))
                                {
                                    lock (channel.UserList)
                                    {
                                        channel.UserList.Add(new User(user, _host, network, _ident));
                                    }
                                    channel.redrawUsers();
                                }
                                return;
                            }
                        }
                    }
                }
                windows["!" + network.server].scrollback.InsertText(text, Scrollback.MessageStyle.System);
            }
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
