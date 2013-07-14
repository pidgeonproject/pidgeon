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
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Xml;
using System.Threading;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Client
{
    /// <summary>
    /// Protocol for pidgeon services
    /// </summary>
    [Serializable]
    public partial class ProtocolSv : Protocol
    {
        private class Cache
        {
            /// <summary>
            /// Size
            /// </summary>
            public double size = 0;
            /// <summary>
            /// Current pointer
            /// </summary>
            public double current = 0;
        }
        
        private class Request
        {
            /// <summary>
            /// Name
            /// </summary>
            public string Name = null;
            public Type type = Type.ChannelInfo;

            public Request(string name, Type Task)
            {
                Name = name;
                type = Task;
            }
            
            /// <summary>
            /// Type
            /// </summary>
            public enum Type
            {
                /// <summary>
                /// Channel info
                /// </summary>
                ChannelInfo,
            }
        }
        
        private System.Threading.Thread main = null;
        private System.Threading.Thread keep = null;
        private DateTime pong = DateTime.Now;

        private System.Net.Sockets.NetworkStream _networkStream;
        private System.IO.StreamReader _StreamReader = null;
        /// <summary>
        /// List of networks loaded on server
        /// </summary>
        public List<Network> NetworkList = new List<Network>();
        private System.IO.StreamWriter _StreamWriter = null;
        /// <summary>
        /// Password
        /// </summary>
        public string password = "";
        private List<Cache> cache = new List<Cache>();
        private Status ConnectionStatus = Status.WaitingPW;
        private SslStream _networkSsl = null;
        /// <summary>
        /// Buffer
        /// </summary>
        public Services.Buffer sBuffer = null;
        private List<Request> RemainingJobs = new List<Request>();
        /// <summary>
        /// Whether the services have finished loading
        /// </summary>
        public bool FinishedLoading = false;
        /// <summary>
        /// Nickname
        /// </summary>
        public string nick = "";
        /// <summary>
        /// This needs to be true when the services are in process of disconnecting
        /// </summary>
        private bool disconnecting = false;
        private List<string> WaitingNetw = new List<string>();

        /// <summary>
        /// Root window
        /// </summary>
        public override Graphics.Window SystemWindow
        {
            get
            {
                lock (Windows)
                {
                    if (Windows.ContainsKey("!root"))
                    {
                        return Windows["!root"];
                    }
                }
                return null;
            }
        }

        private void _Ping()
        {
            try
            {
                while (IsConnected)
                {
                    Deliver(new Datagram("PING"));
                    System.Threading.Thread.Sleep(480000);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (Exception fail)
            {
                Core.handleException(fail, Core.ExceptionKind.Safe);
            }
        }

        private string getInfo()
        {
            if (RemainingJobs.Count > 0)
            {
                this.FinishedLoading = false;
                return "Waiting for services to finish " + RemainingJobs.Count.ToString() + " requests";
            }
            this.FinishedLoading = true;
            return "";
        }
        
        /// <summary>
        /// Send a command to network
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="network"></param>
        /// <returns></returns>
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

        private void Start()
        {
            try
            {
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }),
                Client.ContentLine.MessageStyle.System);

                Core.SystemForm.Status("Connecting to " + Server);

                if (!SSL)
                {
                    _networkStream = new System.Net.Sockets.TcpClient(Server, Port).GetStream();
                    _StreamWriter = new System.IO.StreamWriter(_networkStream);
                    _StreamReader = new System.IO.StreamReader(_networkStream, Encoding.UTF8);
                }

                if (SSL)
                {
                    System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(Server, Port);
                    _networkSsl = new System.Net.Security.SslStream(client.GetStream(), false,
                        new System.Net.Security.RemoteCertificateValidationCallback(Protocol.ValidateServerCertificate), null);
                    _networkSsl.AuthenticateAsClient(Server);
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
            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (Exception b)
            {
                Core.SystemForm.Chat.scrollback.InsertText(b.Message, Client.ContentLine.MessageStyle.System);
                return;
            }
            string text = "";
            try
            {
                sBuffer = new Services.Buffer(this);
                Core.SystemForm.Status(getInfo());
                while (!_StreamReader.EndOfStream && Connected)
                {
                    text = _StreamReader.ReadLine();
                    Core.trafficscanner.Insert(Server, " >> " + text);
                    while (Core.IsBlocked)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    if (Valid(text))
                    {
                        // if this return false the thread must be stopped now
                        if (!Process(text))
                        {
                            return;
                        }
                        continue;
                    }
                }
            }
            catch (System.IO.IOException fail)
            {
                if (IsConnected && !disconnecting)
                {
                    // we need to wrap this in another exception handler because the following functions are easy to throw some
                    try
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("Quit: " + fail.Message, Client.ContentLine.MessageStyle.System);
                        Core.DebugLog("Clearing the sBuffer to prevent corrupted data being written");
                        sBuffer = null;
                        Disconnect();
                    }
                    catch (Exception f1)
                    {
                        Core.handleException(f1);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                Core.KillThread(System.Threading.Thread.CurrentThread, true);
                return;
            }
            catch (Exception fail)
            {
                if (IsConnected)
                {
                    Core.handleException(fail);
                }
                Core.KillThread(System.Threading.Thread.CurrentThread);
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

        private void SendData(string network, string data, Configuration.Priority priority = Configuration.Priority.Normal)
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

        /// <summary>
        /// Part a channel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="network"></param>
        public override void Part(string name, Network network = null)
        {
            Transfer("PART " + name, Configuration.Priority.High, network);
        }

        private bool Process(string dg)
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
                        case "SUSERLIST":
                            ResponsesSv.sUserList(curr, this);
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
            catch (ThreadAbortException)
            {
                return false;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
        }

        /// <summary>
        /// This function remove the network from memory and release all the resources associated with it
        /// </summary>
        /// <param name="network"></param>
        public void RemoveNetworkFromMemory(string network)
        {
            network = network.ToLower();
            Network remove = null;
            lock (Core.SystemForm.ChannelList.ServerList)
            {
                foreach (KeyValuePair<Network, Gtk.TreeIter> n in Core.SystemForm.ChannelList.ServerList)
                {
                    if (n.Key.ServerName.ToLower() == network)
                    {
                        remove = n.Key;
                        if (remove != null)
                        {
                            Core.SystemForm.ChannelList.RemoveServer(remove);
                        }
                        else
                        {
                            Core.DebugLog("Unable to remove " + network);
                        }
                        break;
                    }
                }
            }

            lock (NetworkList)
            {
                if (remove == null)
                {
                    foreach (Network xx in NetworkList)
                    {
                        if (network == xx.ServerName.ToLower())
                        {
                            remove = xx;
                            break;
                        }
                    }
                }

                if (remove != null)
                {
                    if (NetworkList.Contains(remove))
                    {
                        NetworkList.Remove(remove);
                    }

                    if (Configuration.Services.UsingCache)
                    {
                        // we need to remove the network here from db
                        if (sBuffer.networkInfo.ContainsKey(remove.ServerName))
                        {
                            sBuffer.networkInfo.Remove(remove.ServerName);
                        }

                        if (sBuffer.Networks.ContainsKey(remove.ServerName))
                        {
                            sBuffer.Networks.Remove(remove.ServerName);
                        }
                    }

                    remove.flagDisconnect();
                    remove.Destroy();
                }
            }
        }

        /// <summary>
        /// This will close the protocol, that mean it will release all objects and memory, you should only call it when you want to remove this
        /// </summary>
        public override void Exit()
        {
            lock (this)
            {
                if (IsDestroyed)
                {
                    return;
                }
                if (IsConnected)
                {
                    Disconnect();
                }
                disconnecting = true;
                int remaining = 0;
                lock (RemainingJobs)
                {
                    remaining = RemainingJobs.Count;
                    if (RemainingJobs.Count == 0)
                    {
                        FinishedLoading = true;
                    }
                    RemainingJobs.Clear();
                }
                Core.DebugLog("Remaining jobs are now cleared");
                if (sBuffer == null)
                {
                    Core.DebugLog("Warning sBuffer == null");
                }
                if (Configuration.Services.UsingCache && sBuffer != null)
                {
                    if (FinishedLoading)
                    {
                        Core.SystemForm.Status("Writing the service cache");
                        sBuffer.Snapshot();
                        Core.SystemForm.Status("Done");
                    }
                    else
                    {
                        Core.DebugLog("Didn't write the network cache because the services were still waiting on " + remaining.ToString() + " requests");
                    }
                }
                else
                {
                    Core.DebugLog("Didn't write the network cache because it is disallowed");
                }
                lock (NetworkList)
                {
                    foreach (Network network in NetworkList)
                    {
                        network.Destroy();
                        if (Core.SelectedNetwork == network)
                        {
                            Core.SelectedNetwork = null;
                        }
                    }
                    NetworkList.Clear();
                }

                if (keep != null)
                {
                    Core.KillThread(keep);
                    keep = null;
                }

                if (sBuffer != null)
                {
                    sBuffer.Destroy();
                }

                sBuffer = null;

                if (_StreamWriter != null) _StreamWriter.Close();
                if (_StreamReader != null) _StreamReader.Close();
                _StreamWriter = null;
                _StreamReader = null;
                base.Exit();
                destroyed = true;
            }
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        /// <returns></returns>
        public override bool Disconnect()
        {
            lock (this)
            {
                disconnecting = true;
                if (!IsConnected)
                {
                    Core.DebugLog("User attempted to disconnect services that are already disconnected");
                    return false;
                }
                if (System.Threading.Thread.CurrentThread != main)
                {
                    Core.KillThread(main);
                }
                lock (NetworkList)
                {
                    foreach (Network network in NetworkList)
                    {
                        network.flagDisconnect();
                    }
                }
                try
                {
                    if (_StreamWriter != null) _StreamWriter.Close();
                    if (_StreamReader != null) _StreamReader.Close();
                    _StreamWriter = null;
                    _StreamReader = null;
                    if (SSL)
                    {
                        if (_networkSsl != null)
                        {
                            _networkSsl.Close();
                            _networkSsl.Dispose();
                            _networkSsl = null;
                        }
                    }
                    else
                    {
                        if (_networkStream != null)
                        {
                            _networkStream.Close();
                            _networkStream.Dispose();
                            _networkStream = null;
                        }
                    }
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
                        xx.flagDisconnect();
                    }
                }
                if (keep != null)
                {
                    Core.KillThread(keep);
                }
                Connected = false;
                disconnecting = false;
            }
            return true;
        }

        /// <summary>
        /// Send a datagram to server
        /// </summary>
        /// <param name="message"></param>
        public void Deliver(Datagram message)
        {
            Send(message.ToDocumentXmlText());
        }

        /// <summary>
        /// Open
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            CreateChat("!root", true, null, false, null, false, true);
            Core.SystemForm.ChannelList.InsertSv(this);
            main = new System.Threading.Thread(Start);
            Core.SystemThreads.Add(main);
            main.Start();
            return true;
        }

        /// <summary>
        /// Request a global nick
        /// </summary>
        /// <param name="_Nick"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public override int requestNick(string _Nick, Network network = null)
        {
            Deliver(new Datagram("GLOBALNICK", _Nick));
            return 0;
        }

        /// <summary>
        /// Self
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="_priority"></param>
        /// <returns></returns>
        public override int Message2(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            Core.SystemForm.Chat.scrollback.InsertText(">>>>>>" + Core.SelectedNetwork.Nickname + " " + text, Client.ContentLine.MessageStyle.Action);
            Transfer("PRIVMSG " + to + " :" + delimiter.ToString() + "ACTION " + text + delimiter.ToString(), _priority);
            return 0;
        }

        /// <summary>
        /// Send a message to network
        /// </summary>
        /// <param name="text">Text of message</param>
        /// <param name="to">Who is supposed to receive it</param>
        /// <param name="network">Network where it is sent</param>
        /// <param name="_priority">Priority</param>
        /// <param name="pmsg">Whether it is supposed to be considered a private message</param>
        /// <returns></returns>
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
                SystemWindow.scrollback.InsertText("Connecting to " + server, Client.ContentLine.MessageStyle.User, true);
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

        /// <summary>
        /// Disconnect from a specific network on bouncer side
        /// </summary>
        /// <param name="network"></param>
        public void Disconnect(Network network)
        {
            Transfer("QUIT :" + network.Quit, Configuration.Priority.High, network);
            Datagram request = new Datagram("REMOVE", network.ServerName);
            Deliver(request);
        }

        private void SafeDc(string reason)
        {
            try
            {
                SystemWindow.scrollback.InsertText(reason, Client.ContentLine.MessageStyle.User);
                Disconnect();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Write raw data
        /// </summary>
        /// <param name="text"></param>
        public void Send(string text)
        {
            if (IsConnected)
            {
                try
                {
                    lock (_StreamWriter)
                    {
                        _StreamWriter.WriteLine(text);
                        Core.trafficscanner.Insert(Server, " << " + text);
                        _StreamWriter.Flush();
                    }
                }
                catch (System.IO.IOException er)
                {
                    SafeDc(er.Message);
                }
                catch (Exception f)
                {
                    if (IsConnected)
                    {
                        Core.handleException(f);
                    }
                    else
                    {
                        Core.DebugLog("ex " + f.ToString());
                    }
                }
            }
            else
            {
                Core.DebugLog("ERROR: Can't send a datagram because connection to " + Server + " is closed");
            }
        }

        /// <summary>
        /// Join a channel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="network"></param>
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
                if (Core.SelectedNetwork != null && NetworkList.Contains(Core.SelectedNetwork))
                {
                    Datagram data = new Datagram("RAW", text);
                    data.Parameters.Add("network", Core.SelectedNetwork.ServerName);
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

        /// <summary>
        /// Current status of services
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// Waiting for a password
            /// </summary>
            WaitingPW,
            /// <summary>
            /// Everything work
            /// </summary>
            Connected,
        }
    }
}
