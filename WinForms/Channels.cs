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
        private ListViewColumnSorter lvwColumnSorter;
        public List<Network.ChannelData> Data = new List<Network.ChannelData>();

        public Channels(Network nw)
        {
            network = nw;
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = lvwColumnSorter;
        }

        private void Reload()
        {
            listView1.Items.Clear();
            channels = 0;
            lock (network.ChannelList)
            {
                Data.Clear();
                Data.AddRange(network.ChannelList);
            }

            timer1.Enabled = true;
        }

        private void Channels_Close(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (network.Connected)
                {
                    e.Cancel = true;
                    Hide();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Channels_Load(object sender, EventArgs e)
        {
            try
            {
                refreshAutoToolStripMenuItem.Checked = true;
                Reload();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Sort(object sender, ColumnClickEventArgs e)
        {
            try
            {
                if (e.Column == lvwColumnSorter.SortColumn)
                {
                    // Reverse the current sort direction for this column.
                    if (lvwColumnSorter.Order == SortOrder.Ascending)
                    {
                        lvwColumnSorter.Order = SortOrder.Descending;
                    }
                    else
                    {
                        lvwColumnSorter.Order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // Set the column number that is to be sorted; default to ascending.
                    lvwColumnSorter.SortColumn = e.Column;
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }

                // Perform the sotrt with hese new sort options.
                this.listView1.Sort();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void timerl_Tick(object sender, EventArgs e)
        {
            try
            {
                if (network.DownloadingList)
                {
                    return;
                }
                if (timer1.Enabled == false)
                {
                    if (channels != network.ChannelList.Count)
                    {
                        Reload();
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void refreshAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void downloadListFromServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                network.SuppressData = true;
                network.Transfer("LIST");
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
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
            try
            {
                lock (listView1.Items)
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        network.Transfer("KNOCK " + item.Text);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Visible)
                {
                    lock (Data)
                    {
                        int curr = 0;
                        if (Data.Count == 0)
                        {
                            timer1.Enabled = false;
                            return;
                        }
                        while (curr < 100 && Data.Count > 0)
                        {
                            ListViewItem item = new ListViewItem(Data[0].ChannelName);
                            item.SubItems.Add(Data[0].UserCount.ToString());
                            item.SubItems.Add(Data[0].ChannelTopic);
                            listView1.Items.Add(item);
                            Data.RemoveAt(0);
                            channels++;
                            curr++;
                        }
                        Text = "Channel list [" + channels.ToString() + "]";
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
