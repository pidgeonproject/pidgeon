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
using System.Linq;
using Gtk;
using System.Text;

namespace Client.GTK
{
    public class MessageBox
    {
        public ResponseType result;
        public MessageDialog Message;

        public static MessageBox Show(Window parentWindow, MessageType messageType, ButtonsType buttons, string message, string title)
        {
            MessageBox mb = new MessageBox(parentWindow, messageType, buttons, message, title);
            return mb;
        }

        public MessageBox(Window parentWindow, MessageType messageType, ButtonsType buttons, string message, string title)
        {
            Message = new MessageDialog(parentWindow, DialogFlags.Modal, messageType, buttons, message);
            Message.WindowPosition = WindowPosition.Center;
            Message.Title = title;
            result = (ResponseType)Message.Run();
            Message.Destroy();
        }
    }
}
