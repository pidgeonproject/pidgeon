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
using System.Data;
using Gtk;

namespace Client.Forms
{
    /// <summary>
    /// Micro chat
    /// </summary>
    public partial class MicroChat : GTK.PidgeonForm
    {
        /// <summary>
        /// Scrollback
        /// </summary>
        public Scrollback scrollback_mc = null;
        private Gtk.VBox vbox;
        private bool OnTop = true;

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            this.Name = "Client.Forms.MicroChat";
            this.Title = "Micro chat";
            this.Icon = global::Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.WindowPosition = Gtk.WindowPosition.Center;
            vbox = new VBox();
            scrollback_mc = new Scrollback();
            scrollback_mc.isMicro = true;
            scrollback_mc.Create();
            this.TypeHint = Gdk.WindowTypeHint.Normal;
            this.DefaultHeight = 420;
            this.DefaultWidth = 680;
            scrollback_mc.Events = ((global::Gdk.EventMask)(256));
            scrollback_mc.Name = "scrollback1";
            this.DeleteEvent += new DeleteEventHandler(Close);
            vbox.Add(scrollback_mc);
            this.Add(vbox);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Hide();
        }

        private void Close(object sender, DeleteEventArgs e)
        {
            e.RetVal = true;
            Hide();
        }
        
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.8;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.6;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.4;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Opacity = 0.2;
        }

        private void mx_Click(object sender, EventArgs e)
        {
            OnTop = !OnTop;
            this.KeepAbove = OnTop;
        }
        
        [GLib.ConnectBefore]
        private void CreateMenu_simple(object o, Gtk.PopulatePopupArgs e)
        {
            try
            {
                Gtk.SeparatorMenuItem separator1 = new Gtk.SeparatorMenuItem();
                separator1.Show();
                e.Menu.Append(separator1);
                Gtk.Menu m0 = new Gtk.Menu();
                Gtk.MenuItem m1 = new Gtk.MenuItem("Transparency");
                m1.Submenu = m0;
                m1.Show();
                e.Menu.Append(m1);
                Gtk.MenuItem m2 = new Gtk.MenuItem("0%");
                m2.Activated += new EventHandler(toolStripMenuItem6_Click);
                Gtk.MenuItem m3 = new Gtk.MenuItem("20%");
                m3.Activated += new EventHandler(toolStripMenuItem5_Click);
                Gtk.MenuItem m4 = new Gtk.MenuItem("40%");
                m4.Activated += new EventHandler(toolStripMenuItem4_Click);
                Gtk.MenuItem m6 = new Gtk.MenuItem("80%");
                m6.Activated += new EventHandler(toolStripMenuItem2_Click);
                Gtk.MenuItem m5 = new Gtk.MenuItem("60%");
                m5.Activated += new EventHandler(toolStripMenuItem3_Click);
                Gtk.CheckMenuItem ma = new Gtk.CheckMenuItem("On top");
                ma.Active = OnTop;
                ma.Activated += new EventHandler(mx_Click);
                m0.Append(m2);
                m0.Append(m3);
                m0.Append(m4);
                m0.Append(m5);
                m0.Append(m6);
                m2.Show();
                m3.Show();
                m4.Show();
                m5.Show();
                m6.Show();
                ma.Show();
                e.Menu.Append(m1);
                e.Menu.Append(ma);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
        
        /// <summary>
        /// Creates a new instance of micro chat
        /// </summary>
        public MicroChat ()
        {
            this.Build();
            scrollback_mc.RT.textView.PopulatePopup += new PopulatePopupHandler(CreateMenu_simple);
            this.KeepAbove = OnTop;
        }
    }
}

