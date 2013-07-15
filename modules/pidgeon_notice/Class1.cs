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
using Client;

namespace Client
{
    class PidgeonNotice : Client.Extension
    {
        Graphics.Window collector = null;
        Gtk.MenuItem menu;

        public override void  Hook_Initialise(Forms.Main main)
        {
            menu = new Gtk.MenuItem("Display notifications");
            collector = new Graphics.Window();
            collector.CreateChat(null, false, false, false);
            menu.Activated += new EventHandler(Display);
            collector.WindowName = "Notifications";
            main.ToolsMenu.Append(menu);
            menu.Show();
        }

        public override bool Hook_NotificationDisplay(string text, ContentLine.MessageStyle InputStyle, ref bool WriteLog, long Date, ref bool SuppressPing)
        {
            collector.scrollback.InsertText(text, InputStyle, false, Date, true);
            return true;
        }

        private void Display(object s, EventArgs e)
        {
            try
            {
                if (Core.SelectedNetwork != null)
                {
                    Core.SelectedNetwork.RenderedChannel = null;
                    Core.SelectedNetwork._Protocol.Current = collector;
                    Core.SystemForm.SwitchWindow(collector);
                    return;
                }
                collector.Visible = true;
                Core.SystemForm.SwitchWindow(collector);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public override void Initialise()
        {
            Name = "Notification collector";
            Version = "1.0.0";
            Description = "This allows you to collect all notification you get";
            base.Initialise();
        }
    }
}
