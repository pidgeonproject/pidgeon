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

namespace Pidgeon
{
    public class PidgeonChan : Extension
    {
        public Gtk.MenuItem item = null;
        public Gtk.SeparatorMenuItem separator = null;
        private Forms.Main _m = null;


        public override void Initialise()
        {
            Description = "This plugin enable you to open #pidgeon channel from help menu";
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
                HandleException(fail);
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

        private bool join()
        {
            foreach (libirc.IProtocol network in Connections.ConnectionList)
            {
                if (network.GetType() == typeof(Protocols.Services.ProtocolSv))
                {
                    Protocols.Services.ProtocolSv sv = (Protocols.Services.ProtocolSv)network;
                    foreach (Network server in sv.NetworkList)
                    {
                        if (server.ServerName == "irc.tm-irc.org")
                        {
                            server.Join("#pidgeon");
                            return true;
                        }
                    }
                } else if (network.GetType() == typeof(Protocols.ProtocolIrc))
                {
                    Network n_ = ((Protocols.ProtocolIrc)network).NetworkMeta;
                    if (n_.ServerName == "irc.tm-irc.org")
                    {
                        n_.Join("#pidgeon");
                        return true;
                    }
                }
            }
            return false;
        }

        private void pidgeonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!join())
            {
                Connections.ConnectIRC("irc.tm-irc.org");
                join();
            }
        }
    }
}
