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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Client
{
    /// <summary>
    /// Protocol
    /// </summary>
    [Serializable]
    public class ProtocolIrc : Protocol
    {
        /// <summary>
        /// Thread in which the connection to server is handled
        /// </summary>
        public System.Threading.Thread main = null;
        /// <summary>
        /// Thread which is handling the delivery of data
        /// </summary>
        public System.Threading.Thread deliveryqueue = null;
        /// <summary>
        /// Thread which is keeping the connection online
        /// </summary>
        public System.Threading.Thread keep = null;
        /// <summary>
        /// Time of last ping
        /// </summary>
        public DateTime pong = DateTime.Now;
        /// <summary>
        /// Network stream
        /// </summary>
        private NetworkStream _networkStream = null;
        /// <summary>
        /// SSL
        /// </summary>
        private SslStream _networkSsl = null;
        /// <summary>
        /// Stream reader for server
        /// </summary>
        private System.IO.StreamReader _StreamReader = null;
        /// <summary>
        /// Network associated with this connection (we have only 1 network in direct connection)
        /// </summary>
        public Network _IRCNetwork;
        /// <summary>
        /// Stream writer for server
        /// </summary>
        private System.IO.StreamWriter _StreamWriter = null;
        /// <summary>
        /// Messages
        /// </summary>
        private MessagesClass Messages = new MessagesClass();
        /// <summary>
        /// Loging in using sasl
        /// </summary>
        public bool UsingSasl = false;

        class MessagesClass
        {
            /// <summary>
            /// Message
            /// </summary>
            public struct Message
            {
                /// <summary>
                /// Priority
                /// </summary>
                public Configuration.Priority _Priority;
                /// <summary>
                /// Message
                /// </summary>
                public string message;
            }
            /// <summary>
            /// List of all messages that can be processed
            /// </summary>
            public List<Message> messages = new List<Message>();
            /// <summary>
            /// List of all new messages that need to be parsed
            /// </summary>
            public List<Message> newmessages = new List<Message>();
            /// <summary>
            /// Protocol
            /// </summary>
            public ProtocolIrc protocol = null;

            /// <summary>
            /// Send a message to server
            /// </summary>
            /// <param name="Message"></param>
            /// <param name="Pr"></param>
            public void DeliverMessage(string Message, Configuration.Priority Pr = Configuration.Priority.Normal)
            {
                Message text = new Message();
                text._Priority = Pr;
                text.message = Message;
                lock (messages)
                {
                    messages.Add(text);
                    return;
                }
            }

            public void Run()
            {
                try
                {
                    while (protocol.IsConnected)
                    {
                        try
                        {
                            if (messages.Count > 0)
                            {
                                lock (messages)
                                {
                                    newmessages.AddRange(messages);
                                    messages.Clear();
                                }
                            }
                            if (newmessages.Count > 0)
                            {
                                List<Message> Processed = new List<Message>();
                                Configuration.Priority highest = Configuration.Priority.Low;
                                while (newmessages.Count > 0)
                                {
                                    // we need to get all messages that have been scheduled to be send
                                    lock (messages)
                                    {
                                        if (messages.Count > 0)
                                        {
                                            newmessages.AddRange(messages);
                                            messages.Clear();
                                        }
                                    }
                                    highest = Configuration.Priority.Low;
                                    // we need to check the priority we need to handle first
                                    foreach (Message message in newmessages)
                                    {
                                        if (message._Priority > highest)
                                        {
                                            highest = message._Priority;
                                            if (message._Priority == Configuration.Priority.High)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    // send highest priority first
                                    foreach (Message message in newmessages)
                                    {
                                        if (message._Priority >= highest)
                                        {
                                            Processed.Add(message);
                                            protocol.Send(message.message);
                                            System.Threading.Thread.Sleep(1000);
                                            if (highest != Configuration.Priority.High)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    foreach (Message message in Processed)
                                    {
                                        if (newmessages.Contains(message))
                                        {
                                            newmessages.Remove(message);
                                        }
                                    }
                                }
                            }
                            newmessages.Clear();
                            System.Threading.Thread.Sleep(200);
                        }
                        catch (System.Threading.ThreadAbortException)
                        {
                            Core.KillThread(System.Threading.Thread.CurrentThread);
                            return;
                        }
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {
                    Core.KillThread(System.Threading.Thread.CurrentThread);
                    return;
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                    return;
                }
            }
        }

        /// <summary>
        /// Part
        /// </summary>
        /// <param name="name">Channel</param>
        /// <param name="network"></param>
        public override void Part(string name, Network network = null)
        {
            Transfer("PART " + name, Configuration.Priority.High, network);
        }

        /// <summary>
        /// Transfer
        /// </summary>
        /// <param name="text"></param>
        /// <param name="priority"></param>
        /// <param name="network"></param>
        public override void Transfer(string text, Configuration.Priority priority = Configuration.Priority.Normal, Network network = null)
        {
            Messages.DeliverMessage(text, priority);
        }

        private void _Ping()
        {
            try
            {
                while (_IRCNetwork.IsConnected && IsConnected)
                {
                    Transfer("PING :" + _IRCNetwork._Protocol.Server, Configuration.Priority.High);
                    System.Threading.Thread.Sleep(24000);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return;
            }
            catch (Exception fail)
            {
                Core.handleException(fail, Core.ExceptionKind.Safe);
                Core.KillThread(System.Threading.Thread.CurrentThread);
            }
        }

        private void Start()
        {
            try
            {
                Messages.protocol = this;
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }),
                    Client.ContentLine.MessageStyle.System);
            
                if (!SSL)
                {
                    _networkStream = new System.Net.Sockets.TcpClient(Server, Port).GetStream();
                    _StreamWriter = new System.IO.StreamWriter(_networkStream);
                    _StreamReader = new System.IO.StreamReader(_networkStream, NetworkEncoding);
                }

                if (SSL)
                {
                    System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(Server, Port);
                    _networkSsl = new System.Net.Security.SslStream(client.GetStream(), true,
                        new System.Net.Security.RemoteCertificateValidationCallback(Protocol.ValidateServerCertificate), null);
                    _networkSsl.AuthenticateAsClient(Server);
                    _StreamWriter = new System.IO.StreamWriter(_networkSsl);
                    _StreamReader = new System.IO.StreamReader(_networkSsl, NetworkEncoding);
                }

                Hooks._Protocol.BeforeConnect(this);
                _IRCNetwork.flagConnection();

                Connected = true;

                if (Password != "")
                {
                    Send("PASS " + Password);
                }

                Send("USER " + _IRCNetwork.Ident + " 8 * :" + _IRCNetwork.UserName);
                Send("NICK " + _IRCNetwork.Nickname);

                Core.SystemForm.Status("");

                keep = new System.Threading.Thread(_Ping);
                keep.Name = "pinger thread";
                keep.Start();
                Core.SystemThreads.Add(keep);

            }
            catch (Exception b)
            {
                Core.SystemForm.Chat.scrollback.InsertText(b.Message, Client.ContentLine.MessageStyle.System);
                return;
            }
            string text = "";
            try
            {
                deliveryqueue = new System.Threading.Thread(Messages.Run);
                deliveryqueue.Start();

                while (_IRCNetwork.IsConnected && !_StreamReader.EndOfStream && IsConnected)
                {
                    while (Core.IsBlocked)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    text = _StreamReader.ReadLine();
                    Core.trafficscanner.Insert(Server, " >> " + text);
                    ProcessorIRC processor = new ProcessorIRC(_IRCNetwork, text, ref pong);
                    processor.ProfiledResult();
                    pong = processor.pong;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (System.Net.Sockets.SocketException)
            {
                SafeDc();
            }
            catch (System.IO.IOException)
            {
                SafeDc();
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
            Core.KillThread(System.Threading.Thread.CurrentThread);
            return;
        }

        private void SafeDc()
        {
            try
            {
                SystemWindow.scrollback.InsertText("Disconnected", Client.ContentLine.MessageStyle.User);
                Core.SystemForm.Status("Disconnected from server " + Server);
                _IRCNetwork.flagDisconnect();
                Connected = false;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Command
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public override bool Command(string cm, Network network = null)
        {
            try
            {
                if (cm.StartsWith(" ") != true && cm.Contains(" "))
                {
                    // uppercase
                    string first_word = cm.Substring(0, cm.IndexOf(" ")).ToUpper();
                    string rest = cm.Substring(first_word.Length);
                    Transfer(first_word + rest);
                    return true;
                }
                Transfer(cm.ToUpper());
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
            return false;
        }

        private void Send(string ms)
        {
            try
            {
                if (!IsConnected)
                {
                    Core.DebugLog("NETWORK: attempt to send a packet to disconnected network");
                    return;
                }
                lock (_StreamWriter)
                {
                    _StreamWriter.WriteLine(ms);
                    Core.trafficscanner.Insert(Server, " << " + ms);
                    _StreamWriter.Flush();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Send a message either to channel or user
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="priority"></param>
        /// <param name="pmsg"></param>
        /// <returns></returns>
        public override int Message(string text, string to, Configuration.Priority priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            Message(text, to, null, priority, pmsg);
            return 0;
        }

        /// <summary>
        /// Send a message either to channel or user
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="network"></param>
        /// <param name="priority"></param>
        /// <param name="pmsg"></param>
        /// <returns></returns>
        public override int Message(string text, string to, Network network, Configuration.Priority priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            if (!pmsg)
            {
                Core.SystemForm.Chat.scrollback.InsertText(Core.SelectedNetwork._Protocol.PRIVMSG(_IRCNetwork.Nickname, text), Client.ContentLine.MessageStyle.Message, true, 0, true);
            }
            Transfer("PRIVMSG " + to + " :" + text, priority);
            return 0;
        }

        /// <summary>
        /// /me style
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public override int Message2(string text, string to, Configuration.Priority priority = Configuration.Priority.Normal)
        {
            Core.SystemForm.Chat.scrollback.InsertText(">>>>>>" + _IRCNetwork.Nickname + " " + text, Client.ContentLine.MessageStyle.Action, true, 0, true);
            Transfer("PRIVMSG " + to + " :" + delimiter.ToString() + "ACTION " + text + delimiter.ToString(), priority);
            return 0;
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        /// <returns></returns>
        public override bool Disconnect()
        {
            // we lock the function so that it can't be called in same time in different thread
            lock (this)
            {
                if (!IsConnected)
                {
                    return false;
                }
                try
                {
                    Send("QUIT :" + _IRCNetwork.Quit);
                    _IRCNetwork.flagDisconnect();
                    Core.KillThread(deliveryqueue);
                    Core.KillThread(keep);
                    if (SSL)
                    {
                        _networkSsl.Close();
                    }
                    else
                    {
                        _networkStream.Close();
                    }
                    _StreamWriter.Close();
                    _StreamReader.Close();
                    Connected = false;
                }
                catch (System.IO.IOException er)
                {
                    SystemWindow.scrollback.InsertText(er.Message, Client.ContentLine.MessageStyle.User);
                    Connected = false;
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }
            return true;
        }

        /// <summary>
        /// Join
        /// </summary>
        /// <param name="name">Channel</param>
        /// <param name="network"></param>
        public override void Join(string name, Network network = null)
        {
            Transfer("JOIN " + name);
        }

        /// <summary>
        /// Request nick
        /// </summary>
        /// <param name="_Nick"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public override int requestNick(string _Nick, Network network = null)
        {
            Transfer("NICK " + _Nick);
            return 0;
        }

        /// <summary>
        /// Reconnect network
        /// </summary>
        /// <param name="network"></param>
        public override void ReconnectNetwork(Network network)
        {
            if (IsConnected)
            {
                Disconnect();
            }
            if (main != null)
            {
                Core.KillThread(main);
            }
            main = new System.Threading.Thread(Start);
            Core.SystemForm.Status("Connecting to server " + Server + " port " + Port.ToString());
            main.Start();
            Core.SystemThreads.Add(main);
        }

        /// <summary>
        /// Destroy this instance of protocol and release all objects
        /// </summary>
        public override void Exit()
        {
            if (IsDestroyed)
            {
                Core.DebugLog("This object is already destroyed " + Server);
                return;
            }
            if (!Hooks._Network.BeforeExit(_IRCNetwork))
            {
                return;
            }
            if (IsConnected)
            {
                Disconnect();
            }
            Core.KillThread(main);
            _IRCNetwork.Destroy();
            Connected = false;
            System.Threading.Thread.Sleep(200);
            SystemWindow.scrollback.InsertText("You have disconnected from network", Client.ContentLine.MessageStyle.System);
            if (Core.SelectedNetwork == _IRCNetwork)
            {
                Core.SelectedNetwork = null;
            }
            destroyed = true;
            base.Exit();
        }

        /// <summary>
        /// Connect
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            main = new System.Threading.Thread(Start);
            Core.SystemForm.Status("Connecting to server " + Server + " port " + Port.ToString());
            main.Start();
            Core.SystemThreads.Add(main);
            return true;
        }
    }
}
