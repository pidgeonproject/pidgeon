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
using System.Text;
using System.Net;

namespace pidgeon_sv
{
    public class ProtocolIrc : Protocol
    {
        public enum Priority
        {
            High = 8,
            Normal = 2,
            Low = 1
        }
        public System.Threading.Thread main;
        public System.Threading.Thread deliveryqueue;
        public System.Threading.Thread keep;
        public System.Threading.Thread th;
        public Buffer buffer = new Buffer();
        public DateTime pong = DateTime.Now;
        public Account owner = null;

        private System.Net.Sockets.NetworkStream _network;
        private System.IO.StreamReader _reader;
        public Network _server;
        private System.IO.StreamWriter _writer;
        Messages _messages = new Messages();


        public class Buffer
        {
            public struct Message
            {
                public Priority _Priority;
                public ProtocolMain.Datagram message;
            }
            public List<Message> messages = new List<Message>();
            public List<Message> newmessages = new List<Message>();
            public List<Message> oldmessages = new List<Message>();
            public ProtocolIrc protocol;

            public void DeliverMessage(ProtocolMain.Datagram Message, Priority Pr = Priority.Normal)
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
                        if (protocol.owner == null)
                        {
                            System.Threading.Thread.Sleep(100);
                            continue;
                        }
                        lock (protocol.owner.Clients)
                        {
                            if (protocol.owner.Clients.Count == 0)
                            {
                                System.Threading.Thread.Sleep(2000);
                                continue;
                            }
                        if (messages.Count > 0)
                        {
                            lock (messages)
                            {
                                newmessages.AddRange(messages);
                                messages.Clear();
                            }
                        }
                        }
                        if (newmessages.Count > 0)
                        {
                            List<Message> Processed = new List<Message>();
                            Priority highest = Priority.Low;
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
                                highest = Priority.Low;
                                // we need to check the priority we need to handle first
                                foreach (Message message in newmessages)
                                {
                                    if (message._Priority > highest)
                                    {
                                        highest = message._Priority;
                                        if (message._Priority == Priority.High)
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

                                        protocol.owner.Deliver(message.message);
                                        if (highest != Priority.High)
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
                                        if (oldmessages.Count > Core.maxbs)
                                        {
                                            FlushOld();
                                        }
                                        lock (oldmessages)
                                        {
                                            oldmessages.Add(message);
                                        }
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
            public void FlushOld()
            {
                lock (oldmessages)
                {
                    foreach (Message curr in oldmessages)
                    { 
                        string item = "";

                            foreach (KeyValuePair<string, string> data in curr.message.Parameters)
                            {
                                item += " "   + data.Key + "=\"" + System.Web.HttpUtility.HtmlEncode(data.Value) + "\"";  
                            }
                            string line = "<LINE" + item + ">" + System.Web.HttpUtility.HtmlEncode(curr.message._InnerText) + "</LINE>\n";
                            System.IO.File.AppendAllText("db/" + protocol.Server + ".db", line);
                    }
                    oldmessages.Clear();
                }
            }
        }

        class Messages
        {
            public struct Message
            {
                public Priority _Priority;
                public string message;
            }
            public List<Message> messages = new List<Message>();
            public List<Message> newmessages = new List<Message>();
            public ProtocolIrc protocol;

            public void DeliverMessage(string Message, Priority Pr = Priority.Normal)
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
                            Priority highest = Priority.Low;
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
                                highest = Priority.Low;
                                // we need to check the priority we need to handle first
                                foreach (Message message in newmessages)
                                {
                                    if (message._Priority > highest)
                                    {
                                        highest = message._Priority;
                                        if (message._Priority == Priority.High)
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
                                        if (highest != Priority.High)
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

        public override void Transfer(string text, Priority Pr = Priority.Normal)
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
                    Transfer("PING :" + _server._protocol.Server, Priority.High);
                    System.Threading.Thread.Sleep(24000);
                }
            }
            catch (Exception)
            { }
        }

        public void Start()
        {
            _messages.protocol = this;
            try
            {
                _network = new System.Net.Sockets.TcpClient(Server, Port).GetStream();
                _server.Connected = true;

                _writer = new System.IO.StreamWriter(_network);
                _reader = new System.IO.StreamReader(_network, Encoding.UTF8);


                _writer.WriteLine("USER " + _server.ident + " 8 * :" + _server.username);
                _writer.WriteLine("NICK " + _server.nickname);
                _writer.Flush();

                keep = new System.Threading.Thread(_Ping);
                keep.Name = "pinger thread";
                keep.Start();

            }
            catch (Exception b)
            {
                ProtocolMain.Datagram dt = new ProtocolMain.Datagram("CONNECTION", "PROBLEM");
                dt.Parameters.Add("network", Server);
                dt.Parameters.Add("info", b.Message);
                owner.Deliver(dt);
                Console.WriteLine(b.Message);
                return;
            }
            string text = "";
            try
            {
                deliveryqueue = new System.Threading.Thread(_messages.Run);
                deliveryqueue.Start();


                while (_server.Connected && !_reader.EndOfStream)
                {
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
                                                channel.Topic = topic;

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
                                            }
                                        }
                                        break;
                                    case "005":
                                        // PREFIX=(qaohv)~&@%+ CHANMODES=beI,k
                                        if (parameters.Contains("PREFIX=("))
                                        {

                                        }
                                        if (parameters.Contains("CHANMODES="))
                                        {
                                            string xmodes = parameters.Substring(parameters.IndexOf("CHANMODES=") + 11);
                                            

                                        }
                                        break;
                                    case "366":
                                        break;;
                                    case "324":
                                        if (code.Length > 3)
                                        {
                                            string name = code[2];
                                            string topic = _value;
                                            //Channel channel = _server.getChannel(code[3]);
                                            
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

                            }

                            if (command == "NOTICE")
                            {

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
                                        Console.WriteLine("joined");
                                    }
                                }
                                if (_data2.Length > 2)
                                {
                                    if (_data2[1].Contains("NICK"))
                                    {
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
                                                c.ok = false;
                                            }
                                        }
                                    }
                                }
                            }

                            if (command == "PING")
                            {
                                Transfer("PONG ", Priority.High);
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
                                        owner.Deliver(new ProtocolMain.Datagram("CTCP", message));
                                }
                                User user = new User(_nick, _host, _server, _ident);
                                Channel channel = null;
                                if (chan.StartsWith(_server.channel_prefix))
                                {
                                    channel = _server.getChannel(chan);
                                }
                                chan = source.Substring(source.IndexOf("!"));
                                
                            }

                            if (command == "TOPIC")
                            {
                                string chan = parameters;
                                chan = chan.Replace(" ", "");
                                string user = source.Substring(0, source.IndexOf("!"));
                                Channel channel = _server.getChannel(chan);
                                if (channel != null)
                                {
                                    channel.Topic = _value;
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
                                            
                                            string change = parameters.Substring(parameters.IndexOf(" "));
                                            

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
                                    User delete = null;
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
                                            continue;
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
                                            lock (item.UserList)
                                            {
                                                item.UserList.Remove(target);
                                            }
                                        }
                                    }
                                }
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
                                    if (!channel.containUser(user))
                                    {
                                        lock (channel.UserList)
                                        {
                                            channel.UserList.Add(new User(user, _host, _server, _ident));
                                        }
                                    }
                                }
                            }   
                        }
                        ProtocolMain.Datagram dt = new ProtocolMain.Datagram("DATA", text);
                        dt.Parameters.Add("network", Server);
                        buffer.DeliverMessage(dt);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
            }
        }

        public void ClientData(string content)
        {
            ProtocolMain.Datagram dt = new ProtocolMain.Datagram("DATA", content);
            dt.Parameters.Add("network", Server);
            buffer.DeliverMessage(dt);
        }

        public void getDepth(int n)
        {

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

            }
            return false;
        }

        private void Send(string ms)
        {
            try
            {
                _writer.WriteLine(ms);
                _writer.Flush();
            }
            catch (Exception fail)
            {

            }
        }

        public int Message(string text, string to, Priority _priority = Priority.Normal)
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
        public int Message2(string text, string to, Priority _priority = Priority.Normal)
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
            try
            {
                _writer.WriteLine("QUIT :" + _server.quit);
                _writer.Flush();
            }
            catch (Exception) { }
            _server.Connected = false;
            System.Threading.Thread.Sleep(200);
            deliveryqueue.Abort();
            keep.Abort();
            if (main.ThreadState == System.Threading.ThreadState.Running)
            {
                main.Abort();
            }
            return;
        }

        public override bool Open()
        {
            main = new System.Threading.Thread(Start);
            main.Start();
            buffer.protocol = this;
            th = new System.Threading.Thread(buffer.Run);
            th.Start();
            return true;
        }
    }
}
