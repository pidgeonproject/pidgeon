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
using System.IO;
using System.Text;

namespace Pidgeon
{
    /// <summary>
    /// This class is handling all commands
    /// </summary>
    public static partial class Commands
    {
        private static partial class Generic
        {
            /// <summary>
            /// Alias
            /// </summary>
            /// <param name="parameter"></param>
            public static void LinkCmd(string parameter)
            {
                if (string.IsNullOrEmpty(parameter))
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Pidgeon.ContentLine.MessageStyle.Message);
                    return;
                }

                if (!parameter.Contains(" "))
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Pidgeon.ContentLine.MessageStyle.Message);
                    return;
                }

                string alias = parameter.Substring(0, parameter.IndexOf(" ", StringComparison.Ordinal));
                string command = parameter.Substring(parameter.IndexOf(" ", StringComparison.Ordinal) + 1);

                lock (aliases)
                {
                    aliases.Add(alias, new CommandLink(command));
                }

                Core._Configuration.ConfigSave();
            }

            public static void External(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    string args = "";
                    if (parameter.Contains(" "))
                    {
                        args = parameter.Substring(parameter.IndexOf(" ", StringComparison.Ordinal));
                        parameter = parameter.Substring(0, parameter.IndexOf(" ", StringComparison.Ordinal));
                    }
                    ExternalCommand c = new ExternalCommand(Core.SystemForm.Chat, parameter, args);
                    string[] output = c.Execute().Split('\n');

                    foreach (string line in output)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            Core.SystemForm.Chat.scrollback.InsertText(line, ContentLine.MessageStyle.System);
                        }
                    }
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
            }

            /// <summary>
            /// Open a link
            /// </summary>
            /// <param name="parameter"></param>
            public static void Link(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    if (Core.SystemForm.Chat._Network != null && Core.SystemForm.Chat._Network.ParentSv != null)
                    {
                        Core.ParseLink(parameter, Core.SystemForm.Chat._Network.ParentSv);
                        return;
                    }
                    Core.ParseLink(parameter);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
            }

            public static void External2Text(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    string args = "";
                    if (parameter.Contains(" "))
                    {
                        args = parameter.Substring(parameter.IndexOf(" ", StringComparison.Ordinal));
                        parameter = parameter.Substring(0, parameter.IndexOf(" ", StringComparison.Ordinal));
                    }
                    ExternalCommand c = new ExternalCommand(Core.SystemForm.Chat, parameter, args);
                    string[] output = c.Execute().Split('\n');

                    foreach (string line in output)
                    {
                        if (Core.SystemForm.Chat.textbox.Buffer == null)
                        {
                            Core.SystemForm.Chat.textbox.Buffer = "";
                        }
                        if (!string.IsNullOrEmpty(line))
                        {
                            Core.SystemForm.Chat.textbox.Buffer += line + "\n";
                        }
                    }
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
            }

            /// <summary>
            /// Register a module
            /// </summary>
            /// <param name="parameter"></param>
            public static void RegisterModule(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    if (!Core.RegisterPlugin(parameter))
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("Unable to load the specified plugin", Pidgeon.ContentLine.MessageStyle.System, false);
                        return;
                    }
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
            }

            public static void RetrieveUptime(string parameter)
            {
                TimeSpan uptime = DateTime.Now - Core.LoadTime;
                Core.SystemForm.Chat.scrollback.InsertText(uptime.ToString(), Pidgeon.ContentLine.MessageStyle.System, false, 0, true);
            }

            /// <summary>
            /// Display a manual page
            /// </summary>
            /// <param name="parameter"></param>
            public static void man(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    lock (ManualPages)
                    {
                        if (ManualPages.ContainsKey(parameter))
                        {
                            Core.SystemForm.Chat.scrollback.InsertText(Configuration.Version + " | Manual page for "
                                + parameter + Environment.NewLine + "===================================================================="
                                + Environment.NewLine + Environment.NewLine
                                + ManualPages[parameter]
                                + Environment.NewLine + "===================================================================="
                                + Environment.NewLine + "EOM", Pidgeon.ContentLine.MessageStyle.System, false, 0, true);
                            return;
                        }
                        else
                        {
                            Core.SystemForm.Chat.scrollback.InsertText("This command is unknown", Pidgeon.ContentLine.MessageStyle.Message);
                        }
                    }
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
            }

            /// <summary>
            /// Quit pidgeon
            /// </summary>
            /// <param name="parameter"></param>
            public static void pidgeon_quit(string parameter)
            {
                Core.Quit();
            }

            /// <summary>
            /// Clear a parser cache
            /// </summary>
            /// <param name="parameter"></param>
            public static void ParserCache(string parameter)
            {
                int size = Parser.ParserCache.Count;
                lock (Parser.ParserCache)
                {
                    Parser.ParserCache.Clear();
                }
                Core.SystemForm.Chat.scrollback.InsertText("Parser cache: cleared " + size.ToString() + " items from mem", ContentLine.MessageStyle.System, false);
            }

            /// <summary>
            /// Rehash pidgeon
            /// </summary>
            /// <param name="parameter"></param>
            public static void PidgeonRehash(string parameter)
            {
                Core._Configuration.ConfigurationLoad();
                Core.SystemForm.Chat.scrollback.InsertText("Reloaded config", Pidgeon.ContentLine.MessageStyle.System, false);
            }

            /// <summary>
            /// Process a batch file
            /// </summary>
            /// <param name="parameter"></param>
            public static void pidgeon_batch(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
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
                    Core.SystemForm.Chat.scrollback.InsertText("Warning: file not found: " + path, Pidgeon.ContentLine.MessageStyle.System, false);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
                return;
            }

            /// <summary>
            /// Connect to a server
            /// </summary>
            /// <param name="parameter"></param>
            public static void server(string parameter)
            {
                if (string.IsNullOrEmpty(parameter))
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                    return;
                }
                string name = parameter;
                string port = null;
                if (name.Contains(":"))
                {
                    name = name.Substring(0, name.IndexOf(":", StringComparison.Ordinal));
                    port = parameter.Substring(name.Length + 1);
                }
                int n2;
                bool ssl = false;
                if (string.IsNullOrEmpty(name))
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                    return;
                }

                if (name.StartsWith("$", StringComparison.Ordinal))
                {
                    ssl = true;
                    while (name.StartsWith("$", StringComparison.Ordinal))
                    {
                        name = name.Substring(1);
                    }
                }

                if (Uri.CheckHostName(name) == UriHostNameType.Unknown)
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                    return;
                }

                if (!int.TryParse(port, out n2))
                {
                    n2 = 6667;
                }
                Connections.ConnectIRC(name, n2, "", ssl);
                return;
            }

            /// <summary>
            /// Change nick
            /// </summary>
            /// <param name="parameter"></param>
            public static void nick(string parameter)
            {
                string Nick = parameter;
                if (parameter.Length < 1)
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
                    return;
                }
                if (Core.SelectedNetwork == null)
                {
                    Configuration.UserData.nick = Nick;
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("nick", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.User);
                    return;
                }
                if (!Core.SelectedNetwork.IsConnected)
                {
                    Configuration.UserData.nick = Nick;
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("nick", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.User);
                    return;
                }
                Core.SelectedNetwork._Protocol.RequestNick(Nick);
            }

            /// <summary>
            /// Display timers
            /// </summary>
            /// <param name="paramater"></param>
            public static void displaytmdb(string paramater)
            {
                lock (Core.TimerDB)
                {
                    if (Core.TimerDB.Count == 0)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("No timers to display.", Pidgeon.ContentLine.MessageStyle.System, false);
                        return;
                    }
                    foreach (Timer item in Core.TimerDB)
                    {
                        string status = "running";
                        if (!item.Running)
                        {
                            status = "waiting";
                        }
                        Core.SystemForm.Chat.scrollback.InsertText("Timer ID: " + item.ID.ToString() + " status: " + status + " command: " + item.Command + " time to run " + item.Time.ToString(), Pidgeon.ContentLine.MessageStyle.System, false);
                    }
                }
            }

            /// <summary>
            /// Clear ring
            /// </summary>
            /// <param name="parameter"></param>
            public static void ClearRing(string parameter)
            {
                Core.ClearRingBufferLog();
                Core.SystemForm.Chat.scrollback.InsertText("Ring buffer was cleaned", Pidgeon.ContentLine.MessageStyle.System, false);
            }

            /// <summary>
            /// Create a new timer
            /// </summary>
            /// <param name="parameter"></param>
            public static void timer(string parameter)
            {
                try
                {
                    if (!parameter.Contains(" "))
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Pidgeon.ContentLine.MessageStyle.Message);
                    }
                    int time = 0;
                    string tm = parameter.Substring(0, parameter.IndexOf(" ", StringComparison.Ordinal));
                    string command = parameter.Substring(parameter.IndexOf(" ", StringComparison.Ordinal) + 1);
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
                    Core.HandleException(fail);
                }
            }

            /// <summary>
            /// Sleep for specific time
            /// </summary>
            /// <param name="parameter"></param>
            public static void sleep(string parameter)
            {
                int time = 0;
                if (int.TryParse(parameter, out time))
                {
                    System.Threading.Thread.Sleep(time);
                }
            }

            /// <summary>
            /// Truncate a sniffer log
            /// </summary>
            /// <param name="parameter"></param>
            public static void snifferFree(string parameter)
            {
                Core.TrafficScanner.Clean();
                Core.SystemForm.Chat.scrollback.InsertText("Sniffer log was cleared", Pidgeon.ContentLine.MessageStyle.System, false);
            }

            /// <summary>
            /// Clean up a mem
            /// </summary>
            /// <param name="parameter"></param>
            public static void free(string parameter)
            {
                System.GC.Collect();
                Core.SystemForm.Chat.scrollback.InsertText("Memory was cleaned up", Pidgeon.ContentLine.MessageStyle.System, false);
            }

            /// <summary>
            /// Display ring
            /// </summary>
            /// <param name="parameter"></param>
            public static void ring_show(string parameter)
            {
                Core.PrintRing(Core.SystemForm.Chat, false);
            }

            public static void pidgeon_file(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    if (File.Exists(parameter))
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("This file already exist, use .overwite to overwrite it", Pidgeon.ContentLine.MessageStyle.System, false);
                        return;
                    }

                    string data = "";
                    foreach (string text in Core.RingBuffer)
                    {
                        data += text;
                    }

                    try
                    {
                        File.WriteAllText(parameter, data);
                    }
                    catch (IOException fail)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("Unable to write: " + fail.Message.ToString(), Pidgeon.ContentLine.MessageStyle.System, false);
                        Core.DebugLog("Unable to write: " + fail.ToString());
                    }
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
            }

            public static void forced_pidgeon_file(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    string data = "";
                    foreach (string text in Core.RingBuffer)
                    {
                        data += text;
                    }

                    try
                    {
                        File.WriteAllText(parameter, data);
                    }
                    catch (IOException fail)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("Unable to write: " + fail.Message.ToString(), Pidgeon.ContentLine.MessageStyle.System, false);
                        Core.DebugLog("Unable to write: " + fail.ToString());
                    }
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
            }
        }
    }
}
