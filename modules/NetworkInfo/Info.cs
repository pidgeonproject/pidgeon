using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class RestrictedModule : Extension
    {
        public override void Initialise()
        {
            Name = "Network data";
            Version = "1.0";
            Description = "This extension enable some extra features based on version of ircd";
            base.Initialise();
        }

        /*
(16:15:54) :hub.tm-irc.org 292 petan : u = Auditorium mode (/names and /who #channel only show channel ops) [q]
         * */
        public override void Hook_NetworkInfo(Network network, string command, string parameters, string value)
        {
            if (command == "002")
            {
                if (value.Contains("running version Unreal"))
                {
                    string version = value.Substring(value.IndexOf("Unreal"));
                    network.IrcdVersion = version;
                    network.InsertSafeDescription('f', "Flood protection (for more info see /HELPOP CHMODEF) [o]");
                    network.InsertSafeDescription('c', "Block messages containing mIRC color codes [o]");
                    network.InsertSafeDescription('i', "A user must be invited to join the channel [h]");
                    network.InsertSafeDescription('j', "Throttle joins per-user to 'joins' per 'sec' seconds [o]");
                    network.InsertSafeDescription('k', "Users must specify <key> to join [h]");
                    network.InsertSafeDescription('l', "Channel may hold at most <number> of users [o]");
                    network.InsertSafeDescription('m', "Moderated channel (only +vhoaq users may speak) [h]");
                    network.InsertSafeDescription('n', "Users outside the channel can not send PRIVMSGs to the channel [h]");
                    network.InsertSafeDescription('p', "Private channel [o]");
                    network.InsertSafeDescription('r', "The channel is registered (settable by services only)");
                    network.InsertSafeDescription('t', "Only +hoaq may change the topic [h]");
                    network.InsertSafeDescription('z', "Only Clients on a Secure Connection (SSL) can join [o]");
                    network.InsertSafeDescription('A', "Server/Net Admin only channel (settable by Admins)");
                    network.InsertSafeDescription('C', "No CTCPs allowed in the channel [o]");
                    network.InsertSafeDescription('G', "Filters out all Bad words in messages with <censored> [o]");
                    network.InsertSafeDescription('M', "Must be using a registered nick (+r), or have voice access to talk [o]");
                    network.InsertSafeDescription('K', "/KNOCK is not allowed [o]");
                    network.InsertSafeDescription('L', "Channel link (If +l is full, the next user will auto-join <chan2>) [q]");
                    network.InsertSafeDescription('N', "No Nickname changes are permitted in the channel [o]");
                    network.InsertSafeDescription('O', "IRC Operator only channel (settable by IRCops)");
                    network.InsertSafeDescription('Q', "No kicks allowed [o]");
                    network.InsertSafeDescription('R', "Only registered (+r) users may join the channel [o]");
                    network.InsertSafeDescription('S', "Strips mIRC color codes [o]");
                    network.InsertSafeDescription('T', "No NOTICEs allowed in the channel [o]");
                    network.InsertSafeDescription('V', "/INVITE is not allowed [o]");
                }

                if (value.Contains("running version ircd-seven"))
                {
                    string version = value.Substring(value.IndexOf("ircd-seven"));
                    network.IrcdVersion = version;

                }
            }
            base.Hook_NetworkInfo(network, command, parameters, value);
        }
    }
}
