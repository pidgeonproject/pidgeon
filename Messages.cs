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
using System.Windows.Forms;
using System.Text;

namespace Client
{
    public class messages
    {
        /// <summary>
        /// Default language
        /// </summary>
        public static string Language = "en";

        public class container
        {
            public string language = null;
            public Dictionary<string, string> Cache;
            public container(string LanguageCode)
            {
                language = LanguageCode;
                Cache = new Dictionary<string, string>();
            }
        }

        public static Dictionary<string, container> data = new Dictionary<string, container>();

        public static List<Control> GetControls(Control form)
        {
            var controlList = new List<Control>();

            foreach (Control childControl in form.Controls)
            {
                // Recurse child controls.
                controlList.AddRange(GetControls(childControl));
                controlList.Add(childControl);
            }
            return controlList;
        }

        public static List<ToolStripMenuItem> GetMenu(ToolStripMenuItem form)
        {
            var controlList = new List<ToolStripMenuItem>();

            foreach (ToolStripItem childControl in form.DropDownItems)
            {
                // Recurse child controls.
                if (typeof(ToolStripMenuItem) == childControl.GetType())
                {
                    controlList.AddRange(GetMenu((ToolStripMenuItem)childControl));
                    controlList.Add((ToolStripMenuItem)childControl);
                }
            }
            return controlList;
        }

        public static void Localize(Form form)
        {
            try
            {
                lock (form.Controls)
                {
                    foreach (Control control in GetControls(form))
                    {
                        if (control.Text.StartsWith("[["))
                        {
                            control.Text = get(control.Text);
                        }
                        if (control.GetType() == typeof(MenuStrip))
                        {
                            MenuStrip menu = (MenuStrip)control;
                            foreach (ToolStripMenuItem item in menu.Items)
                            {
                                if (item.Text.StartsWith("[["))
                                {
                                    item.Text = get(item.Text);
                                }
                                foreach (ToolStripMenuItem item2 in GetMenu(item))
                                    if (item2.Text.StartsWith("[["))
                                    {
                                        item2.Text = get(item2.Text);
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Load all languages
        /// </summary>
        public static void Read()
        {
            messages.data.Add("en", new messages.container("en"));
            messages.data.Add("cs", new messages.container("cs"));
        }

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
            lock (data)
            {
                if (!data.ContainsKey(lang))
                {
                    return false;
                }
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

        /// <summary>
        /// Return a string for the current language, in case it doesn't exist it fallback to default language, otherwise return key name
        /// </summary>
        /// <param name="item">Key</param>
        /// <param name="language">Language, if no language is specified, the currently selected default language is used</param>
        /// <param name="va">Parameters</param>
        /// <returns></returns>
        public static string get(string item, string language = null, List<string> va = null)
        {
            try
            {
                item = item.Replace("]", "").Replace("[", "");
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
                        text = Client.Properties.Resources.cs_czech;
                        break;
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
            catch (Exception messagesys_error)
            {
                Core.handleException(messagesys_error);
                return "";
            }
        }
    }
}
