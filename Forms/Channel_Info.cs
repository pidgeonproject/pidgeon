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
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;

namespace Client
{
    public partial class Channel_Info : Form
    {
        public Channel channel = null;
        public List<char> cm;
        public Channel_Info()
        {
            InitializeComponent();
        }

        public static string convertUNIX(string time)
        {
            try
            {
                double unixtimestmp = double.Parse(time);
                return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixtimestmp).ToString();
            }
            catch (Exception)
            {
                return "Unable to read: " + time;
            }
        }

        /// <summary>
        /// Prepare the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Channel_Window_Load(object sender, EventArgs e)
        {
            bClose.Text = messages.get("channelinfo-ok", Core.SelectedLanguage);
            tabControl1.TabPages[0].Text = messages.get("channelinfo-t0", Core.SelectedLanguage);
            tabControl1.TabPages[1].Text = messages.get("channelinfo-t2", Core.SelectedLanguage);
            tabControl1.TabPages[2].Text = messages.get("channelinfo-t3", Core.SelectedLanguage);
            tabControl1.TabPages[3].Text = messages.get("channelinfo-t4", Core.SelectedLanguage);

            cm = new List<char>();

            if (channel != null)
            {
                Text = channel.Name;

                ReloadBans();

                lock (channel._mode)
                {
                    foreach (char item in channel._Network.CModes)
                    {
                        string de = "  unknown mode. Refer to ircd manual (/raw help)";
                        cm.Add(item);
                        if (channel._Network.Descriptions.ContainsKey(item))
                        {
                            de = "    " + channel._Network.Descriptions[item];
                        }
                        checkedList.Items.Add(item.ToString() + de, channel._mode._Mode.Contains(item.ToString()));
                        textBox1.Text = channel.Topic;
                    }
                }
            }
        }

        public void ReloadExceptions()
        {
            if (channel != null)
            {
                if (channel.Bans != null)
                {
                    listView3.Items.Clear();
                    lock (channel.Bans)
                    {
                        foreach (Except ex in channel.Exceptions)
                        {
                            ListViewItem li = new ListViewItem(ex.Target);
                            li.SubItems.Add(convertUNIX(ex.Time) + " (" + ex.Time + ")");
                            li.SubItems.Add(ex.User);

                            listView2.Items.Add(li);
                        }
                    }
                }
            }
        }

        public void ReloadBans()
        {
            if (channel != null)
            {
                if (channel.Bans != null)
                {
                    listView3.Items.Clear();
                    lock (channel.Bans)
                    {
                        foreach (SimpleBan sb in channel.Bans)
                        {
                            ListViewItem li = new ListViewItem(sb.Target);
                            li.SubItems.Add(convertUNIX(sb.Time) + " (" + sb.Time + ")");
                            li.SubItems.Add(sb.User);

                            listView3.Items.Add(li);
                        }
                    }
                }
            }
        }

        public void ReloadInvites()
        {
            if (channel != null)
            {
                if (channel.Bans != null)
                {
                    listView3.Items.Clear();
                    lock (channel.Bans)
                    {
                        foreach (Invite sb in channel.Invites)
                        {
                            ListViewItem li = new ListViewItem(sb.Target);
                            li.SubItems.Add(convertUNIX(sb.Time) + " (" + sb.Time + ")");
                            li.SubItems.Add(sb.User);

                            listView1.Items.Add(li);
                        }
                    }
                }
            }
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (channel.Topic != textBox1.Text)
                {
                    channel._Network.Transfer("TOPIC " + channel.Name + " :" + textBox1.Text);
                }
                bool change = false;
                string cset = "+";
                string uset = "-";
                foreach (string item in checkedList.Items)
                {
                    if (checkedList.CheckedItems.Contains(item))
                    {
                        if (!channel._mode._Mode.Contains(item[0].ToString()))
                        {
                            cset += item[0];
                            change = true;
                        }
                        continue;

                    }
                    if (channel._mode._Mode.Contains(item[0].ToString()))
                    {
                        uset += item[0];
                        change = true;
                    }
                }
                if (change)
                {
                    if (uset == "-")
                    {
                        uset = "";
                    }
                    if (cset == "+")
                    {
                        cset = "";
                    }
                    channel._Network.Transfer("MODE " + channel.Name + " " + cset + uset);
                }
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Lines.Count() > 1)
            {
                textBox1.Text = textBox1.Lines[0];
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                if (item.Text != "")
                {
                    channel._Network.Transfer("MODE " + channel.Name + " -I " + item.Text);
                }
            }
        }

        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ReloadBans();
        }

        private void insertToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void enforceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView3.SelectedItems)
            {
                if (item.Text != "")
                {
                    channel._Network.Transfer("MODE " + channel.Name + " -b " + item.Text);
                }
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReloadInvites();
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView2.SelectedItems)
            {
                if (item.Text != "")
                {
                    channel._Network.Transfer("MODE " + channel.Name + " -e " + item.Text);
                }
            }
        }

        private void cleanToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void reloadToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            timerBans.Enabled = true;
            channel.ReloadBans();
        }

        private void timerBans_Tick(object sender, EventArgs e)
        {
            if (!channel.parsing_xb)
            {
                ReloadBans();
                timerBans.Enabled = false;
            }
        }

        private void reloadToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ReloadExceptions();
        } 
    }
}
