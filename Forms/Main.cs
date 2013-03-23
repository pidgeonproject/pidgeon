using System;

namespace Client.Forms
{
	public partial class Main : Gtk.Window
	{
		public Main () : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

