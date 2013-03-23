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
using System.IO;
using System.Windows.Forms;

namespace Client
{
    public partial class SettingsEd : Form
    {
        bool Moving = false;
        public SettingsEd()
        {
            InitializeComponent();
        }

        private void SettingsEd_Load(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(Core.ConfigFile))
                {
                    return;
                }
                richTextBox1.Text = File.ReadAllText(Core.ConfigFile);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ValidateXml())
            {
                File.WriteAllText(Core.ConfigFile, richTextBox1.Text);
                Core.ConfigurationLoad();
                Close();
            }
        }

        public bool ValidateXml()
        {
            try
            {
                System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                document.LoadXml(richTextBox1.Text);
                return true;
            }
            catch (Exception reason)
            {
                MessageBox.Show("Error found: " + reason.Message, "Invalid config");
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ValidateXml();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Moving = true;
                maskedTextBox1.Text = (richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) + 1).ToString();
                maskedTextBox2.Text = (richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexOfCurrentLine() + 1).ToString();
                Moving = false;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void cursor()
        {
            if (Moving != false)
            {
                return;
            }
            try
            {
                int X = int.Parse(maskedTextBox2.Text) - 1;
                int Y = int.Parse(maskedTextBox1.Text) - 1;
                if (X < richTextBox1.Lines[Y].Length)
                {
                    if (X >= 0)
                    {
                        int number = 0;
                        int line = 1;
                        while ((line - 1) < Y)
                        {
                            number = richTextBox1.Lines[line - 1].Length + number + 1;
                            line++;
                        }
                        richTextBox1.SelectionStart = number + X;
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void maskedTextBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                richTextBox1.Focus();
            }
        }

        private void maskedTextBox2_MaskInputRejected(object sender, EventArgs e)
        {
            cursor();
        }

        private void maskedTextBox1_MaskInputRejected(object sender, EventArgs e)
        {
            cursor();
        }
    }
}
