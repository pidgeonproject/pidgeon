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
            public static void join(string parameter)
            {
                if (parameter != "")
                {
                    string channel = parameter;
                    if (Core.SelectedNetwork == null)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.SelectedNetwork.IsConnected)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Channel curr = Core.SelectedNetwork.getChannel(channel);
                    if (curr != null)
                    {
                        Graphics.Window window = curr.RetrieveWindow();
                        if (window != null)
                        {
                            Core.SelectedNetwork._Protocol.ShowChat(Core.SelectedNetwork.SystemWindow + window.WindowName);
                        }
                    }
                    Core.SelectedNetwork._Protocol.Join(channel);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-channel", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
            }

            public static void raw(string parameter)
            {
                if (Core.SelectedNetwork != null)
                {
                    if (parameter != "")
                    {
                        string text = parameter;
                        if (Core.SelectedNetwork.IsConnected)
                        {
                            Core.SelectedNetwork._Protocol.Command(text);
                            return;
                        }
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
            }

            public static void quit(string parameter)
            {
                if (Core.SelectedNetwork != null)
                {
                    string reason = Configuration.UserData.quit;
                    if (parameter.Length > 0)
                    {
                        reason = parameter;
                    }
                    Core.SelectedNetwork._Protocol.Command("QUIT " + reason);
                }
            }

            public static void msg2(string parameter)
            {
                if (parameter != "")
                {
                    string message = parameter;
                    if (Core.SelectedNetwork == null)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.SelectedNetwork.IsConnected)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Graphics.Window window = Core.SystemForm.Chat;
                    if (window != null)
                    {
                        if (window.isChannel || window.isPM)
                        {
                            Core.SelectedNetwork._Protocol.Message2(message, window.WindowName);
                            return;
                        }
                        Core.SystemForm.Chat.scrollback.InsertText("You can't use this command in this type of window", ContentLine.MessageStyle.System, false);
                    }
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);

            }

            public static void msg1(string parameter)
            {
                if (parameter.Contains(" "))
                {
                    string channel = parameter.Substring(0, parameter.IndexOf(" "));
                    if (Core.SelectedNetwork != null)
                    {
                        if (Core.SelectedNetwork.IsConnected)
                        {
                            string ms = parameter.Substring(channel.Length);
                            while (ms.StartsWith(" "))
                            {
                                ms = ms.Substring(1);
                            }
                            Core.SelectedNetwork.SystemWindow.scrollback.InsertText("[>> " + channel + "] <" + Core.SelectedNetwork.Nickname + "> " + ms, Client.ContentLine.MessageStyle.System);
                            Core.SelectedNetwork.Message(ms, channel, Configuration.Priority.Normal, true);
                            return;
                        }
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Client.ContentLine.MessageStyle.Message);
            }

            public static void ctcp(string parameter)
            {
                if (parameter.Contains(" "))
                {
                    string[] Params = parameter.Split(' ');
                    if (Core.SelectedNetwork == null)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.SelectedNetwork.IsConnected)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core.SelectedNetwork.Transfer("PRIVMSG " + Params[0] + " :" + Core.SelectedNetwork._Protocol.delimiter +
                        Params[1].ToUpper() + Core.SelectedNetwork._Protocol.delimiter);
                    Core.SystemForm.Chat.scrollback.InsertText("CTCP to " + Params[0] + " >> " + Params[1], Client.ContentLine.MessageStyle.Message, false);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Client.ContentLine.MessageStyle.Message);
            }

            public static void query(string parameter)
            {
                if (parameter.Length == 0)
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
                    return;
                }
                string channel = parameter;
                if (parameter.Contains(" "))
                {
                    channel = parameter.Substring(parameter.IndexOf(" "));
                }
                if (channel.Contains(Core.SelectedNetwork.channel_prefix))
                {
                    Core.SystemForm.Chat.scrollback.InsertText("Invalid name", Client.ContentLine.MessageStyle.System);
                    return;
                }
                while (channel.StartsWith(" "))
                {
                    channel.Substring(1);
                }
                if (channel.Contains(" "))
                {
                    channel = channel.Substring(0, channel.IndexOf(" "));
                    if (Core.SelectedNetwork != null && Core.SelectedNetwork._Protocol != null)
                    {
                        if (Core.SelectedNetwork.IsConnected)
                        {
                            if (!Core.SelectedNetwork._Protocol.Windows.ContainsKey(Core.SelectedNetwork.SystemWindowID + channel))
                            {
                                Core.SelectedNetwork.Private(channel);
                            }
                            Core.SelectedNetwork._Protocol.ShowChat(Core.SelectedNetwork.SystemWindowID + channel);
                            Core.SelectedNetwork._Protocol.Windows[Core.SelectedNetwork.SystemWindowID + channel].scrollback.InsertText(Core.SelectedNetwork._Protocol.PRIVMSG(Core.SelectedNetwork.Nickname,
                                parameter.Substring(parameter.IndexOf(channel) + 1 + channel.Length)), Client.ContentLine.MessageStyle.Channel);
                            Core.SelectedNetwork.Message(parameter.Substring(parameter.IndexOf(channel) + 1 + channel.Length), channel);
                            return;
                        }
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                    return;
                }
                if (Core.SelectedNetwork != null && Core.SelectedNetwork._Protocol != null)
                {
                    if (Core.SelectedNetwork.IsConnected)
                    {
                        if (!Core.SelectedNetwork._Protocol.Windows.ContainsKey(Core.SelectedNetwork.SystemWindowID + channel))
                        {
                            Core.SelectedNetwork.Private(channel);
                        }
                        Core.SelectedNetwork._Protocol.ShowChat(Core.SelectedNetwork.SystemWindowID + channel);
                        return;
                    }
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
            }
        }
    }
}
