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
using System.Drawing;
using System.Threading;
using System.Text;

namespace Pidgeon
{
    /// <summary>
    /// This is a parser of text provided by user or server
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parser cache
        /// </summary>
        public static Dictionary<string, Pidgeon.RichTBox.Line> ParserCache = new Dictionary<string, RichTBox.Line>();

        /// <summary>
        /// Return if cache is enabled or not
        /// </summary>
        private static bool CacheEnabled
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
        private static Pidgeon.RichTBox.Line FromCache(string line)
        {
            Pidgeon.RichTBox.Line rv;
            if (ParserCache.TryGetValue(line, out rv))
            {
                return rv;
            }
            return null;
        }

        /// <summary>
        /// Store a string to parser cache
        /// </summary>
        /// <param name="line"></param>
        /// <param name="data"></param>
        private static void ToCache(string line, Pidgeon.RichTBox.Line data)
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
        /// Parse ident from a text
        /// </summary>
        /// <param name="text">Text to scan</param>
        /// <param name="SBAB">Rich text box</param>
        /// <param name="under">If text is underlined or not</param>
        /// <param name="bold">If text is bold or not</param>
        /// <returns></returns>
        private static Pidgeon.RichTBox.ContentText parse_ident(string text, Pidgeon.RichTBox SBAB, bool under, bool bold)
        {
            if (text.Contains("%D%") && text.Contains("%/D%"))
            {
                string link = text.Substring(text.IndexOf("%D%", StringComparison.Ordinal) + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/D%", StringComparison.Ordinal));
                    Pidgeon.RichTBox.ContentText Link = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Link = "pidgeon://ident/#" + Protocols.ProtocolIrc.DecodeText(link);
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
        private static Pidgeon.RichTBox.ContentText parse_link(string text, Pidgeon.RichTBox SBAB, bool under, bool bold)
        {
            if (text.Contains("%L%") && text.Contains("%/L%"))
            {
                string link = text.Substring(text.IndexOf("%L%", StringComparison.Ordinal) + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/L%", StringComparison.Ordinal));
                    Pidgeon.RichTBox.ContentText Link = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Link = "pidgeon://text/#" + Protocols.ProtocolIrc.DecodeText(link);
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
        private static Pidgeon.RichTBox.ContentText parse_host(string text, Pidgeon.RichTBox SBAB, bool under, bool bold, Color color)
        {
            if (text.Contains("%H%") && text.Contains("%/H%"))
            {
                string link = text.Substring(text.IndexOf("%H%", StringComparison.Ordinal) + 3);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/H%", StringComparison.Ordinal));
                    Pidgeon.RichTBox.ContentText Link = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Link = "pidgeon://hostname/#" + Protocols.ProtocolIrc.DecodeText(link);
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
        private static Pidgeon.RichTBox.ContentText parse_chan(string text, Pidgeon.RichTBox SBAB, bool under, bool bold, Color color)
        {
            if (text[0] == '#')
            {
                string separator = PrefixString(text);
                if (separator.Length > 0)
                {
                    text = text.Substring(0, text.IndexOf(separator, StringComparison.Ordinal));
                }
                foreach (string xx in Configuration.Parser.SeparatorsCache)
                {
                    if (text.Contains(xx))
                    {
                        text = text.Substring(0, text.IndexOf(xx, StringComparison.Ordinal));
                        break;
                    }
                }
                Pidgeon.RichTBox.ContentText Link = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(text), Configuration.CurrentSkin.LinkColor);
                Link.Link = "pidgeon://join/";
                Link.Link += Protocols.ProtocolIrc.DecodeText(text);
                Link.Underline = under;
                if (Configuration.Colors.ChangeLinks)
                {
                    Link.TextColor = color;
                }
                Link.Bold = bold;
                return Link;
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
        private static Pidgeon.RichTBox.ContentText parse_name(string text, Pidgeon.RichTBox SBAB, bool under, bool bold, Color color)
        {
            if (text.Contains("%USER%") && text.Contains("%/USER%"))
            {
                string link = text.Substring(text.IndexOf("%USER%", StringComparison.Ordinal) + 6);
                if (link.Length > 0)
                {
                    link = link.Substring(0, link.IndexOf("%/USER%", StringComparison.Ordinal));

                    Pidgeon.RichTBox.ContentText Link = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Bold = bold;
                    Link.Underline = under;
                    if (Configuration.Colors.ChangeLinks)
                    {
                        Link.TextColor = color;
                    }
                    Link.Link = "pidgeon://user/#";
                    Link.Link += Protocols.ProtocolIrc.DecodeText(link);
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
        private static Pidgeon.RichTBox.ContentText parse_http(string text, Pidgeon.RichTBox SBAB, bool under, bool bold, Color color, string CurrentProtocol, string prefix = null)
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
                        string sepa = PrefixString(link);
                        if (sepa.Length > 0)
                        {
                            link = link.Substring(0, link.IndexOf(sepa, StringComparison.Ordinal));
                        }
                    }
                    Pidgeon.RichTBox.ContentText Link = new Pidgeon.RichTBox.ContentText(CurrentProtocol + Protocols.ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                    Link.Underline = true;
                    Link.Bold = bold;
                    if (Configuration.Colors.ChangeLinks)
                    {
                        Link.TextColor = color;
                    }
                    Link.Link = CurrentProtocol + Protocols.ProtocolIrc.DecodeText(link);
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
                Pidgeon.RichTBox.ContentText Link = new Pidgeon.RichTBox.ContentText(CurrentProtocol + Protocols.ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                Link.Underline = true;
                Link.Bold = bold;
                if (Configuration.Colors.ChangeLinks)
                {
                    Link.TextColor = color;
                }
                Link.Link = CurrentProtocol + Protocols.ProtocolIrc.DecodeText(link);
                return Link;
            }
            foreach (char curr in Configuration.Parser.Separators)
            {
                if (tempdata[0] == curr)
                {
                    if (tempdata.Contains(CurrentProtocol))
                    {
                        string link = result.Substring(result.IndexOf(CurrentProtocol, StringComparison.Ordinal) + 7);
                        if (link.Length > 0)
                        {
                            foreach (string xx in Configuration.Parser.SeparatorsCache)
                            {
                                if (link.Contains(xx))
                                {
                                    link = link.Substring(0, link.IndexOf(xx, StringComparison.Ordinal));
                                    break;
                                }
                            }
                        }
                        Pidgeon.RichTBox.ContentText Link = new Pidgeon.RichTBox.ContentText(CurrentProtocol + Protocols.ProtocolIrc.DecodeText(link), Configuration.CurrentSkin.LinkColor);
                        Link.Underline = true;
                        Link.Bold = bold;
                        if (Configuration.Colors.ChangeLinks)
                        {
                            Link.TextColor = color;
                        }
                        Link.Link = CurrentProtocol + Protocols.ProtocolIrc.DecodeText(link);
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
        public static Pidgeon.RichTBox.Line FormatLine(string text, RichTBox SBAB, Color _style)
        {
            // this is some hardcore source bellow, have fun
            if (SBAB == null)  throw new PidgeonException("NULL reference to RichTBox object");
            Pidgeon.RichTBox.Line line;
            if (CacheEnabled)
            {
                line = FromCache(text);
                if (line != null) return line;
            }
            line = new Pidgeon.RichTBox.Line("", SBAB);
            Pidgeon.RichTBox.ContentText lprttext = null;
            string tempdata = text;
            Color color = _style;
            StringBuilder templine = new StringBuilder();
            bool bold_ = false;
            bool underline_ = false;
            bool italic_ = false;
            string link = null;
            int Jump = 0;
            int carret = 0;
            while (carret < text.Length)
            {
                Jump = 1;
                // we check if the current string is actually some url
                string protocol = matchesProtocol(tempdata);
                char first_char_ = '\0';
                if (tempdata.Length > 0)
                    first_char_ = tempdata[0];
                if (protocol != null)
                {
                    // check if there is a prefix character that is a symbol which separates the url
                    char prefix = Prefix(tempdata, protocol);
                    if (templine.Length == 0 || prefix != '\0')
                    {
                        // if there is a text in buffer from previous call, we need to build it
                        if (prefix != '\0')
                        {
                            // we append the prefix to previous text because it must not be a part of url
                            lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()) +
                                                                        prefix.ToString(), color);
                        }
                        else
                        {
                            // there was no prefix
                            lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                        }
                        lprttext.Underline = underline_;
                        lprttext.Bold = bold_;
                        lprttext.TextColor = color;
                        lprttext.Italic = italic_;
                        if (link != null)
                            lprttext.Link = link;
                        line.insertData(lprttext);
                        templine.Clear();
                    }
                    if (prefix != '\0')
                    {
                        // now we need to create a hyperlink, we parse it using the prefix
                        line.insertData(parse_http(tempdata, SBAB, underline_, bold_, color, protocol, prefix.ToString()));
                    }
                    else
                    {
                        // create a hyperlink not using the prefix
                        line.insertData(parse_http(tempdata, SBAB, underline_, bold_, color, protocol));
                    }
                    string temp01 = tempdata.Substring(1);
                    // we check if there is another separator in the rest of link
                    if (matchesAPrefix(temp01))
                        Jump = temp01.IndexOf(Prefix(temp01)) + 1;
                    else
                        Jump = tempdata.Length;
                } // here we check if the string is a user link
                else if (first_char_ == '%' && tempdata.StartsWith("%USER%", StringComparison.Ordinal))
                {
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Underline = underline_;
                    lprttext.Italic = italic_;
                    line.insertData(lprttext);
                    templine.Clear();
                    line.insertData(parse_name(tempdata, SBAB, underline_, bold_, color));
                    if (tempdata.Contains("%/USER%"))
                        Jump = tempdata.IndexOf("%/USER%", StringComparison.Ordinal) + 7;
                    else
                        Jump = tempdata.Length - 1;
                } // now we check if string is a channel name
                else if ((first_char_ == ' ' &&  tempdata.StartsWith(" #", StringComparison.Ordinal)) || (first_char_ == '#' &&
                    tempdata.StartsWith("#", StringComparison.Ordinal) && text.StartsWith("#", StringComparison.Ordinal)))
                {
                    if (first_char_ == ' ' && tempdata.StartsWith(" #", StringComparison.Ordinal))
                    {
                        templine.Append(text[carret]);
                        carret++;
                        tempdata = tempdata.Substring(1);
                    }

                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Underline = underline_;
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    line.insertData(lprttext);
                    templine.Clear();
                    line.insertData(parse_chan(tempdata, SBAB, underline_, bold_, color));
                    if (tempdata.Contains(" "))
                        Jump = tempdata.IndexOf(" ", StringComparison.Ordinal);
                    else
                        Jump = tempdata.Length;
                } // Italic
                else if (first_char_ == (char)0016)
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    lprttext.Underline = underline_;
                    line.insertData(lprttext);
                    templine.Clear();
                    italic_ = !italic_;
                    carret++;
                } // Underline
                else if (first_char_ == (char)001)
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    lprttext.Underline = underline_;
                    line.insertData(lprttext);
                    templine.Clear();
                    underline_ = !underline_;
                    carret++;
                } // Bold text
                else if (first_char_ == (char)002)
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    lprttext.Underline = underline_;
                    line.insertData(lprttext);
                    templine.Clear();
                    bold_ = !bold_;
                    carret++;
                }
                else if (first_char_ == (char)003)
                {
                    // change color
                    int colorcode = -2;
                    tempdata = tempdata.Substring(1);
                    carret++;
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    lprttext.Underline = underline_;
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
                            carret += 2;
                        }
                    }
                    Jump = 0;
                    templine.Clear();
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
                else if (first_char_ == (char)004)
                {
                    tempdata = tempdata.Substring(1);
                    Jump = 0;
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    lprttext.Underline = underline_;
                    line.insertData(lprttext);
                    templine.Clear();
                    underline_ = !underline_;
                    carret++;
                }
                else if (first_char_ == '%' && tempdata.StartsWith("%H%", StringComparison.Ordinal))
                {
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    lprttext.Underline = underline_;
                    line.insertData(lprttext);
                    templine.Clear();
                    line.insertData(parse_host(tempdata, SBAB, underline_, bold_, color));
                    if (tempdata.Contains("%/H%"))
                        Jump = tempdata.IndexOf("%/H%", StringComparison.Ordinal) + 4;
                    else
                        Jump = tempdata.Length;
                }
                else if (first_char_ == '%' && tempdata.StartsWith("%L%", StringComparison.Ordinal))
                {
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    lprttext.Underline = underline_;
                    line.insertData(lprttext);
                    templine.Clear();
                    line.insertData(parse_link(tempdata, SBAB, underline_, bold_));
                    if (tempdata.Contains("%/L%"))
                        Jump = tempdata.IndexOf("%/L%", StringComparison.Ordinal) + 4;
                    else
                        Jump = tempdata.Length;
                }
                else if (first_char_ == '%' && tempdata.StartsWith("%D%", StringComparison.Ordinal))
                {
                    lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
                    lprttext.Bold = bold_;
                    lprttext.Italic = italic_;
                    lprttext.Underline = underline_;
                    line.insertData(lprttext);
                    templine.Clear();
                    line.insertData(parse_ident(tempdata, SBAB, underline_, bold_));
                    if (tempdata.Contains("%/D%"))
                        Jump = tempdata.IndexOf("%/D%", StringComparison.Ordinal) + 4;
                    else
                        Jump = tempdata.Length;
                }
                else
                    templine.Append(text[carret]);
                tempdata = tempdata.Substring(Jump);
                carret = carret + Jump;
            }
            lprttext = new Pidgeon.RichTBox.ContentText(Protocols.ProtocolIrc.DecodeText(templine.ToString()), color);
            lprttext.Underline = underline_;
            lprttext.Italic = italic_;
            lprttext.Bold = bold_;
            line.insertData(lprttext);
            if (CacheEnabled)
                ToCache(text, line);
            return line;
        }

        /// <summary>
        /// Check if string starts with a prefix
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool matchesAPrefix(string data)
        {
            foreach (string curr in Configuration.Parser.SeparatorsCache)
            {
                if (data.Contains(curr))
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
            char FirstChar = Original[0];
            foreach (char curr in Configuration.Parser.Separators)
            {
                if (FirstChar == curr && Original.StartsWith(curr.ToString() + Prefix, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the string start with a prefix or not and if it does it return the symbol which
        /// is most close to beginning of string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static char Prefix(string data, string x = null)
        {
            char rv = '\0';
            int size = 99999999;
            foreach (string curr in Configuration.Parser.SeparatorsCache)
            {
                if (x != null)
                {
                    if (data.Contains(curr + x))
                    {
                        int ps = data.IndexOf(curr + x, StringComparison.Ordinal);
                        if (ps < size)
                        {
                            rv = curr[0];
                            size = ps;
                        }
                    }
                }
                else
                {
                    if (data.Contains(curr))
                    {
                        int ps = data.IndexOf(curr, StringComparison.Ordinal);
                        if (ps < size)
                        {
                            rv = curr[0];
                            size = ps;
                        }
                    }
                }
            }
            return rv;
        }

        /// <summary>
        /// Check if the string start with a prefix or not and if it does it return the symbol which
        /// is most close to beginning of string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static string PrefixString(string data, string x = null)
        {
            string rv = "";
            int size = 99999999;
            foreach (string curr in Configuration.Parser.SeparatorsCache)
            {
                if (x != null)
                {
                    if (data.Contains(curr + x))
                    {
                        int ps = data.IndexOf(curr + x, StringComparison.Ordinal);
                        if (ps < size)
                        {
                            rv = curr;
                            size = ps;
                        }
                    }
                }
                else
                {
                    if (data.Contains(curr))
                    {
                        int ps = data.IndexOf(curr, StringComparison.Ordinal);
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
            if (input.StartsWith(Configuration.CommandPrefix, StringComparison.Ordinal) &&
                !input.StartsWith(Configuration.CommandPrefix +
                Configuration.CommandPrefix, StringComparison.Ordinal))
            {
                Core.ProcessCommand(input);
                return 10;
            }
            if (_window._Protocol != null && (_window._Protocol.ParseInput(input) == libirc.IProtocol.Result.Done))
            {
                return 12;
            }
            if (network == null)
            {
                _window.scrollback.InsertText("Not connected", Pidgeon.ContentLine.MessageStyle.User);
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
            }
            return 0;
        }
    }
}
