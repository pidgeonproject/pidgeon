
// This file has been generated by the GUI designer. Do not modify.
namespace Client.Forms
{
	public partial class Preferences
	{
		private global::Gtk.VBox vbox1;
		private global::Gtk.HBox hbox1;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Button button1;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Client.Forms.Preferences
			this.Name = "Client.Forms.Preferences";
			this.Title = "Preferences";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child Client.Forms.Preferences.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			this.vbox1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox1]));
			w1.Position = 0;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.button1 = new global::Gtk.Button ();
			this.button1.CanFocus = true;
			this.button1.Name = "button1";
			this.button1.UseUnderline = true;
			this.button1.Label = "GtkButton";
			this.hbox3.Add (this.button1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.button1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.vbox1.Add (this.hbox3);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox3]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 767;
			this.DefaultHeight = 479;
			this.Show ();
		}
	}
}
