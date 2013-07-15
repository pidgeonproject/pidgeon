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
using System.Linq;
using System.Text;

namespace Client
{
    /// <summary>
    /// IRC protocol handler
    /// </summary>
    public partial class ProcessorIRC
    {
        /// <summary>
        /// Retrieve information about the server
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="value">Text</param>
        /// <returns></returns>
        private bool Info(string command, string parameters, string value)
        {
            if (parameters.Contains("PREFIX=("))
            {
                string cmodes = parameters.Substring(parameters.IndexOf("PREFIX=(") + 8);
                cmodes = cmodes.Substring(0, cmodes.IndexOf(")"));
                lock (_Network.CUModes)
                {
                    _Network.CUModes.Clear();
                    _Network.CUModes.AddRange(cmodes.ToArray<char>());
                }
                cmodes = parameters.Substring(parameters.IndexOf("PREFIX=(") + 8);
                cmodes = cmodes.Substring(cmodes.IndexOf(")") + 1, _Network.CUModes.Count);

                _Network.UChars.Clear();
                _Network.UChars.AddRange(cmodes.ToArray<char>());
            }
            if (parameters.Contains("CHANMODES="))
            {
                string xmodes = parameters.Substring(parameters.IndexOf("CHANMODES=") + 11);
                xmodes = xmodes.Substring(0, xmodes.IndexOf(" "));
                string[] _mode = xmodes.Split(',');
                _Network.ParsedInfo = true;
                if (_mode.Length == 4)
                {
                    _Network.PModes.Clear();
                    _Network.CModes.Clear();
                    _Network.XModes.Clear();
                    _Network.SModes.Clear();
                    _Network.PModes.AddRange(_mode[0].ToArray<char>());
                    _Network.XModes.AddRange(_mode[1].ToArray<char>());
                    _Network.SModes.AddRange(_mode[2].ToArray<char>());
                    _Network.CModes.AddRange(_mode[3].ToArray<char>());
                }
            }
            return true;
        }

        /// <summary>
        /// This command is sent by IRC server when you join a channel, it contains some basic information including list of users
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ChannelData(string command, string parameters, string value)
        {
            string channel_name = parameters.Substring(parameters.IndexOf(" ") + 1);
            uint user_count = 0;
            if (channel_name.Contains(" "))
            {
                if (!uint.TryParse(channel_name.Substring(channel_name.IndexOf(" ") + 1), out user_count))
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

        /// <summary>
        /// This is called for PRIVMSG from server
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ProcessPM(string source, string parameters, string value)
        {
            string _nick = "{unknown nick}";
            string _ident = "{unknown ident}";
            string _host = "{unknown host}";
            string chan = null;
            if (source.Contains("!"))
            {
                _nick = source.Substring(0, source.IndexOf("!"));
                _ident = source.Substring(source.IndexOf("!") + 1);
                _ident = _ident.Substring(0, _ident.IndexOf("@"));
            }
            else
            {
                Core.DebugLog("Parser error: " + source);
            }

            if (source.Contains("@"))
            {
                _host = source.Substring(source.IndexOf("@") + 1);
            }

            chan = parameters.Replace(" ", "");
            User user = null;
            string message = value;
            if (!chan.Contains(_Network.channel_prefix))
            {
                string uc;
                if (message.StartsWith(_Protocol.delimiter.ToString()))
                {
                    string trimmed = message;
                    if (trimmed.StartsWith(_Protocol.delimiter.ToString()))
                    {
                        trimmed = trimmed.Substring(1);
                    }
                    if (trimmed.Length > 1 && trimmed[trimmed.Length - 1] == _Protocol.delimiter)
                    {
                        trimmed = trimmed.Substring(0, trimmed.Length - 1);
                    }
                    if (message.StartsWith(_Protocol.delimiter.ToString() + "ACTION"))
                    {
                        message = message.Substring("xACTION".Length);
                        if (message.Length > 1 && message.EndsWith(_Protocol.delimiter.ToString()))
                        {
                            message = message.Substring(0, message.Length - 1);
                        }
                        user = _Network.getUser(_nick);
                        if (user != null)
                        {
                            lock (_Network.PrivateWins)
                            {
                                if (_Network.PrivateWins.ContainsKey(user))
                                {
                                    WindowText(_Network.PrivateWins[user], ">>>>>>" + _nick + message, Client.ContentLine.MessageStyle.Action, updated_text,
                                        date, !updated_text);
                                    return true;
                                }
                                else
                                {
                                    Core.DebugLog("Network " + _Network.ServerName + " doesn't have user (" + _nick + ") ignoring ctcp");
                                }
                            }
                        }
                        else
                        {
                            Core.DebugLog("Message from unknown user: " + message);
                        }
                        return true;
                    }

                    string reply = null;

                    if (updated_text)
                    {
                        uc = message.Substring(1);
                        if (uc.Contains(_Protocol.delimiter))
                        {
                            uc = uc.Substring(0, uc.IndexOf(_Protocol.delimiter.ToString()));
                        }
                        if (uc.Contains(" "))
                        {
                            uc = uc.Substring(0, uc.IndexOf(" "));
                        }
                        uc = uc.ToUpper();
                        if (!Configuration.irc.FirewallCTCP)
                        {
                            if (Hooks._Network.ParseCTCP(_Network, user, message))
                            {
                                return true;
                            }
                            switch (uc)
                            {
                                case "VERSION":
                                    reply = "VERSION " + Configuration.Version + " build: " + RevisionProvider.GetHash() + " http://pidgeonclient.org/wiki/";
                                    if (Configuration.irc.DetailedVersion)
                                    {
                                        reply += " operating system " + Environment.OSVersion.ToString();
                                    }
                                    _Network.Transfer("NOTICE " + _nick + " :" + _Protocol.delimiter.ToString() + reply,
                                        Configuration.Priority.Low);
                                    break;
                                case "TIME":
                                    reply = "TIME " + DateTime.Now.ToString();
                                    _Network.Transfer("NOTICE " + _nick + " :" + _Protocol.delimiter.ToString() + reply,
                                        Configuration.Priority.Low);
                                    break;
                                case "PING":
                                    if (message.Length > 6)
                                    {
                                        string time = message.Substring(6);
                                        if (time.Contains(_Network._Protocol.delimiter))
                                        {
                                            reply = "PING " + time;
                                            time = message.Substring(0, message.IndexOf(_Network._Protocol.delimiter));
                                            _Network.Transfer("NOTICE " + _nick + " :" + _Protocol.delimiter.ToString() + reply,
                                                Configuration.Priority.Low);
                                        }
                                    }
                                    break;
                                case "DCC":
                                    string message2 = message;
                                    if (message.Length < 5 || !message.Contains(" "))
                                    {
                                        Core.DebugLog("Malformed DCC " + message);
                                        return false;
                                    }
                                    if (message2.EndsWith(_Protocol.delimiter.ToString()))
                                    {
                                        message2 = message2.Substring(0, message2.Length - 1);
                                    }
                                    string[] list = message2.Split(' ');
                                    if (list.Length < 5)
                                    {
                                        Core.DebugLog("Malformed DCC " + message);
                                        return false;
                                    }
                                    string type = list[1];
                                    int port = 0;
                                    if (!int.TryParse(list[4], out port))
                                    {
                                        Core.DebugLog("Malformed DCC " + message2);
                                        return false;
                                    }
                                    if (port < 1)
                                    {
                                        Core.DebugLog("Malformed DCC " + message2);
                                        return false;
                                    }
                                    switch (type.ToLower())
                                    {
                                        case "send":
                                            break;
                                        case "chat":
                                            Core.OpenDCC(list[3], port, _nick, false, false, _Network);
                                            return true;
                                        case "securechat":
                                            Core.OpenDCC(list[3], port, _nick, false, true, _Network);
                                            return true;
                                    }
                                    Core.DebugLog("Malformed DCC " + message2);
                                    return false;
                            }
                        }
                    }
                    if (Configuration.irc.DisplayCtcp)
                    {
                        WindowText(_Network.SystemWindow, "CTCP from (" + _nick + ") " + trimmed,
                            Client.ContentLine.MessageStyle.Message, true, date, !updated_text);
                    }
                    if (Configuration.irc.ShowReplyForCTCP && reply != null)
                    {
                        WindowText(_Network.SystemWindow, "CTCP response to (" + _nick + ") " + reply,
                            Client.ContentLine.MessageStyle.Message, true, date, !updated_text);
                    }
                    return true;
                }

            }
            user = new User(_nick, _host, _Network, _ident);
            Channel channel = null;
            if (chan.StartsWith(_Network.channel_prefix))
            {
                channel = _Network.getChannel(chan);
                if (channel != null)
                {
                    Graphics.Window window;
                    window = channel.RetrieveWindow();
                    if (window != null)
                    {
                        if (Ignoring.Matches(message, user))
                        {
                            if (Hooks._Scrollback.Ignore(window, user, message, updated_text, date))
                            {
                                return true;
                            }
                        }
                        if (message.StartsWith(_Protocol.delimiter.ToString() + "ACTION"))
                        {
                            message = message.Substring("xACTION".Length);
                            if (message.Length > 1 && message.EndsWith(_Protocol.delimiter.ToString()))
                            {
                                message = message.Substring(0, message.Length - 1);
                            }
                            WindowText(window, ">>>>>>" + _nick + message, Client.ContentLine.MessageStyle.Action,
                                !channel.temporary_hide, date, !updated_text);
                            return true;
                        }
                        WindowText(window, _Protocol.PRIVMSG(user.Nick, message),
                            Client.ContentLine.MessageStyle.Message, !channel.temporary_hide, date, !updated_text);
                    }
                    channel.UpdateInfo();
                    return true;
                }
                return true;
            }
            if (chan == _Network.Nickname)
            {
                chan = source.Substring(0, source.IndexOf("!"));
                lock (_Protocol.Windows)
                {
                    if (!_Protocol.Windows.ContainsKey(_Network.SystemWindowID + chan))
                    {
                        _Network.Private(chan);
                    }
                    Graphics.Window w = _Protocol.Windows[_Network.SystemWindowID + chan];
                    WindowText(w, _Protocol.PRIVMSG(chan, message),
                        Client.ContentLine.MessageStyle.Message, updated_text, date, !updated_text);
                }
                return true;
            }
            return false;
        }

        private bool Idle2(string source, string parameters, string value)
        {
            if (parameters.Contains(" "))
            {
                string name = parameters.Substring(parameters.IndexOf(" ") + 1);
                string message = value;
                WindowText(_Network.SystemWindow, name + " is currently away: " + message, Client.ContentLine.MessageStyle.System, true, date, true);
                return true;
            }
            return false;
        }

        private bool WhoisText(string source, string parameters, string value)
        {
            if (parameters.Contains(" "))
            {
                string name = parameters.Substring(parameters.IndexOf(" ") + 1);
                WindowText(_Network.SystemWindow, "WHOIS " + name + " " + value, Client.ContentLine.MessageStyle.System, true, date, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parsing the first line of whois
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool WhoisLoad(string source, string parameters, string value)
        {
            if (!parameters.Contains(" "))
            {
                return false;
            }
            string[] user = parameters.Split(' ');
            if (user.Length > 3)
            {
                string host = user[1] + "!" + user[2] + "@" + user[3];
                WindowText(_Network.SystemWindow, "Start of WHOIS record for " + host + " " + value, Client.ContentLine.MessageStyle.System, true, date, true);
            }
            return true;
        }

        /// <summary>
        /// Parsing last line of whois
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool WhoisFn(string source, string parameters, string value)
        {
            WindowText(_Network.SystemWindow, "END OF WHOIS for " + parameters, Client.ContentLine.MessageStyle.System, true, date, true);
            return true;
        }

        /// <summary>
        /// Parsing the channels of whois
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool WhoisCh(string source, string parameters, string value)
        {
            if (parameters.Contains(" "))
            {
                string name = parameters.Substring(parameters.IndexOf(" ") + 1);
                WindowText(_Network.SystemWindow, "WHOIS " + name + " is in channels: " + value, Client.ContentLine.MessageStyle.System, true, date, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parsing the line of whois text
        /// </summary>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool WhoisSv(string source, string parameters, string value)
        {
            if (parameters.Contains(" "))
            {
                string name = parameters.Substring(parameters.IndexOf(" ") + 1);
                if (!name.Contains(" "))
                {
                    Core.DebugLog("Invalid whois record " + parameters);
                    return false;
                }
                string server = name.Substring(name.IndexOf(" ") + 1);
                name = name.Substring(0, name.IndexOf(" "));
                WindowText(_Network.SystemWindow, "WHOIS " + name + " is using server: " + server + " (" + value + ")", Client.ContentLine.MessageStyle.System, true, date, true);
                return true;
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
                DateTime logintime = Network.convertUNIX(uptime);
                WindowText(_Network.SystemWindow, "WHOIS " + name + " is online since " + logintime.ToString() + " (" + (DateTime.Now - logintime).ToString() + " ago) idle for " + idle + " seconds", Client.ContentLine.MessageStyle.System, true, date, true);
                return true;
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
            foreach (Channel item in _Network.Channels)
            {
                if (item.ChannelWork)
                {
                    User target = item.UserFromName(user);
                    if (target != null)
                    {
                        Graphics.Window window = item.RetrieveWindow();
                        if (window != null && window.scrollback != null)
                        {
                            if (Hooks._Network.UserQuit(_Network, target, value, window, updated_text, date))
                            {
                                WindowText(window, messages.get("protocol-quit", Core.SelectedLanguage,
                                    new List<string> { "%L%" + user + "%/L%!%D%" + _ident + "%/D%@%H%" + _host + "%/H%", value }),
                                    Client.ContentLine.MessageStyle.Join,
                                    !item.temporary_hide, date, !updated_text);
                            }
                        }
                        if (updated_text)
                        {
                            lock (item.UserList)
                            {
                                item.UserList.Remove(target);
                            }
                            item.UpdateInfo();
                            item.RedrawUsers();
                        }
                    }
                }
            }
            return true;
        }
    }
}
