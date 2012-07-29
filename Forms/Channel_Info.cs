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

                if (channel.Bl != null)
                {
                    lock (channel.Bl)
                    {
                        foreach (SimpleBan sb in channel.Bl)
                        { 
                            ListViewItem li = new ListViewItem(sb._Target);
                            li.SubItems.Add(sb.Time);
                            li.SubItems.Add(sb._User);
                            
                            listView3.Items.Add(li);
                        }
                    }
                }

                lock (channel._mode)
                {
                    foreach (char item in channel._Network.CModes)
                    {
                        string de = "  unknown mode. Refer to ircd manual (/raw help)";
                        cm.Add(item);
                        switch (item.ToString())
                        {
                            case "n":
                                de = "  no /knock is allowed on channel";
                                break;
                            case "r":
                                de = "  registered channel";
                                break;
                            case "m":
                                de = "  talking is restricted";
                                break;
                            case "i":
                                de = "  users need to be invited to join";
                                break;
                            case "s":
                                de = "  channel is secret (doesn't appear on list)";
                                break;
                            case "p":
                                de = "  channel is private";
                                break;
                            case "A":
                                de = "  admins only";
                                break;
                            case "O":
                                de = "  opers chan";
                                break;
                            case "t":
                                de = "  topic changes can be done only by operators";
                                break;

                        }
                        checkedList.Items.Add(item.ToString() + de, channel._mode._Mode.Contains(item.ToString()));
                        textBox1.Text = channel.Topic;
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
                    channel._Network._protocol.Transfer("TOPIC " + channel.Name + " :" + textBox1.Text);
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
                    channel._Network._protocol.Transfer("MODE " + channel.Name + " " + cset + uset);
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
    }
}
