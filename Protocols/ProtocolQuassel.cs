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
using System.Text;

namespace Client
{
    public class Quassel_Parser
    {
        private string Input = "";

        public void Proccess()
        {
            switch (Input)
            { 
                case "error":
                    break;
            }
        }

        public Quassel_Parser(ProtocolQuassel parent, string input)
        { 
            
        }
    }

    /// <summary>
    /// Quassel protocol
    /// </summary>
    public class ProtocolQuassel : Protocol
    {
        private System.Threading.Thread _Thread = null;
        private System.Net.Sockets.NetworkStream _networkStream;
        private System.Net.Security.SslStream _networkSsl;
        private System.IO.StreamReader _StreamReader;
        private System.IO.StreamWriter _StreamWriter;
        private Graphics.Window sw = null;
        /// <summary>
        /// Password
        /// </summary>
        public string password = "";
        /// <summary>
        /// Name
        /// </summary>
        public string name = "";

        /// <summary>
        /// System window
        /// </summary>
        public override Graphics.Window SystemWindow
        {
            get
            {
                return sw;
            }
        }

        /// <summary>
        /// Exit
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            return;
        }

        private void Start()
        {
            try
            {
                Core._Main.Status(messages.get("connecting", Core.SelectedLanguage));

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

                while (IsConnected && !_StreamReader.EndOfStream)
                {
                    try
                    {
                        string text = null;
                        while (Core.blocked)
                        {
                            System.Threading.Thread.Sleep(100);
                        }

                        text = _StreamReader.ReadLine();
                        Core.trafficscanner.insert(Server, " >> " + text);
                        Quassel_Parser parser = new Quassel_Parser(this, text);
                        parser.Proccess();
                    }
                    catch (IOException fail)
                    {
                        SystemWindow.scrollback.InsertText("Disconnected: " + fail.Message.ToString(), ContentLine.MessageStyle.System, false, 0);
                        Disconnect();
                    }
                }
            }
            catch (Exception h)
            {
                Core.handleException(h);
                Disconnect();
            }
        }

        public override bool Command(string cm, Network network = null)
        {
            Send(cm);
            return true;
        }

        private void Send(string ms)
        {
            try
            {
                if (IsConnected)
                {
                    _StreamWriter.WriteLine(ms);
                    Core.trafficscanner.insert(Server, " << " + ms);
                    _StreamWriter.Flush();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Connect to quassel core
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            sw = CreateChat("!root", true, null);
            Core._Main.ChannelList.InsertQuassel(this);
            _Thread = new System.Threading.Thread(Start);
            _Thread.Name = "Quassel main";
            Core.SystemThreads.Add(_Thread);
            _Thread.Start();
            return true;
        }
    }
}
