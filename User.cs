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
    public class User
    {
        public string Host;
        public string Ident;
        public Protocol.Mode ChannelMode = new Protocol.Mode();
        public string Nick;
        public List<Channel> ChannelList;
        public User(string _Nick, string host, string ident)
        {
            ChannelList = new List<Channel>();
            if (_Nick.StartsWith("~"))
            {
                ChannelMode._Mode.Add("q");
                _Nick = _Nick.Substring(1);
            }
            if (_Nick.StartsWith("&"))
            {
                ChannelMode._Mode.Add("a");
                _Nick = _Nick.Substring(1);
            }
            if (_Nick.StartsWith("+"))
            {
                ChannelMode._Mode.Add("v");
                _Nick = _Nick.Substring(1);
            }
            if (_Nick.StartsWith("@"))
            {
                _Nick = _Nick.Substring(1);
                ChannelMode._Mode.Add("o");
            }
            if (_Nick.StartsWith("%"))
            {
                ChannelMode._Mode.Add("h");
                _Nick = _Nick.Substring(1);
            }
            Nick = _Nick;
            Ident = ident;
            Host = host;
        }
    }
}
