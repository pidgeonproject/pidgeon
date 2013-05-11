using System;
using System.Collections.Generic;
using System.Text;
using Client;

namespace Client
{
    class RestrictedModule : Client.Extension
    {
        public override bool Hook_OnLoad()
        {
            Client.Commands.RegisterCommand("kick", new Client.Commands.Command(Client.Commands.Type.Plugin, Kick));
            Client.Commands.RegisterCommand("kq", new Client.Commands.Command(Client.Commands.Type.Plugin, Quiet));
            Client.Commands.RegisterCommand("kb", new Client.Commands.Command(Client.Commands.Type.Plugin, Ban));
            Client.Commands.RegisterCommand("op", new Client.Commands.Command(Client.Commands.Type.Plugin, Op));
            Client.Commands.RegisterCommand("jhostban", new Client.Commands.Command(Client.Commands.Type.Plugin, JoinHostBan));
            Client.Commands.RegisterCommand("jb", new Client.Commands.Command(Client.Commands.Type.Plugin, OpenBan));
            return true;
        }

        public override void Initialise()
        {
            Name = "Freenode tools";
            Description = "This plugin enable you to use extra commands";
            Version = "1.8.0";
            base.Initialise();
        }

        public void Kick(string text)
        {
            if (text == "")
            {
                Core._Main.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            string reason = Configuration.irc.DefaultReason;
            if (text.Contains(" "))
            {
                reason = text.Substring(text.IndexOf(" " + 1));
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            Core.network.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.WindowName, Configuration.Priority.High);
            System.Threading.Thread.Sleep(100);
            Core.network.Transfer("KICK " + Core._Main.Chat.WindowName + " " + user + " :" + reason, Configuration.Priority.High);
        }

        public void Op(string text)
        {
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            Core.network._Protocol.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.WindowName, Configuration.Priority.High);
        }

        public void Quiet(string text)
        {
            if (text == "")
            {
                Core._Main.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            if (text.Contains(" "))
            {
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            User host = null;
            Channel curr = Core.network.getChannel(Core._Main.Chat.WindowName);
            if (curr != null)
            {
                host = curr.userFromName(user);
                if (host != null)
                {
                    if (host.Host != "")
                    {
                        Core.network.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.WindowName, Configuration.Priority.High);
                        System.Threading.Thread.Sleep(100);
                        Core.network.Transfer("MODE " + Core._Main.Chat.WindowName + " +q *!*@" + host.Host, Configuration.Priority.High);
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText("Can't resolve hostname of user", ContentLine.MessageStyle.System, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);
                return;
            }
            Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);

        }

        public void OpenBan(string text)
        {
            if (text == "")
            {
                Core._Main.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            if (text.Contains(" "))
            {
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            Core.network.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.WindowName, Configuration.Priority.High);
            System.Threading.Thread.Sleep(100);

            Channel curr = Core.network.getChannel(Core._Main.Chat.WindowName);

                User host = null;
                
                if (curr != null)
                {
                    host = curr.userFromName(user);
                    if (host != null)
                    {
                        if (host.Host != "")
                        {
                            Core.network.Transfer("MODE " + Core._Main.Chat.WindowName + " +b *!*@" + host.Host + "$##fix_your_connection", Configuration.Priority.High);
                            return;
                        }
                    }
                    Core._Main.Chat.scrollback.InsertText("Can't resolve hostname of user", ContentLine.MessageStyle.System, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);
        }

        public void JoinHostBan(string text)
        {
            if (text == "")
            {
                Core._Main.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            if (text.Contains(" "))
            {
                user = text;
            }
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            Core.network.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.WindowName, Configuration.Priority.High);
            System.Threading.Thread.Sleep(100);

            Channel curr = Core.network.getChannel(Core._Main.Chat.WindowName);
                if (curr != null)
                {
                            Core.network.Transfer("MODE " + Core._Main.Chat.WindowName + " +b *!*@" + user + "$##fix_your_connection", Configuration.Priority.High);
                            return;
                }
                Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);
        }

        public void Ban(string text)
        {
            if (text == "")
            {
                Core._Main.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            string reason = Configuration.irc.DefaultReason;
            if (text.Contains(" "))
            {
                reason = text.Substring(text.IndexOf(" " + 1));
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            Core.network.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.WindowName, Configuration.Priority.High);
            System.Threading.Thread.Sleep(100);
            Core.network.Transfer("KICK " + Core._Main.Chat.WindowName + " " + user + " :" + reason, Configuration.Priority.High);
            User host = null;
            Channel curr = Core.network.getChannel(Core._Main.Chat.WindowName);
            if (curr != null)
            {
                host = curr.userFromName(user);
                if (host != null)
                {
                    if (host.Host != "")
                    {
                        Core.network.Transfer("MODE " + Core._Main.Chat.WindowName + " +b *!*@" + host.Host, Configuration.Priority.High);
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText("Can't resolve hostname of user", ContentLine.MessageStyle.System, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the user", ContentLine.MessageStyle.System, false);
                return;
            }
            Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);
        }
    }
}
