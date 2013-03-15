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
        /// Network
        /// </summary>
        public Network _Network = null;
        /// <summary>
        /// Protocol of this network
        /// </summary>
        public Protocol _Protocol = null;
        public string text;
        public DateTime pong;
        public long date = 0;
        public bool updated_text = true;
        public bool isServices = false;

        private void Ping()
        {
            pong = DateTime.Now;
            return;
        }

        public bool ProcessThis(string source, string[] data, string _value)
        {
            if (source.StartsWith(_Network.Nickname + "!"))
            {
                string[] _data2 = data[1].Split(' ');
                if (_data2.Length > 2)
                {
                    if (_data2[1].Contains("JOIN"))
                    {
                        if (!updated_text)
                        {
                            return true;
                        }
                        string channel = _data2[2];
                        if (_data2[2].Contains("#") == false)
                        {
                            channel = data[2];
                        }
                        Channel curr = _Network.getChannel(channel);
                        if (curr == null)
                        {
                            curr = _Network.WindowCreateNewJoin(channel);
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

                if (_data2.Length > 2)
                {
                    if (_data2[1].Contains("NICK"))
                    {
                        _Network.SystemWindow.scrollback.InsertText(messages.get("protocolnewnick", Core.SelectedLanguage, new List<string> { _value }), Scrollback.MessageStyle.User, true, date);
                        _Network.Nickname = _value;
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
                                Window Chat = c.retrieveWindow();
                                if (c != null)
                                {
                                    if (c.ChannelWork)
                                    {
                                        c.ChannelWork = false;
                                        Chat.scrollback.InsertText(messages.get("part1", Core.SelectedLanguage),
                                            Scrollback.MessageStyle.Message, !c.temporary_hide, date);
                                    }
                                    else
                                    {
                                        Chat.scrollback.InsertText(messages.get("part2", Core.SelectedLanguage),
                                            Scrollback.MessageStyle.Message, !c.temporary_hide, date);
                                    }
                                }
                                c.ChannelWork = false;
                                c.UpdateInfo();
                            }
                        }
                        return true;
                    }
                    if (_data2[1].Contains("KICK"))
                    {
                        string channel = _data2[2];
                        if (_data2[2].Contains(_Network.channel_prefix))
                        {
                            channel = _data2[2];
                            Channel c = _Network.getChannel(channel);
                            if (c != null)
                            {
                                c.ChannelWork = false;
                                c.UpdateInfo();
                            }
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        public bool Result()
        {
            string last = "";
            try
            {
                bool OK = false;
                if (text == null || text == "")
                {
                    return false;
                }
                if (text.StartsWith(":"))
                {
                    string[] data = text.Split(':');
                    if (data.Length > 1)
                    {
                        last = text;
                        string command = "";
                        string parameters = "";
                        string command2 = "";
                        string source;
                        string value;
                        source = text.Substring(1);
                        source = source.Substring(0, source.IndexOf(" "));
                        command2 = text.Substring(1);
                        command2 = command2.Substring(source.Length + 1);
                        if (command2.Contains(" :"))
                        {
                            command2 = command2.Substring(0, command2.IndexOf(" :"));
                        }
                        string[] Commands = command2.Split(' ');
                        if (Commands.Length > 0)
                        {
                            command = Commands[0];
                        }
                        if (Commands.Length > 1)
                        {
                            int curr = 1;
                            while (curr < Commands.Length)
                            {
                                parameters += Commands[curr] + " ";
                                curr++;
                            }
                            if (parameters.EndsWith(" "))
                            {
                                parameters = parameters.Substring(0, parameters.Length - 1);
                            }
                        }
                        value = "";
                        if (text.Length > 3 + command2.Length + source.Length)
                        {
                            value = text.Substring(3 + command2.Length + source.Length);
                        }
                        if (value.StartsWith(":"))
                        {
                            value = value.Substring(1);
                        }
                        string[] code = data[1].Split(' ');
                        if (ProcessThis(source, data, value))
                        {
                            OK = true;
                        }
                        switch (command)
                        {
                            case "001":
                                break;
                            case "002":
                            case "003":
                            case "004":
                            case "005":
                                Info(command, parameters, value);
                                break;
                            case "301":
                                if (Idle2(command, parameters, value))
                                {
                                    return true;
                                }
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
                            case "PONG":
                                Ping();
                                return true;
                            case "INFO":
                                _Network.SystemWindow.scrollback.InsertText(text.Substring(text.IndexOf("INFO") + 5), Scrollback.MessageStyle.User, true, date, !updated_text);
                                return true;
                            case "NOTICE":
                                if (parameters.Contains(_Network.channel_prefix))
                                {
                                    Channel channel = _Network.getChannel(parameters);
                                    if (channel != null)
                                    {
                                        Window window;
                                        window = channel.retrieveWindow();
                                        if (window != null)
                                        {
                                            window.scrollback.InsertText("[" + source + "] " + value, Scrollback.MessageStyle.Message, true, date, !updated_text);
                                            return true;
                                        }
                                    }
                                }
                                _Network.SystemWindow.scrollback.InsertText("[" + source + "] " + value, Scrollback.MessageStyle.Message, true, date, !updated_text);
                                return true;
                            case "PING":
                                _Network.Transfer("PONG ", Configuration.Priority.High);
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
                                    if (ParseUser(code))
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
                    }
                }
                if (!OK)
                {
                    // we have no idea what we just were to parse, so print it to system window
                    _Network.SystemWindow.scrollback.InsertText(text, Scrollback.MessageStyle.System, true, date, true);
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
        /// <param name="text"></param>
        /// <param name="InputStyle"></param>
        /// <param name="WriteLog"></param>
        /// <param name="Date"></param>
        /// <param name="SuppressPing"></param>
        public void WindowText(Window window, string text, Scrollback.MessageStyle InputStyle, bool WriteLog = true, long Date = 0, bool SuppressPing = false)
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
