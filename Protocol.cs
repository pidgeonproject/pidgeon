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
        public char delimiter = (char)001;
        public List<char> UModes = new List<char> { 'i', 'w', 'o', 'Q', 'r', 'A' };
        public List<char> UChars = new List<char> { '~', '&', '@', '%', '+' };
        public List<char> CUModes = new List<char> { 'q', 'a', 'o', 'h', 'v' };
        public List<char> CModes = new List<char> { 'n', 'r', 't', 'm' };
        public List<char> SModes = new List<char> { 'k', 'L' };
        public List<char> XModes = new List<char> { 'l' };
        public List<char> PModes = new List<char> { 'b', 'I', 'e' };
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
                    curr++;
                }
                return "+" + _val;
            }

            public bool mode(string text)
            {
                char prefix = ' ';
                foreach (char _x in text)
                {
                    if (_x == ' ')
                    {
                        continue;
                    }
                    if (_x == '-')
                    {
                        prefix = _x;
                        continue;
                    }
                    if (_x == '+')
                    {
                        prefix = _x;
                        continue;
                    }
                    switch (prefix)
                    { 
                        case '+':
                            if (!_Mode.Contains(_x.ToString()))
                            {
                                this._Mode.Add(_x.ToString());
                            }
                            continue;
                        case '-':
                            if (_Mode.Contains(_x.ToString()))
                            {
                                this._Mode.Remove(_x.ToString());
                            }
                            continue;
                    }continue;
                }
                return false;
            }
        }

        public string Server;
        public int Port;
        public bool SSL;

        public string PRIVMSG(string user, string text)
        {
            return Configuration.format_nick.Replace("$1", user) + text;
        }

        public virtual void Transfer(string data, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            
        }

        public virtual int Message2(string text, string to, Configuration.Priority _priority = Configuration.Priority.Normal)
        {
            return 0;
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

        public virtual void WriteMode(Mode _x, string target, Network network = null)
        {
            return;
        }

        public virtual void Join(string name, Network network = null)
        {
            return;
        }

        public virtual void Part(string name, Network network = null)
        {
        
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
