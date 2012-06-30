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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Main : Form
    {
        private string StatusBox;
        public Window main;
        public PidgeonList ChannelList;
        public Window Chat;
        private Preferences fPrefs;
        bool done = false;

        public class _WindowRequest 
        {
            public string name;
            public bool _Focus;
            public bool writable;
            public Network owner;
        }

        public List<_WindowRequest> W = new List<_WindowRequest>();

        public Main()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Resize window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeMe(object sender, EventArgs e)
        {
            _Redraw();
        }

        public void _Redraw()
        {

        }

        public void Changed(object sender, EventArgs dt)
        {
            if (done)
            {
                Configuration.window_size = sX.SplitterDistance;
            }
        }

        public void Reload()
        {
            _Redraw();
        }

	    private void _Enter(object sender, KeyPressEventArgs pt){}

        public Client.Window CreateChat()
        {
            Client.Window Chat = new Window();
            Chat.Dock = DockStyle.Fill;
            Chat.Location = new System.Drawing.Point(0, 0);
            Chat.Visible = true;
            Chat.TabIndex = 6;
            Chat.Redraw();
            Chat.CreateControl();
            if (this.Chat != null)
            {
                Chat.textbox.history.AddRange(this.Chat.textbox.history);
            }
            sX.Panel2.Controls.Add(Chat);
            //Chat.Redraw();
            return Chat;
        }

        /// <summary>
        /// Status line
        /// </summary>
        /// <param name="text"></param>
        public void Status(string text)
        {
            StatusBox = text;
            UpdateStatus();
        }

        public void _Load()
        {
            Core.Load();
            shutDownToolStripMenuItem.Text = messages.get("window-menu-quit", Core.SelectedLanguage);
            if (Configuration.Window_Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            if (Configuration.x4 == 0)
            {
                Configuration.window_size = 80;
                Configuration.x1 = Height - 80;
                Configuration.x4 = 600;
                if (Width > 200)
                {
                    Configuration.x4 = this.Width - 200;
                }
            }
            sX.SplitterDistance = Configuration.window_size;
            ChannelList = new PidgeonList();
            ChannelList.Visible = true;
            ChannelList.Size = new System.Drawing.Size(Width, Height - 60);
            ChannelList.Dock = DockStyle.Fill;
            ChannelList.CreateControl();
            sX.Panel1.Controls.Add(ChannelList);
            main = CreateChat();
            preferencesToolStripMenuItem.Text = messages.get("window-menu-conf", Core.SelectedLanguage);
            //checkForAnUpdateToolStripMenuItem.Text = messages.get("check-u", Core.SelectedLanguage);
            Chat = main;
            main.Redraw();
            Reload();
            Chat.Making = false;
            done = true;
            fileToolStripMenuItem.Text = messages.get("window-menu-file", Core.SelectedLanguage);
        }

        public void UpdateStatus()
        {
            statusStrip1.Text = StatusBox;
        }

        private void shutDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Quit();
        }

        private void MessageLine_Load(object sender, EventArgs e)
        {

        }

        private void channelList1_Load(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {
            _Load();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fPrefs == null || fPrefs.IsDisposed)
            {
                fPrefs = new Preferences();
            }
            fPrefs.Show();
        }

        private void newConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help _Help = new Help();
            _Help.Show();
        }



    }
}
