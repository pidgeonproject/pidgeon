//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;

namespace Client.Forms
{
    /// <summary>
    /// TrafficScanner
    /// </summary>
    public partial class TrafficScanner
    {
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TextView textview2;

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Forms.TrafficScanner
            this.Name = "Client.Forms.TrafficScanner";
            this.Title = "TrafficScanner";
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            // Container child Client.Forms.TrafficScanner.Gtk.Container+ContainerChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.textview2 = new global::Gtk.TextView();
            this.textview2.CanFocus = true;
            this.textview2.Name = "textview2";
            this.textview2.Editable = false;
            this.GtkScrolledWindow.Add(this.textview2);
            this.Add(this.GtkScrolledWindow);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 579;
            this.DefaultHeight = 395;
            this.Hide();
        }
    }
}
