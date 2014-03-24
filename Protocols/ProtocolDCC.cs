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
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Pidgeon.Protocols
{
    /// <summary>
    /// DCC
    /// </summary>
    public class ProtocolDCC : Protocol, IDisposable
    {
        /// <summary>
        /// Name of user who we are supposed to connect to
        /// </summary>
        public string UserName = null;
        /// <summary>
        /// Thread in which the connection is running
        /// </summary>
        private Thread thread = null;
        private System.Net.Sockets.NetworkStream _NetworkStream = null;
        private System.Net.Security.SslStream _NetworkSsl = null;
        private System.IO.StreamReader _StreamReader = null;
        private object StreamLock = new object();
        private System.IO.StreamWriter _StreamWriter = null;
        /// <summary>
        /// Whether DCC is in listener mode
        /// </summary>
        public bool ListenerMode = false;
        /// <summary>
        /// This describes the type of dcc connection
        /// </summary>
        public DCC Dcc = DCC.Chat;
        private Graphics.Window systemwindow = null;
        private bool disposed = false;

        /// <summary>
        /// Window of this DCC
        /// </summary>
        public override Graphics.Window SystemWindow
        {
            get
            {
                return systemwindow;
            }
        }

        private void main()
        {
            try
            {
                if (ListenerMode)
                {
                    OpenListener();
                    return;
                }

                if (!SSL)
                {
                    _NetworkStream = new System.Net.Sockets.TcpClient(Server, Port).GetStream();
                    _StreamWriter = new System.IO.StreamWriter(_NetworkStream);
                    _StreamReader = new System.IO.StreamReader(_NetworkStream, NetworkEncoding);
                }

                if (SSL)
                {
                    System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(Server, Port);
                    _NetworkSsl = new System.Net.Security.SslStream(client.GetStream(), true,
                        new System.Net.Security.RemoteCertificateValidationCallback(Protocol.ValidateServerCertificate), null);
                    _NetworkSsl.AuthenticateAsClient(Server);
                    _StreamWriter = new System.IO.StreamWriter(_NetworkSsl);
                    _StreamReader = new System.IO.StreamReader(_NetworkSsl, NetworkEncoding);
                }

                Connected = true;
                Parse();
            }
            catch (IOException fail)
            {
                SystemWindow.scrollback.InsertText("Connection closed: " + fail.Message, ContentLine.MessageStyle.System);
                Disconnect();
            }
            catch (ThreadAbortException)
            {
                Disconnect();
                Core.KillThread(thread, true);
                return;
            }
            catch (Exception fail)
            {
                Core.KillThread(thread, true);
                Core.HandleException(fail);
            }
        }

        private void Parse()
        {
            while (!_StreamReader.EndOfStream && IsConnected)
            {
                string text = _StreamReader.ReadLine();
                Core.trafficscanner.Insert(Server, text);
                SystemWindow.scrollback.InsertText(PRIVMSG(UserName, text), ContentLine.MessageStyle.Message);
            }
        }

        private void OpenListener()
        {
            SystemWindow.scrollback.InsertText("Opened a listener for CTCP request on port " + Port.ToString(), ContentLine.MessageStyle.System, false);
            // open the socket
            System.Net.Sockets.TcpListener server = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, Port);
            Connected = true;
            server.Start();
            System.Net.Sockets.TcpClient connection = server.AcceptTcpClient();
            string IP = connection.Client.RemoteEndPoint.ToString();
            systemwindow.scrollback.InsertText("Incoming connection for CTCP request from " + IP, ContentLine.MessageStyle.System);

            if (SSL)
            {
                X509Certificate cert = new X509Certificate2(Configuration.irc.CertificateDCC, "pidgeon");
                System.Net.Security.SslStream __networkSsl = new SslStream(connection.GetStream(), false,
                    new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                __networkSsl.AuthenticateAsServer(cert);
                _StreamWriter = new StreamWriter(__networkSsl);
                _StreamReader = new StreamReader(__networkSsl, Encoding.UTF8);
            }
            else
            {
                System.Net.Sockets.NetworkStream ns = connection.GetStream();
                _StreamWriter = new StreamWriter(ns);
                _StreamReader = new StreamReader(ns, Encoding.UTF8);
            }

            Parse();
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        /// <returns></returns>
        public override bool Disconnect()
        {
            if (!Connected)
            {
                return false;
            }
            if (ListenerMode)
            {
                lock (Core.LockedPorts)
                {
                    if (Core.LockedPorts.Contains((uint)Port))
                    {
                        Core.LockedPorts.Remove((uint)Port);
                    }
                }
            }
            if (!SSL)
            {
                if (_NetworkStream != null)
                {
                    _NetworkStream.Close();
                }
            }
            else
            {
                if (_NetworkSsl != null)
                {
                    _NetworkSsl.Close();
                }
            }
            Connected = false;
            SystemWindow.needIcon = true;
            return true;
        }

        /// <summary>
        /// Exit
        /// </summary>
        public override void Exit()
        {
            Disconnect();
            base.Exit();
        }

        /// <summary>
        /// Releases all resources used by this class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this class
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_StreamReader != null)
                    {
                        _StreamReader.Dispose();
                    }
                    if (_NetworkSsl != null)
                    {
                        _NetworkSsl.Dispose();
                    }
                    if (_StreamWriter != null)
                    {
                        _StreamWriter.Dispose();
                    }
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Parser
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool ParseInput(string input)
        {
            if (!IsConnected)
            {
                return false;
            }
            lock (StreamLock)
            {
                SystemWindow.scrollback.InsertText(PRIVMSG(Configuration.UserData.nick, input), ContentLine.MessageStyle.Message);
                Core.trafficscanner.Insert(Server, input);
                _StreamWriter.WriteLine(input);
                _StreamWriter.Flush();
            }
            return true;
        }

        /// <summary>
        /// Open the connection to foreign client OR open a local listener
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            systemwindow = CreateChat(UserName, false, null, false, null, false, true);
            systemwindow._Protocol = this;
            Core.SystemForm.ChannelList.InsertDcc(this);
            thread = new Thread(main);
            thread.Name = "DCC chat " + UserName;
            Core.SystemThreads.Add(thread);
            thread.Start();
            return true;
        }

        /// <summary>
        /// Describes the type of dcc connection
        /// </summary>
        public enum DCC
        {
            /// <summary>
            /// Chat
            /// </summary>
            Chat,
            /// <summary>
            /// Secure chat
            /// </summary>
            SecureChat,
            /// <summary>
            /// File
            /// </summary>
            File,
            /// <summary>
            /// Secure file
            /// </summary>
            SecureFile
        }
    }
}
