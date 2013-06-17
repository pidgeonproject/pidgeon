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

namespace Client.Forms
{
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
        private ProtocolDCC.DCC Type = ProtocolDCC.DCC.Chat;

        public OpenDCC(string server, string user, uint port, bool Listener, bool secure, Network _n)
        {
            Port = port;
            username = user;
            Server = server;
            network = _n;
            SSL = secure;
            ListenerMode = Listener;
            Build();
        }
    }
}
