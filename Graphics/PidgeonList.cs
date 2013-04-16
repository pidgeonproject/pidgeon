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
        private Gdk.Pixbuf icon_2 = Gdk.Pixbuf.LoadFromResource("Client.Resources.icon_hash.png");
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
        private object ObjectStore = null;
        private TreeIter IterStore;
        private bool ResultStore = true;

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                return (ServiceList.Count == 0 && ServerList.Count == 0
                        && UserList.Count == 0 && ChannelList.Count == 0);
            }
        }

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
            pict.PackStart(icon, true);
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

        /// <summary>
        /// This function removes a reference to channel object, it is being called only by destructor of Channel
        /// </summary>
        /// <param name="channel"></param>
        public void RemoveChannel(Channel channel)
        {
            lock (queueChannels)
            {
                if (queueChannels.Contains(channel))
                {
                    queueChannels.Remove(channel);
                }
            }
            lock (ChannelList)
            {
                if (ChannelList.ContainsKey(channel))
                {
                    KeyValuePair<TreeIter, bool> result = getIter(channel);
                    if (result.Value)
                    {
                        TreeIter tree = result.Key;
                        Values.Remove(ref tree);
                    }
                    else
                    {
                        Core.DebugLog("Can't remove from sidebar because reference isn't present: " + channel.Name);
                    }
                    ChannelList.Remove(channel);
                }
            }
        }

        public void RemoveServer(Network server)
        {
            lock (queueNetwork)
            {
                if (queueNetwork.Contains(server))
                {
                    queueNetwork.Remove(server);
                }
            }

            lock (ServerList)
            {
                if (ServerList.ContainsKey(server))
                {
                    KeyValuePair<TreeIter, bool> result = getIter(server);
                    if (result.Value)
                    {
                        TreeIter tree = result.Key;
                        Values.Remove(ref tree);
                    }
                    else
                    {
                        Core.DebugLog("Can't remove from sidebar because reference isn't present: " + server.ServerName);
                    }
                    ServerList.Remove(server);
                }
            }
        }

        /// <summary>
        /// This function removes a reference to user object, it is being called only by destructor of User
        /// </summary>
        /// <param name="channel"></param>
        public void RemoveUser(User user)
        {
            lock (queueUsers)
            {
                if (queueUsers.Contains(user))
                {
                    queueUsers.Remove(user);
                }
            }
            lock (UserList)
            {
                if (UserList.ContainsKey(user))
                {
                    KeyValuePair<TreeIter, bool> result = getIter(user);
                    if (result.Value)
                    {
                        TreeIter tree = result.Key;
                        Values.Remove(ref tree);
                    }
                    else
                    {
                        Core.DebugLog("Can't remove from sidebar because reference isn't present: " + user.Nick);
                    }
                    UserList.Remove(user);
                }
            }
        }

        public void InitStyle()
        {
            tv.ModifyBase(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.backgroundcolor));
            tv.ModifyText(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.colordefault));
        }

        private bool feIter(TreeModel model, TreePath path, TreeIter iter)
        {
            if (Values.GetValue(iter, 1) == ObjectStore)
            {
                ResultStore = true;
                IterStore = iter;
                return true;
            }
            return false;
        }

        /// <summary>
        /// This function return iter and true in case that the iter is found, if not it return some random iter and false
        /// It's ugly shit because the creator of gtk# decided to disallow doing it pretty way
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public KeyValuePair<TreeIter, bool> getIter(object item)
        {
            lock (this)
            {
                ObjectStore = item;
                IterStore = new TreeIter();
                ResultStore = false;
                Values.Foreach(feIter);
                ObjectStore = null;

                return new KeyValuePair<TreeIter, bool>(IterStore, ResultStore);
            }
        }

        /// <summary>
        /// This function handles the icons and colors in list
        /// </summary>
        /// <param name="column"></param>
        /// <param name="cell"></param>
        /// <param name="model"></param>
        /// <param name="iter"></param>
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
                        if (nw.IsDestroyed)
                        {
                            Values.Remove(ref iter);
                            return;
                        }
                        if (nw != null && !nw.IsDestroyed && nw.SystemWindow != null)
                        {
                            (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(nw.SystemWindow.MenuColor);
                        }
                        break;
                    case ItemType.User:
                        User user = (User)model.GetValue(iter, 1);
                        if (user.IsDestroyed)
                        {
                            Values.Remove(ref iter);
                        }
                        lock (user._Network.PrivateWins)
                        {
                            if (user._Network.PrivateWins.ContainsKey(user))
                            {
                                window = user._Network.PrivateWins[user];
                            }
                        }
                        if (window != null && !window.IsDestroyed)
                        {
                            (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(window.MenuColor);
                        }
                        break;
                    case ItemType.Channel:
                        Channel channel = (Channel)model.GetValue(iter, 1);
                        if (channel.IsDestroyed)
                        {
                            Values.Remove(ref iter);
                        }
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
                    if (user._Network._Protocol.Windows.ContainsKey(user._Network.SystemWindowID + user.Nick))
                    {
                        user._Network._Protocol.Windows[user._Network.SystemWindowID + user.Nick].treeNode = text;
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

        /// <summary>
        /// This function insert a service to a list, thread safe
        /// </summary>
        /// <param name="service">Service</param>
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
                    TreeIter text = Values.InsertWithValues(ServerList[channel._Network], 0, channel.Name, channel, ItemType.Channel, channel.retrieveWindow(), channel.MenuData, icon_2);
                    TreePath path = tv.Model.GetPath(ServerList[channel._Network]);
                    tv.ExpandRow(path, true);

                    lock (ChannelList)
                    {
                        ChannelList.Add(channel, text);
                    }
                    channel.TreeNode = text;
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

                // we sort out all networks that are waiting to be inserted to list
                lock (queueNetwork)
                {
                    foreach (Network it in queueNetwork)
                    {
                        _insertNetwork(it);
                    }
                    queueNetwork.Clear();
                }

                // we sort out all channels that are waiting to be inserted to list
                lock (queueChannels)
                {
                    foreach (Channel item in queueChannels)
                    {
                        insertChan(item);
                    }
                    queueChannels.Clear();
                }

                // we sort out all services that are waiting to be inserted to list
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
                            if (chan.Key._Network.Channels.Contains(chan.Key))
                            {
                                chan.Key._Network.Channels.Remove(chan.Key);
                            }
                            Graphics.Window window = chan.Key.retrieveWindow();
                            if (window != null)
                            {
                                window._Destroy();
                            }
                            _channels.Add(chan.Key);
                        }
                    }
                }

                foreach (var chan in _channels)
                {
                    ChannelList.Remove(chan);
                    chan.Destroy();
                }

                // if there are waiting window requests we process them here
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
                        if (!channel.Key.IsDestroyed && channel.Key.Redraw)
                        {
                            channel.Key.redrawUsers();
                        }
                    }
                }

                // we check if there are some data at all, if not we can safely remove
                // items in store and skip all checks
                if (IsEmpty)
                {
                    Values.Clear();
                    return true;
                }

                // check all destroyed windows
                TreeIter iter;
                if (Values.GetIterFirst(out iter))
                {
                    do
                    {
                        // in case the window is destroyed we need to remove the reference
                        var window = (Window)Values.GetValue(iter, 3);
                        if (window != null)
                        {
                            if (window.IsDestroyed)
                            {
                                Values.Remove(ref iter);
                            }
                        }
                    } while (Values.IterNext(ref iter));
                }

                ClearServer();
                ClearUser();
                ClearChan();
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

        /// <summary>
        /// Delete unused channels
        /// </summary>
        private void ClearUser()
        {
            List<User> removedUser = new List<User>();
            lock (UserList)
            {
                foreach (User d in UserList.Keys)
                {
                    if (d.IsDestroyed)
                    {
                        removedUser.Add(d);
                        KeyValuePair<TreeIter, bool> result = getIter(d);
                        if (result.Value)
                        {
                            TreeIter tree = result.Key;
                            Values.Remove(ref tree);
                        }
                        else
                        {
                            Core.DebugLog("Can't remove user from sidebar because there is no reference " + d.Nick);
                        }
                    }
                }
                foreach (User d in removedUser)
                {
                    UserList.Remove(d);
                }
            }
        }

        /// <summary>
        /// Delete unused networks
        /// </summary>
        private void ClearServer()
        {
            List<Network> removedChan = new List<Network>();
            lock (ServerList)
            {
                foreach (Network d in ServerList.Keys)
                {
                    if (d.IsDestroyed)
                    {
                        removedChan.Add(d);
                        KeyValuePair<TreeIter, bool> result = getIter(d);
                        if (result.Value)
                        {
                            TreeIter tree = result.Key;
                            Values.Remove(ref tree);
                        }
                        else
                        {
                            Core.DebugLog("unable to remove network from list " + d.ServerName);
                        }
                    }
                }
                foreach (Network d in removedChan)
                {
                    ServerList.Remove(d);
                }
            }
        }

        /// <summary>
        /// Delete unused channels
        /// </summary>
        private void ClearChan()
        {
            List<Channel> removedChan = new List<Channel>();
            lock (ChannelList)
            {
                foreach (Channel d in ChannelList.Keys)
                {
                    if (d.IsDestroyed)
                    {
                        removedChan.Add(d);
                        KeyValuePair<TreeIter, bool> result = getIter(d);
                        if (result.Value)
                        {
                            TreeIter tree = result.Key;
                            Values.Remove(ref tree);
                        }
                        else
                        {
                            Core.DebugLog("unable to remove channel from sidebar " + d.Name);
                        }
                    }
                }
                foreach (Channel d in removedChan)
                {
                    ChannelList.Remove(d);
                }
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
                    Updated = true;
                    break;
                case ItemType.Server:
                    Network network = (Network)Item;
                    if (network.IsConnected)
                    {
                        Core._Main.Chat.scrollback.InsertText("Server will not be removed from sidebar, because you are still using it, disconnect first", Client.ContentLine.MessageStyle.System, false, 0, true);
                        return;
                    }

                    network._Protocol.Exit();

                    lock (Core.Connections)
                    {
                        if (Core.Connections.Contains(network._Protocol))
                        {
                            Core.Connections.Remove(network._Protocol);
                        }
                    }

                    Updated = true;

                    RemoveServer(network);
                    removed = true;
                    break;
                case ItemType.User:
                    User user = (User)Item;
                    if (user._Network != null && !user._Network.IsDestroyed)
                    {
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
                    }

                    lock (user._Network._Protocol.Windows)
                    {
                        if (user._Network._Protocol.Windows.ContainsKey(user._Network.SystemWindowID + user.Nick))
                        {
                            Core._Main.rootToolStripMenuItem_Click(null, null);
                            user._Network._Protocol.Windows[user._Network.SystemWindowID + user.Nick]._Destroy();
                            user._Network._Protocol.Windows.Remove(user._Network.SystemWindowID + user.Nick);
                        }
                    }

                    RemoveUser(user);
                    removed = true;
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

                    Core._Main.rootToolStripMenuItem_Click(null, null);

                    lock (channel._Network._Protocol.Windows)
                    {
                        if (channel._Network._Protocol.Windows.ContainsKey(channel._Network.SystemWindowID + channel.Name))
                        {
                            channel._Network._Protocol.Windows[channel._Network.SystemWindowID + channel.Name]._Destroy();
                            channel._Network._Protocol.Windows.Remove(channel._Network.SystemWindowID + channel.Name);
                        }
                    }

                    RemoveChannel(channel);
                    removed = true;
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
                        Values.Remove(ref iter);
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

