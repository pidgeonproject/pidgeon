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
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Pidgeon.Protocols.Services
{
    public partial class ProtocolSv
    {
        private static class ResponsesSv
        {
            /// <summary>
            /// Message
            /// </summary>
            /// <param name="curr">Node</param>
            /// <param name="protocol">Protocol</param>
            public static void sMessage(XmlNode curr, ProtocolSv protocol)
            {
                string message_nick = curr.Attributes[0].Value;
                string message_text = curr.InnerText;
                string message_target = curr.Attributes[3].Value;
                string MQID = curr.Attributes[4].Value;
                Graphics.Window message_window = null;
                Network mn = protocol.retrieveNetwork(curr.Attributes[1].Value);
                if (Configuration.Services.UsingCache)
                {
                    string ID = protocol.sBuffer.getUID(curr.Attributes[1].Value);
                    if (ID != null)
                    {
                        protocol.sBuffer.networkInfo[ID].MQID(MQID);
                    }
                }
                long message_time = long.Parse(curr.Attributes[2].Value);
                if (mn != null)
                {
                    if (message_target.StartsWith(mn.ChannelPrefix, StringComparison.Ordinal))
                    {
                        Channel target = mn.GetChannel(message_target);
                        if (target != null)
                        {
                            message_window = target.RetrieveWindow();
                        }
                        else
                        {
                            Core.DebugLog("There is no channel " + message_target);
                            target = mn.MakeChannel(message_target);
                            message_window = target.RetrieveWindow();
                        }
                    }
                    else
                    {
                        message_window = mn.GetPrivateUserWindow(message_target);
                    }
                }

                if (message_window != null)
                {
                    message_window.scrollback.InsertTextAndIgnoreUpdate(Protocol.PRIVMSG(message_nick, message_text), Pidgeon.ContentLine.MessageStyle.Message, true, message_time, true);
                }
                else
                {
                    Core.DebugLog("There is no window for " + message_target);
                }
            }

            /// <summary>
            /// Incoming raw
            /// </summary>
            /// <param name="curr"></param>
            /// <param name="protocol"></param>
            public static void sRaw(XmlNode curr, ProtocolSv protocol)
            {
                if (curr.InnerText == "PERMISSIONDENY")
                {
                    protocol.SystemWindow.scrollback.InsertText("You can't send this command to server, because you aren't logged in",
                                                                    Pidgeon.ContentLine.MessageStyle.System, false);
                    return;
                }
                protocol.SystemWindow.scrollback.InsertText("Server responded to SRAW with this: " + curr.InnerText,
                                                             Pidgeon.ContentLine.MessageStyle.User, false);
            }

            /// <summary>
            /// Load
            /// </summary>
            /// <param name="curr">Node</param>
            /// <param name="protocol">Protocol</param>
            public static void sLoad(XmlNode curr, ProtocolSv protocol)
            {
                protocol.SystemWindow.scrollback.InsertText(curr.InnerText, Pidgeon.ContentLine.MessageStyle.System, false);
            }

            /// <summary>
            /// Status
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sStatus(XmlNode curr, ProtocolSv protocol)
            {
                switch (curr.InnerText)
                {
                    case "Connected":
                        protocol.ConnectionStatus = Status.Connected;
                        break;
                    case "WaitingPW":
                        protocol.ConnectionStatus = Status.WaitingPW;
                        break;
                }
            }

            /// <summary>
            /// Data
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sData(XmlNode curr, ProtocolSv protocol)
            {
                long date = 0;
                bool IsBacklog = false;
                string MQID = null;
                bool range = false;
                string id = "";
                foreach (XmlAttribute xx in curr.Attributes)
                {
                    switch (xx.Name)
                    {
                        case "time":
                            if (!long.TryParse(xx.Value, out date))
                            {
                                Core.DebugLog("Warning: " + xx.Value + " is not a correct time");
                            }
                            break;
                        case "MQID":
                            MQID = xx.Value;
                            break;
                        case "buffer":
                            if (xx.Name == "buffer")
                            {
                                id = xx.Value;
                                IsBacklog = true;
                            }
                            break;
                        case "range":
                            range = true;
                            break;
                    }
                }
                string name = curr.Attributes[0].Value;
                Network server = null;
                server = protocol.retrieveNetwork(name);
                if (server == null)
                {
                    server = new Network(name, protocol);
                    protocol.NetworkList.Add(server);
                    protocol.cache.Add(new Cache());
                    server.Nickname = protocol.nick;
                    server.IsConnected = true;
                }
                if (Configuration.Services.UsingCache)
                {
                    if (MQID != null)
                    {
                        string UID = protocol.sBuffer.getUID(name);

                        if (UID != null)
                        {
                            protocol.sBuffer.networkInfo[UID].MQID(MQID);
                        }
                    }
                }

                libirc.ProcessorIRC processor = null;

                if (IsBacklog || range)
                {
                    if (Core.SystemForm.DisplayingProgress == false)
                    {
                        protocol.BacklogMode(true);
                        Core.SystemForm.progress = double.Parse(id);
                        Core.SystemForm.DisplayingProgress = true;
                        protocol.FinishedLoading = false;
                        protocol.SuppressChanges = true;
                        Core.SystemForm.ProgressMax = protocol.cache[protocol.NetworkList.IndexOf(server)].size;
                    }

                    Core.SystemForm.progress = double.Parse(id);
                    Core.SystemForm.Status("Retrieving backlog from " + name + ", got " + id + "/" 
                        + protocol.cache[protocol.NetworkList.IndexOf(server)].size.ToString() + " datagrams");
                    if ((protocol.cache[protocol.NetworkList.IndexOf(server)].size - 2) < double.Parse(id))
                    {
                        IsBacklog = false;
                        protocol.BacklogMode(false);
                        Core.SystemForm.Status(protocol.getInfo());
                        Core.SystemForm.DisplayingProgress = false;
                        Core.SystemForm.progress = 0;
                        protocol.SuppressChanges = false;
                        if (Configuration.Services.UsingCache && Configuration.Services.MissingFix)
                        {
                            // get all holes we are missing from backlog
                            protocol.sBuffer.retrieveData(name);
                        }
                        lock (server.Channels)
                        {
                            foreach (Channel i in server.Channels.Values)
                            {
                                i.TemporarilyHidden = false;
                                i.IsParsingExceptionData = false;
                                i.IsParsingBanData = false;
                                i.IsParsingWhoData = false;
                            }
                        }
                    }
                }
                processor = new libirc.ProcessorIRC(server, curr.InnerText, ref protocol.pong, date, IsBacklog);
                processor.ProfiledResult();
            }

            /// <summary>
            /// Nick
            /// </summary>
            /// <param name="curr">Node</param>
            /// <param name="protocol">Protocol</param>
            public static void sNick(XmlNode curr, ProtocolSv protocol)
            {
                Network sv = protocol.retrieveNetwork(curr.Attributes[0].Value);
                if (sv != null)
                {
                    sv.Nickname = curr.InnerText;
                    sv.SystemWindow.scrollback.InsertText("Your nick was changed to " + curr.InnerText,
                        Pidgeon.ContentLine.MessageStyle.User, true);
                }
            }

            /// <summary>
            /// Connect to irc network
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sConnect(XmlNode curr, ProtocolSv protocol)
            {
                string network = curr.Attributes[0].Value;
                switch (curr.InnerText)
                {
                    case "CONNECTED":
                        protocol.SystemWindow.scrollback.InsertText("You are already connected to " + network, Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    case "PROBLEM":
                        protocol.SystemWindow.scrollback.InsertText(messages.get("service_error", Core.SelectedLanguage, new List<string> { network, curr.Attributes[1].Value }), Pidgeon.ContentLine.MessageStyle.System, false);
                        return;
                    case "OK":
                        Network _network = new Network(network, protocol);
                        _network.IsConnected = true;
                        _network.Nickname = protocol.nick;
                        protocol.cache.Add(new Cache());
                        protocol.NetworkList.Add(_network);
                        return;
                }
            }

            /// <summary>
            /// Global ident
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sGlobalident(XmlNode curr, ProtocolSv protocol)
            {
                protocol.SystemWindow.scrollback.InsertText(messages.get("pidgeon.globalident", Core.SelectedLanguage,
                                new List<string> { curr.InnerText }), Pidgeon.ContentLine.MessageStyle.User, true);
            }

            /// <summary>
            /// Back log data
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sBacklog(XmlNode curr, ProtocolSv protocol)
            {
                string network = curr.Attributes[0].Value;

                lock (protocol.WaitingNetw)
                {
                    if (protocol.WaitingNetw.Contains(network))
                    {
                        protocol.WaitingNetw.Remove(network);
                    }
                    if (protocol.WaitingNetw.Count == 0)
                    {
                        Core.SystemForm.Status(protocol.getInfo());
                    }
                    else
                    {
                        Core.SystemForm.Status("Waiting for backlog for " + protocol.WaitingNetw[0]);
                    }
                }

                Network server = protocol.retrieveNetwork(network);
                if (server != null)
                {
                    protocol.cache[protocol.NetworkList.IndexOf(server)].size = int.Parse(curr.InnerText);
                    lock (server.Channels)
                    {
                        foreach (Channel i in server.Channels.Values)
                        {
                            i.IsParsingWhoData = true;
                            i.IsParsingBanData = true;
                            i.TemporarilyHidden = true;
                        }
                    }
                }
            }

            /// <summary>
            /// global nick
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sGlobalNick(XmlNode curr, ProtocolSv protocol)
            {
                protocol.nick = curr.InnerText;
                protocol.SystemWindow.scrollback.InsertText(messages.get("pidgeon.globalnick", Core.SelectedLanguage,
                    new List<string> { curr.InnerText }), Pidgeon.ContentLine.MessageStyle.User, true);
            }

            /// <summary>
            /// network info
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sNetworkInfo(XmlNode curr, ProtocolSv protocol)
            {
                bool connected = false;
                switch (curr.InnerText)
                {
                    case "ONLINE":
                        connected = true;
                        break;
                    case "UNKNOWN":
                        return;
                    case "OFFLINE":
                        connected = false;
                        break;
                }
                string nick = null;
                string network = null;
                if (curr.Attributes != null)
                {
                    foreach (XmlAttribute info in curr.Attributes)
                    {
                        switch (info.Name)
                        {
                            case "network":
                                network = info.Value;
                                break;
                            case "nick":
                                nick = info.Value;
                                break;
                        }
                    }
                    foreach (Network s2 in protocol.NetworkList)
                    {
                        if (curr.Attributes[0].Value == s2.ServerName)
                        {
                            if (connected)
                            {
                                s2.IsConnected = true;
                            } else
                            {
                                s2.IsConnected = false;
                            }
                            if (nick != null)
                            {
                                s2.Nickname = nick;
                            }
                            break;
                        }
                    }
                }
            }

            /// <summary>
            /// Error
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sError(XmlNode curr, ProtocolSv protocol)
            {
                string error = "Error ocurred on services: ";
                string code = "unknown";
                string description = "this is an unknown error";
                if (curr.Attributes != null)
                {
                    foreach (XmlAttribute xx in curr.Attributes)
                    {
                        switch (xx.Name.ToLower())
                        {
                            case "description":
                                description = xx.Value;
                                break;
                            case "code":
                                code = xx.Value;
                                break;
                        }
                    }
                }

                error += "code (" + code + ") description: " + description;
                if (protocol.SystemWindow == null)
                {
                    Core.SystemForm.main.scrollback.InsertText(error, Pidgeon.ContentLine.MessageStyle.User);
                    return;
                }
                protocol.SystemWindow.scrollback.InsertText(error, Pidgeon.ContentLine.MessageStyle.User);
            }

            /// <summary>
            /// This is a parser for command which is received when services send us a list of networks
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sNetworkList(XmlNode curr, ProtocolSv protocol)
            {
                if (!string.IsNullOrEmpty(curr.InnerText))
                {
                    string[] _networks = curr.InnerText.Split('|');
                    string[] mq = null;
                    if (Configuration.Services.UsingCache)
                    {
                        if (curr.Attributes.Count > 0)
                        {
                            mq = curr.Attributes[0].Value.Split('|');
                            if (_networks.Length != mq.Length)
                            {
                                Core.DebugLog("Invalid buffer " + curr.Attributes[0].Value);
                                mq = null;
                            }
                        }
                    }

                    if (Configuration.Services.UsingCache)
                    {
                        Core.SystemForm.Status("Loading disk cache from disk...");
                        protocol.sBuffer.ReadDisk();
                    }

                    int id = 0;
                    foreach (string i in _networks)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            protocol.Deliver(new Datagram("NETWORKINFO", i));
                            Network nw = new Network(i, protocol);
                            nw.Nickname = protocol.nick;
                            protocol.cache.Add(new Cache());
                            protocol.NetworkList.Add(nw);
                            // we flag the network as connected until we really know that
                            nw.IsConnected = true;
                            // we ask for information about every channel on that network
                            Datagram response = new Datagram("CHANNELINFO");
                            response._InnerText = "LIST";
                            response.Parameters.Add("network", i);
                            protocol.Deliver(response);
                            Datagram request = new Datagram("BACKLOGSV");
                            request.Parameters.Add("network", i);
                            request.Parameters.Add("size", Configuration.Services.Depth.ToString());
                            // we insert this network to a list of waiting networks
                            lock (protocol.WaitingNetw)
                            {
                                protocol.WaitingNetw.Add(i);
                            }
                            // we need to recover data from local storage
                            if (Configuration.Services.UsingCache && mq != null)
                            {
                                if (!protocol.sBuffer.Networks.ContainsValue(mq[id]))
                                {
                                    protocol.sBuffer.Make(i, mq[id]);
                                    protocol.sBuffer.networkInfo[mq[id]].NetworkID = mq[id];
                                    protocol.sBuffer.networkInfo[mq[id]].Nick = nw.Nickname;
                                    protocol.sBuffer.networkInfo[mq[id]].Server = nw.ServerName;
                                }
                                else
                                {
                                    Services.Buffer.NetworkInfo info = protocol.sBuffer.networkInfo[mq[id]];
                                    info.recoverWindowText(nw.SystemWindow, nw.SystemWindow.WindowName);
                                    nw.CModes = info.CModes;
                                    nw.CUModes = info.CUModes;
                                    nw.PModes = info.PModes;
                                    nw.SModes = info.SModes;
                                    nw.UChars = info.UChars;
                                    nw.UModes = info.UModes;
                                    nw.XModes = info.XModes;
                                    nw.Nickname = info.Nick;
                                    nw.IrcdVersion = info.Version;
                                    nw.ChannelList.Clear();
                                    nw.ChannelList.AddRange(info.ChannelList);
                                    info.ChannelList.Clear();
                                    lock (nw.Descriptions)
                                    {
                                        nw.Descriptions.Clear();
                                        foreach (Pidgeon.Protocols.Services.Buffer.NetworkInfo.Description description in info.Descriptions)
                                        {
                                            nw.Descriptions.Add(description.Char, description.String);
                                        }
                                    }
                                    foreach (string ms in info.PrivateWins)
                                    {
                                        info.recoverWindowText(nw.GetPrivateUserWindow(ms), ms);
                                    }
                                    int mqid = info.lastMQID;
                                    request.Parameters.Add("last", mqid.ToString());
                                }
                            }
                            protocol.Deliver(request);
                        }
                        id++;
                    }
                    Core.SystemForm.Status("Waiting for services to send us remaining backlog");
                }
            }

            /// <summary>
            /// Remove server
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sRemove(XmlNode curr, ProtocolSv protocol)
            {
                protocol.RemoveNetworkFromMemory(curr.InnerText);
            }

            /// <summary>
            /// Channel info
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sChannelInfo(XmlNode curr, ProtocolSv protocol)
            {
                if (string.IsNullOrEmpty(curr.InnerText))
                {
                    if (curr.Attributes.Count > 1)
                    {
                        if (curr.Attributes[1].Name == "channels")
                        {
                            string[] channellist = curr.Attributes[1].Value.Split('!');
                            Network nw = protocol.retrieveNetwork(curr.Attributes[0].Value);
                            if (nw != null)
                            {
                                foreach (string channel in channellist)
                                {
                                    if (!string.IsNullOrEmpty(channel))
                                    {
                                        if (nw.GetChannel(channel) == null)
                                        {
                                            Channel xx = nw.MakeChannel(channel);
                                            if (Configuration.Services.UsingCache)
                                            {
                                                string ID = protocol.sBuffer.getUID(curr.Attributes[0].Value);
                                                if (ID != null && protocol.sBuffer.networkInfo.ContainsKey(ID))
                                                {
                                                    Graphics.Window window = xx.RetrieveWindow();
                                                    if (xx != null)
                                                    {
                                                        protocol.sBuffer.networkInfo[ID].recoverWindowText(window, window.WindowName);
                                                    }
                                                    Services.Buffer.ChannelInfo channel_info = protocol.sBuffer.networkInfo[ID].getChannel(channel);
                                                    if (channel_info != null)
                                                    {
                                                        xx.Bans = channel_info.Bans;
                                                        xx.Exceptions = channel_info.Exceptions;
                                                        xx.ChannelWork = channel_info.ChannelWork;
                                                        xx.Invites = channel_info.Invites;
                                                        xx.Name = channel_info.Name;
                                                        xx.IsParsingBanData = channel_info.parsing_bans;
                                                        xx.IsParsingWhoisData = channel_info.parsing_wh;
                                                        xx.IsParsingWhoData = channel_info.parsing_who;
                                                        xx.ChannelMode = new libirc.NetworkMode(channel_info.mode);
                                                        xx.IsParsingExceptionData = channel_info.parsing_xe;
                                                        xx.Redraw = channel_info.Redraw;
                                                        xx.TemporarilyHidden = channel_info.temporary_hide;
                                                        xx.Topic = channel_info.Topic;
                                                        xx.TopicDate = channel_info.TopicDate;
                                                        xx.TopicUser = channel_info.TopicUser;
                                                    }
                                                }
                                            }
                                        }
                                        if (!Configuration.Services.Retrieve_Sv)
                                        {
                                            protocol.SendData(nw.ServerName, "TOPIC " + channel, libirc.Defs.Priority.Normal);
                                            protocol.SendData(nw.ServerName, "MODE " + channel, libirc.Defs.Priority.Low);
                                        }
                                        Datagram response2 = new Datagram("CHANNELINFO", "INFO");
                                        response2.Parameters.Add("network", curr.Attributes[0].Value);
                                        response2.Parameters.Add("channel", channel);
                                        lock (protocol.RemainingJobs)
                                        {
                                            protocol.RemainingJobs.Add(new ProtocolSv.Request(channel, ProtocolSv.Request.Type.ChannelInfo));
                                        }
                                        protocol.Deliver(response2);
                                    }
                                }
                                System.Threading.Thread.Sleep(800);
                            }
                        }
                    }
                    return;
                }
                if (curr.Attributes[1].Name == "channel")
                {
                    string[] userlist = curr.Attributes[2].Value.Split(':');
                    Network nw = protocol.retrieveNetwork(curr.Attributes[0].Value);
                    if (nw != null)
                    {
                        lock (protocol.RemainingJobs)
                        {
                            ProtocolSv.Request item = null;
                            foreach (ProtocolSv.Request work in protocol.RemainingJobs)
                            {
                                if (work.type == ProtocolSv.Request.Type.ChannelInfo && curr.Attributes[1].Value == work.Name)
                                {
                                    item = work;
                                }
                            }

                            if (item != null)
                            {
                                protocol.RemainingJobs.Remove(item);
                                Core.SystemForm.Status(protocol.getInfo());
                            }
                        }
                        Channel channel = nw.GetChannel(curr.Attributes[1].Value);

                        if (channel != null)
                        {
                            foreach (string user in userlist)
                            {
                                if (user.Contains("!") && user.Contains("@"))
                                {
                                    string us = "";
                                    string ident;
                                    us = user.Substring(0, user.IndexOf("!", StringComparison.Ordinal));
                                    if (channel.ContainsUser(us))
                                    {
                                        continue;
                                    }
                                    ident = user.Substring(user.IndexOf("!", StringComparison.Ordinal) + 1);
                                    if (ident.StartsWith("@", StringComparison.Ordinal))
                                    {
                                        ident = "";
                                    }
                                    else
                                    {
                                        if (ident.Contains("@"))
                                        {
                                            ident = ident.Substring(0, ident.IndexOf("@", StringComparison.Ordinal));
                                        }
                                    }
                                    string host = user.Substring(user.IndexOf("@", StringComparison.Ordinal) + 1);
                                    if (host.StartsWith("+", StringComparison.Ordinal))
                                    {
                                        host = "";
                                    }
                                    else
                                    {
                                        if (host.Contains("+"))
                                        {
                                            host = host.Substring(0, host.IndexOf("+", StringComparison.Ordinal));
                                        }
                                    }
                                    /*lock (channel.UserList)
                                    {
                                        if (!channel.ContainsUser(us))
                                        {
                                            User f2 = new User(us, host, nw, ident);
                                            if (user.Contains("+") && !user.StartsWith("+", StringComparison.Ordinal))
                                            {
                                                f2.ChannelMode.ChangeMode(user.Substring(user.IndexOf("+", StringComparison.Ordinal)));
                                                f2.ResetMode();
                                            }
                                            channel.UserList.Add(f2.Nick.ToLower(), f2);
                                        }
                                    } */
                                }
                            }
                            Datagram response = new Datagram("USERLIST", "INFO");
                            response.Parameters.Add("network", curr.Attributes[0].Value);
                            response.Parameters.Add("channel", channel.Name);
                            protocol.Deliver(response);
                            channel.RedrawUsers();
                            channel.UpdateInfo();
                        }
                    }
                }
            }

            /// <summary>
            /// User list including all various information about user
            /// </summary>
            /// <param name="curr"></param>
            /// <param name="protocol"></param>
            public static void sUserList(XmlNode curr, ProtocolSv protocol)
            {
                Dictionary<string, string> userlist = Core.XmlCollectionToDict(curr.Attributes);
                if (!userlist.ContainsKey("network"))
                {
                    Core.DebugLog("Invalid xml:" + curr.InnerXml);
                    return;
                }
                Network nw = protocol.retrieveNetwork(userlist["network"]);

                if (nw == null)
                {
                    Core.DebugLog("Invalid network " + curr.Attributes["network"].Value);
                    return;
                }
                
                if (!userlist.ContainsKey("channel"))
                {
                    Core.DebugLog("Invalid xml:" + curr.InnerXml);
                    return;
                }

                Channel channel = nw.GetChannel(userlist["channel"]);

                int UserCount = int.Parse(userlist["uc"]);
                int CurrentUser = 0;

                while (CurrentUser < UserCount)
                {
                    if (!userlist.ContainsKey("nickname" + CurrentUser.ToString()))
                    {
                        Core.DebugLog("Invalid xml:" + curr.InnerXml);
                        return;
                    }
                    User user = channel.UserFromName(userlist["nickname" + CurrentUser.ToString()]);

                    if (user == null)
                    {
                        user = new User(userlist["nickname" + CurrentUser.ToString()], nw);
                        channel.InsertUser(user);
                        CurrentUser++;
                        continue;
                    }

                    if (userlist.ContainsKey("away" + CurrentUser.ToString()))
                    {
                        user.Away = bool.Parse(userlist["away" + CurrentUser.ToString()]);
                    }

                    if (userlist.ContainsKey("realname" + CurrentUser.ToString()))
                    {
                        user.RealName = userlist["realname" + CurrentUser.ToString()];
                    }
                    CurrentUser++;
                }

                channel.RedrawUsers();
            }

            /// <summary>
            /// Auth
            /// </summary>
            /// <param name="curr">XmlNode</param>
            /// <param name="protocol">Protocol which owns this request</param>
            public static void sAuth(XmlNode curr, ProtocolSv protocol)
            {
                if (curr.InnerText == "INVALID")
                {
                    protocol.SystemWindow.scrollback.InsertText("You have supplied wrong password, connection closed",
                        Pidgeon.ContentLine.MessageStyle.System, false);
                    protocol.Disconnect();
                }
                if (curr.InnerText == "OK")
                {
                    protocol.ConnectionStatus = Status.Connected;
                    protocol.SystemWindow.scrollback.InsertText("You are now logged in to pidgeon bnc", Pidgeon.ContentLine.MessageStyle.System, false);
                    protocol.SystemWindow.scrollback.InsertText(curr.Attributes[0].Value, Pidgeon.ContentLine.MessageStyle.System);
                }
                return;
            }
        }
    }
}
