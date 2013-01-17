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
    public class Commands
    {
        public class Command
        {
            private Type type;
            public Type _Type
            {
                get
                {
                    return type;
                }
            }
            private Action<string> action = null;
            public virtual void Launch(string parameter = null)
            {
                if (action != null)
                {
                    action(parameter);
                }
            }

            public virtual void Register()
            {
                // this function is called when command is created
            }

            public virtual void Unregister()
            {
                // this function is called when command is deleted
            }

            public Command(Type _type, Action<string> _Action = null)
            {
                type = _type;
                action = _Action;
                Register();
            }

            ~Command()
            {
                Unregister();
            }
        }

        public enum Type
        {
            System,
            SystemSv,
            Services,
            Network,
            Plugin,
            User,
        }

        public static Dictionary<string, Command> commands = new Dictionary<string, Command>();

        public static void RegisterCommand(Command command, string Name)
        {
            Core.DebugLog("Registering a new command by extension: " + Name);
            commands.Add(Name, command);
        }

        public static void Initialise()
        {
            try
            {
                commands.Add("server", new Command(Type.System, Generic.server));
                commands.Add("nick", new Command(Type.System, Generic.nick));
                commands.Add("connect", new Command(Type.Services, Generic.connect));
                commands.Add("join", new Command(Type.SystemSv, Generic.join));
                commands.Add("part", new Command(Type.Network));
                commands.Add("quit", new Command(Type.System, Generic.quit));
                commands.Add("query", new Command(Type.SystemSv, Generic.query));
                commands.Add("me", new Command(Type.SystemSv, Generic.msg2));
                commands.Add("msg", new Command(Type.SystemSv, Generic.msg1));
                commands.Add("away", new Command(Type.Network));
                commands.Add("mode", new Command(Type.Network));
                commands.Add("help", new Command(Type.Network));
                commands.Add("info", new Command(Type.Network));
                commands.Add("invite", new Command(Type.Network));
                commands.Add("ison", new Command(Type.Network));
                commands.Add("kline", new Command(Type.Network));
                commands.Add("knock", new Command(Type.Network));
                commands.Add("kill", new Command(Type.Network));
                commands.Add("links", new Command(Type.Network));
                commands.Add("list", new Command(Type.Network));
                commands.Add("names", new Command(Type.Network));
                commands.Add("namesx", new Command(Type.Network));
                commands.Add("ping", new Command(Type.Network));
                commands.Add("rehash", new Command(Type.Network));
                commands.Add("restart", new Command(Type.Network));
                commands.Add("service", new Command(Type.Network));
                commands.Add("servlist", new Command(Type.Network));
                commands.Add("squit", new Command(Type.Network));
                commands.Add("setname", new Command(Type.Network));
                commands.Add("silence", new Command(Type.Network));
                commands.Add("stats", new Command(Type.Network));
                commands.Add("summon", new Command(Type.Network));
                commands.Add("topic", new Command(Type.Network));
                commands.Add("trace", new Command(Type.Network));
                commands.Add("user", new Command(Type.Network));
                commands.Add("userip", new Command(Type.Network));
                commands.Add("version", new Command(Type.Network));
                commands.Add("wallops", new Command(Type.Network));
                commands.Add("oper", new Command(Type.Network));
                commands.Add("who", new Command(Type.Network));
                commands.Add("whois", new Command(Type.Network));
                commands.Add("whowas", new Command(Type.Network));
                commands.Add("gline", new Command(Type.Network));
                commands.Add("zline", new Command(Type.Network));
                commands.Add("nickserv", new Command(Type.Network));
                commands.Add("raw", new Command(Type.System, Generic.raw));
                commands.Add("chanserv", new Command(Type.Network));
                commands.Add("ctcp", new Command(Type.SystemSv, Generic.ctcp));
                commands.Add("service.quit", new Command(Type.Services, Generic.service_quit));
                commands.Add("service.gnick", new Command(Type.Services, Generic.service_gnick));
                commands.Add("pidgeon.service", new Command(Type.System, Generic.pidgeon_service));
                commands.Add("pidgeon.quit", new Command(Type.System, Generic.pidgeon_quit));
                commands.Add("pidgeon.rehash", new Command(Type.System, Generic.pidgeon_rehash));
                commands.Add("pidgeon.batch", new Command(Type.System, Generic.pidgeon_batch));
                commands.Add("pidgeon.memory.clean.ring", new Command(Type.System, Generic.clearring));
                commands.Add("pidgeon.memory.clean.gc", new Command(Type.System, Generic.free));
                commands.Add("pidgeon.memory.clean.traffic", new Command(Type.System, Generic.sniffer));
                commands.Add("pidgeon.module", new Command(Type.System, Generic.module));
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public static bool Proccess(string command)
        {
            string[] values = command.Split(' ');
            string parameter = command.Substring(values[0].Length);
            if (parameter.Length > 0)
            {
                parameter = parameter.Substring(1);
            }
            if (commands.ContainsKey(values[0]))
            {
                // network commands are handled by server
                if (commands[values[0]]._Type == Type.Network)
                {
                    return false;
                }
                commands[values[0]].Launch(parameter);
                return true;
            }
            return false;
        }

        private class Generic
        {
            public static void join(string parameter)
            {
                if (parameter != "")
                {
                    string channel = parameter;
                    if (Core.network == null)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                    if (!Core.network.Connected)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                    Channel curr = Core.network.getChannel(channel);
                    if (curr != null)
                    {
                        Window window = curr.retrieveWindow();
                        if (window != null)
                        {
                            Core.network._protocol.ShowChat(Core.network.window + window.name);
                        }
                    }
                    Core.network._protocol.Join(channel);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("invalid-channel", Core.SelectedLanguage), Scrollback.MessageStyle.System);
            }

            public static void module(string parameter)
            {
                if (parameter != "")
                {
                    if (!Core.RegisterPlugin(parameter))
                    {
                        Core._Main.Chat.scrollback.InsertText("Unable to load the specified plugin", Scrollback.MessageStyle.System, false);
                        return;
                    }
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Scrollback.MessageStyle.Message);
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
                            Core.network._protocol.Command(text);
                            return;
                        }
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
            }

            public static void quit(string parameter)
            {
                if (Core.network != null)
                {
                    string reason = Configuration.quit;
                    if (parameter.Length > 0)
                    {
                        reason = parameter;
                    }
                    Core.network._protocol.Command("QUIT " + reason);
                }
            }

            public static void pidgeon_quit(string parameter)
            {
                Core.Quit();
            }

            public static void pidgeon_rehash(string parameter)
            {
                Core.ConfigurationLoad();
                Core._Main.Chat.scrollback.InsertText("Reloaded config", Scrollback.MessageStyle.System, false);
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
                    Core._Main.Chat.scrollback.InsertText("Warning: file not found: " + path, Scrollback.MessageStyle.System, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Scrollback.MessageStyle.Message);
                return;
            }

            public static void server(string parameter)
            {
                if (parameter == "")
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                    return;
                }
                string name = parameter;
                string b = parameter.Substring(parameter.IndexOf(name) + name.Length);
                int n2;
                if (name == "")
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                    return;
                }
                if (int.TryParse(b, out n2))
                {
                    Core.connectIRC(name, n2);
                    return;
                }
                Core.connectIRC(name);
            }

            public static void nick(string parameter)
            {
                string Nick = parameter;
                if (parameter.Length < 1)
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Scrollback.MessageStyle.Message);
                    return;
                }
                if (Core.network == null)
                {
                    Configuration.nick = Nick;
                    Core._Main.Chat.scrollback.InsertText(messages.get("nick", Core.SelectedLanguage), Scrollback.MessageStyle.User);
                    return;
                }
                if (!Core.network.Connected)
                {
                    Configuration.nick = Nick;
                    Core._Main.Chat.scrollback.InsertText(messages.get("nick", Core.SelectedLanguage), Scrollback.MessageStyle.User);
                    return;
                }
                Core.network._protocol.requestNick(Nick);
            }

            public static void connect(string parameter)
            {
                if (parameter == "")
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                    return;
                }
                string name2 = parameter;
                string b2 = parameter.Substring(parameter.IndexOf(name2) + name2.Length);
                int n3;
                if (name2 == "")
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("invalid-server", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                    return;
                }
                if (Core._Main.Chat._Protocol == null)
                {
                    return;
                }
                if (Core._Main.Chat._Protocol.type == 3)
                {
                    if (int.TryParse(b2, out n3))
                    {
                        Core._Main.Chat._Protocol.ConnectTo(name2, n3);
                        return;
                    }

                    Core._Main.Chat._Protocol.ConnectTo(name2, 6667);
                    return;
                }
            }

            public static void msg2(string parameter)
            {
                if (parameter != "")
                {
                    string message = parameter;
                    if (Core.network == null)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                    if (!Core.network.Connected)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                    Channel curr = Core.network.RenderedChannel;
                    if (curr != null)
                    {
                        Window window = curr.retrieveWindow();
                        if (window != null)
                        {
                            Core.network._protocol.Message2(message, curr.Name);
                        }
                    }
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Scrollback.MessageStyle.Message);

            }

            public static void msg1(string parameter)
            {
                if (parameter.Contains(" "))
                {
                    string channel = parameter.Substring(parameter.IndexOf(" "));
                    if (Core.network != null)
                    {
                        if (Core.network.Connected)
                        {
                            string ms = parameter.Substring(channel.Length);
                            while (ms.StartsWith(" "))
                            {
                                ms = ms.Substring(1);
                            }
                            Core.network.system.scrollback.InsertText("[>> " + channel + "] <" + Core.network.nickname + "> " + ms, Scrollback.MessageStyle.System);
                            Core.network._protocol.Message(ms, channel, Configuration.Priority.Normal, true);
                            return;
                        }
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Scrollback.MessageStyle.Message);
            }

            public static void ctcp(string parameter)
            {
                if (parameter.Contains(" "))
                {
                    string[] Params = parameter.Split(' ');
                    if (Core.network == null)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                    if (!Core.network.Connected)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                    Core.network._protocol.Transfer("PRIVMSG " + Params[0] + " :" + Core.network._protocol.delimiter +
                        Params[2].ToUpper() + Core.network._protocol.delimiter);
                    Core._Main.Chat.scrollback.InsertText("CTCP to " + Params[0] + " >> " + Params[1], Scrollback.MessageStyle.Message, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "2" }), Scrollback.MessageStyle.Message);
            }

            public static void clearring(string parameter)
            {
                Core.ClearRingBufferLog();
                Core._Main.Chat.scrollback.InsertText("Ring buffer was cleaned", Scrollback.MessageStyle.System, false);
            }

            public static void sniffer(string parameter)
            {
                Core.trafficscanner.Clean();
                Core._Main.Chat.scrollback.InsertText("Sniffer log was truncated", Scrollback.MessageStyle.System, false);
            }

            public static void free(string parameter)
            {
                System.GC.Collect();
                Core._Main.Chat.scrollback.InsertText("Memory was cleaned up", Scrollback.MessageStyle.System, false);
            }

            public static void query(string parameter)
            {
                if (parameter.Length == 0)
                {
                    Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Scrollback.MessageStyle.Message);
                    return;
                }
                string channel = parameter;
                if (parameter.Contains(" "))
                {
                    channel = parameter.Substring(parameter.IndexOf(" "));
                }
                if (channel.Contains(Core.network.channel_prefix))
                {
                    Core._Main.Chat.scrollback.InsertText("Invalid name", Scrollback.MessageStyle.System);
                    return;
                }
                while (channel.StartsWith(" "))
                {
                    channel.Substring(1);
                }
                if (channel.Contains(" "))
                {
                    channel = channel.Substring(0, channel.IndexOf(" "));
                    if (Core.network != null && Core.network._protocol != null)
                    {
                        if (Core.network.Connected)
                        {
                            if (!Core.network._protocol.windows.ContainsKey(Core.network.window + channel))
                            {
                                Core.network.Private(channel);
                            }
                            Core.network._protocol.ShowChat(Core.network.window + channel);
                            Core.network._protocol.windows[Core.network.window + channel].scrollback.InsertText(Core.network._protocol.PRIVMSG(Core.network.nickname, parameter.Substring(parameter.IndexOf(channel) + 1 + channel.Length)), Scrollback.MessageStyle.Channel);
                            Core.network._protocol.Message(parameter.Substring(parameter.IndexOf(channel) + 1 + channel.Length), channel);
                            return;
                        }
                        Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText(messages.get("error1", Core.SelectedLanguage), Scrollback.MessageStyle.System);
                    return;
                }
                if (Core.network != null && Core.network._protocol != null)
                {
                    if (Core.network.Connected)
                    {
                        if (!Core.network._protocol.windows.ContainsKey(Core.network.window + channel))
                        {
                            Core.network.Private(channel);
                        }
                        Core.network._protocol.ShowChat(Core.network.window + channel);
                        return;
                    }
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }), Scrollback.MessageStyle.Message);
            }

            public static void service_gnick(string parameter)
            {
                if (parameter != "")
                {
                    string nick = parameter;
                    if (Core._Main.Chat._Protocol != null)
                    {
                        if (Core._Main.Chat._Protocol.type == 3)
                        {
                            Core._Main.Chat._Protocol.requestNick(nick);
                            return;
                        }
                    }
                    return;
                }
            }

            public static void service_quit(string parameter)
            {
                Core.network._protocol.Exit();
            }

            public static void pidgeon_service(string parameter)
            {
                if (parameter != "")
                {
                    List<string> parameters = new List<string>();
                    parameters.AddRange(parameter.Split(' '));
                    int port = int.Parse(parameters[2]);
                    Core.connectPS(parameters[0], port, parameters[1]);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText(messages.get("command-wrong", Core.SelectedLanguage, new List<string> { "1" }),
                    Scrollback.MessageStyle.Message);
            }
        }
    }
}
