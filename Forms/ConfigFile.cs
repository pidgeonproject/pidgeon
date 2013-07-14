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
using System.IO;

namespace Client.Forms
{
    /// <summary>
    /// Editor
    /// </summary>
    public partial class ConfigFile : Client.GTK.PidgeonForm
    {
        private Gtk.Button button3;
        private Gtk.Button button4;
        private Gtk.TextView textview1;
        private Gtk.Label label;
        
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public ConfigFile()
        {
            try
            {
                this.Build();
                this.LC("ConfigFile");
                this.button3.Activated += new EventHandler(button1_Click);
                this.button4.Activated += new EventHandler(button2_Click);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        public void Load()
        {
            if (!File.Exists(Core.ConfigFile))
            {
                return;
            }
            textview1.Buffer.Text = File.ReadAllText(Core.ConfigFile);
            Locate();
        }

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Forms.ConfigFile
            this.Name = "Client.Forms.ConfigFile";
            this.Title = "ConfigFile";
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            Gtk.VBox vbox1 = new global::Gtk.VBox();
            vbox1.Name = "vbox1";
            vbox1.Spacing = 6;
            Gtk.HBox hbox1 = new Gtk.HBox();
            hbox1.Name = "hbox";
            Gtk.ScrolledWindow GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            GtkScrolledWindow.Name = "GtkScrolledWindow";
            GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            textview1 = new Gtk.TextView();
            vbox1.Add(GtkScrolledWindow);
            GtkScrolledWindow.Add(textview1);
            vbox1.Add(hbox1);
            global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(vbox1[hbox1]));
            w6.Position = 2;
            w6.Expand = false;
            w6.Fill = false;
            button3 = new Gtk.Button();
            button3.Name = "button3";
            button3.UseUnderline = true;
            button3.Label = "Save";
            //button3.SetSizeRequest(220, 20);
            button3.Clicked += new EventHandler(button1_Click);
            hbox1.Add(button3);
            global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(hbox1[this.button3]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            button4 = new Gtk.Button();
            button4.UseUnderline = true;
            button4.Name = "button4";
            //button4.SetSizeRequest(200, 20);
            button4.Label = "Check syntax";
            button4.Clicked += new EventHandler(button2_Click);
            hbox1.Add(button4);
            global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(hbox1[this.button4]));
            w4.Position = 0;
            w4.Expand = false;
            w4.Fill = false;
            this.label = new Gtk.Label();
            label.Name = "label";
            label.Text = "L 0 C 0";
            hbox1.Add(label);
            textview1.MoveCursor += new Gtk.MoveCursorHandler(cursor);
            this.Add(vbox1);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.DefaultWidth = 680;
            this.DefaultHeight = 480;
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ValidateXml())
            {
                File.WriteAllText(Core.ConfigFile, textview1.Buffer.Text);
                Core._Configuration.ConfigurationLoad();
                Hide();
                Destroy();
            }
        }

        private void cursor(object sender, Gtk.MoveCursorArgs e)
        {
            try
            {
                Locate();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Locate()
        {
            Gtk.TextIter iter = textview1.Buffer.GetIterAtOffset(textview1.Buffer.CursorPosition);
            int X = iter.LineOffset;
            int Y = iter.Line;
            label.Text = "L: " + Y.ToString() + " C: " + X.ToString();
        }

        private bool ValidateXml()
        {
            try
            {
                System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                document.LoadXml(textview1.Buffer.Text);
                return true;
            }
            catch (Exception reason)
            {
                GTK.MessageBox.Show(this, Gtk.MessageType.Warning, Gtk.ButtonsType.Ok, "Error found: " + reason.Message, "Invalid config");
                return false;
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            ValidateXml();
        }
    }
}

