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
        public TextBox()
        {
            InitializeComponent();
        }

        public void resize(object sender, EventArgs e)
        {
            richTextBox1.Height = this.Height - 2;
            richTextBox1.Width = this.Width -2;
        }

        private void _Enter(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals('\r'))
            {
                Core._Main._Scrollback.Last = this;
                richTextBox1.Text = richTextBox1.Text.Replace("\n", "");
                history.Add(richTextBox1.Text);
                Parser.parse(richTextBox1.Text);
                richTextBox1.Text = "";
            }
        }

        public void resize()
        {
            richTextBox1.Height = this.Height - 2;
            richTextBox1.Width = this.Width - 2;
        }

        private void TextBox_Load(object sender, EventArgs e)
        {
            history = new List<string>();
            resize();
        }
    }
}
