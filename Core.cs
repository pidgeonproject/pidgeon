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
using System.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Client
{
    public class Core
    {
        public enum Platform
        {
            Linuxx64,
            Linuxx86,
            Windowsx64,
            Windowsx86,
            MacOSx64,
            MacOSx86,
        }

        public static Thread _KernelThread;
        public static DateTime LoadTime;
        public static string ConfigFile = System.Windows.Forms.Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + "configuration.dat";
        public static string SelectedLanguage = "en";
        public static List<Protocol> Connections = new List<Protocol>();
        public static Thread ThUp;

        public static Exception recovery_exception;
        public static Thread _RecoveryThread;
        public static List<Thread> SystemThreads = new List<Thread>();

        public static Network network;
        public static Main _Main;
        public static TrafficScanner trafficscanner;
        public static bool blocked = false;
        public static bool IgnoreErrors = false;

        public static bool Load()
        {
            _KernelThread = System.Threading.Thread.CurrentThread;
            LoadTime = DateTime.Now;
            if (System.Windows.Forms.Application.LocalUserAppDataPath.EndsWith(System.Windows.Forms.Application.ProductVersion))
            {
                ConfigFile = System.Windows.Forms.Application.LocalUserAppDataPath.Substring(0,
                    System.Windows.Forms.Application.LocalUserAppDataPath.Length - System.Windows.Forms.Application.ProductVersion.Length) + "configuration.dat";
            }
            messages.data.Add("en", new messages.container("en"));
            messages.data.Add("cs", new messages.container("cs"));
            trafficscanner = new TrafficScanner();
            if (!System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "pidgeon.dat"))
            {
                ThUp = new Thread(Updater.Run);
                ThUp.Start();
                ConfigurationLoad();
                return true;
            }
            Updater _finalisingupdater = new Updater();
            _finalisingupdater.update.Visible = false;
            _finalisingupdater.finalize = true;
            _finalisingupdater.lStatus.Text = messages.get("update2");
            System.Windows.Forms.Application.Run(_finalisingupdater);
            return false;
        }

        public static void Recover()
        {
            Recovery x = new Recovery();
            System.Windows.Forms.Application.Run(x);
        }

        public static void killThread(Thread name)
        {
            lock (SystemThreads)
            {
                if (SystemThreads.Contains(name))
                {
                    SystemThreads.Remove(name);
                }
            }
        }

        public static bool ProcessCommand(string command)
        {
            try
            {
                if (command.StartsWith(Configuration.CommandPrefix))
                {
                    command = command.Substring(1);
                }
                string[] values = command.Split(' ');
                switch (values[0].ToLower())
                {
                    case "quit":
                        Core.Quit();
                        return true;
                    case "nick":
                        string Nick = "";
                        if (command.Length > "nick ".Length)
                        {
                            Nick = command.Substring("nick ".Length);
                        }
                        if (network == null)
                        {
                            if (Nick != "")
                            {
                                Configuration.nick = Nick;
                                _Main.Chat.scrollback.InsertText(messages.get("nick", SelectedLanguage), Scrollback.MessageStyle.User);
                            }
                            return true;
                        }
                        if (!network.Connected)
                        {
                            if (Nick != "")
                            {
                                Configuration.nick = Nick;
                                _Main.Chat.scrollback.InsertText(messages.get("nick", SelectedLanguage), Scrollback.MessageStyle.User);
                            }
                            return false;
                        }
                        network._protocol.requestNick(Nick);
                        return true;
                    case "msg":
                        if (command.Length > "msg ".Length)
                        {
                            string channel = command;
                            channel = command.Substring("msg ".Length);
                            if (channel.Contains(" "))
                            {
                                channel = channel.Substring(0, channel.IndexOf(" "));
                                if (network != null)
                                {
                                    if (network.Connected)
                                    {
                                        string ms = command.Substring(4 + channel.Length);
                                        while (ms.StartsWith(" "))
                                        {
                                            ms = ms.Substring(1);
                                        }
                                        network.system.scrollback.InsertText("[>> " + channel + "] <" + network.nickname + "> " + ms, Scrollback.MessageStyle.System);
                                        network._protocol.Message(ms, channel);
                                        return false;
                                    }
                                    _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                    return false;
                                }
                                _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                return false;
                            }
                            return false;
                        }
                        return false;
                    case "query":
                        if (command.Length > "query ".Length)
                        {
                            string channel = command;
                            channel = command.Substring("query ".Length);
                            if (channel.Contains(network.channel_prefix))
                            {
                                _Main.Chat.scrollback.InsertText("Invalid name", Scrollback.MessageStyle.System);
                                return false;
                            }
                            while (channel.StartsWith(" "))
                            {
                                channel.Substring(1);
                            }
                            if (channel.Contains(" "))
                            {
                                channel = channel.Substring(0, channel.IndexOf(" "));
                                if (network != null && network._protocol != null)
                                {
                                    if (network.Connected)
                                    {
                                        if (!network._protocol.windows.ContainsKey(channel))
                                        {
                                            network.Private(channel);
                                        }
                                        network._protocol.ShowChat(channel);
                                        network._protocol.windows[channel].scrollback.InsertText(network._protocol.PRIVMSG(network.nickname, command.Substring(command.IndexOf(channel) + 1 + channel.Length)), Scrollback.MessageStyle.Channel);
                                        network._protocol.Message(command.Substring(command.IndexOf(channel) + 1 + channel.Length), channel);
                                        return false;
                                    }
                                    _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                    return false;
                                }
                                _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                return false;
                            }
                            if (network != null && network._protocol != null)
                            {
                                if (network.Connected)
                                {
                                    if (!network._protocol.windows.ContainsKey(channel))
                                    {
                                        network.Private(channel);
                                    }
                                    network._protocol.ShowChat(channel);
                                }
                            }
                            return false;
                        }
                        break;
                    case "me":
                        if (command.Length > "me ".Length)
                        {
                            string message = command.Substring(3);
                            if (network == null)
                            {
                                _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                return false;
                            }
                            if (!network.Connected)
                            {
                                _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                return false;
                            }
                            Channel curr = network.RenderedChannel;
                            if (curr != null)
                            {
                                Window window = curr.retrieveWindow();
                                if (window != null)
                                {
                                    window.scrollback.InsertText(">>>>>>" + network.nickname + " " + message, Scrollback.MessageStyle.Action);
                                    network._protocol.Message2(message, curr.Name);
                                }
                            }
                            return false;
                        }
                        return false;
                    case "join":
                        if (command.Length > "join ".Length)
                        {
                            string channel = command;
                            channel = command.Substring("join ".Length);
                            if (network == null)
                            {
                                _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                return false;
                            }
                            if (!network.Connected)
                            {
                                _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                return false;
                            }
                            Channel curr = network.getChannel(channel);
                            if (curr != null)
                            {
                                Window window = curr.retrieveWindow();
                                if (window != null)
                                {
                                    network._protocol.ShowChat(window.name);
                                }
                            }
                            network._protocol.Join(channel);
                            return false;
                        }
                        _Main.Chat.scrollback.InsertText(messages.get("invalid-channel", SelectedLanguage), Scrollback.MessageStyle.System);
                        return false;
                    case "server":
                        if (command.Length < "server ".Length)
                        {
                            _Main.Chat.scrollback.InsertText(messages.get("invalid-server", SelectedLanguage), Scrollback.MessageStyle.System);
                            return true;
                        }
                        string name = command.Substring("server ".Length);
                        string b = command.Substring(command.IndexOf(name) + name.Length);
                        int n2;
                        if (name == "")
                        {
                            _Main.Chat.scrollback.InsertText(messages.get("invalid-server", SelectedLanguage), Scrollback.MessageStyle.System);
                            return true;
                        }
                        if (int.TryParse(b, out n2))
                        {
                            connectIRC(name, n2);
                            return true;
                        }
                        connectIRC(name);
                        return true;
                    case "pidgeon.rehash":
                        //Core._Main.ShowPr();
                        break;
                    case "connect":
                        if (command.Length < "connect ".Length)
                        {
                            _Main.Chat.scrollback.InsertText(messages.get("invalid-server", SelectedLanguage), Scrollback.MessageStyle.System);
                            return true;
                        }
                        string name2 = command.Substring("server ".Length);
                        string b2 = command.Substring(command.IndexOf(name2) + name2.Length);
                        int n3;
                        if (name2 == "")
                        {
                            _Main.Chat.scrollback.InsertText(messages.get("invalid-server", SelectedLanguage), Scrollback.MessageStyle.System);
                            return false;
                        }
                        if (Core._Main.Chat._Protocol == null)
                        {
                            return false;
                        }
                        if (Core._Main.Chat._Protocol.type == 3)
                        {
                            if (int.TryParse(b2, out n3))
                            {
                                Core._Main.Chat._Protocol.ConnectTo(name2, n3);
                                return false;
                            }

                            Core._Main.Chat._Protocol.ConnectTo(name2, 6667);
                            return false;
                        }
                        break;
                    case "pidgeon.service":
                        if ("pidgeon.service ".Length < command.Length)
                        {
                            List<string> parameters = new List<string>();
                            parameters.AddRange(command.Split(' '));
                            int port = int.Parse(parameters[2]);
                            connectPS(parameters[0], port, parameters[1]);
                            return false;
                        }
                        break;
                    case "service.gnick":
                        string nick = command.Substring("service.gnick ".Length);
                        if (Core._Main.Chat._Protocol != null)
                        {
                            if (Core._Main.Chat._Protocol.type == 3)
                            {
                                Core._Main.Chat._Protocol.requestNick(nick);
                                return false;
                            }
                        }
                        break;
                    case "raw":
                        if (network != null)
                        {
                            if (command.Length > 4)
                            {
                                string text = command.Substring("raw ".Length);
                                if (network.Connected)
                                {
                                    network._protocol.Command(text);
                                    return false;
                                }
                                _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                                return false;
                            }
                        }
                        _Main.Chat.scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                        return false;
                }
                // if we are connected to something let's handle this command by server
                if (Core._Main.Chat._Protocol != null)
                {
                    if (_Main.Chat._Protocol.Connected)
                    {
                        Core._Main.Chat._Protocol.Command(command);
                        return false;
                    }
                }
                _Main.Chat.scrollback.InsertText(messages.get("invalid-command", SelectedLanguage), Scrollback.MessageStyle.System);
                return false;
            }
            catch (Exception f)
            {
                handleException(f);
            }
            return false;
        }

        public static bool Restore(string file)
        {
            if (!File.Exists(file + "~"))
            {
                return false;
            }
            File.Copy(file + "~", file, true);
            return true;
        }

        public static bool Backup(string file)
        {
            if (File.Exists(file + "~"))
            {
                string backup = System.IO.Path.GetRandomFileName();
                File.Copy(file + "~", backup);
            }
            if (File.Exists(file))
            {
                File.Copy(file, file + "~", true);
            }
            return true;
        }

        private static bool makenode(string config_key, string text, XmlNode xmlnode, XmlAttribute key, XmlDocument _c, XmlNode conf)
        {
            key = _c.CreateAttribute("confname");
            xmlnode = _c.CreateElement("data");
            key.Value = config_key;
            xmlnode.Attributes.Append(key);
            xmlnode.InnerText = text;
            conf.AppendChild(xmlnode);
            return true;
        }

        public static bool ConfigSave()
        {
            System.Xml.XmlDocument config = new System.Xml.XmlDocument();
            System.Xml.XmlNode xmlnode = config.CreateElement("configuration.pidgeon");
            System.Xml.XmlNode curr = null;
            XmlAttribute confname = null;
            config.AppendChild(xmlnode);
            makenode("network.ident", Configuration.ident, curr, confname, config, xmlnode);
            makenode("quitmessage", Configuration.quit, curr, confname, config, xmlnode);
            makenode("network.nick", Configuration.nick, curr, confname, config, xmlnode);
            makenode("scrollback.showctcp", Configuration.DisplayCtcp.ToString(), curr, confname, config, xmlnode);
            makenode("formats.user", Configuration.format_nick, curr, confname, config, xmlnode);
            makenode("formats.date", Configuration.format_date, curr, confname, config, xmlnode);
            makenode("irc.auto.whois", Configuration.aggressive_whois.ToString(), curr, confname, config, xmlnode);
            makenode("irc.auto.mode", Configuration.aggressive_mode.ToString(), curr, confname, config, xmlnode);
            makenode("irc.auto.channels", Configuration.aggressive_channel.ToString(), curr, confname, config, xmlnode);
            makenode("irc.auto.bans", Configuration.aggressive_bans.ToString(), curr, confname, config, xmlnode);
            makenode("irc.auto.exception", Configuration.aggressive_exception.ToString(), curr, confname, config, xmlnode);
            makenode("irc.auto.invites", Configuration.aggressive_invites.ToString(), curr, confname, config, xmlnode);

            makenode("location.maxi", Configuration.Window_Maximized.ToString(), curr, confname, config, xmlnode);
            makenode("window.size", Configuration.window_size.ToString(), curr, confname, config, xmlnode);
            makenode("location.x1", Configuration.x1.ToString(), curr, confname, config, xmlnode);
            makenode("location.x4", Configuration.x4.ToString(), curr, confname, config, xmlnode);
            makenode("logs.dir", Configuration.logs_dir, curr, confname, config, xmlnode);
            makenode("logs.type", Configuration.logs_name, curr, confname, config, xmlnode);
            makenode("shield.ctcp", Configuration.ctcp_prot.ToString(), curr, confname, config, xmlnode);
            makenode("shield.flood", Configuration.flood_prot.ToString(), curr, confname, config, xmlnode);
            makenode("shield.notice", Configuration.notice_prot.ToString(), curr, confname, config, xmlnode);
            makenode("ignore.ctcp", Configuration.DisplayCtcp.ToString(), curr, confname, config, xmlnode);
            makenode("logs.html", Configuration.logs_html.ToString(), curr, confname, config, xmlnode);
            makenode("logs.xml", Configuration.logs_xml.ToString(), curr, confname, config, xmlnode);
            makenode("logs.txt", Configuration.logs_txt.ToString(), curr, confname, config, xmlnode);
            makenode("updater.check", Configuration.CheckUpdate.ToString(), curr, confname, config, xmlnode);

            makenode("history.nick", Configuration.LastNick, curr, confname, config, xmlnode);
            makenode("history.host", Configuration.LastHost, curr, confname, config, xmlnode);
            makenode("history.port", Configuration.LastPort, curr, confname, config, xmlnode);

            if (Backup(ConfigFile))
            {
                config.Save(ConfigFile);
            }
            File.Delete(ConfigFile + "~");
            return false;
        }

        /// <summary>
        /// Conf
        /// </summary>
        /// <returns></returns>
        public static bool ConfigurationLoad()
        {
            // Check if config file is present
            if (File.Exists(ConfigFile))
            {
                try
                {
                    XmlDocument configuration = new XmlDocument();
                    configuration.Load(ConfigFile);
                    foreach (XmlNode curr in configuration.ChildNodes[0].ChildNodes)
                    {
                        if (curr.Attributes.Count > 0)
                        {
                            if (curr.Attributes[0].Name == "confname")
                            {
                                switch (curr.Attributes[0].Value)
                                {
                                    case "location.x1":
                                        Configuration.x1 = int.Parse(curr.InnerText);
                                        break;
                                    case "window.size":
                                        Configuration.window_size = int.Parse(curr.InnerText);
                                        break;
                                    case "location.x4":
                                        Configuration.x4 = int.Parse(curr.InnerText);
                                        break;
                                    case "network.nick":
                                        Configuration.nick = curr.InnerText;
                                        break;
                                    case "location.maxi":
                                        Configuration.Window_Maximized = bool.Parse(curr.InnerText);
                                        break;
                                    case "timestamp.display":
                                        Configuration.chat_timestamp = bool.Parse(curr.InnerText);
                                        break;
                                    case "network.ident":
                                        Configuration.ident = curr.InnerText;
                                        break;
                                    case "scrollback.showctcp":
                                        Configuration.DisplayCtcp = bool.Parse(curr.InnerText);
                                        break;
                                    case "formats.user":
                                        Configuration.format_nick = curr.InnerText;
                                        break;
                                    case "formats.date":
                                        Configuration.format_date = curr.InnerText;
                                        break;
                                    case "formats.datetime":
                                        Configuration.timestamp_mask = curr.InnerText;
                                        break;
                                    case "irc.auto.whois":
                                        Configuration.aggressive_whois = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.bans":
                                        Configuration.aggressive_bans = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.exception":
                                        Configuration.aggressive_exception = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.channels":
                                        Configuration.aggressive_channel = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.invites":
                                        Configuration.aggressive_invites = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.mode":
                                        Configuration.aggressive_mode = bool.Parse(curr.InnerText);
                                        break;
                                    case "network.reason":
                                        Configuration.DefaultReason = curr.InnerText;
                                        break;
                                    case "logs.type":
                                        Configuration.logs_name = curr.InnerText;
                                        break;
                                    case "shield.ctcp":
                                        Configuration.ctcp_prot = bool.Parse(curr.InnerText);
                                        break;
                                    case "shield.flood":
                                        Configuration.flood_prot = bool.Parse(curr.InnerText);
                                        break;
                                    case "shield.notice":
                                        Configuration.notice_prot = bool.Parse(curr.InnerText);
                                        break;
                                    case "ignore.ctcp":
                                        Configuration.DisplayCtcp = bool.Parse(curr.InnerText);
                                        break;
                                    case "logs.html":
                                        Configuration.logs_html = bool.Parse(curr.InnerText);
                                        break;
                                    case "logs.dir":
                                        Configuration.logs_dir = curr.InnerText;
                                        break;
                                    case "logs.xml":
                                        Configuration.logs_xml = bool.Parse(curr.InnerText);
                                        break;
                                    case "logs.txt":
                                        Configuration.logs_txt = bool.Parse(curr.InnerText);
                                        break;
                                    case "history.nick":
                                        Configuration.LastNick = curr.InnerText;
                                        break;
                                    case "history.host":
                                        Configuration.LastHost = curr.InnerText;
                                        break;
                                    case "history.port":
                                        Configuration.LastPort = curr.InnerText;
                                        break;
                                    case "updater.check":
                                        Configuration.CheckUpdate = bool.Parse(curr.InnerText);
                                        break;


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
            return false;
        }

        public static bool connectQl(string server, int port)
        {
            ProtocolQuassel _quassel = new ProtocolQuassel();
            _quassel.Open();
            return false;
        }

        /// <summary>
        /// Connect to pidgeon server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool connectPS(string server, int port = 8222, string password = "xx")
        {
            ProtocolSv protocol = new ProtocolSv();
            protocol.Server = server;
            protocol.nick = Configuration.nick;
            protocol.Port = port;
            protocol.password = password;
            Connections.Add(protocol);
            protocol.Open();
            return true;
        }


        /// <summary>
        /// Connect to IRC
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static bool connectIRC(string server, int port = 6667)
        {
            ProtocolIrc protocol = new ProtocolIrc();
            Connections.Add(protocol);
            protocol.Server = server;
            protocol.Port = port;
            protocol._server = new Network(server, protocol);
            network = protocol._server;
            protocol._server._protocol = protocol;
            protocol.Open();
            return true;
        }

        public static int handleException(Exception _exception)
        {
            if (IgnoreErrors)
            {
                return -2;
            }
            Console.WriteLine(_exception.InnerException);
            blocked = true;
            recovery_exception = _exception;
            _RecoveryThread = new Thread(Recover);
            _RecoveryThread.Start();
            while (blocked)
            {
                Thread.Sleep(100);
            }
            return 0;
        }

        public static bool Quit()
        {
            try
            {
                if (!IgnoreErrors)
                {
                    IgnoreErrors = true;
                    _Main.Visible = false;
                    ConfigSave();
                    try
                    {
                        foreach (Protocol server in Connections)
                        {
                            server.Exit();
                        }
                    }
                    catch (Exception)
                    { }
                    System.Threading.Thread.Sleep(200);
                    try
                    {
                        foreach (Thread th in SystemThreads)
                        {
                            if (th.ThreadState == System.Threading.ThreadState.Running)
                            {
                                th.Abort();
                            }
                        }
                    }
                    catch (Exception)
                    { }
                    System.Windows.Forms.Application.Exit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception)
            { }
            return true;
        }
    }
}
