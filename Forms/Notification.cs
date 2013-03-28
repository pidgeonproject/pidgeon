using System;

namespace Client.Forms
{
	public partial class Notification : Gtk.Window
	{
		public Notification () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

