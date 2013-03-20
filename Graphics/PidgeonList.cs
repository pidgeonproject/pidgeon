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
        /// <summary>
        /// List of services which are currently in sidebar
        /// </summary>
        public Dictionary<ProtocolSv, TreeNode> ServiceList = new Dictionary<ProtocolSv, TreeNode>();
        /// <summary>
        /// List of servers which are currently in sidebar
        /// </summary>
        public Dictionary<Network, TreeNode> ServerList = new Dictionary<Network, TreeNode>();
        /// <summary>
        /// List of channels which are currently in sidebar
        /// </summary>
        public Dictionary<Channel, TreeNode> ChannelList = new Dictionary<Channel, TreeNode>();
        public Dictionary<User, TreeNode> UserList = new Dictionary<User, TreeNode>();
        private LinkedList<User> queueUsers = new LinkedList<User>();
        private LinkedList<Channel> queueChannels = new LinkedList<Channel>();
        private List<Network> queueNetwork = new List<Network>();
        public static bool Updated = false;

        public PidgeonList()
        {
            InitializeComponent();
        }

        private void DrawTreeNodeHighlightSelectedEvenWithoutFocus(object sender, DrawTreeNodeEventArgs e)
        {
            try
            {
                Color foreColor;
                if (e.Node == ((TreeView)sender).SelectedNode)
                {
                    foreColor = SystemColors.HighlightText;
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                    ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, foreColor, SystemColors.Highlight);
                }
                else
                {
                    SolidBrush blueBrush = new SolidBrush(Configuration.CurrentSkin.backgroundcolor);
                    foreColor = (e.Node.ForeColor == Color.Empty) ? ((TreeView)sender).ForeColor : e.Node.ForeColor;
                    e.Graphics.FillRectangle(blueBrush, e.Bounds);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    e.Node.Text,
                    e.Node.NodeFont ?? e.Node.TreeView.Font,
                    e.Bounds,
                    foreColor,
                    TextFormatFlags.GlyphOverhangPadding);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
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
        }

        private void _insertUser(User user)
        {
            lock (ServerList)
            {
                if (ServerList.ContainsKey(user._Network))
                {
                    this.SuspendLayout();
                    TreeNode text = new TreeNode();
                    text.ImageIndex = 4;
                    ServerList[user._Network].Nodes.Insert(ServerList[user._Network].Nodes.Count, text);
                    text.Text = user.Nick;

                    lock (UserList)
                    {
                        UserList.Add(user, text);
                    }
                    ServerList[user._Network].Expand();
                    if (user._Network._Protocol.Windows.ContainsKey(user._Network.window + user.Nick))
                    {
                        user._Network._Protocol.Windows[user._Network.window + user.Nick].treeNode = text;
                    }
                    Updated = true;
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
            service.Windows["!root"].treeNode = text;
            this.items.Nodes.Add(text);
            this.ResumeLayout();
        }

        private void insertChan(Channel channel)
        {
            lock (ServerList)
            {
                if (ServerList.ContainsKey(channel._Network))
                {
                    this.SuspendLayout();
                    TreeNode text = new TreeNode();
                    text.Text = channel.Name;
                    ServerList[channel._Network].Expand();
                    ServerList[channel._Network].Nodes.Insert(0, text);
                    lock (ChannelList)
                    {
                        ChannelList.Add(channel, text);
                    }
                    channel.TreeNode = text;
                    text.ImageIndex = 6;
                    Window xx = channel.retrieveWindow();
                    if (xx != null)
                    {
                        xx.treeNode = text;
                    }
                    this.ResumeLayout();
                }
            }
        }

        private void _insertNetwork(Network network)
        {
            if (network.ParentSv == null)
            {
                this.SuspendLayout();
                TreeNode text = new TreeNode();
                text.Text = network.ServerName;
                lock (ServerList)
                {
                    ServerList.Add(network, text);
                }
                text.Expand();
                network.SystemWindow.treeNode = text;
                this.items.Nodes.Add(text);
                return;
            }
            if (this.ServiceList.ContainsKey(network.ParentSv))
            {
                this.SuspendLayout();
                TreeNode text = new TreeNode();
                text.Text = network.ServerName;
                network.SystemWindow.treeNode = text;
                ServiceList[network.ParentSv].Nodes.Add(text);
                ServerList.Add(network, text);
                ServiceList[network.ParentSv].Expand();
            }
            this.ResumeLayout();
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

        private void timer2_Tick(object sender, EventArgs e)
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
                    return;
                }

                Updated = false;

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

                lock (ServerList)
                {
                    if (ServerList.ContainsValue(e.Node))
                    {
                        foreach (var cu in ServerList)
                        {
                            if (cu.Value == e.Node)
                            {
                                if (cu.Key.ParentSv == null)
                                {
                                    cu.Key._Protocol.ShowChat("!system");
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
                                cu.Key._Network._Protocol.ShowChat(cu.Key._Network.window + cu.Key.Nick);
                                closeToolStripMenuItem.Visible = true;
                                Core._Main.UpdateStatus();
                                return;
                            }
                        }
                    }
                }

                lock (ChannelList)
                {
                    if (ChannelList.ContainsValue(e.Node))
                    {
                        foreach (var cu in ChannelList)
                        {
                            if (cu.Value == e.Node)
                            {
                                Core.network = cu.Key._Network;
                                partToolStripMenuItem.Visible = true;
                                closeToolStripMenuItem.Visible = true;
                                cu.Key._Network.RenderedChannel = cu.Key;
                                cu.Key._Network._Protocol.ShowChat(cu.Key._Network.window + cu.Key.Name);
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
                {
                    return;
                }

                RemoveItem(items.SelectedNode);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void RemoveAll(TreeNodeCollection Item, TreeNode node)
        {
            if (Item.Contains(node))
            {
                Item.Remove(node);
                return;
            }
            foreach (TreeNode Node in Item)
            {
                if (Node.Nodes.Count > 0)
                {
                    RemoveAll(Node.Nodes, node);
                }
            }
        }

        public void RemoveAll(TreeNode Item)
        {
            if (items.Nodes.Contains(Item))
            {
                items.Nodes.Remove(Item);
                return;
            }
            foreach (TreeNode node in items.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    RemoveAll(node.Nodes, Item);
                }
            }
        }

        public void RemoveItem(TreeNode Item)
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

            lock (ServerList)
            {
                if (ServerList.ContainsValue(Item))
                {
                    Network network = null;
                    foreach (var cu in ServerList)
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
                        Core.Connections.Remove(network._Protocol);
                        ServerList.Remove(network);
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
                                    lock (cu.Key._Network.PrivateWins)
                                    {
                                        if (cu.Key._Network.PrivateWins.ContainsKey(cu.Key))
                                        {
                                            cu.Key._Network.PrivateWins.Remove(cu.Key);
                                        }
                                        else
                                        {
                                            Core.DebugLog("There was no private window handle for " + cu.Key.Nick);
                                        }
                                    }
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
                        if (curr._Network._Protocol.Windows.ContainsKey(curr._Network.window + curr.Nick))
                        {
                            lock (curr._Network._Protocol.Windows)
                            {
                                curr._Network._Protocol.Windows[curr._Network.window + curr.Nick].Visible = false;
                                curr._Network._Protocol.Windows[curr._Network.window + curr.Nick].Dispose();
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

            lock (ChannelList)
            {
                if (ChannelList.ContainsValue(Item))
                {
                    Channel item = null;
                    foreach (var cu in ChannelList)
                    {
                        if (cu.Value == Item)
                        {
                            if (cu.Key.ChannelWork)
                            {
                                cu.Key._Network._Protocol.Part(cu.Key.Name);
                                //cu.Key.dispose = true;
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
                            RemoveAll(Item);
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
                        lock (ChannelList)
                        {
                            ChannelList.Remove(item);
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

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                lock (ServerList)
                {
                    if (ServerList.ContainsValue(items.SelectedNode))
                    {
                        foreach (var cu in ServerList)
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
                if (ChannelList.ContainsValue(items.SelectedNode))
                {
                    foreach (var cu in ChannelList)
                    {
                        if (cu.Value == items.SelectedNode)
                        {
                            if (cu.Key.ChannelWork)
                            {
                                cu.Key._Network._Protocol.Part(cu.Key.Name);
                                cu.Key.ChannelWork = false;
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
