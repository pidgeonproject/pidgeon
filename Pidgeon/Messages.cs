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

namespace Pidgeon
{
    /// <summary>
    /// Localization
    /// </summary>
    public static class messages
    {
        /// <summary>
        /// Language
        /// </summary>
        public class LanguageContainer
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
            public LanguageContainer(string LanguageCode)
            {
                language = LanguageCode;
                Cache = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// language cache
        /// </summary>
        public static class LanguageCache
        {
            private static Dictionary<string, string> text = new Dictionary<string, string>();

            /// <summary>
            /// Download a text for a given language or provide its copy from cache
            /// </summary>
            /// <param name="language"></param>
            /// <returns></returns>
            public static string GetText(string language)
            {
                lock (text)
                {
                    if (text.ContainsKey(language))
                    {
                        return text[language];
                    }
                }

                string value = null;

                if (System.IO.Directory.Exists(Configuration.Kernel.Lang))
                {
                    if (System.IO.File.Exists(Configuration.Kernel.Lang + System.IO.Path.DirectorySeparatorChar + language))
                    {
                        value = System.IO.File.ReadAllText(Configuration.Kernel.Lang + System.IO.Path.DirectorySeparatorChar + language);
                        lock (text)
                        {
                            text.Add(language, value);
                            return value;
                        }
                    }
                }

                switch (language)
                {
                    case "en":
                        value = Pidgeon.Resources.en_english;
                        break;
                    case "cs":
                        value = Pidgeon.Resources.cs_czech;
                        break;
                }

                if (value == null && text.ContainsKey("en"))
                {
                    return text["en"];
                }

                if (System.IO.Directory.Exists(Configuration.Kernel.Lang))
                {
                    System.IO.File.WriteAllText(Configuration.Kernel.Lang + System.IO.Path.DirectorySeparatorChar + language, value);
                }

                lock (text)
                {
                    text.Add(language, value);
                }
                return value;
            }
        }

        /// <summary>
        /// Default language
        /// </summary>
        public static string Language = "en";

        /// <summary>
        /// List of all loaded data
        /// </summary>
        public static Dictionary<string, LanguageContainer> data = new Dictionary<string, LanguageContainer>();

        /// <summary>
        /// Convert a text to localized version
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Localize(string text)
        {
            if (text[0] == '[' && text.StartsWith("[[", StringComparison.Ordinal))
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
        public static void Read(bool SafeMode = false)
        {
            if (SafeMode)
            {
                Syslog.DebugLog("Loading only built-in english, because of safe mode");
                messages.data.Add("en", new messages.LanguageContainer("en"));
                return;
            }
            if (System.IO.Directory.Exists(Configuration.Kernel.Lang))
            {
                foreach (string file in System.IO.Directory.GetFileSystemEntries(Configuration.Kernel.Lang, "*", System.IO.SearchOption.TopDirectoryOnly))
                {
                    string f = Configuration.Kernel.Lang + System.IO.Path.DirectorySeparatorChar + file;
                    if (System.IO.File.Exists(f))
                    {
                        Syslog.DebugLog("Registering a new language " + f);
                        messages.data.Add(f, new messages.LanguageContainer(f));
                    }
                }
            }
            else
            {
                messages.data.Add("cs", new messages.LanguageContainer("cs"));
            }
            if (!messages.data.ContainsKey("en"))
            {
                messages.data.Add("en", new messages.LanguageContainer("en"));
            }
        }

        private static string parse(string text, string name)
        {
            if (text.Contains(name + "="))
            {
                string x = text;
                x = text.Substring(text.IndexOf(name + "=", StringComparison.Ordinal)).Replace(name + "=", "");
                x = x.Substring(0, x.IndexOf(";", StringComparison.Ordinal));
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
            string text = LanguageCache.GetText(language);
            string value = parse(text, item);
            if (string.IsNullOrEmpty(value))
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
