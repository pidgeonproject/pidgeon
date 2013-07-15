//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

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
        /// <summary>
        /// Parser cache
        /// </summary>
        public static Dictionary<string, Client.RichTBox.Line> ParserCache = new Dictionary<string, RichTBox.Line>();
        public static bool CacheEnabled
        {
            get
            {
                return Configuration.Parser.ParserCache != 0;
            }
        }

        public static Client.RichTBox.Line FromCache(string line)
        {
            lock (ParserCache)
            {
                if (ParserCache.ContainsKey(line))
                {
                    return ParserCache[line];
                }
            }
            return null;
        }

        public static void ToCache(string line, Client.RichTBox.Line data)
        {
            lock (ParserCache)
            {
                if (ParserCache.ContainsKey(line))
                {
                    return;
                }
                // we remove the data from cache by blocks of 100, because doing this per a key would be too slow
                if (ParserCache.Count > Configuration.Parser.ParserCache + 100)
                {
                    List<string> keys = new List<string>();
                    foreach (string key in ParserCache.Keys)
                    {
                        keys.Add(key);
                        if (keys.Count > 100)
                        {
                            break;
                        }
                    }
                    while (ParserCache.Count > Configuration.Parser.ParserCache)
                    {
                        foreach (string key in keys)
                        {
                            ParserCache.Remove(key);
                        }
                    }
                }

                ParserCache.Add(line, data);
            }
        }

        public static char colorchar = (char)3;

        public static Client.RichTBox.ContentText color(string text, Client.RichTBox SBAB)
        {
            if (text.Contains(colorchar.ToString()))
            {
                int color = 0;
                bool closed = false;
                int position = 0;
                while (text.Length > position)
                {
                    if (text[position] == colorchar)
                    {
                        if (closed)
                        {
                            text = text.Substring(position, text.Length - position);
                            Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(text), Configuration.CurrentSkin.mrcl[color]);
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

        private static Client.RichTBox.ContentText parse_ident(string text, Client.RichTBox SBAB, bool under, bool bold)
        {
            if (text.Contains("%D%") && text.Contains("%/D%"))
            {
                string link = text.Substring(text.IndexOf("%D%") + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/D%"));
                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(link), Configuration.CurrentSkin.link);
                    Link.Link = "pidgeon://ident/#" + ProtocolIrc.decode_text(link);
                    Link.Underline = under;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        private static Client.RichTBox.ContentText parse_link(string text, Client.RichTBox SBAB, bool under, bool bold)
        {
            if (text.Contains("%L%") && text.Contains("%/L%"))
            {
                string link = text.Substring(text.IndexOf("%L%") + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/L%"));
                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(link), Configuration.CurrentSkin.link);
                    Link.Link = "pidgeon://text/#" + ProtocolIrc.decode_text(link);
                    Link.Underline = under;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        private static Client.RichTBox.ContentText parse_host(string text, Client.RichTBox SBAB, bool under, bool bold, Color color)
        {
            if (text.Contains("%H%") && text.Contains("%/H%"))
            {
                string link = text.Substring(text.IndexOf("%H%") + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/H%"));
                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(link), Configuration.CurrentSkin.link);
                    Link.Link = "pidgeon://hostname/#" + ProtocolIrc.decode_text(link);
                    Link.Underline = under;
                    Link.TextColor = color;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        private static Client.RichTBox.ContentText parse_chan(string text, Client.RichTBox SBAB, bool under, bool bold, Color color)
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

                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(link), Configuration.CurrentSkin.link);
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

        private static Client.RichTBox.ContentText parse_name(string text, Client.RichTBox SBAB, bool under, bool bold, Color color)
        {
            if (text.Contains("%USER%") && text.Contains("%/USER%"))
            {
                string link = text.Substring(text.IndexOf("%USER%") + 6);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/USER%"));

                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(link), Configuration.CurrentSkin.link);
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

        private static Client.RichTBox.ContentText parse_http(string text, Client.RichTBox SBAB, bool under, bool bold, Color color, string CurrentProtocol, string prefix = null)
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
                        // store a new separator to buffer in case link is not end
                        char sepa = Prefix(link);
                        if (sepa != '\0')
                        {
                            link = link.Substring(0, link.IndexOf(sepa.ToString()));
                        }
                    }
                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(CurrentProtocol + ProtocolIrc.decode_text(link), Configuration.CurrentSkin.link);
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
                    char sepa = Prefix(link);
                    if (sepa != '\0')
                    {
                        link = link.Substring(0, link.IndexOf(sepa));
                    }
                }
                Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(CurrentProtocol + ProtocolIrc.decode_text(link), Configuration.CurrentSkin.link);
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
                        Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(CurrentProtocol + ProtocolIrc.decode_text(link), Configuration.CurrentSkin.link);
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

        /// <summary>
        /// Format line
        /// </summary>
        /// <param name="text"></param>
        /// <param name="SBAB"></param>
        /// <param name="_style"></param>
        /// <returns></returns>
        public static Client.RichTBox.Line FormatLine(string text, RichTBox SBAB, Color _style)
        {
            if (SBAB == null)
            {
                throw new Exception("NULL reference to richtb");
            }

            Client.RichTBox.Line line = null;

            if (CacheEnabled)
            {
                line = FromCache(text);
                if (line != null)
                {
                    return line;
                }
            }

            line = new Client.RichTBox.Line("", SBAB);
            Client.RichTBox.ContentText lprttext = null;
            string tempdata = text;
            Color color = _style;
            string templine = "";
            bool Bold = false;
            bool Underlined = false;
            bool Italic = false;
            string Link = null;
            int Jump = 0;

            int carret = 0;

            while (carret < text.Length)
            {
                Jump = 1;
                // we check if the current string is actually some url
                string protocol = matchesProtocol(tempdata);
                if (protocol != null)
                {
                    // check if there is a prefix character that is a symbol which separates the url
                    char prefix = Prefix(tempdata, protocol);
                    if (templine != "" || prefix != '\0')
                    {
                        // if there is a text in buffer from previous call, we need to build it
                        if (prefix != '\0')
                        {
                            // we append the prefix to previous text because it must not be a part of url
                            lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine) + prefix.ToString(), color);
                        }
                        else
                        {
                            // there was no prefix
                            lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                        }
                        lprttext.Underline = Underlined;
                        lprttext.Bold = Bold;
                        lprttext.TextColor = color;
                        lprttext.Italic = Italic;
                        if (Link != null)
                        {
                            lprttext.Link = Link;
                        }
                        line.insertData(lprttext);
                        templine = "";
                    }
                    if (prefix != '\0')
                    {
                        // now we need to create a hyperlink we parse it using the prefix
                        line.insertData(parse_http(tempdata, SBAB, Underlined, Bold, color, protocol, prefix.ToString()));
                    }
                    else
                    {
                        // create a hyperlink not using the prefix
                        line.insertData(parse_http(tempdata, SBAB, Underlined, Bold, color, protocol));
                    }
                    string temp01 = tempdata.Substring(1);
                    // we check if there is another separator in the rest of link
                    if (matchesAPrefix(temp01))
                    {
                        Jump = temp01.IndexOf(Prefix(temp01)) + 1;
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else if (tempdata.StartsWith("%USER%"))
                {
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    lprttext.Italic = Italic;
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

                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Underline = Underlined;
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
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
                else if (tempdata.StartsWith(((char)0016).ToString()))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Italic = !Italic;
                    carret++;
                }
                else if (tempdata.StartsWith(((char)001).ToString()))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Underlined = !Underlined;
                    carret++;
                }
                else if (tempdata.StartsWith(((char)002).ToString()))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
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
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
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
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Underlined = !Underlined;
                    carret++;
                }
                else if (tempdata.StartsWith("%H%"))
                {
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
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
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
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
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
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
            lprttext = new Client.RichTBox.ContentText(ProtocolIrc.decode_text(templine), color);
            lprttext.Underline = Underlined;
            lprttext.Italic = Italic;
            lprttext.Bold = Bold;
            line.insertData(lprttext);

            if (CacheEnabled)
            {
                ToCache(text, line);
            }

            return line;
        }

        private static bool matchesPrefix(string data, string x)
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

        private static bool matchesAPrefix(string data)
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

        private static string matchesProtocol(string data)
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

        private static bool matchesSWPrefix(string data, string x)
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

        /// <summary>
        /// Check if the string start with a prefix or not
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static char Prefix(string data, string x = null)
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

        /// <summary>
        /// Process input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static int parse(string input, Graphics.Window window = null)
        {
            if (input == "")
            {
                return 0;
            }
            if (Configuration.Parser.InputTrim)
            {
                input = input.Trim();
            }
            Graphics.Window _window = window;
            if (window == null)
            {
                _window = Core.SystemForm.Chat;
            }
            Network network = _window._Network;
            if (network == null)
            {
                network = Core.SelectedNetwork;
            }
            if (input.StartsWith(Configuration.CommandPrefix) && !input.StartsWith(Configuration.CommandPrefix + Configuration.CommandPrefix))
            {
                Core.ProcessCommand(input);
                return 10;
            }
            if (_window._Protocol != null && _window._Protocol.ParseInput(input))
            {
                return 12;
            }
            if (network == null)
            {
                _window.scrollback.InsertText("Not connected", Client.ContentLine.MessageStyle.User);
                return 2;
            }
            if (network.IsConnected)
            {
                if (_window.IsWritable)
                {
                    if (input.StartsWith(Configuration.CommandPrefix))
                    {
                        network.Message(input.Substring(1), _window.WindowName);
                        return 2;
                    }
                    network.Message(input, _window.WindowName);
                }
                return 0;
            }
            return 0;
        }
    }
}
