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
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class TrafficScanner : Form
    {
        bool modified = true;
        List<string> text = new List<string>();
        public TrafficScanner()
        {
            InitializeComponent();
        }

        public void Stop(object O, FormClosingEventArgs L)
        {
            Visible = false;
            L.Cancel = true;
        }

        public void Clean()
        {
            lock (text)
            {
                text.Clear();
            }
            textBox1.Clear();
            modified = true;
        }

        public bool insert(string server, string data)
        {
            modified = true;
            if (!Configuration.Kernel.NetworkSniff)
            {
                return true;
            }
            lock (text)
            {
                text.Add(server + " " + data);
            }
            return true;
        }

        private void refresh_Tick(object sender, EventArgs e)
        {
            try
            {
                refresh.Enabled = false;
                if (Visible && modified)
                {
                    modified = false;
                    lock (text)
                    {
                        if (text.Count > 800)
                        {
                            if (MessageBox.Show("There are too many items in log, which means, that pidgeon may become unresponsive for several minutes if you continue, press yes to continue or no to abort", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                            {
                                refresh.Enabled = false;
                                scrollToolStripMenuItem.Checked = false;
                                return;
                            }
                        }
                        string xx = "";
                        foreach (string x in text)
                        {
                            xx += x + Environment.NewLine;
                        }
                        textBox1.AppendText(xx);
                        text.Clear();
                        textBox1.ScrollToCaret();
                    }
                }
                refresh.Enabled = true;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clean();
        }

        private void scrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scrollToolStripMenuItem.Checked = !scrollToolStripMenuItem.Checked;
            refresh.Enabled = scrollToolStripMenuItem.Checked;
        }
    }
}
