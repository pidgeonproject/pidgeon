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
        public System.Drawing.Color MenuColor;
        /// <summary>
        /// Deprecated, use _Network._Protocol instead
        /// </summary>
        public Protocol _Protocol = null;
        /// <summary>
        /// In case this is true, we are in micro chat
        /// </summary>
        public bool MicroBox = false;
        public bool isPM = false;
        /// <summary>
        /// Network that is associated with this window
        /// </summary>
        public Network _Network = null;
        public bool ignoreChange = false;
        private Channel channel = null;
        public bool isInitialised = false;
        
        // window
        private global::Gtk.VPaned vpaned1;
        private global::Gtk.HPaned hpaned1;
        private global::Client.Scrollback scrollback1;
        private global::Gtk.ScrolledWindow GtkScrolledWindow1;
        private global::Gtk.TreeView listView;
        private global::Client.Graphics.TextBox textbox1;
        private bool destroyed = false;

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

        ~Window()
        {
            if (!IsDestroyed)
            {
                // we need to remove some containers that could hold the references and create a leak
                _Destroy();
            }
            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destructor called for window: " + name);
                //Core.DebugLog("Released: " + Core.GetSizeOfObject(this).ToString() + " bytes of memory");
            }
        }

        protected virtual void Build()
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
            this.vpaned1.AddNotification("position", new GLib.NotifyHandler(Changed));
            this.hpaned1.AddNotification("position", new GLib.NotifyHandler(Changed));

            this.Add(this.vpaned1);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Hide();
        }

        public void InitStyle()
        {
            listView.ModifyBase(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.backgroundcolor));
            listView.ModifyText(StateType.Normal, Core.fromColor(Configuration.CurrentSkin.colordefault));
        }
        
        [GLib.ConnectBefore]
        private void Ignore(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                e.RetVal = true;
            }
        }
        
        public void Init()
        {
            this.scrollback.owner = this;
            this.scrollback.Create();
            this.textbox.Init();
            this.Build();
            this.InitStyle();
            kbToolStripMenuItem.Enabled = false;
            krToolStripMenuItem.Enabled = false;
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

        public void Create()
        {
            scrollback.channelToolStripMenuItem.Visible = isChannel;
            scrollback.retrieveTopicToolStripMenuItem.Visible = isChannel;
            if (!isChannel)
            {
                banToolStripMenuItem.Visible = false;
                whoisToolStripMenuItem.Visible = false;
                kbToolStripMenuItem.Visible = false;
                whoisToolStripMenuItem.Visible = false;
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
                kbToolStripMenuItem.Enabled = false;
                whoisToolStripMenuItem.Enabled = true;
                ctToolStripMenuItem.Enabled = true;
                refreshToolStripMenuItem.Enabled = true;
                kickBanToolStripMenuItem.Enabled = true;
                modeToolStripMenuItem.Enabled = true;
                kickToolStripMenuItem.Enabled = true;
                krToolStripMenuItem.Enabled = false;
                banToolStripMenuItem.Visible = true;
                whoisToolStripMenuItem.Visible = true;
                kbToolStripMenuItem.Visible = true;
                whoisToolStripMenuItem.Visible = true;
                ctToolStripMenuItem.Visible = true;
                messageToolStripMenuItem.Visible = true;
                refreshToolStripMenuItem.Visible = true;
                kickBanToolStripMenuItem.Visible = true;
                modeToolStripMenuItem.Visible = true;
                kickToolStripMenuItem.Visible = true;
                krToolStripMenuItem.Visible = true;
                synchroToolStripMenuItem.Visible = true;
            }
            if (scrollback.owner == null || scrollback.owner._Network == null)
            {
                scrollback.listAllChannelsToolStripMenuItem.Visible = false;
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
                    vpaned1.Position = Configuration.Window.x4;
                }
            }
            ignoreChange = false;
            return true;
        }

        public void _Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }

            if (Configuration.Kernel.Debugging)
            {
                Core.DebugLog("Destroying " + name);
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
        
        public void Changed(object sender, GLib.NotifyArgs dt)
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

        bool Mode(string mode)
        {
            try
            {
                if (isChannel)
                {
                    foreach (User user in SelectedUsers)
                    {
                        Core.network.Transfer("MODE " + name + " " + mode + " " + user.Nick);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
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
    }
}

