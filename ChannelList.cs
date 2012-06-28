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
    public partial class ChannelList : UserControl
    {
        public Dictionary<Network, TreeNode> Servers = new Dictionary<Network,TreeNode>();
        public Dictionary<Channel, TreeNode> Channels = new Dictionary<Channel,TreeNode>();
        public LinkedList<Channel> ChannelsQueue = new LinkedList<Channel>();

        public ChannelList()
        {
            InitializeComponent();
        }

        private void ChannelList_Load(object sender, EventArgs e)
        {
            
        }

        public void Redraw()
        {
            
        }

        public void insertChannel(Channel chan)
        {
            ChannelsQueue.AddLast(chan);
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

        public void insertNetwork(Network network)
        {
            TreeNode text = new TreeNode();
            text.Text = network.server;
            Servers.Add(network, text);
            this.items.Nodes.Add(text);
        }

        private void Display(object sender, EventArgs e)
        {
            items.Height = Height;
            items.Width = Width;
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
                        item.owner._Chat(item.name, item.writable);
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

        private void items_AfterSelect(object sender, TreeViewEventArgs e)
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
    }
}
