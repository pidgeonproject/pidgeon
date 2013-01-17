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
            text.Clear();
            textBox1.Clear();
            modified = true;
        }

        public bool insert(string server, string data)
        {
            modified = true;
            if (!Configuration.NetworkSniff)
            {
                return true;
            }
            text.Add(server + " " + data);
            return true;
        }

        private void refresh_Tick(object sender, EventArgs e)
        {
            if (Visible && modified)
            {
                modified = false;
                textBox1.Lines = text.ToArray<string>();
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (text)
            {
                text.Clear();
            }
            modified = true;
        }

        private void scrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scrollToolStripMenuItem.Checked = !scrollToolStripMenuItem.Checked;
            refresh.Enabled = scrollToolStripMenuItem.Checked;
        }
    }
}
