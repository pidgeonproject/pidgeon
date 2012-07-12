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
using System.IO;
using System.Net;
using System.Xml;
using System.Threading;
using System.Text;

namespace pidgeon_sv
{
    class Core
    {
        public static List<Connection> ActiveUsers = new List<Connection>();
        public static bool running = true;
        public static int server_port = 22;
        public static string userfile = "db/users";

        public static int maxbs = 200000;
        public static int minbs = 20000;


        public static List<Account> _accounts = new List<Account>();
        public static List<Thread> threads = new List<Thread>();

        public static void Quit()
        {
            SL("Killing all connections and running processes");
            foreach (Thread curr in threads)
            {
                curr.Abort();
            }
            SL("Exiting");
        }

        public static void SaveData()
        {
            lock (_accounts)
            {
                System.Xml.XmlDocument config = new System.Xml.XmlDocument();
                foreach (Account user in _accounts)
                {
                    System.Xml.XmlNode xmlnode = config.CreateElement("user");
                    XmlAttribute name = config.CreateAttribute("name");
                    XmlAttribute pw = config.CreateAttribute("password");
                    XmlAttribute nickname = config.CreateAttribute("nickname");
                    XmlAttribute ident = config.CreateAttribute("ident");
                    XmlAttribute realname = config.CreateAttribute("realname");
                    name.Value = user.username;
                    pw.Value = user.password;
                    xmlnode.Attributes.Append(name);
                    nickname.Value = user.nickname;
                    ident.Value = user.ident;
                    xmlnode.Attributes.Append(pw);
                    xmlnode.Attributes.Append(nickname);
                    xmlnode.Attributes.Append(ident);
                    config.AppendChild(xmlnode);

                }
            config.Save(userfile);
            }
        }

        public static void LoadUser()
        {
            if (File.Exists(userfile))
            {
                XmlDocument configuration = new XmlDocument();
                configuration.Load(userfile);
                foreach (XmlNode curr in configuration.ChildNodes)
                {
                    Account line = new Account(curr.Attributes[0].Value, curr.Attributes[1].Value);
                    if (curr.Attributes.Count > 2)
                    {
                        line.nickname = curr.Attributes[2].Value;
                    }
                    _accounts.Add(line);
                    
                }
            }
        }

        public static void SL(string text)
        {
            Console.WriteLine(DateTime.Now.ToString() + ": " + text);
        }

        public static void InitialiseClient(object data)
        {
            System.Net.Sockets.TcpClient client = (System.Net.Sockets.TcpClient)data;
            Connection connection = new Connection();
            ActiveUsers.Add(connection);
            try
            {
                System.Net.Sockets.NetworkStream ns = client.GetStream();

                connection._w = new StreamWriter(ns);
                connection._r = new StreamReader(ns, Encoding.UTF8);

                string text = connection._r.ReadLine();

                connection.Mode = ProtocolMain.Valid(text);
                ProtocolMain protocol = new ProtocolMain(connection);
                try
                {
                    while (connection.Active)
                    {
                        text = connection._r.ReadLine();
                        if (connection.Mode == false)
                        {
                            System.Threading.Thread.Sleep(2000);
                            continue;
                        }

                        if (ProtocolMain.Valid(text))
                        {
                            protocol.parseCommand(text);
                            continue;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(800);
                        }

                    }
                }
                catch (System.IO.IOException)
                {
                    SL("Connection closed");
                    protocol.Exit();
                }
                catch (Exception fail)
                {
                    SL(fail.StackTrace + fail.Message);
                    protocol.Exit();
                }
            }
            catch (System.IO.IOException)
            {
                SL("Connection closed");
            }
            catch (Exception fail)
            {
                SL(fail.StackTrace + fail.Message);
            }
        }

        public static void Listen()
        {
            SL("Pidgeon services loading");

            if (!Directory.Exists("db"))
            {
                Directory.CreateDirectory("db");
            }

            LoadUser();

            SL("Waiting for clients");
            
            System.Net.Sockets.TcpListener server = new System.Net.Sockets.TcpListener(IPAddress.Any, server_port);
            server.Start();

            while (running)
            {
                System.Net.Sockets.TcpClient connection = server.AcceptTcpClient();
                Thread _client = new Thread(InitialiseClient);
                SL("Incoming connection");
                threads.Add(_client);
                _client.Start(connection);
                System.Threading.Thread.Sleep(200);
            }
        }
    }
}
