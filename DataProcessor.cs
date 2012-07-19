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

namespace Client
{
    class DataProcessor
    {
        
    }

    class Parser
    {
        public static string link(string text)
        {
            string result = text;
            string templink = text;
            while (templink.Contains(" http://"))
            {
                string link = templink.Substring(templink.IndexOf(" http://") + 8);
                if (link.Length > 0)
                {
                    if (link.Contains(" "))
                    {
                        link = link.Substring(link.IndexOf(" "));
                        result.Replace(" http://" + link, "<a target=new href=\"http://" + link + "\">http://" + link + "</a>");
                        templink.Replace(" http://" + link, "");
                    }
                }
            }
            return result;
        }

        public static int parse(string input)
        {
            if (input == "")
            {
                return 0;
            }
            if (input.StartsWith(Configuration.CommandPrefix) && !input.StartsWith(Configuration.CommandPrefix + Configuration.CommandPrefix))
            {
                Core.ProcessCommand(input);
                return 10;
            }
            if (Core.network == null)
            {
                Core._Main.Chat.scrollback.InsertText("Not connected", Client.Scrollback.MessageStyle.User);
                return 2;
            }
            if (Core.network.Connected)
            {
                if (Core._Main.Chat.writable)
                {
                    Core.network._protocol.Message(input, Core._Main.Chat.name);
                }
                return 0;
            }
            return 0;
        }
    }
}
