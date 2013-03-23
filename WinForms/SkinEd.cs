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
    public partial class SkinEd : Form
    {
        private Skin Current = null;
        public SkinEd()
        {
            InitializeComponent();
        }

        private void SkinEd_Load(object sender, EventArgs e)
        {
            try
            {
                groupBox1.Text = "Preview:";
                groupBox2.Text = "Parameters:";
                Current = Configuration.CurrentSkin;
                button2.Focus();
                foreach (Skin skin in Configuration.SL)
                {
                    comboBox1.Items.Add(skin.name);
                }
                comboBox1.SelectedText = Configuration.CurrentSkin.name;
                Open(Configuration.CurrentSkin.name);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void _Close()
        {
            this.Close();
        }

        private bool Save()
        {
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Save();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _Close();
        }

        public void Open(string name)
        { 
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Save())
                {
                    _Close();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Open(comboBox1.SelectedText);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
