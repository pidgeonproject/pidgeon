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
    /// Script edit.
    /// </summary>
    public partial class ScriptEdit : GTK.PidgeonForm
    {
        /// <summary>
        /// Network
        /// </summary>
        public Network network = null;
        private global::Gtk.VBox vbox1;
        private global::Gtk.TextView label1;
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TextView textview1;
        private global::Gtk.HBox hbox1;
        private global::Gtk.Button button2;
        private global::Gtk.Button button1;
        
        /// <summary>
        /// Gets the text box1.
        /// </summary>
        /// <value>
        /// The text box1.
        /// </value>
        public TextView textBox1
        {
            get
            {
                return textview1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client.Forms.ScriptEdit"/> class.
        /// </summary>
        public ScriptEdit()
        {
            this.Build();
            this.LC("ScriptEdit");
            this.button1.Clicked += new EventHandler(button1_Click);
            messages.Localize(this);
            this.button2.Clicked += new EventHandler(button2_Click);
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Client.Forms.ScriptEdit"/> is reclaimed by garbage collection.
        /// </summary>
        ~ScriptEdit()
        {
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor for script edit");
            }
        }

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Forms.ScriptEdit
            this.Name = "Client.Forms.ScriptEdit";
            this.Title = "ScriptEdit";
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            // Container child Client.Forms.ScriptEdit.Gtk.Container+ContainerChild
            this.vbox1 = new global::Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 2;
            // Container child vbox1.Gtk.Box+BoxChild
            this.label1 = new Gtk.TextView();
            this.label1.Name = "label1";
            this.label1.Editable = false;
            this.label1.Buffer.Text = "Do you really want to execute following commands?\n\nLines prefixed with following symbols are:\n# - comment\n/ - will be launched as pidgeon command\n\nOther lines will be delivered as raw command to server";
            this.vbox1.Add(label1);
            global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.label1]));
            w1.Position = 0;
            w1.Expand = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.textview1 = new global::Gtk.TextView();
            this.textview1.CanFocus = true;
            this.textview1.Name = "textview1";
            this.GtkScrolledWindow.Add(this.textview1);
            this.vbox1.Add(this.GtkScrolledWindow);
            global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.GtkScrolledWindow]));
            w3.Position = 1;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox1 = new global::Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.button2 = new global::Gtk.Button();
            this.button2.CanFocus = true;
            this.button2.Name = "button2";
            this.button2.UseUnderline = true;
            this.button2.Label = "Execute";
            this.hbox1.Add(this.button2);
            global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.button2]));
            w4.Position = 0;
            w4.Expand = false;
            w4.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.button1 = new global::Gtk.Button();
            this.button1.CanFocus = true;
            this.button1.Name = "button1";
            this.button1.UseUnderline = true;
            this.button1.Label = "Abort";
            this.hbox1.Add(this.button1);
            global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.button1]));
            w5.Position = 1;
            w5.Expand = false;
            w5.Fill = false;
            this.vbox1.Add(this.hbox1);
            global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
            w6.Position = 2;
            w6.Expand = false;
            this.label1.ModifyBase(StateType.Normal, Core.FromColor(Color.LightGray));
            w6.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 740;
            this.WindowPosition = Gtk.WindowPosition.Center;
            this.DefaultHeight = 460;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide ();
            this.Destroy ();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string[] values = textview1.Buffer.Text.Split ('\n');
                foreach (string text in values)
                {
                    if (text != "")
                    {
                        if (text.StartsWith("#"))
                        {
                            continue;
                        }
                        if (text.StartsWith(Configuration.CommandPrefix))
                        {
                            Core.ProcessCommand(text);
                            continue;
                        }
                        network.Transfer(text, Configuration.Priority.High);
                    }
                }
                Hide();
                this.Destroy();
            }
            catch(Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}

