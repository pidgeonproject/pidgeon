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
using System.Text;

namespace pidgeon_sv
{
    public class User : IComparable
    {

        public string Host;
        public Network _Network;
        public string Ident;
        public Protocol.Mode ChannelMode = new Protocol.Mode();
        public string Nick;
        public List<Channel> ChannelList;
        public User(string _Nick, string host, Network x, string ident)
        {
            ChannelList = new List<Channel>();
            _Network = x;
            if (_Nick != "")
            {
                char prefix = _Nick[0];
                if (x._protocol.UChars.Contains(prefix))
                {
                    int Mode = x._protocol.UChars.IndexOf(prefix);
                    if (x._protocol.CUModes.Count >= Mode + 1)
                    {
                        ChannelMode.mode("+" + x._protocol.CUModes[Mode].ToString());
                        _Nick = _Nick.Substring(1);
                    }
                }
            }
            Nick = _Nick;
            Ident = ident;
            Host = host;
        }

        public int CompareTo(object obj)
        {
            if (obj is User)
            {
                return this.Nick.CompareTo((obj as User).Nick);
            }
            return 0;
        }

        public string SummaryInfo()
        {
            return "";
        }
    }
}
