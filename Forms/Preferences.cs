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

            ListStore sl = new ListStore(typeof(string));
            ListStore lg = new ListStore(typeof(string));

            combobox1.Sensitive = false;
            combobox2.Model = lg;
            combobox2.Sensitive = false;

            foreach (Skin skin in Configuration.SL)
            {
                sl.AppendValues(skin.name);
            }
            combobox1.Active = 0;

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
            Extensions.Clear();
            foreach (Extension ex in Core.Extensions)
            {
                Extensions.AppendValues(ex.Name, ex.Version, ex.Description, "Extension", ex);
            }
        }

        public void redrawS()
        {
            Keyboard.Clear();
            foreach (Core.Shortcut s in Configuration.ShortcutKeylist)
            {
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
                Keyboard.AppendValues(keys, s.data, s);
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

                Configuration.irc.ConfirmAll = checkButton_request.Active;
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
                IgnoreDB.Clear();
                foreach (Ignoring.Ignore curr in Ignoring.IgnoreList)
                {
                    IgnoreDB.AppendValues(curr.Text, curr.Simple.ToString(), curr.Enabled.ToString(), curr.type.ToString(), curr);
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //    foreach (ListViewItem curr in list.SelectedItems)
                    {
                //        list.Items.Remove(curr);
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
                    redrawS();
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

