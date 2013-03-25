using System;

namespace Client.Forms
{
	public partial class Help : Gtk.Window
	{
		public Help () : base(Gtk.WindowType.Toplevel)
		{
			try
			{
				this.Build ();
				messages.Localize (this);
				this.label1.Markup = "<span size='18000'>Pidgeon</span>";
				this.label4.Text = Configuration.Version + " build: " + RevisionProvider.GetHash();
			} catch (Exception fail)
			{
				Core.handleException(fail);
			}
		}
	}
}

