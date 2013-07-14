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

namespace Client.Forms
{
    /// <summary>
    /// Notification window which is displayed in corner of window when user should be alerted of some event
    /// </summary>
    public partial class Notification : GTK.PidgeonForm
    {
        /// <summary>
        /// Whether the focus should be returned to textbox after this notification is handled
        /// </summary>
        public bool focus = false;
        private Gtk.VBox vbox1;
        private Gtk.Label label1;
        private Gtk.HBox hbox1;
        private Gtk.Image image1;
        private Gtk.Label label2;
        /// <summary>
        /// This is a root box which all components belong to, you can hook some extra events to it
        /// </summary>
        /// Please do not change it to private, that can break some stuff
        public Gtk.EventBox root;

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Forms.Notification
            this.Name = "notification";
            this.Title = "Notification";
            this.TypeHint = Gdk.WindowTypeHint.Normal;
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            // Container child Client.Forms.Notification.Gtk.Container+ContainerChild
            this.vbox1 = new global::Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.vbox1.Spacing = 6;
            // Container child vbox1.Gtk.Box+BoxChild
            this.label1 = new global::Gtk.Label();
            this.label1.HeightRequest = 20;
            this.label1.Name = "label1";
            this.label1.LabelProp = "Notification";
            this.vbox1.Add(this.label1);
            global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.label1]));
            w1.Position = 0;
            w1.Expand = false;
            w1.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox1 = new global::Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.image1 = new global::Gtk.Image();
            this.image1.Name = "image1";
            this.image1.Pixbuf = global::Gdk.Pixbuf.LoadFromResource("Client.Resources.icon.png");
            this.hbox1.Add(this.image1);
            global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.image1]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.label2 = new global::Gtk.Label();
            this.label2.Name = "label2";
            this.label2.WidthRequest = 260;
            this.label2.Wrap = true;
            this.label2.LabelProp = "Description";
            this.hbox1.Add(this.label2);
            global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.label2]));
            w3.Position = 1;
            w3.Expand = false;
            w3.Fill = false;
            this.vbox1.Add(this.hbox1);
            global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            root = new Gtk.EventBox();
            root.ModifyBg(Gtk.StateType.Normal, Core.FromColor(System.Drawing.Color.Orange));
            root.Add(this.vbox1);
            this.Decorated = false;
            this.Add(root);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 460;
            this.DefaultHeight = 220;
        }

        /// <summary>
        /// Title
        /// </summary>
        public Gtk.Label title
        {
            get
            {
                return label1;
            }
        }

        /// <summary>
        /// Text
        /// </summary>
        public Gtk.Label text
        {
            get
            {
                return label2;
            }
        }

        /// <summary>
        /// Creates a new instance of notification
        /// </summary>
        public Notification ()
        {
            this.Build ();
            this.BorderWidth = 0;
            this.Title = "Notification";
            this.KeepAbove = true;
            this.Opacity = 0.6;
            this.ModifyBg(Gtk.StateType.Normal, Core.FromColor(System.Drawing.Color.Orange));
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.root.ButtonPressEvent += new Gtk.ButtonPressEventHandler(_Remove);
            this.AllowGrow = false;
            this.AllowShrink = false;
            this.KeyPressEvent += new Gtk.KeyPressEventHandler(focusText);
            this.SetSizeRequest(400, 160);
            this.DeleteEvent += new Gtk.DeleteEventHandler(Unshow);
        }

        /// <summary>
        /// Move
        /// </summary>
        public void Relocate()
        {
            if (this.Width < System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width && this.Height < System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height)
            {
                this.Move(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.Width,
                    System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            }
        }

        /// <summary>
        /// This is handler for close event
        /// </summary>
        /// <param name="main"></param>
        /// <param name="xx"></param>
        [GLib.ConnectBefore]
        public void _Remove(object main, Gtk.ButtonPressEventArgs xx)
        {
            try
            {
                this.Hide();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Set the focus to text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="keys"></param>
        public void focusText(object sender, Gtk.KeyPressEventArgs keys)
        {
            try
            {
                if (focus)
                {
                    Core.SystemForm.setFocus();
                    if (Core.SystemForm.Chat != null && Core.SystemForm.Chat.textbox != null)
                    {
                        // workaround because of bugs in GTK#
                        this.Hide();
                        Core.SystemForm.Chat.textbox.setFocus();
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Hide
        /// </summary>
        /// <param name="main"></param>
        /// <param name="closing"></param>
        public void Unshow(object main, Gtk.DeleteEventArgs closing)
        {
            try
            {
                this.Hide();
                closing.RetVal = true;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}

