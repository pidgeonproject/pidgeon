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

// This file contains a definition of all arguments for hooks

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
        /// <summary>
        /// This is a base for args
        /// </summary>
        public class HookArgs
        {
            /// <summary>
            /// Version
            /// </summary>
            public string ClientVersion = Configuration.Version.ToString();
            /// <summary>
            /// This is optional return value
            /// </summary>
            public bool ReturnValue = false;
        }

        /// <summary>
        /// For a message sent to window
        /// </summary>
        public class MessageArgs : HookArgs
        {
            /// <summary>
            /// Window where the message is sent to
            /// </summary>
            public Graphics.Window window = null;
            /// <summary>
            /// Message content
            /// </summary>
            public string text = null;
            /// <summary>
            /// Updated
            /// </summary>
            public bool updated = false;
            /// <summary>
            /// Date
            /// </summary>
            public long date = 0;
            /// <summary>
            /// If there is a user attached to message it is this one
            /// </summary>
            public User user = null;
        }

        /// <summary>
        /// For hook related to before note
        /// </summary>
        public class BeforeNoteArgs : HookArgs
        {
            /// <summary>
            /// Text
            /// </summary>
            public string Text = null;
            /// <summary>
            /// Caption
            /// </summary>
            public string Caption = null;
        }

        /// <summary>
        /// Arguments that are used for before options
        /// </summary>
        public class BeforeOptionsArgs : HookArgs
        {
            /// <summary>
            /// Preferences window
            /// </summary>
            public Forms.Preferences Window = null;
        }

        /// <summary>
        /// System form init args
        /// </summary>
        public class SystemInitialiseArgs : HookArgs
        {
            /// <summary>
            /// Main
            /// </summary>
            public Forms.Main Main = null;
        }

        /// <summary>
        /// This is a base for network args
        /// </summary>
        public class NetworkArgs : HookArgs
        {
            /// <summary>
            /// Network
            /// </summary>
            public Network network = null;
            /// <summary>
            /// This is only needed when bouncer is being used. If this contains 0 you can ignore it, otherwise you should convert it to
            /// binary time and consider the event to happen in that time.
            /// </summary>
            public long date = 0;
            /// <summary>
            /// This information is up to date and not retrieved from logs
            /// </summary>
            public bool updated = false;
            
        }

        /// <summary>
        /// Topic event args
        /// </summary>
        public class TopicArgs : NetworkArgs
        {
            /// <summary>
            /// Channel
            /// </summary>
            public Channel channel = null;
            /// <summary>
            /// This line represent a user in format user!ident@host
            /// </summary>
            public string Source = null;
            /// <summary>
            /// Changed topic
            /// </summary>
            public string Topic = null;
        }

        /// <summary>
        /// Network info args
        /// </summary>
        public class NetworkInfo : NetworkArgs
        {
            /// <summary>
            /// Command
            /// </summary>
            public string command = null;
            /// <summary>
            /// Parameters
            /// </summary>
            public string parameters = null;
            /// <summary>
            /// Value
            /// </summary>
            public string value = null;
        }

        /// <summary>
        /// User part
        /// </summary>
        public class NetworkPartArgs : NetworkArgs
        {
            /// <summary>
            /// User
            /// </summary>
            public User user = null;
            /// <summary>
            /// Channel
            /// </summary>
            public Channel channel = null;
            /// <summary>
            /// Message
            /// </summary>
            public string message = null;
        }

        /// <summary>
        /// User join
        /// </summary>
        public class NetworkJoinArgs : NetworkArgs
        {
            /// <summary>
            /// User
            /// </summary>
            public User user = null;
            /// <summary>
            /// Channel
            /// </summary>
            public Channel channel = null;
        }

        /// <summary>
        /// User talk
        /// </summary>
        public class NetworkTextArgs : NetworkArgs
        {
            /// <summary>
            /// User
            /// </summary>
            public User user = null;
            /// <summary>
            /// Channel
            /// </summary>
            public Channel channel = null;
            /// <summary>
            /// Message
            /// </summary>
            public string message = null;
        }

        /// <summary>
        /// User
        /// </summary>
        public class NetworkUserQuitArgs : NetworkArgs
        {
            /// <summary>
            /// User
            /// </summary>
            public User user = null;
            /// <summary>
            /// Message
            /// </summary>
            public string message = null;
            /// <summary>
            /// Window
            /// </summary>
            public Graphics.Window window = null;
        }

        /// <summary>
        /// These arguments are used for mouse
        /// </summary>
        public class MouseHoverArgs : HookArgs
        {
            /// <summary>
            /// Window
            /// </summary>
            public Graphics.Window Window = null;
            /// <summary>
            /// X
            /// </summary>
            public int X = 0;
            /// <summary>
            /// Y
            /// </summary>
            public int Y = 0;
        }

        /// <summary>
        /// Before user menu is shown
        /// </summary>
        public class BeforeUserMenuArgs : HookArgs
        {
            /// <summary>
            /// Menu which is about to be displayed
            /// </summary>
            public Gtk.Menu menu = null;
            /// <summary>
            /// Window the menu belongs to
            /// </summary>
            public Graphics.Window window = null;
        }
    }
}
