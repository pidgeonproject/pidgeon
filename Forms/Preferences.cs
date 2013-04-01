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
		private global::Gtk.Frame frame2;
		private global::Gtk.Alignment GtkAlignment2;
		private global::Gtk.ScrolledWindow GtkScrolledWindow2;
		private global::Gtk.TreeView treeview2;
		private global::Gtk.Label GtkLabel2;
		private Gtk.Widget widget = null;
		private global::Gtk.Frame frame3;
		private global::Gtk.Alignment GtkAlignment3;
		private global::Gtk.ScrolledWindow GtkScrolledWindow3;
		private global::Gtk.TreeView treeview3;
		private global::Gtk.Label GtkLabel3;
		private global::Gtk.Frame frame4;
		private global::Gtk.Alignment GtkAlignment4;
		private global::Gtk.ScrolledWindow GtkScrolledWindow4;
		private global::Gtk.TreeView treeview4;
		private global::Gtk.Label GtkLabel4;
		private global::Gtk.Frame frame5;
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
		private Gtk.ListStore Highlights = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(Network.Highlighter));
		
		private Gtk.ListStore item = new Gtk.ListStore(typeof(string), typeof(int));
		
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
		
		public void Initialize()
		{
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame1";
			this.frame2.ShadowType = ((global::Gtk.ShadowType)(0));
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
			this.GtkScrolledWindow2.Add (this.treeview2);
			this.GtkAlignment2.Add (this.GtkScrolledWindow2);
			this.frame2.Add (this.GtkAlignment2);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel";
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Extensions</b>");
			this.GtkLabel2.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel2;
			this.frame2.ShowAll();
			
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
			
			this.frame3 = new global::Gtk.Frame ();
			this.frame3.Name = "frame1";
			this.frame3.ShadowType = ((global::Gtk.ShadowType)(0));
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
			this.GtkScrolledWindow3.Add (this.treeview3);
			this.GtkAlignment3.Add (this.GtkScrolledWindow3);
			this.frame3.Add (this.GtkAlignment3);
			this.GtkLabel3 = new global::Gtk.Label ();
			this.GtkLabel3.Name = "GtkLabel";
			this.GtkLabel3.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Keyboard</b>");
			this.GtkLabel3.UseMarkup = true;
			this.frame3.LabelWidget = this.GtkLabel3;
			this.frame3.ShowAll();
			
			this.frame5 = new global::Gtk.Frame ();
			this.frame5.Name = "frame1";
			this.frame5.ShadowType = ((global::Gtk.ShadowType)(0));
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
			this.GtkScrolledWindow5.Add (this.treeview5);
			this.GtkAlignment5.Add (this.GtkScrolledWindow5);
			this.frame5.Add (this.GtkAlignment5);
			this.GtkLabel5 = new global::Gtk.Label ();
			this.GtkLabel5.Name = "GtkLabel";
			this.GtkLabel5.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Ignore list</b>");
			this.GtkLabel5.UseMarkup = true;
			this.frame5.LabelWidget = this.GtkLabel5;
			this.frame5.ShowAll();
			
			this.frame6 = new global::Gtk.Frame ();
			this.frame6.Name = "frame1";
			this.frame6.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment6 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment6.Name = "GtkAlignment";
			this.GtkAlignment6.LeftPadding = ((uint)(12));
			//this.GtkAlignment5.Add (this.GtkScrolledWindow5);
			this.frame6.Add (this.GtkAlignment6);
			this.GtkLabel6 = new global::Gtk.Label ();
			this.GtkLabel6.Name = "GtkLabel";
			this.GtkLabel6.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>System</b>");
			this.GtkLabel6.UseMarkup = true;
			this.frame6.LabelWidget = this.GtkLabel5;
			this.frame6.ShowAll();
			
			this.frame7 = new global::Gtk.Frame ();
			this.frame7.Name = "frame1";
			this.frame7.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment7 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment7.Name = "GtkAlignment";
			this.GtkAlignment7.LeftPadding = ((uint)(12));
			//this.GtkAlignment5.Add (this.GtkScrolledWindow5);
			this.frame7.Add (this.GtkAlignment7);
			this.GtkLabel7 = new global::Gtk.Label ();
			this.GtkLabel7.Name = "GtkLabel";
			this.GtkLabel7.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Logs</b>");
			this.GtkLabel7.UseMarkup = true;
			this.frame7.LabelWidget = this.GtkLabel5;
			this.frame7.ShowAll();
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
            checkbutton1.Active = Configuration.Kernel.CheckUpdate;
            messages.Localize(this);
            if (Configuration.CurrentPlatform != Core.Platform.Windowsx86 && Configuration.CurrentPlatform != Core.Platform.Windowsx64)
            {
                checkbutton1.Active = false;
                checkbutton1.Sensitive = false;
            }
            messages.Localize(this);
            this.DeleteEvent += new DeleteEventHandler(hide);
            button1.Clicked += new EventHandler(bSave_Click);
            entry1.Text = Configuration.UserData.nick;
			this.treeview1.ButtonPressEvent += new ButtonPressEventHandler(s1);
            entry2.Text = Configuration.UserData.quit;
            entry3.Text = Configuration.UserData.ident;
            entry4.Text = Configuration.UserData.user;
            button3.Clicked += new EventHandler(bCancel_Click);
			this.treeview1.Model = item;
			this.treeview1.CursorChanged += new EventHandler(s4);
			Gtk.TreeViewColumn topic_item = new Gtk.TreeViewColumn();
			Gtk.CellRendererText c1 = new Gtk.CellRendererText();
			topic_item.Title = "Option";
			topic_item.PackStart(c1, true);
			topic_item.AddAttribute    (c1, "text", 0);
			treeview1.AppendColumn(topic_item);
			treeview1.RowActivated += new RowActivatedHandler(s2);
            //checkBox3.Checked = Configuration.Logs.logs_xml;
            //checkBox1.Checked = Configuration.Logs.logs_txt;
            //checkBox2.Checked = Configuration.Logs.logs_html;
            //checkBox4.Checked = Configuration.irc.DisplayCtcp;
            //checkBox8.Checked = Configuration.irc.ConfirmAll;
            //checkBox9.Checked = Configuration.Kernel.Notice;
            //textBox5.Text = Configuration.Logs.logs_dir;
            //textBox6.Text = Configuration.Logs.logs_name;
            //textBox7.Text = Configuration.UserData.Nick2;
            //radioButton3.Checked = true;
            switch (Configuration.Logs.ServicesLogs)
            {
                case Configuration.Logs.ServiceLogs.full:
                    //radioButton1.Checked = true;
                    break;
                case Configuration.Logs.ServiceLogs.incremental:
                    //radioButton2.Checked = true;
                    break;
            }

            foreach (Skin skin in Configuration.SL)
            {
            //    comboBox1.Items.Add(skin.name);
            }
            //comboBox1.SelectedIndex = 0;

            foreach (KeyValuePair<string, messages.container> language in messages.data)
            {
            //    comboBox2.Items.Add(language.Key);
            }
            //comboBox2.Text = Core.SelectedLanguage;

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
						setWindow(frame6);
						break;
					case 3:
						setWindow(frame7);
						break;
					case 5:
						setWindow(frame4);
						break;
					case 6:
						setWindow(frame5);
						break;
					case 7:
						setWindow(frame3);
						break;
					case 8:
						setWindow(frame2);
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
                //Configuration.Logs.logs_xml = checkBox3.Checked;
                //Configuration.Logs.logs_txt = checkBox1.Checked;
                //Configuration.Logs.logs_html = checkBox2.Checked;
                //Configuration.Kernel.CheckUpdate = checkBox10.Checked;
                //Configuration.irc.DisplayCtcp = checkBox4.Checked;
                //Configuration.Kernel.Notice = checkBox9.Checked;
                //Configuration.Logs.logs_dir = textBox5.Text;
                //Configuration.Logs.logs_name = textBox6.Text;
                //Configuration.UserData.Nick2 = textBox7.Text;
                //if (radioButton1.Checked)
                {
                //    Configuration.Logs.ServicesLogs = Configuration.Logs.ServiceLogs.full;
                }
                //if (radioButton2.Checked)
                {
                //    Configuration.Logs.ServicesLogs = Configuration.Logs.ServiceLogs.incremental;
                }
                //if (radioButton3.Checked)
                {
                //    Configuration.Logs.ServicesLogs = Configuration.Logs.ServiceLogs.none;
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

