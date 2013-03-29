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


namespace Client.Graphics
{
    [System.ComponentModel.ToolboxItem(true)]
    public partial class Window : Gtk.Bin
    {
        /// <summary>
        /// If true the windows is being made
        /// </summary>
        public bool Making = true;
        /// <summary>
        /// Name
        /// </summary>
        public string name = null;
        /// <summary>
        /// Specifies if it's possible to write into this window
        /// </summary>
        public bool writable = true;
        /// <summary>
        /// Whether it's a channel or not
        /// </summary>
        public bool isChannel = false;
        /// <summary>
        /// Lock the window for any changes
        /// </summary>
        public bool Locked = false;
        public int locktime = 0;
        public TreeIter treeNode;
        public System.Drawing.Color MenuColor;
        /// <summary>
        /// Deprecated, use _Network._Protocol instead
        /// </summary>
        public Protocol _Protocol = null;
        /// <summary>
        /// In case this is true, we are in micro chat
        /// </summary>
        public bool MicroBox = false;
        public bool isPM = false;
        public Network _Network = null;
        public bool Resizing = false;
        public bool ignoreChange = false;
        private Channel channel = null;
        public Gtk.ListStore UserList = new Gtk.ListStore(typeof(string), typeof(User), typeof(string));
        public bool isInitialised = false;

        // menu
        public GTK.Menu messageToolStripMenuItem = new GTK.Menu("Message");
        public GTK.Menu modeToolStripMenuItem = new GTK.Menu(messages.get("mode", Core.SelectedLanguage));
        public GTK.Menu kbToolStripMenuItem = new GTK.Menu(messages.get("kickban+text", Core.SelectedLanguage));
        public GTK.Menu krToolStripMenuItem = new GTK.Menu(messages.get("kick-text", Core.SelectedLanguage));
        public GTK.Menu vToolStripMenuItem = new GTK.Menu(messages.get("give+v", Core.SelectedLanguage));
        public GTK.Menu hToolStripMenuItem = new GTK.Menu(messages.get("give+h", Core.SelectedLanguage));
        public GTK.Menu oToolStripMenuItem = new GTK.Menu(messages.get("give+o", Core.SelectedLanguage));
        public GTK.Menu aToolStripMenuItem = new GTK.Menu(messages.get("give+a", Core.SelectedLanguage));
        public GTK.Menu qToolStripMenuItem = new GTK.Menu(messages.get("give+q", Core.SelectedLanguage));
        public GTK.Menu vToolStripMenuItem1 = new GTK.Menu(messages.get("give-v", Core.SelectedLanguage));
        public GTK.Menu hToolStripMenuItem1 = new GTK.Menu(messages.get("give-h", Core.SelectedLanguage));
        public GTK.Menu oToolStripMenuItem1 = new GTK.Menu(messages.get("give-o", Core.SelectedLanguage));
        public GTK.Menu aToolStripMenuItem1 = new GTK.Menu(messages.get("give-a", Core.SelectedLanguage));
        public GTK.Menu qToolStripMenuItem1 = new GTK.Menu(messages.get("give-q", Core.SelectedLanguage));
        public GTK.Menu banToolStripMenuItem = new GTK.Menu(messages.get("ban", Core.SelectedLanguage));
        public GTK.Menu whoisToolStripMenuItem = new GTK.Menu("Whois");
        public GTK.Menu ctToolStripMenuItem = new GTK.Menu("CTCP");
        public GTK.Menu refreshToolStripMenuItem = new GTK.Menu("Refresh");
        public GTK.Menu kickBanToolStripMenuItem = new GTK.Menu("Kick + Ban");
        public GTK.Menu kickToolStripMenuItem = new GTK.Menu("Kick");
        public GTK.Menu synchroToolStripMenuItem = new GTK.Menu("Reload");

        // window
        private global::Gtk.VPaned vpaned1;
        private global::Gtk.HPaned hpaned1;
        private global::Client.Scrollback scrollback1;
        private global::Gtk.ScrolledWindow GtkScrolledWindow1;
        private global::Gtk.TreeView listView;
        private global::Client.Graphics.TextBox textbox1;

        public List<User> SelectedUsers
        {
            get
            {
                List<User> ul = new List<User>();
                TreeIter iter;
                TreePath[] path = this.listView.Selection.GetSelectedRows();
                foreach (TreePath tree in path)
                {
                    this.listView.Model.GetIter(out iter, tree);
                    User user = (User)this.listView.Model.GetValue(iter, 1);
                    ul.Add(user);
                }
                return ul;
            }
        }

        public Scrollback scrollback
        {
            get
            {
                return scrollback1;
            }
        }

        public Graphics.TextBox textbox
        {
            get
            {
                return textbox1;
            }
        }

        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Graphics.Window
            global::Stetic.BinContainer.Attach(this);
            this.Name = "Client.Graphics.Window";
            // Container child Client.Graphics.Window.Gtk.Container+ContainerChild
            this.vpaned1 = new global::Gtk.VPaned();
            this.vpaned1.CanFocus = true;
            this.vpaned1.Name = "vpaned1";
            this.vpaned1.Position = 319;
            // Container child vpaned1.Gtk.Paned+PanedChild
            this.hpaned1 = new global::Gtk.HPaned();
            this.hpaned1.CanFocus = true;
            this.hpaned1.Name = "hpaned1";
            this.hpaned1.Position = 333;
            // Container child hpaned1.Gtk.Paned+PanedChild
            this.scrollback1.Events = ((global::Gdk.EventMask)(256));
            this.scrollback1.Name = "scrollback1";
            this.hpaned1.Add(this.scrollback1);
            global::Gtk.Paned.PanedChild w1 = ((global::Gtk.Paned.PanedChild)(this.hpaned1[this.scrollback1]));
            w1.Resize = false;
            // Container child hpaned1.Gtk.Paned+PanedChild
            this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
            this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
            this.listView = new global::Gtk.TreeView();
            this.listView.ButtonPressEvent += new ButtonPressEventHandler(Menu2);
            this.listView.CanFocus = true;
			this.listView.ButtonPressEvent += new ButtonPressEventHandler(Ignore);
            this.listView.PopupMenu += new PopupMenuHandler(Menu);
            this.listView.Name = "listView";
            this.listView.Selection.Mode = SelectionMode.Multiple;
            this.GtkScrolledWindow1.Add(this.listView);
            this.hpaned1.Add(this.GtkScrolledWindow1);
            this.vpaned1.Add(this.hpaned1);
            global::Gtk.Paned.PanedChild w4 = ((global::Gtk.Paned.PanedChild)(this.vpaned1[this.hpaned1]));
            w4.Resize = false;
            // Container child vpaned1.Gtk.Paned+PanedChild
            this.textbox1.Events = ((global::Gdk.EventMask)(256));
            this.textbox1.Name = "textbox1";
            this.vpaned1.Add(this.textbox1);
            this.vpaned1.AddNotification("position", new GLib.NotifyHandler(Changed));
            this.hpaned1.AddNotification("position", new GLib.NotifyHandler(Changed));
            this.Add(this.vpaned1);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Hide();
        }

        private void UserListRendererTool(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            try
            {
                User user = (User)model.GetValue(iter, 1);
                switch (user.Status)
                {
                    case User.ChannelStatus.Owner:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(Configuration.CurrentSkin.colorq);
                        break;
                    case User.ChannelStatus.Admin:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(Configuration.CurrentSkin.colora);
                        break;
                    case User.ChannelStatus.Op:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(Configuration.CurrentSkin.coloro);
                        break;
                    case User.ChannelStatus.Voice:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(Configuration.CurrentSkin.colorv);
                        break;
                    case User.ChannelStatus.Regular:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(Configuration.CurrentSkin.colordefault);
                        break;
                    case User.ChannelStatus.Halfop:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.fromColor(Configuration.CurrentSkin.colorh);
                        break;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void InitStyle()
        {
            listView.ModifyBase(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.backgroundcolor));
            listView.ModifyText(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.colordefault));
        }

        public Window()
        {
            this.scrollback1 = new global::Client.Scrollback();
            this.textbox1 = new global::Client.Graphics.TextBox();
            MenuColor = Configuration.CurrentSkin.colordefault;
            textbox1.parent = this;
            if (textbox1.history == null)
            {
                textbox1.history = new List<string>();
            }
        }
		
		[GLib.ConnectBefore]
		private void Ignore(object sender, Gtk.ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3)
			{
				e.RetVal = true;
			}
		}
		
        public void Init()
        {
            this.scrollback.owner = this;
            this.scrollback.Create();
            this.textbox.Init();
            this.Build();
            this.InitStyle();
            kbToolStripMenuItem.Enabled = false;
            krToolStripMenuItem.Enabled = false;
            Gtk.TreeViewColumn column1 = new TreeViewColumn();
			listView.TooltipColumn = 2;
            column1.Title = (messages.get("list", Core.SelectedLanguage));
            listView.AppendColumn(column1);
            listView.Model = UserList;
            Gtk.CellRendererText renderer = new CellRendererText();
            column1.PackStart(renderer, true);
            column1.SetCellDataFunc(renderer, UserListRendererTool);
            column1.AddAttribute(renderer, "text", 0);
        }

        public void Create()
        {
            scrollback.channelToolStripMenuItem.Visible = isChannel;
            scrollback.retrieveTopicToolStripMenuItem.Visible = isChannel;
            if (!isChannel)
            {
                banToolStripMenuItem.Visible = false;
                whoisToolStripMenuItem.Visible = false;
                kbToolStripMenuItem.Visible = false;
                whoisToolStripMenuItem.Visible = false;
                ctToolStripMenuItem.Visible = false;
                refreshToolStripMenuItem.Visible = false;
                kickBanToolStripMenuItem.Visible = false;
                modeToolStripMenuItem.Visible = false;
                kickToolStripMenuItem.Visible = false;
                krToolStripMenuItem.Visible = false;
            }
            else
            {
                messageToolStripMenuItem.Enabled = true;
                banToolStripMenuItem.Enabled = true;
                whoisToolStripMenuItem.Enabled = true;
                kbToolStripMenuItem.Enabled = false;
                whoisToolStripMenuItem.Enabled = true;
                ctToolStripMenuItem.Enabled = true;
                refreshToolStripMenuItem.Enabled = true;
                kickBanToolStripMenuItem.Enabled = true;
                modeToolStripMenuItem.Enabled = true;
                kickToolStripMenuItem.Enabled = true;
                krToolStripMenuItem.Enabled = false;
                banToolStripMenuItem.Visible = true;
                whoisToolStripMenuItem.Visible = true;
                kbToolStripMenuItem.Visible = true;
                whoisToolStripMenuItem.Visible = true;
                ctToolStripMenuItem.Visible = true;
                messageToolStripMenuItem.Visible = true;
                refreshToolStripMenuItem.Visible = true;
                kickBanToolStripMenuItem.Visible = true;
                modeToolStripMenuItem.Visible = true;
                kickToolStripMenuItem.Visible = true;
                krToolStripMenuItem.Visible = true;
                synchroToolStripMenuItem.Visible = true;
            }
            if (scrollback.owner == null || scrollback.owner._Network == null)
            {
                scrollback.listAllChannelsToolStripMenuItem.Visible = false;
            }
            Redraw();
            isInitialised = true;
        }

        public bool Redraw()
        {
            ignoreChange = true;
            if (hpaned1 != null)
            {
                if (this.hpaned1.Position != Configuration.Window.x1)
                {
                    hpaned1.Position = Configuration.Window.x1;
                }
                if (this.vpaned1.Position != Configuration.Window.x4)
                {
                    vpaned1.Position = Configuration.Window.x4;
                }
            }
            ignoreChange = false;
            return true;
        }

        public void Changed(object sender, GLib.NotifyArgs dt)
        {
            try
            {
                if (Making == false && ignoreChange == false)
                {
                    Configuration.Window.x1 = hpaned1.Position;
                    Configuration.Window.x4 = vpaned1.Position;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        [GLib.ConnectBefore]
        private void Menu2(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                Menu(sender, null);
            }
        }

        private void Menu(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                bool display = false;
                Gtk.Menu menu = new Menu();
                if (whoisToolStripMenuItem.Visible)
                {
                    display = true;
                    Gtk.MenuItem whois = new MenuItem("Whois");
                    whois.Sensitive = whoisToolStripMenuItem.Enabled;
                    whois.Activated += new EventHandler(whoisToolStripMenuItem_Click);
                    menu.Append(whois);
                }

                if (messageToolStripMenuItem.Visible)
                {
                    display = true;
                    Gtk.MenuItem message = new MenuItem("Message");
                    message.Sensitive = whoisToolStripMenuItem.Enabled;
                    message.Activated += new EventHandler(messageToolStripMenuItem_Click);
                    menu.Append(message);
                }

                if (modeToolStripMenuItem.Visible)
                {
                    display = true;
                    Gtk.Menu changemode = new Gtk.Menu();
                    Gtk.MenuItem change = new MenuItem("Change mode");
                    change.Sensitive = modeToolStripMenuItem.Enabled;
                    change.Submenu = changemode;
                    menu.Append(change);
                    Gtk.MenuItem qp = new MenuItem(qToolStripMenuItem.Text);
                    changemode.Append(qp);
                    qp.Activated += new EventHandler(qToolStripMenuItem_Click);
                    Gtk.MenuItem ap = new MenuItem(aToolStripMenuItem.Text);
                    changemode.Append(ap);
                    ap.Activated += new EventHandler(aToolStripMenuItem_Click);
                    Gtk.MenuItem op = new MenuItem(oToolStripMenuItem.Text);
                    changemode.Append(op);
                    op.Activated += new EventHandler(oToolStripMenuItem_Click);
                    Gtk.MenuItem hp = new MenuItem(hToolStripMenuItem.Text);
                    changemode.Append(hp);
                    hp.Activated += new EventHandler(hToolStripMenuItem_Click);
                    Gtk.MenuItem vp = new MenuItem(vToolStripMenuItem.Text);
                    changemode.Append(vp);
                    vp.Activated += new EventHandler(vToolStripMenuItem_Click);
                    Gtk.SeparatorMenuItem separator4 = new Gtk.SeparatorMenuItem();
                    separator4.Show();
                    changemode.Append(separator4);
                    Gtk.MenuItem qp2 = new MenuItem(qToolStripMenuItem1.Text);
                    changemode.Append(qp2);
                    qp2.Activated += new EventHandler(qToolStripMenuItem1_Click);
                    Gtk.MenuItem ap2 = new MenuItem(aToolStripMenuItem1.Text);
                    changemode.Append(ap2);
                    ap2.Activated += new EventHandler(aToolStripMenuItem1_Click);
                    Gtk.MenuItem op2 = new MenuItem(oToolStripMenuItem1.Text);
                    changemode.Append(op2);
                    op2.Activated += new EventHandler(oToolStripMenuItem1_Click);
                    Gtk.MenuItem hp2 = new MenuItem(hToolStripMenuItem1.Text);
                    changemode.Append(hp2);
                    hp2.Activated += new EventHandler(hToolStripMenuItem1_Click);
                    Gtk.MenuItem vp2 = new MenuItem(vToolStripMenuItem1.Text);
                    changemode.Append(vp2);
                    vp2.Activated += new EventHandler(vToolStripMenuItem1_Click);
                }

                if (ctToolStripMenuItem.Visible)
                {
                    display = true;
                    Gtk.MenuItem CTCP = new MenuItem(ctToolStripMenuItem.Text);
                    Gtk.SeparatorMenuItem separator1 = new Gtk.SeparatorMenuItem();
                    separator1.Show();
                    menu.Append(separator1);
                }

                if (kickBanToolStripMenuItem.Visible)
                {
                    display = true;
                    Gtk.MenuItem kick = new MenuItem(kickToolStripMenuItem.Text);
                    kick.Activated += new EventHandler(kickToolStripMenuItem_Click);
                    menu.Append(kick);
                    Gtk.MenuItem kickban = new MenuItem(kickBanToolStripMenuItem.Text);
                    kickban.Activated += new EventHandler(kickBanToolStripMenuItem_Click);
                    menu.Append(kickban);
                    Gtk.MenuItem kr = new MenuItem(krToolStripMenuItem.Text);
                    kr.Sensitive = krToolStripMenuItem.Enabled;
                    kr.Activated += new EventHandler(krToolStripMenuItem_Click);
                    menu.Append(kr);
                    Gtk.MenuItem kb = new MenuItem(kbToolStripMenuItem.Text);
                    kb.Sensitive = kbToolStripMenuItem.Enabled;
                    kb.Activated += new EventHandler(kickrToolStripMenuItem_Click);
                    menu.Append(kb);
                    Gtk.SeparatorMenuItem separator6 = new Gtk.SeparatorMenuItem();
                    separator6.Show();
                    menu.Append(separator6);
                }

                if (refreshToolStripMenuItem.Visible)
                {
                    display = true;
                    Gtk.MenuItem refresh = new MenuItem(refreshToolStripMenuItem.Text);
                    refresh.Activated += new EventHandler(refreshToolStripMenuItem_Click);
                    menu.Append(refresh);
                    Gtk.MenuItem reload = new MenuItem(synchroToolStripMenuItem.Text);
                    reload.Activated += new EventHandler(synchroToolStripMenuItem_Click);
                    menu.Append(reload);
                }

                if (display)
                {
                    menu.ShowAll();
                    menu.Popup();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void kickBanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                try
                {
                    string script = "";
                    if (isChannel)
                    {
                        foreach (User user in SelectedUsers)
                        {
                            string current_ban = "";
                            Channel _channel = getChannel();
                            if (_channel != null)
                            {
                                switch (Configuration.irc.DefaultBans)
                                {
                                    case Configuration.TypeOfBan.Host:
                                        if (user.Host != "")
                                        {
                                            current_ban = "MODE " + name + " +b *!*@" + user.Host;
                                            script += current_ban + Environment.NewLine;
                                        }
                                        else
                                        {
                                            script += "# can't find hostname for " + user.Nick + " skipping this ban\n";
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                script += "# can't find a channel for " + user.Nick + " skipping this ban\n";
                            }
                            string current_kick = "KICK " + name + " " + user.Nick + " :" + Configuration.irc.DefaultReason;
                            script += current_kick + "\n";
                            if (!Configuration.irc.ConfirmAll)
                            {
                                _channel._Network.Transfer(current_ban, Configuration.Priority.High);
                                _channel._Network.Transfer(current_kick, Configuration.Priority.High);
                            }
                        }
                        if (Configuration.irc.ConfirmAll)
                        {
                            Core.ProcessScript(script, _Network);
                        }
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }
        }

        public void SendCtcp(string message)
        {
            if (isChannel)
            {
                foreach (User user in SelectedUsers)
                {
                    Channel _channel = getChannel();
                    if (_channel != null)
                    {
                        if (Configuration.irc.DisplayCtcp)
                        {
                            _channel._Network._Protocol.Windows["!" + _channel._Network.window].scrollback.InsertText("[CTCP] " + user.Nick + ": " + message, ContentLine.MessageStyle.User);
                        }
                        _channel._Network.Transfer("PRIVMSG " + user.Nick + " :" + _Protocol.delimiter + message + _Protocol.delimiter);
                    }
                }

            }
        }

        private void whoisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        Core.network.Transfer("WHOIS " + user.Nick);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        bool Mode(string mode)
        {
            try
            {
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        Core.network.Transfer("MODE " + name + " " + mode + " " + user.Nick);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
        }

        private void qToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode("+q");
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode("+a");
        }

        private void qToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Mode("-q");
        }

        private void aToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Mode("-a");
        }

        private void oToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Mode("-o");
        }

        private void kickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isChannel)
                {
                    return;
                }
                string script = "";
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        string current_kick = "KICK " + name + " " + user.Nick + " :" + Configuration.irc.DefaultReason;
                        script += current_kick + "\n";
                        if (!Configuration.irc.ConfirmAll)
                        {
                            _Network.Transfer(current_kick, Configuration.Priority.High);
                        }
                    }
                    if (Configuration.irc.ConfirmAll)
                    {
                        Core.ProcessScript(script, _Network);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void hToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Mode("-h");
        }

        private void vToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Mode("-v");
        }

        private void oToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode("+o");
        }

        private void hToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode("+h");
        }

        private void vToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode("+v");
        }

        private void banToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    string script = "";
                    foreach (User user in SelectedUsers)
                    {
                        string mode = "";
                        Channel _channel = getChannel();
                        if (_channel != null)
                        {
                            switch (Configuration.irc.DefaultBans)
                            {
                                case Configuration.TypeOfBan.Host:
                                    if (user.Host != "")
                                    {
                                        mode = "MODE " + name + " +b *!*@" + user.Host;
                                        script += mode + Environment.NewLine;
                                    }
                                    else
                                    {
                                        script += "# can't find hostname for " + user.Nick + " skipping this ban\n";
                                    }
                                    break;
                            }
                            if (!Configuration.irc.ConfirmAll)
                            {
                                Core.network.Transfer(mode, Configuration.Priority.High);
                            }
                        }
                        else
                        {
                            script += "# can't find a channel for " + user.Nick + " skipping this ban\n";
                        }
                    }
                    if (Configuration.irc.ConfirmAll)
                    {
                        Core.ProcessScript(script, _Network);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void krToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        Core.network.Transfer("KICK " + name + " " + user.Nick + " :" + Configuration.irc.DefaultReason);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void kickrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        string reason = Configuration.irc.DefaultReason;
                    }
                }
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
                if (isChannel)
                {
                    if (_Network == null)
                        return;

                    Channel item = getChannel();
                    if (item != null)
                    {
                        Locked = false;
                        item.redrawUsers();
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void vERSIONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    SendCtcp("VERSION");
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void pINGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    SendCtcp("PING " + DateTime.Now.ToBinary().ToString());
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public Channel getChannel()
        {
            if (channel != null)
            {
                return channel;
            }
            if (isChannel)
            {
                if (_Network != null)
                {
                    Channel chan = _Network.getChannel(name);
                    if (chan != null)
                    {
                        channel = chan;
                        return channel;
                    }
                }
            }
            return null;
        }

        private void pAGEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    SendCtcp("PAGE");
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void tIMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    SendCtcp("TIME");
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void messageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        string nickname = user.Nick;
                        if (nickname != "")
                        {
                            if (!Core.network._Protocol.Windows.ContainsKey(_Network.window + nickname))
                            {
                                _Network.Private(nickname);
                            }
                            _Network._Protocol.ShowChat(_Network.window + nickname);
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void synchroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (_Network.Connected)
                {
                    Channel channel = getChannel();
                    if (channel != null)
                    {
                        channel.UserList.Clear();
                        _Network.Transfer("WHO " + channel.Name);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}

