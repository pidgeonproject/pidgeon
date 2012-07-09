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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class PidgeonList : UserControl
    {
        public Dictionary<Network, TreeNode> Servers = new Dictionary<Network, TreeNode>();
        public LinkedList<User> _User = new LinkedList<User>();
        public Dictionary<Channel, TreeNode> Channels = new Dictionary<Channel, TreeNode>();
        public LinkedList<Channel> ChannelsQueue = new LinkedList<Channel>();
        public Dictionary<User, TreeNode> UserList = new Dictionary<User, TreeNode>();

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
            items.Font = new Font(Configuration.CurrentSkin.localfont, float.Parse(Configuration.CurrentSkin.fontsize.ToString()) * 4);
        }

        public void insertChannel(Channel chan)
        {
            ChannelsQueue.AddLast(chan);
        }

        public void insertUser(User _us)
        {
            _User.AddLast(_us);
        }

        private void _insertUs(User _us)
        {
            if (Servers.ContainsKey(_us._Network))
            {
                TreeNode text = new TreeNode();
                text.Text = _us.Nick;
                Servers[_us._Network].Nodes.Add(text);
                UserList.Add(_us, text);
                if (_us._Network._protocol.windows.ContainsKey(_us.Nick))
                {
                    _us._Network._protocol.windows[_us.Nick].scrollback.ln = text;
                }
            }
        }

        private void insertChan(Channel chan)
        {
            if (Servers.ContainsKey(chan._Network))
            {
                TreeNode text = new TreeNode();
                text.Text = chan.Name;
                Servers[chan._Network].Nodes.Add(text);
                Channels.Add(chan, text);
                Window xx = chan.retrieveWindow();
                if (xx != null)
                {
                    xx.scrollback.ln = text;
                }
            }
        }

        /// <summary>
        /// insert network to lv
        /// </summary>
        /// <param name="network"></param>
        public void insertNetwork(Network network)
        {
            TreeNode text = new TreeNode();
            text.Text = network.server;
            Servers.Add(network, text);
            text.Expand();
            network._protocol.windows["!system"].scrollback.ln = text;
            this.items.Nodes.Add(text);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
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
                    Core._Main.CreateChat(item.window);
                    if (item.owner != null && item._Focus)
                    {
                        item._Focus = false;
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
            try
            {
                if (Servers.ContainsValue(e.Node))
                {
                    foreach (var cu in Servers)
                    {
                        if (cu.Value == e.Node)
                        {
                            cu.Key._protocol.ShowChat("!system");
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
                            cu.Key._Network._protocol.ShowChat(cu.Key.Nick);
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
                            cu.Key._Network._protocol.ShowChat(cu.Key.Name);
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



            try
            {
                if (Servers.ContainsValue(items.SelectedNode))
                {
                    Network network = null;
                    foreach (var cu in Servers)
                    {
                        if (cu.Value == items.SelectedNode)
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
                        items.Nodes.Remove(items.SelectedNode);
                    }
                }
                if (Channels.ContainsValue(items.SelectedNode))
                {
                    Channel item = null;
                    foreach (var cu in Channels)
                    {
                        if (cu.Value == items.SelectedNode)
                        {
                            if (cu.Key.ok)
                            {
                                cu.Key._Network._protocol.Part(cu.Key.Name);
                                cu.Key.dispose = true;
                                return;
                            }
                            item = cu.Key;
                            break;
                        }
                    }
                    if (item != null)
                    {
                        items.Nodes.Remove(items.SelectedNode);
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
