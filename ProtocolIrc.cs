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
            }
        }

        public override void Transfer(string text, Configuration.Priority Pr = Configuration.Priority.Normal)
        {
            _messages.DeliverMessage(text, Pr);
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

            }
            catch (Exception b)
            {
                Core._Main.Chat.scrollback.InsertText(b.Message, Scrollback.MessageStyle.System);
                return;
            }
            string text = "";
            char delimiter = (char)001;
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
                            string[] _command = data[1].Split(' ');
                            string command;
                            string parameters = "";
                            string source;
                            string _value;
                            source = _command[0];
                            command = _command[1];
                            if (_command.Length > 2)
                            {
                                int curr = 2;
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
                            _value = text.Substring(data[1].Length + 1);
                            if (_value.StartsWith(":"))
                            {
                                _value = _value.Substring(1);
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
                                                    curr.scrollback.InsertText("Topic: " + topic, Scrollback.MessageStyle.Channel);
                                                }
                                                channel.Topic = topic;
                                                continue;
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
                                                    continue;
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
                                                        channel.UserList.Add(new User(user, "", _server, ""));
                                                    }
                                                }
                                                channel.redrawUsers();
                                                continue;
                                            }
                                        }
                                        break;
                                    case "005":
                                        // PREFIX=(qaohv)~&@%+ CHANMODES=beI,kfL,lj,psmntirRcOAQKVCuzNSMTG 
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
                                }
                            }
                            if (command == "INFO")
                            {
                                _server.windows["!system"].scrollback.InsertText(text.Substring(text.IndexOf("INFO") + 5), Scrollback.MessageStyle.User);
                                continue;
                            }

                            if (command == "NOTICE")
                            {
                                _server.windows["!system"].scrollback.InsertText("[" + source + "] " + _value, Scrollback.MessageStyle.User);
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
                                        _server.Join(channel);
                                        continue;
                                    }
                                }
                                if (_data2.Length > 2)
                                {
                                    if (_data2[1].Contains("NICK"))
                                    {
                                        _server.windows["!system"].scrollback.InsertText(messages.get("protocolnewnick", Core.SelectedLanguage, new List<string> {_value}), Scrollback.MessageStyle.User);
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
                                        _server.Join(channel);
                                        continue;
                                    }
                                }
                            }

                            if (command == "NICK")
                            {
                                string nick = source.Substring(0, source.IndexOf("!"));
                                string _new = _value;
                                foreach (Channel item in _server.Channels)
                                {
                                    if (item.ok)
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
                                                    x.scrollback.InsertText(messages.get("protocol-nick", Core.SelectedLanguage, new List<string>{ nick, _new }), Scrollback.MessageStyle.Channel);
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
                                User user = new User(_nick, _host, _server, _ident);
                                Channel channel = _server.getChannel(chan);
                                if (channel != null)
                                {
                                    Window window;
                                    window = channel.retrieveWindow();
                                    if (Core.windowReady(window))
                                    {
                                        channel.retrieveWindow().scrollback.InsertText(PRIVMSG(user.Nick, text.Substring(text.IndexOf(data[1]) + 1 + data[1].Length)), Scrollback.MessageStyle.Message);
                                        continue;
                                    }

                                }
                            }
                            if (command == "PART")
                            {
                                string chan = _value;
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
                                        channel.retrieveWindow().scrollback.InsertText(messages.get("part", Core.SelectedLanguage, new List<string> { source }), Scrollback.MessageStyle.Channel);

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
                            if (command == "JOIN")
                            {
                                string chan = _value;
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
                                        channel.retrieveWindow().scrollback.InsertText(messages.get("join", Core.SelectedLanguage, new List<string> { source }), Scrollback.MessageStyle.Channel);

                                        if (!channel.containUser(user))
                                        {
                                            channel.UserList.Add(new User(user, _host, _server, _ident));
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
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
        }

        public override bool Command(string cm)
        {
            try
            {
                _writer.WriteLine(cm);
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
            if (_server.Connected)
            {
                _writer.WriteLine("QUIT " + _server.quit);
                _writer.Flush();
            }
            _server.Connected = false;
            System.Threading.Thread.Sleep(1000);
            if (main.ThreadState == System.Threading.ThreadState.Running)
            {
                main.Abort();
            }
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
