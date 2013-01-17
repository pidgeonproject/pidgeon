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
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Main : Form
    {
        private string StatusBox = "";
        public Window main;
        public PidgeonList ChannelList;
        public Window Chat;
        private Connection fConnection;
        private Preferences fPrefs;
        private bool UpdatedStatus = true;
        SearchItem searchbox = new SearchItem();
        bool done = false;

        public class _WindowRequest 
        {
            public Window window;
            public string name;
            public bool focus;
            public bool writable;
            public Protocol owner;
        }

        public List<_WindowRequest> WindowRequests = new List<_WindowRequest>();

        public Main()
        {
            InitializeComponent();
        }

        public void Changed(object sender, EventArgs dt)
        {
            if (done)
            {
                Configuration.window_size = sX.SplitterDistance;
            }
        }

        /// <summary>
        /// Create a new chat
        /// </summary>
        /// <param name="Chat"></param>
        /// <param name="WindowOwner"></param>
        /// <param name="Focus"></param>
        public void CreateChat(Window Chat, Protocol WindowOwner, bool Focus = true)
        {
            Chat.Init();
            Chat.create();
            Chat.Visible = Focus;
            Chat._Protocol = WindowOwner;
            Chat.Dock = DockStyle.Fill;
            Chat.Location = new System.Drawing.Point(0, 0);
            Chat.Redraw();
            Chat.CreateControl();
            if (Core._Main.Chat != null && Core._Main.Chat.textbox != null)
            {
                Chat.textbox.history.AddRange(Core._Main.Chat.textbox.history);
            }
            lock (Core._Main.sX.Panel2.Controls)
            {
                Core._Main.sX.Panel2.Controls.Add(Chat);
            }
        }

        /// <summary>
        /// Status line text (this method is thread safe)
        /// </summary>
        /// <param name="text"></param>
        public void Status(string text = null)
        {
            if (text != null)
            {
                StatusBox = text;
            }
            UpdatedStatus = true;
        }

        public void _Load()
        {
            fileToolStripMenuItem.Text = messages.get("window-menu-file", Core.SelectedLanguage);
            shutDownToolStripMenuItem.Text = messages.get("window-menu-quit", Core.SelectedLanguage);
            if (Configuration.Window_Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            try
            {
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
                main = new Window();
                CreateChat(main, null);
                main.name = "Pidgeon";
                preferencesToolStripMenuItem.Text = messages.get("window-menu-conf", Core.SelectedLanguage);
                toolStripStatusNetwork.ToolTipText = "windows / channels / pm";
                //checkForAnUpdateToolStripMenuItem.Text = messages.get("check-u", Core.SelectedLanguage);
                Chat = main;
                main.Redraw();
                Chat.Making = false;
                if (Configuration.Debugging)
                {
                    Core.PrintRing(Chat, false);
                }
                Chat.scrollback.InsertText("Welcome to pidgeon client " + Application.ProductVersion, Scrollback.MessageStyle.System, false, 0, true);
                if (Core.Extensions.Count > 0)
                {
                    foreach (Extension nn in Core.Extensions)
                    {
                        Chat.scrollback.InsertText("Extension " + nn.Name + " (" + nn.Version + ")", Scrollback.MessageStyle.System, false, 0, true);
                    }
                }
                done = true;
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
                return true;
            return base.IsInputKey(keyData);
        }

        public static bool ShortcutHandle(Object sender, KeyEventArgs e)
        {
            bool rt = false;
            try
            {
                foreach (Core.Shortcut shortcut in Configuration.ShortcutKeylist)
                {
                    if (shortcut.control == e.Control && shortcut.keys == e.KeyCode && shortcut.alt == e.Alt)
                    {
                        Parser.parse(shortcut.data);
                        rt = true;
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return rt;
        }

        public void UpdateStatus()
        {
            this.toolStripInfo.Text = StatusBox;
            if (Core.network != null)
            {
                toolStripStatusNetwork.Text = Core.network.server + "    w/c/p " + Core.network._protocol.windows.Count.ToString() + "/" + Core.network.Channels.Count.ToString() + "/" + Core.network.PrivateChat.Count.ToString();
                if (Core.network.RenderedChannel != null)
                {
                    string info = "";
                    if (Core.network.RenderedChannel.Bl != null)
                    {
                        info += Core.network.RenderedChannel.Bl.Count.ToString();
                    } else
                    {
                        info += "??";
                    }
                    info = info + " / ";

                    if (Core.network.RenderedChannel.Invites != null)
                    {
                        info += Core.network.RenderedChannel.Invites.Count.ToString();
                    } else
                    {
                        info += "??";
                    }
                    info = info + " / ";
                    if (Core.network.RenderedChannel.Exceptions != null)
                    {
                        info += Core.network.RenderedChannel.Exceptions.Count.ToString();
                    } else
                    {
                        info += "??";
                    }
                    setText(Core.network.RenderedChannel.Name + " - " + Core.network.RenderedChannel.Topic);
                    toolStripStatusChannel.Text = Core.network.RenderedChannel.Name + " u: " + Core.network.RenderedChannel.UserList.Count + " m: " + Core.network.RenderedChannel._mode.ToString() + " b/i/e: " + info;
                }
            }
        }

        public int setText(string name)
        {
            Text = "Pidgeon Client v 1.0 " + name;
            return 2;
        }

        private void shutDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Quit();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _Load();
            //miscToolStripMenuItem.Visible = false;
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fPrefs == null || fPrefs.IsDisposed)
            {
                fPrefs = new Preferences();
            }
            fPrefs.Show();
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://pidgeonclient.org/wiki/Help:Contents");
        }

        public void Unshow(object main, FormClosingEventArgs closing)
        {
            if (Core.IgnoreErrors)
            {
                Core.DebugLog("Closing main");
                closing.Cancel = false;
                return;
            }
                if (MessageBox.Show(messages.get("pidgeon-shut", Core.SelectedLanguage), "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    closing.Cancel = true;
                    return;
                }
                Core.Quit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help _Help = new Help();
            _Help.Show();
        }

        private void newConnectionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (fConnection == null || fConnection.IsDisposed)
                fConnection = new Connection();


            fConnection.Show();
        }

        private void updater_Tick(object sender, EventArgs e)
        {
            // Someone updated status, maybe from outer thread, let's change it
            if (UpdatedStatus)
            {
                UpdatedStatus = false;
                UpdateStatus();
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Core.trafficscanner.Show();
        }

        private void taskbarBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MicroChat.mc.Show();
        }

        private void attachToMicroChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Chat != null)
            {
                Chat.MicroBox = true;
            }
        }

        private void detachFromMicroChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Chat != null)
            {
                Chat.MicroBox = false;
            }
        }

        private void rootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Core.network != null)
            {
                Core.network.RenderedChannel = null;
                main.Visible = true;
                Core.network._protocol.Current.Visible = false;
                Core.network._protocol.Current = main;
                return;
            }
            main.Visible = true;
            main.BringToFront();
        }

        protected void Wheeled(object sender, MouseEventArgs md)
        {
            if (Chat != null)
            {
                Chat.scrollback.RT.Wheeled(sender, md);
            }  
        }

        private void pidgeonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Protocol network in Core.Connections)
            {
                if (network.type == 2)
                {
                    ProtocolSv sv = (ProtocolSv)network;
                    foreach (Network server in sv.NetworkList)
                    {
                        if (server.server == "irc.tm-irc.org")
                        {
                            server._protocol.Join("#pidgeon", server);
                            return;
                        }
                    }
                }
                if (network.Server == "irc.tm-irc.org")
                {
                    network.Join("#pidgeon");
                    return;
                }
            }
            Core.connectIRC("irc.tm-irc.org");
            foreach (Protocol network in Core.Connections)
            {
                if (network.Server == "irc.tm-irc.org")
                {
                    network.Join("#pidgeon");
                    return;
                }
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (searchbox.Visible)
            {
                searchbox.Hide();
                return;
            }
            searchbox.Show();
        }

        private void switchToAdvancedLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Chat != null)
            {
                Chat.scrollback.Switch(true);
            }
        }

        private void configurationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsEd ed = new SettingsEd();
            ed.Show(this);
        }

        private void skinEdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SkinEd skined = new SkinEd();
                skined.Show();
            }
            catch (Exception x)
            {
                Core.handleException(x);
            }
        }
    }
}
