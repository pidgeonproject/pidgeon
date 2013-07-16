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
        public string WindowName = null;
        /// <summary>
        /// Whether it's a channel or not
        /// </summary>
        public bool isChannel = false;
        /// <summary>
        /// Lock the window for any changes
        /// </summary>
        public bool Locked = false;
        /// <summary>
        /// Color in menu
        /// </summary>
        public System.Drawing.Color MenuColor;
        /// <summary>
        /// Protocol associated with this window
        /// </summary>
        public Protocol _Protocol = null;
        /// <summary>
        /// In case this is true, we are in micro chat
        /// </summary>
        public bool MicroBox = false;
        /// <summary>
        /// Private message
        /// </summary>
        public bool isPM = false;
        /// <summary>
        /// Network that is associated with this window
        /// </summary>
        public Network _Network = null;
        /// <summary>
        /// Panels will ignore the changes
        /// </summary>
        public bool ignoreChange = false;
        /// <summary>
        /// If this is true the side list will assign an icon to this item
        /// </summary>
        public bool needIcon = false;
        private Channel channel = null;
        /// <summary>
        /// Whether this window is loaded
        /// </summary>
        public bool isInitialised = false;
        /// <summary>
        /// Turning this to false will disable notifications for this window
        /// </summary>
        public bool Highlights = true;
        /// <summary>
        /// Sound notifications are enabled in here
        /// </summary>
        public bool Sounds = true;
        
        // window
        private global::Gtk.VPaned vpaned1;
        private global::Gtk.HPaned hpaned1;
        private global::Client.Scrollback scrollback1;
        private global::Gtk.ScrolledWindow GtkScrolledWindow1;
        private global::Gtk.TreeView listView;
        private global::Client.Graphics.TextBox textbox1;
        private bool destroyed = false;
        private bool hasUserList = true;
        /// <summary>
        /// Whether control has a user list
        /// </summary>
        public bool HasUserList
        {
            get
            {
                return hasUserList;
            }
            set
            {
                if (hasUserList == value)
                {
                    return;
                }
                if (System.Threading.Thread.CurrentThread != Core._KernelThread)
                {
                    throw new Exception("You can't control other windows from non kernel thread");
                }
                hasUserList = value;
                if (GtkScrolledWindow1 != null)
                {
                    this.GtkScrolledWindow1.Visible = hasUserList;
                }
            }
        }

        private bool hasTextBox = true;
        /// <summary>
        /// Whether control has a text
        /// </summary>
        public bool HasTextBox
        {
            set
            {
                if (hasTextBox == value)
                {
                    // nothing to do
                    return;
                }
                if (System.Threading.Thread.CurrentThread != Core._KernelThread)
                {
                    throw new Exception("You can't control other windows from non kernel thread");
                }
                if (textbox != null)
                {
                    textbox.Visible = hasTextBox;
                }
            }
            get
            {
                return hasTextBox;
            }
        }

        /// <summary>
        /// This will return true in case object was requested to be disposed
        /// you should never work with objects that return true here
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return destroyed;
            }
        }

        /// <summary>
        /// Specifies if it's possible to write into this window
        /// </summary>
        public bool IsWritable
        {
            get
            {
                if (!IsDestroyed)
                {
                    return (isChannel || isPM);
                }
                return false;
            }
        }

        /// <summary>
        /// Pointer
        /// </summary>
        public Scrollback scrollback
        {
            get
            {
                return scrollback1;
            }
        }

        /// <summary>
        /// Pointer
        /// </summary>
        public Graphics.TextBox textbox
        {
            get
            {
                return textbox1;
            }
        }

        /// <summary>
        /// Creates a new window
        /// </summary>
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

        /// <summary>
        /// Destruct
        /// </summary>
        ~Window()
        {
            if (!IsDestroyed)
            {
                // we need to remove some containers that could hold the references and create a leak
                _Destroy();
            }
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor called for window: " + WindowName);
                //Core.DebugLog("Released: " + Core.GetSizeOfObject(this).ToString() + " bytes of memory");
            }
        }

        private void Build()
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

            // this is a nasty bug in version of gtk distributed 
            if (Configuration.CurrentPlatform != Core.Platform.Linuxx64 && Configuration.CurrentPlatform != Core.Platform.Linuxx86)
            {
                this.vpaned1.AddNotification("position", new GLib.NotifyHandler(Changed));
                this.hpaned1.AddNotification("position", new GLib.NotifyHandler(Changed));
            }

            this.Add(this.vpaned1);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }

            if (!HasUserList)
            {
                GtkScrolledWindow1.Visible = false;
            }

            if (!hasTextBox)
            {
                textbox.Visible = false;
            }
        }

        private void InitStyle()
        {
            listView.ModifyBase(StateType.Normal, Core.FromColor(Configuration.CurrentSkin.backgroundcolor));
            listView.ModifyText(StateType.Normal, Core.FromColor(Configuration.CurrentSkin.colordefault));
        }
        
        [GLib.ConnectBefore]
        private void Ignore(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                e.RetVal = true;
            }
        }

        private bool Update()
        {
            if (IsDestroyed)
            {
                return false;
            }
            Changed(null, null);
            return true;
        }

        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            if (Configuration.CurrentPlatform == Core.Platform.Linuxx86 || Configuration.CurrentPlatform == Core.Platform.Linuxx64)
            {
                GLib.TimeoutHandler timer = new GLib.TimeoutHandler(Update);
                GLib.Timeout.Add(800, timer);
            }
            this.scrollback.owner = this;
            this.scrollback.Create();
            this.textbox.Init();
            this.Build();
            this.InitStyle();
            kbToolStripMenuItem.Enabled = true;
            krToolStripMenuItem.Enabled = true;
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

        /// <summary>
        /// Create
        /// </summary>
        private void Create()
        {
            scrollback.channelToolStripMenuItem.Visible = isChannel;
            scrollback.retrieveTopicToolStripMenuItem.Visible = isChannel;
            if (!isChannel)
            {
                banToolStripMenuItem.Visible = false;
                whoisToolStripMenuItem.Visible = false;
                kbToolStripMenuItem.Visible = false;
                whoisToolStripMenuItem.Visible = false;
                dccToolStripMenu.Visible = false;
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
                kbToolStripMenuItem.Enabled = true;
                whoisToolStripMenuItem.Enabled = true;
                ctToolStripMenuItem.Enabled = true;
                refreshToolStripMenuItem.Enabled = true;
                kickBanToolStripMenuItem.Enabled = true;
                modeToolStripMenuItem.Enabled = true;
                kickToolStripMenuItem.Enabled = true;
                krToolStripMenuItem.Enabled = true;
                banToolStripMenuItem.Visible = true;
                whoisToolStripMenuItem.Visible = true;
                kbToolStripMenuItem.Visible = true;
                whoisToolStripMenuItem.Visible = true;
                dccToolStripMenu.Visible = true;
                ctToolStripMenuItem.Visible = true;
                messageToolStripMenuItem.Visible = true;
                refreshToolStripMenuItem.Visible = true;
                kickBanToolStripMenuItem.Visible = true;
                modeToolStripMenuItem.Visible = true;
                kickToolStripMenuItem.Visible = true;
                krToolStripMenuItem.Visible = true;
                synchroToolStripMenuItem.Visible = true;
            }
            Redraw();
            isInitialised = true;
        }

        /// <summary>
        /// Redraw window
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Destroy this instance and release all memory
        /// </summary>
        public void _Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }

            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destroying " + WindowName);
            }

            destroyed = true;

            if (scrollback1 != null)
            {
                scrollback._Destroy();
            }

            if (textbox1 != null)
            {
                textbox._Destroy();
            }

            UserList.Clear();

            channel = null;
            _Network = null;

            this.scrollback1 = null;
            this.textbox1 = null;
            this.Destroy();
        }
        
        private void Changed(object sender, GLib.NotifyArgs dt)
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
  
        /// <summary>
        /// Creates the chat
        /// </summary>
        /// <param name='WindowOwner'>
        /// Window owner.
        /// </param>
        /// <param name='Focus'>
        /// Focus.
        /// </param>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public void CreateChat(Protocol WindowOwner, bool Focus = true)
        {
            if (System.Threading.Thread.CurrentThread != Core._KernelThread)
            {
                throw new Exception("You can't control other windows from non kernel thread");
            }
            this.Init();
            this.Create();
            this.Visible = true;
            this._Protocol = WindowOwner;
            if (Core.SystemForm.Chat != null && Core.SystemForm.Chat.textbox != null)
            {
                if (this.textbox.history.Count == 0)
                {
                    this.textbox.history.AddRange(Core.SystemForm.Chat.textbox.history);
                }
            }
            if (Focus)
            {
                Core.SystemForm.SwitchWindow(this);
            }
        }

        /// <summary>
        /// Creates the chat
        /// </summary>
        /// <param name="WindowOwner">Window owner</param>
        /// <param name="_HasUserList"></param>
        /// <param name="_HasTextBox"></param>
        /// <param name="Focus"></param>
        public void CreateChat(Protocol WindowOwner, bool _HasUserList, bool _HasTextBox, bool Focus = true)
        {
            if (System.Threading.Thread.CurrentThread != Core._KernelThread)
            {
                throw new Exception("You can't control other windows from non kernel thread");
            }
            hasTextBox = _HasTextBox;
            hasUserList = _HasUserList;
            CreateChat(WindowOwner, Focus);
        }
        
        private bool Mode(string mode)
        {
            try
            {
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        Core.SelectedNetwork.Transfer("MODE " + WindowName + " " + mode + " " + user.Nick);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
        }

        /// <summary>
        /// Return a channel associated with this window
        /// </summary>
        /// <returns></returns>
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
                    Channel chan = _Network.getChannel(WindowName);
                    if (chan != null)
                    {
                        channel = chan;
                        return channel;
                    }
                }
            }
            return null;
        }
    }
}

