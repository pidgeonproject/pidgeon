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
    /// <summary>
    /// This is a parser of text provided by user or server
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parser cache
        /// </summary>
        public static Dictionary<string, Client.RichTBox.Line> ParserCache = new Dictionary<string, RichTBox.Line>();

        /// <summary>
        /// This character will change the color of text (it is standard of mirc)
        /// </summary>
        public static char colorchar = (char)3;

        /// <summary>
        /// Return if cache is enabled or not
        /// </summary>
        public static bool CacheEnabled
        {
            get
            {
                return Configuration.Parser.ParserCache != 0;
            }
        }

        /// <summary>
        /// Retrieve a parsed line from a cache
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Store a string to parser cache
        /// </summary>
        /// <param name="line"></param>
        /// <param name="data"></param>
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

        /// <summary>
        /// Change the color of text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="SBAB"></param>
        /// <returns></returns>
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
                            Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(text), Configuration.CurrentSkin.mrcl[color]);
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

        /// <summary>
        /// Parse ident from a text
        /// </summary>
        /// <param name="text">Text to scan</param>
        /// <param name="SBAB">Rich text box</param>
        /// <param name="under">If text is underlined or not</param>
        /// <param name="bold">If text is bold or not</param>
        /// <returns></returns>
        private static Client.RichTBox.ContentText parse_ident(string text, Client.RichTBox SBAB, bool under, bool bold)
        {
            if (text.Contains("%D%") && text.Contains("%/D%"))
            {
                string link = text.Substring(text.IndexOf("%D%", StringComparison.Ordinal) + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/D%", StringComparison.Ordinal));
                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Link = "pidgeon://ident/#" + ProtocolIrc.DecodeText(link);
                    Link.Underline = under;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        /// <summary>
        /// Parse link from a text
        /// </summary>
        /// <param name="text">Text to scan</param>
        /// <param name="SBAB">Rich text box</param>
        /// <param name="under">If text is underlined or not</param>
        /// <param name="bold">If text is bold or not</param>
        /// <returns></returns>
        private static Client.RichTBox.ContentText parse_link(string text, Client.RichTBox SBAB, bool under, bool bold)
        {
            if (text.Contains("%L%") && text.Contains("%/L%"))
            {
                string link = text.Substring(text.IndexOf("%L%", StringComparison.Ordinal) + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/L%", StringComparison.Ordinal));
                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Link = "pidgeon://text/#" + ProtocolIrc.DecodeText(link);
                    Link.Underline = under;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        /// <summary>
        /// Find a hostname
        /// </summary>
        /// <param name="text">Text to scan</param>
        /// <param name="SBAB">Rich text box</param>
        /// <param name="under">If text is underlined or not</param>
        /// <param name="bold">If text is bold or not</param>
        /// <param name="color">Color</param>
        /// <returns></returns>
        private static Client.RichTBox.ContentText parse_host(string text, Client.RichTBox SBAB, bool under, bool bold, Color color)
        {
            if (text.Contains("%H%") && text.Contains("%/H%"))
            {
                string link = text.Substring(text.IndexOf("%H%", StringComparison.Ordinal) + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/H%", StringComparison.Ordinal));
                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Link = "pidgeon://hostname/#" + ProtocolIrc.DecodeText(link);
                    Link.Underline = under;
                    Link.TextColor = color;
                    Link.Bold = bold;
                    return Link;
                }
            }
            return null;
        }

        /// <summary>
        /// Parse channel
        /// </summary>
        /// <param name="text">Text to scan</param>
        /// <param name="SBAB">Rich text box</param>
        /// <param name="under">If text is underlined or not</param>
        /// <param name="bold">If text is bold or not</param>
        /// <param name="color">Color</param>
        /// <returns></returns>
        private static Client.RichTBox.ContentText parse_chan(string text, Client.RichTBox SBAB, bool under, bool bold, Color color)
        {
            if (text.StartsWith("#", StringComparison.Ordinal))
            {
                string link = text.Substring(text.IndexOf("#", StringComparison.Ordinal));
                if (link.Length > 0)
                {
                    char separator = Prefix(link);
                    if (separator != '\0')
                    {
                        link = link.Substring(0, link.IndexOf(separator.ToString(), StringComparison.Ordinal));
                    }
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
                        link = link.Substring(0, link.IndexOf(separator.ToString(), StringComparison.Ordinal));
                    }

                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Link = "pidgeon://join/" + ProtocolIrc.DecodeText(link);
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

        /// <summary>
        /// Function find a username in a string and make a link from it so that we can click on it in scrollback
        /// </summary>
        /// <param name="text">Text to scan</param>
        /// <param name="SBAB">Rich text box</param>
        /// <param name="under">If text is underlined or not</param>
        /// <param name="bold">If text is bold or not</param>
        /// <param name="color">Color</param>
        /// <returns></returns>
        private static Client.RichTBox.ContentText parse_name(string text, Client.RichTBox SBAB, bool under, bool bold, Color color)
        {
            if (text.Contains("%USER%") && text.Contains("%/USER%"))
            {
                string link = text.Substring(text.IndexOf("%USER%", StringComparison.Ordinal) + 6);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/USER%", StringComparison.Ordinal));

                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Bold = bold;
                    Link.Underline = under;
                    if (Configuration.Colors.ChangeLinks)
                    {
                        Link.TextColor = color;
                    }
                    Link.Link = "pidgeon://user/#" + ProtocolIrc.DecodeText(link);
                    return Link;
                }
            }
            return null;
        }

        /// <summary>
        /// Parse http
        /// </summary>
        /// <param name="text">Text to scan</param>
        /// <param name="SBAB">Rich text box</param>
        /// <param name="under">If text is underlined or not</param>
        /// <param name="bold">If text is bold or not</param>
        /// <param name="color">Color</param>
        /// <param name="CurrentProtocol"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static Client.RichTBox.ContentText parse_http(string text, Client.RichTBox SBAB, bool under, bool bold, Color color, string CurrentProtocol, string prefix = null)
        {
            string result = text;
            string tempdata = text;
            if (prefix != null)
            {
                if (tempdata.Contains(prefix + CurrentProtocol))
                {
                    string link = result.Substring(result.IndexOf(CurrentProtocol, StringComparison.Ordinal) + CurrentProtocol.Length);
                    // remove a leading char which we used to parse the link
                    tempdata = tempdata.Substring(1);
                    if (link.Length > 0)
                    {
                        // store a new separator to buffer in case link is not end of text
                        char sepa = Prefix(link);
                        if (sepa != '\0')
                        {
                            link = link.Substring(0, link.IndexOf(sepa.ToString(), StringComparison.Ordinal));
                        }
                    }
                    Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(CurrentProtocol + ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Underline = true;
                    Link.Bold = bold;
                    if (Configuration.Colors.ChangeLinks)
                    {
                        Link.TextColor = color;
                    }
                    Link.Link = CurrentProtocol + ProtocolIrc.DecodeText(link);
                    return Link;
                }
                return null;
            }

            if (tempdata.StartsWith(CurrentProtocol, StringComparison.Ordinal))
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
                Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(CurrentProtocol + ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                Link.Underline = true;
                Link.Bold = bold;
                if (Configuration.Colors.ChangeLinks)
                {
                    Link.TextColor = color;
                }
                Link.Link = CurrentProtocol + ProtocolIrc.DecodeText(link);
                return Link;
            }

            foreach (char curr in Configuration.Parser.Separators)
            {
                if (tempdata.StartsWith(curr.ToString(), StringComparison.Ordinal))
                {
                    if (tempdata.Substring(1).Contains(CurrentProtocol))
                    {
                        string link = result.Substring(result.IndexOf(CurrentProtocol, StringComparison.Ordinal) + 7);
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
                                link = link.Substring(0, link.IndexOf(separator.ToString(), StringComparison.Ordinal));
                            }
                        }
                        Client.RichTBox.ContentText Link = new Client.RichTBox.ContentText(CurrentProtocol + ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                        Link.Underline = true;
                        Link.Bold = bold;
                        if (Configuration.Colors.ChangeLinks)
                        {
                            Link.TextColor = color;
                        }
                        Link.Link = CurrentProtocol + ProtocolIrc.DecodeText(link);
                        return Link;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Format line, this function is used to format text so that it has colors, clickable URL's and so
        /// </summary>
        /// <param name="text">Text that needs to be formatted</param>
        /// <param name="SBAB">This is object which replaced former SBA control, it is now a RichTBox,
        /// which is a custom widget for GTK used only by pidgeon with same interface as SBA had</param>
        /// <param name="_style">Default color for this text</param>
        /// <returns></returns>
        public static Client.RichTBox.Line FormatLine(string text, RichTBox SBAB, Color _style)
        {
            if (SBAB == null)
            {
                throw new Core.PidgeonException("NULL reference to RichTBox object");
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
                    if (!string.IsNullOrEmpty(templine) || prefix != '\0')
                    {
                        // if there is a text in buffer from previous call, we need to build it
                        if (prefix != '\0')
                        {
                            // we append the prefix to previous text because it must not be a part of url
                            lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine) + prefix.ToString(), color);
                        }
                        else
                        {
                            // there was no prefix
                            lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
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
                        // now we need to create a hyperlink, we parse it using the prefix
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
                else if (tempdata.StartsWith("%USER%", StringComparison.Ordinal))
                {
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Underline = Underlined;
                    lprttext.Italic = Italic;
                    line.insertData(lprttext);
                    templine = "";
                    line.insertData(parse_name(tempdata, SBAB, Underlined, Bold, color));
                    if (tempdata.Contains("%/USER%"))
                    {
                        Jump = tempdata.IndexOf("%/USER%", StringComparison.Ordinal) + 7;
                    }
                    else
                    {
                        Jump = tempdata.Length - 1;
                    }
                }
                else if (tempdata.StartsWith(" #", StringComparison.Ordinal)
                    || (tempdata.StartsWith("#", StringComparison.Ordinal) && text.StartsWith("#", StringComparison.Ordinal)))
                {
                    if (tempdata.StartsWith(" #", StringComparison.Ordinal))
                    {
                        templine += text[carret].ToString();
                        carret++;
                        tempdata = tempdata.Substring(1);
                    }

                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Underline = Underlined;
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    line.insertData(lprttext);
                    templine = "";

                    line.insertData(parse_chan(tempdata, SBAB, Underlined, Bold, color));
                    if (tempdata.Contains(" "))
                    {
                        Jump = tempdata.IndexOf(" ", StringComparison.Ordinal);
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else if (tempdata.StartsWith(((char)0016).ToString(), StringComparison.Ordinal))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Italic = !Italic;
                    carret++;
                }
                else if (tempdata.StartsWith(((char)001).ToString(), StringComparison.Ordinal))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Underlined = !Underlined;
                    carret++;
                }
                else if (tempdata.StartsWith(((char)002).ToString(), StringComparison.Ordinal))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Bold = !Bold;
                    carret++;
                }
                else if (tempdata.StartsWith(((char)003).ToString(), StringComparison.Ordinal))
                {
                    // change color
                    int colorcode = -2;
                    tempdata = tempdata.Substring(1);
                    carret++;
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
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
                else if (tempdata.StartsWith(((char)004).ToString(), StringComparison.Ordinal))
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    Underlined = !Underlined;
                    carret++;
                }
                else if (tempdata.StartsWith("%H%", StringComparison.Ordinal))
                {
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    line.insertData(parse_host(tempdata, SBAB, Underlined, Bold, color));
                    if (tempdata.Contains("%/H%"))
                    {
                        Jump = tempdata.IndexOf("%/H%", StringComparison.Ordinal) + 4;
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else if (tempdata.StartsWith("%L%", StringComparison.Ordinal))
                {
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    line.insertData(parse_link(tempdata, SBAB, Underlined, Bold));
                    if (tempdata.Contains("%/L%"))
                    {
                        Jump = tempdata.IndexOf("%/L%", StringComparison.Ordinal) + 4;
                    }
                    else
                    {
                        Jump = tempdata.Length;
                    }
                }
                else if (tempdata.StartsWith("%D%", StringComparison.Ordinal))
                {
                    lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
                    lprttext.Bold = Bold;
                    lprttext.Italic = Italic;
                    lprttext.Underline = Underlined;
                    line.insertData(lprttext);
                    templine = "";
                    line.insertData(parse_ident(tempdata, SBAB, Underlined, Bold));
                    if (tempdata.Contains("%/D%"))
                    {
                        Jump = tempdata.IndexOf("%/D%", StringComparison.Ordinal) + 4;
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
            lprttext = new Client.RichTBox.ContentText(ProtocolIrc.DecodeText(templine), color);
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

        /*
         * 
         * Nice to have
        /// <summary>
        /// Check if string starts with combination of prefix and string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        /// <returns></returns>
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
         */

        /// <summary>
        /// Check if string starts with a prefix
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check if string starts with another string or combination of that string and one of separators
        /// </summary>
        /// <param name="Original">Text to check for occurence of prefix</param>
        /// <param name="Prefix"></param>
        /// <returns>True if string contains the prefix or false in case it doesn't contain any of them</returns>
        private static bool matchesSWPrefix(string Original, string Prefix)
        {
            if (Original.StartsWith(Prefix, StringComparison.Ordinal))
            {
                return true;
            }

            foreach (char curr in Configuration.Parser.Separators)
            {
                if (Original.StartsWith(curr.ToString() + Prefix, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the string start with a prefix or not and if it does it return the symbol which is most close to beginning of string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static char Prefix(string data, string x = null)
        {
            char rv = '\0';
            int size = 99999999;
            foreach (char curr in Configuration.Parser.Separators)
            {
                if (x != null)
                {
                    if (data.Contains(curr.ToString() + x))
                    {
                        int ps = data.IndexOf(curr.ToString() + x, StringComparison.Ordinal);
                        if (ps < size)
                        {
                            rv = curr;
                            size = ps;
                        }
                    }
                }
                else
                {
                    if (data.Contains(curr.ToString()))
                    {
                        int ps = data.IndexOf(curr.ToString(), StringComparison.Ordinal);
                        if (ps < size)
                        {
                            rv = curr;
                            size = ps;
                        }
                    }
                }
            }
            return rv;
        }

        /// <summary>
        /// Process input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static int parse(string input, Graphics.Window window = null)
        {
            if (string.IsNullOrEmpty(input))
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
            if (input.StartsWith(Configuration.CommandPrefix, StringComparison.Ordinal) && !input.StartsWith(Configuration.CommandPrefix 
                + Configuration.CommandPrefix, StringComparison.Ordinal))
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
                    if (input.StartsWith(Configuration.CommandPrefix, StringComparison.Ordinal))
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
