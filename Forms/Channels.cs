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
	public partial class Channels : Gtk.Window
	{
        public Network network = null;
        private int channels = 0;
        //private ListViewColumnSorter lvwColumnSorter;
        public List<Network.ChannelData> channelData = new List<Network.ChannelData>();
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TreeView treeview8;

        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget MainWindow
            this.Name = "MainWindow";
            this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
            this.TypeHint = ((global::Gdk.WindowTypeHint)(6));
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            this.Decorated = false;
            // Container child MainWindow.Gtk.Container+ContainerChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeview8 = new global::Gtk.TreeView();
            this.treeview8.CanFocus = true;
            this.treeview8.Name = "treeview8";
            this.DestroyEvent += new DestroyEventHandler(destroy);
            this.GtkScrolledWindow.Add(this.treeview8);
            this.Add(this.GtkScrolledWindow);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 796;
            this.DefaultHeight = 511;
            this.Show();
        }

        public void destroy(object o, DestroyEventArgs e)
        {
            Hide();
            e.RetVal = true;
        }

        public Channels() : base(Gtk.WindowType.Toplevel)
        {
			this.Build ();
        }
	}
}

