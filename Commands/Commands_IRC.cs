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
using System.Linq;
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
                    if (Core.network == null)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.network.Connected)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Channel curr = Core.network.getChannel(channel);
                    if (curr != null)
                    {
                        Graphics.Window window = curr.retrieveWindow();
                        if (window != null)
                        {
                            Core.network._Protocol.ShowChat(Core.network.SystemWindow + window.name);
                        }
                    }
                    Core.network._Protocol.Join(channel);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("invalid-channel", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
            }

            public static void raw(string parameter)
            {
                if (Core.network != null)
                {
                    if (parameter != "")
                    {
                        string text = parameter;
                        if (Core.network.Connected)
                        {
                            Core.network._Protocol.Command(text);
                            return;
                        }
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
            }

            public static void quit(string parameter)
            {
                if (Core.network != null)
                {
                    string reason = Configuration.UserData.quit;
                    if (parameter.Length > 0)
                    {
                        reason = parameter;
                    }
                    Core.network._Protocol.Command("QUIT " + reason);
                }
            }

            public static void msg2(string parameter)
            {
                if (parameter != "")
                {
                    string message = parameter;
                    if (Core.network == null)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.network.Connected)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Channel curr = Core.network.RenderedChannel;
                    if (curr != null)
                    {
                        Graphics.Window window = curr.retrieveWindow();
                        if (window != null)
                        {
                            Core.network._Protocol.Message2(message, curr.Name);
                        }
                    }
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);

            }

            public static void msg1(string parameter)
            {
                if (parameter.Contains(" "))
                {
                    string channel = parameter.Substring(0, parameter.IndexOf(" "));
                    if (Core.network != null)
                    {
                        if (Core.network.Connected)
                        {
                            string ms = parameter.Substring(channel.Length);
                            while (ms.StartsWith(" "))
                            {
                                ms = ms.Substring(1);
                            }
                            Core.network.SystemWindow.scrollback.InsertText("[>> " + channel + "] <" + Core.network.Nickname + "> " + ms, Client.ContentLine.MessageStyle.System);
                            Core.network.Message(ms, channel, Configuration.Priority.Normal, true);
                            return;
                        }
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Client.ContentLine.MessageStyle.Message);
            }

            public static void ctcp(string parameter)
            {
                if (parameter.Contains(" "))
                {
                    string[] Params = parameter.Split(' ');
                    if (Core.network == null)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    if (!Core.network.Connected)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core.network.Transfer("PRIVMSG " + Params[0] + " :" + Core.network._Protocol.delimiter +
                        Params[1].ToUpper() + Core.network._Protocol.delimiter);
                    Core._Main.Chat.scrollback.InsertText("CTCP to " + Params[0] + " >> " + Params[1], Client.ContentLine.MessageStyle.Message, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Client.ContentLine.MessageStyle.Message);
            }

            public static void query(string parameter)
            {
                if (parameter.Length == 0)
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
                    return;
                }
                string channel = parameter;
                if (parameter.Contains(" "))
                {
                    channel = parameter.Substring(parameter.IndexOf(" "));
                }
                if (channel.Contains(Core.network.channel_prefix))
                {
                    Core._Main.Chat.scrollback.InsertText("Invalid name", Client.ContentLine.MessageStyle.System);
                    return;
                }
                while (channel.StartsWith(" "))
                {
                    channel.Substring(1);
                }
                if (channel.Contains(" "))
                {
                    channel = channel.Substring(0, channel.IndexOf(" "));
                    if (Core.network != null && Core.network._Protocol != null)
                    {
                        if (Core.network.Connected)
                        {
                            if (!Core.network._Protocol.Windows.ContainsKey(Core.network.window + channel))
                            {
                                Core.network.Private(channel);
                            }
                            Core.network._Protocol.ShowChat(Core.network.window + channel);
                            Core.network._Protocol.Windows[Core.network.window + channel].scrollback.InsertText(Core.network._Protocol.PRIVMSG(Core.network.Nickname,
                                parameter.Substring(parameter.IndexOf(channel) + 1 + channel.Length)), Client.ContentLine.MessageStyle.Channel);
                            Core.network.Message(parameter.Substring(parameter.IndexOf(channel) + 1 + channel.Length), channel);
                            return;
                        }
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Client.ContentLine.MessageStyle.System);
                    return;
                }
                if (Core.network != null && Core.network._Protocol != null)
                {
                    if (Core.network.Connected)
                    {
                        if (!Core.network._Protocol.Windows.ContainsKey(Core.network.window + channel))
                        {
                            Core.network.Private(channel);
                        }
                        Core.network._Protocol.ShowChat(Core.network.window + channel);
                        return;
                    }
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Client.ContentLine.MessageStyle.Message);
            }
        }
    }
}
