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
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public partial class ProtocolSv
    {
        class ResponsesSv
        {
            public static void sMessage(XmlNode curr, ProtocolSv protocol)
            {
                string message_nick = curr.Attributes[0].Value;
                string message_text = curr.InnerText;
                string message_target = curr.Attributes[3].Value;
                Window message_window = null;
                Network mn = protocol.retrieveNetwork(curr.Attributes[1].Value);
                long message_time = long.Parse(curr.Attributes[2].Value);
                if (mn != null)
                {
                    if (message_target.StartsWith(mn.channel_prefix))
                    {
                        Channel target = mn.getChannel(message_target);
                        if (target != null)
                        {
                            message_window = target.retrieveWindow();
                        }
                        else
                        {
                            Core.DebugLog("There is no channel " + message_target);
                        }
                    }
                    else
                    {
                        lock (protocol.windows)
                        {
                            if (!protocol.windows.ContainsKey(mn.window + message_target))
                            {
                                mn.Private(message_target);
                            }
                            message_window = protocol.windows[mn.window + message_target];
                        }
                    }
                }


                if (message_window != null)
                {
                    message_window.scrollback.InsertTextAndIgnoreUpdate(mn._protocol.PRIVMSG(message_nick, message_text), Scrollback.MessageStyle.Message, true, message_time, true);
                }
                else
                {
                    Core.DebugLog("There is no window for " + message_target);
                }
            }

            public static void sRaw(XmlNode curr, ProtocolSv protocol)
            {
                if (curr.InnerText == "PERMISSIONDENY")
                {
                    protocol.windows["!root"].scrollback.InsertText("You can't send this command to server, because you aren't logged in",
                        Scrollback.MessageStyle.System, false);
                    return;
                }
                protocol.windows["!root"].scrollback.InsertText("Server responded to SRAW with this: " + curr.InnerText,
                    Scrollback.MessageStyle.User, false);
            }

            public static void sLoad(XmlNode curr, ProtocolSv protocol)
            {
                protocol.windows["!root"].scrollback.InsertText(curr.InnerText, Scrollback.MessageStyle.System, false);
            }

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

            public static void sData(XmlNode curr, ProtocolSv protocol)
            {
                long date = 0;
                bool backlog = false;
                string id = "";
                foreach (XmlAttribute time in curr.Attributes)
                {
                    if (time.Name == "time")
                    {
                        if (!long.TryParse(time.Value, out date))
                        {
                            Core.DebugLog("Warning: " + time.Value + " is not a correct time");
                        }
                    }
                }
                foreach (XmlAttribute i in curr.Attributes)
                {
                    if (i.Name == "buffer")
                    {
                        id = i.Value;
                        backlog = true;
                    }
                }
                string name = curr.Attributes[0].Value;
                Network server4 = null;
                server4 = protocol.retrieveNetwork(name);
                if (server4 == null)
                {
                    server4 = new Network(name, protocol);
                    protocol.NetworkList.Add(server4);
                    protocol.cache.Add(new Cache());
                    server4.Nickname = protocol.nick;
                    server4.Connected = true;
                }
                if (backlog)
                {
                    if (Core._Main.DisplayingProgress == false)
                    {
                        Core._Main.progress = int.Parse(id);
                        Core._Main.DisplayingProgress = true;
                        protocol.SuppressChanges = true;
                        Core._Main.ProgressMax = protocol.cache[protocol.NetworkList.IndexOf(server4)].size;
                    }

                    Core._Main.progress = int.Parse(id);
                    Core._Main.Status("Retrieving backlog from " + name + ", got " + id + " packets from total of " + protocol.cache[protocol.NetworkList.IndexOf(server4)].size.ToString() + " datagrams");
                    if ((protocol.cache[protocol.NetworkList.IndexOf(server4)].size - 2) < int.Parse(id))
                    {
                        Core._Main.Status("");
                        Core._Main.DisplayingProgress = false;
                        Core._Main.progress = 0;
                        protocol.SuppressChanges = false;
                        foreach (Channel i in server4.Channels)
                        {
                            i.temporary_hide = false;
                            i.parsing_xe = false;
                            i.parsing_xb = false;
                            i.parsing_who = false;
                        }
                    }
                    ProcessorIRC processor = new ProcessorIRC(server4, curr.InnerText, ref protocol.pong, date, false);
                    processor.Result();
                    return;
                }
                ProcessorIRC processor2 = new ProcessorIRC(server4, curr.InnerText, ref protocol.pong, date);
                processor2.Result();
            }
            
            public static void sNick(XmlNode curr, ProtocolSv protocol)
            {
                Network sv = protocol.retrieveNetwork(curr.Attributes[0].Value);
                if (sv != null)
                {
                    sv.Nickname = curr.InnerText;
                    protocol.windows["!" + sv.window].scrollback.InsertText("Your nick was changed to " + curr.InnerText,
                        Scrollback.MessageStyle.User, true);
                }
            }

            public static void sConnect(XmlNode curr, ProtocolSv protocol)
            {
                string network = curr.Attributes[0].Value;
                switch (curr.InnerText)
                {
                    case "CONNECTED":
                        protocol.windows["!root"].scrollback.InsertText("You are already connected to " + network, Scrollback.MessageStyle.System);
                        return;
                    case "PROBLEM":
                        protocol.windows["!root"].scrollback.InsertText(messages.get("service_error", Core.SelectedLanguage, new List<string> { network, curr.Attributes[1].Value }), Scrollback.MessageStyle.System, false);
                        return;
                    case "OK":
                        Network _network = new Network(network, protocol);
                        _network.Connected = true;
                        _network.Nickname = protocol.nick;
                        protocol.cache.Add(new Cache());
                        protocol.NetworkList.Add(_network);
                        return;
                }
            }

            public static void sGlobalident(XmlNode curr, ProtocolSv protocol)
            {
                protocol.windows["!root"].scrollback.InsertText(messages.get("pidgeon.globalident", Core.SelectedLanguage,
                                new List<string> { curr.InnerText }), Scrollback.MessageStyle.User, true);
            }

            public static void sBacklog(XmlNode curr, ProtocolSv protocol)
            {
                string network = curr.Attributes[0].Value;
                Network server = protocol.retrieveNetwork(network);
                if (server != null)
                {
                    protocol.cache[protocol.NetworkList.IndexOf(server)].size = int.Parse(curr.InnerText);
                    foreach (Channel i in server.Channels)
                    {
                        i.parsing_who = true;
                        i.parsing_xb = true;
                        i.temporary_hide = true;
                    }
                }
            }

            public static void sGlobalNick(XmlNode curr, ProtocolSv protocol)
            {
                protocol.nick = curr.InnerText;
                protocol.windows["!root"].scrollback.InsertText(messages.get("pidgeon.globalnick", Core.SelectedLanguage,
                    new List<string> { curr.InnerText }), Scrollback.MessageStyle.User, true);
            }

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
                foreach (Network s2 in protocol.NetworkList)
                {
                    if (curr.Attributes[0].Value == s2.server)
                    {
                        s2.Connected = connected;
                        break;
                    }
                }
            }

            public static void sNetworkList(XmlNode curr, ProtocolSv protocol)
            {
                if (curr.InnerText != "")
                {
                    string[] _networks = curr.InnerText.Split('|');
                    foreach (string i in _networks)
                    {
                        if (i != "")
                        {
                            protocol.Deliver(new Datagram("NETWORKINFO", i));
                            Network nw = new Network(i, protocol);
                            nw.Nickname = protocol.nick;
                            protocol.cache.Add(new Cache());
                            protocol.NetworkList.Add(nw);
                            Datagram response = new Datagram("CHANNELINFO");
                            response._InnerText = "LIST";
                            response.Parameters.Add("network", i);
                            protocol.Deliver(response);
                            Datagram request = new Datagram("BACKLOGSV");
                            request.Parameters.Add("network", i);
                            request.Parameters.Add("size", Configuration.Services.Depth.ToString());
                            protocol.Deliver(request);
                        }
                    }
                }
            }

            public static void sRemove(XmlNode curr, ProtocolSv protocol)
            {
                System.Windows.Forms.TreeNode item = null;
                foreach (KeyValuePair<Network, System.Windows.Forms.TreeNode> n in Core._Main.ChannelList.Servers)
                {
                    if (n.Key.server == curr.InnerText)
                    {
                        item = n.Value;
                        break;
                    }
                }
                if (item != null)
                {
                    Core._Main.ChannelList.RemoveAll(item);
                }
                else
                {
                    Core.DebugLog("Unable to remove " + curr.InnerText);
                }
            }

            public static void sChannelInfo(XmlNode curr, ProtocolSv protocol)
            {
                if (curr.InnerText == "")
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
                                    if (channel != "")
                                    {
                                        if (nw.getChannel(channel) == null)
                                        {
                                            nw.WindowCreateNewJoin(channel, true);
                                        }
                                        if (!Configuration.Services.Retrieve_Sv)
                                        {
                                            protocol.SendData(nw.server, "TOPIC " + channel, Configuration.Priority.Normal);
                                            protocol.SendData(nw.server, "MODE " + channel, Configuration.Priority.Low);
                                        }
                                        Datagram response2 = new Datagram("CHANNELINFO", "INFO");
                                        response2.Parameters.Add("network", curr.Attributes[0].Value);
                                        response2.Parameters.Add("channel", channel);
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
                        Channel channel = nw.getChannel(curr.Attributes[1].Value);

                        if (channel != null)
                        {
                            foreach (string user in userlist)
                            {

                                if (user.Contains("!") && user.Contains("@"))
                                {
                                    string us = "";
                                    string ident;
                                    us = user.Substring(0, user.IndexOf("!"));
                                    if (channel.containsUser(us))
                                    {
                                        continue;
                                    }
                                    ident = user.Substring(user.IndexOf("!") + 1);
                                    if (ident.StartsWith("@"))
                                    {
                                        ident = "";
                                    }
                                    else
                                    {
                                        if (ident.Contains("@"))
                                        {
                                            ident = ident.Substring(0, ident.IndexOf("@"));
                                        }
                                    }
                                    string host = user.Substring(user.IndexOf("@") + 1);
                                    if (host.StartsWith("+"))
                                    {
                                        host = "";
                                    }
                                    else
                                    {
                                        if (host.Contains("+"))
                                        {
                                            host = host.Substring(0, host.IndexOf("+"));
                                        }
                                    }
                                    User f2 = new User(us, host, nw, ident);
                                    if (user.Contains("+") && !user.StartsWith("+"))
                                    {
                                        f2.ChannelMode.mode(user.Substring(user.IndexOf("+")));
                                    }
                                    channel.UserList.Add(f2);
                                }

                            }
                            channel.redrawUsers();
                            channel.UpdateInfo();
                        }
                    }
                }
            }

            public static void sAuth(XmlNode curr, ProtocolSv protocol)
            {
                if (curr.InnerText == "INVALID")
                {
                    protocol.windows["!root"].scrollback.InsertText("You have supplied wrong password, connection closed",
                        Scrollback.MessageStyle.System, false);
                    protocol.Connected = false;
                    protocol.Exit();
                }
                if (curr.InnerText == "OK")
                {
                    protocol.ConnectionStatus = Status.Connected;
                    protocol.windows["!root"].scrollback.InsertText("You are now logged in to pidgeon bnc", Scrollback.MessageStyle.System, false);
                    protocol.windows["!root"].scrollback.InsertText(curr.Attributes[0].Value, Scrollback.MessageStyle.System);
                }
                return;
            }
        }
    }
}
