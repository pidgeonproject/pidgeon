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
			this.DefaultWidth = 739;
			this.DefaultHeight = 58;
		}
		
		private void hide2(object sender, DeleteEventArgs e)
        {
            e.RetVal = true;
            Hide();
        }
		
		public SearchItem () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
		
		public void setFocus()
		{
			entry1.GrabFocus();
		}
	}
}

