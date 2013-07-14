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
    public partial class Preferences : GTK.PidgeonForm
    {
        /// <summary>
        /// Creates a new instance of this form
        /// </summary>
        public Preferences()
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
        private void Preferences_Load()
        {
            checkbutton2.Active = Configuration.Kernel.CheckUpdate;
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
            topic_item.AddAttribute(c1, "text", 0);
            this.treeview1.AppendColumn(topic_item);
            this.treeview1.RowActivated += new RowActivatedHandler(s2);
            this.lcheckbutton3.Active = Configuration.Logs.logs_xml;
            this.lcheckbutton1.Active = Configuration.Logs.logs_txt;
            this.lcheckbutton2.Active = Configuration.Logs.logs_html;
            this.checkButton_CTCP.Active = Configuration.irc.DisplayCtcp;
            this.checkButton_request.Active = Configuration.irc.ConfirmAll;
            this.checkButton_Sounds.Active = Configuration.Media.NotificationSound;
            checkbutton1.Active = Configuration.Kernel.Notice;
            lentry2.Text = Configuration.Logs.logs_dir;
            lentry1.Text = Configuration.Logs.logs_name;
            checkButton_DisplayIcon.Active = Configuration.UserData.TrayIcon;
            checkButton_BlockCtcp.Active = Configuration.irc.FirewallCTCP;
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

            combobox2.Model = lg;

            int selectedSkin = 0;
            int currentSkin = 0;

            foreach (Skin skin in Configuration.SL)
            {
                if (Configuration.CurrentSkin == skin)
                {
                    selectedSkin = currentSkin;
                }
                currentSkin++;
                sl.AppendValues(skin.Name);
            }
            combobox1.Model = sl;
            combobox1.Active = selectedSkin;

            int selectedLanguage = 0;
            int current = 0;
            foreach (string language in messages.data.Keys)
            {
                if (language == Core.SelectedLanguage)
                {
                    selectedLanguage = current;
                }
                lg.AppendValues(language);
                current++;
            }
            combobox2.Active = selectedLanguage;

            ReloadHL();

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
            this.hbox1.Remove(widget);
            widget = frame;
            this.hbox1.Add(widget);
        }

        /// <summary>
        /// Reload the highlight list
        /// </summary>
        public void ReloadHL()
        {
            Highlights.Clear();
            lock (Configuration.HighlighterList)
            {
                foreach (Network.Highlighter highlight in Configuration.HighlighterList)
                {
                    Highlights.AppendValues(highlight.text, (!highlight.simple).ToString(), highlight.enabled.ToString(), highlight);
                }
            }
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
                        setWindow(fIgnore);
                        break;
                    case 7:
                        setWindow(fKeyboard);
                        break;
                    case 8:
                        setWindow(fExtensions);
                        break;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        [GLib.ConnectBefore]
        private void s2(object sender, RowActivatedArgs e)
        {
            Switch();
        }

        private void RefreshModules()
        {
            Extensions.Clear();
            foreach (Extension ex in Core.Extensions)
            {
                Extensions.AppendValues(ex.Name, ex.Version, ex.Description, "Extension", ex);
            }
        }

        /// <summary>
        /// Redraw list
        /// </summary>
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
                Configuration.UserData.nick = entry1.Text;
                Configuration.UserData.quit = entry2.Text;
                Configuration.UserData.ident = entry3.Text;
                Configuration.UserData.user = entry4.Text;
                Configuration.UserData.TrayIcon = checkButton_DisplayIcon.Active;
                Configuration.UserData.Nick2 = entry5.Text;
                Configuration.Logs.logs_xml = lcheckbutton3.Active;
                Configuration.Logs.logs_txt = lcheckbutton1.Active;
                Configuration.Logs.logs_html = lcheckbutton2.Active;
                Configuration.Kernel.CheckUpdate = checkbutton2.Active;
                Configuration.irc.DisplayCtcp = checkButton_CTCP.Active;
                Configuration.Kernel.Notice = checkbutton1.Active;
                Configuration.Media.NotificationSound = checkButton_Sounds.Active;
                Configuration.Logs.logs_dir = lentry2.Text;
                Configuration.Logs.logs_name = lentry1.Text;
                Configuration.irc.FirewallCTCP = checkButton_BlockCtcp.Active;
                Configuration.irc.ConfirmAll = checkButton_request.Active;
                Core.SelectedLanguage = combobox2.ActiveText;

                Skin.ReloadSkin(combobox1.ActiveText);
                
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

        /// <summary>
        /// Reload the ignore list
        /// </summary>
        public void ReloadIgnores()
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

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Preferences_Ignore dialog = new Preferences_Ignore();
                dialog.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                Preferences_Shortcut dialog = new Preferences_Shortcut();
                dialog.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void addToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                Preferences_Hl dialog = new Preferences_Hl();
                dialog.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Ignoring.Ignore curr in SelectedIgnore)
                {
                    curr.Enabled = true;
                }
                ReloadIgnores();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Ignoring.Ignore curr in SelectedIgnore)
                {
                    curr.Enabled = false;
                }
                ReloadIgnores();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
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
            try
            {
                foreach (Core.Shortcut x in SelectedShorts)
                {
                    lock (Configuration.ShortcutKeylist)
                    {
                        Configuration.ShortcutKeylist.Remove(x);
                    }
                }
                redrawS();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void loadModuleFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Gtk.FileChooserDialog dialog = new FileChooserDialog("Load module", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
                if (dialog.Run() == (int)ResponseType.Accept)
                {
                    if (dialog.Filename != "")
                    {
                        Core.RegisterPlugin(dialog.Filename);
                    }
                    else
                    {
                        Core.DebugLog("Preferences: provided invalid file name");
                    }
                }
                dialog.Destroy();
                RefreshModules();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void unloadModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Extension x in SelectedExtensions)
            {
                x.Exit();
            }
            RefreshModules();
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Network.Highlighter c in SelectedHs)
                {
                    lock (Configuration.HighlighterList)
                    {
                        if (Configuration.HighlighterList.Contains(c))
                        {
                            Configuration.HighlighterList.Remove(c);
                        }
                    }
                }
                ReloadHL();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Ignoring.Ignore x in SelectedIgnore)
                {
                    lock (Ignoring.IgnoreList)
                    {
                        if (Ignoring.IgnoreList.Contains(x))
                        {
                            Ignoring.IgnoreList.Remove(x);
                        }
                    }
                }
                ReloadIgnores();
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
                Ignoring.Ignore item = new Ignoring.Ignore(false, true, "someone_bad", Ignoring.Ignore.Type.User);
                lock (Ignoring.IgnoreList)
                {
                    Ignoring.IgnoreList.Add(item);
                }
                ReloadIgnores();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void disableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Network.Highlighter x in SelectedHs)
                {
                    x.enabled = false;
                }
                ReloadHL();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void enableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Network.Highlighter x in SelectedHs)
                {
                    x.enabled = true;
                }
                ReloadHL();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void simpleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Network.Highlighter x in SelectedHs)
                {
                    x.simple = true;
                }
                ReloadHL();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void simpleToolStripMenuItemIgnore_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Ignoring.Ignore curr in SelectedIgnore)
                {
                    curr.Simple = true;
                }
                ReloadIgnores();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void regexToolStripMenuItemIgnore_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Ignoring.Ignore curr in SelectedIgnore)
                {
                    curr.Simple = false;
                }
                ReloadIgnores();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void regexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Network.Highlighter x in SelectedHs)
                {
                    x.simple = false;
                }
                ReloadHL();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void matchingOnlyUserStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Ignoring.Ignore curr in SelectedIgnore)
                {
                    curr.type = Ignoring.Ignore.Type.User;
                }
                ReloadIgnores();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void matchingTextInWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Ignoring.Ignore curr in SelectedIgnore)
                {
                    curr.type = Ignoring.Ignore.Type.Everything;
                }
                ReloadIgnores();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}

