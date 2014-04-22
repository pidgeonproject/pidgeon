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
using System.Text;

namespace Pidgeon
{
    class FreenodeTools : Extension
    {
        public override bool Hook_OnLoad()
        {
            Commands.RegisterCommand("kick", new Commands.Command(Commands.Type.Plugin, Kick));
            Commands.RegisterCommand("kq", new Commands.Command(Commands.Type.Plugin, Quiet));
            Commands.RegisterCommand("kb", new Commands.Command(Commands.Type.Plugin, Ban));
            Commands.RegisterCommand("op", new Commands.Command(Commands.Type.Plugin, Op));
            Commands.RegisterCommand("jhostban", new Commands.Command(Commands.Type.Plugin, JoinHostBan));
            Commands.RegisterCommand("jb", new Commands.Command(Commands.Type.Plugin, OpenBan));
            Commands.RegisterCommand("remove", new Commands.Command(Commands.Type.Plugin, Remove));
            return true;
        }

        private void Remove(string text)
        {
            if (text == "")
            {
                Core.SystemForm.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            string reason = Configuration.irc.DefaultReason;
            if (text.Contains(" "))
            {
                reason = text.Substring(text.IndexOf(" " + 1));
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core.SystemForm.Chat.IsChannel)
            {
                Core.SystemForm.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            GetOp(Core.SystemForm.Chat.WindowName);
            System.Threading.Thread.Sleep(100);
            Core.SelectedNetwork.Transfer("REMOVE " + Core.SystemForm.Chat.WindowName + " " + user + " :" + reason, libirc.Defs.Priority.High);
        }

        public override void Initialise()
        {
            Description = "This plugin enable you to use extra commands";
            Version = new Version(2,0,0);
            base.Initialise();
        }

        private void GetOp(string channel)
        {
            Channel channel_ = Core.SelectedNetwork.GetChannel(channel);
            if (channel_ == null)
            {
                return;
            }
            libirc.User user = channel_.GetSelf();
            if (user == null)
            {
                DebugLog("User is null, fail :(");
                return;
            }
            if (!user.IsOp)
            {
                Syslog.DebugLog("Modes: " + channel_.ChannelMode.ToString());
                Core.SelectedNetwork.Transfer("PRIVMSG ChanServ :OP " + channel_.Name, libirc.Defs.Priority.High);
            }
        }

        public void Kick(string text)
        {
            if (text == "")
            {
                Core.SystemForm.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            string reason = Configuration.irc.DefaultReason;
            if (text.Contains(" "))
            {
                reason = text.Substring(text.IndexOf(" " + 1));
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core.SystemForm.Chat.IsChannel)
            {
                Core.SystemForm.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            GetOp(Core.SystemForm.Chat.WindowName);
            System.Threading.Thread.Sleep(100);
            Core.SelectedNetwork.Transfer("KICK " + Core.SystemForm.Chat.WindowName + " " + user + " :" + reason, libirc.Defs.Priority.High);
        }

        public void Op(string text)
        {
            if (!Core.SystemForm.Chat.IsChannel)
            {
                Core.SystemForm.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            Core.SelectedNetwork._Protocol.Transfer("PRIVMSG ChanServ :OP " + Core.SystemForm.Chat.WindowName, libirc.Defs.Priority.High);
        }

        public void Quiet(string text)
        {
            if (text == "")
            {
                Core.SystemForm.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            if (text.Contains(" "))
            {
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core.SystemForm.Chat.IsChannel)
            {
                Core.SystemForm.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            User host = null;
            Channel curr = Core.SelectedNetwork.GetChannel(Core.SystemForm.Chat.WindowName);
            if (curr != null)
            {
                host = curr.UserFromName(user);
                if (host != null)
                {
                    if (host.Host != "")
                    {
                        GetOp(Core.SystemForm.Chat.WindowName);
                        System.Threading.Thread.Sleep(100);
                        Core.SelectedNetwork.Transfer("MODE " + Core.SystemForm.Chat.WindowName + " +q *!*@" + host.Host, libirc.Defs.Priority.High);
                        return;
                    }
                    Core.SystemForm.Chat.scrollback.InsertText("Can't resolve hostname of user", ContentLine.MessageStyle.System, false);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);
                return;
            }
            Core.SystemForm.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);

        }

        public void OpenBan(string text)
        {
            if (text == "")
            {
                Core.SystemForm.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            if (text.Contains(" "))
            {
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core.SystemForm.Chat.IsChannel)
            {
                Core.SystemForm.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            GetOp(Core.SystemForm.Chat.WindowName);
            System.Threading.Thread.Sleep(100);

            Channel curr = Core.SelectedNetwork.GetChannel(Core.SystemForm.Chat.WindowName);
                
            if (curr != null)
            {
                Core.SelectedNetwork.Transfer("MODE " + Core.SystemForm.Chat.WindowName + " +b " + user + "!*@*$##fix_your_connection", libirc.Defs.Priority.High);
                return;
            }
            Core.SystemForm.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);
        }

        public void JoinHostBan(string text)
        {
            if (text == "")
            {
                Core.SystemForm.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            if (text.Contains(" "))
            {
                user = text;
            }
            if (!Core.SystemForm.Chat.IsChannel)
            {
                Core.SystemForm.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            GetOp(Core.SystemForm.Chat.WindowName);
            System.Threading.Thread.Sleep(100);

            Channel curr = Core.SelectedNetwork.GetChannel(Core.SystemForm.Chat.WindowName);
            if (curr != null)
            {
                Core.SelectedNetwork.Transfer("MODE " + Core.SystemForm.Chat.WindowName + " +b *!*@" + user + "$##fix_your_connection", libirc.Defs.Priority.High);
                return;
            }
            Core.SystemForm.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);
        }

        public void Ban(string text)
        {
            if (text == "")
            {
                Core.SystemForm.Chat.scrollback.InsertText("You need to specify at least 1 parameter", ContentLine.MessageStyle.User, false);
                return;
            }
            string user = text;
            string reason = Configuration.irc.DefaultReason;
            if (text.Contains(" "))
            {
                reason = text.Substring(text.IndexOf(" " + 1));
                user = text.Substring(0, text.IndexOf(" "));
            }
            if (!Core.SystemForm.Chat.IsChannel)
            {
                Core.SystemForm.Chat.scrollback.InsertText("This command can be only used in channels", ContentLine.MessageStyle.User, false);
                return;
            }
            GetOp(Core.SystemForm.Chat.WindowName);
            System.Threading.Thread.Sleep(100);
            Core.SelectedNetwork.Transfer("KICK " + Core.SystemForm.Chat.WindowName + " " + user + " :" + reason, libirc.Defs.Priority.High);
            User host = null;
            Channel curr = Core.SelectedNetwork.GetChannel(Core.SystemForm.Chat.WindowName);
            if (curr != null)
            {
                host = curr.UserFromName(user);
                if (host != null)
                {
                    if (host.Host != "")
                    {
                        Core.SelectedNetwork.Transfer("MODE " + Core.SystemForm.Chat.WindowName + " +b *!*@" + host.Host, libirc.Defs.Priority.High);
                        return;
                    }
                    Core.SystemForm.Chat.scrollback.InsertText("Can't resolve hostname of user", ContentLine.MessageStyle.System, false);
                    return;
                }
                Core.SystemForm.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the user", ContentLine.MessageStyle.System, false);
                return;
            }
            Core.SystemForm.Chat.scrollback.InsertText("Unable to ban this user, because I couldn't find the channel in system", ContentLine.MessageStyle.System, false);
        }
    }
}
