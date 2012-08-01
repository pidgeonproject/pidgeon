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

        /// <summary>
        /// Prepare the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChannelList_Load(object sender, EventArgs e)
        {
            RedrawMenu();
            items.BackColor = Configuration.CurrentSkin.backgroundcolor;
            items.ForeColor = Configuration.CurrentSkin.fontcolor;
            items.Font = new Font(Configuration.CurrentSkin.localfont, Configuration.CurrentSkin.fontsize);
            items.ItemHeight = (int)(Configuration.CurrentSkin.fontsize * 2);
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
            if (Servers.ContainsKey(_us._Network))
            {
                this.SuspendLayout();
                TreeNode text = new TreeNode();
                text.ImageIndex = 1;
                Servers[_us._Network].Nodes.Add(text);
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

        public void insertSv(ProtocolSv service)
        {
            this.SuspendLayout();
            TreeNode text = new TreeNode();
            text.Text = service.Server;
            ServiceList.Add(service, text);
            service.windows["!root"].ln = text;
            this.items.Nodes.Add(text);
            this.ResumeLayout();
        }

        private void insertChan(Channel chan)
        {
            if (Servers.ContainsKey(chan._Network))
            {
                this.SuspendLayout();
                TreeNode text = new TreeNode();
                text.Text = chan.Name;
                Servers[chan._Network].Expand();
                Servers[chan._Network].Nodes.Add(text);
                Channels.Add(chan, text);
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

        public void _insertNetwork(Network network)
        {
            if (network.ParentSv == null)
            {
                this.SuspendLayout();
                TreeNode text = new TreeNode();
                text.Text = network.server;
                Servers.Add(network, text);
                text.Expand();
                network.system.ln = text;
                this.items.Nodes.Add(text);
                return;
            }
            if(this.ServiceList.ContainsKey(network.ParentSv))
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
            lock (Core._Main.W)
            {
                foreach (Main._WindowRequest item in Core._Main.W)
                {
                    Core._Main.CreateChat(item.window, item.owner, item._Focus);
                    if (item.owner != null && item._Focus)
                    {
                        item.owner.ShowChat(item.name);
                    }
                }
                Core._Main.W.Clear();
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

        public void RedrawMenu()
        {
            partToolStripMenuItem.Visible = false;
            joinToolStripMenuItem.Visible = false;
            disconnectToolStripMenuItem.Visible = false;
        }

        private void items_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RedrawMenu();
            items.SelectedNode.ForeColor = Configuration.CurrentSkin.fontcolor;
            Core._Main.userToolStripMenuItem.Visible = false;
            try
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
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (items.SelectedNode == null)
            { return; }

            RemoveItem(items.SelectedNode);
        }

        public void RemoveItem(TreeNode Item)
        {
            try
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
                        foreach (TreeNode node in Item.Nodes)
                        {
                            RemoveItem(node);
                        }
                        items.Nodes.Remove(items.SelectedNode);
                        return;
                    }
                }

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
                        items.Nodes.Remove(Item);
                        return;
                    }
                }

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
                        items.Nodes.Remove(Item);
                        if (curr._Network._protocol.windows.ContainsKey(curr._Network.window + curr.Nick))
                        {
                            lock (curr._Network._protocol.windows)
                            {
                                curr._Network._protocol.windows[curr._Network.window + curr.Nick].Visible = false;
                                curr._Network._protocol.windows[curr._Network.window + curr.Nick].Dispose();
                            }
                        }
                        UserList.Remove(curr);
                        return;
                    }
                }

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
                        items.Nodes.Remove(Item);
                        lock (item.retrieveWindow())
                        {
                            if (item.retrieveWindow() != null)
                            {
                                item.retrieveWindow().Visible = false;
                                item.retrieveWindow().Dispose();
                            }
                            item._Network.Channels.Remove(item);
                        }
                        Channels.Remove(item);
                        return;
                    }
                }
                items.Nodes.Remove(Item);
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Servers.ContainsValue(items.SelectedNode))
            {
                foreach (var cu in Servers)
                {
                    if (cu.Value == items.SelectedNode)
                    {
                        cu.Key._protocol.Exit();
                    }
                }
            }
        }

        private void partToolStripMenuItem_Click(object sender, EventArgs e)
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
    }
}
