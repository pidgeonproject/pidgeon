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
    [System.ComponentModel.ToolboxItem(true)]
    public partial class PidgeonList : Gtk.Bin
    {
        /// <summary>
        /// List of services which are currently in sidebar
        /// </summary>
        public Dictionary<ProtocolSv, TreeIter> ServiceList = new Dictionary<ProtocolSv, TreeIter>();
        /// <summary>
        /// List of servers which are currently in sidebar
        /// </summary>
        public Dictionary<Network, TreeIter> ServerList = new Dictionary<Network, TreeIter>();
        /// <summary>
        /// List of channels which are currently in sidebar
        /// </summary>
        public Dictionary<Channel, TreeIter> ChannelList = new Dictionary<Channel, TreeIter>();
        public Dictionary<User, TreeIter> UserList = new Dictionary<User, TreeIter>();
        private LinkedList<User> queueUsers = new LinkedList<User>();
        private LinkedList<Channel> queueChannels = new LinkedList<Channel>();
        public List<ProtocolSv> queueProtocol = new List<ProtocolSv>();
        private List<Network> queueNetwork = new List<Network>();
        public static bool Updated = false;
		private Gdk.Pixbuf icon_at = Gdk.Pixbuf.LoadFromResource("Client.Resources.at.png");
		private Gdk.Pixbuf icon_2 = Gdk.Pixbuf.LoadFromResource("Client.Resources.hash.png");
		private Gdk.Pixbuf icon_0 = Gdk.Pixbuf.LoadFromResource("Client.Resources.exclamation mark.png");
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TreeView tv;
        private Gtk.TreeStore Values = new TreeStore(typeof(string),
                                                     typeof(object),
                                                     typeof(ItemType),
                                                     typeof(Window),
		                                             typeof(string),
		                                             typeof(Gdk.Pixbuf));
        private GLib.TimeoutHandler timer;

        public GTK.Menu partToolStripMenuItem = new GTK.Menu("Part");
        public GTK.Menu closeToolStripMenuItem = new GTK.Menu("Close");
        public GTK.Menu joinToolStripMenuItem = new GTK.Menu("Join");
        public GTK.Menu disconnectToolStripMenuItem = new GTK.Menu("Disconnect");

        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            global::Stetic.BinContainer.Attach(this);
            this.Name = "Client.Graphics.PidgeonList";
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            this.tv = new global::Gtk.TreeView();
            this.tv.CanFocus = true;
            this.tv.Name = "treeview1";
            Gtk.TreeViewColumn Column = new TreeViewColumn();
			Gtk.TreeViewColumn pict = new Gtk.TreeViewColumn();
            Gtk.CellRendererText Item = new Gtk.CellRendererText();
			Gtk.CellRendererPixbuf icon = new CellRendererPixbuf();
            Column.Title = messages.get("list-active-conn", messages.Language);
            Column.PackStart(Item, true);
            Column.SetCellDataFunc(Item, UserListRendererTool);
			pict.AddAttribute(icon, "pixbuf", 5);
			tv.AppendColumn(pict);
            tv.AppendColumn(Column);
            tv.PopupMenu += Menu;
            tv.TooltipColumn = 4;
            tv.ButtonPressEvent += new ButtonPressEventHandler(Menu2);
            Column.AddAttribute(Item, "text", 0);
            this.tv.Model = Values;
            this.GtkScrolledWindow.Add(this.tv);
            timer = new GLib.TimeoutHandler(timer01_Tick);
            GLib.Timeout.Add(200, timer);
            this.tv.RowActivated += new RowActivatedHandler(items_AfterSelect);
            this.Add(this.GtkScrolledWindow);
            this.tv.CursorChanged += new EventHandler(items_AfterSelect2);
            partToolStripMenuItem.Enabled = true;
            disconnectToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Hide();
        }

        [GLib.ConnectBefore]
        private void Menu2(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                Menu(sender, null);
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

        public void InitStyle()
        {
            tv.ModifyBase(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.backgroundcolor));
            tv.ModifyText(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.colordefault));
        }

        private void UserListRendererTool(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            try
            {
                ItemType type = (ItemType)model.GetValue(iter, 2);
                Window window = null;
                switch (type)
                {
                    case ItemType.Server:
                        Network nw = (Network)model.GetValue(iter, 1);
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(nw.SystemWindow.MenuColor);
                        break;
                    case ItemType.User:
                        User user = (User)model.GetValue(iter, 1);
                        lock (user._Network.PrivateWins)
                        {
                            if (user._Network.PrivateWins.ContainsKey(user))
                            {
                                window = user._Network.PrivateWins[user];
                            }
                        }
                        if (window != null)
                        {
                            (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(window.MenuColor);
                        }
                        break;
                    case ItemType.Channel:
                        Channel channel = (Channel)model.GetValue(iter, 1);
                        string data = (string)model.GetValue(iter, 4);
                        if (data != channel.MenuData)
                        {
                            model.SetValue(iter, 4, channel.MenuData);
                        }
                        window = channel.retrieveWindow();
                        if (window != null)
                        {
                            (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(window.MenuColor);
                        }
                        break;
                    case ItemType.Services:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(Configuration.CurrentSkin.colordefault);
                        break;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public PidgeonList()
        {
            try
            {
                this.Build();
                this.InitStyle();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Insert a channel to list
        /// </summary>
        /// <param name="channel"></param>
        public void insertChannel(Channel channel)
        {
            lock (queueChannels)
            {
                if (queueChannels.Contains(channel))
                {
                    return;
                }
                queueChannels.AddLast(channel);
                Updated = true;
            }
        }

        /// <summary>
        /// Insert a user to list (thread safe)
        /// </summary>
        /// <param name="user"></param>
        public void insertUser(User user)
        {
            lock (queueUsers)
            {
                queueUsers.AddLast(user);
            }
            Updated = true;
        }

        private void _insertUser(User user)
        {
            lock (ServerList)
            {
                if (ServerList.ContainsKey(user._Network))
                {
                    //text.ImageIndex = 4;
                    TreeIter text = Values.AppendValues(ServerList[user._Network], user.Nick, user, ItemType.User, null, null, icon_at);
                    TreePath path = tv.Model.GetPath(ServerList[user._Network]);
                    tv.ExpandRow(path, true);

                    lock (UserList)
                    {
                        UserList.Add(user, text);
                    }
                    if (user._Network._Protocol.Windows.ContainsKey(user._Network.window + user.Nick))
                    {
                        user._Network._Protocol.Windows[user._Network.window + user.Nick].treeNode = text;
                    }
                    Updated = true;
                }
            }
        }

        private void insertService(ProtocolSv service)
        {
            TreeIter text = Values.AppendValues(service.Server, service, ItemType.Services, service.SystemWindow, "Root window of services", icon_0);
            lock (ServiceList)
            {
                ServiceList.Add(service, text);
            }
            service.Windows["!root"].treeNode = text;
        }

        public void insertSv(ProtocolSv service)
        {
            lock (queueProtocol)
            {
                queueProtocol.Add(service);
            }
            Updated = true;
        }

        private void insertChan(Channel channel)
        {
            lock (ServerList)
            {
                if (ServerList.ContainsKey(channel._Network))
                {
                    TreeIter text = Values.AppendValues(ServerList[channel._Network], channel.Name, channel, ItemType.Channel, channel.retrieveWindow(), channel.MenuData, icon_2);
                    TreePath path = tv.Model.GetPath(ServerList[channel._Network]);
                    tv.ExpandRow(path, true);

                    lock (ChannelList)
                    {
                        ChannelList.Add(channel, text);
                    }
                    channel.TreeNode = text;
                    //text.ImageIndex = 6;
                    Graphics.Window xx = channel.retrieveWindow();
                    if (xx != null)
                    {
                        xx.treeNode = text;
                    }
                }
            }
        }

        private void _insertNetwork(Network network)
        {
            if (network.ParentSv == null)
            {
                TreeIter text = Values.AppendValues(network.ServerName, network, ItemType.Server, null, null, icon_0);
                lock (ServerList)
                {
                    ServerList.Add(network, text);
                }
                network.SystemWindow.treeNode = text;
                return;
            }
            if (this.ServiceList.ContainsKey(network.ParentSv))
            {
                TreeIter text = Values.AppendValues(ServiceList[network.ParentSv], network.ServerName, network, ItemType.Server, null, null, icon_0);
                TreePath path = tv.Model.GetPath(ServiceList[network.ParentSv]);
                tv.ExpandRow(path, true);
                network.SystemWindow.treeNode = text;
                ServerList.Add(network, text);
            }
        }

        /// <summary>
        /// insert network to lv (thread safe)
        /// </summary>
        /// <param name="network"></param>
        public void insertNetwork(Network network, ProtocolSv ParentSv = null)
        {
            if (queueNetwork.Contains(network)) return;
            lock (queueNetwork)
            {
                network.ParentSv = ParentSv;
                queueNetwork.Add(network);
                Updated = true;
            }
        }

        private bool timer01_Tick()
        {
            try
            {
                if (Core.notification_waiting)
                {
                    Core.DisplayNote();
                }

                // there is no update needed so skip
                if (!Updated)
                {
                    return true;
                }

                Updated = false;

                tv.ColumnsAutosize();

                lock (queueNetwork)
                {
                    foreach (Network it in queueNetwork)
                    {
                        _insertNetwork(it);
                    }
                    queueNetwork.Clear();
                }

                lock (queueChannels)
                {
                    foreach (Channel item in queueChannels)
                    {
                        insertChan(item);
                    }
                    queueChannels.Clear();
                }

                lock (queueProtocol)
                {
                    foreach (ProtocolSv item in queueProtocol)
                    {
                        insertService(item);
                    }
                    queueProtocol.Clear();
                }

                List<Channel> _channels = new List<Channel>();
                lock (ChannelList)
                {
                    foreach (var chan in ChannelList)
                    {
                        if (chan.Key.dispose)
                        {
                            chan.Key._Network.Channels.Remove(chan.Key);
                            chan.Key.retrieveWindow().Dispose();
                            _channels.Add(chan.Key);
                        }
                    }
                }

                foreach (var chan in _channels)
                {
                    ChannelList.Remove(chan);
                }

                lock (Core._Main.WindowRequests)
                {
                    foreach (Forms.Main._WindowRequest item in Core._Main.WindowRequests)
                    {
                        Core._Main.CreateChat(item.window, item.owner, item.focus);
                        if (item.owner != null && item.focus)
                        {
                            item.owner.ShowChat(item.name);
                        }
                    }
                    Core._Main.WindowRequests.Clear();
                }

                lock (queueUsers)
                {
                    foreach (User user in queueUsers)
                    {
                        _insertUser(user);
                    }
                    queueUsers.Clear();
                }

                lock (ChannelList)
                {
                    foreach (var channel in ChannelList)
                    {
                        if (channel.Key.Redraw)
                        {
                            channel.Key.redrawUsers();
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
        }

        public void RedrawMenu()
        {
            partToolStripMenuItem.Visible = false;
            joinToolStripMenuItem.Visible = false;
            disconnectToolStripMenuItem.Visible = false;
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
                        partToolStripMenuItem.Visible = true;
                        closeToolStripMenuItem.Visible = true;
                        chan._Network.RenderedChannel = chan;
                        chan._Network._Protocol.ShowChat(chan._Network.window + chan.Name);
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
                            server.ParentSv.ShowChat("!" + server.window);
                        }
                        server.SystemWindow.MenuColor = Configuration.CurrentSkin.fontcolor;
                        Core.network = server;
                        disconnectToolStripMenuItem.Visible = true;
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
                    case ItemType.System:
                        break;
                    case ItemType.User:
                        User us = (User)tv.Model.GetValue(iter, 1); ;
                        Core.network = us._Network;
                        us._Network._Protocol.ShowChat(us._Network.window + us.Nick);
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

        public void RemoveAll(TreeIter iter)
        {

        }

        public void RemoveItem(TreeIter it, object Item, ItemType type)
        {
            bool removed = false;
            switch (type)
            {
                case ItemType.Services:
                    ProtocolSv service = (ProtocolSv)Item;
                    service.Exit();
                    lock (ServiceList)
                    {
                        if (ServiceList.ContainsKey(service))
                        {
                            lock (Values)
                            {
                                TreeIter iter = ServiceList[service];
                                Values.Remove(ref iter);
                                removed = true;
                            }
                            ServiceList.Remove(service);
                        }
                    }
                    break;
                case ItemType.Server:
                    Network network = (Network)Item;
                    if (network.Connected)
                    {
                        Core._Main.Chat.scrollback.InsertText("Server will not be removed from sidebar, because you are still using it, disconnect first", Client.ContentLine.MessageStyle.System, false, 0, true);
                        return;
                    }

                    lock (Core.Connections)
                    {
                        if (Core.Connections.Contains(network._Protocol))
                        {
                            Core.Connections.Remove(network._Protocol);
                        }
                    }

                    lock (ServerList)
                    {
                        if (ServerList.ContainsKey(network))
                        {
                            TreeIter iter = ServerList[network];
                            removed = true;
                            Values.Remove(ref iter);
                            ServerList.Remove(network);
                        }
                    }
                    break;
                case ItemType.User:
                    User user = (User)Item;

                    lock (user._Network.PrivateChat)
                    {
                        if (user._Network.PrivateChat.Contains(user))
                        {
                            lock (user._Network.PrivateWins)
                            {
                                if (user._Network.PrivateWins.ContainsKey(user))
                                {
                                    user._Network.PrivateWins.Remove(user);
                                }
                                else
                                {
                                    Core.DebugLog("There was no private window handle for " + user.Nick);
                                }
                            }
                            user._Network.PrivateChat.Remove(user);
                        }
                    }
                    if (user._Network._Protocol.Windows.ContainsKey(user._Network.window + user.Nick))
                    {
                        lock (user._Network._Protocol.Windows)
                        {
                            Core._Main.rootToolStripMenuItem_Click(null, null);
                            user._Network._Protocol.Windows.Remove(user._Network.window + user.Nick);
                        }
                    }
                    lock (UserList)
                    {
                        if (UserList.ContainsKey(user))
                        {
                            TreeIter iter = UserList[user];
                            Values.Remove(ref iter);
                            removed = true;
                            UserList.Remove(user);
                        }
                    }
                    break;
                case ItemType.Channel:
                    Channel channel = (Channel)Item;

                    if (channel.ChannelWork && !channel.dispose)
                    {
                        channel._Network._Protocol.Part(channel.Name);
                        channel.dispose = true;
                        Core.DebugLog("Unable to remove channel because it's active: " + channel.Name);
                        return;
                    }

                    lock (channel._Network.Channels)
                    {
                        if (channel._Network.Channels.Contains(channel))
                        {
                            channel._Network.Channels.Remove(channel);
                        }
                    }
	
					lock (channel._Network._Protocol.Windows)
					{
						if (channel._Network._Protocol.Windows.ContainsKey(channel._Network.window + channel.Name))
						{
							channel._Network._Protocol.Windows[channel._Network.window + channel.Name].Dispose();
							channel._Network._Protocol.Windows.Remove(channel._Network.window + channel.Name);
						}
					}

                    lock (ChannelList)
                    {
                        if (ChannelList.ContainsKey(channel))
                        {
                            TreeIter iter = ChannelList[channel];
                            Values.Remove(ref iter);
                            removed = true;
                            ChannelList.Remove(channel);
                        }
                    }
                    break;
            }

            if (removed == false)
            {
                Values.Remove(ref it);
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
                if (type == ItemType.Server)
                {
                    Network item = (Network)tv.Model.GetValue(iter, 1);
                    lock (ServerList)
                    {
                        if (ServerList.ContainsKey(item))
                        {
                            if (item.Connected)
                            {
                                item.Disconnect();
                            }
                            else
                            {
                                Core._Main.Chat.scrollback.InsertText("Not connected", ContentLine.MessageStyle.System, false);
                            }
                        }
                    }
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
                        if (channel.ChannelWork)
                        {
                            channel._Network.Part(channel);
                        }
                        else
                        {
                            Core._Main.Chat.scrollback.InsertText("This channel isn't working", ContentLine.MessageStyle.System, false);
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public enum ItemType
        {
            Server,
            Services,
            System,
            Channel,
            User,
        }
    }
}

