
// This file has been generated by the GUI designer. Do not modify.
namespace Client.Graphics
{
	public partial class Window
	{
		private global::Gtk.VPaned vpaned1;
		private global::Gtk.HPaned hpaned1;
		private Client.Scrollback scrollback1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView listView;
		private global::Client.Graphics.TextBox textbox1;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Client.Graphics.Window
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Client.Graphics.Window";
			// Container child Client.Graphics.Window.Gtk.Container+ContainerChild
			this.vpaned1 = new global::Gtk.VPaned ();
			this.vpaned1.CanFocus = true;
			this.vpaned1.Name = "vpaned1";
			this.vpaned1.Position = 319;
			// Container child vpaned1.Gtk.Paned+PanedChild
			this.hpaned1 = new global::Gtk.HPaned ();
			this.hpaned1.CanFocus = true;
			this.hpaned1.Name = "hpaned1";
			this.hpaned1.Position = 333;
			// Container child hpaned1.Gtk.Paned+PanedChild
            //this.scrollback1 = new global::Client.Scrollback();
            this.scrollback1.Create();
            this.scrollback1.Events = ((global::Gdk.EventMask)(256));
            this.scrollback1.Name = "scrollback1";
            this.hpaned1.Add1(this.scrollback1);
            global::Gtk.Paned.PanedChild w1 = ((global::Gtk.Paned.PanedChild)(this.hpaned1[this.scrollback1]));
            w1.Resize = false;
			// Container child hpaned1.Gtk.Paned+PanedChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.listView = new global::Gtk.TreeView ();
			this.listView.CanFocus = true;
			this.listView.Name = "listView";
			this.GtkScrolledWindow1.Add (this.listView);
			this.hpaned1.Add2 (this.GtkScrolledWindow1);
			this.vpaned1.Add1 (this.hpaned1);
			global::Gtk.Paned.PanedChild w4 = ((global::Gtk.Paned.PanedChild)(this.vpaned1 [this.hpaned1]));
			w4.Resize = false;
			// Container child vpaned1.Gtk.Paned+PanedChild
			//this.textbox1 = new global::Client.Graphics.TextBox ();
			this.textbox1.Events = ((global::Gdk.EventMask)(256));
			this.textbox1.Name = "textbox1";
			this.vpaned1.Add2 (this.textbox1);
			this.Add (this.vpaned1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			//this.Hide ();
		}
	}
}
