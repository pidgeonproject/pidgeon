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
using Gtk;

namespace Client.Graphics
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class TextBox : Gtk.Bin
	{
		public List<string> history = null;
        public int position = 0;
        public string prevtext = "";
        public string original = "";
        public Window parent = null;
        public bool restore = false;

		public Gtk.TextView richTextBox1
		{
			get
			{
				return richTextBox;
			}
		}
		
		public TextBox ()
		{
			this.Build ();
            history = new List<string>();
            richTextBox.Buffer.Changed += new EventHandler(richTextBox1_TextChanged);
            richTextBox.KeyPressEvent += new KeyPressEventHandler(_Enter);
		}

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (restore)
                {
                    int selection = richTextBox1.Buffer.CursorPosition;
                    if (richTextBox1.Buffer.Text.Length != prevtext.Length)
                    {
                        selection = selection - (richTextBox1.Buffer.Text.Length - prevtext.Length);
                    }
                    if (selection < 0)
                    {
                        selection = 0;
                    }
                    richTextBox1.Buffer.Text = prevtext;
                    TextIter iter = new TextIter();
                    iter.Offset = selection;
                    richTextBox1.Buffer.PlaceCursor(iter);
                    return;
                }
                prevtext = richTextBox1.Buffer.Text;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        [GLib.ConnectBefore]
        private void _Enter(object sender, KeyPressEventArgs e)
        {
            try
            {
                //if (Main.ShortcutHandle(sender, e))
                //{
                //    e.SuppressKeyPress = true;
                //    return;
                //}

                if (e.Event.State == Gdk.ModifierType.ShiftMask)
                {
                    return;
                }

                if (e.Event.State == Gdk.ModifierType.ControlMask)
                {
                    switch (e.Event.Key)
                    {
                        case Gdk.Key.KP_Enter:
                        case Gdk.Key.ISO_Enter:
                        case Gdk.Key.Up:
                        case Gdk.Key.Down:
                            return;
                    }
                }

                // enter
                if (e.Event.KeyValue == 65293)
                { 
                    List<string> input = new List<string>();
                    if (richTextBox1.Buffer.Text.Contains("\n"))
                    {
                        input.AddRange(richTextBox1.Buffer.Text.Split('\n'));
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
                        richTextBox1.Buffer.Text = "";
                        position = history.Count;
                        e.RetVal = true;
                        return;
                    }

                    richTextBox1.Buffer.Text = richTextBox1.Buffer.Text.Replace("\n", "");

                    if (richTextBox1.Buffer.Text != "")
                    {
                        Parser.parse(richTextBox1.Buffer.Text);
                        history.Add(richTextBox1.Buffer.Text);
                    }
                    original = "";
                    richTextBox1.Buffer.Text = "";
                    position = history.Count;
                    e.RetVal = true;
                    return;
                }

                if (!(e.Event.Key == Gdk.Key.Tab))
                { restore = false; }
                switch (e.Event.Key)
                {
                    case Gdk.Key.Down:
                        if (position == history.Count)
                        {
                            return;
                        }
                        int max = position + 1;
                        if (history.Count <= max)
                        {
                            position = history.Count;
                            richTextBox1.Buffer.Text = original.Replace("\n", "");
                            return;
                        }
                        position++;
                        if (position >= history.Count)
                        {
                            position = history.Count - 1;
                        }
                        richTextBox1.Buffer.Text = history[position];
                        break;
                    case Gdk.Key.b | Gdk.Key.Control_L:
                    case Gdk.Key.B | Gdk.Key.Control_R:
                            richTextBox1.Buffer.Text += (((char)002).ToString());
                            //e.SuppressKeyPress = true;
                            
                            //if (richTextBox1.SelectionFont.Bold)
                            {
                            //    richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, FontStyle.Regular);
                            //    return;
                            }
                            //richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, FontStyle.Bold);
                        return;
                    //case Keys.K:
                    //    if (e.Control)
                    //    {
                    //        richTextBox1.AppendText(((char)003).ToString());
                    //        e.SuppressKeyPress = true;
                    //    }
                        break;
                    case Gdk.Key.Up:
                        if (position < 1)
                        {
                            return;
                        }
                        if (history.Count == position)
                        {
                            original = richTextBox1.Buffer.Text;
                        }
                        position = position - 1;
                        richTextBox1.Buffer.Text = history[position];
                        return;
                    /*
                case Keys.Tab:
                    int caret = richTextBox1.SelectionStart;
                    string data = richTextBox1.Text;
                    Hooks._Scrollback.TextTab(ref restore, ref data, ref prevtext, ref caret);
                    richTextBox1.Text = data;
                    restore = true;
                    richTextBox1.Text = prevtext;
                    richTextBox1.SelectionStart = caret;
                    break; */
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

		/// <summary>
		/// Deprecated
		/// </summary>
		[Obsolete]
		public void Init()
		{

		}

		public void setFocus()
		{
			this.GrabFocus ();
		}
	}
}

