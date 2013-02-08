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
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Channels : Form
    {
        public Network network = null;
        private int channels = 0;

        public Channels(Network nw)
        {
            network = nw;
            InitializeComponent();
        }

        private void Reload()
        {
            listView1.Items.Clear();
            channels = 0;
            lock (network.ChannelList)
            {
                foreach (Network.ChannelData channel_info in network.ChannelList)
                {
                    ListViewItem item = new ListViewItem(channel_info.ChannelName);
                    item.SubItems.Add(channel_info.UserCount.ToString());
                    item.SubItems.Add(channel_info.ChannelTopic);
                    listView1.Items.Add(item);
                    channels++;
                }
            }
            Text = "Channels [" + listView1.Items.Count.ToString() + "] on " + network.server;
        }

        private void Channels_Load(object sender, EventArgs e)
        {
            refreshAutoToolStripMenuItem.Checked = true;
            Reload();
        }

        private void timerl_Tick(object sender, EventArgs e)
        {
            if (channels != network.ChannelList.Count)
            {
                Reload();
            }
        }

        private void refreshAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (refreshAutoToolStripMenuItem.Checked)
            {
                timerl.Enabled = false;
                refreshAutoToolStripMenuItem.Checked = false;
            }
            else
            {
                timerl.Enabled = true;
                refreshAutoToolStripMenuItem.Checked = true;
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void downloadListFromServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            network.SuppressData = true;
            network.Transfer("LIST");
        }

        private void joinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                lock (listView1.Items)
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        if (item.Text != "")
                        {
                            network.Join(item.Text);
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void knockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (listView1.Items)
            {
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    network.Transfer("KNOCK " + item.Text);
                }
            }
        }
    }
}
