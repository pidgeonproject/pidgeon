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
using System.Linq;
using System.Text;

namespace Client
{
    class ProtocolSv : Protocol
    {
        public System.Threading.Thread main;
        public System.Threading.Thread deliveryqueue;
        public System.Threading.Thread keep;
        public DateTime pong = DateTime.Now;

        private System.Net.Sockets.NetworkStream _network;
        private System.IO.StreamReader _reader;
        public List<Network> sl = new List<Network>();
        private System.IO.StreamWriter _writer;
        public string password = "";
        private Status ConnectionStatus = Status.WaitingPW;
        public string nick = "";
        public bool auth = false;

        public class Datagram
        {
            public Datagram(string Name, string Text = "")
            {
                _Datagram = Name;
                if (Text != "")
                {
                    _InnerText = System.Web.HttpUtility.HtmlEncode(Text);
                }
                _InnerText = Text;
            }
            public string _InnerText;
            public string _Datagram;
            public Dictionary<string, string> Parameters = new Dictionary<string,string>();
        }

        public enum Status
        {
            WaitingPW,
            Connected,
        }

        public void _Ping()
        {
            while (true)
            {
                Deliver(new Datagram("PING"));
                System.Threading.Thread.Sleep(480000);
            }
        }

        public void Start()
        {
            Core._Main.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }), Scrollback.MessageStyle.System);
            try
            {
                _network = new System.Net.Sockets.TcpClient(Server, Port).GetStream();

                _writer = new System.IO.StreamWriter(_network);
                _reader = new System.IO.StreamReader(_network, Encoding.UTF8);

                Deliver(new Datagram("PING"));
                Deliver(new Datagram("LOAD"));
                Datagram login = new Datagram("AUTH", "");
                login.Parameters.Add("user", nick);
                login.Parameters.Add("pw", password);
                Deliver(login);
                Deliver(new Datagram("NICK", nick));
                Deliver(new Datagram("STATUS"));
                

                keep = new System.Threading.Thread(_Ping);
                keep.Name = "pinger thread";
                keep.Start();

            }
            catch (Exception b)
            {
                Core._Main.Chat.scrollback.InsertText(b.Message, Scrollback.MessageStyle.System);
                return;
            }
            string text = "";
            try
            {

                while (!_reader.EndOfStream)
                {
                    text = _reader.ReadLine();
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
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public override void Part(string name, Network network = null)
        {
            Transfer("PART " + name);
        }

        public bool Process(string dg)
        { 
            System.Xml.XmlDocument datagram = new System.Xml.XmlDocument();
            datagram.LoadXml(dg);
            foreach (System.Xml.XmlNode curr in datagram.ChildNodes)
            {
                switch (curr.Name.ToUpper())
                { 
                    case "SLOAD":
                        windows["!root"].scrollback.InsertText(curr.InnerText, Scrollback.MessageStyle.System, false);
                        break;
                    case "SRAW":
                        if (curr.InnerText == "PERMISSIONDENY")
                        {
                            windows["!root"].scrollback.InsertText("You can't send this command to server, because you aren't logged in", Scrollback.MessageStyle.System, false);
                            break;
                        }
                        windows["!root"].scrollback.InsertText("Server responded to SRAW with this: " + curr.InnerText, Scrollback.MessageStyle.User, false);
                        break;
                    case "SSTATUS":
                        switch (curr.InnerText)
                        { 
                            case "Connected":
                                ConnectionStatus = Status.Connected;
                                break;
                            case "WaitingPW":
                                ConnectionStatus = Status.WaitingPW;
                                break;
                        }
                        break;
                    case "SNETWORKLIST":
                        if (curr.InnerText != "")
                        {
                            string[] _networks = curr.InnerText.Split('|');
                            foreach (string i in _networks)
                            {
                                if (i != "")
                                { 
                                    sl.Add(new Network(i, this));
                                }
                            }
                        }
                        break;
                    case "SAUTH":
                        if (curr.InnerText == "INVALID")
                        {
                            windows["!root"].scrollback.InsertText("You have supplied wrong password, connection closed", Scrollback.MessageStyle.System, false);
                            Exit();
                        }
                        if (curr.InnerText == "OK")
                        {
                            windows["!root"].scrollback.InsertText("You are now logged in to pidgeon bnc", Scrollback.MessageStyle.System, false);
                        }
                        break;
                }
            }
            return true;
        }

        public override void Exit()
        {
            Deliver(new Datagram( "QUIT" ));
        }

        public void Deliver(Datagram message)
        {
            string text = "";
            string dl = "";

            foreach (KeyValuePair<string, string> curr in message.Parameters)
            {
                dl += " " + curr.Key + "=\"" + System.Web.HttpUtility.HtmlEncode(curr.Value) + "\"";
            }

            text = "<" + message._Datagram + dl + ">" + message._InnerText + "</" + message._Datagram + ">";


            Send(text);
        }

        public override bool Open()
        {
            CreateChat("!root", true, null);
            main = new System.Threading.Thread(Start);
            main.Start();
            return true;
        }

        public bool Valid(string datagram)
        {
            if (datagram.StartsWith("<") && datagram.EndsWith(">"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Write raw data
        /// </summary>
        /// <param name="text"></param>
        public void Send(string text)
        {
            _writer.WriteLine(text);
            _writer.Flush();
        }

        public override void Transfer(string text, Configuration.Priority Pr = Configuration.Priority.Normal)
        {
            Deliver(new Datagram("RAW", text));
        }
    }
}
