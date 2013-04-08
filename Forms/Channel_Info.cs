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
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using Gtk;

namespace Client.Forms
{
    public partial class Channel_Info : Gtk.Window
    {
        public Channel channel = null;
        public List<char> cm = new List<char>();

        private global::Gtk.Notebook notebook1;
        private global::Gtk.VBox vbox1;
        private global::Gtk.Frame frame4;
        private global::Gtk.Alignment GtkAlignment3;
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TextView textview1;
        private global::Gtk.Label GtkLabel;
        private global::Gtk.Frame frame5;
        private global::Gtk.Alignment GtkAlignment4;
        private global::Gtk.ScrolledWindow GtkScrolledWindow1;
        private global::Gtk.TreeView treeview1;
        private global::Gtk.Label GtkLabel4;
        private global::Gtk.Button button1;
        private global::Gtk.Label label1;
        private global::Gtk.ScrolledWindow GtkScrolledWindow2;
        private global::Gtk.TreeView treeview5;
        private global::Gtk.Label label2;
        private global::Gtk.ScrolledWindow GtkScrolledWindow3;
        private global::Gtk.TreeView treeview6;
        private global::Gtk.Label label3;
        private global::Gtk.ScrolledWindow GtkScrolledWindow4;
        private global::Gtk.TreeView treeview7;
        private global::Gtk.Label label4;
        private Gtk.ListStore exceptions = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(Except));
        private Gtk.ListStore bans = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(SimpleBan));
        public Gtk.VPaned vpaned01 = null;
        private Gtk.ListStore invites = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(Invite));
        private Client.GTK.Menu deleteToolStripMenuItemi = new Client.GTK.Menu("Remove");
        private Client.GTK.Menu reloadToolStripMenuItemi = new Client.GTK.Menu("Reload");
        private Client.GTK.Menu reloadToolStripMenuItemb= new Client.GTK.Menu("Reload");
        private Client.GTK.Menu reloadToolStripMenuIteme = new Client.GTK.Menu("Reload");
        private Client.GTK.Menu deleteToolStripMenuIteme = new Client.GTK.Menu("Remove");
        private Client.GTK.Menu deleteToolStripMenuItemb = new Client.GTK.Menu("Remove");
        private Client.GTK.Menu insertToolStripMenuItemb = new Client.GTK.Menu("Insert");
        private Client.GTK.Menu insertToolStripMenuIteme = new Client.GTK.Menu("Insert");
        private Client.GTK.Menu refreshToolStripMenuItemi = new Client.GTK.Menu("Refresh");
        private Client.GTK.Menu refreshToolStripMenuItemb = new Client.GTK.Menu("Refresh");
        private Client.GTK.Menu refreshToolStripMenuIteme = new Client.GTK.Menu("Refresh");
        private Client.GTK.Menu cleanToolStripMenuIteme = new Client.GTK.Menu();
        private Client.GTK.Menu cleanToolStripMenuItemb = new Client.GTK.Menu();
        private Client.GTK.Menu cleanToolStripMenuItemi = new Client.GTK.Menu();
        private List<CheckButton> options = new List<CheckButton>();
        //private Gtk.ListStore options = new Gtk.ListStore(typeof(bool), typeof(char), typeof(string));

        public List<Except> SelectedExcept
        {
            get
            {
                List<Except> el = new List<Except>();
                TreeIter iter;
                TreePath[] path = treeview6.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return null;
                }
                foreach (TreePath p in path)
                {
                    treeview6.Model.GetIter(out iter, p);
                    el.Add((Except)treeview6.Model.GetValue(iter, 3));
                }
                return el;
            }
        }

        public List<SimpleBan> SelectedBans
        {
            get
            {
                List<SimpleBan> bl = new List<SimpleBan>();
                TreeIter iter;
                TreePath[] path = treeview7.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return null;
                }
                foreach (TreePath p in path)
                {
                    treeview7.Model.GetIter(out iter, p);
                    bl.Add((SimpleBan)treeview7.Model.GetValue(iter, 3));
                }
                return bl;
            }
        }

        public List<Invite> SelectedInvites
        {
            get
            {
                List<Invite> il = new List<Invite>();
                TreeIter iter;
                TreePath[] path = treeview5.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return null;
                }
                foreach (TreePath p in path)
                {
                    treeview5.Model.GetIter(out iter, p);
                    il.Add((Invite)treeview5.Model.GetValue(iter, 3));
                }
                return il;
            }
        }

        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget MainWindow
            this.Name = "MainWindow";
            this.Title = global::Mono.Unix.Catalog.GetString("Channel properties: " + channel.Name);
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            // Container child MainWindow.Gtk.Container+ContainerChild
            this.notebook1 = new global::Gtk.Notebook();
            this.notebook1.CanFocus = true;
            this.notebook1.Name = "notebook1";
            this.notebook1.CurrentPage = 0;
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.vpaned01 = new Gtk.VPaned();
            this.vbox1 = new global::Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            // Container child vbox1.Gtk.Box+BoxChild
            this.frame4 = new global::Gtk.Frame();
            this.frame4.Name = "frame4";
            this.frame4.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame4.Gtk.Container+ContainerChild
            this.GtkAlignment3 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment3.Name = "GtkAlignment3";
            this.GtkAlignment3.HeightRequest = 60;
            this.GtkAlignment3.LeftPadding = ((uint)(12));
            // Container child GtkAlignment3.Gtk.Container+ContainerChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.textview1 = new global::Gtk.TextView();
            this.textview1.CanFocus = true;
            this.textview1.Name = "textview1";
            this.textview1.HeightRequest = 60;
            this.GtkScrolledWindow.Add(this.textview1);
            this.GtkAlignment3.Add(this.GtkScrolledWindow);
            //global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(GtkAlignment3));
            //w2.Position = 0;
            //w2.Expand = false;
            this.frame4.Add(this.GtkAlignment3);
            this.GtkLabel = new global::Gtk.Label();
            this.GtkLabel.Name = "GtkLabel";
            this.GtkLabel.LabelProp = global::Mono.Unix.Catalog.GetString("Topic was last set on " + Network.convertUNIX(channel.TopicDate.ToString()) + " by " + channel.TopicUser);
            if (channel.TopicDate == 0)
            {
                this.GtkLabel.LabelProp = global::Mono.Unix.Catalog.GetString("Topic (information about current topic are not present)");
            }
            this.GtkLabel.UseMarkup = true;
            this.frame4.LabelWidget = this.GtkLabel;
            //this.vbox1.Add(this.frame4);
            this.vpaned01.Add(this.frame4);
            global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.frame4]));
            w4.Position = 0;
            // Container child vbox1.Gtk.Box+BoxChild
            this.frame5 = new global::Gtk.Frame();
            this.frame5.Name = "frame5";
            this.frame5.ShadowType = ((global::Gtk.ShadowType)(0));
            // Container child frame5.Gtk.Container+ContainerChild
            this.GtkAlignment4 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment4.Name = "GtkAlignment4";
            this.GtkAlignment4.LeftPadding = ((uint)(12));
            // Container child GtkAlignment4.Gtk.Container+ContainerChild
            this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
            this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
            //this.treeview1 = new global::Gtk.TreeView();
            //this.treeview1.CanFocus = true;
            //Gtk.TreeViewColumn enabled = new Gtk.TreeViewColumn();
            //Gtk.TreeViewColumn character = new Gtk.TreeViewColumn();
            //Gtk.TreeViewColumn ds = new Gtk.TreeViewColumn();
            //this.treeview1.AppendColumn(enabled);
            //this.treeview1.AppendColumn(character);
            //this.treeview1.AppendColumn(ds);
            //enabled.Title = "Enabled";
            //character.Title = "Channel mode";
            //ds.Title = "Description";
            //Gtk.CellRendererToggle r_enabled = new CellRendererToggle();
            //enabled.PackStart(r_enabled, true);
            //Gtk.CellRendererText r_char = new CellRendererText();
            //Gtk.CellRendererText r_desc = new CellRendererText();
            //ds.PackStart(r_desc, true);
            //character.PackStart(r_char, true);
            //enabled.AddAttribute(r_enabled, "bool", 0);
            //character.AddAttribute(r_char, "char", 1);
            //ds.AddAttribute(r_desc, "text", 2);
            //this.treeview1.Model = options;
            //this.treeview1.Name = "treeview1";
            Gtk.Fixed f = new Fixed();
            ReloadModes(ref f);
            this.GtkScrolledWindow1.Add(f);
            this.GtkAlignment4.Add(this.GtkScrolledWindow1);
            this.frame5.Add(this.GtkAlignment4);
            this.GtkLabel4 = new global::Gtk.Label();
            this.GtkLabel4.Name = "GtkLabel4";
            this.GtkLabel4.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Channel mode</b>");
            this.GtkLabel4.UseMarkup = true;
            this.frame5.LabelWidget = this.GtkLabel4;
            this.vbox1.Add(this.frame5);
            global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.frame5]));
            w8.Position = 1;
            // Container child vbox1.Gtk.Box+BoxChild
            this.button1 = new global::Gtk.Button();
            this.button1.Clicked += new EventHandler(bClose_Click);
            this.button1.CanFocus = true;
            this.button1.Name = "button1";
            this.button1.UseUnderline = true;
            this.button1.Label = global::Mono.Unix.Catalog.GetString("Update");
            this.vbox1.Add(this.button1);
            global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox1[this.button1]));
            w9.Position = 2;
            w9.Expand = false;
            w9.Fill = false;
            vpaned01.Add2(this.vbox1);
            this.notebook1.Add(this.vpaned01);
            // Notebook tab
            this.label1 = new global::Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = global::Mono.Unix.Catalog.GetString(messages.get("channelinfo-t0", Core.SelectedLanguage));
            this.notebook1.SetTabLabel(this.vpaned01, this.label1);
            this.label1.ShowAll();
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
            this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
            this.treeview5 = new global::Gtk.TreeView();
            treeview5.Selection.Mode = SelectionMode.Multiple;
            this.treeview5.Model = invites;
            this.treeview5.CanFocus = true;
            this.treeview5.Name = "treeview5";
            Gtk.TreeViewColumn invite = new TreeViewColumn();
            Gtk.CellRendererText r3 = new CellRendererText();
            Gtk.TreeViewColumn itime = new TreeViewColumn();
            Gtk.CellRendererText r2 = new CellRendererText();
            Gtk.TreeViewColumn iu = new TreeViewColumn();
            Gtk.CellRendererText r1 = new CellRendererText();
            invite.Title = "Invite";
            itime.Title = "Time";
            iu.Title = "Created by";
            invite.PackStart(r1, true);
            itime.PackStart(r2, true);
            iu.PackStart(r3, true);
            invite.AddAttribute(r1, "text", 0);
            itime.AddAttribute(r2, "text", 1);
            iu.AddAttribute(r3, "text", 2);
            treeview5.AppendColumn(invite);
            treeview5.AppendColumn(itime);
            treeview5.ButtonPressEvent += new ButtonPressEventHandler(IgnoreInvite);
            treeview5.AppendColumn(iu);
            this.GtkScrolledWindow2.Add(this.treeview5);
            this.notebook1.Add(this.GtkScrolledWindow2);
            global::Gtk.Notebook.NotebookChild w12 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1[this.GtkScrolledWindow2]));
            w12.Position = 1;
            // Notebook tab
            this.label2 = new global::Gtk.Label();
            this.label2.Name = "label2";
            this.label2.LabelProp = global::Mono.Unix.Catalog.GetString(messages.get("channelinfo-t2", Core.SelectedLanguage));
            this.notebook1.SetTabLabel(this.GtkScrolledWindow2, this.label2);
            this.label2.ShowAll();
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.GtkScrolledWindow3 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow3.Name = "GtkScrolledWindow3";
            this.GtkScrolledWindow3.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow3.Gtk.Container+ContainerChild
            this.treeview6 = new global::Gtk.TreeView();
            this.treeview6.CanFocus = true;
            this.treeview6.Selection.Mode = SelectionMode.Multiple;
            this.treeview6.Name = "treeview6";
            Gtk.TreeViewColumn exception = new TreeViewColumn();
            Gtk.CellRendererText er3 = new CellRendererText();
            Gtk.TreeViewColumn etime = new TreeViewColumn();
            Gtk.CellRendererText er2 = new CellRendererText();
            Gtk.TreeViewColumn eu = new TreeViewColumn();
            Gtk.CellRendererText er1 = new CellRendererText();
            exception.Title = "Exception";
            etime.Title = "Time";
            eu.Title = "Created by";
            exception.PackStart(er1, true);
            etime.PackStart(er2, true);
            eu.PackStart(er3, true);
            exception.AddAttribute(er1, "text", 0);
            etime.AddAttribute(er2, "text", 1);
            eu.AddAttribute(er3, "text", 2);
            treeview6.AppendColumn(exception);
            treeview6.AppendColumn(etime);
            treeview6.AppendColumn(eu);
            this.treeview6.ButtonPressEvent += new ButtonPressEventHandler(IgnoreExcept);
            this.GtkScrolledWindow3.Add(this.treeview6);
            this.notebook1.Add(this.GtkScrolledWindow3);
            global::Gtk.Notebook.NotebookChild w14 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1[this.GtkScrolledWindow3]));
            this.treeview6.Model = exceptions;
            w14.Position = 2;
            
            // Notebook tab
            this.label3 = new global::Gtk.Label();
            this.label3.Name = "label3";
            this.label3.LabelProp = global::Mono.Unix.Catalog.GetString(messages.get("channelinfo-t3", Core.SelectedLanguage));
            this.notebook1.SetTabLabel(this.GtkScrolledWindow3, this.label3);
            this.label3.ShowAll();
            // Container child notebook1.Gtk.Notebook+NotebookChild
            this.GtkScrolledWindow4 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow4.Name = "GtkScrolledWindow4";
            this.GtkScrolledWindow4.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow4.Gtk.Container+ContainerChild
            this.treeview7 = new global::Gtk.TreeView();
            this.treeview7.CanFocus = true;
            this.treeview7.Name = "treeview7";
            this.treeview7.ButtonPressEvent += new ButtonPressEventHandler(IgnoreBans);
            this.treeview7.PopupMenu += new PopupMenuHandler(MenuBans);
            this.treeview7.Selection.Mode = SelectionMode.Multiple;
            this.GtkScrolledWindow4.Add(this.treeview7);
            this.treeview7.Model = bans;
            this.notebook1.Add(this.GtkScrolledWindow4);
            Gtk.TreeViewColumn ban = new TreeViewColumn();
            Gtk.CellRendererText br3 = new CellRendererText();
            Gtk.TreeViewColumn btime = new TreeViewColumn();
            Gtk.CellRendererText br2 = new CellRendererText();
            Gtk.TreeViewColumn bu = new TreeViewColumn();
            Gtk.CellRendererText br1 = new CellRendererText();
            ban.Title = "Host";
            btime.Title = "Time";
            bu.Title = "Created by";
            ban.PackStart(br1, true);
            btime.PackStart(br2, true);
            bu.PackStart(br3, true);
            ban.AddAttribute(br1, "text", 0);
            btime.AddAttribute(br2, "text", 1);
            bu.AddAttribute(br3, "text", 2);
            treeview7.AppendColumn(ban);
            treeview7.AppendColumn(btime);
            treeview7.AppendColumn(bu);
            global::Gtk.Notebook.NotebookChild w16 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1[this.GtkScrolledWindow4]));
            w16.Position = 3;
            // Notebook tab
            this.label4 = new global::Gtk.Label();
            this.label4.Name = "label4";
            this.label4.LabelProp = global::Mono.Unix.Catalog.GetString(messages.get("channelinfo-t4", Core.SelectedLanguage));
            this.notebook1.SetTabLabel(this.GtkScrolledWindow4, this.label4);
            this.label4.ShowAll();
            this.WindowPosition = WindowPosition.Center;
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.Add(this.notebook1);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 796;
            this.DefaultHeight = 511;
            this.Show();
        }

        public Channel_Info() : base(Gtk.WindowType.Toplevel)
        {

        }


        public void MenuBans(object sender, Gtk.PopupMenuArgs e)
        {
            Gtk.Menu menu = new Menu();
            Gtk.MenuItem reload = new MenuItem(reloadToolStripMenuItemb.Text);
            reload.Activated += new EventHandler(reloadToolStripMenuItem1_Click);
            Gtk.MenuItem delete = new MenuItem(deleteToolStripMenuItemb.Text);
            delete.Activated += new EventHandler(deleteToolStripMenuItem1_Click);
            menu.Append(delete);
            menu.Append(reload);
            menu.ShowAll();
            menu.Popup();
        }

        public void ReloadExceptions()
        {
            if (channel != null)
            {
                if (channel.Exceptions != null)
                {
                    exceptions.Clear();
                    lock (channel.Exceptions)
                    {
                        foreach (Except sb in channel.Exceptions)
                        {
                            exceptions.AppendValues(sb.Target, convertUNIX(sb.Time) + " (" + sb.Time + ")", sb.User, sb);
                        }
                    }
                }
            }
        }

        public void ReloadBans()
        {
            if (channel != null)
            {
                if (channel.Bans != null)
                {
                    bans.Clear();
                    lock (channel.Bans)
                    {
                        foreach (SimpleBan sb in channel.Bans)
                        {
                            bans.AppendValues(sb.Target, convertUNIX(sb.Time) + " (" + sb.Time + ")", sb.User, sb);
                        }
                    }
                }
            }
        }

        [GLib.ConnectBefore]
        private void IgnoreBans(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                MenuBans(sender, null);
                e.RetVal = true;
            }
        }

        [GLib.ConnectBefore]
        private void IgnoreExcept(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                e.RetVal = true;
            }
        }

        [GLib.ConnectBefore]
        private void IgnoreInvite(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                e.RetVal = true;
            }
        }

        public void ReloadInvites()
        {
            if (channel != null)
            {
                if (channel.Invites != null)
                {
                    invites.Clear();
                    lock (channel.Invites)
                    {
                        foreach (Invite sb in channel.Invites)
                        {
                            invites.AppendValues(sb.Target, convertUNIX(sb.Time) + " (" + sb.Time + ")", sb.User, sb);
                        }
                    }
                }
            }
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (channel.Topic != textview1.Buffer.Text)
                {
                    channel._Network.Transfer("TOPIC " + channel.Name + " :" + textview1.Buffer.Text);
                }
                bool change = false;
                string cset = "+";
                string uset = "-";
                //foreach (string item in checkedList.Items)
                //{
                //    if (checkedList.CheckedItems.Contains(item))
                //    {
                //        if (!channel.ChannelMode._Mode.Contains(item[0].ToString()))
                //        {
                //            cset += item[0];
                //            change = true;
                //        }
                //        continue;
                //    }
                //    if (channel.ChannelMode._Mode.Contains(item[0].ToString()))
                //    {
                //        uset += item[0];
                //        change = true;
                //    }
                //}
                //if (change)
                //{
                //    if (uset == "-")
                //    {
                //        uset = "";
                //    }
                //    if (cset == "+")
                //    {
                //        cset = "";
                //    }
                //    channel._Network.Transfer("MODE " + channel.Name + " " + cset + uset);
                //}
                this.Hide();
                this.Dispose();
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedInvites != null)
                {
                    foreach (Invite item in SelectedInvites)
                    {
                        channel._Network.Transfer("MODE " + channel.Name + " -I " + item.Target);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadBans();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void insertToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void enforceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedBans != null)
                {
                    foreach (SimpleBan ban in SelectedBans)
                    {
                        channel._Network.Transfer("MODE " + channel.Name + " -b " + ban.Target);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadInvites();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedExcept != null)
                {
                    foreach (Except ex in SelectedExcept)
                    {
                        channel._Network.Transfer("MODE " + channel.Name + " -e " + ex.Target);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void cleanToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void reloadToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                //timerBans.Enabled = true;
                channel.ReloadBans();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private bool timerBans_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!channel.parsing_bans)
                {
                    ReloadBans();
                    return false;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
        }

        private void reloadToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadInvites();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadExceptions();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void ReloadModes(ref Gtk.Fixed vbox)
        {
            options.Clear();
            if (channel != null)
            {
                ReloadBans();
                ReloadExceptions();
                ReloadInvites();

                lock (channel.ChannelMode)
                {
                    int height = 0;
                    foreach (char item in channel._Network.CModes)
                    {
                        string de = "unknown mode. Refer to ircd manual (/raw help)";
                        cm.Add(item);
                        if (channel._Network.Descriptions.ContainsKey(item))
                        {
                            de = channel._Network.Descriptions[item];
                        }
                        CheckButton xx = new CheckButton(item.ToString() + " : " + de);
                        xx.Name = item.ToString();
                        options.Add(xx);
                        vbox.Add(xx);
                        global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(vbox[xx]));
                        w1.X = height;
                        w1.Y = 0;
                        height += 20;
                    }
                }
            }
        }

        public void Load()
        {
            this.Build();
            this.textview1.WrapMode = WrapMode.Word;
            textview1.Buffer.Text = channel.Topic;
        }

        public static string convertUNIX(string time)
        {
            try
            {
                double unixtimestmp = double.Parse(time);
                return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixtimestmp).ToString();
            }
            catch (Exception)
            {
                return "Unable to read: " + time;
            }
        }
    }
}

