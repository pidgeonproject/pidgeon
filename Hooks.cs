//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    /// <summary>
    /// These functions are called when the event in question happen
    /// </summary>
    class Hooks
    {
        /// <summary>
        /// System hooks
        /// </summary>
        public class _Sys
        {
            /// <summary>
            /// Event to happen when core finish
            /// </summary>
            public static void AfterCore()
            {
                Events._System.TriggerAfterCore(null, null);
            }

            /// <summary>
            /// Before a notification is displayed on screen
            /// </summary>
            /// <param name="caption">Title</param>
            /// <param name="text">Text</param>
            /// <returns></returns>
            public static bool BeforeNote(ref string caption, ref string text)
            {
                Extension.BeforeNoteArgs e = new Extension.BeforeNoteArgs();
                e.Caption = caption;
                e.Text = text;
                Events._System.TriggerBeforeNote(null, e);
                caption = e.Caption;
                text = e.Text;
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
                Extension.SystemInitialiseArgs e = new Extension.SystemInitialiseArgs();
                e.Main = main;
                Events._System.TriggerInitialise(null, e);
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
            /// Preferences form is open
            /// </summary>
            /// <param name="preferences">Preferences object</param>
            public static void BeforeOptions(Forms.Preferences preferences)
            {
                Extension.BeforeOptionsArgs e = new Extension.BeforeOptionsArgs();
                e.Window = preferences;
                Events._System.TriggerBeforeOptions(null, e);
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            extension.Hook_BeforeOptions(preferences);
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook BeforeOptions(Forms.Preferences preferences) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
            }

            /// <summary>
            /// This is triggered everytime that there is any activity on network
            /// </summary>
            /// <param name="network"></param>
            public static void Poke(Network network = null)
            {
                Core.ResetMainActivityTimer();
            }

            /// <summary>
            /// This is called before a connection window is finished in building
            /// </summary>
            /// <param name="window"></param>
            public static void Connection(Forms.Connection window)
            {
                return;
            }
        }

        /// <summary>
        /// Window
        /// </summary>
        public class _Window
        {
            /// <summary>
            /// This is called before the user menu is shown
            /// </summary>
            /// <param name="Menu"></param>
            /// <param name="Window"></param>
            /// <returns></returns>
            public static bool BeforeUserMenu(Gtk.Menu Menu, Graphics.Window Window)
            {
                Extension.BeforeUserMenuArgs e = new Extension.BeforeUserMenuArgs();
                e.menu = Menu;
                e.window = Window;
                Events._Window.TriggerBeforeUserMenu(null, e);
                bool ok = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            if (!extension.Hook_BeforeUserMenu(e))
                            {
                                ok = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook BeforeUserMenu(Gtk.Menu Menu, Graphics.Window Window) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return ok;
            }

            /// <summary>
            /// This is called before the user menu is shown
            /// </summary>
            /// <param name="Menu"></param>
            /// <param name="Window"></param>
            /// <returns></returns>
            public static bool AfterUserMenu(Gtk.Menu Menu, Graphics.Window Window)
            {
                Extension.BeforeUserMenuArgs e = new Extension.BeforeUserMenuArgs();
                e.menu = Menu;
                e.window = Window;
                Events._Window.TriggerBeforeUserMenu(null, e);
                bool ok = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            if (!extension.Hook_AfterUserMenu(e))
                            {
                                ok = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook AfterUserMenu(Gtk.Menu Menu, Graphics.Window Window) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return ok;
            }
        }

        /// <summary>
        /// Protocol hooks
        /// </summary>
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
                return;
            }
        }

        /// <summary>
        /// Scrollback hooks
        /// </summary>
        public class _Scrollback
        {
            /// <summary>
            /// When you hover over a link //fixme
            /// </summary>
            /// <param name="URL"></param>
            /// <param name="X"></param>
            /// <param name="Y"></param>
            /// <param name="window"></param>
            public static void LinkHover(string URL, int X, int Y, Graphics.Window window)
            {
                
            }

            /// <summary>
            /// When ignore happen
            /// </summary>
            /// <param name="window">Window</param>
            /// <param name="message">Message that was ignored</param>
            /// <param name="updated"></param>
            /// <param name="date"></param>
            /// <param name="user"></param>
            /// <returns></returns>
            public static bool Ignore(Graphics.Window window, User user, string message, bool updated, long date)
            {
                bool ok = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            Extension.MessageArgs data = new Extension.MessageArgs();
                            data.updated = updated;
                            data.date = date;
                            data.user = user;
                            data.text = message;
                            data.window = window;
                            if (!extension.Hook_BeforeIgnore(data))
                            {
                                ok = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook Ignore(Graphics.Window window, string message, bool updated, long date) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return ok;
            }

            /// <summary>
            /// This hook is triggered on tab press
            /// </summary>
            /// <param name="restore"></param>
            /// <param name="Text"></param>
            /// <param name="PrevText"></param>
            /// <param name="CarretPosition"></param>
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

            /// <summary>
            /// Called on notification display
            /// </summary>
            /// <param name="text">Text of notification form</param>
            /// <param name="InputStyle">Style of line that is shown</param>
            /// <param name="WriteLog">Whether it should be written to logs or not</param>
            /// <param name="Date">Date of line</param>
            /// <param name="SuppressPing">If this ping should be suppressed or not</param>
            /// <returns>If true is returned this notification is ignored and not displayed</returns>
            public static bool NotificationDisplay(string text, Client.ContentLine.MessageStyle InputStyle, ref bool WriteLog, long Date, ref bool SuppressPing)
            {
                bool success = true;

                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            if (!extension.Hook_NotificationDisplay(text, InputStyle, ref WriteLog, Date, ref SuppressPing))
                            {
                                success = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook NotificationDisplay() module " + extension.Name);
                        Core.handleException(mf);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Network
        /// </summary>
        public class _Network
        {
            /// <summary>
            /// Hook that is triggered when you connect to network
            /// </summary>
            /// <param name="network">Network</param>
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
            /// Network info, this is called when network send the info
            /// </summary>
            /// <param name="network">Network</param>
            /// <param name="command">Command</param>
            /// <param name="parameters">Parameters</param>
            /// <param name="value">Values</param>
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
            /// Events to happen before user is kicked from channel by you, in case you return false, the kick is not performed
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
            /// Events to happen before you join a channel, if return false, the join is cancelled
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
            /// Even that happens when a user part a existing channel you are in
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="channel"></param>
            /// <param name="message"></param>
            /// <param name="updated"></param>
            /// <param name="date"></param>
            /// <returns></returns>
            public static bool UserPart(Network network, User user, Channel channel, string message, bool updated, long date)
            {
                bool ok = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            if (!extension.Hook_UserPart(network, user, channel, message, updated))
                            {
                                Core.DebugLog("Compatibility warning: extension with name " + extension.Name + " is using obsolete function extension.Hook_UserPart(network, user, channel, message, updated)");
                                ok = false;
                            }
                            Extension.NetworkPartArgs data = new Extension.NetworkPartArgs();
                            data.network = network;
                            data.user = user;
                            data.channel = channel;
                            data.updated = updated;
                            data.message = message;
                            data.date = date;
                            if (!extension.Hook_UserPart(data))
                            {
                                ok = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook UserPart(Network network, User user, Channel channel, string message) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return ok;
            }

            /// <summary>
            /// If this return true it means that underlying extension or code has processed this CTCP and it can be ignored by irc parser
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="CTCP"></param>
            /// <returns></returns>
            public static bool ParseCTCP(Network network, User user, string CTCP)
            {
                return false;
            }

            /// <summary>
            /// User kick
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="kicker"></param>
            /// <param name="channel"></param>
            /// <param name="message"></param>
            public static bool UserKick(Network network, User user, User kicker, Channel channel, string message)
            {
                return true;
            }

            /// <summary>
            /// Topic is being changed, topic is a new topic, this event happen before the topic is changed and if false is returned
            /// the topic change is ignored
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="channel"></param>
            /// <param name="topic"></param>
            /// <param name="date"></param>
            /// <param name="updated"></param>
            /// <returns></returns>
            public static bool Topic(Network network, string user, Channel channel, string topic, long date, bool updated)
            {
                bool success = true;

                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            Extension.TopicArgs data = new Extension.TopicArgs();
                            data.channel = channel;
                            data.network = network;
                            data.Source = user;
                            data.date = date;
                            data.updated = updated;
                            data.Topic = topic;
                            if (!extension.Hook_Topic(data))
                            {
                                success = false;
                            }
                            if (!extension.Hook_Topic(network, user, channel, topic))
                            {
                                Core.DebugLog("Compatibility warning: extension with name " + extension.Name + " is using obsolete function extension.Hook_Topic(network, user, channel, topic)");
                                success = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook Topic(Network network, User user, Channel channel, string topic) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }

                return success;
            }

            /// <summary>
            /// User talk
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="channel"></param>
            /// <param name="message"></param>
            /// <param name="updated"></param>
            /// <param name="date"></param>
            /// <returns></returns>
            public static bool UserTalk(Network network, User user, Channel channel, string message, bool updated, long date)
            {
                bool ok = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            if (!extension.Hook_UserTalk(network, user, channel, message, updated))
                            {
                                Core.DebugLog("Compatibility warning: extension with name " + extension.Name + " is using obsolete" +
                                    " function extension.Hook_UserTalk(network, user, channel, message, updated)");
                                ok = false;
                            }
                            Extension.NetworkTextArgs data = new Extension.NetworkTextArgs();
                            data.network = network;
                            data.updated = updated;
                            data.date = date;
                            data.message = message;
                            data.user = user;
                            if (!extension.Hook_UserTalk(data))
                            {
                                ok = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook UserTalk(Network network, User user, Channel channel, string message) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return ok;
            }

            /// <summary>
            /// User quit
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="message"></param>
            /// <param name="window"></param>
            /// <param name="updated"></param>
            /// <param name="date"></param>
            /// <returns></returns>
            public static bool UserQuit(Network network, User user, string message, Graphics.Window window, bool updated, long date)
            {
                bool ok = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            if (!extension.Hook_UserQuit(network, user, message, window, updated))
                            {
                                Core.DebugLog("Compatibility warning: extension with name " + extension.Name + " is using obsolete function " +
                                    "extension.Hook_UserQuit(network, user, message, window, updated)");
                                ok = false;
                            }
                            Extension.NetworkUserQuitArgs data = new Extension.NetworkUserQuitArgs();
                            data.network = network;
                            data.user = user;
                            data.message = message;
                            data.date = date;
                            data.window = window;
                            data.updated = updated;
                            if (!extension.Hook_UserQuit(data))
                            {
                                ok = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook UserQuit(Network network, User user, string message) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return ok;
            }

            /// <summary>
            /// User join
            /// </summary>
            /// <param name="network"></param>
            /// <param name="user"></param>
            /// <param name="channel"></param>
            /// <param name="updated"></param>
            /// <param name="date"></param>
            /// <returns></returns>
            public static bool UserJoin(Network network, User user, Channel channel, bool updated, long date)
            {
                bool ok = true;
                foreach (Extension extension in Core.Extensions)
                {
                    try
                    {
                        if (extension._Status == Extension.Status.Active)
                        {
                            Extension.NetworkJoinArgs data = new Extension.NetworkJoinArgs();
                            data.channel = channel;
                            data.updated = updated;
                            data.network = network;
                            data.date = date;
                            data.user = user;
                            if (!extension.Hook_UserJoin(data))
                            {
                                ok = false;
                            }
                            if (!extension.Hook_UserJoin(network, user, channel, updated))
                            {
                                Core.DebugLog("Compatibility warning: extension with name " + extension.Name +
                                    " is using obsolete function extension.Hook_UserJoin(network, user, channel, updated)");
                                ok = false;
                            }
                        }
                    }
                    catch (Exception mf)
                    {
                        Core.DebugLog("Error in hook UserJoin(Network network, User user, Channel channel) module " + extension.Name);
                        Core.handleException(mf);
                    }
                }
                return ok;
            }

            /// <summary>
            /// Channel mode is printed to a window
            /// </summary>
            /// <param name="network"></param>
            /// <param name="channel"></param>
            /// <param name="Mode"></param>
            /// <returns></returns>
            public static bool ChannelInfo(Network network, Channel channel, string Mode)
            {
                bool ok = true;
                return ok;
            }

            /// <summary>
            /// Channel topic is retrieved from server
            /// </summary>
            /// <param name="topic">Topic</param>
            /// <param name="user">User who set it</param>
            /// <param name="network">Network</param>
            /// <param name="channel">Channel</param>
            public static bool ChannelTopic(string topic, User user, Network network, Channel channel)
            {
                bool ok = true;
                return ok;
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

            /// <summary>
            /// Events to happen before change of mode, if return false the change of mode is ignored
            /// </summary>
            /// <param name="network"></param>
            public static void BeforeMode(Network network)
            {
                return;
            }
        }
    }
}
