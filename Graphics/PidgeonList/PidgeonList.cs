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
using System.ComponentModel;
using Gtk;

namespace Client.Graphics
{
    /// <summary>
    /// Menu
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class PidgeonList : Gtk.Bin
    {
        /// <summary>
        /// List of services which are currently in sidebar
        /// </summary>
        public Dictionary<ProtocolSv, TreeIter> ServiceList = new Dictionary<ProtocolSv, TreeIter>();
        /// <summary>
        /// Quassel core
        /// </summary>
        public Dictionary<ProtocolQuassel, TreeIter> QuasselList = new Dictionary<ProtocolQuassel, TreeIter>();
        /// <summary>
        /// List of servers which are currently in sidebar
        /// </summary>
        public Dictionary<Network, TreeIter> ServerList = new Dictionary<Network, TreeIter>();
        /// <summary>
        /// List of channels which are currently in sidebar
        /// </summary>
        public Dictionary<Channel, TreeIter> ChannelList = new Dictionary<Channel, TreeIter>();
        /// <summary>
        /// List of all users that are rendered in list
        /// </summary>
        public Dictionary<User, TreeIter> UserList = new Dictionary<User, TreeIter>();
        /// <summary>
        /// List of DCC
        /// </summary>
        public Dictionary<ProtocolDCC, TreeIter> DirectClientConnectionList = new Dictionary<ProtocolDCC, TreeIter>();
        private LinkedList<User> queueUsers = new LinkedList<User>();
        private LinkedList<Channel> queueChannels = new LinkedList<Channel>();
        private List<ProtocolSv> queueProtocol = new List<ProtocolSv>();
        private List<Network> queueNetwork = new List<Network>();
        private List<ProtocolQuassel> queueQs = new List<ProtocolQuassel>();
        private List<ProtocolDCC> queueDcc = new List<ProtocolDCC>();
        /// <summary>
        /// If this is false the system timer will refresh the list
        /// </summary>
        public static bool Updated = false;
        private Gdk.Pixbuf icon_at2 = Gdk.Pixbuf.LoadFromResource("Client.Resources.at-s.png");
        private Gdk.Pixbuf icon_22 = Gdk.Pixbuf.LoadFromResource("Client.Resources.hash-s.png");
        private Gdk.Pixbuf icon_02 = Gdk.Pixbuf.LoadFromResource("Client.Resources.exclamation-mark-s.png");
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
                return (ServiceList.Count == 0 && ServerList.Count == 0 && QuasselList.Count == 0
                        && UserList.Count == 0 && ChannelList.Count == 0);
            }
        }

        /// <summary>
        /// Creates a new instance of list
        /// </summary>
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

        private void Build()
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
            Column.Title = messages.get("list-active-conn", Core.SelectedLanguage);
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
            soundsToolStripMenuItem.Visible = true;
            highlightToolStripMenuItem.Visible = true;
            this.tv.ModifyFg(StateType.Normal, Core.FromColor(Configuration.CurrentSkin.fontcolor));
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Hide();
        }

        /// <summary>
        /// This function removes a reference to DCC object, it is being called only by destructor of DCC protocol
        /// </summary>
        /// <param name="dcc"></param>
        public bool RemoveDcc(ProtocolDCC dcc)
        {
            lock (queueDcc)
            {
                if (queueDcc.Contains(dcc))
                {
                    queueDcc.Remove(dcc);
                    return true;
                }
            }
            lock (DirectClientConnectionList)
            {
                if (DirectClientConnectionList.ContainsKey(dcc))
                {
                    KeyValuePair<TreeIter, bool> result = getIter(dcc);
                    if (result.Value)
                    {
                        TreeIter tree = result.Key;
                        Values.Remove(ref tree);
                    }
                    else
                    {
                        Core.DebugLog("Can't remove from sidebar because reference isn't present: " + dcc.Server);
                    }
                    this.DirectClientConnectionList.Remove(dcc);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This function removes a reference to channel object, it is being called only by destructor of Channel
        /// </summary>
        /// <param name="channel"></param>
        public bool RemoveChannel(Channel channel)
        {
            lock (queueChannels)
            {
                if (queueChannels.Contains(channel))
                {
                    queueChannels.Remove(channel);
                    return true;
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
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove a server
        /// </summary>
        /// <param name="server"></param>
        public bool RemoveServer(Network server)
        {
            lock (queueNetwork)
            {
                if (queueNetwork.Contains(server))
                {
                    queueNetwork.Remove(server);
                    return true;
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
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This function removes object, it is being called only by destructor of quassel core
        /// </summary>
        /// <param name="protocol">Quassel core</param>
        public bool RemoveQuassel(ProtocolQuassel protocol)
        {
            lock (queueQs)
            {
                if (queueQs.Contains(protocol))
                {
                    queueQs.Remove(protocol);
                }
            }
            lock (QuasselList)
            {
                if (QuasselList.ContainsKey(protocol))
                {
                    KeyValuePair<TreeIter, bool> result = getIter(protocol);
                    if (result.Value)
                    {
                        TreeIter tree = result.Key;
                        Values.Remove(ref tree);
                    }
                    else
                    {
                        Core.DebugLog("Can't remove from sidebar because reference isn't present: " + protocol.Server);
                    }
                    QuasselList.Remove(protocol);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This function removes object, it is being called only by destructor of quassel core
        /// </summary>
        /// <param name="protocol">Quassel core</param>
        public bool RemoveServ(ProtocolSv protocol)
        {
            lock (queueProtocol)
            {
                if (queueProtocol.Contains(protocol))
                {
                    queueProtocol.Remove(protocol);
                }
            }
            lock (ServiceList)
            {
                if (ServiceList.ContainsKey(protocol))
                {
                    KeyValuePair<TreeIter, bool> result = getIter(protocol);
                    if (result.Value)
                    {
                        TreeIter tree = result.Key;
                        Values.Remove(ref tree);
                    }
                    else
                    {
                        Core.DebugLog("Can't remove from sidebar because reference isn't present: " + protocol.Server);
                    }
                    ServiceList.Remove(protocol);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This function removes a reference to user object, it is being called only by destructor of User
        /// </summary>
        /// <param name="user">User</param>
        public bool RemoveUser(User user)
        {
            lock (queueUsers)
            {
                if (queueUsers.Contains(user))
                {
                    queueUsers.Remove(user);
                    return true;
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
                    return true;
                }
            }
            return false;
        }

        private void InitStyle()
        {
            tv.ModifyBase(StateType.Normal, Core.FromColor(Configuration.CurrentSkin.backgroundcolor));
            tv.ModifyText(StateType.Normal, Core.FromColor(Configuration.CurrentSkin.colordefault));
        }

        /// <summary>
        /// This function is foreach of store where the sidebar is located
        /// </summary>
        /// <param name="model"></param>
        /// <param name="path"></param>
        /// <param name="iter"></param>
        /// <returns></returns>
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
                        string info = null;
                        if (nw.IrcdVersion != null)
                        {
                            info = "IRCD version " + nw.IrcdVersion;
                        }
                        string nwinfo = (string)model.GetValue(iter, 4);
                        if (nwinfo != info)
                        {
                            model.SetValue(iter, 4, info);
                        }
                        if (nw != null && !nw.IsDestroyed && nw.SystemWindow != null)
                        {
                            (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(nw.SystemWindow.MenuColor);
                            if (nw.SystemWindow.needIcon)
                            {
                                nw.SystemWindow.needIcon = false;
                                if (nw.IsConnected)
                                {
                                    model.SetValue(iter, 5, icon_0);
                                }
                                else
                                {
                                    model.SetValue(iter, 5, icon_02);
                                }
                            }
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
                            (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(window.MenuColor);
                            if (window.needIcon)
                            {
                                window.needIcon = false;
                                if (user._Network == null || !user._Network.IsConnected)
                                {
                                    model.SetValue(iter, 5, icon_at2);
                                }
                                else
                                {
                                    model.SetValue(iter, 5, icon_at);
                                }
                            }
                        }
                        break;
                    case ItemType.DCC:
                        ProtocolDCC dc = (ProtocolDCC)model.GetValue(iter, 1);
                        if (dc.IsDestroyed)
                        {
                            Values.Remove(ref iter);
                        }
                        if (dc.SystemWindow.needIcon)
                        {
                            dc.SystemWindow.needIcon = false;
                            if (dc.IsConnected)
                            {
                                model.SetValue(iter, 5, icon_0);
                            }
                            else
                            {
                                model.SetValue(iter, 5, icon_02);
                            }
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
                        window = channel.RetrieveWindow();
                        if (window != null)
                        {
                            (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(window.MenuColor);
                            if (window.needIcon)
                            {
                                window.needIcon = false;
                                if (!channel.IsAlive)
                                {
                                    model.SetValue(iter, 5, icon_22);
                                }
                                else
                                {
                                    model.SetValue(iter, 5, icon_2);
                                }
                            }
                        }
                        break;
                    case ItemType.Services:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.colordefault);
                        break;
                }
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
        [Obsolete]
        public void insertChannel(Channel channel)
        {
            InsertChannel(channel);
        }

        /// <summary>
        /// Insert a channel to list
        /// </summary>
        /// <param name="channel"></param>
        public void InsertChannel(Channel channel)
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

        private void insertDcc(ProtocolDCC protocol)
        {
            TreeIter text = Values.AppendValues(protocol.UserName, protocol, ItemType.DCC, protocol.SystemWindow, "DCC connection window", icon_0);
            lock (DirectClientConnectionList)
            {
                this.DirectClientConnectionList.Add(protocol, text);
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

        /// <summary>
        /// Select a window in menu
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public bool ReselectWindow(Graphics.Window window)
        {
            // first we need to find an item this window belongs to
            if (window == null)
            {
                return false;
            }

            lock (ChannelList)
            {
                foreach (Channel channel in ChannelList.Keys)
                {
                    if (channel.RetrieveWindow() == window)
                    {
                        KeyValuePair<TreeIter, bool> result = getIter(channel);
                        if (result.Value)
                        {
                            tv.Selection.SelectIter(result.Key);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void _insertUser(User user)
        {
            lock (ServerList)
            {
                if (ServerList.ContainsKey(user._Network))
                {
                    TreeIter text;
                    if (user._Network.IsConnected)
                    {
                        text = Values.AppendValues(ServerList[user._Network], user.Nick, user, ItemType.User, null, null, icon_at);
                    }
                    else
                    {
                        text = Values.AppendValues(ServerList[user._Network], user.Nick, user, ItemType.User, null, null, icon_at2);
                    }
                    TreePath path = tv.Model.GetPath(ServerList[user._Network]);
                    tv.ExpandRow(path, true);

                    lock (UserList)
                    {
                        UserList.Add(user, text);
                    }
                    Updated = true;
                }
            }
        }  

        private void insertService(ProtocolSv service)
        {
            string tx = "Root window of services [port: " + service.Port.ToString() + " Encrypted: " + service.SSL.ToString() + "]";
            TreeIter text = Values.AppendValues(service.Server, service, ItemType.Services, service.SystemWindow, tx, icon_0);
            lock (ServiceList)
            {
                ServiceList.Add(service, text);
            }
        }

        private void insertQuassel(ProtocolQuassel service)
        {
            TreeIter text = Values.AppendValues(service.Server, service, ItemType.QuasselCore, service.SystemWindow, "Root window of quassel", icon_0);
            lock (QuasselList)
            {
                QuasselList.Add(service, text);
            }
        }

        /// <summary>
        /// Insert dcc
        /// </summary>
        /// <param name="protocol"></param>
        public void InsertDcc(ProtocolDCC protocol)
        {
            lock (queueDcc)
            {
                queueDcc.Add(protocol);
            }
            Updated = true;
        }

        /// <summary>
        /// Insert quassel
        /// </summary>
        /// <param name="service"></param>
        public void InsertQuassel(ProtocolQuassel service)
        {
            lock (queueQs)
            {
                queueQs.Add(service);
            }
            Updated = true;
        }

        /// <summary>
        /// This function insert a service to a list, thread safe
        /// </summary>
        /// <param name="service">Service</param>
        public void InsertSv(ProtocolSv service)
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
                    TreeIter text;
                    if (channel.IsAlive)
                    {
                        text = Values.InsertWithValues(ServerList[channel._Network], 0, channel.Name, channel,
                            ItemType.Channel, channel.RetrieveWindow(), channel.MenuData, icon_2);
                    }
                    else
                    {
                        text = Values.InsertWithValues(ServerList[channel._Network], 0, channel.Name, channel,
                            ItemType.Channel, channel.RetrieveWindow(), channel.MenuData, icon_02);
                    }
                    TreePath path = tv.Model.GetPath(ServerList[channel._Network]);
                    tv.ExpandRow(path, true);

                    lock (ChannelList)
                    {
                        ChannelList.Add(channel, text);
                    }
                }
            }
        }

        private void insertNetwork(Network network)
        {
            string info = null;
            if (network.IrcdVersion != null)
            {
                info = "IRCD version " + network.IrcdVersion;
            }
            if (network.ParentSv == null)
            {
                TreeIter text = Values.AppendValues(network.ServerName, network, ItemType.Server, null, info, icon_0);
                lock (ServerList)
                {
                    ServerList.Add(network, text);
                }
                return;
            }
            if (this.ServiceList.ContainsKey(network.ParentSv))
            {
                TreeIter text = Values.AppendValues(ServiceList[network.ParentSv], network.ServerName, network, ItemType.Server, null, info, icon_0);
                TreePath path = tv.Model.GetPath(ServiceList[network.ParentSv]);
                tv.ExpandRow(path, true);
                ServerList.Add(network, text);
            }
        }

        /// <summary>
        /// insert network to lv (thread safe)
        /// </summary>
        /// <param name="network"></param>
        /// <param name="ParentSv"></param>
        public void InsertNetwork(Network network, ProtocolSv ParentSv = null)
        {
            if (queueNetwork.Contains(network)) return;
            lock (queueNetwork)
            {
                network.ParentSv = ParentSv;
                queueNetwork.Add(network);
                Updated = true;
            }
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

        private void RemoveItem(TreeIter it, object Item, ItemType type)
        {
            bool removed = false;
            switch (type)
            {
                case ItemType.Services:
                    ProtocolSv service = (ProtocolSv)Item;
                    service.Exit();
                    removed = RemoveServ(service);
                    Updated = true;
                    break;
                case ItemType.QuasselCore:
                    ProtocolQuassel protocol = (ProtocolQuassel)Item;
                    protocol.Exit();
                    removed = RemoveQuassel(protocol);
                    Updated = true;
                    break;
                case ItemType.DCC:
                    ProtocolDCC dcc = (ProtocolDCC)Item;
                    dcc.Exit();
                    removed = RemoveDcc(dcc);
                    Updated = true;
                    break;
                case ItemType.Server:
                    Network network = (Network)Item;
                    if (network.ParentSv != null)
                    {
                        lock (ServerList)
                        {
                            if (ServerList.ContainsKey(network))
                            {
                                RemoveServer(network);
                            }
                        }
                        network.Disconnect();
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
                    removed = true;
                    RemoveServer(network);
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
                            Core.SystemForm.SwitchRoot();
                            user._Network._Protocol.Windows[user._Network.SystemWindowID + user.Nick]._Destroy();
                            user._Network._Protocol.Windows.Remove(user._Network.SystemWindowID + user.Nick);
                        }
                    }

                    removed = RemoveUser(user);
                    break;
                case ItemType.Channel:
                    Channel channel = (Channel)Item;

                    if (channel.IsAlive)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("Unable to remove channel because it's active, you need to part it: " + channel.Name, ContentLine.MessageStyle.System, false, 0);
                        return;
                    }

                    lock (channel._Network.Channels)
                    {
                        if (channel._Network.Channels.Contains(channel))
                        {
                            channel._Network.Channels.Remove(channel);
                        }
                    }

                    Core.SystemForm.SwitchRoot();

                    lock (channel._Network._Protocol.Windows)
                    {
                        if (channel._Network._Protocol.Windows.ContainsKey(channel._Network.SystemWindowID + channel.Name))
                        {
                            channel._Network._Protocol.Windows[channel._Network.SystemWindowID + channel.Name]._Destroy();
                            channel._Network._Protocol.Windows.Remove(channel._Network.SystemWindowID + channel.Name);
                        }
                    }

                    removed = RemoveChannel(channel);
                    break;
            }

            if (removed == false)
            {
                Values.Remove(ref it);
            }
        }

        /// <summary>
        /// Item
        /// </summary>
        public enum ItemType
        {
            /// <summary>
            /// Network
            /// </summary>
            Server,
            /// <summary>
            /// ProtocolSv
            /// </summary>
            Services,
            /// <summary>
            /// System window
            /// </summary>
            System,
            /// <summary>
            /// Channel
            /// </summary>
            Channel,
            /// <summary>
            /// User
            /// </summary>
            User,
            /// <summary>
            /// Quassel
            /// </summary>
            QuasselCore,
            /// <summary>
            /// DCC
            /// </summary>
            DCC,
        }
    }
}

