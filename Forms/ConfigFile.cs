using System;
using System.IO;

namespace Client.Forms
{
    public partial class ConfigFile : Gtk.Window
    {
        public Gtk.Button button3;
        public Gtk.Button button4;
        public Gtk.TextView textview1;
        
        public ConfigFile () :  base(Gtk.WindowType.Toplevel)
        {
            try
            {
                this.Build ();
                this.button3.Activated += new EventHandler(button1_Click);
                this.button4.Activated += new EventHandler(button2_Click);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void Load()
        {
            if (!File.Exists(Core.ConfigFile))
            {
                return;
            }
            textview1.Buffer.Text = File.ReadAllText(Core.ConfigFile);
        }

        protected virtual void Build()
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
                Core.ConfigurationLoad();
                Hide();
                Destroy();
            }
        }

        public bool ValidateXml()
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

