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

namespace Client
{
    class Hooks
    {
        public class _Sys
        {
            /// <summary>
            /// Event to happen when core finish
            /// </summary>
            public static void AfterCore()
            {

            }

            public static bool BeforeNote(ref string caption, ref string text)
            {
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_BeforeNote(ref caption, ref text);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook BeforeNote(string, string) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return true;
            }

            public static void Initialise(Client.Forms.Main main)
            {
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_Initialise(main);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook Initialise(Main) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
            }

            public static void BeforeOptions(ref Forms.Preferences preferences)
            {

            }

            public static void Poke(Network network = null)
            {
                Core.ResetMainActivityTimer();
            }
        }

        public class _Protocol
        {
            /// <summary>
            /// This hook allow you to call functions before you open connection
            /// </summary>
            /// <param name="server"></param>
            public static void BeforeConnect(Protocol protocol)
            {
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_BeforeConnect(protocol);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook BeforeConnect(string, string) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return;
            }

            /// <summary>
            /// Events to happen before command, can't be stopped
            /// </summary>
            /// <param name="protocol"></param>
            /// <param name="command"></param>
            public static void BeforeCommand(Protocol protocol, string command)
            {

            }
        }

        public class _Scrollback
        {
            public static string BeforeParser(string Text)
            {
                return Text;
            }

            public static string BeforeInsertLine(string Text, string Line)
            {
                return Text;
            }

            public static string AfterParser(string Text)
            {
                return Text;
            }

            public static string BeforeLinePartLoad(string Text)
            {
                return Text;
            }

            public static void TextTab(ref bool restore, ref string Text, ref string PrevText, ref int CarretPosition)
            {
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_InputOnTab(ref PrevText, ref Text, ref CarretPosition, ref restore);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook TextTab(bool, string, string, int) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
            }
        }

        public class _Network
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="network"></param>
            public static void AfterConnectToNetwork(Network network)
            {
                network.isLoaded = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_AfterConnect(network);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook AfterConnectToNetwork(Network network) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return;
            }

            /// <summary>
            /// Events to happen before user is kicked from channel, in case you return false, the kick is ignored
            /// </summary>
            /// <param name="network">Network</param>
            /// <param name="Source">Performer</param>
            /// <param name="Target">User who get kick msg</param>
            /// <param name="Reason">Reason</param>
            /// <param name="Ch">Channel</param>
            public static bool BeforeKick(Network network, string Source, string Target, string Reason, string Ch)
            {
                return true;
            }

            /// <summary>
            /// Events to happen before joining, if return false, the join is cancelled
            /// </summary>
            /// <param name="network"></param>
            /// <param name="Channel"></param>
            /// <returns></returns>
            public static bool BeforeJoin(Network network, string Channel)
            {
                bool data = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            if (!extension.Hook_BeforeJoin(network, Channel))
                            {
                                data = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook BeforeJoin(Network network,string Channel) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return data;
            }

            public static void UserPart(Network network, User user, Channel channel, string message)
            {
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_UserPart(network, user, channel, message);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook UserPart(Network network, User user, Channel channel, string message) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return;
            }

            public static void UserKick(Network network, User user, User kicker, Channel channel, string message)
            {

            }

            public static void UserTalk(Network network, User user, Channel channel, string message)
            {
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_UserTalk(network, user, channel, message);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook UserTalk(Network network, User user, Channel channel, string message) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return;
            }

            public static void UserQuit(Network network, User user, string message)
            {

            }

            public static void UserJoin(Network network, User user, Channel channel)
            {

            }

            public static void ChannelInfo(Network network, Channel channel, string Mode)
            {

            }

            public static void ChannelTopic(string topic, User user, Network network, Channel channel)
            {

            }

            public static void CreatingNetwork(Network network)
            {
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_Network(network);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook CreatingNetwork(network) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return;
            }

            /// <summary>
            /// Events to happen before leaving a channel, if return false, the quiting is cancelled
            /// </summary>
            /// <param name="network"></param>
            /// <param name="channel"></param>
            /// <returns></returns>
            public static bool BeforePart(Network network, Channel channel)
            {
                return true;
            }

            /// <summary>
            /// Events to happen before quiting, if return false, the exiting is cancelled
            /// </summary>
            /// <param name="network"></param>
            /// <returns></returns>
            public static bool BeforeExit(Network network)
            {
                return true;
            }

            public static void BeforeMode(Network network)
            {

            }
        }
    }
}
