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

namespace pidgeon_sv
{
    public class Item
    {
        public DateTime _date;
        string _text;
        public Item(string Data)
        {
            _date = DateTime.Now;
            _text = Data;
        }
    }

    public class OutgoingQueue
    {

        List<Item> queue = new List<Item>();
    }

    public class IncomingQueue
    {
        List<Item> queue = new List<Item>();
    }

    public class Connection
    {
        public string name = null;
        public string host = null;
        public string user = null;
        public Account account = null;
        public Status status = Status.WaitingPW;
        public Thread queue = null;
        public System.Net.Sockets.TcpClient client = null;
        public System.IO.StreamReader _r = null;
        public System.IO.StreamWriter _w = null;
        public OutgoingQueue _outgoing = new OutgoingQueue();
        public IncomingQueue _incoming = new IncomingQueue();
        public Thread main = null;
        public string IP = "";

        public bool Active = true;

        public enum Status {
            WaitingPW,
            Connected,
            Disconnected,
        }

        public bool Mode = false;

        public Connection()
        { 
            
        }
    }
}
