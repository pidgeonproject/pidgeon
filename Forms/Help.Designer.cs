//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

namespace Client.Forms
{
    public partial class Help
    {
        private global::Gtk.Fixed fixed1;
        private global::Gtk.Label label2;
        private global::Gtk.Label label3;
        private global::Gtk.Label label4;
        private global::Gtk.Image image2;
        private global::Gtk.Label label1;
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TextView textview1;

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Forms.Help
            this.WidthRequest = 520;
            this.HeightRequest = 600;
            this.Name = "Client.Forms.Help";
            this.Title = "Help";
            this.Icon = global::Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.WindowPosition = ((global::Gtk.WindowPosition)(1));
            this.Resizable = false;
            this.AllowGrow = false;
            // Container child Client.Forms.Help.Gtk.Container+ContainerChild
            this.fixed1 = new global::Gtk.Fixed();
            this.fixed1.Name = "fixed1";
            this.fixed1.HasWindow = false;
            // Container child fixed1.Gtk.Fixed+FixedChild
            this.label2 = new global::Gtk.Label();
            this.label2.Name = "label2";
            this.label2.LabelProp = "Made by: Petr Bena";
            this.fixed1.Add(this.label2);
            global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label2]));
            w1.X = 20;
            w1.Y = 61;
            // Container child fixed1.Gtk.Fixed+FixedChild
            Gtk.EventBox eb = new Gtk.EventBox();
            eb.ButtonPressEvent += new Gtk.ButtonPressEventHandler(Link);
            this.label3 = new global::Gtk.Label();
            this.label3.Name = "label3";
            this.label3.UseUnderline = true;
            this.label3.ModifyFg(Gtk.StateType.Normal, Core.FromColor(System.Drawing.Color.Blue));
            this.label3.Markup = "<a href=\"\">http://pidgeonclient.org/wiki/</a>";
            this.label3.UseUnderline = true;
            eb.Add(this.label3);
            this.fixed1.Add(eb);
            global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[eb]));
            w2.X = 20;
            w2.Y = 87;
            // Container child fixed1.Gtk.Fixed+FixedChild
            this.label4 = new global::Gtk.Label();
            this.label4.Name = "label4";
            this.fixed1.Add(this.label4);
            global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label4]));
            w3.X = 20;
            w3.Y = 127;
            // Container child fixed1.Gtk.Fixed+FixedChild
            this.image2 = new global::Gtk.Image();
            this.image2.Name = "image2";
            this.image2.Pixbuf = global::Gdk.Pixbuf.LoadFromResource("Client.Resources.Pigeon_clip_art_hight_mini.png");
            this.fixed1.Add(this.image2);
            global::Gtk.Fixed.FixedChild w4 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.image2]));
            w4.X = 342;
            w4.Y = 47;
            // Container child fixed1.Gtk.Fixed+FixedChild
            this.label1 = new global::Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = "Pidgeon";
            this.fixed1.Add(this.label1);
            global::Gtk.Fixed.FixedChild w5 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.label1]));
            w5.X = 220;
            w5.Y = 14;
            // Container child fixed1.Gtk.Fixed+FixedChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.textview1 = new global::Gtk.TextView();
            this.textview1.Buffer.Text = "About:\n\nThe pidgeon project was established as an open source which anyone can edit or improve, or even suggest new features to. The source code is located at gitorious (link is available on wiki), but in order to submit patches, you don't need to be a member of the project.\n\nThat means:\nEveryone who wants to contribute to this project, is welcome and definitely should be able to do that, there is no need to request any permissions to modify the core sources or to develop plugins. If you like this project and you want to contribute to make pidgeon even better, please see our website\n\nLicense:\n\nThis program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or  (at your option) version 3.\n\nThis program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of\n\nMERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.";
            this.textview1.WidthRequest = 500;
            this.textview1.HeightRequest = 400;
            this.textview1.CanFocus = true;
            this.textview1.Name = "textview1";
            this.textview1.Editable = false;
            this.textview1.WrapMode = ((global::Gtk.WrapMode)(2));
            this.GtkScrolledWindow.Add(this.textview1);
            this.fixed1.Add(this.GtkScrolledWindow);
            global::Gtk.Fixed.FixedChild w7 = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.GtkScrolledWindow]));
            w7.X = 8;
            w7.Y = 171;
            this.Add(this.fixed1);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 579;
            this.DefaultHeight = 629;
            this.Show();
        }
    }
}
