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
using System.Text;

namespace Client
{
    public class ProtocolDCC : IProtocol
    {
        private Thread thread = null;
        private System.Net.Sockets.NetworkStream _networkStream = null;
        private System.Net.Security.SslStream _networkSsl = null;
        private System.IO.StreamReader _StreamReader = null;
        private System.IO.StreamWriter _StreamWriter = null;
        public DCC Dcc = DCC.Chat;

        private void main()
        {
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
                        new System.Net.Security.RemoteCertificateValidationCallback(Protocol.ValidateServerCertificate), null);
                    _networkSsl.AuthenticateAsClient(Server);
                    _StreamWriter = new System.IO.StreamWriter(_networkSsl);
                    _StreamReader = new System.IO.StreamReader(_networkSsl, Encoding.UTF8);
                }
            }
            catch (ThreadAbortException)
            {
                Core.KillThread(thread, true);
                return;
            }
            catch (Exception fail)
            {
                Core.KillThread(thread, true);
                Core.handleException(fail);
            }
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
            Connected = false;
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

        public override bool Open()
        {
            thread = new Thread(main);
            Core.SystemThreads.Add(thread);
            thread.Start();
            return true;
        }

        public enum DCC
        {
            Chat,
            SecureChat,
            File,
            SecureFile
        }
    }
}
