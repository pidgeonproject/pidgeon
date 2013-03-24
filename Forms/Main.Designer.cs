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
using Gtk;

namespace Client.Forms
{
	public partial class Main : Gtk.Window
	{
		private Gtk.VBox vbox1 = null;
		private Gtk.Statusbar statusbar = null;
		public Gtk.MenuBar menuBar = null;
		
		//private System.Windows.Forms.ToolStripStatusLabel toolStripInfo;
        //private System.Windows.Forms.ToolStripStatusLabel toolStripCo;
        //public System.Windows.Forms.MenuStrip menuStrip1;
        public Gtk.MenuItem newConnectionToolStripMenuItem;
        // toolStripMenuItem1;
        public Gtk.MenuItem preferencesToolStripMenuItem;
        //private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        public Gtk.MenuItem shutDownToolStripMenuItem;
        public Gtk.MenuItem contentsToolStripMenuItem;
        public Gtk.MenuItem aboutToolStripMenuItem;
        //public System.Windows.Forms.ToolStripStatusLabel toolStripCurrent;
        //public System.Windows.Forms.ToolStripStatusLabel toolStripStatusChannel;
        //public System.Windows.Forms.ToolStripStatusLabel toolStripStatusNetwork;
        //private System.Windows.Forms.Timer updater;
        public Gtk.MenuItem toolStripMenuItem3;
        public Gtk.MenuItem taskbarBoxToolStripMenuItem;
        //private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        public Gtk.MenuItem attachToMicroChatToolStripMenuItem;
        public Gtk.MenuItem detachFromMicroChatToolStripMenuItem;
        public Gtk.MenuItem searchToolStripMenuItem;
        //public System.Windows.Forms.SplitContainer sX;
        public Gtk.MenuItem rootToolStripMenuItem;
        //private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        public Gtk.MenuItem switchToAdvancedLayoutToolStripMenuItem;
        public Gtk.MenuItem configurationFileToolStripMenuItem;
        public Gtk.MenuItem skinEdToolStripMenuItem;
        //public System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        public Gtk.MenuItem fileToolStripMenuItem;
		public Gtk.Menu   fileToolStripMenu;
        public Gtk.Menu toolsToolStripMenu;
		public Gtk.MenuItem toolsToolStripMenuItem;
        public Gtk.MenuItem helpToolStripMenuItem;
		public Gtk.Menu helpToolStripMenu;
		public Gtk.Menu userToolStripMenu;
        public Gtk.MenuItem miscToolStripMenuItem;
		public Gtk.Menu miscToolStripMenu;
        public Gtk.Menu showToolStripMenu;
		public Gtk.MenuItem showToolStripMenuItem;
        public Gtk.MenuItem userToolStripMenuItem;
		
        public Gtk.MenuItem favoriteNetworksToolStripMenuItem;
        public Gtk.MenuItem toolStripMenuItem6;
		 
		/// <summary>
		/// Initializes the component.
		/// </summary>
		public void InitializeComponent ()
		{
			/*this.Name = "Pidgeon";
			this.Title = "Pidgeon v. 1.0";
			this.DefaultSize = new Gdk.Size(800, 600);
			if (Configuration.Window.Window_Maximized)
			{
				this.Maximize();
			}
			
			vbox1 = new Gtk.VBox(false, 1);
			this.Add (vbox1);
			
			// Menu
			menuBar = new Gtk.MenuBar();
			
			vbox1.PackStart (menuBar, false, false, 0);
			
			//File
			fileToolStripMenu = new Gtk.Menu();
			fileToolStripMenuItem = new Gtk.MenuItem(messages.get ("window-menu-file"));
			fileToolStripMenuItem.Submenu = fileToolStripMenu;
			
			//Shutdown
			shutDownToolStripMenuItem = new Gtk.MenuItem("Quit");
			fileToolStripMenu.Append(shutDownToolStripMenuItem);
			
			menuBar.Append(fileToolStripMenuItem);
		
			statusbar = new Gtk.Statusbar();
			vbox1.Add(statusbar);
			
			ShowAll();
			*/
		}
	}
}

