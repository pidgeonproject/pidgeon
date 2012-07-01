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
    public partial class TextBox : UserControl
    {
        public List<string> history;
        public int position = 0;
        public string original = "";
        public TextBox()
        {
            InitializeComponent();
        }

        public void resize(object sender, EventArgs e)
        {
            richTextBox1.Height = this.Height - 2;
            richTextBox1.Width = this.Width -2;
        }


        private void _Enter(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (position == history.Count)
                    {
                        return;
                    }
                    int max = position + 1;
                    if (history.Count <= max)
                    {
                        position = history.Count;
                        richTextBox1.Text = original.Replace("\n", "");
                        return;
                    }
                    position++;
                    if (position >= history.Count)
                    {
                        position = history.Count - 1;
                    }
                    richTextBox1.Text = history[position];
                    break;
                case Keys.Up:
                    if (position < 1)
                    {
                        return;
                    }
                    if (history.Count == position)
                    {
                        original = richTextBox1.Text;
                    }
                    position = position - 1;
                    richTextBox1.Text = history[position];
                    return;
                case Keys.Tab:
                    int caret = richTextBox1.SelectionStart;
                    //string current = richTextBox1.Text.Substring(0, richTextBox1
                    break;
                case Keys.Enter:        
                    Core._Main.Chat.scrollback.Last = this;
                    richTextBox1.Text = richTextBox1.Text.Replace("\n", "");
                    history.Add(richTextBox1.Text);
                    Parser.parse(richTextBox1.Text);
                    original = "";
                    richTextBox1.Text = "";
                    position = history.Count;
                    break;
            }
        }

        public void resize()
        {
            richTextBox1.Height = this.Height - 2;
            richTextBox1.Width = this.Width - 2;
        }

        private void TextBox_Load(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Font = new Font(Configuration.CurrentSkin.localfont, float.Parse(Configuration.CurrentSkin.fontsize.ToString()) * 4);
                richTextBox1.ForeColor = Configuration.CurrentSkin.fontcolor;
                richTextBox1.BackColor = Configuration.CurrentSkin.backgroundcolor;
                resize();
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }
    }
}
