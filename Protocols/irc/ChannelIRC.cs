//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;

namespace Client
{
    public partial class ProcessorIRC
    {
        private bool ChannelInfo(string[] code, string command, string source, string parameters, string _value)
        {
            if (code.Length > 3)
            {
                Channel channel = _Network.getChannel(code[3]);
                if (channel != null)
                {
                    Graphics.Window curr = channel.RetrieveWindow();
                    if (curr != null)
                    {
                        channel.ChannelMode.ChangeMode(code[4]);
                        channel.UpdateInfo();
                        WindowText(curr, "Mode: " + code[4], Client.ContentLine.MessageStyle.Channel, true, date, !updated_text);
                    }
                    Hooks._Network.ChannelInfo(_Network, channel, code[4]);
                    return true;
                }
            }
            return false;
        }

        private bool ParseUser(string[] code, string realname)
        {
            if (code.Length > 8)
            {
                Channel channel = _Network.getChannel(code[3]);
                string ident = code[4];
                string host = code[5];
                string nick = code[7];
                string server = code[6];
                if (realname != null & realname.Length > 2)
                {
                    realname = realname.Substring(2);
                }
                else if (realname == "0 ")
                {
                    realname = "";
                }
                char mode = '\0';
                bool IsAway = false;
                if (code[8].Length > 0)
                {
                    // if user is away we flag him
                    if (code[8].StartsWith("G"))
                    {
                        IsAway = true;
                    }
                    mode = code[8][code[8].Length - 1];
                    if (!_Network.UChars.Contains(mode))
                    {
                        mode = '\0';
                    }
                }
                if (channel != null)
                {
                    if (updated_text)
                    {
                        if (!channel.ContainsUser(nick))
                        {
                            User _user = null;
                            if (mode != '\0')
                            {
                                _user = new User(mode.ToString() + nick, host, _Network, ident, server);
                            }
                            else
                            {
                                _user = new User(nick, host, _Network, ident, server);
                            }
                            _user.LastAwayCheck = DateTime.Now;
                            _user.RealName = realname;
                            if (IsAway)
                            {
                                _user.AwayTime = DateTime.Now;
                            }
                            _user.Away = IsAway;
                            lock (channel.UserList)
                            {
                                channel.UserList.Add(_user);
                            }
                            return true;
                        }
                        lock (channel.UserList)
                        {
                            foreach (User u in channel.UserList)
                            {
                                if (u.Nick == nick)
                                {
                                    u.Ident = ident;
                                    u.Host = host;
                                    u.Server = server;
                                    u.RealName = realname;
                                    u.LastAwayCheck = DateTime.Now;
                                    if (!u.Away && IsAway)
                                    {
                                        u.AwayTime = DateTime.Now;
                                    }
                                    u.Away = IsAway;
                                    break;
                                }
                            }
                        }
                    }
                    if (Configuration.Kernel.HidingParsed && channel.parsing_who)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ParseInfo(string[] code, string value)
        {
            if (code.Length > 3)
            {
                string name = code[4];
                if (!updated_text)
                {
                    return true;
                }
                Channel channel = _Network.getChannel(name);
                if (channel != null)
                {
                    string[] _chan = value.Split(' ');
                    foreach (var user in _chan)
                    {
                        string _user = user;
                        char _UserMode = '\0';
                        if (_user.Length > 0)
                        {
                            foreach (char mode in _Network.UChars)
                            {
                                if (_user[0] == mode)
                                {
                                    _UserMode = user[0];
                                    _user = _user.Substring(1);
                                }
                            }

                            lock (channel.UserList)
                            {
                                User _u = channel.UserFromName(_user);
                                if (_u == null && _user != "")
                                {
                                    channel.UserList.Add(new User(user, "", _Network, ""));
                                }
                                else
                                {
                                    if (_u != null)
                                    {
                                        _u.SymbolMode(_UserMode);
                                    }
                                }
                            }
                        }
                    }
                    channel.RedrawUsers();
                    channel.UpdateInfo();
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
                Channel channel = _Network.getChannel(name);
                if (channel != null)
                {
                    Graphics.Window curr = channel.RetrieveWindow();
                    if (curr != null)
                    {
                        while (curr.scrollback == null)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        WindowText(curr, "Topic: " + topic, Client.ContentLine.MessageStyle.Channel, true, date, !updated_text);
                    }
                    channel.Topic = topic;
                    channel.UpdateInfo();
                    Hooks._Network.ChannelTopic(topic, null, _Network, channel);
                    return true;
                }
            }
            return false;
        }

        private bool FinishChan(string[] code)
        {
            if (code.Length > 2)
            {
                Channel channel = _Network.getChannel(code[3]);
                if (channel != null)
                {
                    channel.RedrawUsers();
                    if (Configuration.Kernel.HidingParsed && channel.parsing_who)
                    {
                        channel.parsing_who = false;
                        return true;
                    }
                    channel.parsing_who = false;
                    channel.UpdateInfo();
                }
            }
            return false;
        }

        private bool TopicInfo(string[] code, string parameters)
        {
            if (code.Length > 5)
            {
                string name = code[3];
                string user = code[4];
                string time = code[5];
                Channel channel = _Network.getChannel(name);
                if (channel != null)
                {
                    channel.TopicDate = int.Parse(time);
                    channel.TopicUser = user;
                    Graphics.Window curr = channel.RetrieveWindow();
                    if (curr != null)
                    {
                        WindowText(curr, "Topic by: " + user + " date " + Network.convertUNIX(time).ToString(),
                            Client.ContentLine.MessageStyle.Channel, !channel.temporary_hide, date, !updated_text);
                        return true;
                    }
                    channel.UpdateInfo();
                }
            }
            return false;
        }

        private bool Kick(string source, string parameters, string value)
        {
            string user = parameters.Substring(parameters.IndexOf(" ") + 1);
            // petan!pidgeon@petan.staff.tm-irc.org KICK #support HelpBot :Removed from the channel
            string chan = parameters.Substring(0, parameters.IndexOf(" "));
            Channel channel = _Network.getChannel(chan);
            if (channel != null)
            {
                Graphics.Window window;
                window = channel.RetrieveWindow();
                if (window != null)
                {
                    if (Hooks._Network.UserKick(_Network, new User(user, null, _Network, null), new User(source, _Network), channel, value))
                    {
                        WindowText(window, messages.get("userkick", Core.SelectedLanguage,
                        new List<string> { source, user, value }),
                        Client.ContentLine.MessageStyle.Join, !channel.temporary_hide, date, !updated_text);
                    }

                    if (updated_text && channel.ContainsUser(user))
                    {
                        User delete = null;
                        delete = channel.UserFromName(user);
                        if (delete != null)
                        {
                            channel.UserList.Remove(delete);
                        }
                        if (delete.IsPidgeon)
                        {
                            channel.ChannelWork = false;
                            window.needIcon = true;
                        }
                    }
                    channel.RedrawUsers();
                    channel.UpdateInfo();
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
            Channel channel = _Network.getChannel(chan);
            if (channel != null)
            {
                Graphics.Window window;
                window = channel.RetrieveWindow();
                if (window != null)
                {
                    if (Hooks._Network.UserJoin(_Network, new User(user, _host, _Network, _ident), channel, updated_text, date))
                    {
                        WindowText(window, messages.get("join", Core.SelectedLanguage,
                            new List<string> { "%L%" + user + "%/L%!%D%" + _ident + "%/D%@%H%" + _host + "%/H%" }),
                            Client.ContentLine.MessageStyle.Join, !channel.temporary_hide, date, !updated_text);
                    }
                    if (updated_text)
                    {
                        lock (channel.UserList)
                        {
                            if (!channel.ContainsUser(user))
                            {
                                channel.UserList.Add(new User(user, _host, _Network, _ident));
                            }
                        }
                        channel.RedrawUsers();
                    }
                    channel.UpdateInfo();
                    return true;
                }
            }
            return false;
        }

        private bool ChannelBans2(string[] code)
        {
            if (code.Length > 4)
            {
                Channel channel = _Network.getChannel(code[3]);
                if (channel != null)
                {
                    if (channel.parsing_bans)
                    {
                        channel.parsing_bans = false;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ChannelBans(string[] code)
        {
            if (code.Length > 6)
            {
                Channel channel = _Network.getChannel(code[3]);
                if (channel != null)
                {
                    if (channel.Bans == null)
                    {
                        channel.Bans = new List<SimpleBan>();
                    }
                    if (!channel.ContainsBan(code[4]))
                    {
                        channel.Bans.Add(new SimpleBan(code[5], code[4], code[6]));
                        Core.SystemForm.Status();
                    }
                    channel.UpdateInfo();
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
            Channel channel = _Network.getChannel(chan);
            if (channel != null)
            {
                Graphics.Window window;
                window = channel.RetrieveWindow();
                User delete = null;
                if (window != null)
                {
                    if (Hooks._Network.UserPart(_Network, new User(user, _host, _Network, _ident), channel, value, updated_text, date))
                    {
                        WindowText(window, messages.get("window-p1",
                            Core.SelectedLanguage, new List<string> { "%L%" + user + "%/L%!%D%" + _ident + "%/D%@%H%" + _host + "%/H%", value }),
                            Client.ContentLine.MessageStyle.Part,
                            !channel.temporary_hide, date, !updated_text);
                    }

                    if (updated_text)
                    {
                        if (channel.ContainsUser(user))
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
                            channel.RedrawUsers();
                            channel.UpdateInfo();
                            return true;
                        }
                        return true;
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
            Channel channel = _Network.getChannel(chan);
            if (channel != null)
            {
                Graphics.Window window;
                window = channel.RetrieveWindow();
                if (window != null)
                {
                    if (Hooks._Network.Topic(_Network, source, channel, value, date, updated_text))
                    {
                        WindowText(window, messages.get("channel-topic",
                            Core.SelectedLanguage, new List<string> { source, value }), Client.ContentLine.MessageStyle.Channel,
                            !channel.temporary_hide, date, !updated_text);
                    }
                }
                channel.Topic = value;
                channel.UpdateInfo();
                return true;
            }
            return false;
        }

        private bool ProcessNick(string source, string parameters, string value)
        {
            string nick = source.Substring(0, source.IndexOf("!"));
            string _new = value;
            if (value == "" && parameters != "")
            {
                // server is fucked
                _new = parameters;
                // server is totally borked
                if (_new.Contains(" "))
                {
                    _new = _new.Substring(0, _new.IndexOf(" "));
                }
            }
            foreach (Channel item in _Network.Channels)
            {
                if (item.ChannelWork)
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
                                    item.RedrawUsers();
                                }
                                Graphics.Window window = item.RetrieveWindow();
                                if (window != null)
                                {
                                    WindowText(window, messages.get("protocol-nick", Core.SelectedLanguage,
                                        new List<string> { nick, _new }), Client.ContentLine.MessageStyle.Channel,
                                        !item.temporary_hide, date, !updated_text);
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
                if (chan.StartsWith(_Network.channel_prefix))
                {
                    Channel channel = _Network.getChannel(chan);
                    if (channel != null)
                    {
                        Graphics.Window window;
                        window = channel.RetrieveWindow();
                        string change = parameters.Substring(parameters.IndexOf(" "));
                        if (window != null)
                        {
                            WindowText(window, messages.get("channel-mode", Core.SelectedLanguage,
                                new List<string> { source, parameters.Substring(parameters.IndexOf(" ")) }),
                                Client.ContentLine.MessageStyle.Action, !channel.temporary_hide, date, !updated_text);
                        }

                        if (!updated_text)
                        {
                            return true;
                        }

                        while (change.StartsWith(" "))
                        {
                            change = change.Substring(1);
                        }

                        Client.Protocols.irc.Formatter formatter = new Protocols.irc.Formatter();

                        while (change.EndsWith(" ") && change.Length > 1)
                        {
                            change = change.Substring(0, change.Length - 1);
                        }

                        // we get all the mode changes for this channel
                        formatter.RewriteBuffer(change, _Network);

                        channel.ChannelMode.ChangeMode("+" + formatter.channelModes);

                        foreach (SimpleMode m in formatter.getMode)
                        {
                            if (_Network.CUModes.Contains(m.Mode) && m.ContainsParameter)
                            {
                                User flagged_user = channel.UserFromName(m.Parameter);
                                if (flagged_user != null)
                                {
                                    flagged_user.ChannelMode.ChangeMode("+" + m.Mode);
                                }
                            }

                            if (m.ContainsParameter)
                            {
                                switch (m.Mode.ToString())
                                {
                                    case "b":
                                        if (channel.Bans == null)
                                        {
                                            channel.Bans = new List<SimpleBan>();
                                        }
                                        break;
                                }

                                if (channel.Bans == null)
                                {
                                    channel.Bans = new List<SimpleBan>();
                                }

                                lock (channel.Bans)
                                {
                                    channel.Bans.Add(new SimpleBan(user, m.Parameter, ""));
                                }
                            }
                        }

                        foreach (SimpleMode m in formatter.getRemovingMode)
                        {
                            if (_Network.CUModes.Contains(m.Mode) && m.ContainsParameter)
                            {
                                User flagged_user = channel.UserFromName(m.Parameter);
                                if (flagged_user != null)
                                {
                                    flagged_user.ChannelMode.ChangeMode("-" + m.Mode);
                                }
                            }

                            if (m.ContainsParameter)
                            {
                                switch (m.Mode.ToString())
                                {
                                    case "b":
                                        if (channel.Bans == null)
                                        {
                                            channel.Bans = new List<SimpleBan>();
                                        }
                                        channel.RemoveBan(m.Parameter);
                                        break;
                                }
                            }
                        }
                        channel.UpdateInfo();
                        channel.RedrawUsers();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
