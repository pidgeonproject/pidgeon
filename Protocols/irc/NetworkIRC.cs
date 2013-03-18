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
                _Network.parsed_info = true;
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

        private bool ParseUser(string[] code)
        {
            if (code.Length > 8)
            {
                Channel channel = _Network.getChannel(code[3]);
                string ident = code[4];
                string host = code[5];
                string nick = code[7];
                string server = code[6];
                char mode = '\0';
                if (code[8].Length > 0)
                {
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
                        if (!channel.containsUser(nick))
                        {
                            User _user = null;
                            if (mode != '\0')
                            {
                                _user = new User(mode.ToString() + nick, host, _Network, ident);
                            }
                            else
                            {
                                _user = new User(nick, host, _Network, ident);
                            }
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

        private bool ParseInfo(string[] code, string[] data)
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
                    string[] _chan = data[2].Split(' ');
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
                                User _u = channel.userFromName(_user);
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
                    channel.redrawUsers();
                    channel.UpdateInfo();
                    return true;
                }
            }
            return false;
        }

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
            string message = value;
            if (!chan.Contains(_Network.channel_prefix))
            {
                string uc;
                if (message.StartsWith(_Protocol.delimiter.ToString()))
                {
                    if (updated_text)
                    {
                        uc = message.Substring(1);
                        if (uc.Contains(_Protocol.delimiter))
                        {
                            uc = uc.Substring(0, uc.IndexOf(_Protocol.delimiter.ToString()));
                        }
                        uc = uc.ToUpper();
                        switch (uc)
                        {
                            case "VERSION":
                                _Network.Transfer("NOTICE " + _nick + " :" + _Protocol.delimiter.ToString() + "VERSION " + Configuration.Version + " http://pidgeonclient.org/wiki/",
                                    Configuration.Priority.Low);
                                break;
                            case "TIME":
                                _Network.Transfer("NOTICE " + _nick + " :" + _Protocol.delimiter.ToString() + "TIME " + DateTime.Now.ToString(),
                                    Configuration.Priority.Low);
                                break;
                            case "PING":
                                if (message.Length > 6)
                                {
                                    string time = message.Substring(6);
                                    if (time.Contains(_Network._Protocol.delimiter))
                                    {
                                        time = message.Substring(0, message.IndexOf(_Network._Protocol.delimiter));
                                        _Network.Transfer("NOTICE " + _nick + " :" + _Protocol.delimiter.ToString() + "PING " + time,
                                            Configuration.Priority.Low);
                                    }
                                }
                                break;
                        }
                    }
                    if (Configuration.irc.DisplayCtcp)
                    {
                        WindowText(_Network.SystemWindow, "CTCP from (" + _nick + ") " + message,
                            Scrollback.MessageStyle.Message, true, date, !updated_text);
                        return true; ;
                    }
                    return true;
                }

            }
            User user = new User(_nick, _host, _Network, _ident);
            if (Ignoring.Matches(message, user))
            {
                return true;
            }
            Channel channel = null;
            if (chan.StartsWith(_Network.channel_prefix))
            {
                channel = _Network.getChannel(chan);
                if (channel != null)
                {
                    Window window;
                    window = channel.retrieveWindow();
                    if (window != null)
                    {
                        if (message.StartsWith(_Protocol.delimiter.ToString() + "ACTION"))
                        {
                            message = message.Substring("xACTION".Length);
                            channel.retrieveWindow().scrollback.InsertText(">>>>>>" + _nick + message, Scrollback.MessageStyle.Action,
                                !channel.temporary_hide, date, !updated_text);
                            return true;
                        }
                        channel.retrieveWindow().scrollback.InsertText(_Protocol.PRIVMSG(user.Nick, message),
                            Scrollback.MessageStyle.Message, !channel.temporary_hide, date, !updated_text);
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
                    if (!_Protocol.Windows.ContainsKey(_Network.window + chan))
                    {
                        _Network.Private(chan);
                    }
                    _Protocol.Windows[_Network.window + chan].scrollback.InsertText(_Protocol.PRIVMSG(chan, message),
                        Scrollback.MessageStyle.Message, updated_text, date, !updated_text);
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
                WindowText(_Network.SystemWindow, name + " is currently away: " + message, Scrollback.MessageStyle.System, true, date, true);
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
                if (logintime == null)
                {
                    return false;
                }
                WindowText(_Network.SystemWindow, "WHOIS " + name + " is online since " + logintime.ToString() + "(" + (DateTime.Now - logintime).ToString() + " ago) idle for " + idle + " seconds", Scrollback.MessageStyle.System, true, date, true);
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
            string _new = value;
            foreach (Channel item in _Network.Channels)
            {
                if (item.ChannelWork)
                {
                    User target = item.userFromName(user);
                    if (target != null)
                    {
                        Window window = item.retrieveWindow();
                        if (window != null && window.scrollback != null)
                        {
                            WindowText(window, messages.get("protocol-quit", Core.SelectedLanguage,
                                new List<string> { "%L%" + user + "%/L%!%D%" + _ident + "%/D%@%H%" + _host + "%/H%", value }),
                                Scrollback.MessageStyle.Join,
                                !item.temporary_hide, date, !updated_text);
                        }
                        if (updated_text)
                        {
                            lock (item.UserList)
                            {
                                item.UserList.Remove(target);
                            }
                            item.UpdateInfo();
                            item.redrawUsers();
                        }
                    }
                }
            }
            return true;
        }
    }
}
