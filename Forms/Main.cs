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
        public int progress = 0;
        public bool DisplayingProgress = false;
        public int ProgressMax = 0;

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
                Configuration.Window.window_size = sX.SplitterDistance;
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
            Chat.Create();
            Chat.Visible = Focus;
            Chat._Protocol = WindowOwner;
            Chat.Dock = DockStyle.Fill;
            Chat.Location = new System.Drawing.Point(0, 0);
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
            try
            {
                messages.Localize(this);
                skinEdToolStripMenuItem.Enabled = false;
                if (Configuration.Window.Window_Maximized)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                if (Configuration.Window.x4 == 0)
                {
                    Configuration.Window.window_size = 80;
                    Configuration.Window.x1 = Height - 80;
                    Configuration.Window.x4 = 600;
                    if (Width > 200)
                    {
                        Configuration.Window.x4 = this.Width - 200;
                    }
                }
                sX.SplitterDistance = Configuration.Window.window_size;
                ChannelList = new PidgeonList();
                toolStripProgressBar1.Visible = false;
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
                if (Configuration.Kernel.Debugging)
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

                foreach (string text in Core.Parameters)
                {
                    Core.ParseLink(text);
                }
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
                toolStripStatusNetwork.Text = Core.network.ServerName + "    w/c/p " + Core.network._Protocol.Windows.Count.ToString() + "/" + Core.network.Channels.Count.ToString() + "/" + Core.network.PrivateChat.Count.ToString();
                if (Core.network.RenderedChannel != null)
                {
                    string info = "";
                    if (Core.network.RenderedChannel.Bans != null)
                    {
                        info += Core.network.RenderedChannel.Bans.Count.ToString();
                    }
                    else
                    {
                        info += "??";
                    }
                    info = info + " / ";

                    if (Core.network.RenderedChannel.Invites != null)
                    {
                        info += Core.network.RenderedChannel.Invites.Count.ToString();
                    }
                    else
                    {
                        info += "??";
                    }
                    info = info + " / ";
                    if (Core.network.RenderedChannel.Exceptions != null)
                    {
                        info += Core.network.RenderedChannel.Exceptions.Count.ToString();
                    }
                    else
                    {
                        info += "??";
                    }
                    setText(Core.network.RenderedChannel.Name + " - " + Core.network.RenderedChannel.Topic);
                    toolStripStatusChannel.Text = Core.network.RenderedChannel.Name + " u: " + Core.network.RenderedChannel.UserList.Count + " m: " + Core.network.RenderedChannel.ChannelMode.ToString() + " b/I/e: " + info;
                    if (Configuration.Kernel.DisplaySizeOfBuffer)
                    {
                        if (Chat != null)
                        {
                            toolStripStatusChannel.Text += " messages: " + Chat.scrollback.Lines.ToString();
                        }
                    }
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
            try
            {
                Core.Quit();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                _Load();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (fPrefs == null || fPrefs.IsDisposed)
                {
                    fPrefs = new Preferences();
                }
                fPrefs.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://pidgeonclient.org/wiki/Help:Contents");
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void Unshow(object main, FormClosingEventArgs closing)
        {
            try
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
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Help _Help = new Help();
                _Help.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void newConnectionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (fConnection == null || fConnection.IsDisposed)
                    fConnection = new Connection();


                fConnection.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void updater_Tick(object sender, EventArgs e)
        {
            try
            {
                // Someone updated status, maybe from outer thread, let's change it
                if (UpdatedStatus)
                {
                    UpdatedStatus = false;
                    UpdateStatus();
                }

                // Visible is not a variable but property so don't change it even to same value for performance reasons
                if (toolStripProgressBar1.Visible != DisplayingProgress)
                {
                    toolStripProgressBar1.Visible = DisplayingProgress;
                }

                if (DisplayingProgress)
                {
                    if (updater.Interval != 10)
                    {
                        updater.Interval = 10;
                    }
                }
                else
                {
                    if (updater.Interval != 200)
                    {
                        updater.Interval = 200;
                    }
                }

                if (toolStripProgressBar1.Maximum != ProgressMax)
                {
                    if (toolStripProgressBar1.Value > ProgressMax)
                    {
                        toolStripProgressBar1.Value = ProgressMax;
                    }
                    toolStripProgressBar1.Maximum = ProgressMax;
                }
                if (toolStripProgressBar1.Value != progress)
                {
                    toolStripProgressBar1.Value = progress;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                Core.trafficscanner.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void taskbarBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MicroChat.mc.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void attachToMicroChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Chat != null)
                {
                    Chat.MicroBox = true;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void detachFromMicroChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Chat != null)
                {
                    Chat.MicroBox = false;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void rootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Core.network != null)
                {
                    Core.network.RenderedChannel = null;
                    main.Visible = true;
                    Core.network._Protocol.Current.Visible = false;
                    Core.network._Protocol.Current = main;
                    return;
                }
                main.Visible = true;
                main.BringToFront();
                main.scrollback.Display();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        protected void Wheeled(object sender, MouseEventArgs md)
        {
            try
            {
                if (Chat != null)
                {
                    Chat.scrollback.RT.Wheeled(sender, md);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchbox == null)
                {
                    searchbox = new SearchItem();
                }
                if (searchbox.IsDisposed)
                {
                    searchbox = new SearchItem();
                }
                if (searchbox.Visible)
                {
                    searchbox.Hide();
                    return;
                }
                searchbox.Show();
                searchbox.TopMost = true;
                searchbox.textBox1.Focus();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void switchToAdvancedLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Chat != null)
                {
                    Chat.scrollback.Switch(true);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void configurationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SettingsEd ed = new SettingsEd();
                ed.Show(this);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
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
