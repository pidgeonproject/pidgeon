
// This file has been generated by the GUI designer. Do not modify.
namespace Client.Forms
{
	public partial class Help
	{
		private global::Gtk.Fixed fixed1;
		private global::Gtk.Label label1;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Client.Forms.Help
			this.WidthRequest = 460;
			this.HeightRequest = 600;
			this.Name = "Client.Forms.Help";
			this.Title = "Help";
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("Client.Resources.pigeon_clip_art_hight.ico");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.Resizable = false;
			this.AllowGrow = false;
			// Container child Client.Forms.Help.Gtk.Container+ContainerChild
			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = "Pidgeon";
			this.fixed1.Add (this.label1);
			global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixed1 [this.label1]));
			w1.X = 216;
			w1.Y = 28;
			this.Add (this.fixed1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 516;
			this.DefaultHeight = 600;
			this.Show ();
		}
	}
}
