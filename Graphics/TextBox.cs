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
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        public global::Gtk.TextView richTextBox;

        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Graphics.TextBox
            global::Stetic.BinContainer.Attach(this);
            this.Name = "Client.Graphics.TextBox";
            // Container child Client.Graphics.TextBox.Gtk.Container+ContainerChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.richTextBox = new global::Gtk.TextView();
            this.richTextBox.CanFocus = true;
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.AcceptsTab = true;
            this.GtkScrolledWindow.Add(this.richTextBox);
            this.Add(this.GtkScrolledWindow);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Hide();
        }

        public Gtk.TextView richTextBox1
        {
            get
            {
                return richTextBox;
            }
        }

		public void InitStyle()
		{
			richTextBox.ModifyBase (StateType.Normal, Core.fromColor(Configuration.CurrentSkin.backgroundcolor));
			richTextBox.ModifyText(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.colordefault));
		}
		
        public TextBox()
        {
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (false && restore)
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
                    TextIter iter = richTextBox.Buffer.GetIterAtOffset(selection);
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
                if (Forms.Main.ShortcutHandle(sender, e))
                {
                    e.RetVal = true;
                    return;
                }

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
                        e.RetVal = true;
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
                        e.RetVal = true;
                        return;
                    case Gdk.Key.Tab:
                        int caret = richTextBox1.Buffer.CursorPosition;
                        string data = richTextBox1.Buffer.Text;
                        Hooks._Scrollback.TextTab(ref restore, ref data, ref prevtext, ref caret);
                        richTextBox1.Buffer.Text = data;
                        restore = true;
                        richTextBox1.Buffer.Text = prevtext;
                        //richTextBox1.Buffer.CursorPosition = caret;
                        TextIter position2 = richTextBox.Buffer.GetIterAtOffset(caret);
                        //position2. = caret;
                        richTextBox.Buffer.PlaceCursor(position2);
                        e.RetVal = true;
                        break;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void Init()
        {
            this.Build();
			this.InitStyle();
            richTextBox.Buffer.Changed += new EventHandler(richTextBox1_TextChanged);
            richTextBox.KeyPressEvent += new KeyPressEventHandler(_Enter);
            if (history == null)
            {
                history = new List<string>();
            }
        }

        public void setFocus()
        {
            this.GrabFocus();
        }
    }
}

