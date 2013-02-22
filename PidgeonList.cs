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
using System.Windows.Forms;

namespace Client
{
    public partial class PidgeonList : UserControl
    {
        public Dictionary<ProtocolSv, TreeNode> ServiceList = new Dictionary<ProtocolSv, TreeNode>();

        public Dictionary<Network, TreeNode> Servers = new Dictionary<Network, TreeNode>();
        public LinkedList<User> _User = new LinkedList<User>();
        public Dictionary<Channel, TreeNode> Channels = new Dictionary<Channel, TreeNode>();
        public LinkedList<Channel> ChannelsQueue = new LinkedList<Channel>();
        public Dictionary<User, TreeNode> UserList = new Dictionary<User, TreeNode>();
        public List<Network> NetworkQueue = new List<Network>();

        public PidgeonList()
        {
            InitializeComponent();
        }

        private void list_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    items.SelectedNode = e.Node;
                    contextMenuStrip1.Show(items, e.Location);
                    closeToolStripMenuItem.Enabled = true;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Clicked(object sender, EventArgs e)
        {
            try
            {
                if (items.SelectedNode == null)
                {
                    closeToolStripMenuItem.Enabled = false;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Prepare the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelList_Load(object sender, EventArgs e)
        {
            try
            {
                RedrawMenu();
                items.BackColor = Configuration.CurrentSkin.backgroundcolor;
                items.ForeColor = Configuration.CurrentSkin.fontcolor;
                items.Font = new Font(Configuration.CurrentSkin.localfont, Configuration.CurrentSkin.fontsize);
                items.ItemHeight = (int)(Configuration.CurrentSkin.fontsize * 2);
                versionToolStripMenuItem.Enabled = false;
                closeToolStripMenuItem.Enabled = false;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void insertChannel(Channel chan)
        {
            lock (ChannelsQueue)
            {
                if (ChannelsQueue.Contains(chan))
                {
                    return;
                }
                ChannelsQueue.AddLast(chan);
            }
        }

        public void insertUser(User _us)
        {
            lock (_User)
            {
                _User.AddLast(_us);
            }
        }

        private void _insertUs(User _us)
        {
            lock (Servers)
            {
                if (Servers.ContainsKey(_us._Network))
                {
                    this.SuspendLayout();
                    TreeNode text = new TreeNode();
                    text.ImageIndex = 1;
                    Servers[_us._Network].Nodes.Insert(Servers[_us._Network].Nodes.Count, text);
                    text.Text = _us.Nick;

                    UserList.Add(_us, text);
                    Servers[_us._Network].Expand();
                    if (_us._Network._protocol.windows.ContainsKey(_us._Network.window + _us.Nick))
                    {
                        _us._Network._protocol.windows[_us._Network.window + _us.Nick].ln = text;
                    }
                    this.ResumeLayout();
                }
            }
        }

        public void insertSv(ProtocolSv service)
        {
            this.SuspendLayout();
            TreeNode text = new TreeNode();
            text.Text = service.Server;
            lock (ServiceList)
            {
                ServiceList.Add(service, text);
            }
            service.windows["!root"].ln = text;
            this.items.Nodes.Add(text);
            this.ResumeLayout();
        }

        private void insertChan(Channel chan)
        {
            lock (Servers)
            {
                if (Servers.ContainsKey(chan._Network))
                {
                    this.SuspendLayout();
                    TreeNode text = new TreeNode();
                    text.Text = chan.Name;
                    Servers[chan._Network].Expand();
                    Servers[chan._Network].Nodes.Insert(0, text);
                    lock (Channels)
                    {
                        Channels.Add(chan, text);
                    }
                    chan.tn = text;
                    text.ImageIndex = 2;
                    Window xx = chan.retrieveWindow();
                    if (xx != null)
                    {
                        xx.ln = text;
                    }
                    this.ResumeLayout();
                }
            }
        }

        public void _insertNetwork(Network network)
        {
            if (network.ParentSv == null)
            {
                this.SuspendLayout();
                TreeNode text = new TreeNode();
                text.Text = network.server;
                lock (Servers)
                {
                    Servers.Add(network, text);
                }
                text.Expand();
                network.system.ln = text;
                this.items.Nodes.Add(text);
                return;
            }
            if (this.ServiceList.ContainsKey(network.ParentSv))
            {
                this.SuspendLayout();
                TreeNode text = new TreeNode();
                text.Text = network.server;
                network.system.ln = text;
                ServiceList[network.ParentSv].Nodes.Add(text);
                Servers.Add(network, text);
                ServiceList[network.ParentSv].Expand();
            }
            this.ResumeLayout();
        }

        /// <summary>
        /// insert network to lv
        /// </summary>
        /// <param name="network"></param>
        public void insertNetwork(Network network, ProtocolSv ParentSv = null)
        {
            if (NetworkQueue.Contains(network)) return;
            lock (NetworkQueue)
            {
                network.ParentSv = ParentSv;
                NetworkQueue.Add(network);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Core.notification_waiting)
                {
                    Core.DisplayNote();
                }
                lock (NetworkQueue)
                {
                    foreach (Network it in NetworkQueue)
                    {
                        _insertNetwork(it);
                    }
                    NetworkQueue.Clear();
                }
                lock (ChannelsQueue)
                {
                    foreach (Channel item in ChannelsQueue)
                    {
                        insertChan(item);
                    }
                    ChannelsQueue.Clear();
                }
                List<Channel> _channels = new List<Channel>();
                lock (Channels)
                {
                    foreach (var chan in Channels)
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
                    Channels.Remove(chan);
                }
                lock (Core._Main.WindowRequests)
                {
                    foreach (Main._WindowRequest item in Core._Main.WindowRequests)
                    {
                        Core._Main.CreateChat(item.window, item.owner, item.focus);
                        if (item.owner != null && item.focus)
                        {
                            item.owner.ShowChat(item.name);
                        }
                    }
                    Core._Main.WindowRequests.Clear();
                }
                lock (UserList)
                {
                    foreach (User user in _User)
                    {
                        _insertUs(user);
                    }
                    _User.Clear();
                }
                lock (Channels)
                {
                    foreach (var channel in Channels)
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
        }

        public void RedrawMenu()
        {
            partToolStripMenuItem.Visible = false;
            joinToolStripMenuItem.Visible = false;
            disconnectToolStripMenuItem.Visible = false;
        }

        private void items_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                RedrawMenu();
                items.SelectedNode.ForeColor = Configuration.CurrentSkin.fontcolor;
                Core._Main.userToolStripMenuItem.Visible = false;
                lock (ServiceList)
                {
                    if (ServiceList.ContainsValue(e.Node))
                    {
                        foreach (var sv in ServiceList)
                        {
                            if (sv.Value == e.Node)
                            {
                                sv.Key.ShowChat("!root");
                                Core.network = null;
                                disconnectToolStripMenuItem.Visible = true;
                                Core._Main.UpdateStatus();
                                return;
                            }
                        }
                    }
                }
                lock (Servers)
                {
                    if (Servers.ContainsValue(e.Node))
                    {
                        foreach (var cu in Servers)
                        {
                            if (cu.Value == e.Node)
                            {
                                if (cu.Key.ParentSv == null)
                                {
                                    cu.Key._protocol.ShowChat("!system");
                                }
                                else
                                {
                                    cu.Key.ParentSv.ShowChat("!" + cu.Key.window);
                                }
                                Core.network = cu.Key;
                                disconnectToolStripMenuItem.Visible = true;
                                Core._Main.UpdateStatus();
                                return;
                            }
                        }
                    }
                }

                lock (UserList)
                {
                    if (UserList.ContainsValue(e.Node))
                    {
                        foreach (var cu in UserList)
                        {
                            if (cu.Value == e.Node)
                            {
                                Core.network = cu.Key._Network;
                                cu.Key._Network._protocol.ShowChat(cu.Key._Network.window + cu.Key.Nick);
                                closeToolStripMenuItem.Visible = true;
                                Core._Main.UpdateStatus();
                                return;
                            }
                        }
                    }
                }

                lock (Channels)
                {
                    if (Channels.ContainsValue(e.Node))
                    {
                        foreach (var cu in Channels)
                        {
                            if (cu.Value == e.Node)
                            {
                                Core.network = cu.Key._Network;
                                partToolStripMenuItem.Visible = true;
                                closeToolStripMenuItem.Visible = true;
                                cu.Key._Network.RenderedChannel = cu.Key;
                                cu.Key._Network._protocol.ShowChat(cu.Key._Network.window + cu.Key.Name);
                                Core._Main.UpdateStatus();
                                return;
                            }
                        }
                    }
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
                if (items.SelectedNode == null)
                { return; }

                RemoveItem(items.SelectedNode);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void RemoveItem(TreeNode Item)
        {
            try
            {
                lock (ServiceList)
                {
                    if (ServiceList.ContainsValue(Item))
                    {
                        ProtocolSv network = null;
                        foreach (var curr in ServiceList)
                        {
                            if (curr.Value == Item)
                            {
                                network = curr.Key;
                                network.Exit();
                            }
                            lock (Item.Nodes)
                            {
                                foreach (TreeNode node in Item.Nodes)
                                {
                                    RemoveItem(node);
                                }
                                if (items.Nodes.Contains(items.SelectedNode))
                                {
                                    items.Nodes.Remove(items.SelectedNode);
                                }
                            }
                        }
                    }
                }

                lock (Servers)
                {
                    if (Servers.ContainsValue(Item))
                    {
                        Network network = null;
                        foreach (var cu in Servers)
                        {
                            if (cu.Value == Item)
                            {
                                network = cu.Key;
                                if (cu.Key.Connected)
                                {
                                    Core._Main.Chat.scrollback.InsertText("Server will not be removed from sidebar, because you are still using it, disconnect first", Scrollback.MessageStyle.System, false, 0, true);
                                    return;
                                }

                            }
                        }
                        if (network != null)
                        {
                            Core.Connections.Remove(network._protocol);
                            Servers.Remove(network);
                            foreach (TreeNode item in items.SelectedNode.Nodes)
                            {
                                RemoveItem(item);
                            }
                            lock (items.Nodes)
                            {
                                if (items.Nodes.Contains(Item))
                                {
                                    items.Nodes.Remove(Item);
                                }
                            }
                        }
                    }
                }

                lock (UserList)
                {
                    if (UserList.ContainsValue(Item))
                    {
                        User curr = null;
                        foreach (var cu in UserList)
                        {
                            if (cu.Value == Item)
                            {
                                lock (cu.Key._Network.PrivateChat)
                                {
                                    if (cu.Key._Network.PrivateChat.Contains(cu.Key))
                                    {
                                        cu.Key._Network.PrivateChat.Remove(cu.Key);
                                    }
                                }
                                curr = cu.Key;
                                break;
                            }
                        }
                        if (curr != null)
                        {
                            lock (items.Nodes)
                            {
                                if (items.Nodes.Contains(Item))
                                {
                                    items.Nodes.Remove(Item);
                                }
                            }
                            if (curr._Network._protocol.windows.ContainsKey(curr._Network.window + curr.Nick))
                            {
                                lock (curr._Network._protocol.windows)
                                {
                                    curr._Network._protocol.windows[curr._Network.window + curr.Nick].Visible = false;
                                    curr._Network._protocol.windows[curr._Network.window + curr.Nick].Dispose();
                                }
                            }
                            lock (UserList)
                            {
                                if (UserList.ContainsKey(curr))
                                {
                                    UserList.Remove(curr);
                                }
                            }
                        }
                    }
                }

                lock (Channels)
                {
                    if (Channels.ContainsValue(Item))
                    {
                        Channel item = null;
                        foreach (var cu in Channels)
                        {
                            if (cu.Value == Item)
                            {
                                if (cu.Key.ok)
                                {
                                    cu.Key._Network._protocol.Part(cu.Key.Name);
                                    cu.Key.dispose = true;
                                    return;
                                }
                                lock (cu.Key._Network.Channels)
                                {
                                    if (cu.Key._Network.Channels.Contains(cu.Key))
                                    {
                                        cu.Key.Dispose();
                                        cu.Key._Network.Channels.Remove(cu.Key);
                                    }
                                }
                                item = cu.Key;
                                break;
                            }
                        }
                        if (item != null)
                        {
                            lock (items.Nodes)
                            {
                                if (items.Nodes.Contains(Item))
                                {
                                    items.Nodes.Remove(Item);
                                }
                            }
                            lock (item.retrieveWindow())
                            {
                                if (item.retrieveWindow() != null)
                                {
                                    item.retrieveWindow().Visible = false;
                                    item.retrieveWindow().Dispose();
                                }
                                lock (item._Network.Channels)
                                {
                                    item._Network.Channels.Remove(item);
                                }
                            }
                            lock (Channels)
                            {
                                Channels.Remove(item);
                            }
                        }
                    }
                }
                lock (items.Nodes)
                {
                    if (items.Nodes.Contains(Item))
                    {
                        items.Nodes.Remove(Item);
                    }
                }
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                lock (Servers)
                {
                    if (Servers.ContainsValue(items.SelectedNode))
                    {
                        foreach (var cu in Servers)
                        {
                            if (cu.Value == items.SelectedNode)
                            {
                                cu.Key.Disconnect();
                                return;
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
                if (Channels.ContainsValue(items.SelectedNode))
                {
                    foreach (var cu in Channels)
                    {
                        if (cu.Value == items.SelectedNode)
                        {
                            if (cu.Key.ok)
                            {
                                cu.Key._Network._protocol.Part(cu.Key.Name);
                                cu.Key.ok = false;
                                return;
                            }
                            return;
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
