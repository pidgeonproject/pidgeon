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
    /// <summary>
    /// Localization
    /// </summary>
    public class messages
    {
        /// <summary>
        /// Language
        /// </summary>
        public class container
        {
            /// <summary>
            /// ID
            /// </summary>
            public string language = null;
            /// <summary>
            /// Values
            /// </summary>
            public Dictionary<string, string> Cache;

            /// <summary>
            /// Creates a new instance of this language container
            /// </summary>
            /// <param name="LanguageCode"></param>
            public container(string LanguageCode)
            {
                language = LanguageCode;
                Cache = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Default language
        /// </summary>
        public static string Language = "en";

        /// <summary>
        /// List of all loaded data
        /// </summary>
        public static Dictionary<string, container> data = new Dictionary<string, container>();

        private static List<Control> GetControls(Control form)
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

        private static List<ToolStripMenuItem> GetMenu(ToolStripMenuItem form)
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

        /// <summary>
        /// Convert a text to localized version
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Localize(string text)
        {
            if (text.StartsWith("[["))
            {
                return get(text, Core.SelectedLanguage);
            }
            return text;
        }

        /// <summary>
        /// Convert all controls in a form to localized version
        /// </summary>
        /// <param name="form"></param>
        public static void Localize(Gtk.Window form)
        {
            foreach (Gtk.Widget widget in form.Children)
            {
                LocalizeWidget(widget);
            }
        }

        /// <summary>
        /// Convert all controls in a widget
        /// </summary>
        /// <param name="widget"></param>
        public static void LocalizeWidget(Gtk.Widget widget)
        {
            if (widget.GetType() == typeof(Gtk.Label))
            {
                Gtk.Label label = (Gtk.Label)widget;
                label.Text = Localize(label.Text);
            }

            if (widget.GetType() == typeof(Gtk.CheckButton))
            {
                Gtk.CheckButton checkButton = (Gtk.CheckButton)widget;
                checkButton.Label = Localize(checkButton.Label);
            }
            LocalizeWChildrens(widget);
        }

        private static void LocalizeWChildrens(Gtk.Widget widget)
        {
            if (widget.GetType() == typeof(Gtk.VBox))
            {
                foreach (Gtk.Widget ch in ((Gtk.VBox)widget).Children)
                {
                    LocalizeWidget(ch);
                }
            }

            if (widget.GetType() == typeof(Gtk.HBox))
            {
                foreach (Gtk.Widget ch in ((Gtk.HBox)widget).Children)
                {
                    LocalizeWidget(ch);
                }
            }

            if (widget.GetType() == typeof(Gtk.Table))
            {
                foreach (Gtk.Widget ch in ((Gtk.Table)widget).Children)
                {
                    LocalizeWidget(ch);
                }
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

        private static string finalize(string text, List<string> va)
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
