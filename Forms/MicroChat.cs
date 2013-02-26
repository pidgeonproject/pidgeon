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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class MicroChat : Form
    {
        public static MicroChat mc;
        public MicroChat()
        {
            InitializeComponent();
            this.TopMost = true;
            scrollback_mc.owner = null;
            scrollback_mc.Create();
            scrollback_mc.contextMenuStrip1.Items.Clear();
            scrollback_mc.contextMenuStrip1.Items.Add(scrollback_mc.scrollToolStripMenuItem);
            scrollback_mc.contextMenuStrip1.Items.Add(transparencyToolStripMenuItem);
        }

        public void Unload(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.8;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.6;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.4;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.2;
        }

        private void scrollToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MicroChat_Load(object sender, EventArgs e)
        {

        }
    }
}
