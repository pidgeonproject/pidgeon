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
using System.Threading;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;

namespace pidgeon_sv
{
    public class Item
    {
        public DateTime _date;
        string _text;
        public Item(string Data)
        {
            _date = DateTime.Now;
            _text = Data;
        }
    }

    public class OutgoingQueue
    {

        List<Item> queue = new List<Item>();
    }

    public class IncomingQueue
    {
        List<Item> queue = new List<Item>();
    }

    public class Connection
    {
        public string name = null;
        public string host = null;
        public static List<Connection> ActiveUsers = new List<Connection>();
        public string user = null;
        public Account account = null;
        public Status status = Status.WaitingPW;
        public Thread queue = null;
        public System.Net.Sockets.TcpClient client = null;
        public System.IO.StreamReader _r = null;
        public System.IO.StreamWriter _w = null;
        public OutgoingQueue _outgoing = new OutgoingQueue();
        public IncomingQueue _incoming = new IncomingQueue();
        public Thread main = null;
        public string IP = "";
        public bool working = true;

        public bool Active = true;

        public enum Status {
            WaitingPW,
            Connected,
            Disconnected,
        }

        public bool Mode = false;

        public Connection()
        { 
            
        }

        public static void ConnectionKiller(object data)
        {
            try
            {
                Connection conn = (Connection)data;
                if (conn.main != null)
                {
                    Thread.Sleep(60000);
                    if (conn.status == Connection.Status.WaitingPW)
                    {
                        Core.SL("Failed to authenticate in time - killing connection " + conn.IP);
                        conn.main.Abort();
                        lock (ActiveUsers)
                        {
                            if (ActiveUsers.Contains(conn))
                            {
                                Core.SL("DEBUG: Connection was not properly terminated! Trying again " + conn.IP);
                            }
                        }
                        ConnectionClean(conn);
                        return;
                    }
                    if (!conn.working)
                    {
                        Core.SL("Failed to respond in time - killing connection " + conn.IP);
                        conn.main.Abort();
                        lock (ActiveUsers)
                        {
                            if (ActiveUsers.Contains(conn))
                            {
                                Core.SL("DEBUG: Connection was not properly terminated! Trying again " + conn.IP);
                            }
                        }
                        ConnectionClean(conn);
                        return;
                    }
                    else
                    {
                        conn.working = false;
                    }
                }
                else
                {
                    Core.SL("DEBUG: NULL " + conn.IP);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public static void InitialiseClient(object data)
        {
            System.Net.Sockets.TcpClient client = (System.Net.Sockets.TcpClient)data;
            Connection connection = new Connection();
            connection.main = Thread.CurrentThread;
            Core.SL("Opening a new connection to " + client.Client.RemoteEndPoint.ToString());
            try
            {
                connection.client = client;
                connection.IP = client.Client.RemoteEndPoint.ToString();
                Thread checker = new Thread(ConnectionKiller);
                checker.Name = "watcher";
                checker.Start(connection);
                lock (ActiveUsers)
                {
                    ActiveUsers.Add(connection);
                }
                System.Net.Sockets.NetworkStream ns = client.GetStream();

                connection._w = new StreamWriter(ns);
                connection._r = new StreamReader(ns, Encoding.UTF8);

                string text = connection._r.ReadLine();

                connection.Mode = ProtocolMain.Valid(text);
                ProtocolMain protocol = new ProtocolMain(connection);
                try
                {
                    while (connection.Active && !connection._r.EndOfStream)
                    {
                        try
                        {
                            text = connection._r.ReadLine();
                            if (text == "" || text == null)
                            {
                                break;
                            }
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
                                Core.SL("Debug: invalid text: " + text + " from " + client.Client.RemoteEndPoint.ToString());
                                System.Threading.Thread.Sleep(800);
                            }
                        }
                        catch (IOException)
                        {
                            Core.SL("Connection closed: " + connection.IP);
                            protocol.Exit();
                            ConnectionClean(connection);
                            return;

                        }
                        catch (ThreadAbortException)
                        {
                            protocol.Exit();
                            ConnectionClean(connection);
                            return;
                        }
                        catch (Exception fail)
                        {
                            Core.handleException(fail);
                        }
                    }
                    Core.SL("Connection closed by remote: " + connection.IP);
                    protocol.Exit();
                    ConnectionClean(connection);
                }
                catch (System.IO.IOException)
                {
                    Core.SL("Connection closed: " + connection.IP);
                    protocol.Exit();
                    ConnectionClean(connection);
                    return;
                }
                catch (ThreadAbortException)
                {
                    protocol.Exit();
                    ConnectionClean(connection);
                    return;
                }
                catch (Exception fail)
                {
                    Core.SL(fail.StackTrace + fail.Message);
                    protocol.Exit();
                    ConnectionClean(connection);
                    return;
                }
            }
            catch (System.IO.IOException)
            {
                Core.SL("Connection closed: " + connection.IP);
                ConnectionClean(connection);
                return;
            }
            catch (ThreadAbortException)
            {
                Core.SL("Connection closed");
                ConnectionClean(connection);
                return;
            }
            catch (Exception fail)
            {
                Core.SL(fail.StackTrace + fail.Message);
                ConnectionClean(connection);
                return;
            }
        }

        public static void ConnectionClean(Connection connection)
        {
            try
            {
                Core.SL("Cleaning data for connection: " + connection.IP);
                lock (ActiveUsers)
                {
                    if (ActiveUsers.Contains(connection))
                    {
                        ActiveUsers.Remove(connection);
                        Core.SL("Data for connection were cleaned: " + connection.IP);
                        connection.client.Client.Close();
                        connection.client.Close();
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
