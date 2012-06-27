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
    public class Protocol
    {
        public class Mode
        {
            public List<string> _Mode = new List<string>();
            public override string ToString()
            {
                string _val = "";
                int curr = 0;
                while (curr < _Mode.Count)
                {
                    _val += _Mode[curr];
                }
                return "+" + _val;
            }
        }

        public string Server;
        public int Port;
        public bool SSL;

        public string PRIVMSG(string user, string text)
        {
            return "<" + user + "> " + text + "";
        }

        public virtual int Message(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            return 0;
        }

        public virtual int requestNick(string _Nick)
        {
            return 2;
        }

        public virtual bool Command(string cm)
        {
            return false;
        }

        public virtual void Join(string name, Network network = null)
        {
            return;
        }

        public virtual void Exit() { }  

        public class UserMode : Mode
        {
            
        }

        public virtual bool Open()
        {
            return false;
        }
    }
}
