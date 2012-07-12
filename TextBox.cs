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
        public static List<TextBox> _control = new List<TextBox>(); 
        public List<string> history;
        public int position = 0;
        public string prevtext = "";
        public string original = "";
        public bool restore = false;

        public TextBox()
        {
            lock (_control)
            {
                _control.Add(this);
            }
            InitializeComponent();
        }

        public void resize(object sender, EventArgs e)
        {
            richTextBox1.Height = this.Height - 2;
            richTextBox1.Width = this.Width - 2;
        }


        private void _Enter(object sender, KeyEventArgs e)
        {
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
                    restore = true;
                    richTextBox1.Text = prevtext;
                    richTextBox1.SelectionStart = caret;
                    if (caret < 1) return;
                    if (richTextBox1.Text == "") return;
                    int x = caret - 1;
                    if (x > richTextBox1.Text.Length)
                    { x = richTextBox1.Text.Length - 2; }
                    while (x >= 0)
                    {
                        if (richTextBox1.Text[x] == '\n' || richTextBox1.Text[x] == ' ')
                        {
                            x++;
                            break;
                        }
                        x--;
                    }
                    if (x < 0)
                    { x = 0; }
                    string text = richTextBox1.Text.Substring(x);
                    if (text.Contains(" "))
                    {
                        text = text.Substring(0, text.IndexOf(" "));
                    }

                    // check if it's a command :)

                    List<string> commands = new List<string> { "server", "nick", "pidgeon.rehash", "connect" };

                    if (Core.network != null && Core.network.Connected)
                    {
                        commands.AddRange( new List<string> { "join", "part", "squit", "quit", "query", "me", "msg", "mode", "oper", "who",
                                                        "whois", "whowas", "help", "wall", "list", "kill", "kline", "zline", "away", "gline",
                                                        "stats", "nickserv", "chanserv" });
                    }

                    if (Core._Main.Chat._Protocol != null)
                    {
                        if (Core._Main.Chat._Protocol.type == 3)
                        {
                            commands.AddRange(new List<string> { "service.gnick", "service.gident", "service.quit" });
                        }
                    }

                    if (text.StartsWith(Configuration.CommandPrefix))
                    {
                        List<string> Results = new List<string>();
                        string Resd = "";
                        foreach (var item in commands)
                        {
                            if (item.StartsWith(text.Substring(1).ToLower()))
                            {
                                Resd += item + ", ";
                                Results.Add(item);
                            }
                        }
                        if (Results.Count > 1)
                        {
                            Core._Main.Chat.scrollback.InsertText(messages.get("autocomplete-result", Core.SelectedLanguage, new List<string> { Resd } ), Scrollback.MessageStyle.System);
                            string part = "";
                            int curr = 0;
                            bool match = true;
                            while (match)
                            {
                                char diff = ' ';
                                foreach (var item in Results)
                                {
                                    if (item.Length <= curr || diff != item[curr])
                                    {
                                        match = false;
                                        break;
                                    }
                                    if (diff == ' ')
                                    {
                                        diff = item[curr];
                                        continue;
                                    }
                                }
                                if (match)
                                {
                                    curr = curr + 1;
                                    part += diff.ToString();
                                }
                            }
                            string result = richTextBox1.Text;
                            result = result.Substring(0, x + 1);
                            result = result + part + richTextBox1.Text.Substring(x + text.Length);
                            richTextBox1.Text = result;
                            richTextBox1.SelectionStart = result.Length;
                            prevtext = result;
                            return;
                        }
                        if (Results.Count == 1)
                        {
                            string result = richTextBox1.Text;
                            result = result.Substring(0, x + 1);
                            result = result + Results[0] + " " + richTextBox1.Text.Substring(x + text.Length);
                            richTextBox1.Text = result;
                            richTextBox1.SelectionStart = result.Length;
                            prevtext = result;
                            return;
                        }
                    }

                    if (Core.network == null)
                        return;

                    if (text.StartsWith(Core.network.channel_prefix))
                    {
                    }
                    // check if it's a nick

                    List<string> Results2 = new List<string>();
                    string Resd2 = "";
                    if (Core.network.RenderedChannel == null) { return; }
                    lock (Core.network.RenderedChannel.UserList)
                    {
                        foreach (var item in Core.network.RenderedChannel.UserList)
                        {
                            if ((item.Nick.ToUpper()).StartsWith(text.ToUpper()))
                            {
                                Resd2 += item.Nick + ", ";
                                Results2.Add(item.Nick);
                            }
                        }
                    }
                    if (Results2.Count == 1)
                    {
                        string result = richTextBox1.Text;
                            result = result.Substring(0, x);
                        result = result + Results2[0] + richTextBox1.Text.Substring(x + text.Length);
                        richTextBox1.Text = result;

                        richTextBox1.SelectionStart = result.Length;

                        prevtext = result;
                        return;
                    }

                    if (Results2.Count > 1)
                    {
                        Core._Main.Chat.scrollback.InsertText(messages.get("autocomplete-result", Core.SelectedLanguage, new List<string> { Resd2 }), Scrollback.MessageStyle.System);
                        string part = "";
                        int curr = 0;
                        char orig = ' ';
                        bool match = true;
                        while (match)
                        {
                            char diff = ' ';
                            foreach (var item in Results2)
                            {
                                string value = item.ToLower();
                                if (diff == ' ')
                                {
                                    orig = item[curr];
                                    diff = value[curr];
                                    continue;
                                }
                                if (item.Length <= curr || diff != value[curr])
                                {
                                    match = false;
                                    break;
                                }
                            }
                            if (match)
                            {
                                curr = curr + 1;
                                part += orig.ToString();
                            }
                        }
                        string result = richTextBox1.Text;
                        result = result.Substring(0, x);
                        result = result + part + richTextBox1.Text.Substring(x + text.Length);
                        richTextBox1.Text = result;
                        richTextBox1.SelectionStart = result.Length;
                        prevtext = result;


                        return;
                    }

                    break;
                case Keys.Enter:
                    Core._Main.Chat.scrollback.Last = this;
                    List<string> input = new List<string>();
                    if (richTextBox1.Text.Contains("\n"))
                    {
                        input.AddRange(richTextBox1.Text.Split('\n'));
                        foreach (var line in input)
                        {
                            if (line != "")
                            {
                                Parser.parse(line);
                            }
                        }
                        original = "";
                        richTextBox1.Text = "";
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
                    return;
            }
        }

        public void resize()
        {
            richTextBox1.Height = this.Height - 2;
            richTextBox1.Width = this.Width - 2;
        }

        new public void Dispose()
        {
            lock (_control)
            {
                if (_control.Contains(this))
                {
                    _control.Remove(this);
                }
            }
            this.components.Dispose();
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

        private void richTextBox1_TextChanged(object sender, EventArgs e)
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
            //richTextBox1.Text = richTextBox1.Text.Replace("\t", ""
        }
    }
}
