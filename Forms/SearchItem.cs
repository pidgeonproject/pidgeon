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
using Gtk;

namespace Client.Forms
{
	public partial class SearchItem : Gtk.Window
	{	
		private global::Gtk.HBox hbox1;
		private global::Gtk.Entry entry1;
		private global::Gtk.CheckButton checkbutton1;
		private global::Gtk.Button button1;
		private global::Gtk.Button button2;
		public bool Direction = true;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MainWindow
			this.Name = "MainWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
			this.TypeHint = ((global::Gdk.WindowTypeHint)(5));
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child MainWindow.Gtk.Container+ContainerChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.entry1 = new global::Gtk.Entry ();
			this.entry1.CanFocus = true;
			this.entry1.Name = "entry1";
			this.entry1.IsEditable = true;
			this.entry1.InvisibleChar = '•';
			this.hbox1.Add (this.entry1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.entry1]));
			w1.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.checkbutton1 = new global::Gtk.CheckButton ();
			this.checkbutton1.CanFocus = true;
			this.checkbutton1.Name = "checkbutton1";
			this.checkbutton1.Label = global::Mono.Unix.Catalog.GetString ("Regular expression");
			this.checkbutton1.DrawIndicator = true;
			this.checkbutton1.UseUnderline = true;
			this.hbox1.Add (this.checkbutton1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.checkbutton1]));
			w2.Position = 1;
			// Container child hbox1.Gtk.Box+BoxChild
			this.button1 = new global::Gtk.Button ();
			this.button1.CanFocus = true;
			this.button1.Name = "button1";
			this.button1.UseUnderline = true;
			this.button1.Label = global::Mono.Unix.Catalog.GetString ("Up");
			this.hbox1.Add (this.button1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.button1]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.button2 = new global::Gtk.Button ();
			this.button2.CanFocus = true;
			this.button2.Name = "button2";
			this.button1.Clicked += new EventHandler(button1_Click);
			this.button2.Clicked += new EventHandler(button2_Click);
			this.button2.UseUnderline = true;
			this.button2.Label = global::Mono.Unix.Catalog.GetString ("Down");
			this.hbox1.Add (this.button2);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.button2]));
			w4.Position = 3;
			w4.Expand = false;
			w4.Fill = false;
			this.DeleteEvent += new DeleteEventHandler(hide2);
			this.Add (this.hbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			Move (80, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 200);
			this.DefaultWidth = 800;
			this.DefaultHeight = 20;
		}
		
		private void hide2(object sender, DeleteEventArgs e)
        {
            e.RetVal = true;
            Hide();
        }
		
		public SearchItem () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			//Move(Core._Main.Left + 200, Core._Main.Top + Core._Main.Height - 200);
			messages.Localize(this);
		}
		
		public void SearchItem_Keys(object sender, Gtk.KeyPressEventArgs keys)
        {
            try
            {
                if (keys.Event.KeyValue == 65293)
                {
                    SearchRun(Direction);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void SearchRun(bool tp)
        {
            /*Direction = tp;
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
                    if (tp)
                    {
                        text.RT.SearchDown(new System.Text.RegularExpressions.Regex(textBox1.Text));
                    }
                    else
                    {
                        text.RT.SearchUp(new System.Text.RegularExpressions.Regex(textBox1.Text));
                    }
                }
                else
                {
                    if (tp)
                    {
                        text.RT.SearchDown(textBox1.Text);
                    }
                    else
                    {
                        text.RT.SearchUp(new System.Text.RegularExpressions.Regex(textBox1.Text));
                    }
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
            }*/
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
            //textBox1.BackColor = Color.White;
        }
		
		public void setFocus()
		{
			entry1.GrabFocus();
		}
	}
}

