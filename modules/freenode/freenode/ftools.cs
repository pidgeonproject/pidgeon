using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Client;

namespace Client
{
    class RestrictedModule : Client.Extension
    {
        public override bool Hook_OnLoad()
        {
            Client.Commands.commands.Add("kick", new Client.Commands.Command(Client.Commands.Type.Plugin, Kick));
            Client.Commands.commands.Add("kq", new Client.Commands.Command(Client.Commands.Type.Plugin, Quiet));
            Client.Commands.commands.Add("kb", new Client.Commands.Command(Client.Commands.Type.Plugin, Ban));
            return true;
        }

        public override void Initialise()
        {
            Name = "Freenode tools";
            Description = "This plugin enable you to use extra commands";
            Version = "1.0.0";
            base.Initialise();
        }

        public void Kick(string text)
        {
            if (text == "")
            {
                Core._Main.Chat.scrollback.InsertText("You need to specify at least 1 parameter", Scrollback.MessageStyle.User, false);
                return;
            }
            string user = text;
            string reason = Configuration.DefaultReason;
            if (text.Contains(" "))
            {
                reason = text.Substring(text.IndexOf(" " + 1));
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", Scrollback.MessageStyle.User, false);
                return;
            }
            Core.network._protocol.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.name, Configuration.Priority.High);
            System.Threading.Thread.Sleep(100);
            Core.network._protocol.Transfer("KICK " + Core._Main.Chat.name + " " + user + " :" + reason, Configuration.Priority.High);
        }

        public void Quiet(string text)
        {
            if (text == "")
            {
                Core._Main.Chat.scrollback.InsertText("You need to specify at least 1 parameter", Scrollback.MessageStyle.User, false);
                return;
            }
            string user = text;
            string reason = Configuration.DefaultReason;
            if (text.Contains(" "))
            {
                reason = text.Substring(text.IndexOf(" " + 1));
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", Scrollback.MessageStyle.User, false);
                return;
            }
            User host = null;
            Channel curr = Core.network.getChannel(Core._Main.Chat.name);
            if (curr != null)
            {
                host = curr.userFromName(user);
                if (host != null)
                {
                    if (host.Host != "")
                    {
                        Core.network._protocol.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.name, Configuration.Priority.High);
                        System.Threading.Thread.Sleep(100);
                        Core.network._protocol.Transfer("MODE " + Core._Main.Chat.name + " +q *!*@" + host.Host, Configuration.Priority.High);
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText("Can't resolve hostname of user", Scrollback.MessageStyle.System, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", Scrollback.MessageStyle.System, false);
                return;
            }
            Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", Scrollback.MessageStyle.System, false);

        }

        public void Ban(string text)
        {
            if (text == "")
            {
                Core._Main.Chat.scrollback.InsertText("You need to specify at least 1 parameter", Scrollback.MessageStyle.User, false);
                return;
            }
            string user = text;
            string reason = Configuration.DefaultReason;
            if (text.Contains(" "))
            {
                reason = text.Substring(text.IndexOf(" " + 1));
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core._Main.Chat.isChannel)
            {
                Core._Main.Chat.scrollback.InsertText("This command can be only used in channels", Scrollback.MessageStyle.User, false);
                return;
            }
            Core.network._protocol.Transfer("PRIVMSG ChanServ :OP " + Core._Main.Chat.name, Configuration.Priority.High);
            System.Threading.Thread.Sleep(100);
            Core.network._protocol.Transfer("KICK " + Core._Main.Chat.name + " " + user + " :" + reason, Configuration.Priority.High);
            User host = null;
            Channel curr = Core.network.getChannel(Core._Main.Chat.name);
            if (curr != null)
            {
                host = curr.userFromName(user);
                if (host != null)
                {
                    if (host.Host != "")
                    {
                        Core.network._protocol.Transfer("MODE " + Core._Main.Chat.name + " +b *!*@" + host.Host, Configuration.Priority.High);
                        return;
                    }
                    Core._Main.Chat.scrollback.InsertText("Can't resolve hostname of user", Scrollback.MessageStyle.System, false);
                    return;
                }
                Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", Scrollback.MessageStyle.System, false);
                return;
            }
            Core._Main.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", Scrollback.MessageStyle.System, false);
        }
    }
}
