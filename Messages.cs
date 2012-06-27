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
    public class messages
    {
        public static string Language = "en";
        public class container
        {
            public string language;
            public Dictionary<string, string> Cache;
            public container(string LanguageCode)
            {
                language = LanguageCode;
                Cache = new Dictionary<string, string>();
            }
        }

        public static Dictionary<string, container> data = new Dictionary<string, container>();

        private static string parse(string text, string name)
        {
            if (text.Contains(name + "="))
            {
                string x = text;
                x = text.Substring(text.IndexOf(name + "=")).Replace(name + "=", "");
                x = x.Substring(0, x.IndexOf(";"));
                return x;
            }
            return "";
        }

        public static bool exist(string lang)
        {
            if (!data.ContainsKey(lang))
            {
                return false;
            }
            return true;
        }

        public static string finalize(string text, List<string> va)
        {
            string Text = text;
            int position = 0;
            if (va == null)
            {
                return text;
            }
            foreach (string part in va)
            {
                position++;
                Text = Text.Replace("$" + position.ToString(), part);
            }
            return Text;
        }

        public static string get(string item, string language = null, List<string> va = null)
        {
            if (language == null)
            {
                language = Language;
            }
            if (!data.ContainsKey(language))
            {
                return "error - invalid language: " + language;
            }
            if (data[language].Cache.ContainsKey(item))
            {
                return finalize(data[language].Cache[item], va);
            }
            string text = "";
            switch (language)
            {
                case "en":
                    text = Client.Properties.Resources.en_english;
                    break;
                case "cs":
                    //text = MockingJay.Properties.Resources.cs_czech;
                    //break;
                case "zh":
                    //text = wmib.Properties.Resources.zh_chinese;
                    text = "";
                    break;
                default:
                    return "invalid language: " + language;
            }
            string value = parse(text, item);
            if (value == "")
            {
                if (Language != language)
                {
                    return get(item, null, va);
                }
                else
                {
                    return "[" + item + "]";
                }
            }

            data[language].Cache.Add(item, value);
            return finalize(value, va);
        }
    }
}
