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
    class Hooks
    {
        public static void AfterCore()
        {
            
        }

        /// <summary>
        /// This hook allow you to call functions before you open connection
        /// </summary>
        /// <param name="server"></param>
        public static void BeforeIRCConnect(Protocol server)
        {
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="network"></param>
        public static void AfterIRCConnect(Network network)
        {
            
        }

        /// <summary>
        /// Events to happen before user is kicked from channel
        /// </summary>
        /// <param name="network">Network</param>
        /// <param name="Source">Performer</param>
        /// <param name="Target">User who get kick msg</param>
        /// <param name="Reason">Reason</param>
        /// <param name="Ch">Channel</param>
        public static void BeforeKick(Network network, string Source, string Target, string Reason, string Ch)
        {
        
        }

        /// <summary>
        /// Events to happen before joining, if return false, the join is cancelled
        /// </summary>
        /// <param name="network"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public static bool BeforeJoin(Network network, string Channel)
        {
            return true;
        }

        /// <summary>
        /// Events to happen before leaving a channel, if return false, the quiting is cancelled
        /// </summary>
        /// <param name="network"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool BeforePart(Network network, Channel channel)
        {
            return true;
        }

        /// <summary>
        /// Events to happen before quiting, if return false, the exiting is cancelled
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public static bool BeforeExit(Network network)
        {
            return true;
        }

        /// <summary>
        /// Events to happen before command, can't be stopped
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="command"></param>
        public static void BeforeCommand(Protocol protocol, string command)
        { 
        
        }

        public static void BeforeMode(Network network)
        { 
            
        }
    }
}
