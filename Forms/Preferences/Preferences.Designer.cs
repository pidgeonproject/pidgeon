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
        private global::Gtk.TreeView treeviewEx;
        private global::Gtk.Label GtkLabel2;
        private Gtk.Widget widget = null;
        private global::Gtk.Frame fKeyboard;
        private global::Gtk.Alignment GtkAlignment3;
        private global::Gtk.ScrolledWindow GtkScrolledWindow3;
        private global::Gtk.TreeView treeviewSh;
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
        private Gtk.ListStore IgnoreDB = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(string), typeof(Ignoring.Ignore));
        public Gtk.ListStore Extensions = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(string), typeof(Extension));
        private Gtk.ListStore Keyboard = new Gtk.ListStore(typeof(string), typeof(string), typeof(Core.Shortcut));

        private Gtk.ListStore item = new Gtk.ListStore(typeof(string), typeof(int));
        private GTK.Menu enableToolStripMenuItem = new GTK.Menu("Enable");
        private GTK.Menu simpleToolStripMenuItem = new GTK.Menu("Simple");
        private GTK.Menu disableToolStripMenuItem = new GTK.Menu("Disable");
        private GTK.Menu addToolStripMenuItem1 = new GTK.Menu("Add");
        private GTK.Menu addToolStripMenuItem = new GTK.Menu("Add");
        private GTK.Menu addToolStripMenuItem2 = new GTK.Menu("Add");
        private GTK.Menu modifyToolStripMenuItem = new GTK.Menu("Modify");
        private GTK.Menu deleteToolStripMenuItem = new GTK.Menu("Delete");
        private GTK.Menu loadModuleFromFileToolStripMenuItem = new GTK.Menu("Load from a file");
        private GTK.Menu unloadModuleToolStripMenuItem = new GTK.Menu("Unload");
        private GTK.Menu deleteToolStripMenuItem1 = new GTK.Menu("Delete");
        private GTK.Menu deleteToolStripMenuItem2 = new GTK.Menu("Delete");
        private GTK.Menu createToolStripMenuItem = new GTK.Menu("Create");
        private GTK.Menu disableToolStripMenuItem1 = new GTK.Menu("Disable");
        private GTK.Menu enableToolStripMenuItem1 = new GTK.Menu("Enable");
        private GTK.Menu simpleToolStripMenuItem1 = new GTK.Menu("Simple");
        private GTK.Menu regexToolStripMenuItem = new GTK.Menu("Regex");
        private GTK.Menu matchingTextInWindowToolStripMenuItem = new GTK.Menu("Matching text");
        private GTK.Menu matchingOnlyUserStringToolStripMenuItem = new GTK.Menu("Matching user");

        public List<Extension> SelectedExtensions
        {
            get
            {
                List<Extension> list = new List<Extension>();
                TreeIter iter;
                TreePath[] path = treeviewEx.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return list;
                }
                foreach (TreePath currentPath in path)
                {
                    treeviewEx.Model.GetIter(out iter, currentPath);
                    list.Add((Extension)treeviewEx.Model.GetValue(iter, 4));
                }
                return list;
            }
        }

        public List<Network.Highlighter> SelectedHs
        {
            get
            {
                List<Network.Highlighter> list = new List<Network.Highlighter>();
                TreeIter iter;
                TreePath[] path = treeview4.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return list;
                }
                foreach (TreePath currentPath in path)
                {
                    treeview4.Model.GetIter(out iter, currentPath);
                    list.Add((Network.Highlighter)treeview4.Model.GetValue(iter, 3));
                }
                return list;
            }
        }

        public List<Ignoring.Ignore> SelectedIgnore
        {
            get
            {
                List<Ignoring.Ignore> list = new List<Ignoring.Ignore>();
                TreeIter iter;
                TreePath[] path = treeview5.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return list;
                }
                foreach (TreePath currentPath in path)
                {
                    treeview5.Model.GetIter(out iter, currentPath);
                    list.Add((Ignoring.Ignore)treeview5.Model.GetValue(iter, 4));
                }
                return list;
            }
        }

        public List<Core.Shortcut> SelectedShorts
        {
            get
            {
                List<Core.Shortcut> list = new List<Core.Shortcut>();
                TreeIter iter;
                TreePath[] path = treeviewSh.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return list;
                }
                foreach (TreePath currentPath in path)
                {
                    treeviewSh.Model.GetIter(out iter, currentPath);
                    list.Add((Core.Shortcut)treeviewSh.Model.GetValue(iter, 2));
                }
                return list;
            }
        }

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

        [GLib.ConnectBefore]
        private void ExtensionMenu(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                EM(sender, null);
            }
        }

        [GLib.ConnectBefore]
        private void HlMenu(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                HM(sender, null);
            }
        }

        [GLib.ConnectBefore]
        private void ShMenu(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                SM(sender, null);
            }
        }

        [GLib.ConnectBefore]
        private void IgMenu(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                IM(sender, null);
            }
        }

        /// <summary>
        /// Extensions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EM(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                Gtk.Menu menu = new Menu();

                MenuItem load = new MenuItem(loadModuleFromFileToolStripMenuItem.Text);
                load.Activated += new EventHandler(loadModuleFromFileToolStripMenuItem_Click);
                menu.Append(load);

                MenuItem remove = new MenuItem(disableToolStripMenuItem.Text);
                remove.Activated += new EventHandler(disableToolStripMenuItem_Click);
                menu.Append(remove);

                menu.ShowAll();
                menu.Popup();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Highlights
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HM(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                Gtk.Menu menu = new Menu();

                Gtk.MenuItem make = new MenuItem(addToolStripMenuItem2.Text);

                menu.Append(make);

                Gtk.MenuItem remove = new MenuItem(deleteToolStripMenuItem2.Text);
                remove.Activated += new EventHandler(deleteToolStripMenuItem2_Click);
                menu.Append(remove);

                menu.Append(new Gtk.SeparatorMenuItem());

                Gtk.MenuItem m8 = new MenuItem(enableToolStripMenuItem.Text);
                m8.Activated += new EventHandler(enableToolStripMenuItem1_Click);
                menu.Append(m8);

                Gtk.MenuItem m7 = new MenuItem(disableToolStripMenuItem.Text);
                m7.Activated += new EventHandler(disableToolStripMenuItem1_Click);
                menu.Append(m7);

                menu.ShowAll();
                menu.Popup();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Ignores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IM(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                Gtk.Menu menu = new Menu();

                Gtk.MenuItem m3 = new MenuItem(addToolStripMenuItem1.Text);
                m3.Activated += new EventHandler(addToolStripMenuItem_Click);
                menu.Append(m3);

                Gtk.MenuItem m4 = new MenuItem(deleteToolStripMenuItem1.Text);
                m4.Activated += new EventHandler(deleteToolStripMenuItem1_Click);
                menu.Append(m4);

                menu.Append(new Gtk.SeparatorMenuItem());

                Gtk.MenuItem m5 = new MenuItem(simpleToolStripMenuItem1.Text);
                m5.Activated += new EventHandler(simpleToolStripMenuItem1_Click);
                menu.Append(m5);

                Gtk.MenuItem m6 = new MenuItem(regexToolStripMenuItem.Text);
                m6.Activated += new EventHandler(regexToolStripMenuItem_Click);
                menu.Append(m6);

                menu.Append(new Gtk.SeparatorMenuItem());

                Gtk.MenuItem m1 = new MenuItem(matchingOnlyUserStringToolStripMenuItem.Text);
                m1.Activated += new EventHandler(matchingOnlyUserStringToolStripMenuItem_Click);
                menu.Append(m1);

                Gtk.MenuItem m2 = new MenuItem(matchingTextInWindowToolStripMenuItem.Text);
                m2.Activated += new EventHandler(matchingTextInWindowToolStripMenuItem_Click);
                menu.Append(m2);

                menu.Append(new Gtk.SeparatorMenuItem());

                Gtk.MenuItem m8 = new MenuItem(enableToolStripMenuItem.Text);
                m8.Activated += new EventHandler(enableToolStripMenuItem_Click);
                menu.Append(m8);

                Gtk.MenuItem m7 = new MenuItem(disableToolStripMenuItem.Text);
                m7.Activated += new EventHandler(disableToolStripMenuItem_Click);
                menu.Append(m7);

                menu.ShowAll();
                menu.Popup();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SM(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                Gtk.Menu menu = new Menu();

                Gtk.MenuItem make = new MenuItem(addToolStripMenuItem.Text);

                menu.Append(make);

                Gtk.MenuItem remove = new MenuItem(deleteToolStripMenuItem.Text);
                remove.Activated += new EventHandler(deleteToolStripMenuItem_Click);
                menu.Append(remove);

                menu.ShowAll();
                menu.Popup();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
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
            this.fExtensions = new global::Gtk.Frame();
            this.fExtensions.Name = "frame1";
            this.fExtensions.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment2 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment2.Name = "GtkAlignment";
            this.GtkAlignment2.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow2.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeviewEx = new global::Gtk.TreeView();
            this.treeviewEx.CanFocus = true;
            this.treeviewEx.Name = "treeview1";
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
            this.treeviewEx.AppendColumn(el_1);
            this.treeviewEx.AppendColumn(el_2);
            this.treeviewEx.AppendColumn(el_3);
            this.treeviewEx.AppendColumn(el_4);
            el_1.AddAttribute(exr1, "text", 0);
            el_2.AddAttribute(exr2, "text", 1);
            el_3.AddAttribute(exr3, "text", 2);
            el_4.AddAttribute(exr4, "text", 3);
            this.treeviewEx.AppendColumn(el_1);
            this.treeviewEx.AppendColumn(el_2);
            this.treeviewEx.AppendColumn(el_3);
            this.treeviewEx.AppendColumn(el_4);
            this.treeviewEx.Model = Extensions;
            this.treeviewEx.Selection.Mode = SelectionMode.Multiple;
            this.treeviewEx.PopupMenu += new PopupMenuHandler(EM);
            this.treeviewEx.ButtonPressEvent += new ButtonPressEventHandler(ExtensionMenu);
            this.GtkScrolledWindow2.Add(this.treeviewEx);
            this.GtkAlignment2.Add(this.GtkScrolledWindow2);
            this.fExtensions.Add(this.GtkAlignment2);
            this.GtkLabel2 = new global::Gtk.Label();
            this.GtkLabel2.Name = "GtkLabel";
            this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Extensions</b>");
            this.GtkLabel2.UseMarkup = true;
            this.fExtensions.LabelWidget = this.GtkLabel2;
            this.fExtensions.ShowAll();

            this.frame4 = new global::Gtk.Frame();
            this.frame4.Name = "frame1";
            this.frame4.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment4 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment4.Name = "GtkAlignment";
            this.GtkAlignment4.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.GtkScrolledWindow4 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow4.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow4.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeview4 = new global::Gtk.TreeView();
            this.treeview4.CanFocus = true;
            this.treeview4.Name = "treeview1";
            this.treeview4.Selection.Mode = SelectionMode.Multiple;
            this.treeview4.PopupMenu += new PopupMenuHandler(HM);
            this.treeview4.ButtonPressEvent += new ButtonPressEventHandler(HlMenu);
            this.GtkScrolledWindow4.Add(this.treeview4);
            this.GtkAlignment4.Add(this.GtkScrolledWindow4);
            treeview4.Model = Highlights;
            this.frame4.Add(this.GtkAlignment4);
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
            this.GtkLabel4 = new global::Gtk.Label();
            this.GtkLabel4.Name = "GtkLabel";
            this.GtkLabel4.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Highlighting</b>");
            this.GtkLabel4.UseMarkup = true;
            this.frame4.LabelWidget = this.GtkLabel4;
            this.frame4.ShowAll();

            this.fKeyboard = new global::Gtk.Frame();
            this.fKeyboard.Name = "frame1";
            this.fKeyboard.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment3 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment3.Name = "GtkAlignment";
            this.GtkAlignment3.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.GtkScrolledWindow3 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow3.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow3.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeviewSh = new global::Gtk.TreeView();
            this.treeviewSh.CanFocus = true;
            this.treeviewSh.Name = "treeview1";
            this.treeviewSh.Model = Keyboard;
            Gtk.TreeViewColumn keyboard_keys = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn keyboard_line = new Gtk.TreeViewColumn();
            Gtk.CellRendererText kr1 = new Gtk.CellRendererText();
            Gtk.CellRendererText kr2 = new Gtk.CellRendererText();
            keyboard_keys.PackStart(kr1, true);
            keyboard_keys.Title = "Shortcut";
            keyboard_line.Title = "Command";
            keyboard_line.PackStart(kr2, true);
            this.treeviewSh.Selection.Mode = SelectionMode.Multiple;
            this.treeviewSh.PopupMenu += new PopupMenuHandler(SM);
            this.treeviewSh.ButtonPressEvent += new ButtonPressEventHandler(ShMenu);
            this.treeviewSh.AppendColumn(keyboard_keys);
            keyboard_keys.AddAttribute(kr1, "text", 0);
            keyboard_line.AddAttribute(kr2, "text", 1);
            this.treeviewSh.AppendColumn(keyboard_line);
            this.GtkScrolledWindow3.Add(this.treeviewSh);
            this.GtkAlignment3.Add(this.GtkScrolledWindow3);
            this.fKeyboard.Add(this.GtkAlignment3);
            this.GtkLabel3 = new global::Gtk.Label();
            this.GtkLabel3.Name = "GtkLabel";
            this.GtkLabel3.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Keyboard</b>");
            this.GtkLabel3.UseMarkup = true;
            this.fKeyboard.LabelWidget = this.GtkLabel3;
            this.fKeyboard.ShowAll();

            this.fIgnore = new global::Gtk.Frame();
            this.fIgnore.Name = "frame1";
            this.fIgnore.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment5 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment5.Name = "GtkAlignment";
            this.GtkAlignment5.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.GtkScrolledWindow5 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow5.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow5.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeview5 = new global::Gtk.TreeView();
            this.treeview5.CanFocus = true;
            this.treeview5.Name = "treeview1";
            this.treeview5.Selection.Mode = SelectionMode.Multiple;
            this.treeview5.PopupMenu += new PopupMenuHandler(IM);
            this.treeview5.ButtonPressEvent += new ButtonPressEventHandler(IgMenu);
            Gtk.TreeViewColumn list_1 = new Gtk.TreeViewColumn();
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
            this.treeview5.Model = IgnoreDB;
            this.treeview5.AppendColumn(list_3);
            this.GtkScrolledWindow5.Add(this.treeview5);
            this.GtkAlignment5.Add(this.GtkScrolledWindow5);
            this.fIgnore.Add(this.GtkAlignment5);
            this.GtkLabel5 = new global::Gtk.Label();
            this.GtkLabel5.Name = "GtkLabel";
            this.GtkLabel5.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Ignore list</b>");
            this.GtkLabel5.UseMarkup = true;
            this.fIgnore.LabelWidget = this.GtkLabel5;
            this.fIgnore.ShowAll();

            this.frame6 = new global::Gtk.Frame();
            this.frame6.Name = "frame1";
            this.frame6.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment6 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
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
            this.GtkLabel6.LabelProp = global::Mono.Unix.Catalog.GetString("<b>System</b>");
            this.GtkLabel6.UseMarkup = true;
            this.frame6.LabelWidget = this.GtkLabel6;
            this.frame6.ShowAll();

            this.frame7 = new global::Gtk.Frame();
            this.frame7.Name = "frame1";
            this.frame7.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment7 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment7.Name = "GtkAlignment";
            this.GtkAlignment7.LeftPadding = ((uint)(12));
            CreateLogs();
            //this.GtkAlignment5.Add (this.GtkScrolledWindow5);
            this.frame7.Add(this.GtkAlignment7);
            this.GtkLabel7 = new global::Gtk.Label();
            this.GtkLabel7.Name = "GtkLabel";
            this.GtkLabel7.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Logs</b>");
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
    }
}
