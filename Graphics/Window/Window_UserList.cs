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

namespace Client.Graphics
{
    /// <summary>
    /// Window
    /// </summary>
    public partial class Window : Gtk.Bin
    {
        /// <summary>
        /// User list
        /// </summary>
        public Gtk.ListStore UserList = new Gtk.ListStore(typeof(string), typeof(User), typeof(string));
        // menu
        /// <summary>
        /// Version menu
        /// </summary>
        public GTK.Menu vERSIONToolStripMenuItem = new GTK.Menu("Version");
        /// <summary>
        /// Time menu
        /// </summary>
        public GTK.Menu tIMEToolStripMenuItem = new GTK.Menu("Time");
        /// <summary>
        /// Ping menu
        /// </summary>
        public GTK.Menu pINGToolStripMenuItem = new GTK.Menu("PING");
        /// <summary>
        /// Page menu
        /// </summary>
        public GTK.Menu pAGEToolStripMenuItem = new GTK.Menu("PAGE");
        /// <summary>
        /// Message menu
        /// </summary>
        public GTK.Menu messageToolStripMenuItem = new GTK.Menu("Message");
        /// <summary>
        /// Mode menu
        /// </summary>
        public GTK.Menu modeToolStripMenuItem = new GTK.Menu(messages.get("mode", Core.SelectedLanguage));
        /// <summary>
        /// Kb menu
        /// </summary>
        public GTK.Menu kbToolStripMenuItem = new GTK.Menu(messages.get("kickban+text", Core.SelectedLanguage));
        /// <summary>
        /// Kick with reason menu
        /// </summary>
        public GTK.Menu krToolStripMenuItem = new GTK.Menu(messages.get("kick-text", Core.SelectedLanguage));
        /// <summary>
        /// +V menu
        /// </summary>
        public GTK.Menu vToolStripMenuItem = new GTK.Menu(messages.get("give+v", Core.SelectedLanguage));
        /// <summary>
        /// +H menu
        /// </summary>
        public GTK.Menu hToolStripMenuItem = new GTK.Menu(messages.get("give+h", Core.SelectedLanguage));
        /// <summary>
        /// +O menu
        /// </summary>
        public GTK.Menu oToolStripMenuItem = new GTK.Menu(messages.get("give+o", Core.SelectedLanguage));
        /// <summary>
        /// +A menu
        /// </summary>
        public GTK.Menu aToolStripMenuItem = new GTK.Menu(messages.get("give+a", Core.SelectedLanguage));
        /// <summary>
        /// +Q menu
        /// </summary>
        public GTK.Menu qToolStripMenuItem = new GTK.Menu(messages.get("give+q", Core.SelectedLanguage));
        /// <summary>
        /// -V menu
        /// </summary>
        public GTK.Menu vToolStripMenuItem1 = new GTK.Menu(messages.get("give-v", Core.SelectedLanguage));
        /// <summary>
        /// -H menu
        /// </summary>
        public GTK.Menu hToolStripMenuItem1 = new GTK.Menu(messages.get("give-h", Core.SelectedLanguage));
        /// <summary>
        /// -O menu
        /// </summary>
        public GTK.Menu oToolStripMenuItem1 = new GTK.Menu(messages.get("give-o", Core.SelectedLanguage));
        /// <summary>
        /// -A menu
        /// </summary>
        public GTK.Menu aToolStripMenuItem1 = new GTK.Menu(messages.get("give-a", Core.SelectedLanguage));
        /// <summary>
        /// -Q menu
        /// </summary>
        public GTK.Menu qToolStripMenuItem1 = new GTK.Menu(messages.get("give-q", Core.SelectedLanguage));
        /// <summary>
        /// Ban
        /// </summary>
        public GTK.Menu banToolStripMenuItem = new GTK.Menu(messages.get("ban", Core.SelectedLanguage));
        /// <summary>
        /// Whois
        /// </summary>
        public GTK.Menu whoisToolStripMenuItem = new GTK.Menu("Whois");
        /// <summary>
        /// DirectCC
        /// </summary>
        public GTK.Menu dccToolStripMenu = new GTK.Menu("DCC");
        /// <summary>
        /// Secure chat
        /// </summary>
        public GTK.Menu dccSChatToolStripMenuItem = new GTK.Menu("Secure chat");
        /// <summary>
        /// Chat
        /// </summary>
        public GTK.Menu dccChatToolStripMenuItem = new GTK.Menu("Chat");
        /// <summary>
        /// File
        /// </summary>
        public GTK.Menu dccFileToolStripMenuItem = new GTK.Menu("File");
        /// <summary>
        /// Ctcp
        /// </summary>
        public GTK.Menu ctToolStripMenuItem = new GTK.Menu("CTCP");
        /// <summary>
        /// Refresh menu
        /// </summary>
        public GTK.Menu refreshToolStripMenuItem = new GTK.Menu("Refresh");
        /// <summary>
        /// KickBan menu
        /// </summary>
        public GTK.Menu kickBanToolStripMenuItem = new GTK.Menu("Kick + Ban");
        /// <summary>
        /// Kick menu
        /// </summary>
        public GTK.Menu kickToolStripMenuItem = new GTK.Menu("Kick");
        /// <summary>
        /// Reload menu
        /// </summary>
        public GTK.Menu synchroToolStripMenuItem = new GTK.Menu("Reload");
        
        /// <summary>
        /// List of selected users
        /// </summary>
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
        
        [GLib.ConnectBefore]
        private void Menu2(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                Menu(sender, null);
            }

            if (e.Event.Button == 1 && e.Event.Type == Gdk.EventType.TwoButtonPress)
            {
                Click(Configuration.Window.DoubleClick);
            }

            if (e.Event.Button == 2)
            {
                Click(Configuration.Window.MiddleClick);
            }
        }

        private void Click(Configuration.UserList_MouseClick xx)
        {
            switch (xx)
            {
                case Configuration.UserList_MouseClick.aop:
                    this.aToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.deaop:
                    this.aToolStripMenuItem1_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.dehop:
                    this.hToolStripMenuItem1_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.deop:
                    this.oToolStripMenuItem1_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.deqop:
                    this.qToolStripMenuItem1_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.devoice:
                    this.vToolStripMenuItem1_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.hop:
                    this.hToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.Kick:
                    this.kickToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.KickBan:
                    this.kickBanToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.op:
                    this.oToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.ping:
                    this.pINGToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.qop:
                    this.qToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.version:
                    this.vERSIONToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.voice:
                    this.vToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.Whois:
                    this.whoisToolStripMenuItem_Click(null, null);
                    break;
                case Configuration.UserList_MouseClick.SendMsg:
                    this.messageToolStripMenuItem_Click(null, null);
                    break;
            }
        }
        
        private void UserListRendererTool(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
        {
            try
            {
                User user = (User)model.GetValue(iter, 1);
                if (user.Away)
                {
                    (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.coloraway);
                    return;
                }
                switch (user.Status)
                {
                    case User.ChannelStatus.Owner:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.colorq);
                        break;
                    case User.ChannelStatus.Admin:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.colora);
                        break;
                    case User.ChannelStatus.Op:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.coloro);
                        break;
                    case User.ChannelStatus.Voice:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.colorv);
                        break;
                    case User.ChannelStatus.Regular:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.colordefault);
                        break;
                    case User.ChannelStatus.Halfop:
                        (cell as Gtk.CellRendererText).ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.colorh);
                        break;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
        
        private void Menu(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                bool display = false;
                Gtk.Menu menu = new Menu();

                Hooks._Window.BeforeUserMenu(menu, this);

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

                if (dccToolStripMenu.Visible)
                {
                    Gtk.SeparatorMenuItem separatorX = new Gtk.SeparatorMenuItem();
                    separatorX.Show();
                    menu.Append(separatorX);
                    Gtk.Menu dcc = new Gtk.Menu();
                    Gtk.MenuItem DCC = new MenuItem("DCC");
                    DCC.Submenu = dcc;
                    menu.Append(DCC);
                    Gtk.MenuItem sc = new MenuItem(this.dccSChatToolStripMenuItem.Text);
                    sc.Activated += new EventHandler(dccSChatToolStripMenuItem_Click);
                    dcc.Append(sc);
                    Gtk.MenuItem nc = new MenuItem(this.dccChatToolStripMenuItem.Text);
                    nc.Activated += new EventHandler(dccChatToolStripMenuItem_Click);
                    dcc.Append(nc);
                }

                if (ctToolStripMenuItem.Visible)
                {
                    Gtk.SeparatorMenuItem separatorX = new Gtk.SeparatorMenuItem();
                    separatorX.Show();
                    menu.Append(separatorX);
                    Gtk.Menu ctcp = new Gtk.Menu();
                    Gtk.MenuItem Ctcp = new MenuItem("CTCP");
                    Ctcp.Submenu = ctcp;
                    menu.Append(Ctcp);
                    Gtk.MenuItem ping = new MenuItem(pINGToolStripMenuItem.Text);
                    ping.Activated += new EventHandler(pINGToolStripMenuItem_Click);
                    ctcp.Append(ping);
                    Gtk.MenuItem page = new MenuItem(pAGEToolStripMenuItem.Text);
                    page.Activated += new EventHandler(pAGEToolStripMenuItem_Click);
                    ctcp.Append(page);
                    Gtk.MenuItem version = new MenuItem(vERSIONToolStripMenuItem.Text);
                    version.Activated += new EventHandler(vERSIONToolStripMenuItem_Click);
                    ctcp.Append(version);
                    Gtk.MenuItem dt = new MenuItem(tIMEToolStripMenuItem.Text);
                    dt.Activated += new EventHandler(tIMEToolStripMenuItem_Click);
                    ctcp.Append(dt);
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

                Hooks._Window.AfterUserMenu(menu, this);

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
                                            current_ban = "MODE " + WindowName + " +b *!*@" + user.Host;
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
                            string current_kick = "KICK " + WindowName + " " + user.Nick + " :" + Configuration.irc.DefaultReason;
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
                        string current_kick = "KICK " + WindowName + " " + user.Nick + " :" + Configuration.irc.DefaultReason;
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
                                        mode = "MODE " + WindowName + " +b *!*@" + user.Host;
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
                                Core.SelectedNetwork.Transfer(mode, Configuration.Priority.High);
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

        private void dccChatToolStripMenuItem_Click(object sender,  EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        if (user.Nick != "")
                        {
                            uint port = Core.GetPort();
                            Forms.OpenDCC form = new Forms.OpenDCC("localhost", user.Nick, port, true, false, _Network);
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// This function create a new certificate it is basically called only once
        /// </summary>
        /// <param name="name"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        private static void certificate(string name, string host)
        {
            byte[] c = Client.Protocols.CertificateMaker.CreateSelfSignCertificatePfx(
                "CN=" + host, //host name
                DateTime.Parse("2000-01-01"), //not valid before
                DateTime.Parse("2020-01-01"), //not valid after
                "pidgeon"); //password to encrypt key file

            using (System.IO.BinaryWriter binWriter = new System.IO.BinaryWriter(System.IO.File.Open(name, System.IO.FileMode.Create)))
            {
                binWriter.Write(c);
            }
        }

        private void dccSChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    if (!System.IO.File.Exists(Configuration.irc.CertificateDCC))
                    {
                        if (Configuration.CurrentPlatform != Core.Platform.Windowsx64 && Configuration.CurrentPlatform != Core.Platform.Windowsx86)
                        {
                            GTK.MessageBox error = new GTK.MessageBox(null, MessageType.Error, ButtonsType.Ok, "Warning: In order to open ssl connection you need to have ssl certificate installed in " + Configuration.irc.CertificateDCC + ", but you don't have any! In order to create it use command openssl pkcs12.", "Certificate error");
                            return;
                        }
                        GTK.MessageBox message = new GTK.MessageBox(null, MessageType.Question, ButtonsType.YesNo, "Warning: In order to open ssl connection you need to have ssl certificate installed in " + Configuration.irc.CertificateDCC + ", but you don't have any! Do you want to create a self signed certificate now?", "Certificate error");
                        if (message.result == ResponseType.No)
                        {
                            return;
                        }
                        certificate(Configuration.irc.CertificateDCC, Core.GetIP());
                        if (!System.IO.File.Exists(Configuration.irc.CertificateDCC))
                        {
                            GTK.MessageBox.Show(null, MessageType.Error, ButtonsType.Ok, "Unable to create a certificate file, the error is in ring log", "Certificate error");
                            return;
                        }
                    }
                    foreach (User user in SelectedUsers)
                    {
                        if (user.Nick != "")
                        {
                            uint port = Core.GetPort();
                            Forms.OpenDCC form = new Forms.OpenDCC("localhost", user.Nick, port, true, true, _Network);
                        }
                    }
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
                            if (!Core.SelectedNetwork._Protocol.Windows.ContainsKey(_Network.SystemWindowID + nickname))
                            {
                                _Network.Private(nickname);
                            }
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
                if (_Network.IsConnected)
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
        
        private void krToolStripMenuItem_Click(object sender, EventArgs e)
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
                        string current_kick = "KICK " + WindowName + " " + user.Nick + " :" + Configuration.irc.DefaultReason;
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

        private void kickrToolStripMenuItem_Click(object sender, EventArgs e)
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
                                            current_ban = "MODE " + WindowName + " +b *!*@" + user.Host;
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
                            string current_kick = "KICK " + WindowName + " " + user.Nick + " :" + Configuration.irc.DefaultReason;
                            script += current_kick + "\n";
                        }
                        Core.ProcessScript(script, _Network);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
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
                        item.RedrawUsers();
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
        
        private void SendCtcp(string message)
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
                            _channel._Network._Protocol.Windows["!" + _channel._Network.SystemWindowID].scrollback.InsertText("[CTCP] " + user.Nick + ": " + message, ContentLine.MessageStyle.User);
                        }
                        _channel._Network.Transfer("PRIVMSG " + user.Nick + " :" + _Network._Protocol.delimiter + message + _Network._Protocol.delimiter);
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
                        Core.SelectedNetwork.Transfer("WHOIS " + user.Nick);
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

