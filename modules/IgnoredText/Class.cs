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
    class IgnoredText : Extension
    {
        Graphics.Window collector = null;
        Gtk.MenuItem menu;

        public override void Initialise()
        {
            Name = "Ignored text";
            Version = "1.0.0";
            Description = "This allows you to display all ignored text in a separate window";
            base.Initialise();
        }

        public override void Hook_Initialise(Forms.Main main)
        {
            menu = new Gtk.MenuItem("Display ignored text");
            collector = new Graphics.Window();
            collector.CreateChat(false);
            menu.Activated += new EventHandler(Display);
            collector.WindowName = "Ignored";
            main.ToolsMenu.Append(menu);
            menu.Show();
        }

        public override bool Hook_BeforeIgnore(Extension.MessageArgs _IgnoreArgs)
        {
            if (_IgnoreArgs.user == null)
            {
                collector.scrollback.InsertText("{" + _IgnoreArgs.window.WindowName + "} " + _IgnoreArgs.text, ContentLine.MessageStyle.Message, false, _IgnoreArgs.date, true);
                return true;
            }
            collector.scrollback.InsertText("{" + _IgnoreArgs.window.WindowName + "} " + Protocol.PRIVMSG(_IgnoreArgs.user.Nick, _IgnoreArgs.text), ContentLine.MessageStyle.Message, false, _IgnoreArgs.date, true);
            return true;
        }

        private void Display(object s, EventArgs e)
        {
            try
            {
                if (Core.network != null)
                {
                    Core.SystemForm.SwitchWindow(collector);
                    return;
                }
                collector.Visible = true;
                Core.SystemForm.SwitchWindow(collector);
            }
            catch (Exception fail)
            {
                HandleException(fail);
            }
        }
    }
}
