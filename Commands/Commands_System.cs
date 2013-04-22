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
    /// <summary>
    /// This class is handling all commands
    /// </summary>
    public partial class Commands
    {
        private partial class Generic
        {
            public static void module(string parameter)
            {
                if (parameter != "")
                {
                    if (!Core.RegisterPlugin(parameter))
                    {
                        Core._Main.Chat.scrollback.InsertText("Unable to load the specified plugin", Client.ContentLine.MessageStyle.System, false);
                        return;
                    }
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
            }

            public static void up(string parameter)
            {
                TimeSpan uptime = DateTime.Now - Core.LoadTime;
                Core._Main.Chat.scrollback.InsertText(uptime.ToString(), Client.ContentLine.MessageStyle.System, false, 0, true);
            }

            public static void man(string parameter)
            {
                if (parameter != "")
                {
                    lock (ManualPages)
                    {
                        if (ManualPages.ContainsKey(parameter))
                        {
                            Core._Main.Chat.scrollback.InsertText(Configuration.Version + " | Manual page for "
                                + parameter + Environment.NewLine + "===================================================================="
                                + Environment.NewLine + Environment.NewLine
                                + ManualPages[parameter]
                                + Environment.NewLine + "===================================================================="
                                + Environment.NewLine + "EOM", Client.ContentLine.MessageStyle.System, false, 0, true);
                            return;
                        }
                        else
                        {
                            Core._Main.Chat.scrollback.InsertText("This command is unknown", Client.ContentLine.MessageStyle.Message);
                        }
                    }
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
            }

            public static void pidgeon_quit(string parameter)
            {
                Core.Quit();
            }

            public static void parsercache(string parameter)
            {
                int size = Parser.ParserCache.Count;
                lock (Parser.ParserCache)
                {
                    Parser.ParserCache.Clear();
                }
                Core._Main.Chat.scrollback.InsertText("Parser cache: cleared " + size.ToString() + " items from mem", ContentLine.MessageStyle.System, false);
            }

            public static void pidgeon_rehash(string parameter)
            {
                Core.ConfigurationLoad();
                Core._Main.Chat.scrollback.InsertText("Reloaded config", Client.ContentLine.MessageStyle.System, false);
            }

            public static void pidgeon_batch(string parameter)
            {
                if (parameter != "")
                {
                    string path = parameter;
                    if (!System.IO.Path.IsPathRooted(path))
                    {
                        path = System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "scripts" +
                        System.IO.Path.DirectorySeparatorChar + path;
                    }
                    if (System.IO.File.Exists(path))
                    {
                        foreach (string Line in System.IO.File.ReadAllLines(path))
                        {
                            Parser.parse(Line);
                        }
                        return;
                    }
                    if (System.IO.File.Exists(path))
                    {
                        foreach (string Line in System.IO.File.ReadAllLines(path))
                        {
                            Parser.parse(Line);
                        }
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText("Warning: file not found: " + path, Client.ContentLine.MessageStyle.System, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
                return;
            }

            public static void server(string parameter)
            {
                if (parameter == "")
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                    return;
                }
                string name = parameter;
                string port = null;
                if (name.Contains(":"))
                {
                    name = name.Substring(0, name.IndexOf(":"));
                    port = parameter.Substring(name.Length + 1);
                }
                int n2;
                bool ssl = false;
                if (name == "")
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                    return;
                }

                if (name.StartsWith("$"))
                {
                    ssl = true;
                    while (name.StartsWith("$"))
                    {
                        name = name.Substring(1);
                    }
                }

                if (!int.TryParse(port, out n2))
                {
                    n2 = 6667;
                }
                Core.connectIRC(name, n2, "", ssl);
                return;
            }

            public static void nick(string parameter)
            {
                string Nick = parameter;
                if (parameter.Length < 1)
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
                    return;
                }
                if (Core.network == null)
                {
                    Configuration.UserData.nick = Nick;
                    Core._Main.Chat.scrollback.InsertText(messages.get("nick", Core.SelectedLanguage), Client.ContentLine.MessageStyle.User);
                    return;
                }
                if (!Core.network.IsConnected)
                {
                    Configuration.UserData.nick = Nick;
                    Core._Main.Chat.scrollback.InsertText(messages.get("nick", Core.SelectedLanguage), Client.ContentLine.MessageStyle.User);
                    return;
                }
                Core.network._Protocol.requestNick(Nick);
            }

            public static void displaytmdb(string paramater)
            {
                lock (Core.TimerDB)
                {
                    if (Core.TimerDB.Count == 0)
                    {
                        Core._Main.Chat.scrollback.InsertText("No timers to display.", Client.ContentLine.MessageStyle.System, false);
                        return;
                    }
                    foreach (Timer item in Core.TimerDB)
                    {
                        string status = "running";
                        if (!item.Running)
                        {
                            status = "waiting";
                        }
                        Core._Main.Chat.scrollback.InsertText("Timer ID: " + item.ID.ToString() + " status: " + status + " command: " + item.Command + " time to run " + item.Time.ToString(), Client.ContentLine.MessageStyle.System, false);
                    }
                }
            }

            public static void clearring(string parameter)
            {
                Core.ClearRingBufferLog();
                Core._Main.Chat.scrollback.InsertText("Ring buffer was cleaned", Client.ContentLine.MessageStyle.System, false);
            }

            public static void timer(string parameter)
            {
                try
                {
                    if (!parameter.Contains(" "))
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Client.ContentLine.MessageStyle.Message);
                    }
                    int time = 0;
                    string tm = parameter.Substring(0, parameter.IndexOf(" "));
                    string command = parameter.Substring(parameter.IndexOf(" ") + 1);
                    if (int.TryParse(tm, out time))
                    {
                        Timer timer = new Timer(time, command);
                        lock (Core.TimerDB)
                        {
                            Core.TimerDB.Add(timer);
                        }
                        timer.Execute();
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }

            public static void sleep(string parameter)
            {
                int time = 0;
                if (int.TryParse(parameter, out time))
                {
                    System.Threading.Thread.Sleep(time);
                }
            }

            public static void sniffer(string parameter)
            {
                Core.trafficscanner.Clean();
                Core._Main.Chat.scrollback.InsertText("Sniffer log was truncated", Client.ContentLine.MessageStyle.System, false);
            }

            public static void free(string parameter)
            {
                System.GC.Collect();
                Core._Main.Chat.scrollback.InsertText("Memory was cleaned up", Client.ContentLine.MessageStyle.System, false);
            }

            public static void ring_writetologs(string parameter)
            {
                Core.PrintRing(Core._Main.Chat, true);
            }

            public static void ring_show(string parameter)
            {
                Core.PrintRing(Core._Main.Chat, false);
            }

            public static void pidgeon_file(string parameter)
            {
                if (parameter != "")
                {
                    if (System.IO.File.Exists(parameter))
                    {
                        Core._Main.Chat.scrollback.InsertText("This file already exist, use .overwite to overwrite it", Client.ContentLine.MessageStyle.System, false);
                        return;
                    }

                    string data = "";
                    foreach (string text in Core.RingBuffer)
                    {
                        data += text;
                    }

                    try
                    {
                        System.IO.File.WriteAllText(parameter, data);
                    }
                    catch (Exception fail)
                    {
                        Core._Main.Chat.scrollback.InsertText("Unable to write: " + fail.Message.ToString(), Client.ContentLine.MessageStyle.System, false);
                        Core.DebugLog("Unable to write: " + fail.ToString());
                    }
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
            }

            public static void forced_pidgeon_file(string parameter)
            {
                if (parameter != "")
                {
                    string data = "";
                    foreach (string text in Core.RingBuffer)
                    {
                        data += text;
                    }

                    try
                    {
                        System.IO.File.WriteAllText(parameter, data);
                    }
                    catch (Exception fail)
                    {
                        Core._Main.Chat.scrollback.InsertText("Unable to write: " + fail.Message.ToString(), Client.ContentLine.MessageStyle.System, false);
                        Core.DebugLog("Unable to write: " + fail.ToString());
                    }
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
            }
        }
    }
}
