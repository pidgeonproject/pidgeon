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
using Gtk;
using System.Text;

namespace Client.GTK
{
    /// <summary>
    /// MessageBox
    /// </summary>
    public class MessageBox
    {
        /// <summary>
        /// Result of this box
        /// </summary>
        public ResponseType result;
        /// <summary>
        /// Message box
        /// </summary>
        public MessageDialog Message;

        /// <summary>
        /// Display a new message box
        /// </summary>
        /// <param name="parentWindow">Window this box belongs to</param>
        /// <param name="messageType">Type of box</param>
        /// <param name="buttons">Buttons</param>
        /// <param name="message">Message</param>
        /// <param name="title">Title</param>
        /// <returns></returns>
        public static MessageBox Show(Window parentWindow, MessageType messageType, ButtonsType buttons, string message, string title)
        {
            MessageBox mb = new MessageBox(parentWindow, messageType, buttons, message, title);
            return mb;
        }

        /// <summary>
        /// Creates a new message box
        /// </summary>
        /// <param name="parentWindow">Window this box belongs to</param>
        /// <param name="messageType">Type of box</param>
        /// <param name="buttons">Buttons</param>
        /// <param name="message">Value</param>
        /// <param name="title">Title</param>
        public MessageBox(Window parentWindow, MessageType messageType, ButtonsType buttons, string message, string title)
        {
            Message = new MessageDialog(parentWindow, DialogFlags.Modal, messageType, buttons, false, null);
            Message.WindowPosition = WindowPosition.Center;
            Message.Text = message;
            Message.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            Message.Title = title;
            result = (ResponseType)Message.Run();
            Message.Destroy();
        }
    }
}
