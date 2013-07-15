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
using System.Text;
using System.Data;
using Gtk;

// Documentation
//////////////////////
// Traffic scanner is allowing you to display all traffic sent within the protocols

namespace Client.Forms
{
    public partial class TrafficScanner : GTK.PidgeonForm
    {
        private List<string> traf = new List<string>();
        private GLib.TimeoutHandler timer;
        private bool ScannerEnabled = true;
        private GTK.Menu scroll = new GTK.Menu("Scroll");
        private GTK.Menu remove = new GTK.Menu("Delete");
        
        /// <summary>
        /// Creates a new instance of scanner
        /// </summary>
        public TrafficScanner ()
        {
            try
            {
                this.Build ();
                this.LC("TrafficScanner");
                this.textview2.PopulatePopup += new PopulatePopupHandler(CreateMenu_simple);
                this.timer = new GLib.TimeoutHandler(Tick);
                GLib.Timeout.Add (1000, timer);
                this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
                this.DeleteEvent += new DeleteEventHandler(Unshow);
                textview2.Buffer.Text = "";
                LoadStyle();
                textview2.WrapMode = WrapMode.Char;
                scroll.Checked = ScannerEnabled;
                
                this.Hide ();
            } catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        [GLib.ConnectBefore]
        private void CreateMenu_simple(object o, Gtk.PopulatePopupArgs e)
        {
            try
            {
                Gtk.SeparatorMenuItem separator1 = new Gtk.SeparatorMenuItem();
                separator1.Show();
                e.Menu.Append(separator1);
                Gtk.CheckMenuItem sc = new CheckMenuItem(scroll.Text);
                sc.Active = scroll.Checked;
                sc.Show();
                e.Menu.Append(sc);
                Gtk.MenuItem m1 = new Gtk.MenuItem(remove.Text);
                m1.Activated += new EventHandler(Clear);
                m1.Show();
                sc.Activated += new EventHandler(Scroll);
                e.Menu.Append(m1);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Scroll(object sender, EventArgs e)
        {
            scroll.Checked = !scroll.Checked;
            ScannerEnabled = scroll.Checked;
        }

        private void Clear(object sender, EventArgs e)
        {
            Clean();
        }

        private void LoadStyle()
        {
            textview2.ModifyBase (StateType.Normal, Core.FromColor(Configuration.CurrentSkin.backgroundcolor));
            textview2.ModifyText(StateType.Normal, Core.FromColor(Configuration.CurrentSkin.colordefault));
        }
        
        /// <summary>
        /// Remove all data from operating memory
        /// </summary>
        public void Clean()
        {
            lock (traf)
            {
                traf.Clear();
            }
            textview2.Buffer.Text = "";
        }
        
        private void Unshow(object main, Gtk.DeleteEventArgs closing)
        {
            Hide();
            closing.RetVal = true;
        }
        
        private bool Tick()
        {
            if (!ScannerEnabled)
            {
                return true;
            }
            if (!this.Visible)
            {
                return true;
            }
            StringBuilder text = new StringBuilder("");
            lock (traf)
            {
                if (traf.Count > 800)
                {
                    Client.GTK.MessageBox message = new Client.GTK.MessageBox(this, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, "There are too many items in log, which means, that pidgeon may become unresponsive for several minutes if you continue, press yes to continue or no to abort", "Warning");
                    if (message.result == ResponseType.No)
                    {
                        ScannerEnabled = false;
                        scroll.Checked = false;
                        return true;
                    }
                }
                foreach (string xx in traf)
                {
                    text.Append(xx + Environment.NewLine);
                }
                traf.Clear();
            }
            TextIter iter = textview2.Buffer.EndIter;
            textview2.Buffer.Insert(ref iter, text.ToString());
            return true;
        }
        
        /// <summary>
        /// Add a line to scanner list
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Text"></param>
        public void Insert(string Server, string Text)
        {
            lock (traf)
            {
                traf.Add(Server + " " + Text);
            }
        }

        /// <summary>
        /// Add a line to scanner list
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Text"></param>
        [Obsolete]
        public void insert(string Server, string Text)
        {
            Insert(Server, Text);
        }
    }
}

