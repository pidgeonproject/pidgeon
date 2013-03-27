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
        /// <summary>
        /// Deprecated, use _Network._Protocol instead
        /// </summary>
        public Protocol _Protocol = null;
        /// <summary>
        /// In case this is true, we are in micro chat
        /// </summary>
        public bool MicroBox = false;
        private System.Windows.Forms.ListView.SelectedListViewItemCollection SelectedUser = null;
        public bool isPM = false;
        public Network _Network = null;
        public bool Resizing = false;
        public bool ignoreChange = false;
        private Channel channel = null;
        public Gtk.ListStore UserList = new Gtk.ListStore(typeof(string), typeof(User));
        public bool isInitialised = false;
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
                                                       
		public Window ()
		{
            this.scrollback1 = new global::Client.Scrollback();
            this.textbox1 = new global::Client.Graphics.TextBox();
            textbox1.parent = this;
            if (textbox1.history == null)
            {
                textbox1.history = new List<string>();
            }
		}
		
		public void Init()
        {
            this.scrollback.owner = this;
            this.scrollback.Create();
            this.textbox.Init();
            this.Build();
            //kbToolStripMenuIm.Enabled = false;
            //kickrToolStripMenuItem.Enabled = false;
            Gtk.TreeViewColumn column1 = new TreeViewColumn();
            column1.Title = (messages.get("list", Core.SelectedLanguage));
            listView.AppendColumn(column1);
            listView.Model = UserList;
            Gtk.CellRendererText renderer = new CellRendererText();
            column1.PackStart(renderer, true);
            column1.AddAttribute(renderer, "text", 0);
            //listView.BackColor = Configuration.CurrentSkin.backgroundcolor;
            //listView.ForeColor = Configuration.CurrentSkin.fontcolor;
        }

        public void Create()
        {
            //scrollback.channelToolStripMenuItem.Visible = isChannel;
            //scrollback.retrieveTopicToolStripMenuItem.Visible = isChannel;
            if (scrollback.owner == null || scrollback.owner._Network == null)
            {
                //scrollback.listAllChannelsToolStripMenuItem.Visible = false;
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
                    vpaned1.Position  = Configuration.Window.x4;
                }
            }
            ignoreChange = false;
            return true;
        }
		
        public void Changed(object sender, EventArgs dt)
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

        /*


        private void kickBanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                try
                {
                    string script = "";
                    if (isChannel)
                    {
                        if (SelectedUser != null)
                        {
                            foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                            {
                                string current_ban = "";
                                Channel _channel = getChannel();
                                if (_channel != null)
                                {
                                    User target = _channel.userFromName(Decode(user.Text));
                                    if (target != null)
                                    {
                                        switch (Configuration.irc.DefaultBans)
                                        {
                                            case Configuration.TypeOfBan.Host:
                                                if (target.Host != "")
                                                {
                                                    current_ban = "MODE " + name + " +b *!*@" + target.Host;
                                                    script += current_ban + Environment.NewLine;
                                                }
                                                else
                                                {
                                                    script += "# can't find hostname for " + Decode(user.Text) + " skipping this ban" + Environment.NewLine;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        script += "# can't find hostname for " + Decode(user.Text) + " skipping this ban" + Environment.NewLine;
                                    }
                                }
                                else
                                {
                                    script += "# can't find a channel for " + Decode(user.Text) + " skipping this ban" + Environment.NewLine;
                                }
                                string current_kick = "KICK " + name + " " + Decode(user.Text) + " :" + Configuration.irc.DefaultReason;
                                script += current_kick + Environment.NewLine;
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
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        Channel _channel = getChannel();
                        if (_channel != null)
                        {
                            if (Configuration.irc.DisplayCtcp)
                            {
                                _channel._Network._Protocol.Windows["!" + _channel._Network.window].scrollback.InsertText("[CTCP] " + Decode(user.Text) + ": " + message, Scrollback.MessageStyle.User);
                            }
                            _channel._Network.Transfer("PRIVMSG " + Decode(user.Text) + " :" + _Protocol.delimiter + message + _Protocol.delimiter);
                        }
                    }
                }
            }
        }

        private void textbox_Load(object sender, EventArgs e)
        {
            try
            {
                banToolStripMenuItem.Text = messages.get("ban", Core.SelectedLanguage);
                modeToolStripMenuItem.Text = messages.get("mode", Core.SelectedLanguage);
                kbToolStripMenuItem.Text = messages.get("kickban+text", Core.SelectedLanguage);
                kickrToolStripMenuItem.Text = messages.get("kick-text", Core.SelectedLanguage);
                vToolStripMenuItem.Text = messages.get("give+v", Core.SelectedLanguage);
                hToolStripMenuItem.Text = messages.get("give+h", Core.SelectedLanguage);
                oToolStripMenuItem.Text = messages.get("give+o", Core.SelectedLanguage);
                aToolStripMenuItem.Text = messages.get("give+a", Core.SelectedLanguage);
                qToolStripMenuItem.Text = messages.get("give+q", Core.SelectedLanguage);
                vToolStripMenuItem1.Text = messages.get("give-v", Core.SelectedLanguage);
                hToolStripMenuItem1.Text = messages.get("give-h", Core.SelectedLanguage);
                oToolStripMenuItem1.Text = messages.get("give-o", Core.SelectedLanguage);
                aToolStripMenuItem1.Text = messages.get("give-a", Core.SelectedLanguage);
                qToolStripMenuItem1.Text = messages.get("give-q", Core.SelectedLanguage);
                if (!isChannel)
                {
                    banToolStripMenuItem.Visible = false;
                    whoisToolStripMenuItem.Visible = false;
                    toolStripMenuItem1.Visible = false;
                    toolStripMenuItem2.Visible = false;
                    toolStripMenuItem3.Visible = false;
                    kbToolStripMenuItem.Visible = false;
                    whoisToolStripMenuItem.Visible = false;
                    ctToolStripMenuItem.Visible = false;
                    refreshToolStripMenuItem.Visible = false;
                    kickBanToolStripMenuItem.Visible = false;
                    modeToolStripMenuItem.Visible = false;
                    kickToolStripMenuItem.Visible = false;
                    kickrToolStripMenuItem.Visible = false;
                }
                else
                {
                    synchroToolStripMenuItem.Visible = true;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void whoisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChannel)
                {
                    if (SelectedUser != null)
                    {
                        foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                        {
                            if (user.Text != "")
                            {
                                Core.network.Transfer("WHOIS " + Decode(user.Text));
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

        public string Decode(string user)
        {
            foreach (char item in _Network.UChars)
            {
                if (user.Contains(item))
                {
                    user = user.Replace(item.ToString(), "");
                }
            }
            return user;
        }

        bool Mode(string mode)
        {
            try
            {
                if (isChannel)
                {
                    if (SelectedUser != null)
                    {
                        foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                        {
                            if (user.Text != "")
                            {
                                Core.network.Transfer("MODE " + name + " " + mode + " " + Decode(user.Text));
                            }
                        }
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
                    if (SelectedUser != null)
                    {
                        foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                        {
                            string current_kick = "KICK " + name + " " + Decode(user.Text) + " :" + Configuration.irc.DefaultReason;
                            script += current_kick + Environment.NewLine;
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
                    if (SelectedUser != null)
                    {
                        foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                        {
                            string mode = "";
                            Channel _channel = getChannel();
                            if (_channel != null)
                            {
                                User target = _channel.userFromName(Decode(user.Text));
                                if (target != null)
                                {
                                    switch (Configuration.irc.DefaultBans)
                                    {
                                        case Configuration.TypeOfBan.Host:
                                            if (target.Host != "")
                                            {
                                                mode = "MODE " + name + " +b *!*@" + target.Host;
                                                script += mode + Environment.NewLine;
                                            }
                                            else
                                            {
                                                script += "# can't find hostname for " + Decode(user.Text) + " skipping this ban" + Environment.NewLine;
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
                                    script += "# can't find hostname for " + Decode(user.Text) + " skipping this ban" + Environment.NewLine;
                                }
                            }
                            else
                            {
                                script += "# can't find a channel for " + Decode(user.Text) + " skipping this ban" + Environment.NewLine;
                            }
                        }
                        if (Configuration.irc.ConfirmAll)
                        {
                            Core.ProcessScript(script, _Network);
                        }
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
                    if (SelectedUser != null)
                    {
                        foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                        {
                            Core.network.Transfer("KICK " + name + " " + Decode(user.Text) + " :" + Configuration.irc.DefaultReason);
                        }
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
                    if (SelectedUser != null)
                    {
                        foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                        {
                            string reason = Configuration.irc.DefaultReason;
                        }
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
                    if (SelectedUser != null)
                    {
                        SendCtcp("VERSION");
                    }
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
                    if (SelectedUser != null)
                    {
                        SendCtcp("PING " + DateTime.Now.ToBinary().ToString());
                    }
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
                    if (SelectedUser != null)
                    {
                        SendCtcp("PAGE");
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
                    if (SelectedUser != null)
                    {
                        SendCtcp("TIME");
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                lock (Core._Main.sX.Panel2.Controls)
                {
                    if (Core._Main.sX.Panel2.Controls.Contains(this))
                    {
                        Core._Main.sX.Panel2.Controls.Remove(this);
                    }
                }
                if (_Protocol != null)
                {
                    lock (_Protocol.Windows)
                    {
                        if (_Network != null)
                        {
                            if (_Protocol.Windows.ContainsKey(_Network.window + name))
                            {
                                _Protocol.Windows.Remove(_Network.window + name);
                            }
                        }
                        if (_Protocol.Windows.ContainsKey(name))
                        {
                            _Protocol.Windows.Remove(name);
                        }
                    }
                }
                base.Dispose(disposing);
            }
            catch (Exception df)
            {
                Core.handleException(df);
            }
        }

        private void listViewd_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedUser = listViewd.SelectedItems;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void listView_ColumnsChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            try
            {
                if (!Resizing)
                {
                    Resizing = true;
                    listViewd.Columns[0].Width = listView.Columns[0].Width;
                }
                else
                {
                    Resizing = false;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void listViewd_ColumnsChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            try
            {
                if (!Resizing)
                {
                    Resizing = true;
                    listView.Columns[0].Width = listViewd.Columns[0].Width;
                }
                else
                {
                    Resizing = false;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void listWork(object sender, EventArgs e)
        {
            try
            {
                Locked = true;
                lockwork.Stop();
                lockwork.Enabled = true;
                lockwork.Start();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectedUser = listView.SelectedItems;
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
                    if (SelectedUser != null)
                    {
                        foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                        {
                            string nickname = Decode(user.Text);
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

        private void lo_Tick(object sender, EventArgs e)
        {
            try
            {
                Locked = false;
                lockwork.Enabled = false;
                if (isChannel)
                {
                    Channel channel = getChannel();
                    if (channel != null && channel.UserListRefreshWait)
                    {
                        channel.redrawUsers();
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
		*/
	}
}

