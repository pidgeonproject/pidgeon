using System;

namespace Client.Forms
{
	public partial class Notification : Gtk.Window
	{
        public Gtk.Label title
        {
            get
            {
                return label1;
            }
        }

        public Gtk.Label text
        {
            get
            {
                return label2;
            }
        }

		public Notification () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
            this.BorderWidth = 0;
            this.Title = "";
            this.KeepAbove = true;
            this.Hide();
            this.ModifyBase(Gtk.StateType.Normal, Core.fromColor(System.Drawing.Color.Orange));
            this.label2.ModifyBase(Gtk.StateType.Normal, Core.fromColor(System.Drawing.Color.Orange));
            this.label1.ModifyBase(Gtk.StateType.Normal, Core.fromColor(System.Drawing.Color.Orange));
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.label1.ButtonPressEvent += new Gtk.ButtonPressEventHandler(_Remove);
            this.label2.ButtonPressEvent += new Gtk.ButtonPressEventHandler(_Remove);
            this.image1.ButtonPressEvent += new Gtk.ButtonPressEventHandler(_Remove);
            this.ButtonPressEvent += new Gtk.ButtonPressEventHandler(_Remove);
            this.AllowGrow = false;
            this.AllowShrink = false;
            this.SetSizeRequest(400, 200);
            this.DeleteEvent += new Gtk.DeleteEventHandler(Unshow);
		}

        public void Relocate()
        {
            if (this.DefaultWidth < System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width && this.DefaultHeight < System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height)
            {
                this.Move(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.DefaultWidth,
                    System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.DefaultHeight);
            }
            this.label1.Markup = "<span size='18000'>" + this.label1.Text + "</span>";
        }

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

