using System;
using System.IO;

namespace Client.Forms
{
	public partial class ConfigFile : Gtk.Window
	{
		public Gtk.Button button3;
		public Gtk.Button button4;
		public Gtk.TextView textview1;
		
		public ConfigFile () : 	base(Gtk.WindowType.Toplevel)
		{
			try
            {
				this.Build ();
                if (!File.Exists(Core.ConfigFile))
                {
                    return;
                }
				this.button3.Activated += new EventHandler(button1_Click);
				this.button4.Activated += new EventHandler(button2_Click);
				textview1.Buffer.Text = File.ReadAllText(Core.ConfigFile);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
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

