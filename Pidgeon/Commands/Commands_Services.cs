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
using Pidgeon.Protocols.Services;

namespace Pidgeon
{
    public static partial class Commands
    {
        private static partial class Generic
        {
            public static void services_flush(string parameter)
            {
                if (Core.SelectedNetwork == null)
                {
                    return;
                }
                if (Core.SelectedNetwork._Protocol.GetType() == typeof(ProtocolSv))
                {
                    Protocols.Services.ProtocolSv protocol = (Protocols.Services.ProtocolSv)Core.SelectedNetwork._Protocol;
                    protocol.sBuffer.Snapshot();
                }
            }

            public static void connect(string parameter)
            {
                if (string.IsNullOrEmpty(parameter))
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                    return;
                }
                string ipv6;
                Core.ExtractIPv6(parameter, out ipv6);
                string name2 = parameter;
                string b2 = parameter.Substring(parameter.IndexOf(name2, StringComparison.Ordinal) + name2.Length);
                int n3;
                if (string.IsNullOrEmpty(name2))
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                    return;
                }
                if (ipv6 != null)
                    name2 = ipv6;
                // we need to fetch the reference here because SelectedNetwork is property that can change from line to line
                libirc.Protocol protocol = null;
                if (Core.SystemForm.Chat._Protocol != null)
                {
                    protocol = Core.SystemForm.Chat._Protocol;
                }
                else
                {
                    Network network = Core.SelectedNetwork;
                    if (network == null || network._Protocol == null)
                    {
                        protocol = network._Protocol;
                    }
                }
                if (protocol != null && protocol.GetType() == typeof(ProtocolSv))
                {
                    if (int.TryParse(b2, out n3))
                    {
                        protocol.ConnectTo(name2, n3);
                        return;
                    }

                    protocol.ConnectTo(name2, 6667);
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
                        Core.SystemForm.Chat.scrollback.InsertText("Removed " + f.Name, Pidgeon.ContentLine.MessageStyle.System, false);
                    }
                    return;
                }
                if (Core.SelectedNetwork._Protocol.GetType() == typeof(ProtocolSv))
                {
                    ProtocolSv protocol = (ProtocolSv)Core.SelectedNetwork._Protocol;
                    protocol.sBuffer.DeleteCache();
                    Core.SystemForm.Chat.scrollback.InsertText("Services cache was cleared", Pidgeon.ContentLine.MessageStyle.System, false);
                }
            }

            public static void ServiceGnick(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    string nick = parameter.Trim();
                    if (Core.SystemForm.Chat._Protocol == null)
                    {
                        return;
                    }
                    if (Core.SystemForm.Chat._Protocol.GetType() == typeof(ProtocolSv))
                    {
                        ((ProtocolSv)Core.SystemForm.Chat._Protocol).RequestGlobalNick(nick);
                    }
                }
            }

            public static void ServiceQuit(string parameter)
            {
                Core.SelectedNetwork._Protocol.Exit();
            }

            public static void pidgeon_service(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    List<string> parameters = new List<string>();
                    parameters.AddRange(parameter.Split(' '));
                    int port = int.Parse(parameters[2]);
                    Connections.ConnectPS(parameters[0], port, parameters[1]);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }),
                    Pidgeon.ContentLine.MessageStyle.Message);
            }
        }
    }
}
