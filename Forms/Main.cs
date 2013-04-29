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

namespace Client.Forms
{
    /// <summary>
    /// Main
    /// </summary>
    public partial class Main : Client.GTK.PidgeonForm
    {
        /// <summary>
        /// Micro Chat
        /// </summary>
        public MicroChat micro = null;
        private string StatusBox = "";
        /// <summary>
        /// Primary window
        /// </summary>
        public Graphics.Window main = null;
        /// <summary>
        /// Side bar
        /// </summary>
        public Graphics.PidgeonList ChannelList = null;
        /// <summary>
        /// Currently displayed window
        /// </summary>
        public Graphics.Window Chat = null;
        private Connection fConnection;
        /// <summary>
        /// Preferences form
        /// </summary>
        public Preferences fPrefs;
        private bool UpdatedStatus = true;
        private SearchItem searchbox = new SearchItem();
        private bool done = false;
        /// <summary>
        /// Progress
        /// </summary>
        public double progress = 0;
        /// <summary>
        /// Displaying progress
        /// </summary>
        public bool DisplayingProgress = false;
        /// <summary>
        /// Maximum value of progress
        /// </summary>
        public double ProgressMax = 0;
        public List<_WindowRequest> WindowRequests = new List<_WindowRequest>();
        private GLib.TimeoutHandler timer = null;
        /// <summary>
        /// Tray icon
        /// </summary>
        public Gtk.StatusIcon icon = null;
        /// <summary>
        /// pointer
        /// </summary>
        public Gtk.HPaned hPaned
        {
            get
            {
                return hpaned1;
            }
        }

        /// <summary>
        /// Window request
        /// </summary>
        public class _WindowRequest
        {
            /// <summary>
            /// Window handle
            /// </summary>
            public Graphics.Window window = null;
            /// <summary>
            /// Name
            /// </summary>
            public string name = null;
            /// <summary>
            /// Focus
            /// </summary>
            public bool focus = false;
            /// <summary>
            /// Protocol that owns this request
            /// </summary>
            public Protocol owner = null;
        }

        /// <summary>
        /// Creates a main form
        /// </summary>
        public Main()
        {
            try
            {
                Core._Main = this;
                if (Configuration.UserData.TrayIcon)
                {
                    icon = new StatusIcon(global::Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico"));
                    icon.Visible = true;
                    icon.PopupMenu += new PopupMenuHandler(TrayMenu);
                }
                this.Build();
                timer = new GLib.TimeoutHandler(updater_Tick);
                GLib.Timeout.Add(200, timer);
                this.DetachFromMicroChatAction.Activated += new EventHandler(detachFromMicroChatToolStripMenuItem_Click);
                this.AttachToMicroChatAction.Activated += new EventHandler(attachToMicroChatToolStripMenuItem_Click);
                this.SearchAction.Activated += new EventHandler(searchToolStripMenuItem_Click);
                this.ContentsAction.Activated += new EventHandler(contentsToolStripMenuItem_Click);
                this.ConfigurationFileAction.Activated += new EventHandler(configurationFileToolStripMenuItem_Click);
                this.FavoriteNetworksAction.Activated += new EventHandler(favoriteNetworksToolStripMenuItem_Click);
                this.RootAction.Activated += new EventHandler(rootToolStripMenuItem_Click);
                this.LoadMoreToScrollbackAction.Activated += new EventHandler(loadToolStripMenuItem_Click);
                this.FavoriteNetworksAction.Sensitive = false;
                _Load();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void TrayMenu(object o, EventArgs args)
        {
            Menu menu = new Menu();
            ImageMenuItem menuItemQuit = new ImageMenuItem("Quit");
            Gtk.Image appimg = new Gtk.Image(Stock.Quit, IconSize.Menu);
            CheckMenuItem notifications = new CheckMenuItem("Enable notifications");
            menu.Add(notifications);
            notifications.Active = Configuration.Kernel.Notice;
            notifications.Activated += new EventHandler(itemNotification);
            if (!this.Visible)
            {
                MenuItem show = new MenuItem("Show");
                menu.Add(show);
                show.Activated += new EventHandler(itemShow);
            }
            else
            {
                MenuItem hide = new MenuItem("Hide");
                hide.Activated += new EventHandler(itemHide);
                menu.Add(hide);
            }
            menu.Add(new Gtk.SeparatorMenuItem());
            menuItemQuit.Image = appimg;
            menu.Add(menuItemQuit);
            // Quit the application when quit has been clicked.
            menuItemQuit.Activated += new EventHandler(shutDownToolStripMenuItem_Click);
            menu.ShowAll();
            menu.Popup();
        }

        private void itemShow(object o, EventArgs e)
        {
            this.Visible = true;
        }

        private void itemHide(object o, EventArgs e)
        {
            this.Visible = false;
        }

        private void itemNotification(object o, EventArgs e)
        {
            Configuration.Kernel.Notice = !Configuration.Kernel.Notice;
        }

        private void _Load()
        {
            try
            {
                messages.Localize(this);
                SkinEditorAction.Sensitive = false;
                setText("");
                if (Configuration.Window.Window_Maximized)
                {
                    this.Maximize();
                }
                if (Configuration.Window.x4 == 0)
                {
                    Configuration.Window.window_size = 200;
                    Configuration.Window.x1 = Width - 480;
                    Configuration.Window.x4 = Height - 120;
                }
                hpaned1.Position = Configuration.Window.window_size;
                ChannelList = pidgeonlist1;
                toolStripProgressBar1.Visible = false;
                micro = new MicroChat();
                ChannelList.Visible = true;
                main = new Client.Graphics.Window();
                main.Events = ((global::Gdk.EventMask)(256));
                UserAction.Visible = false;
                CreateChat(main, null);
                main.WindowName = "Pidgeon";
                toolStripStatusNetwork.TooltipText = "windows / channels / pm";
                Chat = main;
                main.Redraw();
                Chat.Making = false;
                if (Configuration.Kernel.Debugging)
                {
                    Core.PrintRing(Chat, false);
                }

                Chat.scrollback.InsertText("Welcome to pidgeon client " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, Client.ContentLine.MessageStyle.System, false, 0, true);

                if (Core.Extensions.Count > 0)
                {
                    foreach (Extension nn in Core.Extensions)
                    {
                        Chat.scrollback.InsertText("Extension " + nn.Name + " (" + nn.Version + ")", Client.ContentLine.MessageStyle.System, false, 0, true);
                    }
                }
                done = true;

                foreach (string text in Core.Parameters)
                {
                    Core.ParseLink(text);
                }
                Hooks._Sys.Initialise(this);
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        private void Changed(object sender, EventArgs dt)
        {
            if (done)
            {
                Configuration.Window.window_size = hpaned1.Position;
            }
        }

        /// <summary>
        /// Create a new chat
        /// </summary>
        /// <param name="Chat"></param>
        /// <param name="WindowOwner"></param>
        /// <param name="Focus"></param>
        public void CreateChat(Graphics.Window Chat, Protocol WindowOwner, bool Focus = true)
        {
            if (System.Threading.Thread.CurrentThread != Core._KernelThread)
            {
                throw new Exception("You can't control other windows from non kernel thread");
            }
            Chat.Init();
            Chat.Create();
            Chat.Visible = true;
            Chat._Protocol = WindowOwner;
            if (Core._Main.Chat != null && Core._Main.Chat.textbox != null)
            {
                if (Chat.textbox.history.Count == 0)
                {
                    Chat.textbox.history.AddRange(Core._Main.Chat.textbox.history);
                }
            }
            if (Focus)
            {
                SwitchWindow(Chat);
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

        /// <summary>
        /// Shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool ShortcutHandle(object sender, KeyPressEventArgs e)
        {
            bool rt = false;
            try
            {
                foreach (Core.Shortcut shortcut in Configuration.ShortcutKeylist)
                {
                    if (shortcut.control == (e.Event.State == Gdk.ModifierType.ControlMask)
                        && shortcut.keys == Core.convertKey(e.Event.Key)) //&& shortcut.alt == )
                    {
                        Parser.parse(shortcut.data);
                        rt = true;
                    }
                }

                e.RetVal = rt;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return rt;
        }

        /// <summary>
        /// Update the status bar
        /// </summary>
        public void UpdateStatus()
        {
            if (System.Threading.Thread.CurrentThread != Core._KernelThread)
            {
                throw new Exception("You can't control other windows from non kernel thread");
            }
            this.toolStripInfo.Text = StatusBox;
            if (Core.network != null && !Core.network.IsDestroyed)
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
                    toolStripStatusChannel.Text = Core.network.RenderedChannel.Name + " user count: " + Core.network.RenderedChannel.UserList.Count + " channel modes: " + Core.network.RenderedChannel.ChannelMode.ToString() + " b/I/e: " + info;
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

        /// <summary>
        /// Change focus to this window
        /// </summary>
        public void setFocus()
        {
            GrabFocus();
        }

        /// <summary>
        /// Change title
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int setText(string name)
        {
            if (Core._KernelThread != System.Threading.Thread.CurrentThread)
            {
                throw new Exception("This function can be called only from kernel thread");
            }
            this.Title = "Pidgeon Client v 1.2 " + name;
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


        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (fPrefs == null)
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
                Hyperlink.OpenLink("http://pidgeonclient.org/wiki/Help:Contents");
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Unshow(object main, Gtk.DeleteEventArgs closing)
        {
            try
            {
                if (Core.IgnoreErrors)
                {
                    Core.DebugLog("Closing main");
                    return;
                }
                MessageDialog message = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, messages.get("pidgeon-shut", Core.SelectedLanguage));
                message.WindowPosition = WindowPosition.Center;
                message.Title = "Shut down?";
                ResponseType result = (ResponseType)message.Run();
                if (result != ResponseType.Yes)
                {
                    message.Destroy();
                    closing.RetVal = true;
                    return;
                }
                Core.Quit();
                return;
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
                Forms.Help _Help = new Help();
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
                if (fConnection == null)
                {
                    fConnection = new Forms.Connection();
                }

                fConnection.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private bool updater_Tick()
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

                if (hpaned1.Position != Configuration.Window.window_size)
                {
                    if (done)
                    {
                        Configuration.Window.window_size = hpaned1.Position;
                    }
                }

                if (this.toolStripProgressBar1.Adjustment.Upper != ProgressMax)
                {
                    if (toolStripProgressBar1.Adjustment.Value > ProgressMax)
                    {
                        toolStripProgressBar1.Adjustment.Value = ProgressMax;
                    }
                    toolStripProgressBar1.Adjustment.Upper = ProgressMax;
                }
                if (toolStripProgressBar1.Adjustment.Value != progress)
                {
                    toolStripProgressBar1.Adjustment.Value = progress;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
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
                micro.Show();
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

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Chat != null)
                {
                    Chat.scrollback.IncreaseLimits();
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

        /// <summary>
        /// Switches window (thread unsafe)
        /// </summary>
        /// <param name="window"></param>
        public void SwitchWindow(Graphics.Window window)
        {
            if (System.Threading.Thread.CurrentThread != Core._KernelThread)
            {
                throw new Exception("You can't control other windows from non kernel thread");
            }
            if (hpaned1.Child2 != null)
            {
                hpaned1.Remove(hpaned1.Child2);
            }
            hpaned1.Add2(window);
            Chat = window;
        }

        /// <summary>
        /// Change a name of currently displayed channel (thread unsafe)
        /// </summary>
        /// <param name="channel"></param>
        public void setChannel(string channel)
        {
            if (Core._KernelThread != System.Threading.Thread.CurrentThread)
            {
                throw new Exception("This function can be called only from kernel thread");
            }
            toolStripStatusChannel.Text = channel;
        }

        /// <summary>
        /// Display a root
        /// </summary>
        public void SwitchRoot()
        {
            if (Core.network != null)
            {
                Core.network.RenderedChannel = null;
                Core.network._Protocol.Current = main;
                SwitchWindow(main);
                return;
            }
            main.Visible = true;
            SwitchWindow(main);
        }

        /// <summary>
        /// This function is called when you 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rootToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SwitchRoot();
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

                if (searchbox.Visible)
                {
                    searchbox.Hide();
                    return;
                }

                searchbox.Show();
                searchbox.GrabFocus();
                searchbox.setFocus();
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
                Forms.ConfigFile ed = new Forms.ConfigFile();
                ed.Load();
                ed.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void favoriteNetworksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Forms.NetworkDB form = new Forms.NetworkDB();
                form.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}

