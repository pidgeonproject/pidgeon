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

        public class ProcessorIRC
        {
            public Network _server = null;
            public Protocol protocol = null;
            public string text;
            public string sn;
            public DateTime pong;
            public long date = 0;
            public string system = "";
            public bool updated_text = true;

            private void Ping()
            {
                pong = DateTime.Now;
                return;
            }

            private bool Info(string command, string parameters, string value)
            {
                /*if (parameters.Contains("PREFIX=("))
                {
                    string cmodes = parameters.Substring(parameters.IndexOf("PREFIX=(") + 8);
                    cmodes = cmodes.Substring(0, cmodes.IndexOf(")"));
                    lock (_server.CUModes)
                    {
                        _server.CUModes.Clear();
                        _server.CUModes.AddRange(cmodes.ToArray<char>());
                    }
                    cmodes = parameters.Substring(parameters.IndexOf("PREFIX=(") + 8);
                    cmodes = cmodes.Substring(cmodes.IndexOf(")") + 1, _server.CUModes.Count);

                    _server.UChars.Clear();
                    _server.UChars.AddRange(cmodes.ToArray<char>());

                }
                if (parameters.Contains("CHANMODES="))
                {
                    string xmodes = parameters.Substring(parameters.IndexOf("CHANMODES=") + 11);
                    xmodes = xmodes.Substring(0, xmodes.IndexOf(" "));
                    string[] _mode = xmodes.Split(',');
                    _server.parsed_info = true;
                    if (_mode.Length == 4)
                    {
                        _server.PModes.Clear();
                        _server.CModes.Clear();
                        _server.XModes.Clear();
                        _server.SModes.Clear();
                        _server.PModes.AddRange(_mode[0].ToArray<char>());
                        _server.XModes.AddRange(_mode[1].ToArray<char>());
                        _server.SModes.AddRange(_mode[2].ToArray<char>());
                        _server.CModes.AddRange(_mode[3].ToArray<char>());
                    }

                }
                 * */
                return true;
            }

            private bool ChannelInfo(string[] code, string command, string source, string parameters, string _value)
            {
                if (code.Length > 3)
                {
                    string name = code[2];
                    string topic = _value;
                    Channel channel = _server.getChannel(code[3]);
                    if (channel != null)
                    {
                            channel._mode.mode(code[4]);
                        return true;
                    }
                }
                return false;
            }

            private bool ChannelTopic(string[] code, string command, string source, string parameters, string value)
            {
                if (code.Length > 3)
                {
                    string name = "";
                    if (parameters.Contains("#"))
                    {
                        name = parameters.Substring(parameters.IndexOf("#")).Replace(" ", "");
                    }
                    string topic = value;
                    Channel channel = _server.getChannel(name);
                    if (channel != null)
                    {
                        //Window curr = channel.retrieveWindow();
                        channel.Topic = topic;
                        return true;
                    }
                }
                return false;
            }

            private bool FinishChan(string[] code)
            {
                return false;
            }

            private bool TopicInfo(string[] code, string parameters)
            {
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
                    }
                }
                return false;
            }

            private bool ParseUs(string[] code)
            {
                if (code.Length > 8)
                {
                    Channel channel = _server.getChannel(code[3]);
                    string ident = code[4];
                    string host = code[5];
                    string nick = code[7];
                    string server = code[6];
                    string mode = "";
                    if (code[8].Length > 0)
                    {
                        mode = code[8][code[8].Length - 1].ToString();
                        if (mode == "G" || mode == "H")
                        {
                            mode = "";
                        }
                    }
                    if (channel != null)
                    {
                        if (updated_text)
                        {
                            if (!channel.containsUser(nick))
                            {
                                User _user = new User(mode + nick, host, _server, ident);
                                channel.UserList.Add(_user);
                                return true;
                            }
                            foreach (User u in channel.UserList)
                            {
                                if (u.Nick == nick)
                                {
                                    u.Ident = ident;
                                    u.Host = host;
                                    break;
                                }
                            }
                        }
                    }
                }
                return false;
            }

            private bool ParseInfo(string[] code, string[] data)
            {
                if (code.Length > 3)
                {
                    string name = code[4];
                    if (!updated_text)
                    {
                        return true;
                    }
                    Channel channel = _server.getChannel(name);
                    if (channel != null)
                    {
                        string[] _chan = data[2].Split(' ');
                        foreach (var user in _chan)
                        {
                            if (!channel.containsUser(user) && user != "")
                            {
                                lock (channel.UserList)
                                {
                                    channel.UserList.Add(new User(user, "", _server, ""));
                                }
                            }
                        }
                        return true;
                    }
                }
                return false;
            }

            private bool ProcessPM(string source, string parameters, string value)
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
                string message = value;
                User user = new User(_nick, _host, _server, _ident);
                Channel channel = null;
                if (chan.StartsWith(_server.channel_prefix))
                {
                    channel = _server.getChannel(chan);
                    if (channel != null)
                    {

                        // here we handle a message to channel
                        return true;
                    }
                    return true;
                }
                if (chan == _server.nickname)
                {
                    chan = source.Substring(0, source.IndexOf("!"));
                    // here we handle private message
                    return true;
                }
                return false;
            }

            private bool ChannelBans(string[] code)
            {
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
                return false;
            }

            private bool ProcessNick(string source, string parameters, string value)
            {
                string nick = source.Substring(0, source.IndexOf("!"));
                string _new = value;
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
                                    if (updated_text)
                                    {
                                        curr.Nick = _new;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }

            private bool Mode(string source, string parameters, string value)
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
                                        continue;
                                    }
                                    if (_server.CUModes.Contains(m) && curr <= parameters2.Count)
                                    {
                                        User flagged_user = channel.userFromName(parameters2[curr]);
                                        if (flagged_user != null)
                                        {
                                            flagged_user.ChannelMode.mode(type.ToString() + m.ToString());
                                        }
                                        curr++;
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
                                                            if (xx.Target == parameters2[curr])
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
                            return true;
                        }
                    }
                }
                return false;
            }

            private bool Part(string source, string parameters, string value)
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
                    User delete = null;
                        if (channel.containsUser(user))
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

                            if (updated_text)
                            {
                                if (delete != null)
                                {
                                    channel.UserList.Remove(delete);
                                }
                            }
                            return true;
                        }
                }
                return false;
            }

            private bool Topic(string source, string parameters, string value)
            {
                string chan = parameters;
                chan = chan.Replace(" ", "");
                string user = source.Substring(0, source.IndexOf("!"));
                Channel channel = _server.getChannel(chan);
                if (channel != null)
                {
                    channel.Topic = value;
                }
                return false;
            }

            private bool Quit(string source, string parameters, string value)
            {
                string user = source.Substring(0, source.IndexOf("!"));
                string _ident;
                string _host;
                _host = source.Substring(source.IndexOf("@") + 1);
                _ident = source.Substring(source.IndexOf("!") + 1);
                _ident = _ident.Substring(0, _ident.IndexOf("@"));
                string _new = value;
                foreach (Channel item in _server.Channels)
                {
                    if (item.ok)
                    {
                        User target = null;
                        lock (item.UserList)
                        {
                            foreach (User curr in item.UserList)
                            {
                                if (curr.Nick == user)
                                {
                                    target = curr;
                                    break;
                                }
                            }
                        }
                        if (target != null)
                        {
                            if (updated_text)
                            {
                                lock (item.UserList)
                                {
                                    item.UserList.Remove(target);
                                }
                            }
                        }
                    }
                }
                return true;
            }

            private bool Kick(string source, string parameters, string value)
            {
                string user = parameters.Substring(parameters.IndexOf(" ") + 1);
                // petan!pidgeon@petan.staff.tm-irc.org KICK #support HelpBot :Removed from the channel
                Channel channel = _server.getChannel(parameters.Substring(0, parameters.IndexOf(" ")));
                if (channel != null)
                {
                        if (updated_text && channel.containsUser(user))
                        {
                            User delete = null;
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
                                if (delete != null)
                                {
                                    channel.UserList.Remove(delete);
                                }
                            }
                        }
                    return true;
                }
                return false;
            }

            private bool Join(string source, string parameters, string value)
            {
                string chan = parameters;
                chan = chan.Replace(" ", "");
                if (chan == "")
                {
                    chan = value;
                }
                string user = source.Substring(0, source.IndexOf("!"));
                string _ident;
                string _host;
                _host = source.Substring(source.IndexOf("@") + 1);
                _ident = source.Substring(source.IndexOf("!") + 1);
                _ident = _ident.Substring(0, _ident.IndexOf("@"));
                Channel channel = _server.getChannel(chan);
                if (channel != null)
                {
                            if (!channel.containsUser(user))
                            {
                                lock (channel.UserList)
                                {
                                    channel.UserList.Add(new User(user, _host, _server, _ident));
                                }
                            }
                }
                return false;
            }

            private bool IdleTime(string source, string parameters, string value)
            {
                if (parameters.Contains(" "))
                {
                    string name = parameters.Substring(parameters.IndexOf(" ") + 1);
                    if (name.Contains(" ") != true)
                    {
                        return false;
                    }
                    string idle = name.Substring(name.IndexOf(" ") + 1);
                    if (idle.Contains(" ") != true)
                    {
                        return false;
                    }
                    string uptime = idle.Substring(idle.IndexOf(" ") + 1);
                    name = name.Substring(0, name.IndexOf(" "));
                    idle = idle.Substring(0, idle.IndexOf(" "));
                    return true;
                }
                return false;
            }

            public bool Result()
            {
                try
                {
                    if (text == null || text == "")
                    {
                        return false;
                    }
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
                            string[] code = data[1].Split(' ');
                            switch (command)
                            {
                                case "001":
                                    return true;
                                case "002":
                                case "003":
                                case "004":
                                case "005":
                                    if (Info(command, parameters, _value))
                                    {
                                        //return true;
                                    }
                                    break;
                                case "317":
                                    
                                    break;
                                case "PONG":
                                    Ping();
                                    return true;
                                case "INFO":
                                    
                                    return true;
                                case "NOTICE":
                                    if (parameters.Contains(_server.channel_prefix))
                                    {
                                        
                                    }
                                    return true;
                                case "PING":
                                    protocol.Transfer("PONG ", Priority.High);
                                    return true;
                                case "NICK":
                                    if (ProcessNick(source, parameters, _value))
                                    {
                                        return true;
                                    }
                                    break;
                                case "PRIVMSG":
                                    if (ProcessPM(source, parameters, _value))
                                    {
                                        return true;
                                    }
                                    break;
                                case "TOPIC":
                                    if (Topic(source, parameters, _value))
                                    {
                                        return true;
                                    }
                                    break;
                                case "MODE":
                                    if (Mode(source, parameters, _value))
                                    {
                                        return true;
                                    }
                                    break;
                                case "PART":
                                    if (Part(source, parameters, _value))
                                    {
                                        return true;
                                    }
                                    break;
                                case "QUIT":
                                    if (Quit(source, parameters, _value))
                                    {
                                        return true;
                                    }
                                    break;
                                case "JOIN":
                                    if (Join(source, parameters, _value))
                                    {
                                        return true;
                                    }
                                    break;
                                case "KICK":
                                    if (Kick(source, parameters, _value))
                                    {
                                        return true;
                                    }
                                    break;
                            }
                            if (data[1].Contains(" "))
                            {
                                switch (command)
                                {
                                    case "315":
                                        if (FinishChan(code))
                                        {
                                            return true;
                                        }
                                        break;
                                    case "324":
                                        if (ChannelInfo(code, command, source, parameters, _value))
                                        {
                                            return true;
                                        }
                                        break;
                                    case "332":
                                        if (ChannelTopic(code, command, source, parameters, _value))
                                        {
                                            return true;
                                        }
                                        break;
                                    case "333":
                                        if (TopicInfo(code, parameters))
                                        {
                                            return true;
                                        }
                                        break;
                                    case "352":
                                        if (ParseUs(code))
                                        {
                                            return true;
                                        }
                                        break;
                                    case "353":
                                        if (ParseInfo(code, data))
                                        {
                                            return true;
                                        }
                                        break;
                                    case "366":
                                        return true;
                                    case "367":
                                        if (ChannelBans(code))
                                        {
                                            return true;
                                        }
                                        break;
                                }
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
                                        Channel curr = _server.getChannel(channel);
                                        if (curr == null)
                                        {
                                            curr = _server.Join(channel);
                                        }
                                        return true;
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
                                        if (_data2[2].Contains(_server.channel_prefix))
                                        {
                                            channel = _data2[2];
                                            Channel c = _server.getChannel(channel);
                                            if (c != null)
                                            {
                                                c.ok = false;
                                            }
                                        }
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    // unhandled

                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
                return true;
            }

            public ProcessorIRC(Network server, Protocol _protocol, string _text, string _sn, string ws, ref DateTime _pong, long d = 0, bool updated = true)
            {
                _server = server;
                protocol = _protocol;
                text = _text;
                sn = _sn;
                system = ws;
                pong = _pong;
                date = d;
                updated_text = updated;
            }
        }

        public class MessageOrigin
        {
            public string text = null;
            public DateTime time;
        }

        private System.Net.Sockets.NetworkStream _network;
        private System.IO.StreamReader _reader;
        
        public Network _server;
        private System.IO.StreamWriter _writer;
        
        
        Messages _messages = new Messages();
        
        
        public List<ProtocolMain.Datagram> info = new List<ProtocolMain.Datagram>();
        public List<MessageOrigin> MessageBuffer = new List<MessageOrigin>();


        public System.Threading.Thread main;
        public System.Threading.Thread deliveryqueue; 
        public System.Threading.Thread keep;
        public System.Threading.Thread th;
        
        
        public Buffer buffer = new Buffer();
        public DateTime pong = DateTime.Now;
        public Account owner = null;


        public class Buffer
        {
            public class Message
            {
                public Priority _Priority;
                public DateTime time;
                public ProtocolMain.Datagram message;
                    public Message()
                    {
                        time = DateTime.Now;
                    }
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
                try
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
                            bool skip = false;
                            lock (protocol.owner.Clients)
                            {
                                if (protocol.owner.Clients.Count == 0)
                                {
                                    skip = true;
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
                                if (skip)
                                {
                                    System.Threading.Thread.Sleep(100);
                                    continue;
                                }
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

                                    foreach (Message message in newmessages)
                                    {
                                        protocol.owner.Deliver(message.message);
                                    }

                                    if (oldmessages.Count > Config.maxbs)
                                    {
                                        FlushOld();
                                    }
                                    lock (oldmessages)
                                    {
                                        oldmessages.AddRange(newmessages);
                                    }
                                    lock (newmessages)
                                    {
                                        newmessages.Clear();
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
                catch (Exception fail)
                {
                    Core.handleException(fail, true);
                }
            }
            public void FlushOld()
            {
                List<Message> stored = new List<Message>();
                lock (oldmessages)
                {
                    int current = 0;

                    while( current < (Config.maxbs - Config.minbs))
                    {
                        string item = "";

                        foreach (KeyValuePair<string, string> data in oldmessages[current].message.Parameters)
                        {
                            item += " " + data.Key + "=\"" + System.Web.HttpUtility.HtmlEncode(data.Value) + "\"";
                        }
                        stored.Add(oldmessages[current]);
                        string line = "<" + oldmessages[current].message._Datagram + item + ">" + System.Web.HttpUtility.HtmlEncode(oldmessages[current].message._InnerText) + "</" + oldmessages[current].message._Datagram + ">\n";
                        System.IO.File.AppendAllText("db/" + protocol.Server + ".db", line);
                    }
                    foreach (Message message in stored)
                    {
                        oldmessages.Remove(message);
                    }
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
                                    case "1":
                                    case "2":
                                    case "3":
                                    case "4":
                                    case "5":
                                        ProtocolMain.Datagram p = new ProtocolMain.Datagram("DATA", text);
                                        //p.Parameters.Add("network", Server);
                                        info.Add(p);
                                        break;
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
                                                if (!channel.containsUser(nick))
                                                {
                                                    channel.UserList.Add(new User(nick, host, _server, ident));
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
                                                    if (!channel.containsUser(user) && user != "")
                                                    {
                                                        lock (channel.UserList)
                                                        {
                                                            channel.UserList.Add(new User(user, "", _server, ""));
                                                        }
                                                    }
                                                }
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
                                        break; ;
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
                                                _server.Channels.Remove(c);
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
                                                                            if (xx.Target == parameters2[curr])
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
                                    if (channel.containsUser(user))
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
                                    }
                                }
                            }

                            if (command == "PONG")
                            {
                                continue;
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
                                if (chan == "")
                                {
                                    chan = _value;
                                }
                                _ident = source.Substring(source.IndexOf("!") + 1);
                                _ident = _ident.Substring(0, _ident.IndexOf("@"));
                                Channel channel = _server.getChannel(chan);
                                if (channel != null)
                                {
                                    if (!channel.containsUser(user))
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
            catch (Exception)
            {

            }
        }

        public void ClientData(string content)
        {
            ProtocolMain.Datagram dt = new ProtocolMain.Datagram("DATA", content);
            dt.Parameters.Add("network", Server);
            buffer.DeliverMessage(dt);
        }
        
        private void recoverDatagrams(int n)
        {
            lock (buffer.oldmessages)
            { 
                List<string> line = new List<string>();
                if (System.IO.File.Exists("db/" + Server + ".db"))
                {
                    line.AddRange(System.IO.File.ReadAllLines("db/" + Server + ".db"));
                    foreach (string Line in line)
                    {
                        try
                        {
                            System.Xml.XmlDocument node = new System.Xml.XmlDocument();
                            Buffer.Message message = new Buffer.Message();
                            
                            node.LoadXml(Line);
                            ProtocolMain.Datagram text = new ProtocolMain.Datagram(node.ChildNodes[0].Name);
                            text._InnerText = node.ChildNodes[0].InnerText;
                            foreach (System.Xml.XmlAttribute part in node.ChildNodes[0].Attributes)
                            {
                                text.Parameters.Add(part.Name, part.Value);
                            }
                            message._Priority = Priority.Normal;
                            message.message = text;
                            buffer.oldmessages.Insert(0, message);
                        }
                        catch (Exception fail)
                        {
                            Console.WriteLine(fail.Message);
                        }
                    }
                }
            }
        }

        public void getDepth(int n, ProtocolMain user)
        {
            lock (buffer.oldmessages)
            {
                if (buffer.oldmessages.Count == 0)
                {
                    ProtocolMain.Datagram size = new ProtocolMain.Datagram("BACKLOG", "0");
                    size.Parameters.Add("network", Server);
                    user.Deliver(size);
                    return;
                }
                if (buffer.oldmessages.Count < n)
                {
                    recoverDatagrams(n);
                }
                if (buffer.oldmessages.Count < n)
                {
                    n = buffer.oldmessages.Count;
                    ProtocolMain.Datagram count = new ProtocolMain.Datagram("BACKLOG", n.ToString());
                    count.Parameters.Add("network", Server);
                    user.Deliver(count);
                    int i = 0;
                    while (i < n)
                    {
                        ProtocolMain.Datagram text = new ProtocolMain.Datagram("");
                        text._InnerText = buffer.oldmessages[i].message._InnerText;
                        text._Datagram = buffer.oldmessages[i].message._Datagram;
                        foreach (KeyValuePair<string, string> current in buffer.oldmessages[i].message.Parameters)
                        {
                            text.Parameters.Add(current.Key, current.Value);
                        }
                        text.Parameters.Add("buffer", i.ToString());
                        text.Parameters.Add("time", buffer.oldmessages[i].time.ToBinary().ToString());
                        //text.Parameters.Add("network", Server);
                        //text.Parameters.Add("index", i.ToString());
                        //text.Parameters.Add("priority", buffer.oldmessages[i]._Priority.ToString());
                        //text.Parameters.Add("type", buffer.oldmessages[i].message._Datagram);
                        user.Deliver(text);
                        i = i + 1;
                        
                    }
                    foreach (MessageOrigin d in MessageBuffer)
                    {
                        ProtocolMain.Datagram text = new ProtocolMain.Datagram("SELFDG");
                        text._InnerText = d.text;
                        text.Parameters.Add("network", Server);
                        text.Parameters.Add("buffer", i.ToString());
                        text.Parameters.Add("time", d.time.ToBinary().ToString());
                        user.Deliver(text);
                    }
                }
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
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
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
            catch (Exception)
            {

            }
        }

        public override int Message(string text, string to, Priority _priority = Priority.Normal)
        {
            Transfer("PRIVMSG " + to + " :" + text, _priority);
            ProtocolMain.Datagram messageback = new ProtocolMain.Datagram("MESSAGE", text);
            messageback.Parameters.Add("network", Server);
            messageback.Parameters.Add("priority", _priority.ToString());
            messageback.Parameters.Add("target", to);
            buffer.DeliverMessage(messageback);
            return 0;
        }

        /// <summary>
        /// /me style
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="_priority"></param>
        /// <returns></returns>
        public override int Message2(string text, string to, Priority _priority = Priority.Normal)
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
