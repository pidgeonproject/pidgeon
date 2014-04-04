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

using Pidgeon.Protocols.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using Gtk;

namespace Pidgeon.Graphics
{
    /// <summary>
    /// Menu
    /// </summary>
    public partial class PidgeonList : Gtk.Bin
    {
        /// <summary>
        /// Sounds
        /// </summary>
        public PidgeonGtkToolkit.Menu soundsToolStripMenuItem = new PidgeonGtkToolkit.Menu("Enable sound");
        /// <summary>
        /// Notifications
        /// </summary>
        public PidgeonGtkToolkit.Menu highlightToolStripMenuItem = new PidgeonGtkToolkit.Menu("Enable notifications for this window");
        /// <summary>
        /// Menu
        /// </summary>
        public PidgeonGtkToolkit.Menu partToolStripMenuItem = new PidgeonGtkToolkit.Menu("Part");
        /// <summary>
        /// Menu
        /// </summary>
        public PidgeonGtkToolkit.Menu closeToolStripMenuItem = new PidgeonGtkToolkit.Menu("Close");
        /// <summary>
        /// join menu
        /// </summary>
        public PidgeonGtkToolkit.Menu joinToolStripMenuItem = new PidgeonGtkToolkit.Menu("Join");
        /// <summary>
        /// disconnect menu
        /// </summary>
        public PidgeonGtkToolkit.Menu disconnectToolStripMenuItem = new PidgeonGtkToolkit.Menu("Disconnect");
        /// <summary>
        /// Reconnect menu
        /// </summary>
        public PidgeonGtkToolkit.Menu reconnectToolStripMenuItem = new PidgeonGtkToolkit.Menu("Reconnect");
        /// <summary>
        /// Window that is selected now
        /// </summary>
        private Graphics.Window SelectedWindow = null;

        [GLib.ConnectBefore]
        private void Menu2(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                showMenu(sender, null);
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

        /// <summary>
        /// This is called when menu is clicked (right click) on mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [GLib.ConnectBefore]
        private void showMenu(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                bool display = false;
                Gtk.Menu menu = new Menu();
                if (soundsToolStripMenuItem.Visible)
                {
                    Gtk.CheckMenuItem sounds = new CheckMenuItem(soundsToolStripMenuItem.Text);
                    if (SelectedWindow != null)
                    {
                        sounds.Active = SelectedWindow.Sounds;
                    }
                    sounds.Sensitive = Configuration.Media.NotificationSound;
                    sounds.Activated += new EventHandler(soundsToolStripMenuItem_Click);
                    display = true;
                    menu.Append(sounds);
                }
                if (highlightToolStripMenuItem.Visible)
                {
                    Gtk.CheckMenuItem notification = new CheckMenuItem(highlightToolStripMenuItem.Text);
                    notification.Sensitive = Configuration.Kernel.Notice;
                    if (SelectedWindow != null)
                    {
                        notification.Active = SelectedWindow.Highlights;
                    }
                    notification.Activated += new EventHandler(notificationToolStripMenuItem_Click);
                    display = true;
                    menu.Append(notification);
                }
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
                Core.HandleException(fail);
            }
        }

        private void RedrawMenu()
        {
            partToolStripMenuItem.Visible = false;
            joinToolStripMenuItem.Visible = false;
            disconnectToolStripMenuItem.Visible = false;
        }

        private void soundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedWindow != null)
            {
                SelectedWindow.Sounds = !SelectedWindow.Sounds;
            }
        }

        private void notificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedWindow != null)
            {
                SelectedWindow.Highlights = !SelectedWindow.Highlights;
            }
        }

        private void joinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception fail)
            {
                Core.HandleException(fail);
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
                        item.Disconnect();
                        break;
                    case ItemType.Services:
                        ProtocolSv services = (ProtocolSv)tv.Model.GetValue(iter, 1);
                        services.Disconnect();
                        break;
                    case ItemType.DCC:
                        Protocols.ProtocolDCC pd = (Protocols.ProtocolDCC)tv.Model.GetValue(iter, 1);
                        pd.Disconnect();
                        break;
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
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
                Core.HandleException(fail);
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
                SelectedWindow = null;
                RedrawMenu();
                TreeIter iter;
                TreePath[] path = tv.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    Core.DebugLog("items_AfterSelect(object sender, RowActivatedArgs e): GetSelectedRows returned 0 value");
                    return;
                }
                tv.Model.GetIter(out iter, path[0]);
                Hooks._Sys.Poke();
                Window window = null;
                ItemType type = (ItemType)tv.Model.GetValue(iter, 2);
                switch (type)
                {
                    case ItemType.Channel:
                        Channel chan = (Channel)tv.Model.GetValue(iter, 1);
                        Core.SelectedNetwork = chan._Network;
                        window = chan.RetrieveWindow();
                        if (window != null)
                        {
                            SelectedWindow = window;
                            window.MenuColor = Configuration.CurrentSkin.FontColor;
                        }
                        if (!chan.IsAlive)
                        {
                            joinToolStripMenuItem.Visible = true;
                        }
                        partToolStripMenuItem.Visible = true;
                        closeToolStripMenuItem.Visible = true;
                        chan._Network.RenderedChannel = chan;
                        WindowsManager.ShowChat(chan._Network.SystemWindowID + chan.Name, chan._Network);
                        Core.SystemForm.UpdateStatus();
                        return;
                    case ItemType.Server:
                        Network server = (Network)tv.Model.GetValue(iter, 1);
                        if (server.ParentSv == null)
                        {
                            WindowsManager.ShowChat("!system", server);
                        }
                        else
                        {
                            WindowsManager.ShowChat("!" + server.SystemWindowID, server);
                        }
                        server.SystemWindow.MenuColor = Configuration.CurrentSkin.FontColor;
                        SelectedWindow = server.SystemWindow;
                        Core.SelectedNetwork = server;
                        disconnectToolStripMenuItem.Visible = true;
                        closeToolStripMenuItem.Visible = true;
                        Core.SystemForm.UpdateStatus();
                        return;
                    case ItemType.Services:
                        ProtocolSv protocol = (ProtocolSv)tv.Model.GetValue(iter, 1);
                        closeToolStripMenuItem.Visible = true;
                        SelectedWindow = protocol.SystemWindow;
                        WindowsManager.ShowChat("!root", protocol);
                        Core.SelectedNetwork = null;
                        disconnectToolStripMenuItem.Visible = true;
                        Core.SystemForm.UpdateStatus();
                        return;
                    case ItemType.DCC:
                        Protocols.ProtocolDCC dcc = (Protocols.ProtocolDCC)tv.Model.GetValue(iter, 1);
                        closeToolStripMenuItem.Visible = true;
                        SelectedWindow = dcc.SystemWindow;
                        WindowsManager.ShowChat(dcc.SystemWindow.WindowName, dcc);
                        Core.SelectedNetwork = null;
                        disconnectToolStripMenuItem.Visible = true;
                        Core.SystemForm.UpdateStatus();
                        return;
                    case ItemType.QuasselCore:
                        Protocols.ProtocolQuassel quassel = (Protocols.ProtocolQuassel)tv.Model.GetValue(iter, 1);
                        closeToolStripMenuItem.Visible = true;
                        SelectedWindow = quassel.SystemWindow;
                        WindowsManager.ShowChat("!root", quassel);
                        Core.SelectedNetwork = null;
                        disconnectToolStripMenuItem.Visible = true;
                        return;
                    case ItemType.System:
                        return;
                    case ItemType.User:
                        User us = (User)tv.Model.GetValue(iter, 1);
                        Core.SelectedNetwork = us._Network;
                        lock (us._Network.PrivateWins)
                        {
                            if (us._Network.PrivateWins.ContainsKey(us))
                            {
                                window = us._Network.PrivateWins[us];
                            }
                        }
                        if (window != null)
                        {
                            SelectedWindow = window;
                            window.MenuColor = Configuration.CurrentSkin.FontColor;
                        }
                        WindowsManager.ShowChat(us._Network.SystemWindowID + us.Nick, us._Network);
                        closeToolStripMenuItem.Visible = true;
                        Core.SystemForm.UpdateStatus();
                        return;
                }
            }
            catch (Exception f)
            {
                Core.HandleException(f);
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
                Core.HandleException(fail);
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
                Core.HandleException(fail);
            }
        }
    }
}
