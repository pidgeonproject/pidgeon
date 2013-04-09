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
    public partial class Preferences : Gtk.Window
    {
        private global::Gtk.Frame fExtensions;
        private global::Gtk.Alignment GtkAlignment2;
        private global::Gtk.ScrolledWindow GtkScrolledWindow2;
        private global::Gtk.TreeView treeview2;
        private global::Gtk.Label GtkLabel2;
        private Gtk.Widget widget = null;
        private global::Gtk.Frame fKeyboard;
        private global::Gtk.Alignment GtkAlignment3;
        private global::Gtk.ScrolledWindow GtkScrolledWindow3;
        private global::Gtk.TreeView treeview3;
        private global::Gtk.Label GtkLabel3;
        private global::Gtk.Frame frame4;
        private global::Gtk.Alignment GtkAlignment4;
        private global::Gtk.ScrolledWindow GtkScrolledWindow4;
        private global::Gtk.TreeView treeview4;
        private global::Gtk.Label GtkLabel4;
        private global::Gtk.Frame fIgnore;
        private global::Gtk.Alignment GtkAlignment5;
        private global::Gtk.ScrolledWindow GtkScrolledWindow5;
        private global::Gtk.TreeView treeview5;
        private global::Gtk.Label GtkLabel5;
        private global::Gtk.Frame frame6;
        private global::Gtk.Alignment GtkAlignment6;
        private global::Gtk.Label GtkLabel6;
        private global::Gtk.Frame frame7;
        private global::Gtk.Alignment GtkAlignment7;
        private global::Gtk.Label GtkLabel7;
        private global::Gtk.Frame fSys;
        private global::Gtk.Alignment GtkAlignment8;
        private global::Gtk.Label GtkLabel8;

        // logs
        private global::Gtk.VBox vbox2;
        private global::Gtk.Table ltable2;
        private global::Gtk.CheckButton lcheckbutton1;
        private global::Gtk.CheckButton lcheckbutton2;
        private global::Gtk.CheckButton lcheckbutton3;
        private global::Gtk.Entry lentry1;
        private global::Gtk.Entry lentry2;
        private global::Gtk.Label llabel1;
        private global::Gtk.Label llabel2;
        private global::Gtk.Frame lframe1;
        private global::Gtk.Alignment lGtkAlignment;
        private global::Gtk.VBox lvbox3;
        private global::Gtk.RadioButton lradiobutton1;
        private global::Gtk.RadioButton lradiobutton2;
        private global::Gtk.RadioButton lradiobutton3;
        private global::Gtk.Label lGtkLabel1;

        // system
        private global::Gtk.CheckButton checkButton_CTCP;
        private global::Gtk.CheckButton checkButton_request;

        private Gtk.ListStore Highlights = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(Network.Highlighter));
        public Gtk.ListStore Extensions = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(string), typeof(Extension));
        private Gtk.ListStore Keyboard = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(Core.Shortcut));
        
        private Gtk.ListStore item = new Gtk.ListStore(typeof(string), typeof(int));
        private GTK.Menu enableToolStripMenuItem = new GTK.Menu();
        private GTK.Menu simpleToolStripMenuItem = new GTK.Menu();
        private GTK.Menu disableToolStripMenuItem = new GTK.Menu();
        private GTK.Menu addToolStripMenuItem1 = new GTK.Menu();
        private GTK.Menu modifyToolStripMenuItem = new GTK.Menu();
        private GTK.Menu deleteToolStripMenuItem = new GTK.Menu();
        private GTK.Menu loadModuleFromFileToolStripMenuItem = new GTK.Menu();
        private GTK.Menu unloadModuleToolStripMenuItem = new GTK.Menu();
        private GTK.Menu deleteToolStripMenuItem1 = new GTK.Menu();
        private GTK.Menu createToolStripMenuItem = new GTK.Menu();
        private GTK.Menu disableToolStripMenuItem1 = new GTK.Menu();
        private GTK.Menu enableToolStripMenuItem1 = new GTK.Menu();
        private GTK.Menu simpleToolStripMenuItem1 = new GTK.Menu();
        private GTK.Menu regexToolStripMenuItem = new GTK.Menu();
        private GTK.Menu matchingTextInWindowToolStripMenuItem = new GTK.Menu();
        private GTK.Menu matchingOnlyUserStringToolStripMenuItem = new GTK.Menu();
        
        public int SelectedItem
        {
            get
            {
                TreeIter iter;
                TreePath[] path = treeview1.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return 0;
                }
                treeview1.Model.GetIter(out iter, path[0]);
                return (int)treeview1.Model.GetValue(iter, 1);
            }
        }

        public void CreateLogs()
        {
            this.vbox2 = new global::Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.ltable2 = new global::Gtk.Table(((uint)(5)), ((uint)(2)), false);
            this.ltable2.Name = "table2";
            this.ltable2.RowSpacing = ((uint)(6));
            this.ltable2.ColumnSpacing = ((uint)(6));
            // Container child table2.Gtk.Table+TableChild
            this.lcheckbutton1 = new global::Gtk.CheckButton();
            this.lcheckbutton1.CanFocus = true;
            this.lcheckbutton1.Name = "checkbutton1";
            this.lcheckbutton1.Label = global::Mono.Unix.Catalog.GetString("TXT logs");
            this.lcheckbutton1.DrawIndicator = true;
            this.lcheckbutton1.UseUnderline = true;
            this.ltable2.Add(this.lcheckbutton1);
            global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.ltable2[this.lcheckbutton1]));
            w1.TopAttach = ((uint)(2));
            w1.BottomAttach = ((uint)(3));
            w1.LeftAttach = ((uint)(1));
            w1.RightAttach = ((uint)(2));
            w1.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.lcheckbutton2 = new global::Gtk.CheckButton();
            this.lcheckbutton2.CanFocus = true;
            this.lcheckbutton2.Name = "checkbutton2";
            this.lcheckbutton2.Label = global::Mono.Unix.Catalog.GetString("HTML logs");
            this.lcheckbutton2.DrawIndicator = true;
            this.lcheckbutton2.UseUnderline = true;
            this.ltable2.Add(this.lcheckbutton2);
            global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.ltable2[this.lcheckbutton2]));
            w2.TopAttach = ((uint)(3));
            w2.BottomAttach = ((uint)(4));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.lcheckbutton3 = new global::Gtk.CheckButton();
            this.lcheckbutton3.CanFocus = true;
            this.lcheckbutton3.Name = "checkbutton3";
            this.lcheckbutton3.Label = global::Mono.Unix.Catalog.GetString("XML logs");
            this.lcheckbutton3.DrawIndicator = true;
            this.lcheckbutton3.UseUnderline = true;
            this.ltable2.Add(this.lcheckbutton3);
            global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.ltable2[this.lcheckbutton3]));
            w3.TopAttach = ((uint)(4));
            w3.BottomAttach = ((uint)(5));
            w3.LeftAttach = ((uint)(1));
            w3.RightAttach = ((uint)(2));
            w3.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.lentry1 = new global::Gtk.Entry();
            this.lentry1.CanFocus = true;
            this.lentry1.Name = "entry1";
            this.lentry1.IsEditable = true;
            this.lentry1.InvisibleChar = '●';
            this.ltable2.Add(this.lentry1);
            global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.ltable2[this.lentry1]));
            w4.TopAttach = ((uint)(1));
            w4.BottomAttach = ((uint)(2));
            w4.LeftAttach = ((uint)(1));
            w4.RightAttach = ((uint)(2));
            w4.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.lentry2 = new global::Gtk.Entry();
            this.lentry2.CanFocus = true;
            this.lentry2.Name = "entry2";
            this.lentry2.IsEditable = true;
            this.lentry2.InvisibleChar = '●';
            this.ltable2.Add(this.lentry2);
            global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.ltable2[this.lentry2]));
            w5.LeftAttach = ((uint)(1));
            w5.RightAttach = ((uint)(2));
            w5.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.llabel1 = new global::Gtk.Label();
            this.llabel1.Name = "label1";
            this.llabel1.LabelProp = global::Mono.Unix.Catalog.GetString("Path:");
            this.ltable2.Add(this.llabel1);
            global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.ltable2[this.llabel1]));
            w6.XOptions = ((global::Gtk.AttachOptions)(4));
            w6.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table2.Gtk.Table+TableChild
            this.llabel2 = new global::Gtk.Label();
            this.llabel2.Name = "label2";
            this.llabel2.LabelProp = global::Mono.Unix.Catalog.GetString("File name:");
            this.ltable2.Add(this.llabel2);
            global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.ltable2[this.llabel2]));
            w7.TopAttach = ((uint)(1));
            w7.BottomAttach = ((uint)(2));
            w7.XOptions = ((global::Gtk.AttachOptions)(4));
            w7.YOptions = ((global::Gtk.AttachOptions)(4));
            this.vbox2.Add(this.ltable2);
            global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.ltable2]));
            w8.Position = 0;
            w8.Expand = false;
            w8.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.lframe1 = new global::Gtk.Frame();
            this.lframe1.Name = "frame1";
            this.lframe1.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.lGtkAlignment = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.lGtkAlignment.Name = "GtkAlignment";
            this.lGtkAlignment.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.lvbox3 = new global::Gtk.VBox();
            this.lvbox3.Name = "vbox3";
            this.lvbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.lradiobutton1 = new global::Gtk.RadioButton(global::Mono.Unix.Catalog.GetString("Enable logs for all text"));
            this.lradiobutton1.CanFocus = true;
            this.lradiobutton1.Name = "radiobutton1";
            this.lradiobutton1.DrawIndicator = true;
            this.lradiobutton1.UseUnderline = true;
            this.lradiobutton1.Group = new global::GLib.SList(global::System.IntPtr.Zero);
            this.lvbox3.Add(this.lradiobutton1);
            global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.lvbox3[this.lradiobutton1]));
            w9.Position = 0;
            w9.Expand = false;
            w9.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.lradiobutton2 = new global::Gtk.RadioButton(global::Mono.Unix.Catalog.GetString("Append"));
            this.lradiobutton2.CanFocus = true;
            this.lradiobutton2.Name = "radiobutton2";
            this.lradiobutton2.DrawIndicator = true;
            this.lradiobutton2.UseUnderline = true;
            this.lradiobutton2.Group = this.lradiobutton1.Group;
            this.lvbox3.Add(this.lradiobutton2);
            global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.lvbox3[this.lradiobutton2]));
            w10.Position = 1;
            w10.Expand = false;
            w10.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.lradiobutton3 = new global::Gtk.RadioButton(global::Mono.Unix.Catalog.GetString("Disable logs"));
            this.lradiobutton3.CanFocus = true;
            this.lradiobutton3.Name = "radiobutton3";
            this.lradiobutton3.DrawIndicator = true;
            this.lradiobutton3.UseUnderline = true;
            this.lradiobutton3.Group = this.lradiobutton1.Group;
            this.lvbox3.Add(this.lradiobutton3);
            global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.lvbox3[this.lradiobutton3]));
            w11.Position = 2;
            w11.Expand = false;
            w11.Fill = false;
            this.lGtkAlignment.Add(this.lvbox3);
            this.lframe1.Add(this.lGtkAlignment);
            this.lGtkLabel1 = new global::Gtk.Label();
            this.lGtkLabel1.Name = "GtkLabel1";
            this.lGtkLabel1.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Services logs</b>");
            this.lGtkLabel1.UseMarkup = true;
            this.lframe1.LabelWidget = this.lGtkLabel1;
            this.vbox2.Add(this.lframe1);
            global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.lframe1]));
            w14.Position = 1;
            w14.Expand = false;
            w14.Fill = false;
            this.GtkAlignment7.Add(this.vbox2);
        }

        public void Initialize()
        {
            this.fExtensions = new global::Gtk.Frame ();
            this.fExtensions.Name = "frame1";
            this.fExtensions.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
            this.GtkAlignment2.Name = "GtkAlignment";
            this.GtkAlignment2.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
            this.GtkScrolledWindow2.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeview2 = new global::Gtk.TreeView ();
            this.treeview2.CanFocus = true;
            this.treeview2.Name = "treeview1";
            Gtk.TreeViewColumn el_1 = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn el_2 = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn el_3 = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn el_4 = new Gtk.TreeViewColumn();
            Gtk.CellRendererText exr1 = new Gtk.CellRendererText();
            Gtk.CellRendererText exr2 = new Gtk.CellRendererText();
            Gtk.CellRendererText exr3 = new Gtk.CellRendererText();
            Gtk.CellRendererText exr4 = new Gtk.CellRendererText();
            el_1.PackStart(exr1, true);
            el_1.Title = "Name";
            el_2.PackStart(exr2, true);
            el_2.Title = "Version";
            el_3.PackStart(exr3, true);
            el_3.Title = "Description";
            el_4.PackStart(exr4, true);
            el_4.Title = "Type";
            this.treeview2.AppendColumn(el_1);
            this.treeview2.AppendColumn(el_2);
            this.treeview2.AppendColumn(el_3);
            this.treeview2.AppendColumn(el_4);
            el_1.AddAttribute(exr1, "text", 0);
            el_2.AddAttribute(exr2, "text", 1);
            el_3.AddAttribute(exr3, "text", 2);
            el_4.AddAttribute(exr4, "text", 2);
            this.treeview5.AppendColumn(el_1);
            this.treeview5.AppendColumn(el_2);
            this.treeview5.AppendColumn(el_3);
            this.treeview5.AppendColumn(el_4);
            this.GtkScrolledWindow2.Add (this.treeview2);
            this.GtkAlignment2.Add (this.GtkScrolledWindow2);
            this.fExtensions.Add (this.GtkAlignment2);
            this.GtkLabel2 = new global::Gtk.Label ();
            this.GtkLabel2.Name = "GtkLabel";
            this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Extensions</b>");
            this.GtkLabel2.UseMarkup = true;
            this.fExtensions.LabelWidget = this.GtkLabel2;
            this.fExtensions.ShowAll();
            
            this.frame4 = new global::Gtk.Frame ();
            this.frame4.Name = "frame1";
            this.frame4.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment4 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
            this.GtkAlignment4.Name = "GtkAlignment";
            this.GtkAlignment4.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.GtkScrolledWindow4 = new global::Gtk.ScrolledWindow ();
            this.GtkScrolledWindow4.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow4.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeview4 = new global::Gtk.TreeView ();
            this.treeview4.CanFocus = true;
            this.treeview4.Name = "treeview1";
            this.GtkScrolledWindow4.Add (this.treeview4);
            this.GtkAlignment4.Add (this.GtkScrolledWindow4);
            treeview4.Model = Highlights;
            this.frame4.Add (this.GtkAlignment4);
            Gtk.TreeViewColumn highlight_text = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn highlight_type = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn highlight_stat = new Gtk.TreeViewColumn();
            Gtk.CellRendererText r1 = new Gtk.CellRendererText();
            Gtk.CellRendererText r2 = new Gtk.CellRendererText();
            Gtk.CellRendererText r3 = new Gtk.CellRendererText();
            highlight_text.PackStart(r1, true);
            highlight_text.Title = "Text";
            highlight_type.Title = "Regular expression";
            highlight_type.PackStart(r2, true);
            highlight_stat.PackStart(r3, true);
            highlight_stat.Title = "Highlight enabled";
            this.treeview4.AppendColumn(highlight_text);
            highlight_text.AddAttribute(r1, "text", 0);
            highlight_type.AddAttribute(r2, "text", 1);
            highlight_stat.AddAttribute(r3, "text", 2);
            this.treeview4.AppendColumn(highlight_type);
            this.treeview4.AppendColumn(highlight_stat);
            this.GtkLabel4 = new global::Gtk.Label ();
            this.GtkLabel4.Name = "GtkLabel";
            this.GtkLabel4.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Highlighting</b>");
            this.GtkLabel4.UseMarkup = true;
            this.frame4.LabelWidget = this.GtkLabel4;
            this.frame4.ShowAll();
            
            this.fKeyboard = new global::Gtk.Frame ();
            this.fKeyboard.Name = "frame1";
            this.fKeyboard.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment3 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
            this.GtkAlignment3.Name = "GtkAlignment";
            this.GtkAlignment3.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.GtkScrolledWindow3 = new global::Gtk.ScrolledWindow ();
            this.GtkScrolledWindow3.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow3.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeview3 = new global::Gtk.TreeView ();
            this.treeview3.CanFocus = true;
            this.treeview3.Name = "treeview1";
            this.treeview3.Model = Keyboard;
            Gtk.TreeViewColumn keyboard_keys = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn keyboard_line = new Gtk.TreeViewColumn();
            Gtk.CellRendererText kr1 = new Gtk.CellRendererText();
            Gtk.CellRendererText kr2 = new Gtk.CellRendererText();
            keyboard_keys.PackStart(kr1, true);
            keyboard_keys.Title = "Shortcut";
            keyboard_line.Title = "Command";
            keyboard_line.PackStart(kr2, true);
            this.treeview3.AppendColumn(keyboard_keys);
            keyboard_keys.AddAttribute(kr1, "text", 0);
            keyboard_line.AddAttribute(kr2, "text", 1);
            this.treeview3.AppendColumn(keyboard_line);
            this.GtkScrolledWindow3.Add (this.treeview3);
            this.GtkAlignment3.Add (this.GtkScrolledWindow3);
            this.fKeyboard.Add (this.GtkAlignment3);
            this.GtkLabel3 = new global::Gtk.Label ();
            this.GtkLabel3.Name = "GtkLabel";
            this.GtkLabel3.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Keyboard</b>");
            this.GtkLabel3.UseMarkup = true;
            this.fKeyboard.LabelWidget = this.GtkLabel3;
            this.fKeyboard.ShowAll();
            
            this.fIgnore = new global::Gtk.Frame ();
            this.fIgnore.Name = "frame1";
            this.fIgnore.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment5 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
            this.GtkAlignment5.Name = "GtkAlignment";
            this.GtkAlignment5.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.GtkScrolledWindow5 = new global::Gtk.ScrolledWindow ();
            this.GtkScrolledWindow5.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow5.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeview5 = new global::Gtk.TreeView ();
            this.treeview5.CanFocus = true;
            this.treeview5.Name = "treeview1";
            Gtk.TreeViewColumn list_1= new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn list_2 = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn list_3 = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn list_4 = new Gtk.TreeViewColumn();
            Gtk.CellRendererText lr1 = new Gtk.CellRendererText();
            Gtk.CellRendererText lr2 = new Gtk.CellRendererText();
            Gtk.CellRendererText lr3 = new Gtk.CellRendererText();
            Gtk.CellRendererText lr4 = new Gtk.CellRendererText();
            list_1.PackStart(lr1, true);
            list_1.Title = "Ignore";
            list_2.PackStart(lr2, true);
            list_2.Title = "Simple";
            list_3.PackStart(lr3, true);
            list_3.Title = "Enabled";
            list_4.PackStart(lr4, true);
            list_4.Title = "Type";
            this.treeview5.AppendColumn(list_1);
            this.treeview5.AppendColumn(list_2);
            this.treeview5.AppendColumn(list_3);
            this.treeview5.AppendColumn(list_4);
            list_1.AddAttribute(lr1, "text", 0);
            list_2.AddAttribute(lr2, "text", 1);
            list_3.AddAttribute(lr3, "text", 2);
            list_4.AddAttribute(lr4, "text", 2);
            this.treeview5.AppendColumn(list_1);
            this.treeview5.AppendColumn(list_2);
            this.treeview5.AppendColumn(list_4);
            this.treeview5.AppendColumn(list_3);
            this.GtkScrolledWindow5.Add (this.treeview5);
            this.GtkAlignment5.Add (this.GtkScrolledWindow5);
            this.fIgnore.Add (this.GtkAlignment5);
            this.GtkLabel5 = new global::Gtk.Label ();
            this.GtkLabel5.Name = "GtkLabel";
            this.GtkLabel5.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Ignore list</b>");
            this.GtkLabel5.UseMarkup = true;
            this.fIgnore.LabelWidget = this.GtkLabel5;
            this.fIgnore.ShowAll();
            
            this.frame6 = new global::Gtk.Frame ();
            this.frame6.Name = "frame1";
            this.frame6.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment6 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
            this.GtkAlignment6.Name = "GtkAlignment";
            this.GtkAlignment6.LeftPadding = ((uint)(12));
            VBox vb01 = new VBox();
            vb01.Name = "vb01";
            vb01.Spacing = 2;
            this.checkButton_request = new CheckButton();
            this.checkButton_request.CanFocus = true;
            this.checkButton_request.Name = "request";
            this.checkButton_request.Label = "Request a confirmation for every system generated kickban";
            this.checkButton_request.DrawIndicator = true;
            this.checkButton_request.UseUnderline = true;
            this.checkButton_CTCP = new CheckButton();
            this.checkButton_CTCP.CanFocus = true;
            this.checkButton_CTCP.Name = "ctcp";
            this.checkButton_CTCP.Label = "Display CTCP";
            this.checkButton_CTCP.DrawIndicator = true;
            this.checkButton_CTCP.UseUnderline = true;
            vb01.Add(checkButton_CTCP);
            global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(vb01[this.checkButton_CTCP]));
            w20.Position = 1;
            w20.Expand = false;
            w20.Fill = false;
            vb01.Add(checkButton_request);
            global::Gtk.Box.BoxChild w60 = ((global::Gtk.Box.BoxChild)(vb01[this.checkButton_request]));
            w60.Position = 1;
            w60.Expand = false;
            w60.Fill = false;
            GtkAlignment6.Add(vb01);
            this.frame6.Add(this.GtkAlignment6);
            this.GtkLabel6 = new global::Gtk.Label();
            this.GtkLabel6.Name = "GtkLabel";
            this.GtkLabel6.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>System</b>");
            this.GtkLabel6.UseMarkup = true;
            this.frame6.LabelWidget = this.GtkLabel6;
            this.frame6.ShowAll();
            
            this.frame7 = new global::Gtk.Frame ();
            this.frame7.Name = "frame1";
            this.frame7.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment7 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
            this.GtkAlignment7.Name = "GtkAlignment";
            this.GtkAlignment7.LeftPadding = ((uint)(12));
            CreateLogs();
            //this.GtkAlignment5.Add (this.GtkScrolledWindow5);
            this.frame7.Add (this.GtkAlignment7);
            this.GtkLabel7 = new global::Gtk.Label ();
            this.GtkLabel7.Name = "GtkLabel";
            this.GtkLabel7.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Logs</b>");
            this.GtkLabel7.UseMarkup = true;
            this.frame7.LabelWidget = this.GtkLabel7;
            this.frame7.ShowAll();

            this.fSys = new global::Gtk.Frame();
            this.fSys.Name = "frame1";
            this.fSys.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment8 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment8.Name = "GtkAlignment";
            this.GtkAlignment8.LeftPadding = ((uint)(12));
            //this.GtkAlignment5.Add (this.GtkScrolledWindow5);
            this.fSys.Add(this.GtkAlignment8);
            this.GtkLabel8 = new global::Gtk.Label();
            this.GtkLabel8.Name = "GtkLabel";
            this.GtkLabel8.LabelProp = global::Mono.Unix.Catalog.GetString("<b>System</b>");
            this.GtkLabel8.UseMarkup = true;
            this.fSys.LabelWidget = this.GtkLabel8;
            this.fSys.ShowAll();
        }
        
        public Preferences() : base(Gtk.WindowType.Toplevel)
        {
            try
            {
                this.Build();
                this.widget = frame1;
                this.Initialize();
                this.Preferences_Load();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
        
        /// <summary>
        /// Prepare the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Preferences_Load()
        {
            checkbutton2.Active = Configuration.Kernel.CheckUpdate;
            messages.Localize(this);
            if (Configuration.CurrentPlatform != Core.Platform.Windowsx86 && Configuration.CurrentPlatform != Core.Platform.Windowsx64)
            {
                checkbutton2.Active = false;
                checkbutton2.Sensitive = false;
            }
            messages.Localize(this);
            this.DeleteEvent += new DeleteEventHandler(hide);
            button1.Clicked += new EventHandler(bCancel_Click);
            button3.Clicked += new EventHandler(bSave_Click);
            entry1.Text = Configuration.UserData.nick;
            this.treeview1.ButtonPressEvent += new ButtonPressEventHandler(s1);
            entry2.Text = Configuration.UserData.quit;
            entry3.Text = Configuration.UserData.ident;
            entry4.Text = Configuration.UserData.user;
            entry5.Text = Configuration.UserData.Nick2;
            button3.Clicked += new EventHandler(bCancel_Click);
            this.treeview1.Model = item;
            this.treeview1.CursorChanged += new EventHandler(s4);
            Gtk.TreeViewColumn topic_item = new Gtk.TreeViewColumn();
            Gtk.CellRendererText c1 = new Gtk.CellRendererText();
            topic_item.Title = "Option";
            topic_item.PackStart(c1, true);
            topic_item.AddAttribute    (c1, "text", 0);
            this.treeview1.AppendColumn(topic_item);
            this.treeview1.RowActivated += new RowActivatedHandler(s2);
            this.lcheckbutton3.Active = Configuration.Logs.logs_xml;
            this.lcheckbutton1.Active = Configuration.Logs.logs_txt;
            this.lcheckbutton2.Active = Configuration.Logs.logs_html;
            this.checkButton_CTCP.Active = Configuration.irc.DisplayCtcp;
            this.checkButton_request.Active = Configuration.irc.ConfirmAll;
            checkbutton1.Active = Configuration.Kernel.Notice;
            lentry2.Text = Configuration.Logs.logs_dir;
            lentry1.Text = Configuration.Logs.logs_name;
            switch (Configuration.Logs.ServicesLogs)
            {
                case Configuration.Logs.ServiceLogs.full:
                    lradiobutton1.Active = true;
                    break;
                case Configuration.Logs.ServiceLogs.incremental:
                    lradiobutton2.Active = true;
                    break;
                case Configuration.Logs.ServiceLogs.none:
                    lradiobutton3.Active = true;
                    break;
            }

            ListStore lg = new ListStore(typeof(string));

            combobox2.Model = lg;

            foreach (Skin skin in Configuration.SL)
            {
            //    comboBox1.Items.Add(skin.name);
            }
            //comboBox1.SelectedIndex = 0;

            int selectedLanguage = 0;
            int current = 0;
            foreach (KeyValuePair<string, messages.container> language in messages.data)
            {
                if (language.Key == Core.SelectedLanguage)
                {
                    selectedLanguage = current;
                }
                lg.AppendValues(language.Key);
                current++;
            }
            combobox2.Active = selectedLanguage;

            foreach (Network.Highlighter highlight in Configuration.HighlighterList)
            {
                Highlights.AppendValues(highlight.text, (!highlight.simple).ToString (), highlight.enabled.ToString(), highlight);
            }
            
            item.AppendValues("IRC", 1);
            item.AppendValues("System", 2);
            item.AppendValues("Logs", 3);
            //item.AppendValues("Network", 4);
            item.AppendValues("Highlighting", 5);
            item.AppendValues("Ignore list", 6);
            item.AppendValues("Keyboard", 7);
            item.AppendValues("Extensions", 8);
            RefreshModules();
            ReloadIgnores();
            redrawS();
        }

        private void setWindow(Gtk.Frame frame)
        {
            this.hbox1.Remove (widget);
            widget = frame;
            this.hbox1.Add(widget);
        }
        
        
        [GLib.ConnectBefore]
        private void s1(object sender, ButtonPressEventArgs e)
        {
            if (e.Event.Button == 1)
            {
                Switch();
            }
        }
        
        [GLib.ConnectBefore]
        private void s4(object sender, EventArgs e)
        {
            Switch();
        }
        
        private void Switch()
        {
            try
            {
                switch (SelectedItem)
                {
                    case 1:
                        setWindow(frame1);
                        break;
                    case 2:
                        setWindow(fSys);
                        break;
                    case 3:
                        setWindow(frame7);
                        break;
                    case 5:
                        setWindow(frame4);
                        break;
                    case 6:
                        setWindow(fIgnore);
                        break;
                    case 7:
                        setWindow(fKeyboard);
                        break;
                    case 8:
                        setWindow(fExtensions);
                        break;
                }
            } catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
        
        [GLib.ConnectBefore]
        private void s2(object sender, RowActivatedArgs e)
        {
            Switch();
        }
        
        public void RefreshModules()
        {
            //listView3.Items.Clear();
            foreach (Extension ex in Core.Extensions)
            {
                //ListViewItem item = new ListViewItem();
                //item.Text = ex.Name;
                //item.SubItems.Add(ex.Version);
                //item.SubItems.Add(ex.Description);
                //item.SubItems.Add("Extension");
                //listView3.Items.Add(item);
            }
        }

        public void redrawS()
        {
            //listView2.Items.Clear();
            foreach (Core.Shortcut s in Configuration.ShortcutKeylist)
            {
                //ListViewItem item = new ListViewItem();
                string keys = "";
                if (s.control)
                {
                    keys += "ctrl + ";
                }
                if (s.alt)
                {
                    keys += "alt + ";
                }
                if (s.shift)
                {
                    keys += "shift + ";
                }
                keys += s.keys.ToString();
                //item.Text = keys;
                //item.SubItems.Add(s.data);
                //listView2.Items.Add(item);
            }
        }

        private void lquit_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void hide(object x, DeleteEventArgs e)
        {
            e.RetVal = true;
            Hide();
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            try
            {
                lock (Configuration.HighlighterList)
                {
                    //Configuration.HighlighterList.Clear();
                    //foreach (ListViewItem curr in list.Items)
                    {
                       // Network.Highlighter hl = new Network.Highlighter();
                       // hl.enabled = bool.Parse(curr.SubItems[1].Text);
                       // hl.text = curr.Text;
                       /// hl.simple = bool.Parse(curr.SubItems[2].Text);
                       // Configuration.HighlighterList.Add(hl);
                    }
                }

                //Configuration.irc.ConfirmAll = checkBox8.Checked;
                Configuration.UserData.nick = entry1.Text;
                Configuration.UserData.quit = entry2.Text;
                Configuration.UserData.ident = entry3.Text;
                Configuration.UserData.user = entry4.Text;
                Configuration.Logs.logs_xml = lcheckbutton3.Active;
                Configuration.Logs.logs_txt = lcheckbutton1.Active;
                Configuration.Logs.logs_html = lcheckbutton2.Active;
                Configuration.Kernel.CheckUpdate = checkbutton2.Active;
                Configuration.irc.DisplayCtcp = checkButton_CTCP.Active;
                Configuration.Kernel.Notice = checkbutton1.Active;
                Configuration.Logs.logs_dir = lentry2.Text;
                Configuration.Logs.logs_name = lentry1.Text;
                Configuration.UserData.Nick2 = entry5.Text;
                if (lradiobutton1.Active)
                {
                    Configuration.Logs.ServicesLogs = Configuration.Logs.ServiceLogs.full;
                }
                if (lradiobutton2.Active)
                {
                    Configuration.Logs.ServicesLogs = Configuration.Logs.ServiceLogs.incremental;
                }
                if (lradiobutton3.Active)
                {
                    Configuration.Logs.ServicesLogs = Configuration.Logs.ServiceLogs.none;
                }
                lock (Ignoring.IgnoreList)
                {
                    //Ignoring.IgnoreList.Clear();
                    //foreach (ListViewItem curr in listView4.Items)
                    {
                    //    Ignoring.Ignore.Type type = Ignoring.Ignore.Type.Everything;
                    //    if (curr.SubItems[3].Text == "User")
                    //    {
                    //        type = Ignoring.Ignore.Type.User;
                    //    }
                    //    Ignoring.Ignore x = new Ignoring.Ignore(bool.Parse(curr.SubItems[2].Text), bool.Parse(curr.SubItems[1].Text), curr.Text, type);
                    //    Ignoring.IgnoreList.Add(x);
                    }
                }
                //if (Configuration.SL.Count >= comboBox1.SelectedIndex)
                {
                    //Configuration.CurrentSkin = Configuration.SL[comboBox1.SelectedIndex];
                }
                Core._Configuration.ConfigSave();
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
            Hide();
        }

        /// <summary>
        /// User wants to discard changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void ReloadIgnores()
        {
            lock (Ignoring.IgnoreList)
            {
                //listView4.Items.Clear();
                foreach (Ignoring.Ignore curr in Ignoring.IgnoreList)
                {
                //    ListViewItem item = new ListViewItem();
                //    item.Text = curr.Text;
                //    item.SubItems.Add(curr.Simple.ToString());
                //    item.SubItems.Add(curr.Enabled.ToString());
                //    item.SubItems.Add(curr.type.ToString());
                //    listView4.Items.Add(item);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //if (listView1.SelectedItems.Count != 1) return;
                
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //if (list.SelectedItems.Count > 0)
                {
                //    foreach (ListViewItem curr in list.SelectedItems)
                    {
                //        list.Items.Remove(curr);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //ListViewItem item = new ListViewItem();
                //item.Text = "$nick!$ident@$host.*$name";
                //item.SubItems.Add("true");
                //item.SubItems.Add("true");
                //list.Items.Add(item);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in list.SelectedItems)
            {
            //    curr.SubItems[1].Text = "true";
            }
        }

        private void simpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in list.SelectedItems)
            {
            //    if (curr.SubItems[2].Text == "false")
            //    {
            //        curr.SubItems[2].Text = "true";
            //    }
            //    else
            //    {
            //        curr.SubItems[2].Text = "false";
            //    }
            }

        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in list.SelectedItems)
            //{
            //    curr.SubItems[1].Text = "false";
            //}
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //ShortcutBox sb = new ShortcutBox();
            //sb.Show();
            //sb.Left = this.Left + (this.Width / 2);
            //sb.config = this;
            //sb.Top = this.Top + (this.Height / 2);

        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (listView2.SelectedItems.Count > 0)
            //{
            //    ShortcutBox sb = new ShortcutBox();
            //    sb.Show();
            //    sb.Left = this.Left + (this.Width / 2);
            //    sb.config = this;
            //    sb.Top = this.Top + (this.Height / 2);
            //}
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (listView2.SelectedItems.Count > 0)
            //{
            //    foreach (ListViewItem curr in listView2.SelectedItems)
            //    {
            //        if (Configuration.ShortcutKeylist.Count > curr.Index)
            //        {
            //            Configuration.ShortcutKeylist.RemoveAt(curr.Index);
            //        }
            //        redrawS();
            //    }
            //}
        }

        private void loadModuleFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //if (openFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                //{
                //    if (openFileDialog1.FileName != "")
                //    {
                //        Core.RegisterPlugin(openFileDialog1.FileName);
                //    }
                //    else
                //    {
                //        Core.DebugLog("Preferences: provided invalid file name");
                //    }
                //}
                RefreshModules();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void unloadModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (listView3.SelectedItems != null && listView3.SelectedItems[0] != null)
            //{
            //    string name = listView3.SelectedItems[0].Text;
            //}
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                //foreach (ListViewItem x in listView4.SelectedItems)
                //{
                //    listView4.Items.Remove(x);
                //}
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //ListViewItem item = new ListViewItem();
                //item.Text = "someone_bad";
                //item.SubItems.Add("true");
                //item.SubItems.Add("false");
                //item.SubItems.Add("User");
                //listView4.Items.Add(item);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void disableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in listView4.SelectedItems)
            //{
            //    curr.SubItems[2].Text = "false";
            //}
        }

        private void enableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in listView4.SelectedItems)
            //{
            //    curr.SubItems[2].Text = "true";
            //}
        }

        private void simpleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in listView4.SelectedItems)
            //{
            //    curr.SubItems[1].Text = "true";
            //}
        }

        private void regexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in listView4.SelectedItems)
            //{
            //    curr.SubItems[1].Text = "false";
            //}
        }

        private void matchingOnlyUserStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in listView4.SelectedItems)
            //{
            //    curr.SubItems[3].Text = "User";
            //}
        }

        private void matchingTextInWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //foreach (ListViewItem curr in listView4.SelectedItems)
            //{
            //    curr.SubItems[3].Text = "Everything";
            //}
        }
    }
}

