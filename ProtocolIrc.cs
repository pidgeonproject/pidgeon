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
using System.Linq;
using System.Net;

namespace Client
{
    class ProtocolIrc : Protocol
    {
        public System.Threading.Thread main;
        public System.Threading.Thread deliveryqueue;
        public System.Threading.Thread keep;
        public DateTime pong = DateTime.Now;

        private System.Net.Sockets.NetworkStream _network;
        private System.IO.StreamReader _reader;
        public Network _server;
        private System.IO.StreamWriter _writer;
        Messages _messages = new Messages();

        class Messages
        {
            public struct Message
            {
                public Configuration.Priority _Priority;
                public string message;
            }
            public List<Message> messages = new List<Message>();
            public List<Message> newmessages = new List<Message>();
            public ProtocolIrc protocol;

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
            Transfer("PART " + name);
        }

        public override void Transfer(string text, Configuration.Priority Pr = Configuration.Priority.Normal)
        {
            _messages.DeliverMessage(text, Pr);
        }

        public void _Ping()
        {
            try
            {
                while (_server.Connected)
                {
                    Transfer("PING :" + _server._protocol.Server, Configuration.Priority.High);
                    System.Threading.Thread.Sleep(24000);
                }
            }
            catch (Exception)
            {
                Core.killThread(System.Threading.Thread.CurrentThread);
            }
        }

        public void Start()
        {
            _messages.protocol = this;
            Core._Main.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }),
                Scrollback.MessageStyle.System);
            try
            {
                _network = new System.Net.Sockets.TcpClient(Server, Port).GetStream();

                Hooks.BeforeIRCConnect(this);
                _server.Connected = true;

                _writer = new System.IO.StreamWriter(_network);
                _reader = new System.IO.StreamReader(_network, Encoding.UTF8);

                Connected = true;

                Send("USER " + _server.ident + " 8 * :" + _server.username);
                Send("NICK " + _server.nickname);
                if (pswd != "")
                {
                    Send("PASS " + pswd);
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
                deliveryqueue = new System.Threading.Thread(_messages.Run);
                deliveryqueue.Start();


                while (_server.Connected && !_reader.EndOfStream)
                {
                    while (Core.blocked)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    text = _reader.ReadLine();
                    Core.trafficscanner.insert(Server, " >> " + text);
                    ProcessorIRC processor = new ProcessorIRC(_server, this, text, _server.server, _server.sw, ref pong);
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
                windows["!system"].scrollback.InsertText("Disconnected", Scrollback.MessageStyle.User);
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

        public override bool Command(string cm)
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
                _writer.WriteLine(ms);
                Core.trafficscanner.insert(Server, " << " + ms);
                _writer.Flush();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public override int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal, bool pmsg = false)
        {
            if (!pmsg)
            {
                Core._Main.Chat.scrollback.InsertText(Core.network._protocol.PRIVMSG(_server.nickname, text), Scrollback.MessageStyle.Message, true, 0, true);
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
            Core._Main.Chat.scrollback.InsertText(">>>>>>" + _server.nickname + " " + text, Scrollback.MessageStyle.Action, true, 0, true);
            Transfer("PRIVMSG " + to + " :" + delimiter.ToString() + "ACTION " + text + delimiter.ToString(), _priority);
            return 0;
        }

        public override void Join(string name, Network network = null)
        {
            Transfer("JOIN " + name);
        }

        public override int requestNick(string _Nick)
        {
            Transfer("NICK " + _Nick);
            return 0;
        }

        public override void Exit()
        {
            if (!_server.Connected)
            {
                return;
            }
            if (!Hooks.BeforeExit(_server))
            {
                return;
            }
            try
            {
                Send("QUIT :" + _server.quit);
            }
            catch (Exception) { }
            _server.Connected = false;
            System.Threading.Thread.Sleep(200);
            deliveryqueue.Abort();
            keep.Abort();
            if (main.ThreadState == System.Threading.ThreadState.Running)
            {
                main.Abort();
            }
            windows["!system"].scrollback.InsertText("You have disconnected from network", Scrollback.MessageStyle.System);
            if (Core.network == _server)
            {
                Core.network = null;
            }
            base.Exit();
            return;
        }

        public override bool Open()
        {
            main = new System.Threading.Thread(Start);
            type = 2;
            Core._Main.Status(messages.get("connecting", Core.SelectedLanguage));
            main.Start();
            Core.SystemThreads.Add(main);
            return true;
        }
    }
}
