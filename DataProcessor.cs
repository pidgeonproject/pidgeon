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
            string tempdata = text;
            while (tempdata.Contains(" http://"))
            {
                    string link = templink.Substring(templink.IndexOf(" http://") + 8);
                    if (link.Length > 0)
                    {
                        if (link.Contains(" "))
                        {
                            link = link.Substring(0, link.IndexOf(" "));
                        }
                            result = result.Replace(" http://" + link, " <a href=\"http://" + link + "\">http://" + link + "</a>");
                            templink = templink.Replace(" http://" + link, "");
                            tempdata = tempdata.Substring(tempdata.IndexOf(" http://") + 8);
                            continue;
                        
                    }
                    tempdata = tempdata.Substring(tempdata.IndexOf(" http://") + 8);
            }
            tempdata = result;
            templink = result;
            while (tempdata.Contains(" #"))
            {
                string link = templink.Substring(templink.IndexOf(" #") + 2);
                if (link.Length > 0)
                {
                    if (link.Contains(" "))
                    {
                        link = link.Substring(0, link.IndexOf(" "));
                    }
                    result = result.Replace(" #" + link, " <a href=\"pidgeon://join#" + link + "\">#" + link + "</a>");
                    templink = templink.Replace(" #" + link, "");
                    tempdata = tempdata.Substring(tempdata.IndexOf(" #") + 2);
                    continue;

                }
                tempdata = tempdata.Substring(tempdata.IndexOf(" #") + 2);
            }
            if (result.Contains("%USER%") && result.Contains("%/USER%"))
            {
                string link = result.Substring(templink.IndexOf("%USER%") + 6);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/USER%"));
                    result = result.Replace("%USER%" + link + "%/USER%", "<a href=\"pidgeon://user/#" + link + "\">" + link + "</a>");
                }
            }
            templink = result;
            tempdata = result;
            while (tempdata.Contains(" https://"))
            {
                string link = templink.Substring(templink.IndexOf(" https://") + 9);
                if (link.Length > 0)
                {
                    if (link.Contains(" "))
                    {
                        link = link.Substring(0, link.IndexOf(" "));
                    }
                    result = result.Replace(" https://" + link, " <a href=\"https://" + link + "\">https://" + link + "</a>");
                    templink = templink.Replace(" https://" + link, "");
                    tempdata = tempdata.Substring(tempdata.IndexOf(" https://") + 9);
                    continue;

                }
                tempdata = tempdata.Substring(tempdata.IndexOf(" https://") + 9);
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
