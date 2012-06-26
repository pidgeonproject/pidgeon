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
        public Scrollback _Scrollback;
        public Network._window Chat;

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
            channelList1.Top = listView.Top;
            channelList1.Left = 20;
            channelList1.Height = this.Height - 160;
            Scrollback.Width = this.Width - 380;
            Scrollback.Height = this.Height - 160;
            listView.Left = Scrollback.Left + Scrollback.Width + 20;
            listView.Height = Scrollback.Height + 46;
            MessageLine.Left = Scrollback.Left;
            MessageLine.Width = this.Width - 380;
            MessageLine.Top = Scrollback.Height + 40;
            if (Chat != null)
            {
                Chat.scrollback.Width = this.Width - 380;
                Chat.scrollback.Height = this.Height - 160;
                Chat.userlist.Left = Chat.scrollback.Left + Chat.scrollback.Width + 20;
                Chat.userlist.Height = Chat.scrollback.Height + 46;
            }
        }

        public void Reload()
        {
            _Redraw();
        }

        private void _Enter(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals( '\r' ))
            {
                _Scrollback.Last = MessageLine;
                MessageLine.richTextBox1.Text = MessageLine.richTextBox1.Text.Replace("\n", "");
                MessageLine.history.Add(MessageLine.richTextBox1.Text);
                Parser.parse(MessageLine.richTextBox1.Text);
                MessageLine.richTextBox1.Text = "";
            }
        }

        public Client.Scrollback CreateS()
        {
            Client.Scrollback SB = new Scrollback();
            SB.Location = new System.Drawing.Point(189, 27);
            SB.Visible = false;
            SB.Size = new System.Drawing.Size(465, 195);
            SB.TabIndex = 6;
            SB.CreateControl();
            this.Controls.Add(SB);
            this.PerformLayout();
            return SB;
        }

        public System.Windows.Forms.ListView CreateList()
        {
            System.Windows.Forms.ListView list = new ListView();
            list.Location = new System.Drawing.Point(12, 27);
            list.Size = new System.Drawing.Size(152, 332);
            list.TabIndex = 2;
            list.CreateControl();
            list.Visible = false;
            this.Controls.Add(list);
            this.PerformLayout();
            return list;
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
            preferencesToolStripMenuItem.Text = messages.get("window-menu-conf", Core.SelectedLanguage);
            fileToolStripMenuItem.Text = messages.get("window-menu-file", Core.SelectedLanguage);
            _Scrollback = Scrollback;
            if (Configuration.Window_Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            ResizeMe(null, null);
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



    }
}
