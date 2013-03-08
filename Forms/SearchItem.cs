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
    public partial class SearchItem : Form
    {
        public bool Direction = false;

        public SearchItem()
        {
            InitializeComponent();
        }

        public void SearchItem_Shut(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                Hide();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void SearchItem_Keys(object sender, KeyEventArgs keys)
        {
            try
            {
                if (keys.KeyCode == Keys.Enter)
                {
                    SearchRun(Direction);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void SearchItem_Load(object sender, EventArgs e)
        {
            try
            {
                messages.Localize(this);
                Top = Core._Main.Top + Core._Main.Height - 200;
                Left = Core._Main.Left + 200;
                textBox1.Focus();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void SearchRun(bool tp)
        {
            Direction = tp;
            Scrollback text = null;
            if (Core._Main.Chat == null || Core._Main.Chat.scrollback == null)
            {
                return;
            }
            else
            {
                text = Core._Main.Chat.scrollback;
            }
            if (!text.simple)
            {
                if (checkBox1.Checked)
                {
                    text.RT.SearchDown(new System.Text.RegularExpressions.Regex(textBox1.Text));
                }
                else
                {
                    text.RT.SearchDown(textBox1.Text);
                }
            }
            else
            {
                if (tp)
                {
                    // we need to scroll down from current position
                    string Data = text.simpleview.Text;
                    int offset = 0;
                    if (text.simpleview.SelectedText != "")
                    {
                        offset = text.simpleview.SelectionStart + text.simpleview.SelectionLength;
                        Data = text.simpleview.Text.Substring(offset);
                    }
                    if (Data.Contains(textBox1.Text))
                    {
                        text.simpleview.Select(offset + Data.IndexOf(textBox1.Text), textBox1.Text.Length);
                        text.simpleview.ScrollToCaret();
                        textBox1.BackColor = Color.LightSeaGreen;
                        text.simpleview.Focus();
                        return;
                    }
                    textBox1.BackColor = Color.PaleVioletRed;
                }
                else
                {
                    // we need to scroll up from current position
                    string Data = text.simpleview.Text;
                    if (text.simpleview.SelectedText != "")
                    {
                        Data = text.simpleview.Text.Substring(0, text.simpleview.SelectionStart);
                    }
                    if (Data.Contains(textBox1.Text))
                    {
                        text.simpleview.Select(Data.LastIndexOf(textBox1.Text), textBox1.Text.Length);
                        text.simpleview.ScrollToCaret();
                        textBox1.BackColor = Color.LightSeaGreen;
                        text.simpleview.Focus();
                        return;
                    }
                    textBox1.BackColor = Color.PaleVioletRed;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SearchRun(true);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SearchRun(false);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
        }
    }
}
