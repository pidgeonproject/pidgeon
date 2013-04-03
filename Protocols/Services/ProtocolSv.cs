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
using System.Text;
using System.Net.Sockets;
using System.Xml;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Client
{
    [Serializable]
    public partial class ProtocolSv : Protocol
    {
        public class Cache
        {
            public double size = 0;
            public double current = 0;
        }
        
        public class Work
        {
            public string Name = null;
            public Type type = Type.ChannelInfo;
            public Work(string name, Type Task)
            {
                Name = name;
                type = Task;
            }
            
            public enum Type
            {
                ChannelInfo,
            }
        }
        
        public System.Threading.Thread main = null;
        public System.Threading.Thread keep = null;
        public DateTime pong = DateTime.Now;

        private System.Net.Sockets.NetworkStream _networkStream;
        private System.IO.StreamReader _StreamReader = null;
        /// <summary>
        /// List of networks loaded on server
        /// </summary>
        public List<Network> NetworkList = new List<Network>();
        private System.IO.StreamWriter _StreamWriter = null;
        public string password = "";
        public List<Cache> cache = new List<Cache>();
        public Status ConnectionStatus = Status.WaitingPW;
        private SslStream _networkSsl = null;
        public Services.Buffer sBuffer = null;
        public List<Work> RemainingJobs = new List<Work>();

        public string nick = "";
        public bool auth = false;
        

        public List<string> WaitingNetw = new List<string>();

        public enum Status
        {
            WaitingPW,
            Connected,
        }

        public void _Ping()
        {
            try
            {
                while (Connected)
                {
                    Deliver(new Datagram("PING"));
                    System.Threading.Thread.Sleep(480000);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (Exception)
            {
                Core.DebugLog("ProtocolSv: Exception in Ping()");
            }
        }

        public string getInfo()
        {
            if (RemainingJobs.Count > 0)
            {
                return "Waiting for services to finish " + RemainingJobs.Count.ToString() + " requests";
            }
            return "";
        }
        
        public override bool Command(string cm, Network network = null)
        {
            if (cm.StartsWith(" ") != true && cm.Contains(" "))
            {
                // uppercase
                string first_word = cm.Substring(0, cm.IndexOf(" ")).ToUpper();
                string rest = cm.Substring(first_word.Length);
                Transfer(first_word + rest, Configuration.Priority.Normal, network);
                return true;
            }
            Transfer(cm.ToUpper(), Configuration.Priority.Normal, network);
            return true;
        }

        public void Start()
        {
            try
            {
                Core._Main.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }),
                Client.ContentLine.MessageStyle.System);

                sBuffer = new Services.Buffer(this);
                _networkStream = new System.Net.Sockets.TcpClient(Server, Port).GetStream();

                _StreamWriter = new System.IO.StreamWriter(_networkStream);
                _StreamReader = new System.IO.StreamReader(_networkStream, Encoding.UTF8);

                if (!SSL)
                {
                    _networkStream = new System.Net.Sockets.TcpClient(Server, Port).GetStream();
                    _StreamWriter = new System.IO.StreamWriter(_networkStream);
                    _StreamReader = new System.IO.StreamReader(_networkStream, Encoding.UTF8);
                }

                if (SSL)
                {
                    System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(Server, Port);
                    _networkSsl = new System.Net.Security.SslStream(client.GetStream(), true,
                        new System.Net.Security.RemoteCertificateValidationCallback(Protocol.ValidateServerCertificate), null);
                    _StreamWriter = new System.IO.StreamWriter(_networkSsl);
                    _StreamReader = new System.IO.StreamReader(_networkSsl, Encoding.UTF8);
                }

                Connected = true;

                Deliver(new Datagram("PING"));
                Deliver(new Datagram("LOAD"));
                
                Datagram login = new Datagram("AUTH", "");
                login.Parameters.Add("user", nick);
                login.Parameters.Add("pw", password);
                Deliver(login);
                Deliver(new Datagram("GLOBALNICK"));
                Deliver(new Datagram("NETWORKLIST"));
                Deliver(new Datagram("STATUS"));


                keep = new System.Threading.Thread(_Ping);
                Core.SystemThreads.Add(keep);
                keep.Name = "pinger thread";
                keep.Start();
            }
            catch (Exception b)
            {
                Core._Main.Chat.scrollback.InsertText(b.Message, Client.ContentLine.MessageStyle.System);
                return;
            }
            string text = "";
            try
            {
                while (!_StreamReader.EndOfStream && Connected)
                {
                    text = _StreamReader.ReadLine();
                    Core.trafficscanner.insert(Server, " >> " + text);
                    while (Core.blocked)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    if (Valid(text))
                    {
                        Process(text);
                        continue;
                    }
                }
            }
            catch (System.IO.IOException fail)
            {
                if (Connected)
                {
                    Core._Main.Chat.scrollback.InsertText("Quit: " + fail.Message, Client.ContentLine.MessageStyle.System);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                if (Core.IgnoreErrors)
                {
                    return;
                }
            }
            catch (Exception fail)
            {
                if (Connected)
                {
                    Core.handleException(fail);
                }
                Core.killThread(System.Threading.Thread.CurrentThread);
            }
        }

        private Network retrieveNetwork(string server)
        {
            foreach (Network i in NetworkList)
            {
                if (i.ServerName == server)
                {
                    return i;
                }
            }
            return null;
        }

        public void SendData(string network, string data, Configuration.Priority priority = Configuration.Priority.Normal)
        {
            Datagram line = new Datagram("RAW", data);
            string Pr = "Normal";
            switch (priority)
            {
                case Configuration.Priority.High:
                    Pr = "High";
                    break;
                case Configuration.Priority.Low:
                    Pr = "Low";
                    break;
            }
            line.Parameters.Add("network", network);
            line.Parameters.Add("priority", Pr);
            Deliver(line);
        }

        public override void Part(string name, Network network = null)
        {
            Transfer("PART " + name, Configuration.Priority.High, network);
        }

        public bool Process(string dg)
        {
            try
            {
                System.Xml.XmlDocument datagram = new System.Xml.XmlDocument();
                datagram.LoadXml(dg);
                foreach (XmlNode curr in datagram.ChildNodes)
                {
                    switch (curr.Name.ToUpper())
                    {
                        case "SMESSAGE":
                            ResponsesSv.sMessage(curr, this);
                            break;
                        case "SLOAD":
                            ResponsesSv.sLoad(curr, this);
                            break;
                        case "SRAW":
                            ResponsesSv.sRaw(curr, this);
                            break;
                        case "SSTATUS":
                            ResponsesSv.sStatus(curr, this);
                            break;
                        case "SDATA":
                            ResponsesSv.sData(curr, this);
                            break;
                        case "SNICK":
                            ResponsesSv.sNick(curr, this);
                            break;
                        case "SREMOVE":
                            ResponsesSv.sRemove(curr, this);
                            break;
                        case "SCONNECT":
                            ResponsesSv.sConnect(curr, this);
                            break;
                        case "SGLOBALIDENT":
                            ResponsesSv.sGlobalident(curr, this);
                            break;
                        case "SBACKLOG":
                            ResponsesSv.sBacklog(curr, this);
                            break;
                        case "SGLOBALNICK":
                            ResponsesSv.sGlobalNick(curr, this);
                            break;
                        case "SNETWORKINFO":
                            ResponsesSv.sNetworkInfo(curr, this);
                            break;
                        case "SNETWORKLIST":
                            ResponsesSv.sNetworkList(curr, this);
                            break;
                        case "SCHANNELINFO":
                            ResponsesSv.sChannelInfo(curr, this);
                            break;
                        case "SAUTH":
                            ResponsesSv.sAuth(curr, this);
                            break;
                        case "SFAIL":
                            ResponsesSv.sError(curr, this);
                            break;
                    }
                }
            }
            catch (System.Xml.XmlException xx)
            {
                Core.DebugLog("Unable to parse: " + xx.ToString());
                Core.Ringlog("Invalid xml: " + dg);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
        }

        /// <summary>
        /// This will close the protocol, that mean it will release all objects and memory, you should only call it when you want to remove this
        /// </summary>
        public override void Exit()
        {
            if (!Connected)
            {
                Core.DebugLog("Request to disconnect from services that aren't connected");
                return;
            }
            Connected = false;
            if (Configuration.Services.UsingCache)
            {
                sBuffer.Snapshot();
            }
            lock (NetworkList)
            {
                foreach (Network network in NetworkList)
                {
                    network.Connected = false;
					network.Destroy();
                    if (Core.network == network)
                    {
                        Core.network = null;
                    }
                }
                NetworkList.Clear();
            }
            if (keep != null)
            {
                keep.Abort();
                Core.killThread(keep);
            }
			
			if (sBuffer != null)
			{
				sBuffer.Networks.Clear();
				sBuffer.networkInfo.Clear();
			}
			
			sBuffer = null;
			
            if (_StreamWriter != null) _StreamWriter.Close();
            if (_StreamReader != null) _StreamReader.Close();
            base.Exit();
        }

        public override bool Disconnect()
        {
            if (!Connected)
            {
                Core.DebugLog("User attempted to disconnect services that are already disconnected");
                return false;
            }
            Connected = false;
            try
            {
                if (_StreamWriter != null) _StreamWriter.Close();
                if (_StreamReader != null) _StreamReader.Close();
            }
            catch (System.Net.Sockets.SocketException fail)
            {
                Core.DebugLog("Problem when disconnecting from network " + Server + ": " + fail.ToString());
            }
            lock (NetworkList)
            {
                foreach (Network xx in NetworkList)
                {
                    // we need to flag all networks here as disconnected so that it knows we can't use them
                    xx.Connected = false;
                }
            }
            if (keep != null && (keep.ThreadState == System.Threading.ThreadState.Running || keep.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
            {
                keep.Abort();
            }
            return true;
        }

        public void Deliver(Datagram message)
        {
            Send(message.ToDocumentXmlText());
        }

        public override bool Open()
        {
            ProtocolType = 3;
            CreateChat("!root", true, null);
            main = new System.Threading.Thread(Start);
            Core._Main.ChannelList.insertSv(this);
            Core.SystemThreads.Add(main);
            main.Start();
            return true;
        }

        public override int requestNick(string _Nick, Network network = null)
        {
            Deliver(new Datagram("GLOBALNICK", _Nick));
            return 0;
        }

        public override int Message2(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            Core._Main.Chat.scrollback.InsertText(">>>>>>" + Core.network.Nickname + " " + text, Client.ContentLine.MessageStyle.Action);
            Transfer("PRIVMSG " + to + " :" + delimiter.ToString() + "ACTION " + text + delimiter.ToString(), _priority);
            return 0;
        }

        public override int Message(string text, string to, Network network, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            Datagram message = new Datagram("MESSAGE", text);
            if (network != null && NetworkList.Contains(network))
            {
                message.Parameters.Add("network", network.ServerName);
                message.Parameters.Add("priority", _priority.ToString());
                message.Parameters.Add("to", to);
                Deliver(message);
            }
            else
            {
                Core.DebugLog("Invalid network for message to: " + to);
            }
            return 0;
        }

        /// <summary>
        /// Create a remote connection to server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public override bool ConnectTo(string server, int port)
        {
            // remove space
            while (server.StartsWith(" "))
            {
                server = server.Substring(1);
            }
            // in case that server is invalid, we ignore the request
            if (server == "")
            {
                return false;
            }
            // We also ignore it if we aren't connected to services
            if (ConnectionStatus == Status.Connected)
            {
                Windows["!root"].scrollback.InsertText("Connecting to " + server, Client.ContentLine.MessageStyle.User, true);
                Datagram request = new Datagram("CONNECT", server);
                request.Parameters.Add("port", port.ToString());
                Deliver(request);
            }
            return true;
        }

        /// <summary>
        /// Check if it's a valid data
        /// </summary>
        /// <param name="datagram"></param>
        /// <returns></returns>
        public bool Valid(string datagram)
        {
            if (datagram.StartsWith("<") && datagram.EndsWith(">"))
            {
                return true;
            }
            return false;
        }

        public void Disconnect(Network network)
        {
            Transfer("QUIT :" + network.Quit, Configuration.Priority.High, network);
            Datagram request = new Datagram("REMOVE", network.ServerName);
            Deliver(request);
        }

        /// <summary>
        /// Write raw data
        /// </summary>
        /// <param name="text"></param>
        public void Send(string text)
        {
            if (Connected)
            {
                try
                {
                    _StreamWriter.WriteLine(text);
                    Core.trafficscanner.insert(Server, " << " + text);
                    _StreamWriter.Flush();
                }
                catch (System.IO.IOException er)
                {
                    Windows["!root"].scrollback.InsertText(er.Message, Client.ContentLine.MessageStyle.User);
                    Exit();
                }
                catch (Exception f)
                {
                    if (Connected)
                    {
                        Core.handleException(f);
                    }
                }
            }
        }

        public override void Join(string name, Network network = null)
        {
            Transfer("JOIN " + name);
        }

        /// <summary>
        /// Send a raw data to irc server you pick, you should always fill in network, or current network will be used
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="priority">priority</param>
        /// <param name="network">This value must be filled in - it's not required because some old functions do not provide it</param>
        public override void Transfer(string text, Configuration.Priority priority = Configuration.Priority.Normal, Network network = null)
        {
            if (network == null)
            {
                if (Core.network != null && NetworkList.Contains(Core.network))
                {
                    Datagram data = new Datagram("RAW", text);
                    data.Parameters.Add("network", Core.network.ServerName);
                    Deliver(data);
                    return;
                }
            }
            else
            {
                if (NetworkList.Contains(network))
                {
                    Datagram data = new Datagram("RAW", text);
                    data.Parameters.Add("network", network.ServerName);
                    Deliver(data);
                }
                else
                {
                    Core.DebugLog("Network is not a part of this services connection " + network.ServerName);
                }
            }
        }
    }
}
