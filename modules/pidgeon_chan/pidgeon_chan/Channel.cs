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

namespace Client
{
    public class PidgeonChan : Extension
    {
        public Gtk.MenuItem item = null;
        public Gtk.SeparatorMenuItem separator = null;
        private Forms.Main _m = null;


        public override void Initialise()
        {
            Name = "Help";
            Description = "This plugin enable you to open #pidgeon channel from help menu";
            Version = "1.2.0";
            base.Initialise();
        }

        public override bool Hook_Unload()
        {
            try
            {
                if (_m == null)
                {
                    Core.DebugLog("No handle to main");
                    return false;
                }

                _m.HelpMenu.Remove(separator);
                _m.HelpMenu.Remove(item);
                return true;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
                return false;
            }
        }

        public override void Hook_Initialise(Forms.Main main)
        {
            _m = main;
            item = new Gtk.MenuItem("#pidgeon");
            item.Activated += new EventHandler(pidgeonToolStripMenuItem_Click);
            separator = new Gtk.SeparatorMenuItem();
            main.HelpMenu.Append(separator);
            main.HelpMenu.Append(item);
            separator.Show();
            item.Show();
            Core.DebugLog("Registered #pidgeon in menu");
        }

        private void pidgeonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Protocol network in Core.Connections)
            {
                if (network.GetType() == typeof(ProtocolSv))
                {
                    ProtocolSv sv = (ProtocolSv)network;
                    foreach (Network server in sv.NetworkList)
                    {
                        if (server.ServerName == "irc.tm-irc.org")
                        {
                            server.Join("#pidgeon");
                            return;
                        }
                    }
                }

                if (network.Server == "irc.tm-irc.org")
                {
                    network.Join("#pidgeon");
                    return;
                }
            }
            Core.ConnectIRC("irc.tm-irc.org");
            foreach (Protocol network in Core.Connections)
            {
                if (network.Server == "irc.tm-irc.org")
                {
                    network.Join("#pidgeon");
                    return;
                }
            }
        }
    }
}
