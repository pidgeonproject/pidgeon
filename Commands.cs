using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class Commands
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

        public static void Initialise()
        {
            commands.Add("pidgeon.quit", new Command(Type.System, Generic.pidgeon_quit));
            commands.Add("pidgeon.rehash", new Command(Type.System, Generic.pidgeon_rehash));
            commands.Add("pidgeon.batch", new Command(Type.System, Generic.pidgeon_batch));
            commands.Add("server", new Command(Type.System, Generic.server));
            commands.Add("nick", new Command(Type.System, Generic.nick));
            commands.Add("connect", new Command(Type.Services, Generic.connect));
            commands.Add("join", new Command(Type.SystemSv, Generic.join));
            commands.Add("part", new Command(Type.Network));
            commands.Add("quit", new Command(Type.System, Generic.quit));
            commands.Add("squit", new Command(Type.Network));
            commands.Add("query", new Command(Type.SystemSv, Generic.query));
            commands.Add("me", new Command(Type.SystemSv, Generic.msg2));
            commands.Add("msg", new Command(Type.SystemSv, Generic.msg1));
            commands.Add("mode", new Command(Type.Network));
            commands.Add("oper", new Command(Type.Network));
            commands.Add("who", new Command(Type.Network));
            commands.Add("whois", new Command(Type.Network));
            commands.Add("whowas", new Command(Type.Network));
            commands.Add("help", new Command(Type.Network));
            commands.Add("list", new Command(Type.Network));
            commands.Add("topic", new Command(Type.Network));
            commands.Add("kill", new Command(Type.Network));
            commands.Add("gline", new Command(Type.Network));
            commands.Add("kline", new Command(Type.Network));
            commands.Add("zline", new Command(Type.Network));
            commands.Add("away", new Command(Type.Network));
            commands.Add("stats", new Command(Type.Network));
            commands.Add("nickserv", new Command(Type.Network));
            commands.Add("chanserv", new Command(Type.Network));
            commands.Add("ctcp", new Command(Type.SystemSv, Generic.ctcp));
            commands.Add("pidgeon.service", new Command(Type.System, Generic.pidgeon_service));
            commands.Add("service.quit", new Command(Type.Services, Generic.service_quit));
            commands.Add("service.gnick", new Command(Type.Services, Generic.service_gnick));
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
                    if (System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "scripts"
                        + System.IO.Path.DirectorySeparatorChar + path))
                    {
                        foreach (string Line in System.IO.File.ReadAllLines(System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar
                            + "scripts" + System.IO.Path.DirectorySeparatorChar + path))
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
                    Core._Main.Chat.scrollback.InsertText("Warning: file not found", Scrollback.MessageStyle.System, false);
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
