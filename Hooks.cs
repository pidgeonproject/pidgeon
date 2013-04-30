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

            /// <summary>
            /// Before note
            /// </summary>
            /// <param name="caption">Title</param>
            /// <param name="text">Text</param>
            /// <returns></returns>
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

            /// <summary>
            /// This function is called during the main form is created, in case you want to register some menu's, this is a good time
            /// </summary>
            /// <param name="main"></param>
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

            /// <summary>
            /// Preferences
            /// </summary>
            /// <param name="preferences"></param>
            public static void BeforeOptions(ref Forms.Preferences preferences)
            {

            }

            /// <summary>
            /// This is triggered everytime that there is any activity on network
            /// </summary>
            /// <param name="network"></param>
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
            /// <param name="protocol"></param>
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
            /// <summary>
            /// When you hover over a link
            /// </summary>
            /// <param name="URL"></param>
            /// <param name="X"></param>
            /// <param name="Y"></param>
            /// <param name="window"></param>
            public static void LinkHover(string URL, int X, int Y, Graphics.Window window)
            {
                
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
            /// Hook that is triggered when you connect to network
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

            public static void NetworkInfo(Network network, string command, string parameters, string value)
            {
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_NetworkInfo(network, command, parameters, value);
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

            /// <summary>
            /// User part
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="channel"></param>
            /// <param name="message"></param>
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

            /// <summary>
            /// User kick
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="kicker"></param>
            /// <param name="channel"></param>
            /// <param name="message"></param>
            public static void UserKick(Network network, User user, User kicker, Channel channel, string message)
            {

            }

            /// <summary>
            /// User talk
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="channel"></param>
            /// <param name="message"></param>
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

            /// <summary>
            /// User quit
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="message"></param>
            public static void UserQuit(Network network, User user, string message)
            {

            }

            /// <summary>
            /// User join
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="channel"></param>
            public static void UserJoin(Network network, User user, Channel channel)
            {

            }

            public static void ChannelInfo(Network network, Channel channel, string Mode)
            {

            }

            /// <summary>
            /// Channel topic is changed
            /// </summary>
            /// <param name="topic">New topic</param>
            /// <param name="user">User who changed it</param>
            /// <param name="network">Network</param>
            /// <param name="channel">Channel</param>
            public static void ChannelTopic(string topic, User user, Network network, Channel channel)
            {

            }

            /// <summary>
            /// Network is created
            /// </summary>
            /// <param name="network"></param>
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
