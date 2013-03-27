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

namespace Client
{
    public partial class ProcessorIRC
    {
        private bool ChannelInfo(string[] code, string command, string source, string parameters, string _value)
        {
            if (code.Length > 3)
            {
                string name = code[2];
                string topic = _value;
                Channel channel = _Network.getChannel(code[3]);
                if (channel != null)
                {
                    Graphics.Window curr = channel.retrieveWindow();
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

        private bool ChannelData(string command, string parameters, string value)
        {
            string channel_name = parameters.Substring(parameters.IndexOf(" ") + 1);
            int user_count = 0;
            if (channel_name.Contains(" "))
            {
                if (!int.TryParse(channel_name.Substring(channel_name.IndexOf(" ") + 1), out user_count))
                {
                    user_count = 0;
                }

                channel_name = channel_name.Substring(0, channel_name.IndexOf(" "));
            }

            _Network.DownloadingList = true;

            lock (_Network.ChannelList)
            {
                Network.ChannelData channel = _Network.ContainsChannel(channel_name);
                if (channel == null)
                {
                    channel = new Network.ChannelData(user_count, channel_name, value);
                    _Network.ChannelList.Add(channel);
                }
                else
                {
                    channel.UserCount = user_count;
                    channel.ChannelTopic = value;
                }
                if (_Network.SuppressData)
                {
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
                    Graphics.Window curr = channel.retrieveWindow();
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
                    channel.redrawUsers();
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
                    Graphics.Window curr = channel.retrieveWindow();
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
            Channel channel = _Network.getChannel(parameters.Substring(0, parameters.IndexOf(" ")));
            if (channel != null)
            {
                Graphics.Window window;
                window = channel.retrieveWindow();
                if (window != null)
                {
                    window.scrollback.InsertText(messages.get("userkick", Core.SelectedLanguage,
                        new List<string> { source, user, value }),
                        Client.ContentLine.MessageStyle.Join, !channel.temporary_hide, date, !updated_text);

                    if (updated_text && channel.containsUser(user))
                    {
                        User delete = null;
                        delete = channel.userFromName(user);
                        if (delete != null)
                        {
                            channel.UserList.Remove(delete);
                        }
                    }
                    channel.redrawUsers();
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
                window = channel.retrieveWindow();
                if (window != null)
                {
                    WindowText(window, messages.get("join", Core.SelectedLanguage,
                        new List<string> { "%L%" + user + "%/L%!%D%" + _ident + "%/D%@%H%" + _host + "%/H%" }),
                        Client.ContentLine.MessageStyle.Join, !channel.temporary_hide, date, !updated_text);
                    if (updated_text)
                    {
                        lock (channel.UserList)
                        {
                            if (!channel.containsUser(user))
                            {
                                channel.UserList.Add(new User(user, _host, _Network, _ident));
                            }
                        }
                        channel.redrawUsers();
                    }
                    channel.UpdateInfo();
                    return true;
                }
                Hooks._Network.UserJoin(_Network, channel.userFromName(user), channel);
            }
            return false;
        }

        private bool ChannelBans(string[] code)
        {
            if (code.Length > 6)
            {
                string chan = code[3];
                Channel channel = _Network.getChannel(code[3]);
                if (channel != null)
                {
                    channel.UpdateInfo();
                    if (channel.Bans == null)
                    {
                        channel.Bans = new List<SimpleBan>();
                    }
                    if (!channel.containsBan(code[4]))
                    {
                        channel.Bans.Add(new SimpleBan(code[5], code[4], code[6]));
                        Core._Main.Status();
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
            Channel channel = _Network.getChannel(chan);
            if (!Hooks._Network.BeforePart(_Network, channel)) { return true; }
            if (channel != null)
            {
                Graphics.Window window;
                window = channel.retrieveWindow();
                User delete = null;
                if (window != null)
                {
                    WindowText(window, messages.get("window-p1",
                        Core.SelectedLanguage, new List<string> { "%L%" + user + "%/L%!%D%" + _ident + "%/D%@%H%" + _host + "%/H%", value }),
                        Client.ContentLine.MessageStyle.Part,
                        !channel.temporary_hide, date, !updated_text);

                    if (updated_text)
                    {
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
                            channel.redrawUsers();
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
            string user = source.Substring(0, source.IndexOf("!"));
            Channel channel = _Network.getChannel(chan);
            if (channel != null)
            {
                Graphics.Window window;
                channel.Topic = value;
                window = channel.retrieveWindow();
                if (window != null)
                {
                    WindowText(window, messages.get("channel-topic",
                        Core.SelectedLanguage, new List<string> { source, value }), Client.ContentLine.MessageStyle.Channel,
                        !channel.temporary_hide, date, !updated_text);
                    return true;
                }
                channel.UpdateInfo();
            }
            return false;
        }

        private bool ProcessNick(string source, string parameters, string value)
        {
            string nick = source.Substring(0, source.IndexOf("!"));
            string _new = value;
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
                                    item.redrawUsers();
                                }
                                Graphics.Window window = item.retrieveWindow();
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
                        window = channel.retrieveWindow();
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

                        channel.ChannelMode.ChangeMode(change);

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
                                if (_Network.CUModes.Contains(m) && curr <= parameters2.Count)
                                {
                                    User flagged_user = channel.userFromName(parameters2[curr]);
                                    if (flagged_user != null)
                                    {
                                        flagged_user.ChannelMode.ChangeMode(type.ToString() + m.ToString());
                                    }
                                    curr++;
                                }
                                if (parameters2.Count > curr)
                                {
                                    switch (m.ToString())
                                    {
                                        case "b":
                                            if (channel.Bans == null)
                                            {
                                                channel.Bans = new List<SimpleBan>();
                                            }

                                            if (type == '-')
                                            {
                                                channel.RemoveBan(parameters2[curr]);
                                            }
                                            else
                                            {
                                                lock (channel.Bans)
                                                {
                                                    channel.Bans.Add(new SimpleBan(user, parameters2[curr], ""));
                                                }
                                            }

                                            curr++;
                                            break;
                                    }
                                }
                            }
                        }
                        channel.UpdateInfo();
                        channel.redrawUsers();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
