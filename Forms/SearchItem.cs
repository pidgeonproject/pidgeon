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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using Gtk;

namespace Client.Forms
{
    /// <summary>
    /// Search item.
    /// </summary>
    public partial class SearchItem : GTK.PidgeonForm
    {   
        private bool NeedReset = false;
        private global::Gtk.HBox hbox1;
        private global::Gtk.Entry entry1;
        //private global::Gtk.CheckButton checkbutton1;
        /// <summary>
        /// The direction
        /// </summary>
        public new bool Direction = false;
        private global::Gtk.Button button1;
        private global::Gtk.Button button2;
        private TextIter position;
        
        private void Build ()
        {
            global::Stetic.Gui.Initialize (this);
            // Widget MainWindow
            this.Name = "MainWindow";
            this.Title = "Search text";
            this.TypeHint = Gdk.WindowTypeHint.Normal;
            this.Resizable = false;
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            // Container child MainWindow.Gtk.Container+ContainerChild
            this.hbox1 = new global::Gtk.HBox ();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            // Container child hbox1.Gtk.Box+BoxChild
            this.entry1 = new global::Gtk.Entry ();
            this.entry1.CanFocus = true;
            this.entry1.Name = "entry1";
            this.entry1.IsEditable = true;
            this.entry1.WidthRequest = 800;
            this.entry1.InvisibleChar = '•';
            this.hbox1.Add (this.entry1);
            global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.entry1]));
            w1.Position = 0;
            // Container child hbox1.Gtk.Box+BoxChild
            this.button1 = new global::Gtk.Button ();
            this.button1.CanFocus = true;
            this.button1.Name = "button1";
            this.button1.UseUnderline = true;
            this.button1.Label = "Up";
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
            this.ConfigureEvent += new ConfigureEventHandler(Relocate);
            this.button2.UseUnderline = true;
            this.button2.Label = "Down";
            this.hbox1.Add (this.button2);
            this.entry1.KeyPressEvent += new KeyPressEventHandler(enter_press);
            global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.button2]));
            w4.Position = 3;
            entry1.Changed += new EventHandler(textBox1_TextChanged);
            w4.Expand = false;
            w4.Fill = false;
            this.DeleteEvent += new DeleteEventHandler(hide2);
            this.Add (this.hbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll ();
            }
            Move (Configuration.Window.Search_X, Configuration.Window.Search_Y);
            this.DefaultWidth = 800;
            this.DefaultHeight = 20;
            Init();
        }
        
        private void hide2(object sender, DeleteEventArgs e)
        {
            e.RetVal = true;
            Hide();
        }

        private void Init()
        {
            Direction = false;
        }

        private void Relocate(object sender, ConfigureEventArgs e)
        {
            try
            {
                Configuration.Window.Search_Y = this.Top;
                Configuration.Window.Search_X = this.Left;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Client.Forms.SearchItem"/> class.
        /// </summary>
        public SearchItem()
        {
            this.Build();
            this.LC("SearchItem");
            messages.Localize(this);
        }
        
        [GLib.ConnectBefore]
        private void enter_press(object sender, KeyPressEventArgs keys)
        {
            try
            {
                if (keys.Event.KeyValue == 65293)
                {
                    SearchRun(Direction);
                    Configuration.Window.Search_Y = this.Top;
                    Configuration.Window.Search_X = this.Left;
                }
            }catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
        
        private void SearchItem_Keys(object sender, Gtk.KeyPressEventArgs keys)
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

        private void SearchRun(bool tp)
        {
            Direction = tp;
            Scrollback text = null;
            Gtk.TextView tv = null;
            if (Core.SystemForm.Chat == null || Core.SystemForm.Chat.scrollback == null)
            {
                return;
            }

            text = Core.SystemForm.Chat.scrollback;
            
            if (!text.IsSimple)
            {
                tv = text.RT.textView;
            }
            else
            {
                tv = text.simpleview;
            }
            TextIter start;
            TextIter stop;
            if (tp)
            {
                // we need to scroll down from current position
                if (!NeedReset)
                {
                    position = tv.Buffer.StartIter;
                }
                //position = tv.Buffer.GetIterAtOffset (tv.Buffer.CursorPosition);
                if (position.ForwardSearch(entry1.Text, TextSearchFlags.TextOnly, out start, out stop, tv.Buffer.EndIter))
                {
                    NeedReset = true;
                    entry1.ModifyBase(StateType.Normal, Core.FromColor (System.Drawing.Color.LightGreen));
                    position = stop;
                    tv.Buffer.SelectRange (start, stop);
                    tv.ScrollToIter (start, 0, false, 0, 0);
                } else
                {
                    NeedReset = true;
                    entry1.ModifyBase(StateType.Normal, Core.FromColor (System.Drawing.Color.Pink));
                }
            }
            else
            {
                // we need to scroll up from current position
                if (!NeedReset)
                {
                    position = tv.Buffer.EndIter;
                }
                if (position.BackwardSearch(entry1.Text, TextSearchFlags.TextOnly, out start, out stop, tv.Buffer.StartIter))
                {
                    NeedReset = true;
                    position = start;
                    entry1.ModifyBase(StateType.Normal, Core.FromColor (System.Drawing.Color.LightGreen));
                    tv.Buffer.SelectRange (start, stop);
                    tv.ScrollToIter (start, 0, false, 0, 0);
                } else
                {
                    NeedReset = true;
                    entry1.ModifyBase(StateType.Normal, Core.FromColor (System.Drawing.Color.Pink));
                }
            }
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
            if (!NeedReset)
            {
                return;
            }
            NeedReset = false;
            entry1.ModifyBase(StateType.Normal, Core.FromColor (System.Drawing.Color.White));
            return;
        }
        
        /// <summary>
        /// Change focus
        /// </summary>
        public void setFocus()
        {
            entry1.GrabFocus();
        }
    }
}

