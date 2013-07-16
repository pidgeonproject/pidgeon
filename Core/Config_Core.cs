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


// Definitions of functions that handle configuration

using System.IO;
using System.Threading;
using System.Net;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Client
{
    public partial class Core
    {
        /// <summary>
        /// This is controlling the system configuration
        /// </summary>
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

            private static Configuration.PidgeonList_MouseClick from1(string text)
            {
                switch (text)
                {
                    case "Close":
                        return Configuration.PidgeonList_MouseClick.Close;
                    case "Disconnect":
                        return Configuration.PidgeonList_MouseClick.Disconnect;
                }
                return Configuration.PidgeonList_MouseClick.Nothing;
            }

            /// <summary>
            /// Save all configuration to a file
            /// </summary>
            /// <returns></returns>
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
                    make_node("Configuration.UserData.ident", Configuration.UserData.ident, curr, confname, config, xmlnode);
                    make_comment("Message that is displayed when you quit the network", config, xmlnode);
                    make_node("Configuration.UserData.quit", Configuration.UserData.quit, curr, confname, config, xmlnode);
                    make_comment("Nickname which is being used on all networks", config, xmlnode);
                    make_node("Configuration.UserData.nick", Configuration.UserData.nick, curr, confname, config, xmlnode);
                    // Scrollback
                    make_comment(" ============= SCROLLBACK ============= ", config, xmlnode);
                    make_comment("If this is true, you will see when someone tries to CTCP you", config, xmlnode);
                    make_node("Configuration.irc.DisplayCtcp", Configuration.irc.DisplayCtcp.ToString(), curr, confname, config, xmlnode);
                    make_comment("Format of user in window", config, xmlnode);
                    make_node("Configuration.Scrollback.format_nick", Configuration.Scrollback.format_nick, curr, confname, config, xmlnode);
                    make_comment("Format of date in window", config, xmlnode);
                    make_node("Configuration.Scrollback.format_date", Configuration.Scrollback.format_date, curr, confname, config, xmlnode);
                    make_comment("Set this to true to automatically whois everyone in channel upon join", config, xmlnode);
                    make_comment("Limit of scrollback size", config, xmlnode);
                    make_node("Configuration.Scrollback.scrollback_plimit", Configuration.Scrollback.scrollback_plimit.ToString(), curr, confname, config, xmlnode);
                    // irc
                    make_comment(" ============= IRC ============= ", config, xmlnode);
                    make_node("Configuration.ChannelModes.aggressive_whois", Configuration.ChannelModes.aggressive_whois.ToString(), curr, confname, config, xmlnode);
                    make_comment("Set this to true to check the channel modes on join", config, xmlnode);
                    make_node("Configuration.ChannelModes.aggressive_mode", Configuration.ChannelModes.aggressive_mode.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.ChannelModes.aggressive_channel", Configuration.ChannelModes.aggressive_channel.ToString(), curr, confname, config, xmlnode);
                    make_comment("Set this to true to get a list of bans on join", config, xmlnode);
                    make_node("Configuration.ChannelModes.aggressive_bans", Configuration.ChannelModes.aggressive_bans.ToString(), curr, confname, config, xmlnode);
                    make_comment("Set this to true to get a list of exception on join", config, xmlnode);
                    make_node("Configuration.ChannelModes.aggressive_exception", Configuration.ChannelModes.aggressive_exception.ToString(), curr, confname, config, xmlnode);
                    make_comment("Set this to true to get a list of invite on join", config, xmlnode);
                    make_node("Configuration.ChannelModes.aggressive_invites", Configuration.ChannelModes.aggressive_invites.ToString(), curr, confname, config, xmlnode);
                    make_comment("If CTCP should be completely ignored", config, xmlnode);
                    make_node("Configuration.irc.DisplayCtcp", Configuration.irc.DisplayCtcp.ToString(), curr, confname, config, xmlnode);
                    make_comment("Whether all generated commands need to be confirmed", config, xmlnode);
                    make_node("Configuration.irc.ConfirmAll", Configuration.irc.ConfirmAll.ToString(), curr, confname, config, xmlnode);
                    make_comment("Reason for kick / ban", config, xmlnode);
                    make_node("Configuration.irc.DefaultReason", Configuration.irc.DefaultReason, curr, confname, config, xmlnode);
                    make_comment("Second nick", config, xmlnode);
                    make_node("Configuration.UserData.Nick2", Configuration.UserData.Nick2, curr, confname, config, xmlnode);
                    make_comment("CTCP requests are blocked", config, xmlnode);
                    make_node("Configuration.irc.FirewallCTCP", Configuration.irc.FirewallCTCP.ToString(), curr, confname, config, xmlnode);
                    make_comment("CTCP port", config, xmlnode);
                    make_node("Configuration.irc.DefaultCTCPPort", Configuration.irc.DefaultCTCPPort.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.irc.CertificateDCC", Configuration.irc.CertificateDCC, curr, confname, config, xmlnode);
                    make_comment("Whether the response should be displayed", config, xmlnode);
                    make_node("Configuration.irc.ShowReplyForCTCP", Configuration.irc.ShowReplyForCTCP.ToString(), curr, confname, config, xmlnode);
                    make_comment("If this is true the whois records will be parsed and changed", config, xmlnode);
                    make_node("Configuration.irc.FriendlyWhois", Configuration.irc.FriendlyWhois.ToString(), curr, confname, config, xmlnode);
                    // Windows
                    make_comment(" ============= WINDOWS ============= ", config, xmlnode);
                    make_comment("Main window is maximized", config, xmlnode);
                    make_node("Configuration.Window.Window_Maximized", Configuration.Window.Window_Maximized.ToString(), curr, confname, config, xmlnode);
                    make_comment("Size of main win", config, xmlnode);
                    make_node("Configuration.Window.window_size", Configuration.Window.window_size.ToString(), curr, confname, config, xmlnode);
                    make_comment("Location of slide bar", config, xmlnode);
                    make_node("Configuration.Window.x1", Configuration.Window.x1.ToString(), curr, confname, config, xmlnode);
                    make_comment("Location of slide bar", config, xmlnode);
                    make_node("Configuration.Window.x4", Configuration.Window.x4.ToString(), curr, confname, config, xmlnode);
                    make_comment("Maximum history", config, xmlnode);
                    make_node("Configuration.Window.history", Configuration.Window.history.ToString(), curr, confname, config, xmlnode);
                    make_comment("Search left", config, xmlnode);
                    make_node("Configuration.Search.X", Configuration.Window.Search_X.ToString(), curr, confname, config, xmlnode);
                    make_comment("Search top", config, xmlnode);
                    make_node("Configuration.Search.Y", Configuration.Window.Search_Y.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.UserData.TrayIcon", Configuration.UserData.TrayIcon.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Window.MiddleClick_Side", Configuration.Window.MiddleClick_Side.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Window.RememberPosition", Configuration.Window.RememberPosition.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Window.DoubleClick", Configuration.Window.DoubleClick.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Window.MiddleClick", Configuration.Window.MiddleClick.ToString(), curr, confname, config, xmlnode);
                    // Logs
                    make_comment(" ============= LOGS ============= ", config, xmlnode);
                    make_comment("Where the logs are being saved", config, xmlnode);
                    make_node("Configuration.Logs.logs_dir", Configuration.Logs.logs_dir, curr, confname, config, xmlnode);
                    make_comment("Type of logs to save", config, xmlnode);
                    make_node("Configuration.Logs.logs_name", Configuration.Logs.logs_name, curr, confname, config, xmlnode);
                    make_comment("If you want to save logs in html", config, xmlnode);
                    make_node("Configuration.Logs.logs_html", Configuration.Logs.logs_html.ToString(), curr, confname, config, xmlnode);
                    make_comment("If you want to save logs in XML", config, xmlnode);
                    make_node("Configuration.Logs.logs_xml", Configuration.Logs.logs_xml.ToString(), curr, confname, config, xmlnode);
                    make_comment("If you want to save logs in TXT", config, xmlnode);
                    make_node("Configuration.Logs.logs_txt", Configuration.Logs.logs_txt.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Logs.ServicesLogs", Configuration.Logs.ServicesLogs.ToString(), curr, confname, config, xmlnode);
                    // System
                    make_comment(" ============= SYSTEM ============= ", config, xmlnode);
                    make_comment("Check for updates (recommended)", config, xmlnode);
                    make_node("Configuration.Kernel.CheckUpdate", Configuration.Kernel.CheckUpdate.ToString(), curr, confname, config, xmlnode);
                    make_comment("If pidgeon is running in debug mode", config, xmlnode);
                    make_node("Configuration.Kernel.Debugging", Configuration.Kernel.Debugging.ToString(), curr, confname, config, xmlnode);
                    make_comment("If notification are displayed", config, xmlnode);
                    make_node("Configuration.Kernel.Notice", Configuration.Kernel.Notice.ToString(), curr, confname, config, xmlnode);
                    make_comment("Network sniffer", config, xmlnode);
                    make_node("Configuration.Kernel.NetworkSniff", Configuration.Kernel.NetworkSniff.ToString(), curr, confname, config, xmlnode);
                    make_comment("Skin", config, xmlnode);
                    make_node("Configuration.CurrentSkin.Name", Configuration.CurrentSkin.Name, curr, confname, config, xmlnode);
                    make_comment("Whether color codes override the system color for links", config, xmlnode);
                    make_node("Configuration.Colors.ChangeLinks", Configuration.Colors.ChangeLinks.ToString(), curr, confname, config, xmlnode);
                    make_comment("Maximum size of scrollback in memory", config, xmlnode);
                    make_node("Configuration.Memory.MaximumChannelBufferSize", Configuration.Memory.MaximumChannelBufferSize.ToString(), curr, confname, config, xmlnode);
                    make_comment("This disable the simpleview, saving some memory", config, xmlnode);
                    make_node("Configuration.Memory.EnableSimpleViewCache", Configuration.Memory.EnableSimpleViewCache.ToString(), curr, confname, config, xmlnode);
                    make_comment("The minimal value which you want to see profiler results for", config, xmlnode);
                    make_node("Configuration.Kernel.Profiler_Minimal", Configuration.Kernel.Profiler_Minimal.ToString(), curr, confname, config, xmlnode);
                    make_comment("This enable a performance profiler", config, xmlnode);
                    make_node("Configuration.Kernel.Profiler", Configuration.Kernel.Profiler.ToString(), curr, confname, config, xmlnode);
                    make_comment("This change a ring log size", config, xmlnode);
                    make_node("Configuration.Kernel.MaximalRingLogSize", Configuration.Kernel.MaximalRingLogSize.ToString(), curr, confname, config, xmlnode);
                    make_node("SelectedLanguage", Core.SelectedLanguage.ToString(), curr, confname, config, xmlnode);
                    // Services
                    make_comment(" ============= SERVICES ============= ", config, xmlnode);
                    make_comment("Size of backlog for services", config, xmlnode);
                    make_node("Configuration.Services.Depth", Configuration.Services.Depth.ToString(), curr, confname, config, xmlnode);
                    make_comment("If services are using local cache (incomparably faster)", config, xmlnode);
                    make_node("services.usingcache", Configuration.Services.UsingCache.ToString(), curr, confname, config, xmlnode);
                    // Histories
                    make_comment(" ============= USER ============= ", config, xmlnode);
                    make_comment("Last nickname you used", config, xmlnode);
                    make_node("Configuration.UserData.LastNick", Configuration.UserData.LastNick, curr, confname, config, xmlnode);
                    make_comment("Last host you used", config, xmlnode);
                    make_node("Configuration.UserData.LastHost", Configuration.UserData.LastHost, curr, confname, config, xmlnode);
                    make_comment("Last host you connected to", config, xmlnode);
                    make_node("Configuration.UserData.LastPort", Configuration.UserData.LastPort, curr, confname, config, xmlnode);
                    make_node("Configuration.irc.mq", Configuration.irc.mq.ToString(), curr, confname, config, xmlnode);
                    make_comment("Change this to false in order to disable clickable browser links", config, xmlnode);
                    make_node("Configuration.UserData.OpenLinkInBrowser", Configuration.UserData.OpenLinkInBrowser.ToString(), curr, confname, config, xmlnode);
                    make_comment("SSL", config, xmlnode);
                    make_node("Configuration.UserData.LastSSL", Configuration.UserData.LastSSL.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.UserData.SwitchWindowOnJoin", Configuration.UserData.SwitchWindowOnJoin.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.irc.DetailedVersion", Configuration.irc.DetailedVersion.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Kernel.KernelDump", Configuration.Kernel.KernelDump.ToString(), curr, confname, config, xmlnode);
                    make_comment(" ============= MISC ============= ", config, xmlnode);
                    make_node("Configuration.Parser.formatter", Configuration.Parser.formatter.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Parser.ParserCache", Configuration.Parser.ParserCache.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Media.NotificationSound", Configuration.Media.NotificationSound.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.Kernel.Lang", Configuration.Kernel.Lang, curr, confname, config, xmlnode);
                    make_node("Configuration.Parser.InputTrim", Configuration.Parser.InputTrim.ToString(), curr, confname, config, xmlnode);
                    make_node("Configuration.CurrentSkin", Configuration.CurrentSkin.Name, curr, confname, config, xmlnode);
                    make_node("Configuration.irc.NetworkEncoding", Configuration.irc.NetworkEncoding.ToString(), curr, confname, config, xmlnode);
                    make_comment(" ============= EXTENSION CONFIGURATION ============= ", config, xmlnode);
                    foreach (KeyValuePair<string, string> data in Configuration.Extensions)
                    {
                        make_node("extension." + data.Key, data.Value, curr, confname, config, xmlnode);
                    }

                    string separators = "";
                    make_comment(" ============= SEPARATORS ============= ", config, xmlnode);
                    foreach (char separator in Configuration.Parser.Separators)
                    {
                        separators = separators + separator.ToString();
                    }
                    make_node("delimiters", separators, curr, confname, config, xmlnode);

                    make_comment(" ============= PROTOCOLS ============= ", config, xmlnode);
                    lock (Configuration.Parser.Protocols)
                    {
                        foreach (string xx in Configuration.Parser.Protocols)
                        {
                            curr = config.CreateElement("protocol");
                            curr.InnerText = xx;
                            xmlnode.AppendChild(curr);
                        }
                    }

                    make_comment(" ============= NETWORKS ============= ", config, xmlnode);
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

                    make_comment(" ============= HIGHLIGHTERS ============= ", config, xmlnode);
                    lock (Configuration.HighlighterList)
                    {
                        foreach (Network.Highlighter high in Configuration.HighlighterList)
                        {
                            curr = config.CreateElement("list");
                            XmlAttribute highlightenabled = config.CreateAttribute("enabled");
                            XmlAttribute highlighttext = config.CreateAttribute("text");
                            XmlAttribute highlightsimple = config.CreateAttribute("simple");
                            highlightenabled.Value = high.enabled.ToString();
                            highlightsimple.Value = high.simple.ToString();
                            highlighttext.Value = high.text;
                            curr.Attributes.Append(highlightsimple);
                            curr.Attributes.Append(highlighttext);
                            curr.Attributes.Append(highlightenabled);
                            xmlnode.AppendChild(curr);
                        }
                    }
                    config.AppendChild(xmlnode);

                    make_comment(" ============= ALIASES ============= ", config, xmlnode);
                    lock (Commands.aliases)
                    {
                        foreach (KeyValuePair<string, Commands.CommandLink> keys in Commands.aliases)
                        {
                            curr = config.CreateElement("alias");
                            XmlAttribute name = config.CreateAttribute("name");
                            XmlAttribute target = config.CreateAttribute("target");
                            XmlAttribute overrides = config.CreateAttribute("overrides");
                            name.Value = keys.Key;
                            target.Value = keys.Value.Target;
                            overrides.Value = keys.Value.Overrides.ToString();
                            curr.Attributes.Append(name);
                            curr.Attributes.Append(target);
                            curr.Attributes.Append(overrides);
                            xmlnode.AppendChild(curr);
                        }
                    }
                    config.AppendChild(xmlnode);

                    make_comment(" ============= HISTORY ============= ", config, xmlnode);
                    lock (Configuration.UserData.History)
                    {
                        foreach (string Name in Configuration.UserData.History)
                        {
                            curr = config.CreateElement("history");
                            curr.InnerText = Name;
                            xmlnode.AppendChild(curr);
                        }
                    }
                    config.AppendChild(xmlnode);

                    make_comment(" ============= IGNORE LIST ============= ", config, xmlnode);
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

                    make_comment(" ============= SHORTCUTS ============= ", config, xmlnode);

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

                    make_comment(" ============= WINDOWS ============= ", config, xmlnode);

                    lock (GTK.PidgeonForm.WindowInfo)
                    {
                        foreach (KeyValuePair<string, GTK.PidgeonForm.Info> xx in GTK.PidgeonForm.WindowInfo)
                        {
                            curr = config.CreateElement("window");
                            XmlAttribute height = config.CreateAttribute("height");
                            XmlAttribute width = config.CreateAttribute("width");
                            XmlAttribute x = config.CreateAttribute("x");
                            XmlAttribute y = config.CreateAttribute("y");
                            curr.InnerText = xx.Key;
                            height.Value = xx.Value.Height.ToString();
                            width.Value = xx.Value.Width.ToString();
                            x.Value = xx.Value.X.ToString();
                            y.Value = xx.Value.Y.ToString();
                            curr.Attributes.Append(x);
                            curr.Attributes.Append(y);
                            curr.Attributes.Append(width);
                            curr.Attributes.Append(height);
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

            /// <summary>
            /// Load all configuration and override the current values
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
                        configuration.Load(ConfigFile);
                        lock (Configuration.ShortcutKeylist)
                        {
                            Configuration.ShortcutKeylist = new List<Shortcut>();
                        }
                        lock (Ignoring.IgnoreList)
                        {
                            Ignoring.IgnoreList.Clear();
                        }
                        lock (Configuration.HighlighterList)
                        {
                            Configuration.HighlighterList = new List<Network.Highlighter>();
                        }
                        lock (NetworkData.Networks)
                        {
                            NetworkData.Networks.Clear();
                        }
                        lock (Configuration.UserData.History)
                        {
                            Configuration.UserData.History.Clear();
                        }
                        Commands.ClearAliases();
                        foreach (XmlNode node in configuration.ChildNodes)
                        {
                            if (node.Name == "configuration.pidgeon")
                            {
                                foreach (XmlNode curr in node.ChildNodes)
                                {
                                    if (curr.Name == "history")
                                    {
                                        lock (Configuration.UserData.History)
                                        {
                                            Configuration.UserData.History.Add(curr.InnerText);
                                        }
                                        continue;
                                    }
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
                                        if (curr.Name == "alias")
                                        {
                                            Commands.RegisterAlias(curr.Attributes[0].InnerText, curr.Attributes[1].InnerText, bool.Parse(curr.Attributes[2].InnerText));
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
                                        if (curr.Name == "window")
                                        {
                                            string name = curr.InnerText;
                                            GTK.PidgeonForm.Info w = new GTK.PidgeonForm.Info();
                                            if (curr.Attributes == null)
                                            {
                                                continue;
                                            }
                                            foreach (XmlAttribute option in curr.Attributes)
                                            {
                                                if (curr.Value == null)
                                                {
                                                    // invalid config
                                                    continue;
                                                }
                                                switch (option.Name)
                                                {
                                                    case "width":
                                                       w.Width = int.Parse (curr.Value);
                                                       break;
                                                    case "height":
                                                       w.Height = int.Parse (curr.Value);
                                                       break;
                                                    case "x":
                                                       w.X = int.Parse (curr.Value);
                                                       break;
                                                    case "y":
                                                       w.Y = int.Parse (curr.Value);
                                                       break;
                                                }
                                            }
                                            lock(GTK.PidgeonForm.WindowInfo)
                                            {
                                                if (!GTK.PidgeonForm.WindowInfo.ContainsKey(name))
                                                {
                                                    GTK.PidgeonForm.WindowInfo.Add(name, w);
                                                }
                                            }
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
                                                Shortcut list = new Shortcut(ParseKey(curr.Attributes[0].Value), bool.Parse(curr.Attributes[1].Value), bool.Parse(curr.Attributes[2].Value), bool.Parse(curr.Attributes[3].Value));
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
                                                    case "Configuration.Window.x1":
                                                    case "location.x1":
                                                        Configuration.Window.x1 = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Window.window_size":
                                                    case "window.size":
                                                        Configuration.Window.window_size = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Window.x4":
                                                    case "location.x4":
                                                        Configuration.Window.x4 = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Window.Window_Maximized":
                                                    case "location.maxi":
                                                        Configuration.Window.Window_Maximized = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "timestamp.display":
                                                    case "Configuration.Scrollback.chat_timestamp":
                                                        Configuration.Scrollback.chat_timestamp = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.UserData.nick":
                                                    case "network.nick":
                                                        Configuration.UserData.nick = curr.InnerText;
                                                        break;
                                                    case "Configuration.UserData.Nick2":
                                                    case "network.n2":
                                                        Configuration.UserData.Nick2 = curr.InnerText;
                                                        break;
                                                    case "Configuration.UserData.ident":
                                                    case "network.ident":
                                                        Configuration.UserData.ident = curr.InnerText;
                                                        break;
                                                    case "Configuration.irc.DisplayCtcp":
                                                    case "scrollback.showctcp":
                                                        Configuration.irc.DisplayCtcp = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Scrollback.format_nick":
                                                    case "formats.user":
                                                        Configuration.Scrollback.format_nick = curr.InnerText;
                                                        break;
                                                    case "Configuration.Scrollback.format_date":
                                                    case "formats.date":
                                                        Configuration.Scrollback.format_date = curr.InnerText;
                                                        break;
                                                    case "Configuration.Scrollback.timestamp_mask":
                                                    case "formats.datetime":
                                                        Configuration.Scrollback.timestamp_mask = curr.InnerText;
                                                        break;
                                                    case "Configuration.ChannelModes.aggressive_whois":
                                                    case "irc.auto.whois":
                                                        Configuration.ChannelModes.aggressive_whois = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.ChannelModes.aggressive_bans":
                                                    case "irc.auto.bans":
                                                        Configuration.ChannelModes.aggressive_bans = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.ChannelModes.aggressive_exception":
                                                    case "irc.auto.exception":
                                                        Configuration.ChannelModes.aggressive_exception = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.ChannelModes.aggressive_channel":
                                                    case "irc.auto.channels":
                                                        Configuration.ChannelModes.aggressive_channel = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.ChannelModes.aggressive_invites":
                                                    case "irc.auto.invites":
                                                        Configuration.ChannelModes.aggressive_invites = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.ChannelModes.aggressive_mode":
                                                    case "irc.auto.mode":
                                                        Configuration.ChannelModes.aggressive_mode = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.irc.DefaultReason":
                                                    case "network.reason":
                                                        Configuration.irc.DefaultReason = curr.InnerText;
                                                        break;
                                                    case "Configuration.Logs.logs_name":
                                                    case "logs.type":
                                                        Configuration.Logs.logs_name = curr.InnerText;
                                                        break;
                                                    case "logs.html":
                                                    case "Configuration.Logs.logs_html":
                                                        Configuration.Logs.logs_html = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "logs.dir":
                                                    case "Configuration.Logs.logs_dir":
                                                        Configuration.Logs.logs_dir = curr.InnerText;
                                                        break;
                                                    case "logs.xml":
                                                    case "Configuration.Logs.logs_xml":
                                                        Configuration.Logs.logs_xml = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "logs.txt":
                                                    case "Configuration.Logs.logs_txt":
                                                        Configuration.Logs.logs_txt = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Scrollback.scrollback_plimit":
                                                    case "scrollback_plimit":
                                                        Configuration.Scrollback.scrollback_plimit = int.Parse(curr.InnerText);
                                                        break;
                                                    case "notification.tray":
                                                    case "Configuration.Kernel.Notice":
                                                        Configuration.Kernel.Notice = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "pidgeon.size":
                                                    case "Configuration.Services.Depth":
                                                        Configuration.Services.Depth = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.UserData.LastNick":
                                                    case "history.nick":
                                                        Configuration.UserData.LastNick = curr.InnerText;
                                                        break;
                                                    case "Configuration.Kernel.NetworkSniff":
                                                    case "sniffer":
                                                        Configuration.Kernel.NetworkSniff = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.UserData.LastHost":
                                                    case "history.host":
                                                        Configuration.UserData.LastHost = curr.InnerText;
                                                        break;
                                                    case "Configuration.Kernel.CheckUpdate":
                                                    case "updater.check":
                                                        Configuration.Kernel.CheckUpdate = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.UserData.LastPort":
                                                    case "history.port":
                                                        Configuration.UserData.LastPort = curr.InnerText;
                                                        break;
                                                    case "Configuration.Parser.Separators":
                                                    case "delimiters":
                                                        List<char> temp = new List<char>();
                                                        foreach (char part in curr.InnerText)
                                                        {
                                                            temp.Add(part);
                                                        }
                                                        Configuration.Parser.Separators = temp;
                                                        break;
                                                    case "colors.changelinks":
                                                    case "Configuration.Colors.ChangeLinks":
                                                        Configuration.Colors.ChangeLinks = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Kernel.Debugging":
                                                    case "debugger":
                                                        Configuration.Kernel.Debugging = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Services.UsingCache":
                                                    case "services.usingcache":
                                                        Configuration.Services.UsingCache = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.irc.mq":
                                                    case "message_mq":
                                                        Configuration.irc.mq = int.Parse(curr.InnerText);
                                                        break;
                                                    case "logs.services.log.type":
                                                    case "Configuration.Logs.ServicesLogs":
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
                                                    case "Configuration.UserData.OpenLinkInBrowser":
                                                        Configuration.UserData.OpenLinkInBrowser = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Parser.formatter":
                                                    case "formatter":
                                                        Configuration.Parser.formatter = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Memory.MaximumChannelBufferSize":
                                                    case "system.maximumchannelbuffersize":
                                                        Configuration.Memory.MaximumChannelBufferSize = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Memory.EnableSimpleViewCache":
                                                    case "system.enablesimpleviewcache":
                                                        Configuration.Memory.EnableSimpleViewCache = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Search.Y":
                                                    case "Configuration.Window.Search_Y":
                                                        Configuration.Window.Search_Y = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Search.X":
                                                    case "Configuration.Window.Search_X":
                                                        Configuration.Window.Search_X = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Kernel.Profiler":
                                                        Configuration.Kernel.Profiler = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.irc.FirewallCTCP":
                                                        Configuration.irc.FirewallCTCP = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Kernel.MaximalRingLogSize":
                                                    case "maximumlog":
                                                        Configuration.Kernel.MaximalRingLogSize = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Parser.ParserCache":
                                                        Configuration.Parser.ParserCache = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Kernel.Profiler_Minimal":
                                                        Configuration.Kernel.Profiler_Minimal = int.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.UserData.TrayIcon":
                                                        Configuration.UserData.TrayIcon = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.UserData.LastSSL":
                                                        Configuration.UserData.LastSSL = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Window.MiddleClick_Side":
                                                        Configuration.Window.MiddleClick_Side = from1(curr.InnerText);
                                                        break;
                                                    case "SelectedLanguage":
                                                        Core.SelectedLanguage = curr.InnerText;
                                                        break;
                                                    case "Configuration.Media.NotificationSound":
                                                        Configuration.Media.NotificationSound = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.CurrentSkin":
                                                        Skin.ReloadSkin(curr.InnerText);
                                                        break;
                                                    case "Configuration.Window.RememberPosition":
                                                        Configuration.Window.RememberPosition = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.irc.DefaultCTCPPort":
                                                        Configuration.irc.DefaultCTCPPort = uint.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.irc.CertificateDCC":
                                                        Configuration.irc.CertificateDCC = curr.InnerText;
                                                        break;
                                                    case "Configuration.UserData.SwitchWindowOnJoin":
                                                        Configuration.UserData.SwitchWindowOnJoin = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Window.DoubleClick":
                                                        if (!Enum.TryParse<Configuration.UserList_MouseClick>(curr.InnerText, out Configuration.Window.DoubleClick))
                                                        {
                                                            Configuration.Window.DoubleClick = Configuration.UserList_MouseClick.Nothing;
                                                        }
                                                        break;
                                                    case "Configuration.Window.MiddleClick":
                                                        if (!Enum.TryParse<Configuration.UserList_MouseClick>(curr.InnerText, out Configuration.Window.MiddleClick))
                                                        {
                                                            Configuration.Window.MiddleClick = Configuration.UserList_MouseClick.Nothing;
                                                        }
                                                        break;
                                                    case "Configuration.irc.ShowReplyForCTCP":
                                                        Configuration.irc.ShowReplyForCTCP = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.irc.FriendlyWhois":
                                                        Configuration.irc.FriendlyWhois = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.irc.DetailedVersion":
                                                        Configuration.irc.DetailedVersion = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.irc.NetworkEncoding":
                                                        switch (curr.InnerText)
                                                        {
                                                            case "ASCIIEncoding":
                                                                Configuration.irc.NetworkEncoding = System.Text.Encoding.ASCII;
                                                                break;
                                                            case "UTF32Encoding":
                                                                Configuration.irc.NetworkEncoding = System.Text.Encoding.UTF32;
                                                                break;
                                                            case "UnicodeEncoding":
                                                                Configuration.irc.NetworkEncoding = System.Text.Encoding.Unicode;
                                                                break;
                                                            case "UTF7Encoding":
                                                                Configuration.irc.NetworkEncoding = System.Text.Encoding.UTF7;
                                                                break;
                                                        }
                                                        break;
                                                    case "Configuration.Parser.InputTrim":
                                                        Configuration.Parser.InputTrim = bool.Parse(curr.InnerText);
                                                        break;
                                                    case "Configuration.Kernel.KernelDump":
                                                        Configuration.Kernel.KernelDump = bool.Parse(curr.InnerText);
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
}
