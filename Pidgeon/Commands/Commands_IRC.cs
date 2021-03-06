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

namespace Pidgeon
{
    public static partial class Commands
    {
        private static partial class Generic
        {
            public static void join(string parameter)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    string channel = parameter;
                    if (Core.SelectedNetwork == null)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.SelectedNetwork.IsConnected)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                    Channel curr = Core.SelectedNetwork.GetChannel(channel);
                    if (curr != null)
                    {
                        Graphics.Window window = curr.RetrieveWindow();
                        if (window != null)
                        {
                            WindowsManager.ShowChat(Core.SelectedNetwork.SystemWindow + window.WindowName);
                        }
                    }
                    Core.SelectedNetwork._Protocol.Join(channel);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("invalid-channel", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
            }

            public static void raw(string parameter)
            {
                if (Core.SelectedNetwork != null)
                {
                    if (!string.IsNullOrEmpty(parameter))
                    {
                        string text = parameter;
                        if (Core.SelectedNetwork.IsConnected)
                        {
                            Core.SelectedNetwork._Protocol.Command(text);
                            return;
                        }
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
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
                if (!string.IsNullOrEmpty(parameter))
                {
                    string message = parameter;
                    if (Core.SelectedNetwork == null)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.SelectedNetwork.IsConnected)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                    Graphics.Window window = Core.SystemForm.Chat;
                    if (window != null)
                    {
                        if (window.IsChannel || window.IsPrivMsg)
                        {
                            Core.SelectedNetwork.Act(message, window.WindowName);
                            return;
                        }
                        Core.SystemForm.Chat.scrollback.InsertText("You can't use this command in this type of window", ContentLine.MessageStyle.System, false);
                    }
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);

            }

            public static void msg1(string parameter)
            {
                if (parameter.Contains(" "))
                {
                    string channel = parameter.Substring(0, parameter.IndexOf(" ", StringComparison.Ordinal));
                    if (Core.SelectedNetwork != null)
                    {
                        if (Core.SelectedNetwork.IsConnected)
                        {
                            string ms = parameter.Substring(channel.Length);
                            while (ms.StartsWith(" ", StringComparison.Ordinal))
                            {
                                ms = ms.Substring(1);
                            }
                            Core.SelectedNetwork.SystemWindow.scrollback.InsertText("[>> " + channel + "] <" + Core.SelectedNetwork.Nickname + "> " + ms, Pidgeon.ContentLine.MessageStyle.System);
                            Core.SelectedNetwork.Message(ms, channel, libirc.Defs.Priority.Normal);
                            return;
                        }
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Pidgeon.ContentLine.MessageStyle.Message);
            }

            public static void ctcp(string parameter)
            {
                if (parameter.Contains(" "))
                {
                    string[] Params = parameter.Split(' ');
                    if (Core.SelectedNetwork == null)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.SelectedNetwork.IsConnected)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core.SelectedNetwork.Transfer("PRIVMSG " + Params[0] + " :" + Core.SelectedNetwork._Protocol.Separator +
                        Params[1].ToUpper() + Core.SelectedNetwork._Protocol.Separator);
                    Core.SystemForm.Chat.scrollback.InsertText("CTCP to " + Params[0] + " >> " + Params[1], Pidgeon.ContentLine.MessageStyle.Message, false);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Pidgeon.ContentLine.MessageStyle.Message);
            }

            public static void query(string parameter)
            {
                if (parameter.Length == 0)
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
                    return;
                }
                string channel = parameter;
                if (parameter.Contains(" "))
                {
                    channel = parameter.Substring(parameter.IndexOf(" ", StringComparison.Ordinal));
                }
                Network network = Core.SelectedNetwork;
                if (channel.Contains(Core.SelectedNetwork.ChannelPrefix))
                {
                    Core.SystemForm.Chat.scrollback.InsertText("Invalid name", Pidgeon.ContentLine.MessageStyle.System);
                    return;
                }
                while (channel.StartsWith(" ", StringComparison.Ordinal))
                {
                    channel = channel.Substring(1);
                }
                if (channel.Contains(" "))
                {
                    channel = channel.Substring(0, channel.IndexOf(" ", StringComparison.Ordinal));
                    if (network != null && network._Protocol != null)
                    {
                        if (network.IsConnected)
                        {
                            Graphics.Window window = network.GetPrivateUserWindow(channel);
                            WindowsManager.ShowChat(window);
                            window.scrollback.InsertText (Protocol.PRIVMSG (network.Nickname, parameter.Substring(parameter.IndexOf (channel, StringComparison.Ordinal)
                                                          + 1 + channel.Length)), Pidgeon.ContentLine.MessageStyle.Channel);
                            network.Message(parameter.Substring(parameter.IndexOf(channel, StringComparison.Ordinal) + 1 + channel.Length), channel);
                            return;
                        }
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
                    return;
                }
                if (network != null && network._Protocol != null)
                {
                    if (network.IsConnected)
                    {
                        WindowsManager.ShowChat(network.GetPrivateUserWindow(channel));
                        return;
                    }
                }
                Core.SystemForm.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Pidgeon.ContentLine.MessageStyle.Message);
            }
        }
    }
}
