/***************************************************************************
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) version 3.                                           *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.         *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using Gtk;

namespace Client.Forms
{
    public class Preferences_Shortcut : Gtk.Dialog
    {
        private global::Gtk.VBox vbox3;
        private global::Gtk.HBox hbox1;
        private global::Gtk.Label label3;
        private global::Gtk.Entry entry2;
        private global::Gtk.HBox hbox2;
        private global::Gtk.CheckButton checkbutton2;
        private global::Gtk.CheckButton checkbutton3;
        private global::Gtk.CheckButton checkbutton4;
        private global::Gtk.ComboBox combobox2;
        private global::Gtk.Button buttonCancel;
        private global::Gtk.Button buttonOk;

        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget blah.Ignore
            this.Name = "blah.Ignore";
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.Title = global::Mono.Unix.Catalog.GetString("Shortcut");
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            // Internal child blah.Ignore.VBox
            global::Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.vbox3 = new global::Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.hbox1 = new global::Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.label3 = new global::Gtk.Label();
            this.label3.Name = "label3";
            this.label3.LabelProp = global::Mono.Unix.Catalog.GetString("Command");
            this.hbox1.Add(this.label3);
            global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.label3]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.entry2 = new global::Gtk.Entry();
            this.entry2.CanFocus = true;
            this.entry2.Name = "entry2";
            this.entry2.IsEditable = true;
            this.entry2.InvisibleChar = '●';
            this.hbox1.Add(this.entry2);
            global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.entry2]));
            w3.Position = 1;
            this.vbox3.Add(this.hbox1);
            global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.hbox1]));
            w4.Position = 0;
            w4.Expand = false;
            w4.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.hbox2 = new global::Gtk.HBox();
            this.hbox2.Name = "hbox2";
            this.hbox2.Spacing = 6;
            // Container child hbox2.Gtk.Box+BoxChild
            this.checkbutton2 = new global::Gtk.CheckButton();
            this.checkbutton2.CanFocus = true;
            this.checkbutton2.Name = "checkbutton2";
            this.checkbutton2.Label = global::Mono.Unix.Catalog.GetString("Ctrl");
            this.checkbutton2.DrawIndicator = true;
            this.checkbutton2.UseUnderline = true;
            this.hbox2.Add(this.checkbutton2);
            global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.checkbutton2]));
            w5.Position = 0;
            // Container child hbox2.Gtk.Box+BoxChild
            this.checkbutton3 = new global::Gtk.CheckButton();
            this.checkbutton3.CanFocus = true;
            this.checkbutton3.Name = "checkbutton3";
            this.checkbutton3.Label = global::Mono.Unix.Catalog.GetString("Alt");
            this.checkbutton3.DrawIndicator = true;
            this.checkbutton3.UseUnderline = true;
            this.hbox2.Add(this.checkbutton3);
            global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.checkbutton3]));
            w6.Position = 1;
            // Container child hbox2.Gtk.Box+BoxChild
            this.checkbutton4 = new global::Gtk.CheckButton();
            this.checkbutton4.CanFocus = true;
            this.checkbutton4.Name = "checkbutton4";
            this.checkbutton4.Label = global::Mono.Unix.Catalog.GetString("Shift");
            this.checkbutton4.DrawIndicator = true;
            this.checkbutton4.UseUnderline = true;
            this.hbox2.Add(this.checkbutton4);
            global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.checkbutton4]));
            w7.Position = 2;
            // Container child hbox2.Gtk.Box+BoxChild
            this.combobox2 = global::Gtk.ComboBox.NewText();
            this.combobox2.WidthRequest = 200;
            this.combobox2.Name = "combobox2";
            this.hbox2.Add(this.combobox2);
            global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.combobox2]));
            w8.Position = 3;
            w8.Expand = false;
            w8.Fill = false;
            this.vbox3.Add(this.hbox2);
            global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.hbox2]));
            w9.Position = 1;
            w9.Expand = false;
            w9.Fill = false;
            w1.Add(this.vbox3);
            global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(w1[this.vbox3]));
            w10.Position = 0;
            w10.Expand = false;
            w10.Fill = false;
            // Internal child blah.Ignore.ActionArea
            global::Gtk.HButtonBox w11 = this.ActionArea;
            w11.Name = "dialog1_ActionArea";
            w11.Spacing = 10;
            w11.BorderWidth = ((uint)(5));
            w11.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonCancel = new global::Gtk.Button();
            this.buttonCancel.CanDefault = true;
            this.buttonCancel.CanFocus = true;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseStock = true;
            this.buttonCancel.UseUnderline = true;
            this.buttonCancel.Label = "gtk-cancel";
            this.AddActionWidget(this.buttonCancel, -6);
            global::Gtk.ButtonBox.ButtonBoxChild w12 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w11[this.buttonCancel]));
            w12.Expand = false;
            w12.Fill = false;
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonOk = new global::Gtk.Button();
            this.buttonOk.CanDefault = true;
            this.buttonOk.CanFocus = true;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseStock = true;
            this.buttonOk.UseUnderline = true;
            this.buttonOk.Label = "gtk-ok";
            this.AddActionWidget(this.buttonOk, -5);
            global::Gtk.ButtonBox.ButtonBoxChild w13 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w11[this.buttonOk]));
            w13.Position = 1;
            w13.Expand = false;
            w13.Fill = false;
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 738;
            this.DefaultHeight = 178;
            this.Show();
        }
    }
}
