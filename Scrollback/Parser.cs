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
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Text;

namespace Client
{
    class Parser
    {
        public static SBABox.ContentText color(string text, SBABox SBAB)
        {
            char colorchar = (char)3;
            int color = 0;
            bool closed = false;

            if (text.Contains(colorchar.ToString()))
            {
                int position = 0;
                while (text.Length > position)
                {
                    if (text[position] == colorchar)
                    {
                        if (closed)
                        {
                            text = text.Substring(position, text.Length - position);
                            SBABox.ContentText Link = new SBABox.ContentText(ProtocolIrc.decode_text(text), SBAB, Configuration.CurrentSkin.mrcl[color]);
                            return Link;
                        }

                        if (!closed)
                        {
                            if (!int.TryParse(text[position + 1].ToString() + text[position + 2].ToString(), out color))
                            {
                                if (!int.TryParse(text[position + 1].ToString(), out color))
                                {
                                    color = 0;
                                }
                            }
                            if (color > 9)
                            {
                                text = text.Remove(position, 3);
                            }
                            else
                            {
                                text = text.Remove(position, 2);
                            }
                            closed = true;
                            if (color < 16)
                            {
                                text = text.Substring(position);
                                break;
                            }
                        }
                    }
                    position++;
                }
            }
            return null;
        }

        public static SBABox.ContentText parse_ident(string text, SBABox SBAB, bool under, bool bold)
        {
            if (text.Contains("%D%") && text.Contains("%/D%"))
            {
                string link = text.Substring(text.IndexOf("%D%") + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/D%"));
                    SBABox.ContentText Link = new SBABox.ContentText(ProtocolIrc.decode_text(link), SBAB, Configuration.CurrentSkin.link);
                    Link.Link = "pidgeon://ident/#" + ProtocolIrc.decode_text(link);
                    Link.Underline = under;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        public static SBABox.ContentText parse_link(string text, SBABox SBAB, bool under, bool bold)
        {
            if (text.Contains("%L%") && text.Contains("%/L%"))
            {
                string link = text.Substring(text.IndexOf("%L%") + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/L%"));
                    SBABox.ContentText Link = new SBABox.ContentText(ProtocolIrc.decode_text(link), SBAB, Configuration.CurrentSkin.link);
                    Link.Link = "pidgeon://text/#" + ProtocolIrc.decode_text(link);
                    Link.Underline = under;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        public static SBABox.ContentText parse_host(string text, SBABox SBAB, bool under, bool bold, Color color)
        {
            if (text.Contains("%H%") && text.Contains("%/H%"))
            {
                string link = text.Substring(text.IndexOf("%H%") + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/H%"));
                    SBABox.ContentText Link = new SBABox.ContentText(ProtocolIrc.decode_text(link), SBAB, Configuration.CurrentSkin.link);
                    Link.Link = "pidgeon://hostname/#" + ProtocolIrc.decode_text(link);
                    Link.Underline = under;
                    Link.TextColor = color;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }


        public static SBABox.ContentText parse_chan(string text, SBABox SBAB, bool under, bool bold, Color color)
        {
            if (text.StartsWith("#"))
            {
                string link = text.Substring(text.IndexOf("#"));
                if (link.Length > 0)
                {
                    char separator = ' ';
                    foreach (char xx in Configuration.Parser.Separators)
                    {
                        if (link.Contains(xx.ToString()))
                        {
                            separator = xx;
                            break;
                        }
                    }
                    if (link.Contains(separator.ToString()))
                    {
                        link = link.Substring(0, link.IndexOf(separator.ToString()));
                    }

                    SBABox.ContentText Link = new SBABox.ContentText(ProtocolIrc.decode_text(link), SBAB, Configuration.CurrentSkin.link);
                    Link.Link = "pidgeon://join/" + ProtocolIrc.decode_text(link);
                    Link.Underline = under;
                    if (Configuration.Colors.ChangeLinks)
                    {
                        Link.TextColor = color;
                    }
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        public static SBABox.ContentText parse_name(string text, SBABox SBAB, bool under, bool bold, Color color)
        {
            if (text.Contains("%USER%") && text.Contains("%/USER%"))
            {
                string link = text.Substring(text.IndexOf("%USER%") + 6);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/USER%"));

                    SBABox.ContentText Link = new SBABox.ContentText(ProtocolIrc.decode_text(link), SBAB, Configuration.CurrentSkin.link);
                    Link.Bold = bold;
                    Link.Underline = under;
                    if (Configuration.Colors.ChangeLinks)
                    {
                        Link.TextColor = color;
                    }
                    Link.Link = "pidgeon://user/#" + ProtocolIrc.decode_text(link);
                    return Link;
                }
            }
            return null;
        }

        public static SBABox.ContentText parse_http(string text, SBABox SBAB, bool under, bool bold, Color color, string CurrentProtocol, string prefix = null)
        {
            string result = text;
            string tempdata = text;
            if (prefix != null)
            {
                if (tempdata.Contains(prefix + CurrentProtocol))
                {
                    string link = result.Substring(result.IndexOf(CurrentProtocol) + CurrentProtocol.Length);
                    tempdata = tempdata.Substring(1);
                    if (link.Length > 0)
                    {
                        if (link.Contains(prefix))
                        {
                            link = link.Substring(0, link.IndexOf(prefix));
                        }
                    }
                    SBABox.ContentText Link = new SBABox.ContentText(CurrentProtocol + ProtocolIrc.decode_text(link), SBAB, Configuration.CurrentSkin.link);
                    Link.Underline = true;
                    Link.Bold = bold;
                    if (Configuration.Colors.ChangeLinks)
                    {
                        Link.TextColor = color;
                    }
                    Link.Link = CurrentProtocol + ProtocolIrc.decode_text(link);
                    return Link;
                }
                return null;
            }

            if (tempdata.StartsWith(CurrentProtocol))
            {
                string link = result.Substring(7);
                if (link.Length > 0)
                {
                    char separator = ' ';
                    foreach (char xx in Configuration.Parser.Separators)
                    {
                        if (link.Contains(xx.ToString()))
                        {
                            separator = xx;
                            break;
                        }
                    }
                    if (link.Contains(separator.ToString()))
                    {
                        link = link.Substring(0, link.IndexOf(separator.ToString()));
                    }
                }
                SBABox.ContentText Link = new SBABox.ContentText(CurrentProtocol + ProtocolIrc.decode_text(link), SBAB, Configuration.CurrentSkin.link);
                Link.Underline = true;
                Link.Bold = bold;
                if (Configuration.Colors.ChangeLinks)
                {
                    Link.TextColor = color;
                }
                Link.Link = CurrentProtocol + ProtocolIrc.decode_text(link);
                return Link;
            }

            foreach (char curr in Configuration.Parser.Separators)
            {
                if (tempdata.StartsWith(curr.ToString()))
                {
                    if (tempdata.Substring(1).Contains(CurrentProtocol))
                    {
                        string link = result.Substring(result.IndexOf(CurrentProtocol) + 7);
                        tempdata = tempdata.Substring(1);
                        if (link.Length > 0)
                        {
                            char separator = ' ';
                            foreach (char xx in Configuration.Parser.Separators)
                            {
                                if (link.Contains(xx.ToString()))
                                {
                                    separator = xx;
                                    break;
                                }
                            }
                            if (link.Contains(separator.ToString()))
                            {
                                link = link.Substring(0, link.IndexOf(separator.ToString()));
                            }
                        }
                        SBABox.ContentText Link = new SBABox.ContentText(CurrentProtocol + ProtocolIrc.decode_text(link), SBAB, Configuration.CurrentSkin.link);
                        Link.Underline = true;
                        Link.Bold = bold;
                        if (Configuration.Colors.ChangeLinks)
                        {
                            Link.TextColor = color;
                        }
                        Link.Link = CurrentProtocol + ProtocolIrc.decode_text(link);
                        return Link;
                    }
                }
            }
            return null;
        }



        public static SBABox.Line FormatLine(string text, SBABox SBAB, Color _style)
        {
            if (SBAB == null)
            {
                throw new Exception("invalid text");
            }

            SBABox.Line line = new SBABox.Line("", SBAB);
            string result = text;
            SBABox.ContentText lprttext = null;
            string templink = text;
            string tempdata = text;
            Color color = _style;
            string templine = "";
            bool Bold = false;
            bool Underlined = false;
            string Link = null;
            int Jump = 0;

            int carret = 0;

            while (carret < text.Length)
            {
                Jump = 1;
                string protocol = matchesProtocol(tempdata);
                if (protocol != null)
                {
                    char prefix = Prefix(tempdata, protocol);
                    if (templine != "")
                    {
                        if (prefix != '\0')
                        {
                            lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine) + prefix.ToString(), SBAB, color);
                        }
                        else
                        {
                            lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                        }
                        lprttext.Underline = Underlined;
                        lprttext.Bold = Bold;
                        lprttext.TextColor = color;
                        if (Link != null)
                        {
                            lprttext.Link = Link;
                        }
                        line.insertData(lprttext);
                        templine = "";
                    }
                    if (prefix != '\0')
                    {
                        line.insertData(parse_http(tempdata, SBAB, Underlined, Bold, color, protocol, prefix.ToString()));
                    }
                    else
                    {
                        line.insertData(parse_http(tempdata, SBAB, Underlined, Bold, color, protocol));
                    }
                    if (matchesAPrefix(tempdata.Substring(1)))
                    {
                        Jump = tempdata.Substring(1).IndexOf(Prefix(tempdata.Substring(1))) + 1;
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else if (tempdata.StartsWith("%USER%"))
                {
                    lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    line.insertData(parse_name(tempdata, SBAB, Underlined, Bold, color));
                    if (tempdata.Contains("%/USER%"))
                    {
                        Jump = tempdata.IndexOf("%/USER%") + 7;
                    }
                    else
                    {
                        Jump = tempdata.Length - 1;
                    }
                }
                else if (tempdata.StartsWith(" #") || (tempdata.StartsWith("#") && text.StartsWith("#")))
                {
                    if (tempdata.StartsWith(" #"))
                    {
                        templine += text[carret].ToString();
                        carret++;
                        tempdata = tempdata.Substring(1);
                    }

                    lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                    lprttext.Underline = Underlined;
                    lprttext.Bold = Bold;
                    line.insertData(lprttext);
                    templine = "";

                    line.insertData(parse_chan(tempdata, SBAB, Underlined, Bold, color));
                    if (tempdata.Contains(" "))
                    {
                        Jump = tempdata.IndexOf(" ");
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else if (tempdata.StartsWith(((char)002).ToString()))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Bold = !Bold;
                    carret++;
                }
                else if (tempdata.StartsWith(((char)003).ToString()))
                {
                    // change color
                    int colorcode = -2;
                    tempdata = tempdata.Substring(1);
                    carret++;
                    lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    if (tempdata.Length > 1)
                    {
                        if (!int.TryParse(tempdata.Substring(0, 2), out colorcode))
                        {
                            if (!int.TryParse(tempdata.Substring(0, 1), out colorcode))
                            {
                                // we just terminated the color
                                color = _style;
                                colorcode = -2;
                            }
                            else
                            {
                                tempdata = tempdata.Substring(1);
                                carret++;
                            }
                        }
                        else
                        {
                            tempdata = tempdata.Substring(2);
                            carret++;
                            carret++;
                        }
                    }
                    Jump = 0;
                    templine = "";
                    if (colorcode >= 0)
                    {
                        if (colorcode < 16)
                        {
                            color = Configuration.CurrentSkin.mrcl[colorcode];
                        }
                        else
                        {
                            color = Configuration.CurrentSkin.mrcl[0];
                            Core.DebugLog("Invalid color for link: " + colorcode.ToString());
                        }
                    }
                }
                else if (tempdata.StartsWith(((char)004).ToString()))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Underlined = !Underlined;
                    carret++;
                }
                else if (tempdata.StartsWith("%H%"))
                {
                    lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    line.insertData(parse_host(tempdata, SBAB, Underlined, Bold, color));
                    if (tempdata.Contains("%/H%"))
                    {
                        Jump = tempdata.IndexOf("%/H%") + 4;
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else if (tempdata.StartsWith("%L%"))
                {
                    lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    line.insertData(parse_link(tempdata, SBAB, Underlined, Bold));
                    if (tempdata.Contains("%/L%"))
                    {
                        Jump = tempdata.IndexOf("%/L%") + 4;
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else if (tempdata.StartsWith("%D%"))
                {
                    lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    line.insertData(parse_ident(tempdata, SBAB, Underlined, Bold));
                    if (tempdata.Contains("%/D%"))
                    {
                        Jump = tempdata.IndexOf("%/D%") + 4;
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else
                {
                    templine += text[carret];
                }
                tempdata = tempdata.Substring(Jump);
                carret = carret + Jump;
            }
            lprttext = new SBABox.ContentText(ProtocolIrc.decode_text(templine), SBAB, color);
            lprttext.Underline = Underlined;
            lprttext.Bold = Bold;
            line.insertData(lprttext);

            return line;
        }

        public static bool matchesPrefix(string data, string x)
        {
            foreach (char curr in Configuration.Parser.Separators)
            {
                if (data.Contains(curr.ToString() + x))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool matchesAPrefix(string data)
        {
            foreach (char curr in Configuration.Parser.Separators)
            {
                if (data.Contains(curr.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public static string matchesProtocol(string data)
        {
            foreach (string CurrentProtocol in Configuration.Parser.Protocols)
            {
                if (matchesSWPrefix(data, CurrentProtocol))
                {
                    return CurrentProtocol;
                }
            }
            return null;
        }

        public static bool matchesSWPrefix(string data, string x)
        {
            if (data.StartsWith(x))
            {
                return true;
            }

            foreach (char curr in Configuration.Parser.Separators)
            {
                if (data.StartsWith(curr.ToString() + x))
                {
                    return true;
                }
            }
            return false;
        }

        public static char Prefix(string data, string x = null)
        {
            foreach (char curr in Configuration.Parser.Separators)
            {
                if (x != null)
                {
                    if (data.Contains(curr.ToString() + x))
                    {
                        return curr;
                    }
                }
                else
                {
                    if (data.Contains(curr.ToString()))
                    {
                        return curr;
                    }
                }
            }
            return '\0';
        }

        public static string link2(string text)
        {
            try
            {
                string result = text;
                string templink = text;
                string tempdata = text;
                while (matchesPrefix(tempdata, "http://"))
                {
                    string separator = Prefix(tempdata, "http://").ToString();
                    string link = templink.Substring(templink.IndexOf(separator + "http://") + 8);
                    if (link.Length > 0)
                    {
                        if (link.Contains(separator))
                        {
                            link = link.Substring(0, link.IndexOf(separator));
                        }
                        result = result.Replace(" http://" + link, " <a href=\"http://" + link + "\">http://" + link + "</a>");
                        templink = templink.Replace(separator + "http://" + link, "");
                        tempdata = tempdata.Substring(tempdata.IndexOf(separator + "http://") + 8);
                        continue;

                    }
                    tempdata = tempdata.Substring(tempdata.IndexOf(separator + "http://") + 8);
                }
                tempdata = result;
                templink = result;
                while (matchesPrefix(tempdata, "#"))
                {
                    string separator = Prefix(tempdata, "#").ToString();
                    string link = templink.Substring(templink.IndexOf(separator + "#") + 2);
                    if (link.Length > 0)
                    {
                        if (link.Contains(separator))
                        {
                            link = link.Substring(0, link.IndexOf(separator));
                        }
                        result = result.Replace(separator + "#" + link, " <a href=\"pidgeon://join#" + link + "\">#" + link + "</a>");
                        templink = templink.Replace(separator + "#" + link, "");
                        tempdata = tempdata.Substring(tempdata.IndexOf(separator + "#") + 2);
                        continue;

                    }
                    tempdata = tempdata.Substring(tempdata.IndexOf(separator + "#") + 2);
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
                while (result.Contains("%L%") && result.Contains("%/L%"))
                {
                    string link = result.Substring(result.IndexOf("%L%") + 3);
                    if (link.Length > 0)
                    {
                        link = link.Substring(0, link.IndexOf("%/L%"));
                        result = result.Replace("%L%" + link + "%/L%", "<a href=\"pidgeon://text/#" + link + "\">" + link + "</a>");
                    }
                }
                templink = result;
                tempdata = result;
                while (matchesPrefix(tempdata, "https://"))
                {
                    string pr = Prefix(tempdata, "https://").ToString();
                    string link = templink.Substring(templink.IndexOf(pr + "https://") + 9);
                    if (link.Length > 0)
                    {
                        if (link.Contains(pr))
                        {
                            link = link.Substring(0, link.IndexOf(pr));
                        }
                        result = result.Replace(pr + "https://" + link, " <a href=\"https://" + link + "\">https://" + link + "</a>");
                        templink = templink.Replace(pr + "https://" + link, "");
                        tempdata = tempdata.Substring(tempdata.IndexOf(pr + "https://") + 9);
                        continue;

                    }
                    tempdata = tempdata.Substring(tempdata.IndexOf(pr + "https://") + 9);
                }
                char colorchar = (char)3;
                int color = 0;
                bool closed = false;

                while (result.Contains(colorchar.ToString()))
                {
                    int position = 0;
                    while (result.Length > position)
                    {
                        if (result[position] == colorchar)
                        {
                            if (closed)
                            {
                                result = result.Remove(position, 1);
                                result = result.Insert(position, "</FONT>");
                                closed = false;
                                break;
                            }

                            if (!closed)
                            {
                                if (result.Length > position + 3)
                                {
                                    if (!int.TryParse(result[position + 1].ToString() + result[position + 2].ToString(), out color))
                                    {
                                        if (!int.TryParse(result[position + 1].ToString(), out color))
                                        {
                                            color = 0;
                                        }
                                    }
                                }
                                if (color > 9)
                                {
                                    if (result.Length > position + 4)
                                    {
                                        result = result.Remove(position, 3);
                                    }
                                }
                                else
                                {
                                    if (result.Length > position + 3)
                                    {
                                        result = result.Remove(position, 2);
                                    }
                                }
                                closed = true;
                                if (color < 16)
                                {
                                    result = result.Insert(position, "<font color=\"" + Configuration.CurrentSkin.mrcl[color].ToArgb().ToString() + "\">");
                                    break;
                                }
                            }
                        }
                        position++;
                    }
                }

                return result;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
                return "";
            }
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
                Core._Main.Chat.scrollback.InsertText("Not connected", Client.ContentLine.MessageStyle.User);
                return 2;
            }
            if (Core.network.Connected)
            {
                if (Core._Main.Chat.writable)
                {
                    if (input.StartsWith(Configuration.CommandPrefix))
                    {
                        Core.network.Message(input.Substring(1), Core._Main.Chat.name);
                        return 2;
                    }
                    Core.network.Message(input, Core._Main.Chat.name);
                }
                return 0;
            }
            return 0;
        }
    }
}
