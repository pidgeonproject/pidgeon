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
    public class NetworkData : Extension
    {
        public override void Initialise()
        {
            Description = "This extension enable some extra features based on version of ircd";
            base.Initialise();
        }

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
                    return;
                }

                if (value.Contains("running version ircd-seven"))
                {
                    string version = value.Substring(value.IndexOf("ircd-seven"));
                    network.IrcdVersion = version;
                    network.InsertSafeDescription('c', "No color.  All color codes in messages are stripped");
                    network.InsertSafeDescription('i', "A user must be invited to join the channel");
                    network.InsertSafeDescription('m', "Moderated channel (only +vo users may speak)");
                    network.InsertSafeDescription('n', "No external messages.  Only channel members may talk in the channel");
                    network.InsertSafeDescription('p', "Private channel");
                    network.InsertSafeDescription('r', "Registered users only.  Only users identified to services may join");
                    network.InsertSafeDescription('t', "Only +o may change the topic");
                    network.InsertSafeDescription('z', "Op moderated.  Messages blocked by +m, +b and +q are instead sent to ops");
                    network.InsertSafeDescription('g', "Free invite.  Everyone may invite users.  Significantly weakens +i control");
                    network.InsertSafeDescription('C', "Disable CTCP. All CTCP messages to the channel, except ACTION, are disallowed");
                    network.InsertSafeDescription('L', "Large ban list.  Increase maximum number of +beIq entries. Only settable by opers");
                    network.InsertSafeDescription('P', "Permanent.  Channel does not disappear when empty.  Only settable by opers");
                    network.InsertSafeDescription('j', "Join throttle.  Limits number of joins to the channel per time PARAMS: /mode #channel +j count:time");
                    network.InsertSafeDescription('f', "Forward.  Forwards users who cannot join because of +i, +j, +l or +r.");
                    network.InsertSafeDescription('k', "Key.  Requires users to issue /join #channel KEY to join");
                    network.InsertSafeDescription('l', "Limit.  Impose a maximum number of LIMIT people in the channel. PARAMS: /mode #channel +l limit");
                    network.InsertSafeDescription('Q', "Disable forward.  Users cannot be forwarded to the channel (however, new forwards can still be set subject to +F)");
                    network.InsertSafeDescription('F', "Free target.  Anyone may set forwards to this (otherwise ops are necessary)");
                    return;
                }

                if (value.Contains("running version InspIRCd"))
                {
                    string version = value.Substring(value.IndexOf("InspIRCd"));
                    network.IrcdVersion = version;
                    network.InsertSafeDescription('f', "Kicks on text flood equal to or above the specified rate. With *, the user is banned (requires messageflood module)");
                    network.InsertSafeDescription('c', "Blocks messages containing mIRC color codes");
                    network.InsertSafeDescription('i', "Makes the channel invite-only");
                    network.InsertSafeDescription('j', "Limits joins to the specified rate (requires joinflood module)");
                    network.InsertSafeDescription('k', "Users must specify <key> to join");
                    network.InsertSafeDescription('l', "Channel may hold at most <number> of users");
                    network.InsertSafeDescription('m', "Enable moderation. Only users with +v, +h, or +o can speak");
                    network.InsertSafeDescription('s', "Make channel secret, hiding it in users' whoises and /LIST");
                    network.InsertSafeDescription('n', "Blocks users who are not members of the channel from messaging it");
                    network.InsertSafeDescription('p', "Make channel private, hiding it in users' whoises and replacing it with * in /LIST.");
                    network.InsertSafeDescription('r', "Marks the channel as registered with Services (requires services account module)");
                    network.InsertSafeDescription('t', "Only +hoaq may change the topic");
                    network.InsertSafeDescription('z', "Only Clients on a Secure Connection (SSL) can join");
                    network.InsertSafeDescription('A', "Allows anyone to invite users to the channel (normally only chanops can invite, requires allowinvite module)");
                    network.InsertSafeDescription('C', "Blocks any CTCPs to the channel (requires noctcp module)");
                    network.InsertSafeDescription('G', "Filters out all Bad words in messages with <censored>");
                    network.InsertSafeDescription('M', "Blocks unregistered users from speaking (requires services account module)");
                    network.InsertSafeDescription('K', "/KNOCK is not allowed");
                    network.InsertSafeDescription('N', "Prevents users on the channel from chainging nick (requires nonicks module)");
                    network.InsertSafeDescription('O', "Channel is IRCops only (can only be set by IRCops, requires operchans module)");
                    network.InsertSafeDescription('S', "Strips mIRC color codes");
                    network.InsertSafeDescription('u', "Makes the channel an auditorium; normal users only see themselves or themselves and the operators, while operators see all the users (requires auditorium module)");
                    network.InsertSafeDescription('d', "Blocks messages to a channel from new users until they have been in the channel for [time] seconds (requires delaymsg module).");
                    network.InsertSafeDescription('w', "Adds basic channel access controls of [flag] to [banmask], via the +w listmode. For example, +w o:R:Brain will op anyone identified to the account 'Brain' on join. (requires autoop module)");
                    network.InsertSafeDescription('B', "Blocks messages with too many capital letters, as determined by the network configuration (requires blockcaps module)");
                    network.InsertSafeDescription('D', "Delays join messages from users until they message the channel (requires delayjoin module)");
                    network.InsertSafeDescription('F', "Blocks nick changes when they equal or exceed the specified rate (requires nickflood module)");
                    network.InsertSafeDescription('J', "Prevents rejoin after kick for the specified number of seconds. This prevents auto-rejoin (requires kicknorejoin module)");
                    network.InsertSafeDescription('P', "Makes the channel permanent; Bans, invites, the topic, modes, and such will not be lost when it empties (can only be set by IRCops, requires permchannels module)");
                    network.InsertSafeDescription('Q', "Only ulined servers and their users can kick (requires nokicks module)");
                    network.InsertSafeDescription('R', "Blocks unregistered users from joining (requires services account module)");
                    network.InsertSafeDescription('T', "Blocks /NOTICEs to the channel from users who are not at least halfop (requires nonotice module)");
                    return;
                }

                if (value.Contains("running version "))
                {
                    string version = value.Substring(value.IndexOf("version ") + 8);
                    network.IrcdVersion = version;
                }
            }
            base.Hook_NetworkInfo(network, command, parameters, value);
        }
    }
}
