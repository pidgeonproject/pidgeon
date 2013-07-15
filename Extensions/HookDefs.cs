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

// This file contains a definition of all hooks for an extension

using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Client
{
    /// <summary>
    /// Extension
    /// </summary>
    public partial class Extension
    {

        /////////////////////////////////////////////////////////////////////////////////////
        // Extension related
        /////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This hook is called on load of extension
        /// </summary>
        /// <returns></returns>
        public virtual bool Hook_OnLoad()
        {
            return true;
        }

        /// <summary>
        /// This hook is started when extension is unloaded
        /// </summary>
        /// <returns></returns>
        public virtual bool Hook_Unload()
        {
            return true;
        }

        /// <summary>
        /// This hook is called before the extension is loaded
        /// </summary>
        /// <returns></returns>
        public virtual bool Hook_OnRegister()
        {
            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // Channel
        /////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This hook is started when user part from a channel
        /// </summary>
        /// <param name="_PartArgs">Arguments</param>
        /// <returns>True if OK, False if the part should be ignored</returns>
        public virtual bool Hook_UserPart(NetworkPartArgs _PartArgs)
        {
            return true;
        }

        /// <summary>
        /// Use Hook_UserPart(NetworkPartArgs _PartArgs) instead 
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        [Obsolete]
        public virtual bool Hook_UserPart(Network network, User user, Channel channel, string message, bool updated)
        {
            return true;
        }

        /// <summary>
        /// This hook is started before pidgeon try to join a channel, return false will abort the action
        /// </summary>
        /// <param name="network"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public virtual bool Hook_BeforeJoin(Network network, string Channel)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when user quit
        /// </summary>
        /// <param name="_QuitArgs"></param>
        /// <returns></returns>
        public virtual bool Hook_UserQuit(NetworkUserQuitArgs _QuitArgs)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when user talk in a channel
        /// </summary>
        /// <param name="_TalkArgs"></param>
        /// <returns></returns>
        public virtual bool Hook_UserTalk(NetworkTextArgs _TalkArgs)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when user talk in a channel
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        [Obsolete]
        public virtual bool Hook_UserTalk(Network network, User user, Channel channel, string message, bool updated)
        {
            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // Network
        /////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This hook is started when a network object is created
        /// </summary>
        /// <param name="network"></param>
        public virtual void Hook_Network(Network network)
        {
            return;
        }

        /// <summary>
        /// This hook is started after connection to a network
        /// </summary>
        /// <param name="network"></param>
        public virtual void Hook_AfterConnect(Network network)
        {

        }

        /// <summary>
        /// This function is called when network send us a network info
        /// </summary>
        /// <param name="network"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        public virtual void Hook_NetworkInfo(Network network, string command, string parameters, string value)
        {
            return;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // Window
        /////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This hook is called right before the menu is displayed
        /// </summary>
        /// <param name="_MenuArgs"></param>
        /// <returns></returns>
        public virtual bool Hook_AfterUserMenu(BeforeUserMenuArgs _MenuArgs)
        {
            return true;
        }

        /// <summary>
        /// This hook is called before the menu is created
        /// </summary>
        /// <param name="_MenuArgs"></param>
        /// <returns></returns>
        public virtual bool Hook_BeforeUserMenu(BeforeUserMenuArgs _MenuArgs)
        {
            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // Scrollback
        /////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This hook is started before the item is ignored
        /// </summary>
        /// <param name="_IgnoreArgs"></param>
        /// <returns></returns>
        public virtual bool Hook_BeforeIgnore(MessageArgs _IgnoreArgs)
        {
            return true;
        }

        /// <summary>
        /// DEPRECATED Use Hook_UserQuit(NetworkUserQuitArgs _QuitArgs) instead of this
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="window"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        [Obsolete]
        public virtual bool Hook_UserQuit(Network network, User user, string message, Graphics.Window window, bool updated)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when user join to a channel you are in
        /// </summary>
        /// <param name="_JoinArgs"></param>
        /// <returns></returns>
        public virtual bool Hook_UserJoin(NetworkJoinArgs _JoinArgs)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when user join to a channel you are in
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        [Obsolete]
        public virtual bool Hook_UserJoin(Network network, User user, Channel channel, bool updated)
        {
            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // System
        /////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This hook is called before the option form is displayed
        /// </summary>
        /// <param name="window"></param>
        public virtual void Hook_BeforeOptions(Forms.Preferences window)
        {}

        /// <summary>
        /// This hook is started before you connect to a protocol
        /// </summary>
        /// <param name="protocol"></param>
        public virtual void Hook_BeforeConnect(Protocol protocol)
        {
            return;
        }

        /// <summary>
        /// This hook is started when main form is loaded
        /// </summary>
        /// <param name="main"></param>
        public virtual void Hook_Initialise(Client.Forms.Main main)
        {
            return;
        }

        /// <summary>
        /// This hook is started when tab key is pressed in a text box
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="text"></param>
        /// <param name="caret"></param>
        /// <param name="restore"></param>
        public virtual void Hook_InputOnTab(ref string prev, ref string text, ref int caret, ref bool restore)
        {
            return;
        }

        /// <summary>
        /// This hook is called before the Note is displayed
        /// </summary>
        /// <param name="name">Caption</param>
        /// <param name="text">Data</param>
        /// <returns></returns>
        public virtual bool Hook_BeforeNote(ref string name, ref string text)
        {
            return true;
        }

        /// <summary>
        /// Called on notification display
        /// </summary>
        /// <param name="text"></param>
        /// <param name="InputStyle"></param>
        /// <param name="WriteLog"></param>
        /// <param name="Date"></param>
        /// <param name="SuppressPing"></param>
        /// <returns>if false, the notification is not displayed</returns>
        public virtual bool Hook_NotificationDisplay(string text, Client.ContentLine.MessageStyle InputStyle, ref bool WriteLog, long Date, ref bool SuppressPing)
        {
            return true;
        }

        /// <summary>
        /// Topic is being changed, this event happen before the topic is changed and if false is returned
        /// the topic change is ignored - this function is obsolete, use Hook_Topic(TopicArgs _TopicArgs)
        /// </summary>
        /// <param name="network">Network</param>
        /// <param name="userline">Line of text</param>
        /// <param name="channel">Channel</param>
        /// <param name="topic">Topic</param>
        /// <returns></returns>
        [Obsolete]
        public virtual bool Hook_Topic(Network network, string userline, Channel channel, string topic)
        {
            return true;
        }

        /// <summary>
        /// Topic is being changed, this event happen before the topic is changed and if false is returned
        /// the topic change is ignored
        /// </summary>
        /// <param name="_TopicArgs"></param>
        /// <returns></returns>
        public virtual bool Hook_Topic(TopicArgs _TopicArgs)
        {
            return true;
        }
    }
}
