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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using Gtk;

namespace Client.Forms
{
    /// <summary>
    /// Highlight 
    /// </summary>
    public class Preferences_Hl : Gtk.Dialog
    {
        private global::Gtk.Table table1;
        private global::Gtk.CheckButton checkbutton1;
        private global::Gtk.Entry entry1;
        private global::Gtk.Label label1;
        private global::Gtk.Button buttonCancel;
        private global::Gtk.Button buttonOk;

        //private ListStore list = new ListStore(typeof(string), typeof(Ignoring.Ignore.Type));

        /// <summary>
        /// Initializes a new instance of the <see cref="Client.Forms.Preferences_Hl"/> class.
        /// </summary>
        public Preferences_Hl()
        {
            this.Build();
        }

        private void close(object sender, EventArgs e)
        {
            this.Hide();
            this.Destroy();
        }

        private void Insert(object sender, EventArgs e)
        {
            try
            {
                Network.Highlighter hl = new Network.Highlighter();
                hl.enabled = true;
                hl.simple = !checkbutton1.Active;
                hl.text = entry1.Text;
                lock (Configuration.HighlighterList)
                {
                    Configuration.HighlighterList.Add(hl);
                }
                Core.SystemForm.fPrefs.ReloadHL();
                this.Hide();
                Destroy();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget blah.Ignore
            this.Name = "blah.Ignore";
            this.Title = messages.Localize("[[preferences-highlighter-add]]");
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.WindowPosition = Gtk.WindowPosition.Center;
            // Internal child blah.Ignore.VBox
            global::Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog1_VBox";
            w1.BorderWidth = ((uint)(2));
            // Container child dialog1_VBox.Gtk.Box+BoxChild
            this.table1 = new global::Gtk.Table(((uint)(3)), ((uint)(2)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(6));
            this.table1.ColumnSpacing = ((uint)(6));
            // Container child table1.Gtk.Table+TableChild
            this.checkbutton1 = new global::Gtk.CheckButton();
            this.checkbutton1.CanFocus = true;
            this.checkbutton1.Name = "checkbutton1";
            this.checkbutton1.Label = messages.Localize("[[preferences-highlighter-regex]]");
            this.checkbutton1.DrawIndicator = true;
            this.checkbutton1.UseUnderline = true;
            this.table1.Add(this.checkbutton1);
            global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1[this.checkbutton1]));
            w2.TopAttach = ((uint)(1));
            w2.BottomAttach = ((uint)(2));
            w2.XOptions = ((global::Gtk.AttachOptions)(4));
            w2.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.entry1 = new global::Gtk.Entry();
            this.entry1.WidthRequest = 620;
            this.entry1.CanFocus = true;
            this.entry1.Text = "$nick!$ident@$host.*$name";
            this.entry1.Name = "entry1";
            this.entry1.IsEditable = true;
            this.entry1.InvisibleChar = '●';
            this.table1.Add(this.entry1);
            global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1[this.entry1]));
            w4.LeftAttach = ((uint)(1));
            w4.RightAttach = ((uint)(2));
            w4.XOptions = ((global::Gtk.AttachOptions)(4));
            w4.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label1 = new global::Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = messages.Localize("[[preferences-highlighter-text]]");
            this.table1.Add(this.label1);
            global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1[this.label1]));
            w5.XOptions = ((global::Gtk.AttachOptions)(4));
            w5.YOptions = ((global::Gtk.AttachOptions)(4));
            w1.Add(this.table1);
            global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(w1[this.table1]));
            w7.Position = 0;
            w7.Expand = false;
            w7.Fill = false;
            // Internal child blah.Ignore.ActionArea
            global::Gtk.HButtonBox w8 = this.ActionArea;
            w8.Name = "dialog1_ActionArea";
            w8.Spacing = 10;
            w8.BorderWidth = ((uint)(5));
            w8.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonCancel = new global::Gtk.Button();
            this.buttonCancel.CanDefault = true;
            this.buttonCancel.CanFocus = true;
            this.buttonCancel.Clicked += new EventHandler(close);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseStock = true;
            this.buttonCancel.UseUnderline = true;
            this.buttonCancel.Label = "gtk-cancel";
            this.AddActionWidget(this.buttonCancel, -6);
            global::Gtk.ButtonBox.ButtonBoxChild w9 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w8[this.buttonCancel]));
            w9.Expand = false;
            w9.Fill = false;
            // Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.buttonOk = new global::Gtk.Button();
            this.buttonOk.CanDefault = true;
            this.buttonOk.CanFocus = true;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseStock = true;
            this.buttonOk.Clicked += new EventHandler(Insert);
            this.buttonOk.UseUnderline = true;
            this.buttonOk.Label = "gtk-ok";
            this.AddActionWidget(this.buttonOk, -5);
            global::Gtk.ButtonBox.ButtonBoxChild w10 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w8[this.buttonOk]));
            w10.Position = 1;
            w10.Expand = false;
            w10.Fill = false;
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
