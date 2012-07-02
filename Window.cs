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
    public partial class Window : UserControl
    {
        public bool Making = true;
        public string name;
        public bool writable;

        public Window()
        {
            InitializeComponent();
            listView.View = View.Details;
            listView.Columns.Add(messages.get("list", Core.SelectedLanguage));
            listView.BackColor = Configuration.CurrentSkin.backgroundcolor;
            listView.ForeColor = Configuration.CurrentSkin.fontcolor;
            textbox.history = new List<string>();
        }

        public void create()
        {
            scrollback.create();
        }

        public bool Redraw()
        {
                this.xContainer1.SplitterDistance = Configuration.x1;
                this.xContainer4.SplitterDistance = Configuration.x4;


            listView.Columns[0].Width = listView.Width - 8;
            return true;
        }


        public void Changed(object sender, SplitterEventArgs dt)
        {
            if (Making == false)
            {
                Configuration.x1 = xContainer1.SplitterDistance;
                Configuration.x4 = xContainer4.SplitterDistance;
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
                return true;
            return base.IsInputKey(keyData);
        }

        private void splitContainer1_Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void kickBanToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textbox_Load(object sender, EventArgs e)
        {
            banToolStripMenuItem.Text = messages.get("ban", Core.SelectedLanguage);
            modeToolStripMenuItem.Text = messages.get("mode", Core.SelectedLanguage);
            vToolStripMenuItem.Text = messages.get("give+v", Core.SelectedLanguage);
            hToolStripMenuItem.Text = messages.get("give+h", Core.SelectedLanguage);
            oToolStripMenuItem.Text = messages.get("give+o", Core.SelectedLanguage);
            aToolStripMenuItem.Text = messages.get("give+a", Core.SelectedLanguage);
            qToolStripMenuItem.Text = messages.get("give+q", Core.SelectedLanguage);
            vToolStripMenuItem1.Text = messages.get("give-v", Core.SelectedLanguage);
            hToolStripMenuItem1.Text = messages.get("give-h", Core.SelectedLanguage);
            oToolStripMenuItem1.Text = messages.get("give-o", Core.SelectedLanguage);
            aToolStripMenuItem1.Text = messages.get("give-a", Core.SelectedLanguage);
            qToolStripMenuItem1.Text = messages.get("give-q", Core.SelectedLanguage);
        }

        private void whoisToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void qToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void qToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void aToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void oToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void kickToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void hToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void vToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void oToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void hToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void vToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void banToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void krToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void kickrToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
