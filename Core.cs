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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class Core
    {
        public static DateTime LoadTime;
        public static string ConfigFile = "configuration.dat";
        public static string SelectedLanguage = "en";
        public static List<Protocol> Connections = new List<Protocol>();
        public static Network network;
        public static Main _Main;

        public static bool Load()
        {
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
                            _Main._Scrollback.InsertText(messages.get("nick", SelectedLanguage), Scrollback.MessageStyle.User);
                        }
                        return true;
                    }
                    if (!network.Connected)
                    {
                        if (Nick != "")
                        {
                            Configuration.nick = Nick;
                            _Main._Scrollback.InsertText(messages.get("nick", SelectedLanguage), Scrollback.MessageStyle.User);
                        }
                        return false;
                    }
                    network._protocol.requestNick(Nick);
                    return true;
                case "join":
                    if (command.Length > "join ".Length)
                    {
                        string channel = command;
                        channel = command.Substring("join ".Length);
                        if (network == null)
                        {
                            _Main._Scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                            return false;
                        }
                        if (!network.Connected)
                        {
                            _Main._Scrollback.InsertText(messages.get("error1", SelectedLanguage), Scrollback.MessageStyle.System);
                            return false;
                        }
                        network._protocol.Join(channel);
                        return false;
                    }
                    _Main._Scrollback.InsertText(messages.get("invalid-channel", SelectedLanguage), Scrollback.MessageStyle.System);
                    return false;
                case "server":
                    if (command.Length < "server ".Length)
                    {
                        _Main._Scrollback.InsertText(messages.get("invalid-server", SelectedLanguage), Scrollback.MessageStyle.System);
                        return true;
                    }
                    string name = command.Substring("server ".Length);
                    if (name == "")
                    {
                        _Main._Scrollback.InsertText(messages.get("invalid-server", SelectedLanguage), Scrollback.MessageStyle.System);
                        return true;
                    }
                    connectIRC(name);
                    return true;
            }
            if (network != null)
            {
                if (network.Connected)
                {
                    network._protocol.Command(command);
                    return false;
                }
            }
            _Main._Scrollback.InsertText(messages.get("invalid-command", SelectedLanguage), Scrollback.MessageStyle.System);

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
            foreach (Protocol server in Connections)
            {
                server.Exit();
            }
            System.Windows.Forms.Application.Exit();
            return true;
        }
    }
}
