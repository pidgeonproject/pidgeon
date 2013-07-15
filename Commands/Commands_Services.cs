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
using System.Text;

namespace Client
{
    public partial class Commands
    {
        private partial class Generic
        {
            public static void services_flush(string parameter)
            {
                if (Core.SelectedNetwork == null)
                {
                    return;
                }
                if (Core.SelectedNetwork._Protocol.GetType() == typeof(ProtocolSv))
                {
                    ProtocolSv protocol = (ProtocolSv)Core.SelectedNetwork._Protocol;
                    protocol.sBuffer.Snapshot();
                }
            }

            public static void connect(string parameter)
            {
                if (parameter == "")
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                    return;
                }
                string name2 = parameter;
                string b2 = parameter.Substring(parameter.IndexOf(name2) + name2.Length);
                int n3;
                if (name2 == "")
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                    return;
                }
                if (Core.SystemForm.Chat._Protocol == null)
                {
                    return;
                }
                if (Core.SystemForm.Chat._Protocol.GetType() == typeof(ProtocolSv))
                {
                    if (int.TryParse(b2, out n3))
                    {
                        Core.SystemForm.Chat._Protocol.ConnectTo(name2, n3);
                        return;
                    }

                    Core.SystemForm.Chat._Protocol.ConnectTo(name2, 6667);
                    return;
                }
            }

            public static void services_cache(string parameter)
            {
                if (Core.SelectedNetwork == null)
                {
                    return;
                }
                if (Core.SelectedNetwork._Protocol.GetType() == typeof(ProtocolSv))
                {
                    ProtocolSv protocol = (ProtocolSv)Core.SelectedNetwork._Protocol;
                    protocol.sBuffer.PrintInfo();
                }
            }

            public static void services_clear(string parameter)
            {
                if (Core.SelectedNetwork == null)
                {
                    foreach(System.IO.DirectoryInfo f in new System.IO.DirectoryInfo(Core.PermanentTemp).GetDirectories("buffer_*"))
                    {
                        f.Delete(true);
                        Core.SystemForm.Chat.scrollback.InsertText("Removed " + f.Name, Client.ContentLine.MessageStyle.System, false);
                    }
                    return;
                }
                if (Core.SelectedNetwork._Protocol.GetType() == typeof(ProtocolSv))
                {
                    ProtocolSv protocol = (ProtocolSv)Core.SelectedNetwork._Protocol;
                    protocol.sBuffer.Clear();
                    Core.SystemForm.Chat.scrollback.InsertText("Services cache was cleared", Client.ContentLine.MessageStyle.System, false);
                }
            }

            public static void service_gnick(string parameter)
            {
                if (parameter != "")
                {
                    string nick = parameter;
                    if (Core.SystemForm.Chat._Protocol != null)
                    {
                        if (Core.SystemForm.Chat._Protocol.GetType() == typeof(ProtocolSv))
                        {
                            Core.SystemForm.Chat._Protocol.requestNick(nick);
                            return;
                        }
                    }
                    if (Core.SystemForm.Chat._Network != null)
                    {
                        if (Core.SystemForm.Chat._Network._Protocol != null)
                        {
                            if (Core.SystemForm.Chat._Network._Protocol.GetType() == typeof(ProtocolSv))
                            {
                                Core.SystemForm.Chat._Network._Protocol.requestNick(nick);
                                return;
                            }
                        }
                    }
                    return;
                }
            }

            public static void service_quit(string parameter)
            {
                Core.SelectedNetwork._Protocol.Exit();
            }

            public static void pidgeon_service(string parameter)
            {
                if (parameter != "")
                {
                    List<string> parameters = new List<string>();
                    parameters.AddRange(parameter.Split(' '));
                    int port = int.Parse(parameters[2]);
                    Core.ConnectPS(parameters[0], port, parameters[1]);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }),
                    Client.ContentLine.MessageStyle.Message);
            }
        }
    }
}
