using System;

namespace Client.Forms
{
	public partial class Help : Gtk.Window
	{
		public Help () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			this.label1.Markup = "<span size='18000'>Pidgeon</span>";
		}
	}
}

