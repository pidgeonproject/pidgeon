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
using System.Linq;
using System.Text;
using System.Net;

namespace Client
{
    class ProtocolIrc : Protocol
    {
        public System.Threading.Thread main;
        public System.Threading.Thread deliveryqueue;
        public System.Threading.Thread keep;
        public DateTime pong = DateTime.Now;

        private System.Net.Sockets.NetworkStream _network;
        private System.IO.StreamReader _reader;
        public Network _server;
        private System.IO.StreamWriter _writer;
        Messages _messages = new Messages();

        class Messages
        {
            public struct Message
            {
                public Configuration.Priority _Priority;
                public string message;
            }
            public List<Message> messages = new List<Message>();
            public List<Message> newmessages = new List<Message>();
            public ProtocolIrc protocol;

            public void DeliverMessage(string Message, Configuration.Priority Pr = Configuration.Priority.Normal)
            {
                Message text = new Message();
                text._Priority = Pr;
                text.message = Message;
                lock (messages)
                {
                    messages.Add(text);
                    return;
                }
            }

            public void Run()
            {
                while (true)
                {
                    try
                    {
                        if (messages.Count > 0)
                        {
                            lock (messages)
                            {
                                newmessages.AddRange(messages);
                                messages.Clear();
                            }
                        }
                        if (newmessages.Count > 0)
                        {
                            List<Message> Processed = new List<Message>();
                            Configuration.Priority highest = Configuration.Priority.Low;
                            while (newmessages.Count > 0)
                            {
                                // we need to get all messages that have been scheduled to be send
                                lock (messages)
                                {
                                    if (messages.Count > 0)
                                    {
                                        newmessages.AddRange(messages);
                                        messages.Clear();
                                    }
                                }
                                highest = Configuration.Priority.Low;
                                // we need to check the priority we need to handle first
                                foreach (Message message in newmessages)
                                {
                                    if (message._Priority > highest)
                                    {
                                        highest = message._Priority;
                                        if (message._Priority == Configuration.Priority.High)
                                        {
                                            break;
                                        }
                                    }
                                }
                                // send highest priority first
                                foreach (Message message in newmessages)
                                {
                                    if (message._Priority >= highest)
                                    {
                                        Processed.Add(message);
                                        protocol.Send(message.message);
                                        System.Threading.Thread.Sleep(1000);
                                        if (highest != Configuration.Priority.High)
                                        {
                                            break;
                                        }
                                    }
                                }
                                foreach (Message message in Processed)
                                {
                                    if (newmessages.Contains(message))
                                    {
                                        newmessages.Remove(message);
                                    }
                                }
                            }
                        }
                        newmessages.Clear();
                        System.Threading.Thread.Sleep(200);
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        return;
                    }
                }
            }
        }

        public override void Part(string name, Network network = null)
        {
            Transfer("PART " + name);
        }

        public override void Transfer(string text, Configuration.Priority Pr = Configuration.Priority.Normal)
        {
            _messages.DeliverMessage(text, Pr);
        }

        public string convertUNIX(string time)
        {
            long baseTicks = 621355968000000000;
            long tickResolution = 10000000;

            long epoch = (DateTime.Now.ToUniversalTime().Ticks - baseTicks) / tickResolution;
            long epochTicks = (epoch * tickResolution) + baseTicks;
            return new DateTime(epochTicks, DateTimeKind.Utc).ToString();
        }

        public void _Ping()
        {
            try
            {
                while (_server.Connected)
                {
                    Transfer("PING :" + _server._protocol.Server, Configuration.Priority.High);
                    System.Threading.Thread.Sleep(24000);
                }
            }
            catch (Exception)
            { }
        }

        public void Start()
        {
            _messages.protocol = this;
            Core._Main.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }), Scrollback.MessageStyle.System);
            try
            {
                _network = new System.Net.Sockets.TcpClient(Server, Port).GetStream();

                _server.Connected = true;

                _writer = new System.IO.StreamWriter(_network);
                _reader = new System.IO.StreamReader(_network, Encoding.UTF8);

                _writer.WriteLine("USER " + _server.username + " 8 * :" + _server.ident);
                _writer.WriteLine("NICK " + _server.nickname);
                _writer.Flush();

                keep = new System.Threading.Thread(_Ping);
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
                deliveryqueue = new System.Threading.Thread(_messages.Run);
                deliveryqueue.Start();


                while (_server.Connected && !_reader.EndOfStream)
                {
                    while (Core.blocked)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    text = _reader.ReadLine();
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
                                continue;
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
                                            Channel channel = _server.getChannel(name);
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
                                                continue;
                                            }
                                        }
                                        break;
                                    case "315":
                                        if (code.Length > 2)
                                        {
                                            Channel channel = _server.getChannel(code[3]);
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
                                            Channel channel = _server.getChannel(name);
                                            if (channel != null)
                                            {
                                                channel.TopicDate = int.Parse(time);
                                                channel.TopicUser = user;
                                                Window curr = channel.retrieveWindow();
                                                if (Core.windowReady(curr))
                                                {
                                                    curr.scrollback.InsertText("Topic by: " + user + " date " + convertUNIX(time), Scrollback.MessageStyle.Channel);
                                                    continue;
                                                }
                                            }
                                        }
                                        break;
                                    case "352":
                                    // cameron.freenode.net 352 petan2 #debian thelineva nikita.tnnet.fi kornbluth.freenode.net t0h H :0 Tommi Helineva
                                        if (code.Length > 6)
                                        {
                                            Channel channel = _server.getChannel(code[3]);
                                            string ident = code[4];
                                            string host = code[5];
                                            string nick = code[7];
                                            string server = code[6];
                                            if (channel != null)
                                            {
                                                if (!channel.containUser(nick))
                                                {
                                                    channel.UserList.Add(new User(nick, host, _server, ident));
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
                                            Channel channel = _server.getChannel(name);
                                            if (channel != null)
                                            {
                                                string[] _chan = data[2].Split(' ');
                                                foreach (var user in _chan)
                                                {
                                                    if (!channel.containUser(user) && user != "")
                                                    {
                                                        lock (channel.UserList)
                                                        {
                                                            channel.UserList.Add(new User(user, "", _server, ""));
                                                        }
                                                    }
                                                }
                                                channel.redrawUsers();
                                                continue;
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
                                        continue;
                                    case "324":
                                        if (code.Length > 3)
                                        {
                                            string name = code[2];
                                            string topic = _value;
                                            Channel channel = _server.getChannel(code[3]);
                                            if (channel != null)
                                            {
                                                Window curr = channel.retrieveWindow();
                                                if (curr != null)
                                                {
                                                    channel._mode.mode(code[4]);
                                                    curr.scrollback.InsertText("Mode: " + code[4], Scrollback.MessageStyle.Channel);
                                                }
                                                continue;
                                            }
                                        }
                                        break;
                                    //  367 petan # *!*@173.45.238.81
                                    case "367":
                                        if (code.Length > 6)
                                        {
                                            string chan = code[3];
                                            Channel channel = _server.getChannel(code[3]);
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
                                _server.windows["!system"].scrollback.InsertText(text.Substring(text.IndexOf("INFO") + 5), Scrollback.MessageStyle.User);
                                continue;
                            }

                            if (command == "NOTICE")
                            {
                                _server.windows["!system"].scrollback.InsertText("[" + source + "] " + _value, Scrollback.MessageStyle.Message);
                                continue;
                            }
                            if (source.StartsWith(_server.nickname + "!"))
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
                                        Channel curr = _server.Join(channel);
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
                                        continue;
                                    }
                                }
                                if (_data2.Length > 2)
                                {
                                    if (_data2[1].Contains("NICK"))
                                    {
                                        _server.windows["!system"].scrollback.InsertText(messages.get("protocolnewnick", Core.SelectedLanguage, new List<string> { _value }), Scrollback.MessageStyle.User);
                                        _server.nickname = _value;
                                    }
                                    if (_data2[1].Contains("PART"))
                                    {
                                        string channel = _data2[2];
                                        if (_data2[2].Contains("#") == false)
                                        {
                                            channel = data[2];
                                            Channel c = _server.getChannel(channel);
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
                                        continue;
                                    }
                                }
                            }

                            if (command == "PING")
                            {
                                Transfer("PONG ", Configuration.Priority.High);
                                continue;
                            }

                            if (command == "NICK")
                            {
                                string nick = source.Substring(0, source.IndexOf("!"));
                                string _new = _value;
                                foreach (Channel item in _server.Channels)
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
                                continue;
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
                                if (!chan.Contains(_server.channel_prefix))
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
                                            _server.windows["!system"].scrollback.InsertText("CTCP from (" + chan + ") " + message, Scrollback.MessageStyle.Message);
                                            continue;
                                        }
                                        continue;
                                    }

                                }
                                User user = new User(_nick, _host, _server, _ident);
                                Channel channel = null;
                                if (chan.StartsWith(_server.channel_prefix))
                                {
                                    channel = _server.getChannel(chan);
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
                                                continue;
                                            }
                                            channel.retrieveWindow().scrollback.InsertText(PRIVMSG(user.Nick, message), Scrollback.MessageStyle.Message);
                                        }
                                        continue;
                                    }
                                    continue;
                                }
                                chan = source.Substring(source.IndexOf("!"));
                                if (!_server.windows.ContainsKey(chan))
                                {
                                    _server.Private(chan);
                                }
                                _server.windows[chan].scrollback.InsertText(PRIVMSG(source, message), Scrollback.MessageStyle.Channel);
                                continue;
                            }

                            if (command == "TOPIC")
                            {
                                string chan = parameters;
                                chan = chan.Replace(" ", "");
                                string user = source.Substring(0, source.IndexOf("!"));
                                Channel channel = _server.getChannel(chan);
                                if (channel != null)
                                {
                                    Window window;
                                    channel.Topic = _value;
                                    window = channel.retrieveWindow();
                                    if (Core.windowReady(window))
                                    {
                                        channel.retrieveWindow().scrollback.InsertText(messages.get("channel-topic", Core.SelectedLanguage, new List<string> { source, _value }), Scrollback.MessageStyle.Channel);
                                        continue;
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
                                    if (chan.StartsWith(_server.channel_prefix))
                                    {
                                        Channel channel = _server.getChannel(chan);
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
                                            continue;
                                        }
                                    }
                                }
                            }

                            if (command == "PART")
                            {
                                string chan = parameters;
                                chan = chan.Replace(" ", "");
                                string user = source.Substring(0, source.IndexOf("!"));
                                Channel channel = _server.getChannel(chan);
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
                                            continue;
                                        }
                                    }
                                }
                            }

                            if (command == "QUIT")
                            {
                                string nick = source.Substring(0, source.IndexOf("!"));
                                string _new = _value;
                                foreach (Channel item in _server.Channels)
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
                                continue;
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
                                Channel channel = _server.getChannel(chan);
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
                                                channel.UserList.Add(new User(user, _host, _server, _ident));
                                            }
                                            channel.redrawUsers();
                                        }
                                        continue;
                                    }
                                }
                            }
                            //if (command == "")
                        }
                        _server.windows["!system"].scrollback.InsertText(text, Scrollback.MessageStyle.User);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
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
                    _writer.WriteLine(first_word + rest);
                    _writer.Flush();
                    return true;
                }
                _writer.WriteLine(cm.ToUpper());
                _writer.Flush();
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
            return false;
        }

        private void Send(string ms)
        {
            _writer.WriteLine(ms);
            _writer.Flush();
        }

        public override int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            Transfer("PRIVMSG " + to + " :" + text, _priority);
            return 0;
        }

        /// <summary>
        /// /me style
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="_priority"></param>
        /// <returns></returns>
        public override int Message2(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            Transfer("PRIVMSG " + to + " :" + delimiter.ToString() + "ACTION " + text + delimiter.ToString(), _priority);
            return 0;
        }

        public override void Join(string name, Network network = null)
        {
            Transfer("JOIN " + name);
        }

        public override int requestNick(string _Nick)
        {
            Transfer("NICK " + _Nick);
            return 0;
        }

        public override void Exit()
        {
            if (!_server.Connected)
            {
                return;
            }
            _writer.WriteLine("QUIT :" + _server.quit);
            _writer.Flush();
            _server.Connected = false;
            System.Threading.Thread.Sleep(200);
            deliveryqueue.Abort();
            keep.Abort();
            if (main.ThreadState == System.Threading.ThreadState.Running)
            {
                main.Abort();
            }
            _server.windows["!system"].scrollback.InsertText("You have disconnected from network", Scrollback.MessageStyle.System);
            return;
        }

        public override bool Open()
        {
            main = new System.Threading.Thread(Start);
            Core._Main.Status(messages.get("connecting", Core.SelectedLanguage));
            main.Start();
            return true;
        }
    }
}
