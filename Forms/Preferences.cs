﻿/***************************************************************************
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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Preferences : Form
    {
        public Preferences()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prepare the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Preferences_Load(object sender, EventArgs e)
        {
            bSave.Text = messages.get("global-ok", Core.SelectedLanguage);
            bCancel.Text = messages.get("global-cancel", Core.SelectedLanguage);
            lnick.Text = messages.get("preferences-nick", Core.SelectedLanguage);
            lident.Text = messages.get("preferences-ident", Core.SelectedLanguage);
            lname.Text = messages.get("preferences-name", Core.SelectedLanguage);
            lquit.Text = messages.get("preferences-quit", Core.SelectedLanguage);
            textBox1.Text = Configuration.nick;
            textBox2.Text = Configuration.quit;
            textBox3.Text = Configuration.ident;
            textBox4.Text = Configuration.user;
            checkBox3.Checked = Configuration.logs_xml;
            checkBox5.Checked = Configuration.flood_prot;
            checkBox1.Checked = Configuration.logs_txt;
            checkBox2.Checked = Configuration.logs_html;
            checkBox7.Checked = Configuration.notice_prot;
            checkBox4.Checked = Configuration.DisplayCtcp;
            checkBox6.Checked = Configuration.ctcp_prot;
            textBox5.Text = Configuration.logs_dir;
            textBox6.Text = Configuration.logs_name;
            foreach (Network.Highlighter highlight in Configuration.HighlighterList)
            {
                ListViewItem item = new ListViewItem();
                item.Text = highlight.text;
                item.SubItems.Add(highlight.enabled.ToString().ToLower());
                item.SubItems.Add(highlight.simple.ToString());
                list.Items.Add(item);
            }
            listView1.Items.Add(new ListViewItem("IRC"));
            listView1.Items.Add(new ListViewItem("System"));
            listView1.Items.Add(new ListViewItem("Logs"));
            listView1.Items.Add(new ListViewItem("Protections"));
            listView1.Items.Add(new ListViewItem("Network"));
            listView1.Items.Add(new ListViewItem("Highlighting"));
        }

        private void lquit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                lock (Configuration.HighlighterList)
                {
                    Configuration.HighlighterList = new List<Network.Highlighter>();
                    foreach (ListViewItem curr in list.Items)
                    {
                        Network.Highlighter hl = new Network.Highlighter();
                        hl.enabled = bool.Parse(curr.SubItems[1].Text);
                        hl.text = curr.Text;
                        hl.simple = bool.Parse(curr.SubItems[2].Text);
                        Configuration.HighlighterList.Add(hl);
                    }
                }
                Configuration.nick = textBox1.Text;
                Configuration.quit = textBox2.Text;
                Configuration.ident = textBox3.Text;
                Configuration.user = textBox4.Text;
                Configuration.logs_xml = checkBox3.Checked;
                Configuration.flood_prot = checkBox5.Checked;
                Configuration.logs_txt = checkBox1.Checked;
                Configuration.logs_html = checkBox2.Checked;
                Configuration.notice_prot = checkBox7.Checked;
                Configuration.DisplayCtcp = checkBox4.Checked;
                Configuration.ctcp_prot = checkBox6.Checked;
                Configuration.logs_dir = textBox5.Text;
                Configuration.logs_name = textBox6.Text;
                Core.ConfigSave();
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
            Close();
        }

        /// <summary>
        /// User wants to discard changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1) return;
            switch (listView1.SelectedItems[0].Index)
            { 
                case 0:
                    gro1.BringToFront();
                    break;
                case 1:
                    gro2.BringToFront();
                    break;
                case 2:
                    gro3.BringToFront();
                    break;
                case 3:
                    gro4.BringToFront();
                    break;
                case 4:
                    gro5.BringToFront();
                    break;
                case 5:
                    gro6.BringToFront();
                    break;
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count > 0)
            { 
                foreach(ListViewItem curr in list.SelectedItems)
                {
                    list.Items.Remove(curr);
                }
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            item.Text = "$nick!$ident@$host.*$name";
            item.SubItems.Add("true");
            item.SubItems.Add("true");
            list.Items.Add(item);
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem curr in list.SelectedItems)
            {
                curr.SubItems[1].Text = "true";
            }
        }

        private void simpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem curr in list.SelectedItems)
            {
                if (curr.SubItems[2].Text == "false")
                {
                    curr.SubItems[2].Text = "true";
                }
                else
                {
                    curr.SubItems[2].Text = "false";
                }
            }
            
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem curr in list.SelectedItems)
            {
                curr.SubItems[1].Text = "false";
            }
        }
    }
}
