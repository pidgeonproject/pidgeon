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
using System.Text;

namespace Client
{
    public partial class ProcessorIRC
    {
        /// <summary>
        /// Network
        /// </summary>
        private Network _Network = null;
        /// <summary>
        /// Protocol of this network
        /// </summary>
        private Protocol _Protocol = null;
        private string text;
        private long date = 0;
        private bool updated_text = true;
        private bool isServices = false;
        /// <summary>
        /// Time
        /// </summary>
        public DateTime pong;

        private void Ping()
        {
            pong = DateTime.Now;
            return;
        }

        private bool Pong(string source, string parameters, string _value)
        {
            _Network.Transfer("PONG :" + _value, Configuration.Priority.Low);
            return true;
        }

        /// <summary>
        /// Process a data that affect the current user
        /// </summary>
        /// <param name="source"></param>
        /// <param name="_data2"></param>
        /// <param name="_value"></param>
        /// <returns></returns>
        private bool ProcessThis(string source, string[] _data2, string _value)
        {
            if (source.StartsWith(_Network.Nickname + "!"))
            {
                if (_data2.Length > 1)
                {
                    if (_data2[1].Contains("JOIN"))
                    {
                        string channel = null;
                        if (!updated_text)
                        {
                            return true;
                        }
                        if (_data2.Length > 2)
                        {
                            if (_data2[2] != null && _data2[2] != "")
                            {
                                channel = _data2[2];
                            }
                        }
                        if (channel == null)
                        {
                            channel = _value;
                        }
                        Channel curr = _Network.getChannel(channel);
                        if (curr == null)
                        {
                            curr = _Network.Channel(channel, !Configuration.UserData.SwitchWindowOnJoin);
                        }
                        else
                        {
                            curr.ChannelWork = true;
                            curr.partRequested = false;
                            Graphics.Window xx = curr.RetrieveWindow();
                            if (xx != null)
                            {
                                xx.needIcon = true;
                            }
                        }
                        if (updated_text)
                        {
                            if (Configuration.ChannelModes.aggressive_mode)
                            {
                                _Network.Transfer("MODE " + channel, Configuration.Priority.Low);
                            }

                            if (Configuration.ChannelModes.aggressive_exception)
                            {
                                curr.parsing_xe = true;
                                _Network.Transfer("MODE " + channel + " +e", Configuration.Priority.Low);
                            }

                            if (Configuration.ChannelModes.aggressive_bans)
                            {
                                curr.parsing_bans = true;
                                _Network.Transfer("MODE " + channel + " +b", Configuration.Priority.Low);
                            }

                            if (Configuration.ChannelModes.aggressive_invites)
                            {
                                _Network.Transfer("MODE " + channel + " +I", Configuration.Priority.Low);
                            }

                            if (Configuration.ChannelModes.aggressive_channel)
                            {
                                curr.parsing_who = true;
                                _Network.Transfer("WHO " + channel, Configuration.Priority.Low);
                            }
                        }
                        return true;
                    }
                }

                if (_data2.Length > 1)
                {
                    if (_data2[1].Contains("NICK"))
                    {
                        string _new = _value;
                        if (_value == "" && _data2.Length > 2 && _data2[2] != "")
                        {
                            // server is fucked
                            _new = _data2[2];
                            // server is totally borked
                            if (_new.Contains(" "))
                            {
                                _new = _new.Substring(0, _new.IndexOf(" "));
                            }
                        }
                        _Network.SystemWindow.scrollback.InsertText(messages.get("protocolnewnick", Core.SelectedLanguage, new List<string> { _new }),
                            Client.ContentLine.MessageStyle.User, true, date);
                        _Network.Nickname = _new;
                    }
                    if (_data2[1].Contains("PART"))
                    {
                        string channel = _data2[2];
                        if (_data2[2].Contains(_Network.channel_prefix))
                        {
                            channel = _data2[2];
                            Channel c = _Network.getChannel(channel);
                            if (c != null)
                            {
                                Graphics.Window Chat = c.RetrieveWindow();
                                c.ChannelWork = false;
                                if (Chat != null)
                                {
                                    Chat.needIcon = true;
                                    if (!c.partRequested)
                                    {
                                        c.ChannelWork = false;
                                        Chat.scrollback.InsertText(messages.get("part1", Core.SelectedLanguage),
                                            Client.ContentLine.MessageStyle.Message, !c.temporary_hide, date);
                                    }
                                    else
                                    {
                                        Chat.scrollback.InsertText(messages.get("part2", Core.SelectedLanguage),
                                            Client.ContentLine.MessageStyle.Message, !c.temporary_hide, date);
                                    }
                                }
                                c.UpdateInfo();
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Process the line
        /// </summary>
        /// <returns></returns>
        public bool ProfiledResult()
        {
            if (Configuration.Kernel.Profiler)
            {
                Core.Profiler profiler = new Core.Profiler("IRC.ProfiledResult()");
                bool result = Result();
                profiler.Done();
                return result;
            }
            return Result();
        }

        private bool Result()
        {
            string last = "";
            try
            {
                bool OK = false;
                if (text == null)
                {
                    return false;
                }
                if (text.StartsWith(":"))
                {
                    // :ottermaton!~mark@lancaster.hacc.edu JOIN #debian
                    last = text;
                    string command = "";
                    string parameters = "";
                    string value = "";
                    string command_body = text.Substring(1);
                    string source = command_body;
                    if (command_body.Contains(" :"))
                    {
                        if (command_body.IndexOf(" :", StringComparison.Ordinal) < 0)
                        {
                            Core.DebugLog("Malformed text, probably hacker: " + text);
                            return false;
                        }
                        command_body = command_body.Substring(0, command_body.IndexOf(" :", StringComparison.Ordinal));
                    }
                    source = source.Substring(0, source.IndexOf(" "));
                    if (command_body.Length < source.Length + 1)
                    {
                        Core.DebugLog("Invalid IRC string: " + text);
                    }
                    string command2 = command_body.Substring(source.Length + 1);

                    if (command2.Contains(" "))
                    {
                        command = command2.Substring(0, command2.IndexOf(" "));
                        if (command2.Length > 1 + command.Length)
                        {
                            parameters = command2.Substring(1 + command.Length);
                            if (parameters.EndsWith(" "))
                            {
                                parameters = parameters.Substring(0, parameters.Length - 1);
                            }
                        }
                    }
                    else
                    {
                        command = command2;
                    }

                    if (text.Length > (3 + command2.Length + source.Length))
                    {
                        value = text.Substring(3 + command2.Length + source.Length);
                    }

                    if (value.StartsWith(":"))
                    {
                        value = value.Substring(1);
                    }

                    string[] code = command_body.Split(' ');

                    if (ProcessThis(source, code, value))
                    {
                        OK = true;
                    }

                    switch (command)
                    {
                        case "002":
                        case "003":
                        case "004":
                            Hooks._Network.NetworkInfo(_Network, command, parameters, value);
                            break;
                        case "005":
                            Info(command, parameters, value);
                            Hooks._Network.NetworkInfo(_Network, command, parameters, value);
                            if (!_Network.isLoaded)
                            {
                                Hooks._Network.AfterConnectToNetwork(_Network);
                            }
                            break;
                        case "301":
                            if (Idle2(command, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "305":
                            _Network.IsAway = false;
                            break;
                        case "306":
                            _Network.IsAway = true;
                            break;
                        case "317":
                            if (Configuration.irc.FriendlyWhois)
                            {
                                if (IdleTime(command, parameters, value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case "321":
                            if (_Network.SuppressData)
                            {
                                return true;
                            }
                            break;
                        case "322":
                            if (ChannelData(command, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "323":
                            if (_Network.SuppressData)
                            {
                                _Network.SuppressData = false;
                                return true;
                            }
                            _Network.DownloadingList = false;
                            break;
                        case "307":
                        case "310":
                        case "313":
                        case "378":
                        case "671":
                            if (Configuration.irc.FriendlyWhois)
                            {
                                if (WhoisText(command, parameters, value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case "311":
                            if (Configuration.irc.FriendlyWhois)
                            {
                                if (WhoisLoad(command, parameters, value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case "312":
                            if (Configuration.irc.FriendlyWhois)
                            {
                                if (WhoisSv(command, parameters, value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case "318":
                            if (Configuration.irc.FriendlyWhois)
                            {
                                if (WhoisFn(command, parameters, value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case "319":
                            if (Configuration.irc.FriendlyWhois)
                            {
                                if (WhoisCh(command, parameters, value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case "433":
                            if (!_Network.usingNick2 && Configuration.UserData.Nick2 != "")
                            {
                                _Network.usingNick2 = true;
                                _Network.Transfer("NICK " + Configuration.UserData.Nick2, Configuration.Priority.High);
                                _Network.Nickname = Configuration.UserData.Nick2;
                            }
                            break;
                        case "PING":
                            if (Pong(command, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "PONG":
                            Ping();
                            return true;
                        case "INFO":
                            _Network.SystemWindow.scrollback.InsertText(text.Substring(text.IndexOf("INFO") + 5), Client.ContentLine.MessageStyle.User, true, date, !updated_text);
                            return true;
                        case "NOTICE":
                            if (parameters.Contains(_Network.channel_prefix))
                            {
                                Channel channel = _Network.getChannel(parameters);
                                if (channel != null)
                                {
                                    Graphics.Window window;
                                    window = channel.RetrieveWindow();
                                    if (window != null)
                                    {
                                        window.scrollback.InsertText("[" + source + "] " + value, Client.ContentLine.MessageStyle.Message, true, date, !updated_text);
                                        return true;
                                    }
                                }
                            }
                            _Network.SystemWindow.scrollback.InsertText("[" + source + "] " + value, Client.ContentLine.MessageStyle.Message, true, date, !updated_text);
                            return true;
                        case "NICK":
                            if (ProcessNick(source, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "PRIVMSG":
                            if (ProcessPM(source, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "TOPIC":
                            if (Topic(source, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "MODE":
                            if (Mode(source, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "PART":
                            if (Part(source, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "QUIT":
                            if (Quit(source, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "JOIN":
                            if (Join(source, parameters, value))
                            {
                                return true;
                            }
                            break;
                        case "KICK":
                            if (Kick(source, parameters, value))
                            {
                                return true;
                            }
                            break;
                    }

                    if (command_body.Contains(" "))
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
                                if (ChannelInfo(code, command, source, parameters, value))
                                {
                                    return true;
                                }
                                break;
                            case "332":
                                if (ChannelTopic(code, command, source, parameters, value))
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
                                if (ParseUser(code, value))
                                {
                                    return true;
                                }
                                break;
                            case "353":
                                if (ParseInfo(code, value))
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
                            case "368":
                                if (ChannelBans2(code))
                                {
                                    return true;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    // malformed requests this needs to exist so that it works with some broked ircd
                    string command = text;
                    string value = "";
                    if (command.Contains(" :"))
                    {
                        value = command.Substring(command.IndexOf(" :") + 2);
                        command = command.Substring(0, command.IndexOf(" :"));
                    }
                    // for extra borked ircd
                    if (command.Contains(" "))
                    {
                        command = command.Substring(0, command.IndexOf(" "));
                    }

                    switch (command)
                    {
                        case "PING":
                            Pong(command, null, value);
                            OK = true;
                            break;
                    }
                }
                if (!OK)
                {
                    // we have no idea what we just were to parse, so print it to system window
                    _Network.SystemWindow.scrollback.InsertText(text, Client.ContentLine.MessageStyle.System, true, date, true);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
                Core.DebugLog("String: " + last);
            }
            return true;
        }

        /// <summary>
        /// This function handle the writing to scrollback
        /// </summary>
        /// <param name="window"></param>
        /// <param name="text"></param>
        /// <param name="InputStyle"></param>
        /// <param name="WriteLog"></param>
        /// <param name="Date"></param>
        /// <param name="SuppressPing"></param>
        public void WindowText(Graphics.Window window, string text, Client.ContentLine.MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false)
        {
            bool logging = WriteLog;

            if (logging && isServices)
            {
                if (Configuration.Services.UsingCache && Configuration.Logs.ServicesLogs != Configuration.Logs.ServiceLogs.none)
                {
                    logging = true;
                }
                else if (Configuration.Logs.ServicesLogs == Configuration.Logs.ServiceLogs.none)
                {
                    logging = false;
                }
                else if (Configuration.Logs.ServicesLogs == Configuration.Logs.ServiceLogs.full)
                {
                    logging = true;
                }
                else if (Configuration.Logs.ServicesLogs == Configuration.Logs.ServiceLogs.incremental)
                {
                    logging = false;
                    if (updated_text)
                    {
                        logging = true;
                    }
                }
            }

            window.scrollback.InsertText(text, InputStyle, logging, Date, SuppressPing);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_network"></param>
        /// <param name="_text"></param>
        /// <param name="_pong"></param>
        /// <param name="_date">Date of this message, if you specify 0 the current time will be used</param>
        /// <param name="updated">If true this text will be considered as newly obtained information</param>
        public ProcessorIRC(Network _network, string _text, ref DateTime _pong, long _date = 0, bool updated = true)
        {
            _Network = _network;
            _Protocol = _network._Protocol;
            text = _text;
            pong = _pong;
            date = _date;
            updated_text = updated;
            if (_network._Protocol.GetType() == typeof(ProtocolSv))
            {
                isServices = true;
            }
        }
    }

}
