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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Window : UserControl
    {
        public static List<Window> _control = new List<Window>();
        bool initializationComplete;
        bool isDisposing;
        private BufferedGraphicsContext backbufferContext;
        private BufferedGraphics backbufferGraphics;
        private Graphics drawingGraphics;
        public bool Making = true;
        public string name;
        public bool writable;
        public bool isChannel = false;
        public Protocol _Protocol = null;
        private System.Windows.Forms.ListView.SelectedListViewItemCollection SelectedUser = null;
        public bool isPM = false;
        public Network _Network = null;

        public Window()
        {
            lock (_control)
            {
                _control.Add(this);
            }
            this.scrollback = new Client.Scrollback();
        }

        public void Init()
        {
            InitializeComponent();
            this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer, true);
            backbufferContext = BufferedGraphicsManager.Current;
            initializationComplete = true;
            RecreateBuffers();
            scrollback.owner = this;
            listView.View = View.Details;
            listView.Columns.Add(messages.get("list", Core.SelectedLanguage));
            listView.BackColor = Configuration.CurrentSkin.backgroundcolor;
            listView.ForeColor = Configuration.CurrentSkin.fontcolor;
            listViewd.View = View.Details;
            listViewd.Columns.Add(messages.get("list", Core.SelectedLanguage));
            listViewd.BackColor = Configuration.CurrentSkin.backgroundcolor;
            listViewd.ForeColor = Configuration.CurrentSkin.fontcolor;
            listView.Visible = false;
            textbox.history = new List<string>();
            this.Refresh();
        }

        private void RecreateBuffers()
        {
            // Check initialization has completed so we know backbufferContext has been assigned.
            // Check that we aren't disposing or this could be invalid.
            if (!initializationComplete || isDisposing)
                return;

            // We recreate the buffer with a width and height of the control. The "+ 1"
            // guarantees we never have a buffer with a width or height of 0.
            backbufferContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

            // Dispose of old backbufferGraphics (if one has been created already)
            if (backbufferGraphics != null)
                backbufferGraphics.Dispose();

            // Create new backbufferGrpahics that matches the current size of buffer.
            backbufferGraphics = backbufferContext.Allocate(this.CreateGraphics(),
            new Rectangle(0, 0, Math.Max(this.Width, 1), Math.Max(this.Height, 1)));

            // Assign the Graphics object on backbufferGraphics to "drawingGraphics" for easy reference elsewhere.
            drawingGraphics = backbufferGraphics.Graphics;

            // This is a good place to assign drawingGraphics.SmoothingMode if you want a better anti-aliasing technique.

            // Invalidate the control so a repaint gets called somewhere down the line.
            this.Invalidate();
        }

        public void create()
        {
            scrollback.create();
            scrollback.channelToolStripMenuItem.Visible = isChannel;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!isDisposing && backbufferGraphics != null)
                backbufferGraphics.Render(e.Graphics);
        }

        public bool Redraw()
        {
                this.xContainer1.SplitterDistance = Configuration.x1;
                this.xContainer4.SplitterDistance = Configuration.x4;
                listViewd.Columns[0].Width = listView.Width - 8;
                listView.Columns[0].Width = listView.Width - 8;
            this.Refresh();
            return true;
        }


        public void Changed(object sender, SplitterEventArgs dt)
        {
            if (Making == false)
            {
                Configuration.x1 = xContainer1.SplitterDistance;
                Configuration.x4 = xContainer4.SplitterDistance;
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
                return true;
            return base.IsInputKey(keyData);
        }

        private void kickBanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        Channel _channel = _Network.getChannel(name);
                        if (_channel != null)
                        {
                            User target = _channel.userFromName(user.Text);
                            if (target != null)
                            {
                                switch (Configuration.DefaultBans)
                                {
                                    case Configuration.TypeOfBan.Host:
                                        if (target.Host != "")
                                        { 
                                            Core.network._protocol.Transfer("MODE " + name + " +b *!*@" + target.Host, Configuration.Priority.High);
                                        }
                                        break;
                                }
                            }
                        }
                        Core.network._protocol.Transfer("KICK " + name + " " + user.Text + " " + Configuration.DefaultReason, Configuration.Priority.High);
                    }
                }
            }
        }

        private void textbox_Load(object sender, EventArgs e)
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
        }

        private void whoisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        if (user.Text != "")
                        {
                            Core.network._protocol.Transfer("WHOIS " + user.Text);
                        }
                    }
                }
            }
        }

        string Decode(string user)
        { 
            foreach(char item in _Network._protocol.UChars)
            {
                if (user.Contains(item))
                {
                    user = user.Replace(item.ToString(), "");
                }
            }
            return user;
        }

        double Mode(string mode)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        if (user.Text != "")
                        {
                            Core.network._protocol.Transfer("MODE " + name + " " + mode + " " + Decode(user.Text));
                        }
                    }
                }
            }
            return 0;
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
            if (isChannel)
            {
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        if (user.Text != "")
                        {
                            Core.network._protocol.Transfer("KICK " + name + " " + Decode(user.Text) + " " + Configuration.DefaultReason, Configuration.Priority.High);
                        }
                    }
                }
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
            if (isChannel)
            {
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        Channel _channel = _Network.getChannel(name);
                        if (_channel != null)
                        {
                            User target = _channel.userFromName(user.Text);
                            if (target != null)
                            {
                                switch (Configuration.DefaultBans)
                                {
                                    case Configuration.TypeOfBan.Host:
                                        if (target.Host != "")
                                        {
                                            Core.network._protocol.Transfer("MODE " + name + " +b *!*@" + target.Host, Configuration.Priority.High);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void krToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        Core.network._protocol.Transfer("KICK " + name + " " + Decode(user.Text) + " " + Configuration.DefaultReason);
                    }
                }
            }
        }

        private void kickrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        string reason = Configuration.DefaultReason;
                        
                    }
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (_Network == null)
                    return;

                Channel item = _Network.getChannel(name);
                if (item != null)
                {
                    item.redrawUsers();
                }
            }
        }

        private void vERSIONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {

                }
            }
        }

        private void pINGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {

                }
            }
        }

        private void pAGEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {

                }
            }
        }

        private void tIMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {

                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            isDisposing = true;
            lock (_control)
            {
                if (_control.Contains(this))
                {
                    _control.Remove(this);
                }
            }
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                // We must dispose of backbufferGraphics before we dispose of backbufferContext or we will get an exception.
                if (backbufferGraphics != null)
                    backbufferGraphics.Dispose();
                if (backbufferContext != null)
                    backbufferContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private void listViewd_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedUser = listViewd.SelectedItems;
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedUser = listView.SelectedItems;
        }

        private void messageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isChannel)
            {
                if (SelectedUser != null)
                {
                    foreach (System.Windows.Forms.ListViewItem user in SelectedUser)
                    {
                        if (user.Text != "")
                        {
                            if (!Core.network._protocol.windows.ContainsKey(user.Text))
                            {
                                _Network.Private(user.Text);
                            }
                            _Network._protocol.ShowChat(user.Text);
                        }
                    }
                }
            }
        }
    }
}
