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

// This window is being used for ctcp requests for direct connect protocol

using System;

namespace Client.Forms
{
    /// <summary>
    /// Open form
    /// </summary>
    public partial class OpenDCC : Client.GTK.PidgeonForm
    {
        /// <summary>
        /// Listener mode
        /// </summary>
        public bool ListenerMode = false;
        /// <summary>
        /// Server
        /// </summary>
        private string Server = null;
        /// <summary>
        /// Port
        /// </summary>
        private uint Port = 0;
        private string username = null;
        private Network network;
        private bool SSL;
        private ProtocolDCC.DCC type = ProtocolDCC.DCC.Chat;

        private void _click(object sender, EventArgs e)
        {
            if (!ListenerMode)
            {
                Core.ConnectDcc(Server, (int)Port, null, type, false, Configuration.UserData.nick, SSL, username);
                Hide();
                Destroy();
                return;
            }
            Core.ConnectDcc("localhost", (int)Port, null, type, true, Configuration.UserData.nick, SSL, username);
            string message = "DCC CHAT chat " + Core.GetIP() + " " + Port.ToString();
            if (SSL)
            {
                message = "DCC SECURECHAT securechat " + Core.GetIP() + " " + Port.ToString();
            }
            if (Configuration.irc.DisplayCtcp)
            {
                network.SystemWindow.scrollback.InsertText("[CTCP] " + username + ": " + message, ContentLine.MessageStyle.User);
            }
            network.Transfer("PRIVMSG " + username + " :" + network._Protocol.delimiter + message + network._Protocol.delimiter);
            Hide();
            Connected = true;
            Destroy();
        }

        /// <summary>
        /// DCC
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="port"></param>
        /// <param name="Listener"></param>
        /// <param name="secure"></param>
        /// <param name="_n"></param>
        public OpenDCC(string server, string user, uint port, bool Listener, bool secure, Network _n)
        {
            Port = port;
            username = user;
            Server = server;
            network = _n;
            SSL = secure;
            if (SSL)
            {
                type = ProtocolDCC.DCC.SecureChat;
            }
            ListenerMode = Listener;
            Build();
            button9.Clicked += new EventHandler(_click);
        }
    }
}
