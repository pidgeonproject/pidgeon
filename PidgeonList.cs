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
            items.BackColor = Configuration.CurrentSkin.backgroundcolor;
            items.ForeColor = Configuration.CurrentSkin.fontcolor;
            items.Font = new Font(Configuration.CurrentSkin.localfont, float.Parse(Configuration.CurrentSkin.fontsize.ToString()) * 4);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Redraw()
        {

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
            if (Servers.ContainsKey(_us.network))
            {
                TreeNode text = new TreeNode();
                text.Text = _us.Nick;
                Servers[_us.network].Nodes.Add(text);
                UserList.Add(_us, text);
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
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="network"></param>
        public void insertNetwork(Network network)
        {
            TreeNode text = new TreeNode();
            text.Text = network.server;
            Servers.Add(network, text);
            this.items.Nodes.Add(text);
        }

        private void _Display(object sender, EventArgs e)
        {

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
            lock (Core._Main.W)
            {
                foreach (Main._WindowRequest item in Core._Main.W)
                {
                    Core._Main.CreateChat(item.window);
                    item.owner._Chat(item.name, item.writable, item);
                    if (item._Focus)
                    {
                        item.owner.ShowChat(item.name);
                    }
                }
                Core._Main.W.Clear();
            }
            lock (Channels)
            {
                foreach (var channel in Channels)
                {
                    if (channel.Key.Redraw)
                    {
                        channel.Key.redrawUsers();
                        channel.Key.Redraw = false;
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
            try
            {
            if (Servers.ContainsValue(e.Node))
            {
                foreach (var cu in Servers)
                {
                    if (cu.Value == e.Node)
                    {
                        cu.Key.ShowChat("!system");
                        Core.network = cu.Key;
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
                        Core._Main.UpdateStatus();
                        cu.Key._Network.ShowChat(cu.Key.Name);
                        return;
                    }
                }
            }
            }
            catch(Exception f)
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

                        }
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
            catch(Exception f)
            {
                Core.handleException(f);
            }

        }
    }
}
