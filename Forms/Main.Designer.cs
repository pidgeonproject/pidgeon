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

namespace Client.Forms
{
    public partial class Main
    {
        /// <summary>
        /// File menu
        /// </summary>
        public Gtk.Menu FileMenu = new Gtk.Menu();
        /// <summary>
        /// The file action.
        /// </summary>
        public Gtk.MenuItem FileAction;
        /// <summary>
        /// The shut down action.
        /// </summary>
        public Gtk.ImageMenuItem ShutDownAction;
        /// <summary>
        /// Tools menu
        /// </summary>
        public Gtk.Menu ToolsMenu = new Gtk.Menu();
        /// <summary>
        /// The tools action.
        /// </summary>
        public Gtk.MenuItem ToolsAction;
        /// <summary>
        /// Misc
        /// </summary>
        public Gtk.Menu MiscMenu = new Gtk.Menu();
        /// <summary>
        /// The misc action.
        /// </summary>
        public Gtk.MenuItem MiscAction;
        /// <summary>
        /// Show menu
        /// </summary>
        public Gtk.Menu ShowMenu = new Gtk.Menu();
        /// <summary>
        /// The show action.
        /// </summary>
        public Gtk.MenuItem ShowAction;
        /// <summary>
        /// The help action.
        /// </summary>
        public Gtk.ImageMenuItem HelpAction;
        /// <summary>
        /// The about action.
        /// </summary>
        public Gtk.ImageMenuItem AboutAction;
        /// <summary>
        /// The contents action.
        /// </summary>
        public Gtk.MenuItem ContentsAction;
        /// <summary>
        /// Help menu
        /// </summary>
        public Gtk.Menu HelpMenu = new Gtk.Menu();
        /// <summary>
        /// The root action.
        /// </summary>
        public Gtk.MenuItem RootAction;
        /// <summary>
        /// The search action.
        /// </summary>
        public Gtk.MenuItem SearchAction;
        /// <summary>
        /// The configuration file action.
        /// </summary>
        public Gtk.MenuItem ConfigurationFileAction;
        /// <summary>
        /// The open new connection action.
        /// </summary>
        public Gtk.ImageMenuItem OpenNewConnectionAction;
        /// <summary>
        /// The favorite networks action.
        /// </summary>
        public Gtk.MenuItem FavoriteNetworksAction;
        /// <summary>
        /// The preferences action.
        /// </summary>
        public Gtk.ImageMenuItem PreferencesAction;
        /// <summary>
        /// The packet viewer action.
        /// </summary>
        public Gtk.MenuItem PacketViewerAction;
        /// <summary>
        /// The skin editor action.
        /// </summary>
        public Gtk.MenuItem SkinEditorAction;
        /// <summary>
        /// The small chat action.
        /// </summary>
        public Gtk.MenuItem SmallChatAction;
        /// <summary>
        /// The attach to micro chat action.
        /// </summary>
        public Gtk.ImageMenuItem AttachToMicroChatAction;
        /// <summary>
        /// The detach from micro chat action.
        /// </summary>
        public Gtk.ImageMenuItem DetachFromMicroChatAction;
        /// <summary>
        /// The load more to scrollback action.
        /// </summary>
        public Gtk.ImageMenuItem LoadMoreToScrollbackAction;
        /// <summary>
        /// The vbox3.
        /// </summary>
        public Gtk.VBox vbox3;
        /// <summary>
        /// The menubar2.
        /// </summary>
        public Gtk.MenuBar menubar2;
        /// <summary>
        /// The hpaned1.
        /// </summary>
        public Gtk.HPaned hpaned1;
        /// <summary>
        /// The pidgeonlist1.
        /// </summary>
        public Client.Graphics.PidgeonList pidgeonlist1;
        /// <summary>
        /// The tool strip.
        /// </summary>
        public Gtk.Statusbar toolStrip;
        /// <summary>
        /// The tool strip status network.
        /// </summary>
        public Gtk.Label toolStripStatusNetwork;
        /// <summary>
        /// The tool strip status channel.
        /// </summary>
        public Gtk.Label toolStripStatusChannel;
        /// <summary>
        /// The tool strip info.
        /// </summary>
        public Gtk.Label toolStripInfo;
        /// <summary>
        /// The tool strip progress bar1.
        /// </summary>
        public Gtk.ProgressBar toolStripProgressBar1;

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            this.menubar2 = new Gtk.MenuBar();
            Gtk.AccelGroup agrp = new Gtk.AccelGroup ( );
            this.AddAccelGroup (agrp);
            ////////////////////// FILE //////////////////////
            FileAction = new Gtk.MenuItem(messages.Localize("[[window-menu-file]]"));
            FileAction.Submenu = this.FileMenu;
            OpenNewConnectionAction = new Gtk.ImageMenuItem(messages.Localize("[[window-menu-open]]"));
            OpenNewConnectionAction.AddAccelerator("activate", agrp, new Gtk.AccelKey(Gdk.Key.N, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible));
            OpenNewConnectionAction.Image = new Gtk.Image(Gtk.Stock.Connect, Gtk.IconSize.Menu);
            FileAction.Submenu = this.FileMenu;
            FileMenu.Append(OpenNewConnectionAction);
            FileMenu.Append(new Gtk.SeparatorMenuItem());
            FavoriteNetworksAction = new Gtk.MenuItem(messages.Localize("[[window-menu-favorite]]"));
            FileMenu.Append(FavoriteNetworksAction);
            FileMenu.Append(new Gtk.SeparatorMenuItem());
            PreferencesAction = new Gtk.ImageMenuItem(messages.Localize("[[window-menu-conf]]"));
            PreferencesAction.Image = new Gtk.Image(Gtk.Stock.Preferences, Gtk.IconSize.Menu);
            FileMenu.Append(PreferencesAction);
            FileMenu.Append(new Gtk.SeparatorMenuItem());
            ShutDownAction = new Gtk.ImageMenuItem(messages.Localize("[[window-menu-quit]]"));
            FileMenu.Append(ShutDownAction);
            ////////////////////// TOOLS //////////////////////
            ToolsAction = new Gtk.MenuItem(messages.Localize("[[window-menu-tools]]"));
            ToolsAction.Submenu = ToolsMenu;
            PacketViewerAction = new Gtk.MenuItem(messages.Localize("[[window-menu-viewer]]"));
            PacketViewerAction.AddAccelerator("activate", agrp, new Gtk.AccelKey(Gdk.Key.P, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible));
            ToolsMenu.Append(PacketViewerAction);
            SmallChatAction = new Gtk.MenuItem(messages.Localize("[[window-menu-chat]]"));
            ToolsMenu.Append(SmallChatAction);
            ToolsMenu.Append(new Gtk.SeparatorMenuItem());
            AttachToMicroChatAction = new Gtk.ImageMenuItem(messages.Localize("[[window-menu-chat1]]"));
            AttachToMicroChatAction.AddAccelerator("activate", agrp, new Gtk.AccelKey(Gdk.Key.M, Gdk.ModifierType.Mod1Mask, Gtk.AccelFlags.Visible));
            ToolsMenu.Append(AttachToMicroChatAction);
            DetachFromMicroChatAction = new Gtk.ImageMenuItem(messages.Localize("[[window-menu-chat2]]"));
            DetachFromMicroChatAction.AddAccelerator("activate", agrp, new Gtk.AccelKey(Gdk.Key.D, Gdk.ModifierType.Mod1Mask, Gtk.AccelFlags.Visible));
            ToolsMenu.Append(DetachFromMicroChatAction);
            ////////////////////// MISC //////////////////////
            MiscAction = new Gtk.MenuItem(messages.Localize("[[window-menu-misc]]"));
            MiscAction.Submenu = MiscMenu;
            SearchAction = new Gtk.MenuItem(messages.Localize("[[window-menu-search]]"));
            SearchAction.AddAccelerator("activate", agrp, new Gtk.AccelKey(Gdk.Key.F, Gdk.ModifierType.ControlMask, Gtk.AccelFlags.Visible));
            MiscMenu.Append(SearchAction);
            LoadMoreToScrollbackAction = new Gtk.ImageMenuItem(messages.Localize("[[window-menu-more]]"));
            LoadMoreToScrollbackAction.AddAccelerator("activate", agrp, new Gtk.AccelKey(Gdk.Key.U, Gdk.ModifierType.Mod1Mask, Gtk.AccelFlags.Visible));
            MiscMenu.Append(LoadMoreToScrollbackAction);
            MiscMenu.Append(new Gtk.SeparatorMenuItem());
            ConfigurationFileAction = new Gtk.MenuItem(messages.Localize("[[window-menu-cf]]"));
            ////////////////////// SHOW //////////////////////
            ShowAction = new Gtk.MenuItem(messages.Localize("[[window-menu-show]]"));
            ShowAction.Submenu = ShowMenu;
            RootAction = new Gtk.MenuItem(messages.Localize("[[window-menu-root]]"));
            ShowMenu.Append(RootAction);
            HelpAction = new Gtk.ImageMenuItem(messages.Localize("[[window-menu-help]]"));
            HelpAction.Submenu = HelpMenu;
            MiscMenu.Append(ConfigurationFileAction);
            AboutAction = new Gtk.ImageMenuItem(messages.Localize("[[window-menu-about]]"));
            AboutAction.Image = new Gtk.Image(Gtk.Stock.About, Gtk.IconSize.Menu);
            ContentsAction = new Gtk.MenuItem(messages.Localize("[[window-menu-contents]]"));
            HelpMenu.Append(AboutAction);
            FavoriteNetworksAction.Sensitive = false;
            ContentsAction.AddAccelerator("activate", agrp, new Gtk.AccelKey(Gdk.Key.F1, Gdk.ModifierType.None, Gtk.AccelFlags.Visible));
            HelpMenu.Append(ContentsAction);
            menubar2.Add(this.FileAction);
            menubar2.Add(this.ToolsAction);
            menubar2.Add(this.MiscAction);
            this.menubar2.Add(this.ShowAction);
            this.menubar2.Add(this.HelpAction);
            this.Name = "Client.Forms.Main";
            this.Title = "Pidgeon Client";
            this.Icon = global::Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            // Container child Client.Forms.Main.Gtk.Container+ContainerChild
            this.vbox3 = new global::Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            // Container child vbox3.Gtk.Box+BoxChild
            this.menubar2.Name = "menubar2";
            this.vbox3.Add(this.menubar2);
            global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.menubar2]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.hpaned1 = new global::Gtk.HPaned();
            this.hpaned1.CanFocus = true;
            this.hpaned1.Name = "hpaned1";
            this.hpaned1.Position = 183;
            // Container child hpaned1.Gtk.Paned+PanedChild
            this.pidgeonlist1 = new global::Client.Graphics.PidgeonList();
            this.pidgeonlist1.Events = ((global::Gdk.EventMask)(256));
            this.pidgeonlist1.Name = "pidgeonlist1";

            this.hpaned1.Add(this.pidgeonlist1);
            global::Gtk.Paned.PanedChild w3 = ((global::Gtk.Paned.PanedChild)(this.hpaned1[this.pidgeonlist1]));
            w3.Resize = false;
            this.vbox3.Add(this.hpaned1);
            global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.hpaned1]));
            w4.Position = 1;
            // Container child vbox3.Gtk.Box+BoxChild
            this.toolStrip = new global::Gtk.Statusbar();
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Spacing = 6;
            // Container child toolStrip.Gtk.Box+BoxChild
            this.toolStripStatusNetwork = new global::Gtk.Label();
            this.toolStripStatusNetwork.Name = "toolStripStatusNetwork";
            this.toolStrip.Add(this.toolStripStatusNetwork);
            global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.toolStrip[this.toolStripStatusNetwork]));
            w5.Position = 0;
            w5.Expand = false;
            w5.Fill = false;
            // Container child toolStrip.Gtk.Box+BoxChild
            this.toolStripStatusChannel = new global::Gtk.Label();
            this.toolStripStatusChannel.Name = "toolStripStatusChannel";
            this.toolStrip.Add(this.toolStripStatusChannel);
            global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.toolStrip[this.toolStripStatusChannel]));
            w6.Position = 1;
            w6.Expand = false;
            w6.Fill = false;
            // Container child toolStrip.Gtk.Box+BoxChild
            this.toolStripInfo = new global::Gtk.Label();
            this.toolStripInfo.Name = "toolStripInfo";
            this.toolStrip.Add(this.toolStripInfo);
            global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.toolStrip[this.toolStripInfo]));
            w7.Position = 3;
            w7.Expand = false;
            w7.Fill = false;
            // Container child toolStrip.Gtk.Box+BoxChild
            this.toolStripProgressBar1 = new global::Gtk.ProgressBar();
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStrip.Add(this.toolStripProgressBar1);
            global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.toolStrip[this.toolStripProgressBar1]));
            w8.Position = 4;
            this.vbox3.Add(this.toolStrip);
            global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox3[this.toolStrip]));
            w9.Position = 2;
            w9.Expand = false;
            w9.Fill = false;
            this.Add(this.vbox3);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 1018;
            this.DefaultHeight = 600;
            this.Show();
            this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.Unshow);
            this.ShutDownAction.Activated += new global::System.EventHandler(this.shutDownToolStripMenuItem_Click);
            this.AboutAction.Activated += new global::System.EventHandler(this.aboutToolStripMenuItem_Click);
            this.OpenNewConnectionAction.Activated += new global::System.EventHandler(this.newConnectionToolStripMenuItem_Click_1);
            this.PreferencesAction.Activated += new global::System.EventHandler(this.preferencesToolStripMenuItem_Click);
            this.PacketViewerAction.Activated += new global::System.EventHandler(this.toolStripMenuItem3_Click);
            this.SmallChatAction.Activated += new global::System.EventHandler(this.taskbarBoxToolStripMenuItem_Click);
            this.AttachToMicroChatAction.Activated += new global::System.EventHandler(this.attachToMicroChatToolStripMenuItem_Click);
            this.DetachFromMicroChatAction.Activated += new global::System.EventHandler(this.detachFromMicroChatToolStripMenuItem_Click);
        }
    }
}
