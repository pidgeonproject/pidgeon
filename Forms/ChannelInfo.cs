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
using System.Text;
using Gtk;

namespace Pidgeon.Forms
{
    /// <summary>
    /// Window
    /// </summary>
    public partial class ChannelInfo : Pidgeon.PidgeonGtkToolkit.PidgeonForm
    {
        private Channel channel = null;
        private List<char> cm = new List<char>();

        private Gtk.ListStore exceptions = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(libirc.ChannelBanException));
        private Gtk.ListStore bans = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(libirc.SimpleBan));
        private Gtk.ListStore invites = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(libirc.Invite));
        private List<CheckButton> options = new List<CheckButton>();

        /// <summary>
        /// List of selected items
        /// </summary>
        public List<libirc.ChannelBanException> SelectedExcept
        {
            get
            {
                List<libirc.ChannelBanException> el = new List<libirc.ChannelBanException>();
                TreeIter iter;
                TreePath[] path = treeview6.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return null;
                }
                foreach (TreePath p in path)
                {
                    treeview6.Model.GetIter(out iter, p);
                    el.Add((libirc.ChannelBanException)treeview6.Model.GetValue(iter, 3));
                }
                return el;
            }
        }

        /// <summary>
        /// List of selected items
        /// </summary>
        public List<libirc.SimpleBan> SelectedBans
        {
            get
            {
                List<libirc.SimpleBan> bl = new List<libirc.SimpleBan>();
                TreeIter iter;
                TreePath[] path = treeview7.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return null;
                }
                foreach (TreePath p in path)
                {
                    treeview7.Model.GetIter(out iter, p);
                    bl.Add((libirc.SimpleBan)treeview7.Model.GetValue(iter, 3));
                }
                return bl;
            }
        }

        /// <summary>
        /// List of selected items
        /// </summary>
        public List<libirc.Invite> SelectedInvites
        {
            get
            {
                List<libirc.Invite> il = new List<libirc.Invite>();
                TreeIter iter;
                TreePath[] path = treeview5.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return null;
                }
                foreach (TreePath p in path)
                {
                    treeview5.Model.GetIter(out iter, p);
                    il.Add((libirc.Invite)treeview5.Model.GetValue(iter, 3));
                }
                return il;
            }
        }

        /// <summary>
        /// Creates a new window
        /// </summary>
        /// <param name="ch"></param>
        public ChannelInfo(Channel ch)
        {
            channel = ch;
            Load();
        }

        private void MenuBans(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                Gtk.Menu menu = new Menu();
                Gtk.MenuItem reload = new MenuItem(reloadToolStripMenuItemb.Text);
                reload.Activated += new EventHandler(retrieveToolStripMenuItem1_Click);
                Gtk.MenuItem delete = new MenuItem(deleteToolStripMenuItemb.Text);
                delete.Activated += new EventHandler(deleteToolStripMenuItem1_Click);
                Gtk.MenuItem refresh = new MenuItem(refreshToolStripMenuItemb.Text);
                refresh.Activated += new EventHandler(reloadToolStripMenuItem1_Click);
                menu.Append(delete);
                menu.Append(refresh);
                menu.Append(new SeparatorMenuItem());
                menu.Append(reload);
                menu.ShowAll();
                menu.Popup();
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void MenuInvites(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                Gtk.Menu menu = new Menu();
                Gtk.MenuItem reload = new MenuItem(reloadToolStripMenuItemb.Text);
                reload.Activated += new EventHandler(retrieveToolStripMenuItem_Click);
                Gtk.MenuItem delete = new MenuItem(deleteToolStripMenuItemb.Text);
                delete.Activated += new EventHandler(deleteToolStripMenuItem_Click);
                Gtk.MenuItem refresh = new MenuItem(refreshToolStripMenuItemb.Text);
                refresh.Activated += new EventHandler(reloadToolStripMenuItem_Click);
                menu.Append(delete);
                menu.Append(refresh);
                menu.Append(new SeparatorMenuItem());
                menu.Append(reload);
                menu.ShowAll();
                menu.Popup();
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void MenuExceptions(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                Gtk.Menu menu = new Menu();
                Gtk.MenuItem reload = new MenuItem(reloadToolStripMenuItemb.Text);
                reload.Activated += new EventHandler(retrieveToolStripMenuItem2_Click);
                Gtk.MenuItem delete = new MenuItem(deleteToolStripMenuItemb.Text);
                delete.Activated += new EventHandler(deleteToolStripMenuItem2_Click);
                Gtk.MenuItem refresh = new MenuItem(refreshToolStripMenuItemb.Text);
                refresh.Activated += new EventHandler(reloadToolStripMenuItem1_Click);
                menu.Append(delete);
                menu.Append(refresh);
                menu.Append(new SeparatorMenuItem());
                menu.Append(reload);
                menu.ShowAll();
                menu.Popup();
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void ReloadExceptions()
        {
            if (channel != null)
            {
                if (channel.Exceptions != null)
                {
                    exceptions.Clear();
                    lock (channel.Exceptions)
                    {
                        foreach (libirc.ChannelBanException sb in channel.Exceptions)
                        {
                            exceptions.AppendValues(sb.Target, Core.ConvertFromUNIXToString(sb.Time) + " (" + sb.Time + ")", sb.User, sb);
                        }
                    }
                }
            }
        }

        private void RefreshBans()
        {
            if (channel != null)
            {
                if (channel.Bans != null)
                {
                    bans.Clear();
                    lock (channel.Bans)
                    {
                        foreach (libirc.SimpleBan sb in channel.Bans)
                        {
                            bans.AppendValues(sb.Target, Core.ConvertFromUNIXToString(sb.Time) + " (" + sb.Time + ")", sb.User, sb);
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

        private void RefreshInvites()
        {
            if (channel != null)
            {
                if (channel.Invites != null)
                {
                    invites.Clear();
                    lock (channel.Invites)
                    {
                        foreach (libirc.Invite sb in channel.Invites)
                        {
                            invites.AppendValues(sb.Target, Core.ConvertFromUNIXToString(sb.Time) + " (" + sb.Time + ")", sb.User, sb);
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
                    channel._Network.Transfer("TOPIC " + channel.Name + " :" + textview1.Buffer.Text.Replace("\n", ""));
                }
                bool change = false;
                string cset = "+";
                string uset = "-";
                foreach (CheckButton item in options)
                {
                    if (item.Active)
                    {
                        if (!channel.ChannelMode._Mode.Contains(item.Name))
                        {
                            cset += item.Name;
                            change = true;
                        }
                        continue;
                    }
                    if (channel.ChannelMode._Mode.Contains(item.Name))
                    {
                        uset += item.Name;
                        change = true;
                    }
                }
                if (change)
                {
                    if (uset == "-")
                    {
                        uset = "";
                    }
                    if (cset == "+")
                    {
                        cset = "";
                    }
                    channel._Network.Transfer("MODE " + channel.Name + " " + cset + uset);
                }
                this.Hide();
                this.Dispose();
            }
            catch (Exception f)
            {
                Core.HandleException(f);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedInvites != null)
                {
                    foreach (libirc.Invite item in SelectedInvites)
                    {
                        channel._Network.Transfer("MODE " + channel.Name + " -I " + item.Target);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void retrieveToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                if (channel != null)
                {
                    channel.ReloadExceptions();
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void retrieveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (channel != null)
                {
                    channel.ReloadInvites();
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void retrieveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (channel != null)
                {
                    channel.ReloadBans();
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void reloadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshBans();
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedBans != null)
                {
                    List<libirc.SimpleMode> mode = new List<libirc.SimpleMode>();
                    foreach (libirc.SimpleBan ban in SelectedBans)
                    {
                        if (Configuration.Parser.formatter)
                        {
                            mode.Add(new libirc.SimpleMode('b', ban.Target));
                        }
                        else
                        {
                            channel._Network.Transfer("MODE " + channel.Name + " -b " + ban.Target);
                        }
                    }
                    if (Configuration.Parser.formatter)
                    {
                        libirc.Formatter formatter = new libirc.Formatter(20, 4);
                        formatter.Prefix = "MODE " + channel.Name + " ";
                        formatter.Removing = true;
                        formatter.InsertModes(mode);
                        Core.ProcessScript(formatter.ToString(), channel._Network);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshInvites();
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedExcept != null)
                {
                    foreach (libirc.ChannelBanException ex in SelectedExcept)
                    {
                        channel._Network.Transfer("MODE " + channel.Name + " -e " + ex.Target);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
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
                Core.HandleException(fail);
            }
        }

        private void InsertMode(char item, ref Gtk.Layout vbox, ref int height)
        {
            string de = "unknown mode. Refer to ircd manual (/raw help)";
            cm.Add(item);
            if (channel._Network.Descriptions.ContainsKey(item))
            {
                de = channel._Network.Descriptions[item];
            }
            CheckButton xx = new CheckButton(item.ToString() + " : " + de);
            xx.Active = channel.ChannelMode._Mode.Contains(item.ToString());
            xx.Name = item.ToString();
            options.Add(xx);
            vbox.Add(xx);
            global::Gtk.Layout.LayoutChild w1 = ((global::Gtk.Layout.LayoutChild)(vbox[xx]));
            w1.X = 0;
            w1.Y = height;
            height += 20;
        }

        private void ReloadModes(ref Gtk.Layout vbox)
        {
            options.Clear();
            if (channel != null)
            {
                RefreshBans();
                ReloadExceptions();
                RefreshInvites();

                lock (channel.ChannelMode)
                {
                    int height = 0;
                    foreach (char item in channel._Network.SModes)
                    {
                        if (channel.ChannelMode._Mode.Contains(item.ToString()))
                        {
                            InsertMode(item, ref vbox, ref height);
                        }
                    }
                    foreach (char item in channel._Network.XModes)
                    {
                        if (channel.ChannelMode._Mode.Contains(item.ToString()))
                        {
                            InsertMode(item, ref vbox, ref height);
                        }
                    }
                    foreach (char item in channel._Network.CModes)
                    {
                        InsertMode(item, ref vbox, ref height);
                    }
                    Adjustment ad = new Adjustment(0, 0, height * 100, 10, 20, 20);
                    vbox.SetScrollAdjustments(new Adjustment(10, 0, 10, 10, 10, 10), ad);
                    this.GtkScrolledWindow1.SetScrollAdjustments(new Adjustment(10, 0, 10, 10, 10, 10), ad);
                    vbox.Height = (uint)height;
                }
            }
        }

        private void Load()
        {
            this.Build();
            this.textview1.WrapMode = WrapMode.Word;
            textview1.Buffer.Text = channel.Topic;
        }
    }
}

