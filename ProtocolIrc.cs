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
                public string channel;
            }
            public List<Message> messages = new List<Message>();
            public List<Message> newmessages = new List<Message>();
            public ProtocolIrc protocol;

            public void DeliverMessage(string Message, string Channel, Configuration.Priority Pr = Configuration.Priority.Normal)
            {
                Message text = new Message();
                text._Priority = Pr;
                text.message = Message;
                text.channel = Channel;
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
                                    protocol.Send(message.message, message.channel);
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

        public void Start()
        {
            _messages.protocol = this;
            Core._Main.Scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }), Scrollback.MessageStyle.System);
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
                Core._Main._Scrollback.InsertText(b.Message, Scrollback.MessageStyle.System);
                return;
            }
            string text = "";
            char delimiter = (char)001;
            deliveryqueue = new System.Threading.Thread(_messages.Run);
            deliveryqueue.Start();

            while (_server.Connected)
            {
                text = _reader.ReadLine();
                if (text.StartsWith(":"))
                {
                    string[] data = text.Split(':');
                    if (data.Length > 1)
                    {
                        if (data[1].Contains("INFO"))
                        {
                            Core._Main._Scrollback.InsertText(text.Substring(text.IndexOf("INFO") + 5), Scrollback.MessageStyle.User);
                            continue;
                        }
                        if (data[1].Contains("NOTICE"))
                        {
                            Core._Main._Scrollback.InsertText(text.Substring(text.IndexOf("NOTICE") + 7), Scrollback.MessageStyle.User);
                            continue;
                        }
                        if (data[1].StartsWith(_server.nickname + "!"))
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
                        }
                        if (data[1].Contains("PRIVMSG"))
                        {
                            string _nick;
                            string _ident;
                            string _host;
                            string chan;
                            _nick = data[1].Substring(0, data[1].IndexOf("!"));
                            _host = data[1].Substring(data[1].IndexOf("@") + 1);
                            _host = _host.Substring(0, _host.IndexOf(" PRIVMSG"));
                            _ident = data[1].Substring(data[1].IndexOf("!") + 1);
                            _ident = _ident.Substring(0, _ident.IndexOf("@"));
                            chan = data[1].Substring(data[1].IndexOf("PRIVMSG") + "PRIVMSG ".Length).Replace(" ", "");
                            User user = new User(_nick, _host, _ident);
                            Channel channel = _server.getChannel(chan);
                            if (channel != null)
                            {
                                Network._window window;
                                window = channel.retrieveWindow();
                                if (window != null)
                                {
                                    channel.retrieveWindow().scrollback.InsertText(PRIVMSG( user.Nick, text.Substring(text.IndexOf(data[1]) + 1 + data[1].Length)), Scrollback.MessageStyle.Message);
                                    continue;
                                }
                                
                            }
                        }
                    }
                    if (_server.windows.ContainsKey("&system"))
                    {
                        _server.windows["&system"].scrollback.InsertText(text, Scrollback.MessageStyle.User);
                    }
                }
            }
        }

        public override bool Command(string cm)
        {
            _writer.WriteLine(cm);
            _writer.Flush();
            return false;
        }

        private void Send(string data, string to)
        {
            _writer.WriteLine("PRIVMSG " + to + " :" + data);
            _writer.Flush();
        }

        public override int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            _messages.DeliverMessage(text, to, _priority);
            return 0;
        }

        public override void Join(string name, Network network = null)
        {
            _writer.WriteLine("JOIN " + name);
            _writer.Flush();
        }

        public override int requestNick(string _Nick)
        {
            _writer.WriteLine("NICK " + _Nick);
            _writer.Flush();
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
            main.Abort();
        }

        public override bool Open()
        {
            main = new System.Threading.Thread(Start);
            main.Start();
            return true;
        }
    }
}
