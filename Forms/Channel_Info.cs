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

namespace Client.Forms
{
    /// <summary>
    /// Window
    /// </summary>
    public partial class Channel_Info : Client.GTK.PidgeonForm
    {
        private Channel channel = null;
        private List<char> cm = new List<char>();

        private Gtk.ListStore exceptions = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(ChannelBanException));
        private Gtk.ListStore bans = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(SimpleBan));
        private Gtk.ListStore invites = new Gtk.ListStore(typeof(string), typeof(string), typeof(string), typeof(Invite));
        private List<CheckButton> options = new List<CheckButton>();

        /// <summary>
        /// List of selected items
        /// </summary>
        public List<ChannelBanException> SelectedExcept
        {
            get
            {
                List<ChannelBanException> el = new List<ChannelBanException>();
                TreeIter iter;
                TreePath[] path = treeview6.Selection.GetSelectedRows();
                if (path.Length < 1)
                {
                    return null;
                }
                foreach (TreePath p in path)
                {
                    treeview6.Model.GetIter(out iter, p);
                    el.Add((ChannelBanException)treeview6.Model.GetValue(iter, 3));
                }
                return el;
            }
        }

        /// <summary>
        /// List of selected items
        /// </summary>
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

        /// <summary>
        /// List of selected items
        /// </summary>
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

        /// <summary>
        /// Creates a new window
        /// </summary>
        /// <param name="ch"></param>
        public Channel_Info(Channel ch)
        {
            channel = ch;
            Load();
        }

        private void MenuBans(object sender, Gtk.PopupMenuArgs e)
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

        private void ReloadExceptions()
        {
            if (channel != null)
            {
                if (channel.Exceptions != null)
                {
                    exceptions.Clear();
                    lock (channel.Exceptions)
                    {
                        foreach (ChannelBanException sb in channel.Exceptions)
                        {
                            exceptions.AppendValues(sb.Target, convertUNIX(sb.Time) + " (" + sb.Time + ")", sb.User, sb);
                        }
                    }
                }
            }
        }

        private void ReloadBans()
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

        private void ReloadInvites()
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

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (SelectedBans != null)
                {
                    List<SimpleMode> mode = new List<SimpleMode>();
                    foreach (SimpleBan ban in SelectedBans)
                    {
                        if (Configuration.Parser.formatter)
                        {
                            mode.Add(new SimpleMode('b', ban.Target));
                        }
                        else
                        {
                            channel._Network.Transfer("MODE " + channel.Name + " -b " + ban.Target);
                        }
                    }
                    if (Configuration.Parser.formatter)
                    {
                        Protocols.irc.Formatter formatter = new Protocols.irc.Formatter(20, 4);
                        formatter.Prefix = "MODE " + channel.Name + " ";
                        formatter.Removing = true;
                        formatter.InsertModes(mode);
                        Core.ProcessScript(formatter.ToString(), channel._Network);
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
                    foreach (ChannelBanException ex in SelectedExcept)
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
                ReloadBans();
                ReloadExceptions();
                ReloadInvites();

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

        private string convertUNIX(string time)
        {
            try
            {
                if (time == null)
                {
                    return "Unable to read: NULL";
                }
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

