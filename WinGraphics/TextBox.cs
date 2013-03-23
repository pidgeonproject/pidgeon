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
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class TextBox : UserControl
    {
        public List<string> history = null;
        public int position = 0;
        public string prevtext = "";
        public string original = "";
        public Window parent = null;
        public bool restore = false;

        public void Init()
        {
            InitializeComponent();
        }

        private void Wheeled(object sender, MouseEventArgs e)
        {
            try
            {
                if (parent.scrollback != null)
                {
                    parent.scrollback.RT.Wheeled(sender, e);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void _Enter(object sender, KeyEventArgs e)
        {
            try
            {
                if (Main.ShortcutHandle(sender, e))
                {
                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.Shift)
                {
                    return;
                }

                if (e.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                        case Keys.Down:
                        case Keys.Up:
                            return;
                    }
                }

                if (!(e.KeyCode == Keys.Tab))
                { restore = false; }
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
                    case Keys.B:
                        if (e.Control)
                        {
                            richTextBox1.AppendText(((char)002).ToString());
                            e.SuppressKeyPress = true;
                            if (richTextBox1.SelectionFont.Bold)
                            {
                                richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, FontStyle.Regular);
                                return;
                            }
                            richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, FontStyle.Bold);
                        }
                        return;
                    case Keys.K:
                        if (e.Control)
                        {
                            richTextBox1.AppendText(((char)003).ToString());
                            e.SuppressKeyPress = true;
                        }
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
                        string data = richTextBox1.Text;
                        Hooks._Scrollback.TextTab(ref restore, ref data, ref prevtext, ref caret);
                        richTextBox1.Text = data;
                        restore = true;
                        richTextBox1.Text = prevtext;
                        richTextBox1.SelectionStart = caret;
                        break;
                    case Keys.Enter:
                        List<string> input = new List<string>();
                        if (richTextBox1.Text.Contains("\n"))
                        {
                            input.AddRange(richTextBox1.Text.Split('\n'));
                            foreach (var line in input)
                            {
                                Parser.parse(line);
                                if (line != "")
                                {
                                    lock (history)
                                    {
                                        while (history.Count > Configuration.Window.history)
                                        {
                                            history.RemoveAt(0);
                                        }
                                        history.Add(line);
                                    }
                                }
                            }
                            original = "";
                            richTextBox1.Text = "";
                            position = history.Count;
                            e.SuppressKeyPress = true;
                            return;
                        }

                        richTextBox1.Text = richTextBox1.Text.Replace("\n", "");

                        if (richTextBox1.Text != "")
                        {
                            Parser.parse(richTextBox1.Text);
                            history.Add(richTextBox1.Text);
                        }
                        original = "";
                        richTextBox1.Text = "";
                        position = history.Count;
                        e.SuppressKeyPress = true;
                        return;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
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
                richTextBox1.Font = new Font(Configuration.CurrentSkin.localfont, Configuration.CurrentSkin.fontsize);
                richTextBox1.ForeColor = Configuration.CurrentSkin.fontcolor;
                richTextBox1.BackColor = Configuration.CurrentSkin.backgroundcolor;
                resize();
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (restore)
                {
                    int selection = richTextBox1.SelectionStart;
                    if (richTextBox1.Text.Length != prevtext.Length)
                    {
                        selection = selection - (richTextBox1.Text.Length - prevtext.Length);
                    }
                    if (selection < 0)
                    {
                        selection = 0;
                    }
                    richTextBox1.Text = prevtext;

                    richTextBox1.SelectionStart = selection;
                    return;
                }
                prevtext = richTextBox1.Text;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
