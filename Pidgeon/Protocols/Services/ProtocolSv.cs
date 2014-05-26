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

namespace Pidgeon.Protocols.Services
{
    /// <summary>
    /// Protocol for pidgeon services
    /// </summary>
    public partial class ProtocolSv : Protocol
    {
        private class Cache
        {
            /// <summary>
            /// Size
            /// </summary>
            public double size = 0;
        }
        
        private class Request
        {
            public string Name = null;
            public Type type = Type.ChannelInfo;

            public Request(string name, Type Task)
            {
                Name = name;
                type = Task;
            }

            public enum Type
            {
                ChannelInfo,
            }
        }
        
        private System.Threading.Thread main = null;
        private System.Threading.Thread tPinger = null;
        private object StreamLock = new object();
        private DateTime pong = DateTime.Now;

        private System.Net.Sockets.NetworkStream _networkStream;
        private System.IO.StreamReader _StreamReader = null;
        /// <summary>
        /// List of networks loaded on server
        /// </summary>
        public List<libirc.Network> NetworkList = new List<libirc.Network>();
        private System.IO.StreamWriter _StreamWriter = null;
        public string password = "";
        private List<Cache> cache = new List<Cache>();
        private Status ConnectionStatus = Status.WaitingPW;
        private SslStream _networkSsl = null;
        public Pidgeon.Protocols.Services.Buffer sBuffer = null;
        private List<Request> RemainingJobs = new List<Request>();
        /// <summary>
        /// Whether the services have finished loading
        /// </summary>
        public bool FinishedLoading = false;
        public string Username = "";
        /// <summary>
        /// This needs to be true when the services are in process of disconnecting
        /// </summary>
        private bool IsDisconnecting = false;
        private List<string> WaitingNetw = new List<string>();
        private bool SSL = false;
        public override bool SupportSSL
        {
            get
            {
                return true;
            }
        }
        public override bool UsingSSL
        {
            get
            {
                return SSL;
            }
            set
            {
                SSL = value;
            }
        }
        /// <summary>
        /// Root window
        /// </summary>
        public Graphics.Window SystemWindow = null;

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
                Core.ThreadManager.UnregisterThread(Thread.CurrentThread);
                return;
            }
            catch (Exception fail)
            {
                Core.HandleException(fail, Core.ExceptionKind.Safe);
            }
            Core.ThreadManager.UnregisterThread(Thread.CurrentThread);
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

        private void BacklogMode(bool enable)
        {
            lock (this.NetworkList)
            {
                foreach (libirc.Network network in this.NetworkList)
                {
                    network.IsDownloadingBouncerBacklog = enable;
                }
            }
        }

        public override libirc.IProtocol.Result Command(string cm, libirc.Network network = null)
        {
            if (cm.StartsWith(" ", StringComparison.Ordinal) != true && cm.Contains(" "))
            {
                // uppercase
                string first_word = cm.Substring(0, cm.IndexOf(" ", StringComparison.Ordinal)).ToUpper();
                string rest = cm.Substring(first_word.Length);
                Transfer(first_word + rest, libirc.Defs.Priority.Normal, network);
                return Result.Done;
            }
            Transfer(cm.ToUpper(), libirc.Defs.Priority.Normal, network);
            return Result.Done;
        }

        private void Start()
        {
            try
            {
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }),
                Pidgeon.ContentLine.MessageStyle.System);
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
                Datagram login = new Datagram("AUTH");
                login.Parameters.Add("user", Username);
                login.Parameters.Add("pw", password);
                Deliver(login);
                Deliver(new Datagram("GLOBALNICK"));
                Deliver(new Datagram("NETWORKLIST"));
                Deliver(new Datagram("STATUS"));
                tPinger = new System.Threading.Thread(_Ping);
                tPinger.Name = "Pidgeon:Services/pinger";
                Core.ThreadManager.RegisterThread(tPinger);
                tPinger.Start();
                sBuffer = new Services.Buffer(this);
                Core.SystemForm.Status(getInfo());
                while (!_StreamReader.EndOfStream && Connected)
                {
                    string text = _StreamReader.ReadLine();
                    Core.TrafficScanner.Insert(Server, " >> " + text);
                    while (Core.IsBlocked)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    if (Valid(text))
                    {
                        Process(text);
                    }
                }
            }
            catch (System.IO.IOException fail)
            {
                if (IsConnected && !IsDisconnecting)
                {
                    // we need to wrap this in another exception handler because the following functions are easy to throw some
                    try
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("Quit: " + fail.Message, Pidgeon.ContentLine.MessageStyle.System);
                        Syslog.DebugLog("Clearing the sBuffer to prevent corrupted data being written");
                        sBuffer = null;
                        Disconnect();
                    }
                    catch (Exception f1)
                    {
                        Core.HandleException(f1);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                Core.ThreadManager.UnregisterThread(System.Threading.Thread.CurrentThread);
                return;
            }
            catch (Exception fail)
            {
                if (IsConnected)
                {
                    Core.HandleException(fail);
                }
                Core.ThreadManager.UnregisterThread(System.Threading.Thread.CurrentThread);
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

        private void SendData(string network, string data, libirc.Defs.Priority priority = libirc.Defs.Priority.Normal)
        {
            Datagram line = new Datagram("RAW", data);
            string Pr = "Normal";
            switch (priority)
            {
                case libirc.Defs.Priority.High:
                    Pr = "High";
                    break;
                case libirc.Defs.Priority.Low:
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
        public override Result Part(string name, libirc.Network network = null)
        {
            Transfer("PART " + name, libirc.Defs.Priority.High, network);
            return Result.Done;
        }

        private void Process(string dg)
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
                Syslog.DebugLog("Unable to parse: " + xx.ToString());
                Core.Ringlog("Invalid xml: " + dg);
            }
        }

        /// <summary>
        /// This function remove the network from memory and release all the resources associated with it
        /// </summary>
        /// <param name="network"></param>
        public void RemoveNetworkFromMemory(string network)
        {
            network = network.ToLower();
            lock (this.NetworkList)
            {
                Network network_ = retrieveNetwork(network);
                if (network_ != null)
                {
                    Core.SystemForm.ChannelList.RemoveNetwork(network_);
                    NetworkList.Remove(network_);
                    if (Configuration.Services.UsingCache)
                    {
                        // we need to remove the network here from db
                        if (sBuffer.networkInfo.ContainsKey(network_.ServerName))
                        {
                            sBuffer.networkInfo.Remove(network_.ServerName);
                        }

                        if (sBuffer.Networks.ContainsKey(network_.ServerName))
                        {
                            sBuffer.Networks.Remove(network_.ServerName);
                        }
                    }
                    network_.IsConnected = false;
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
                if (IsConnected)
                {
                    Disconnect();
                }
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
                Syslog.DebugLog("Remaining jobs are now cleared");
                if (sBuffer == null)
                {
                    Syslog.DebugLog("Warning sBuffer == null");
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
                        Syslog.DebugLog("Didn't write the network cache because the services were still waiting on " + remaining.ToString() + " requests");
                    }
                }
                else
                {
                    Syslog.DebugLog("Didn't write the network cache because it is disallowed");
                }
                lock (NetworkList)
                {
                    foreach (Network network in NetworkList)
                    {
                        if (Core.SelectedNetwork == network)
                        {
                            Core.SelectedNetwork = null;
                        }
                    }
                    NetworkList.Clear();
                }
                Core.ThreadManager.KillThread(main);
                Core.ThreadManager.KillThread(tPinger);
                tPinger = null;
                Core.SystemForm.DisplayingProgress = false;
                if (sBuffer != null)
                {
                    sBuffer.Wipe();
                }
                sBuffer = null;
                if (_StreamWriter != null) _StreamWriter.Close();
                if (_StreamReader != null) _StreamReader.Close();
                base.Exit();
            }
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        /// <returns></returns>
        public override Result Disconnect()
        {
            lock (this)
            {
                if (!IsConnected)
                {
                    Syslog.DebugLog("User attempted to disconnect services that are already disconnected");
                    return Result.Failure;
                }
                IsDisconnecting = true;
                if (System.Threading.Thread.CurrentThread != main)
                {
                    Core.ThreadManager.KillThread(main);
                }
                Core.ThreadManager.KillThread(tPinger);
                tPinger = null;
                lock (NetworkList)
                {
                    foreach (Network network in NetworkList)
                    {
                        network.IsConnected = false;
                    }
                }
                try
                {
                    if (_StreamWriter != null) _StreamWriter.Close();
                    if (_StreamReader != null) _StreamReader.Close();
                    if (SSL)
                    {
                        if (_networkSsl != null) _networkSsl.Close();
                    }
                    else
                    {
                        if (_networkStream != null) _networkStream.Close();
                    }
                }
                catch (System.Net.Sockets.SocketException fail)
                {
                    Syslog.DebugLog("Problem when disconnecting from network " + Server + ": " + fail.ToString());
                }
                Connected = false;
                IsDisconnecting = false;
            }
            return Result.Done;
        }

        /// <summary>
        /// Send a datagram to server
        /// </summary>
        /// <param name="message"></param>
        public void Deliver(Datagram message)
        {
            Send(message.ToDocumentXmlText());
        }

        public override System.Threading.Thread Open()
        {
            SystemWindow = WindowsManager.CreateChat("!root", true, null, false, null, false, true, this);
            SystemWindow._Protocol = this;
            Core.SystemForm.ChannelList.InsertSv(this);
            main = new System.Threading.Thread(Start);
            main.Name = "Pidgeon:Services/Main";
            Core.ThreadManager.RegisterThread(main);
            main.Start();
            return main;
        }

        /// <summary>
        /// Request a global nick
        /// </summary>
        /// <param name="_Nick"></param>
        /// <returns></returns>
        public Result RequestGlobalNick(string _Nick)
        {
            Deliver(new Datagram("GLOBALNICK", _Nick));
            return Result.Done;
        }

        /// <summary>
        /// Self
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="_priority"></param>
        /// <returns></returns>
        public override Result Act(string text, string to, libirc.Network network, libirc.Defs.Priority _priority = libirc.Defs.Priority.Normal)
        {
            //Core.SystemForm.Chat.scrollback.InsertText(Configuration.CurrentSkin.Message2 + Core.SelectedNetwork.Nickname + " " + text, Pidgeon.ContentLine.MessageStyle.Action, true, 0, true);
            Transfer("PRIVMSG " + to + " :" + this.Separator.ToString() + "ACTION " + text + this.Separator.ToString(), _priority);
            return Result.Done;
        }

        public override void DebugLog(string Text, int Verbosity)
        {
            base.DebugLog(Text, Verbosity);
        }

        public override libirc.IProtocol.Result Message(string text, string to, libirc.Network network, libirc.Defs.Priority priority = libirc.Defs.Priority.Normal)
        {
            Datagram message = new Datagram("MESSAGE", text);
            if (network != null && NetworkList.Contains(network))
            {
                message.Parameters.Add("network", network.ServerName);
                message.Parameters.Add("priority", priority.ToString());
                message.Parameters.Add("to", to);
                Deliver(message);
                return Result.Queued;
            }
            else
            {
                Syslog.DebugLog("Invalid network for message to: " + to);
                return Result.Failure;
            }
        }

        /// <summary>
        /// Create a remote connection to server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public override Result ConnectTo(string server, int port)
        {
            server = server.Trim();
            // in case that server is invalid, we ignore the request
            if (string.IsNullOrEmpty(server))
            {
                return Result.Failure;
            }
            // We also ignore it if we aren't connected to services
            if (ConnectionStatus == Status.Connected)
            {
                SystemWindow.scrollback.InsertText("Connecting to " + server, Pidgeon.ContentLine.MessageStyle.User, true);
                Datagram request = new Datagram("CONNECT", server);
                request.Parameters.Add("port", port.ToString());
                Deliver(request);
            }
            return Result.Done;
        }

        /// <summary>
        /// Check if it's a valid data
        /// </summary>
        /// <param name="datagram"></param>
        /// <returns></returns>
        public static bool Valid(string datagram)
        {
            if (datagram.StartsWith("<", StringComparison.Ordinal) && datagram.EndsWith(">", StringComparison.Ordinal))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disconnect from a specific network on bouncer side
        /// </summary>
        /// <param name="network"></param>
        public void DisconnectRemote(Network network)
        {
            Transfer("QUIT :" + network.Quit, libirc.Defs.Priority.High, network);
            Datagram request = new Datagram("REMOVE", network.ServerName);
            Deliver(request);
        }

        private void SafeDc(string reason)
        {
            try
            {
                SystemWindow.scrollback.InsertText(reason, Pidgeon.ContentLine.MessageStyle.User);
                Disconnect();
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
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
                    lock (StreamLock)
                    {
                        _StreamWriter.WriteLine(text);
                        Core.TrafficScanner.Insert(Server, " << " + text);
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
                        Core.HandleException(f);
                    }
                    else
                    {
                        Syslog.DebugLog("ex " + f.ToString());
                    }
                }
            }
            else
            {
                Syslog.DebugLog("ERROR: Can't send a datagram because connection to " + Server + " is closed");
            }
        }

        /// <summary>
        /// Join a channel
        /// </summary>
        /// <param name="name"></param>
        /// <param name="network"></param>
        public override Result Join(string name, libirc.Network network = null)
        {
            Transfer("JOIN " + name);
            return Result.Done;
        }

        /// <summary>
        /// Send a raw data to irc server you pick, you should always fill in network, or current network will be used
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="priority">priority</param>
        /// <param name="network">This value must be filled in - it's not required because some old functions do not provide it</param>
        public override Result Transfer(string text, libirc.Defs.Priority priority = libirc.Defs.Priority.Normal, libirc.Network network = null)
        {
            if (network == null)
            {
                if (Core.SelectedNetwork != null && NetworkList.Contains(Core.SelectedNetwork))
                {
                    Datagram data = new Datagram("RAW", text);
                    data.Parameters.Add("network", Core.SelectedNetwork.ServerName);
                    Deliver(data);
                    return Result.Done;
                }
            }
            else
            {
                if (NetworkList.Contains(network))
                {
                    Datagram data = new Datagram("RAW", text);
                    data.Parameters.Add("network", network.ServerName);
                    Deliver(data);
                    return Result.Done;
                }
                else
                {
                    Syslog.DebugLog("Network is not a part of this services connection " + network.ServerName);
                    return Result.Failure;
                }
            }
            return Result.Failure;
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
