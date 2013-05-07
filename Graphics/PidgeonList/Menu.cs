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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using Gtk;

namespace Client.Graphics
{
    /// <summary>
    /// Menu
    /// </summary>
    public partial class PidgeonList : Gtk.Bin
    {
        [GLib.ConnectBefore]
        private void Menu2(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                Menu(sender, null);
            }

            if (e.Event.Button == 2)
            {
                switch (Configuration.Window.MiddleClick_Side)
                {
                    case Configuration.PidgeonList_MouseClick.Close:
                        closeToolStripMenuItem_Click(null, null);
                        break;
                    case Configuration.PidgeonList_MouseClick.Disconnect:
                        disconnectToolStripMenuItem_Click(null, null);
                        break;
                }
            }
        }

        [GLib.ConnectBefore]
        private void Menu(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                bool display = false;
                Gtk.Menu menu = new Menu();
                if (partToolStripMenuItem.Visible)
                {
                    Gtk.MenuItem part = new MenuItem(partToolStripMenuItem.Text);
                    part.Sensitive = partToolStripMenuItem.Enabled;
                    part.Activated += new EventHandler(partToolStripMenuItem_Click);
                    display = true;
                    menu.Append(part);
                }
                if (closeToolStripMenuItem.Visible)
                {
                    Gtk.MenuItem close = new MenuItem(closeToolStripMenuItem.Text);
                    close.Activated += new EventHandler(closeToolStripMenuItem_Click);
                    close.Sensitive = closeToolStripMenuItem.Enabled;
                    display = true;
                    menu.Append(close);
                }
                if (disconnectToolStripMenuItem.Visible)
                {
                    Gtk.MenuItem disconnect = new MenuItem(disconnectToolStripMenuItem.Text);
                    disconnect.Activated += new EventHandler(disconnectToolStripMenuItem_Click);
                    disconnect.Sensitive = disconnectToolStripMenuItem.Enabled;
                    display = true;
                    menu.Append(disconnect);
                }
                if (joinToolStripMenuItem.Visible)
                {
                    Gtk.MenuItem join = new MenuItem(joinToolStripMenuItem.Text);
                    join.Activated += new EventHandler(joinToolStripMenuItem_Click);
                    display = true;
                    menu.Append(join);
                }
                if (Configuration.Kernel.Debugging)
                {
                    if (reconnectToolStripMenuItem.Visible)
                    {
                        Gtk.MenuItem reconnect = new MenuItem(reconnectToolStripMenuItem.Text);
                        reconnect.Activated += new EventHandler(reconnectToolStripMenuItem_Click);
                        reconnect.Sensitive = reconnectToolStripMenuItem.Enabled;
                        display = true;
                        menu.Append(reconnect);
                    }
                }
                if (display)
                {
                    menu.ShowAll();
                    menu.Popup();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void RedrawMenu()
        {
            partToolStripMenuItem.Visible = false;
            joinToolStripMenuItem.Visible = false;
            disconnectToolStripMenuItem.Visible = false;
        }

        private void joinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeIter iter;
            TreePath[] path = tv.Selection.GetSelectedRows();
            tv.Model.GetIter(out iter, path[0]);
            ItemType type = (ItemType)tv.Model.GetValue(iter, 2);
            if (type == ItemType.Channel)
            {
                Channel ch = (Channel)tv.Model.GetValue(iter, 1);
                ch._Network.Join(ch.Name);
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeIter iter;
                TreePath[] path = tv.Selection.GetSelectedRows();
                tv.Model.GetIter(out iter, path[0]);
                ItemType type = (ItemType)tv.Model.GetValue(iter, 2);
                switch (type)
                {
                    case ItemType.Server:
                        Network item = (Network)tv.Model.GetValue(iter, 1);
                        lock (ServerList)
                        {
                            if (ServerList.ContainsKey(item))
                            {
                                if (item.IsConnected)
                                {
                                    RemoveServer(item);
                                    item.Disconnect();
                                }
                                else
                                {
                                    Core._Main.Chat.scrollback.InsertText("Not connected", ContentLine.MessageStyle.System, false);
                                }
                            }
                        }
                        break;
                    case ItemType.Services:
                        ProtocolSv services = (ProtocolSv)tv.Model.GetValue(iter, 1);
                        services.Disconnect();
                        break;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void partToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeIter iter;
                TreePath[] path = tv.Selection.GetSelectedRows();
                tv.Model.GetIter(out iter, path[0]);
                ItemType type = (ItemType)tv.Model.GetValue(iter, 2);
                if (type == ItemType.Channel)
                {
                    Channel channel = (Channel)tv.Model.GetValue(iter, 1);
                    if (ChannelList.ContainsKey(channel))
                    {
                        channel.Part();
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void items_AfterSelect2(object sender, EventArgs e)
        {
            items_AfterSelect(sender, null);
        }

        private void items_AfterSelect(object sender, RowActivatedArgs e)
        {
            try
            {
                RedrawMenu();
                TreeIter iter;
                TreePath[] path = tv.Selection.GetSelectedRows();
                tv.Model.GetIter(out iter, path[0]);
                Hooks._Sys.Poke();
                Window window = null;
                ItemType type = (ItemType)tv.Model.GetValue(iter, 2);
                switch (type)
                {
                    case ItemType.Channel:
                        Channel chan = (Channel)tv.Model.GetValue(iter, 1);
                        Core.network = chan._Network;
                        window = chan.retrieveWindow();
                        if (window != null)
                        {
                            window.MenuColor = Configuration.CurrentSkin.fontcolor;
                        }
                        if (!chan.IsAlive)
                        {
                            joinToolStripMenuItem.Visible = true;
                        }
                        partToolStripMenuItem.Visible = true;
                        closeToolStripMenuItem.Visible = true;
                        chan._Network.RenderedChannel = chan;
                        chan._Network._Protocol.ShowChat(chan._Network.SystemWindowID + chan.Name);
                        Core._Main.UpdateStatus();
                        break;
                    case ItemType.Server:
                        Network server = (Network)tv.Model.GetValue(iter, 1);
                        if (server.ParentSv == null)
                        {
                            server._Protocol.ShowChat("!system");
                        }
                        else
                        {
                            server.ParentSv.ShowChat("!" + server.SystemWindowID);
                        }
                        server.SystemWindow.MenuColor = Configuration.CurrentSkin.fontcolor;
                        Core.network = server;
                        disconnectToolStripMenuItem.Visible = true;
                        closeToolStripMenuItem.Visible = true;
                        Core._Main.UpdateStatus();
                        break;
                    case ItemType.Services:
                        ProtocolSv protocol = (ProtocolSv)tv.Model.GetValue(iter, 1);
                        closeToolStripMenuItem.Visible = true;
                        protocol.ShowChat("!root");
                        Core.network = null;
                        disconnectToolStripMenuItem.Visible = true;
                        Core._Main.UpdateStatus();
                        break;
                    case ItemType.QuasselCore:
                        ProtocolQuassel quassel = (ProtocolQuassel)tv.Model.GetValue(iter, 1);
                        closeToolStripMenuItem.Visible = true;
                        quassel.ShowChat("!root");
                        Core.network = null;
                        disconnectToolStripMenuItem.Visible = true;
                        break;
                    case ItemType.System:
                        break;
                    case ItemType.User:
                        User us = (User)tv.Model.GetValue(iter, 1); ;
                        Core.network = us._Network;
                        lock (us._Network.PrivateWins)
                        {
                            if (us._Network.PrivateWins.ContainsKey(us))
                            {
                                window = us._Network.PrivateWins[us];
                            }
                        }
                        if (window != null)
                        {
                            window.MenuColor = Configuration.CurrentSkin.fontcolor;
                        }
                        us._Network._Protocol.ShowChat(us._Network.SystemWindowID + us.Nick);
                        closeToolStripMenuItem.Visible = true;
                        Core._Main.UpdateStatus();
                        break;
                }
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        private void reconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeIter iter;
                TreePath[] path = tv.Selection.GetSelectedRows();
                if (path.Length > 0)
                {
                    tv.Model.GetIter(out iter, path[0]);
                    object data = tv.Model.GetValue(iter, 1);
                    ItemType type = (ItemType)tv.Model.GetValue(iter, 2);
                    switch (type)
                    {
                        case ItemType.QuasselCore:
                        case ItemType.Services:
                            Protocol protocol = (Protocol)data;
                            protocol.Reconnect();
                            break;
                        case ItemType.Server:
                            Network network = (Network)data;
                            network.Reconnect();
                            break;
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeIter iter;
                TreePath[] path = tv.Selection.GetSelectedRows();
                if (path.Length > 0)
                {
                    tv.Model.GetIter(out iter, path[0]);
                    object data = tv.Model.GetValue(iter, 1);
                    ItemType type = (ItemType)tv.Model.GetValue(iter, 2);
                    RemoveItem(iter, data, type);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
