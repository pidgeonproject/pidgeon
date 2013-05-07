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
    public partial class ShortcutBox : Form
    {
        public Preferences config;
        public ShortcutBox()
        {
            InitializeComponent();
        }

        private void ShortcutBox_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("a");
            comboBox1.Items.Add("b");
            comboBox1.Items.Add("c");
            comboBox1.Items.Add("d");
            comboBox1.Items.Add("e");
            comboBox1.Items.Add("f");
            comboBox1.Items.Add("g");
            comboBox1.Items.Add("h");
            comboBox1.Items.Add("i");
            comboBox1.Items.Add("j");
            comboBox1.Items.Add("k");
            comboBox1.Items.Add("l");
            comboBox1.Items.Add("m");
            comboBox1.Items.Add("n");
            comboBox1.Items.Add("o");
            comboBox1.Items.Add("p");
            comboBox1.Items.Add("q");
            comboBox1.Items.Add("r");
            comboBox1.Items.Add("s");
            comboBox1.Items.Add("t");
            comboBox1.Items.Add("u");
            comboBox1.Items.Add("v");
            comboBox1.Items.Add("w");
            comboBox1.Items.Add("x");
            comboBox1.Items.Add("y");
            comboBox1.Items.Add("z");
            comboBox1.Items.Add("f1");
            comboBox1.Items.Add("f2");
            comboBox1.Items.Add("f3");
            comboBox1.Items.Add("f4");
            comboBox1.Items.Add("f5");
            comboBox1.Items.Add("f6");
            comboBox1.Items.Add("f7");
            comboBox1.Items.Add("f8");
            comboBox1.Items.Add("f9");

            comboBox1.Items.Add("f10");
            comboBox1.Items.Add("f11");
            comboBox1.SelectedIndex = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (config == null)
            {
                Close();
            }
            Configuration.ShortcutKeylist.Add(new Core.Shortcut(Core.parseKey(comboBox1.Text), checkBox1.Checked, checkBox2.Checked, false, textBox1.Text));
            config.redrawS();
            Close();
        }
    }
}
