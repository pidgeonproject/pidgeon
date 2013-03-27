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
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Client
{
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
        private SslStream _networkSsl = null;
        /// <summary>
        /// Stream reader for server
        /// </summary>
        private System.IO.StreamReader _StreamReader;
        /// <summary>
        /// Network associated with this connection (we have only 1 network in direct connection)
        /// </summary>
        public Network _IRCNetwork;
        /// <summary>
        /// Stream writer for server
        /// </summary>
        private System.IO.StreamWriter _StreamWriter;
        /// <summary>
        /// Messages
        /// </summary>
        private MessagesClass Messages = new MessagesClass();

        class MessagesClass
        {
            public struct Message
            {
                public Configuration.Priority _Priority;
                public string message;
            }
            public List<Message> messages = new List<Message>();
            public List<Message> newmessages = new List<Message>();
            public ProtocolIrc protocol;

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
                while (true)
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
                        Core.killThread(System.Threading.Thread.CurrentThread);
                        return;
                    }
                }
            }
        }

        public override void Part(string name, Network network = null)
        {
            Transfer("PART " + name, Configuration.Priority.High, network);
        }

        public override void Transfer(string text, Configuration.Priority Pr = Configuration.Priority.Normal, Network network = null)
        {
            Messages.DeliverMessage(text, Pr);
        }

        public void _Ping()
        {
            try
            {
                while (_IRCNetwork.Connected)
                {
                    Transfer("PING :" + _IRCNetwork._Protocol.Server, Configuration.Priority.High);
                    System.Threading.Thread.Sleep(24000);
                }
            }
            catch (Exception)
            {
                Core.killThread(System.Threading.Thread.CurrentThread);
            }
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void Start()
        {
            Messages.protocol = this;
            Core._Main.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }),
                Scrollback.MessageStyle.System);
            try
            {
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
                        new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    _StreamWriter = new System.IO.StreamWriter(_networkSsl);
                    _StreamReader = new System.IO.StreamReader(_networkSsl, Encoding.UTF8);
                }

                Hooks._Protocol.BeforeConnect(this);
                _IRCNetwork.Connected = true;

                Connected = true;

                Send("USER " + _IRCNetwork.Ident + " 8 * :" + _IRCNetwork.UserName);
                Send("NICK " + _IRCNetwork.Nickname);
                if (Password != "")
                {
                    Send("PASS " + Password);
                }

                Core._Main.Status("");

                keep = new System.Threading.Thread(_Ping);
                keep.Name = "pinger thread";
                keep.Start();
                Core.SystemThreads.Add(keep);

            }
            catch (Exception b)
            {
                Core._Main.Chat.scrollback.InsertText(b.Message, Scrollback.MessageStyle.System);
                return;
            }
            string text = "";
            try
            {
                deliveryqueue = new System.Threading.Thread(Messages.Run);
                deliveryqueue.Start();


                while (_IRCNetwork.Connected && !_StreamReader.EndOfStream)
                {
                    while (Core.blocked)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    text = _StreamReader.ReadLine();
                    Core.trafficscanner.insert(Server, " >> " + text);
                    ProcessorIRC processor = new ProcessorIRC(_IRCNetwork, text, ref pong);
                    processor.Result();
                    pong = processor.pong;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                return;
            }
            catch (System.IO.IOException)
            {
                SystemWindow.scrollback.InsertText("Disconnected", Scrollback.MessageStyle.User);
                Core._Main.Status("Disconnected from server " + Server);
                Exit();
            }
            catch (Exception ex)
            {
                Core.handleException(ex);
            }
            Core.killThread(System.Threading.Thread.CurrentThread);
            return;
        }

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
                _StreamWriter.WriteLine(ms);
                Core.trafficscanner.insert(Server, " << " + ms);
                _StreamWriter.Flush();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public override int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            Message(text, to, null, _priority, pmsg);
            return 0;
        }

        public override int Message(string text, string to, Network network, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            if (!pmsg)
            {
                Core._Main.Chat.scrollback.InsertText(Core.network._Protocol.PRIVMSG(_IRCNetwork.Nickname, text), Scrollback.MessageStyle.Message, true, 0, true);
            }
            Transfer("PRIVMSG " + to + " :" + text, _priority);
            return 0;
        }

        /// <summary>
        /// /me style
        /// </summary>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="_priority"></param>
        /// <returns></returns>
        public override int Message2(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            Core._Main.Chat.scrollback.InsertText(">>>>>>" + _IRCNetwork.Nickname + " " + text, Scrollback.MessageStyle.Action, true, 0, true);
            Transfer("PRIVMSG " + to + " :" + delimiter.ToString() + "ACTION " + text + delimiter.ToString(), _priority);
            return 0;
        }

        public override void Join(string name, Network network = null)
        {
            Transfer("JOIN " + name);
        }

        public override int requestNick(string _Nick, Network network = null)
        {
            Transfer("NICK " + _Nick);
            return 0;
        }

        public override void Exit()
        {
            if (!_IRCNetwork.Connected)
            {
                return;
            }
            if (!Hooks._Network.BeforeExit(_IRCNetwork))
            {
                return;
            }
            try
            {
                Send("QUIT :" + _IRCNetwork.Quit);
            }
            catch (Exception) { }
            _IRCNetwork.Connected = false;
            System.Threading.Thread.Sleep(200);
            deliveryqueue.Abort();
            keep.Abort();
            if (main.ThreadState == System.Threading.ThreadState.Running)
            {
                main.Abort();
            }
            SystemWindow.scrollback.InsertText("You have disconnected from network", Scrollback.MessageStyle.System);
            if (Core.network == _IRCNetwork)
            {
                Core.network = null;
            }
            base.Exit();
            return;
        }

        public override bool Open()
        {
            main = new System.Threading.Thread(Start);
            ProtocolType = 2;
            Core._Main.Status(messages.get("connecting", Core.SelectedLanguage));
            main.Start();
            Core.SystemThreads.Add(main);
            return true;
        }
    }
}
