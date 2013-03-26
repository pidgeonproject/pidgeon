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
	public partial class Main : Gtk.Window
	{
		private string StatusBox = "";
        public Graphics.Window main = null;
        public Graphics.PidgeonList ChannelList = null;
        public Graphics.Window Chat = null;
        private Connection fConnection;
        private Preferences fPrefs;
        private bool UpdatedStatus = true;
        SearchItem searchbox = new SearchItem();
        bool done = false;
        public int progress = 0;
        public bool DisplayingProgress = false;
        public int ProgressMax = 0;
		public List<_WindowRequest> WindowRequests = new List<_WindowRequest>();
        public global::Gtk.HPaned hPaned
        {
            get
            {
                return hpaned1;
            }
        }

		public int Height
		{
			get
			{
				int height;
				int width;
				GetSize(out width, out height);
				return height;
			}
			set
			{
				this.SetSizeRequest (Width, value);
			}
		}
		
		public int Width
		{
			get
			{
				int height;
				int width;
				GetSize(out width, out height);
				return width;
			}
			set
			{
				this.SetSizeRequest (value, Height);
			}
		}	

        public class _WindowRequest
        {
            public Graphics.Window window;
            public string name;
            public bool focus;
            public bool writable;
            public Protocol owner;
        }

		
		public Main () : base(Gtk.WindowType.Toplevel)
		{
			try
			{
				Core._Main = this;
				this.Build ();
				_Load();
			}
			catch (Exception fail)
			{
				Core.handleException(fail);
			}
		}
		
		public void _Load()
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
                    Configuration.Window.window_size = 80;
                    Configuration.Window.x1 = Height - 80;
                    Configuration.Window.x4 = 600;
                    if (Width > 200)
                    {
                        Configuration.Window.x4 = this.Width - 200;
                    }
                }
                hpaned1.Position = Configuration.Window.window_size;
				ChannelList = pidgeonlist1;
                toolStripProgressBar1.Visible = false;
                ChannelList.Visible = true;
                main = new Client.Graphics.Window();
				main.Events = ((global::Gdk.EventMask)(256));
				UserAction.Visible = false;
                CreateChat(main, null);
                main.name = "Pidgeon";
				toolStrip.TooltipText = "windows / channels / pm";
                Chat = main;
                main.Redraw();
                Chat.Making = false;
                if (Configuration.Kernel.Debugging)
                {
                    Core.PrintRing(Chat, false);
                }

                Chat.scrollback.InsertText("Welcome to pidgeon client " +  System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, Scrollback.MessageStyle.System, false, 0, true);

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
                Hooks._Sys.Initialise(this);
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }
		
		public void Changed(object sender, EventArgs dt)
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
            Chat.Init();
            Chat.Create();
            Chat.Visible = Focus;
            Chat._Protocol = WindowOwner;
            if (Core._Main.Chat != null && Core._Main.Chat.textbox != null)
            {
                Chat.textbox.history.AddRange(Core._Main.Chat.textbox.history);
            }
			this.hpaned1.Add2 (Chat);
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

        public static bool ShortcutHandle(object sender, KeyPressEventArgs e)
        {
            bool rt = false;
            try
            {
                foreach (Core.Shortcut shortcut in Configuration.ShortcutKeylist)
                {
                    if (shortcut.control == (e.Event.State == Gdk.ModifierType.ControlMask) 
                        && shortcut.keys == e.Event.Key) //&& shortcut.alt == )
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
		
		public void setFocus()
		{
			GrabFocus();
		}
		
        public int setText(string name)
        {
            this.Title = "Pidgeon Client v 1.0 " + name;
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
                System.Diagnostics.Process.Start("http://pidgeonclient.org/wiki/Help:Contents");
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
		
        public void Unshow(object main, Gtk.DeleteEventArgs closing)
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
				ResponseType result = (ResponseType)message.Run ();	
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

        public void Test(object sender, EventArgs e)
        { 
            
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
                    fConnection = new Forms.Connection();


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

                if (DisplayingProgress)
                {
                    //if (updater.Interval != 10)
                    {
                    //    updater.Interval = 10;
                    }
                }
                else
                {
                    //if (updater.Interval != 200)
                    {
                    //    updater.Interval = 200;
                    }
                }

                //if (toolStripProgressBar1.Maximum != ProgressMax)
                {
                 //   if (toolStripProgressBar1.Value > ProgressMax)
                    {
                 //       toolStripProgressBar1.Value = ProgressMax;
                    }
                 //   toolStripProgressBar1.Maximum = ProgressMax;
                }
                //if (toolStripProgressBar1.Value != progress)
                {
                //    toolStripProgressBar1.Value = progress;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
			return false;
        }
		
		/*
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
		*/
	}
}

