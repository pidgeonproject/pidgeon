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

// this is a designer file for connection

using System;
using System.Collections.Generic;

namespace Client.Forms
{
    public partial class Connection
    {
        private global::Gtk.HBox hbox1;
        private global::Gtk.Table table1;
        private global::Gtk.CheckButton checkbutton1;
        private global::Gtk.ComboBox combobox1;
        private global::Gtk.ComboBoxEntry comboboxentry1;
        private global::Gtk.Entry entry1;
        private global::Gtk.Entry entry2;
        private global::Gtk.Entry entry3;
        private global::Gtk.Entry entry4;
        private global::Gtk.Fixed fixed1;
        private global::Gtk.Label label1;
        private global::Gtk.Label label2;
        private global::Gtk.Label label3;
        private global::Gtk.Label label4;
        private global::Gtk.Label label5;
        private global::Gtk.Label label6;
        private global::Gtk.VBox vbox2;
        private global::Gtk.Image image1;
        private global::Gtk.Button button1;

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Forms.Connection
            this.Name = "Client.Forms.Connection";
            this.Title = messages.Localize("[[newconnection-title]]");
            this.Icon = global::Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.WindowPosition = ((global::Gtk.WindowPosition)(1));
            this.AllowGrow = false;
            // Container child Client.Forms.Connection.Gtk.Container+ContainerChild
            this.hbox1 = new global::Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.table1 = new global::Gtk.Table(((uint)(8)), ((uint)(2)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(6));
            this.table1.ColumnSpacing = ((uint)(6));
            // Container child table1.Gtk.Table+TableChild
            this.checkbutton1 = new global::Gtk.CheckButton();
            this.checkbutton1.CanFocus = true;
            this.checkbutton1.Name = "checkbutton1";
            this.checkbutton1.Label = messages.Localize("[[newconnection-secure]]");
            this.checkbutton1.DrawIndicator = true;
            this.checkbutton1.UseUnderline = true;
            this.table1.Add(this.checkbutton1);
            global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1[this.checkbutton1]));
            w1.TopAttach = ((uint)(7));
            w1.BottomAttach = ((uint)(8));
            w1.LeftAttach = ((uint)(1));
            w1.RightAttach = ((uint)(2));
            w1.XOptions = ((global::Gtk.AttachOptions)(4));
            w1.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.combobox1 = global::Gtk.ComboBox.NewText();
            this.combobox1.WidthRequest = 580;
            this.combobox1.Name = "combobox1";
            this.table1.Add(this.combobox1);
            global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1[this.combobox1]));
            w2.TopAttach = ((uint)(6));
            w2.BottomAttach = ((uint)(7));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.XOptions = ((global::Gtk.AttachOptions)(4));
            w2.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.comboboxentry1 = global::Gtk.ComboBoxEntry.NewText();
            this.comboboxentry1.WidthRequest = 580;
            this.comboboxentry1.Name = "comboboxentry1";
            this.table1.Add(this.comboboxentry1);
            global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1[this.comboboxentry1]));
            w3.TopAttach = ((uint)(4));
            w3.BottomAttach = ((uint)(5));
            w3.LeftAttach = ((uint)(1));
            w3.RightAttach = ((uint)(2));
            w3.XOptions = ((global::Gtk.AttachOptions)(4));
            w3.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.entry1 = new global::Gtk.Entry();
            this.entry1.WidthRequest = 580;
            this.entry1.CanFocus = true;
            this.entry1.Name = "entry1";
            this.entry1.IsEditable = true;
            this.entry1.InvisibleChar = '•';
            this.table1.Add(this.entry1);
            global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1[this.entry1]));
            w4.TopAttach = ((uint)(1));
            w4.BottomAttach = ((uint)(2));
            w4.LeftAttach = ((uint)(1));
            w4.RightAttach = ((uint)(2));
            w4.XOptions = ((global::Gtk.AttachOptions)(4));
            w4.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.entry2 = new global::Gtk.Entry();
            this.entry2.WidthRequest = 580;
            this.entry2.CanFocus = true;
            this.entry2.Name = "entry2";
            this.entry2.IsEditable = true;
            this.entry2.InvisibleChar = '•';
            this.table1.Add(this.entry2);
            global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1[this.entry2]));
            w5.TopAttach = ((uint)(2));
            w5.BottomAttach = ((uint)(3));
            w5.LeftAttach = ((uint)(1));
            w5.RightAttach = ((uint)(2));
            w5.XOptions = ((global::Gtk.AttachOptions)(4));
            w5.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.entry3 = new global::Gtk.Entry();
            this.entry3.WidthRequest = 580;
            this.entry3.CanFocus = true;
            this.entry3.Name = "entry3";
            this.entry3.IsEditable = true;
            this.entry3.InvisibleChar = '•';
            this.table1.Add(this.entry3);
            global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1[this.entry3]));
            w6.TopAttach = ((uint)(3));
            w6.BottomAttach = ((uint)(4));
            w6.LeftAttach = ((uint)(1));
            w6.RightAttach = ((uint)(2));
            w6.XOptions = ((global::Gtk.AttachOptions)(4));
            w6.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.entry4 = new global::Gtk.Entry();
            this.entry4.WidthRequest = 580;
            this.entry4.CanFocus = true;
            this.entry4.Name = "entry4";
            this.entry4.IsEditable = true;
            this.entry4.InvisibleChar = '•';
            this.table1.Add(this.entry4);
            global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1[this.entry4]));
            w7.TopAttach = ((uint)(5));
            w7.BottomAttach = ((uint)(6));
            w7.LeftAttach = ((uint)(1));
            w7.RightAttach = ((uint)(2));
            w7.XOptions = ((global::Gtk.AttachOptions)(4));
            w7.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.fixed1 = new global::Gtk.Fixed();
            this.fixed1.HeightRequest = 20;
            this.fixed1.Name = "fixed1";
            this.fixed1.HasWindow = false;
            this.table1.Add(this.fixed1);
            global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1[this.fixed1]));
            w8.LeftAttach = ((uint)(1));
            w8.RightAttach = ((uint)(2));
            w8.XOptions = ((global::Gtk.AttachOptions)(4));
            w8.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label1 = new global::Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = messages.Localize("[[newconnection-nick]]");
            this.table1.Add(this.label1);
            global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1[this.label1]));
            w9.TopAttach = ((uint)(1));
            w9.BottomAttach = ((uint)(2));
            w9.XOptions = ((global::Gtk.AttachOptions)(4));
            w9.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label2 = new global::Gtk.Label();
            this.label2.Name = "label2";
            this.label2.LabelProp = messages.Localize("[[newconnection-ident]]");
            this.table1.Add(this.label2);
            global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1[this.label2]));
            w10.TopAttach = ((uint)(2));
            w10.BottomAttach = ((uint)(3));
            w10.XOptions = ((global::Gtk.AttachOptions)(4));
            w10.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label3 = new global::Gtk.Label();
            this.label3.Name = "label3";
            this.label3.LabelProp = messages.Localize("[[newconnection-port]]");
            this.table1.Add(this.label3);
            global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1[this.label3]));
            w11.TopAttach = ((uint)(3));
            w11.BottomAttach = ((uint)(4));
            w11.XOptions = ((global::Gtk.AttachOptions)(4));
            w11.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label4 = new global::Gtk.Label();
            this.label4.Name = "label4";
            this.label4.LabelProp = messages.Localize("[[newconnection-address]]");
            this.table1.Add(this.label4);
            global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table1[this.label4]));
            w12.TopAttach = ((uint)(4));
            w12.BottomAttach = ((uint)(5));
            w12.XOptions = ((global::Gtk.AttachOptions)(4));
            w12.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label5 = new global::Gtk.Label();
            this.label5.Name = "label5";
            this.label5.LabelProp = "Password:";
            this.table1.Add(this.label5);
            global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table1[this.label5]));
            w13.TopAttach = ((uint)(5));
            w13.BottomAttach = ((uint)(6));
            w13.XOptions = ((global::Gtk.AttachOptions)(4));
            w13.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label6 = new global::Gtk.Label();
            this.label6.Name = "label6";
            this.label6.LabelProp = "Protocol:";
            this.table1.Add(this.label6);
            global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table1[this.label6]));
            w14.TopAttach = ((uint)(6));
            w14.BottomAttach = ((uint)(7));
            w14.XOptions = ((global::Gtk.AttachOptions)(4));
            w14.YOptions = ((global::Gtk.AttachOptions)(4));
            this.hbox1.Add(this.table1);
            global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.table1]));
            w15.Position = 0;
            w15.Expand = false;
            w15.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.vbox2 = new global::Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.image1 = new global::Gtk.Image();
            this.image1.Name = "image1";
            this.image1.Pixbuf = global::Gdk.Pixbuf.LoadFromResource("Client.Resources.darknetwork.png");
            this.vbox2.Add(this.image1);
            global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.image1]));
            w16.Position = 0;
            w16.Expand = false;
            w16.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.button1 = new global::Gtk.Button();
            this.button1.CanFocus = true;
            this.button1.Name = "button1";
            this.button1.UseUnderline = true;
            this.button1.Label = "Connect to selected server";
            this.vbox2.Add(this.button1);
            global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.button1]));
            w17.Position = 1;
            w17.Expand = false;
            w17.Fill = false;
            this.hbox1.Add(this.vbox2);
            global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.vbox2]));
            w18.Position = 1;
            w18.Expand = false;
            w18.Fill = false;
            this.Add(this.hbox1);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 927;
            this.DefaultHeight = 320;
            Hooks._Sys.Connection(this);
            this.Show();
        }
    }
}
