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

using System.IO;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Client
{
    public partial class Core
    {
        public class _Configuration
        {
            private static void make_comment(string text, XmlDocument conf, XmlNode node)
            {
                XmlComment xx = conf.CreateComment(text);
                node.AppendChild(xx);
            }

            /// <summary>
            /// Create a node
            /// </summary>
            /// <param name="config_key">Key</param>
            /// <param name="text">Text</param>
            /// <param name="xmlnode">Node</param>
            /// <param name="key">Key</param>
            /// <param name="_c">Config</param>
            /// <param name="conf">Node</param>
            /// <param name="nn">Config name</param>
            /// <returns></returns>
            private static bool make_node(string config_key, string text, XmlNode xmlnode, XmlAttribute key, XmlDocument _c, XmlNode conf, string nn = "confname")
            {
                key = _c.CreateAttribute(nn);
                xmlnode = _c.CreateElement("data");
                key.Value = config_key;
                xmlnode.Attributes.Append(key);
                xmlnode.InnerText = text;
                conf.AppendChild(xmlnode);
                return true;
            }

            public static bool ConfigSave()
            {
                try
                {
                    System.Xml.XmlDocument config = new System.Xml.XmlDocument();
                    XmlComment notice = config.CreateComment("This is a configuration file of pidgeon client, see http://pidgeonclient.org/wiki/Help:Configuration");
                    config.AppendChild(notice);
                    System.Xml.XmlNode xmlnode = config.CreateElement("configuration.pidgeon");
                    System.Xml.XmlNode curr = null;
                    XmlAttribute confname = null;
                    // Network
                    make_comment(" ============= NETWORK ============= ", config, xmlnode);
                    make_comment("Ident", config, xmlnode);
                    make_node("network.ident", Configuration.UserData.ident, curr, confname, config, xmlnode);
                    make_comment("Message that is displayed when you quit the network", config, xmlnode);
                    make_node("quitmessage", Configuration.UserData.quit, curr, confname, config, xmlnode);
                    make_comment("Nickname which is being used on all networks", config, xmlnode);
                    make_node("network.nick", Configuration.UserData.nick, curr, confname, config, xmlnode);
                    // Scrollback
                    make_comment(" ============= SCROLLBACK ============= ", config, xmlnode);
                    make_comment("If this is true, you will see when someone tries to CTCP you", config, xmlnode);
                    make_node("scrollback.showctcp", Configuration.irc.DisplayCtcp.ToString(), curr, confname, config, xmlnode);
                    make_comment("Format of user in window", config, xmlnode);
                    make_node("formats.user", Configuration.Scrollback.format_nick, curr, confname, config, xmlnode);
                    make_comment("Format of date in window", config, xmlnode);
                    make_node("formats.date", Configuration.Scrollback.format_date, curr, confname, config, xmlnode);
                    make_comment("Set this to true to automatically whois everyone in channel upon join", config, xmlnode);
                    make_comment("Limit of scrollback size", config, xmlnode);
                    make_node("scrollback_plimit", Configuration.Scrollback.scrollback_plimit.ToString(), curr, confname, config, xmlnode);
                    // irc
                    make_comment(" ============= IRC ============= ", config, xmlnode);
                    make_node("irc.auto.whois", Configuration.ChannelModes.aggressive_whois.ToString(), curr, confname, config, xmlnode);
                    make_comment("Set this to true to check the channel modes on join", config, xmlnode);
                    make_node("irc.auto.mode", Configuration.ChannelModes.aggressive_mode.ToString(), curr, confname, config, xmlnode);
                    make_node("irc.auto.channels", Configuration.ChannelModes.aggressive_channel.ToString(), curr, confname, config, xmlnode);
                    make_comment("Set this to true to get a list of bans on join", config, xmlnode);
                    make_node("irc.auto.bans", Configuration.ChannelModes.aggressive_bans.ToString(), curr, confname, config, xmlnode);
                    make_comment("Set this to true to get a list of exception on join", config, xmlnode);
                    make_node("irc.auto.exception", Configuration.ChannelModes.aggressive_exception.ToString(), curr, confname, config, xmlnode);
                    make_comment("Set this to true to get a list of invite on join", config, xmlnode);
                    make_node("irc.auto.invites", Configuration.ChannelModes.aggressive_invites.ToString(), curr, confname, config, xmlnode);
                    make_comment("If CTCP should be completely ignored", config, xmlnode);
                    make_node("ignore.ctcp", Configuration.irc.DisplayCtcp.ToString(), curr, confname, config, xmlnode);
                    make_comment("Whether all generated commands need to be confirmed", config, xmlnode);
                    make_node("confirm.all", Configuration.irc.ConfirmAll.ToString(), curr, confname, config, xmlnode);
                    make_comment("Reason for kick / ban", config, xmlnode);
                    make_node("default.reason", Configuration.irc.DefaultReason, curr, confname, config, xmlnode);
                    make_comment("Second nick", config, xmlnode);
                    make_node("network.n2", Configuration.UserData.Nick2, curr, confname, config, xmlnode);
                    // Windows
                    make_comment(" ============= WINDOWS ============= ", config, xmlnode);
                    make_comment("Main window is maximized", config, xmlnode);
                    make_node("location.maxi", Configuration.Window.Window_Maximized.ToString(), curr, confname, config, xmlnode);
                    make_comment("Size of main win", config, xmlnode);
                    make_node("window.size", Configuration.Window.window_size.ToString(), curr, confname, config, xmlnode);
                    make_comment("Location of slide bar", config, xmlnode);
                    make_node("location.x1", Configuration.Window.x1.ToString(), curr, confname, config, xmlnode);
                    make_comment("Location of slide bar", config, xmlnode);
                    make_node("location.x4", Configuration.Window.x4.ToString(), curr, confname, config, xmlnode);
                    make_comment("Maximum history", config, xmlnode);
                    make_node("Configuration.Window.history", Configuration.Window.history.ToString(), curr, confname, config, xmlnode);
                    // Logs
                    make_comment(" ============= LOGS ============= ", config, xmlnode);
                    make_comment("Where the logs are being saved", config, xmlnode);
                    make_node("logs.dir", Configuration.Logs.logs_dir, curr, confname, config, xmlnode);
                    make_comment("Type of logs to save", config, xmlnode);
                    make_node("logs.type", Configuration.Logs.logs_name, curr, confname, config, xmlnode);
                    make_comment("If you want to save logs in html", config, xmlnode);
                    make_node("logs.html", Configuration.Logs.logs_html.ToString(), curr, confname, config, xmlnode);
                    make_comment("If you want to save logs in XML", config, xmlnode);
                    make_node("logs.xml", Configuration.Logs.logs_xml.ToString(), curr, confname, config, xmlnode);
                    make_comment("If you want to save logs in TXT", config, xmlnode);
                    make_node("logs.txt", Configuration.Logs.logs_txt.ToString(), curr, confname, config, xmlnode);
                    make_node("logs.services.log.type", Configuration.Logs.ServicesLogs.ToString(), curr, confname, config, xmlnode);
                    // System
                    make_comment(" ============= SYSTEM ============= ", config, xmlnode);
                    make_comment("Check for updates (recommended)", config, xmlnode);
                    make_node("updater.check", Configuration.Kernel.CheckUpdate.ToString(), curr, confname, config, xmlnode);
                    make_comment("If pidgeon is running in debug mode", config, xmlnode);
                    make_node("debugger", Configuration.Kernel.Debugging.ToString(), curr, confname, config, xmlnode);
                    make_comment("If notification are displayed", config, xmlnode);
                    make_node("notification.tray", Configuration.Kernel.Notice.ToString(), curr, confname, config, xmlnode);
                    make_comment("Network sniffer", config, xmlnode);
                    make_node("sniffer", Configuration.Kernel.NetworkSniff.ToString(), curr, confname, config, xmlnode);
                    make_comment("Skin", config, xmlnode);
                    make_node("skin", Configuration.CurrentSkin.name, curr, confname, config, xmlnode);
                    make_comment("Whether color codes override the system color for links", config, xmlnode);
                    make_node("colors.changelinks", Configuration.Colors.ChangeLinks.ToString(), curr, confname, config, xmlnode);
                    // Services
                    make_comment(" ============= SERVICES ============= ", config, xmlnode);
                    make_comment("Size of backlog for services", config, xmlnode);
                    make_node("pidgeon.size", Configuration.Services.Depth.ToString(), curr, confname, config, xmlnode);
                    make_comment("If services are using local cache (incomparably faster)", config, xmlnode);
                    make_node("services.usingcache", Configuration.Services.UsingCache.ToString(), curr, confname, config, xmlnode);
                    // Histories
                    make_comment(" ============= USER ============= ", config, xmlnode);
                    make_comment("Last nickname you used", config, xmlnode);
                    make_node("history.nick", Configuration.UserData.LastNick, curr, confname, config, xmlnode);
                    make_comment("Last host you used", config, xmlnode);
                    make_node("history.host", Configuration.UserData.LastHost, curr, confname, config, xmlnode);
                    make_comment("Last host you connected to", config, xmlnode);
                    make_node("history.port", Configuration.UserData.LastPort, curr, confname, config, xmlnode);
                    make_node("message_mq", Configuration.irc.mq.ToString(), curr, confname, config, xmlnode);
                    make_comment("Change this to false in order to disable clickable browser links", config, xmlnode);
                    make_node("userdata.openlinkinbrowser", Configuration.UserData.OpenLinkInBrowser.ToString(), curr, confname, config, xmlnode);


                    foreach (KeyValuePair<string, string> data in Configuration.Extensions)
                    {
                        make_node("extension." + data.Key, data.Value, curr, confname, config, xmlnode);
                    }

                    string separators = "";
                    foreach (char separator in Configuration.Parser.Separators)
                    {
                        separators = separators + separator.ToString();
                    }
                    make_node("delimiters", separators, curr, confname, config, xmlnode);

                    lock (Configuration.Parser.Protocols)
                    {
                        foreach (string xx in Configuration.Parser.Protocols)
                        {
                            curr = config.CreateElement("protocol");
                            curr.InnerText = xx;
                            xmlnode.AppendChild(curr);
                        }
                    }

                    lock (NetworkData.Networks)
                    {
                        foreach (NetworkData.NetworkInfo ni in NetworkData.Networks)
                        {
                            curr = config.CreateElement("network");
                            XmlAttribute name = config.CreateAttribute("name");
                            XmlAttribute server = config.CreateAttribute("server");
                            XmlAttribute ssl = config.CreateAttribute("ssl");
                            XmlAttribute type = config.CreateAttribute("type");
                            type.Value = ni.protocolType.ToString();
                            name.Value = ni.Name;
                            server.Value = ni.Server;
                            ssl.Value = ni.SSL.ToString();
                            curr.Attributes.Append(name);
                            curr.Attributes.Append(server);
                            curr.Attributes.Append(type);
                            curr.Attributes.Append(ssl);
                            xmlnode.AppendChild(curr);
                        }
                    }
                    config.AppendChild(xmlnode);

                    lock (Configuration.HighlighterList)
                    {
                        foreach (Network.Highlighter high in Configuration.HighlighterList)
                        {
                            curr = config.CreateElement("list");
                            XmlAttribute highlightenabled = config.CreateAttribute("enabled");
                            XmlAttribute highlighttext = config.CreateAttribute("text");
                            XmlAttribute highlightsimple = config.CreateAttribute("simple");
                            highlightenabled.Value = "true";
                            highlightsimple.Value = high.simple.ToString();
                            highlighttext.Value = high.text;
                            curr.Attributes.Append(highlightsimple);
                            curr.Attributes.Append(highlighttext);
                            curr.Attributes.Append(highlightenabled);
                            xmlnode.AppendChild(curr);
                        }
                    }
                    config.AppendChild(xmlnode);

                    lock (Ignoring.IgnoreList)
                    {
                        foreach (Ignoring.Ignore ci in Ignoring.IgnoreList)
                        {
                            curr = config.CreateElement("ignore");
                            XmlAttribute enabled = config.CreateAttribute("enabled");
                            XmlAttribute simple = config.CreateAttribute("simple");
                            XmlAttribute text = config.CreateAttribute("text");
                            XmlAttribute type = config.CreateAttribute("type");
                            enabled.Value = ci.Enabled.ToString();
                            simple.Value = ci.Simple.ToString();
                            text.Value = ci.Text;
                            type.Value = ci.type.ToString();
                            curr.Attributes.Append(enabled);
                            curr.Attributes.Append(simple);
                            curr.Attributes.Append(text);
                            curr.Attributes.Append(type);
                            xmlnode.AppendChild(curr);
                        }
                    }
                    config.AppendChild(xmlnode);

                    lock (Configuration.ShortcutKeylist)
                    {
                        foreach (Shortcut RR in Configuration.ShortcutKeylist)
                        {
                            curr = config.CreateElement("shortcut");
                            XmlAttribute name = config.CreateAttribute("name");
                            XmlAttribute ctrl = config.CreateAttribute("ctrl");
                            XmlAttribute alt = config.CreateAttribute("alt");
                            XmlAttribute shift = config.CreateAttribute("shift");
                            curr.InnerText = RR.data;
                            name.Value = RR.keys.ToString();
                            shift.Value = RR.shift.ToString();
                            ctrl.Value = RR.control.ToString();
                            alt.Value = RR.alt.ToString();
                            curr.Attributes.Append(name);
                            curr.Attributes.Append(ctrl);
                            curr.Attributes.Append(alt);
                            curr.Attributes.Append(shift);
                            xmlnode.AppendChild(curr);
                        }
                    }
                    config.AppendChild(xmlnode);

                    if (Backup(ConfigFile))
                    {
                        config.Save(ConfigFile);
                    }
                    File.Delete(ConfigFile + "~");
                }
                catch (Exception t)
                {
                    Core.handleException(t);
                }
                return false;
            }
        }

        /// <summary>
        /// Conf
        /// </summary>
        /// <returns></returns>
        public static bool ConfigurationLoad()
        {
            // Check if config file is present
            Restore(ConfigFile);
            bool Protocols = false;
            if (File.Exists(ConfigFile))
            {
                try
                {
                    XmlDocument configuration = new XmlDocument();
                    lock (Configuration.ShortcutKeylist)
                    {
                        Configuration.ShortcutKeylist = new List<Shortcut>();
                    }
                    lock (Configuration.HighlighterList)
                    {
                        Configuration.HighlighterList = new List<Network.Highlighter>();
                    }
                    configuration.Load(ConfigFile);
                    lock (NetworkData.Networks)
                    {
                        NetworkData.Networks.Clear();
                    }
                    foreach (XmlNode node in configuration.ChildNodes)
                    {
                        if (node.Name == "configuration.pidgeon")
                        {
                            foreach (XmlNode curr in node.ChildNodes)
                            {
                                if (curr.Attributes == null)
                                {
                                    continue;
                                }
                                if (curr.Attributes.Count > 0)
                                {
                                    if (curr.Name.StartsWith("extension."))
                                    {
                                        Configuration.SetConfig(curr.Name.Substring(10), curr.InnerText);
                                        continue;
                                    }
                                    if (curr.Name == "network")
                                    {
                                        if (curr.Attributes.Count > 3)
                                        {
                                            NetworkData.NetworkInfo info = new NetworkData.NetworkInfo();
                                            foreach (XmlAttribute xx in curr.Attributes)
                                            {
                                                switch (xx.Name.ToLower())
                                                { 
                                                    case "name":
                                                        info.Name = xx.Value;
                                                        break;
                                                    case "server":
                                                        info.Server = xx.Value;
                                                        break;
                                                    case "ssl":
                                                        info.SSL = bool.Parse(xx.Value);
                                                        break;
                                                }
                                            }
                                            lock (NetworkData.Networks)
                                            {
                                                NetworkData.Networks.Add(info);
                                            }
                                        }
                                        continue;
                                    }
                                    if (curr.Name == "ignore")
                                    {
                                        if (curr.Attributes.Count > 3)
                                        {
                                            Ignoring.Ignore.Type _t = Ignoring.Ignore.Type.Everything;
                                            if (curr.Attributes[3].Value == "User")
                                            {
                                                _t = Ignoring.Ignore.Type.User;
                                            }
                                            Ignoring.Ignore ignore = new Ignoring.Ignore(bool.Parse(curr.Attributes[0].Value), bool.Parse(curr.Attributes[1].Value), curr.Attributes[2].Value, _t);
                                            lock (Ignoring.IgnoreList)
                                            {
                                                Ignoring.IgnoreList.Add(ignore);
                                            }
                                        }
                                        continue;
                                    }
                                    if (curr.Name == "list")
                                    {
                                        if (curr.Attributes.Count > 2)
                                        {
                                            Network.Highlighter list = new Network.Highlighter();
                                            list.simple = bool.Parse(curr.Attributes[0].Value);
                                            list.text = curr.Attributes[1].Value;
                                            list.enabled = bool.Parse(curr.Attributes[2].Value);
                                            lock (Configuration.HighlighterList)
                                            {
                                                Configuration.HighlighterList.Add(list);
                                            }
                                        }
                                        continue;
                                    }

                                    if (curr.Name == "protocol")
                                    {
                                        if (!Protocols)
                                        {
                                            Configuration.Parser.Protocols.Clear();
                                            Protocols = true;
                                        }
                                        Configuration.Parser.Protocols.Add(curr.InnerText);
                                        continue;
                                    }

                                    if (curr.Name == "shortcut")
                                    {
                                        if (curr.Attributes.Count > 2)
                                        {
                                            Shortcut list = new Shortcut(parseKey(curr.Attributes[0].Value), bool.Parse(curr.Attributes[1].Value), bool.Parse(curr.Attributes[2].Value), bool.Parse(curr.Attributes[3].Value));
                                            list.data = curr.InnerText;
                                            Configuration.ShortcutKeylist.Add(list);
                                        }
                                        continue;
                                    }
                                    if (curr.Attributes.Count > 0)
                                    {
                                        if (curr.Attributes[0].Name == "confname")
                                        {
                                            switch (curr.Attributes[0].Value)
                                            {
                                                case "location.x1":
                                                    Configuration.Window.x1 = int.Parse(curr.InnerText);
                                                    break;
                                                case "window.size":
                                                    Configuration.Window.window_size = int.Parse(curr.InnerText);
                                                    break;
                                                case "location.x4":
                                                    Configuration.Window.x4 = int.Parse(curr.InnerText);
                                                    break;
                                                case "location.maxi":
                                                    Configuration.Window.Window_Maximized = bool.Parse(curr.InnerText);
                                                    break;
                                                case "timestamp.display":
                                                    Configuration.Scrollback.chat_timestamp = bool.Parse(curr.InnerText);
                                                    break;
                                                case "network.nick":
                                                    Configuration.UserData.nick = curr.InnerText;
                                                    break;
                                                case "network.n2":
                                                    Configuration.UserData.Nick2 = curr.InnerText;
                                                    break;
                                                case "network.ident":
                                                    Configuration.UserData.ident = curr.InnerText;
                                                    break;
                                                case "scrollback.showctcp":
                                                    Configuration.irc.DisplayCtcp = bool.Parse(curr.InnerText);
                                                    break;
                                                case "formats.user":
                                                    Configuration.Scrollback.format_nick = curr.InnerText;
                                                    break;
                                                case "formats.date":
                                                    Configuration.Scrollback.format_date = curr.InnerText;
                                                    break;
                                                case "formats.datetime":
                                                    Configuration.Scrollback.timestamp_mask = curr.InnerText;
                                                    break;
                                                case "irc.auto.whois":
                                                    Configuration.ChannelModes.aggressive_whois = bool.Parse(curr.InnerText);
                                                    break;
                                                case "irc.auto.bans":
                                                    Configuration.ChannelModes.aggressive_bans = bool.Parse(curr.InnerText);
                                                    break;
                                                case "irc.auto.exception":
                                                    Configuration.ChannelModes.aggressive_exception = bool.Parse(curr.InnerText);
                                                    break;
                                                case "irc.auto.channels":
                                                    Configuration.ChannelModes.aggressive_channel = bool.Parse(curr.InnerText);
                                                    break;
                                                case "irc.auto.invites":
                                                    Configuration.ChannelModes.aggressive_invites = bool.Parse(curr.InnerText);
                                                    break;
                                                case "irc.auto.mode":
                                                    Configuration.ChannelModes.aggressive_mode = bool.Parse(curr.InnerText);
                                                    break;
                                                case "network.reason":
                                                    Configuration.irc.DefaultReason = curr.InnerText;
                                                    break;
                                                case "logs.type":
                                                    Configuration.Logs.logs_name = curr.InnerText;
                                                    break;
                                                case "ignore.ctcp":
                                                    Configuration.irc.DisplayCtcp = bool.Parse(curr.InnerText);
                                                    break;
                                                case "logs.html":
                                                    Configuration.Logs.logs_html = bool.Parse(curr.InnerText);
                                                    break;
                                                case "logs.dir":
                                                    Configuration.Logs.logs_dir = curr.InnerText;
                                                    break;
                                                case "logs.xml":
                                                    Configuration.Logs.logs_xml = bool.Parse(curr.InnerText);
                                                    break;
                                                case "logs.txt":
                                                    Configuration.Logs.logs_txt = bool.Parse(curr.InnerText);
                                                    break;
                                                case "scrollback_plimit":
                                                    Configuration.Scrollback.scrollback_plimit = int.Parse(curr.InnerText);
                                                    break;
                                                case "notification.tray":
                                                    Configuration.Kernel.Notice = bool.Parse(curr.InnerText);
                                                    break;
                                                case "pidgeon.size":
                                                    Configuration.Services.Depth = int.Parse(curr.InnerText);
                                                    break;
                                                case "history.nick":
                                                    Configuration.UserData.LastNick = curr.InnerText;
                                                    break;
                                                case "sniffer":
                                                    Configuration.Kernel.NetworkSniff = bool.Parse(curr.InnerText);
                                                    break;
                                                case "history.host":
                                                    Configuration.UserData.LastHost = curr.InnerText;
                                                    break;
                                                case "updater.check":
                                                    Configuration.Kernel.CheckUpdate = bool.Parse(curr.InnerText);
                                                    break;
                                                case "history.port":
                                                    Configuration.UserData.LastPort = curr.InnerText;
                                                    break;
                                                case "delimiters":
                                                    List<char> temp = new List<char>();
                                                    foreach (char part in curr.InnerText)
                                                    {
                                                        temp.Add(part);
                                                    }
                                                    Configuration.Parser.Separators = temp;
                                                    break;
                                                case "colors.changelinks":
                                                    Configuration.Colors.ChangeLinks = bool.Parse(curr.InnerText);
                                                    break;
                                                case "debugger":
                                                    Configuration.Kernel.Debugging = bool.Parse(curr.InnerText);
                                                    break;
                                                case "services.usingcache":
                                                    Configuration.Services.UsingCache = bool.Parse(curr.InnerText);
                                                    break;
                                                case "message_mq":
                                                    Configuration.irc.mq = int.Parse(curr.InnerText);
                                                    break;
                                                case "logs.services.log.type":
                                                    Configuration.Logs.ServiceLogs type = Configuration.Logs.ServiceLogs.none;
                                                    switch (curr.InnerText.ToLower())
                                                    {
                                                        case "incremental":
                                                            type = Configuration.Logs.ServiceLogs.incremental;
                                                            break;
                                                        case "full":
                                                            type = Configuration.Logs.ServiceLogs.full;
                                                            break;
                                                    }
                                                    Configuration.Logs.ServicesLogs = type;
                                                    break;
                                                case "Configuration.Window.history":
                                                    Configuration.Window.history = int.Parse(curr.InnerText);
                                                    break;
                                                case "userdata.openlinkinbrowser":
                                                    Configuration.UserData.OpenLinkInBrowser = bool.Parse(curr.InnerText);
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    handleException(f);
                    return false;
                }
            }
            return true;
        }
    }
}
