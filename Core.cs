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
using System.Linq;
using System.Text;

namespace Client
{
    class Core
    {
        public static Thread _KernelThread;
        public static DateTime LoadTime;
        public static string ConfigFile = "configuration.dat";
        public static string SelectedLanguage = "en";
        public static List<Protocol> Connections = new List<Protocol>();
        public static Network network;
        public static Main _Main;

        public static bool Load()
        {
            _KernelThread = System.Threading.Thread.CurrentThread;
            ConfigurationLoad();
            LoadTime = DateTime.Now;
            messages.data.Add("en", new messages.container("en"));
            return true;
        }

        public static bool ProcessCommand(string command)
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
                                    network._protocol.Message(command.Substring(command.IndexOf(channel) + 1 + channel.Length), channel);
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
                    }
                    break;
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
                                network.ShowChat(window.name);
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
                case "pidgeon.config":
                    //Core._Main.ShowPr();
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
            if (network != null)
            {
                if (network.Connected)
                {
                    network._protocol.Command(command);
                    return false;
                }
            }
            _Main.Chat.scrollback.InsertText(messages.get("invalid-command", SelectedLanguage), Scrollback.MessageStyle.System);

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

        private static bool makenode(string config_key, string text, XmlNode xmlnode, XmlAttribute key, XmlDocument _c , XmlNode conf)
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
            makenode("ident", Configuration.ident, curr, confname, config, xmlnode);
            makenode("quitmessage", Configuration.quit, curr, confname, config, xmlnode);
            makenode("nick", Configuration.nick, curr, confname, config, xmlnode);
            makenode("maxi", Configuration.Window_Maximized.ToString(), curr, confname, config, xmlnode);
            makenode("window.size", Configuration.window_size.ToString(), curr, confname, config, xmlnode);
            makenode("x1", Configuration.x1.ToString() , curr, confname, config, xmlnode);
            makenode("x4", Configuration.x4.ToString(), curr, confname, config, xmlnode);
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
                                case "x1":
                                    Configuration.x1 = int.Parse(curr.InnerText);
                                    break;
                                case "window.size":
                                    Configuration.window_size = int.Parse(curr.InnerText);
                                    break;
                                case "x4":
                                    Configuration.x4 = int.Parse(curr.InnerText);
                                    break;
                                case "nick":
                                    Configuration.nick = curr.InnerText;
                                    break;
                                case "maxi":
                                    Configuration.Window_Maximized = bool.Parse(curr.InnerText);
                                    break;
                                case "ident":
                                    Configuration.ident = curr.InnerText;
                                    break;
                                

                            }
                        }
                    }
                }
            }
            return false;
        }

        public static bool windowReady(Window chat)
        {
            if (chat != null)
            {
                while (chat.Making)
                {
                    System.Threading.Thread.Sleep(100);
                }
                return true;
            }
            return false;
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
            protocol._server = new Network(server);
            network = protocol._server;
            protocol._server._protocol = protocol;
            protocol.Open();
            return true;
        }

        public static int handleException(Exception _exception)
        {
            Console.WriteLine(_exception.InnerException);
            return 0;
        }

        public static bool Quit()
        {
            _Main.Visible = false;
            ConfigSave();
            foreach (Protocol server in Connections)
            {
                server.Exit();
            }
            System.Threading.Thread.Sleep(200);
            System.Windows.Forms.Application.Exit();
            return true;
        }
    }
}
