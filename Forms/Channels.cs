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

namespace Client.Forms
{
    /// <summary>
    /// Channel list
    /// </summary>
    public partial class Channels : GTK.PidgeonForm
    {
        private Network network = null;
        private bool Loaded = false;
        private int channels = 0;
        private List<Network.ChannelData> channelData = new List<Network.ChannelData>();
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TreeView treeview8;
        private Gtk.ListStore data = new Gtk.ListStore(typeof(string), typeof(uint), typeof(string), typeof(Network.ChannelData)); 
        private GTK.Menu refreshToolStripMenuItem = new GTK.Menu("Refresh");
        private GTK.Menu knockToolStripMenuItem = new GTK.Menu("Knock");
        private GTK.Menu joinToolStripMenuItem = new GTK.Menu("Join");
        private GTK.Menu downloadListFromServerToolStripMenuItem = new GTK.Menu("Download from server");
        private GTK.Menu listtoclipboard = new GTK.Menu("Copy selected to clipboard");
        //private GTK.Menu listtoclipboard2 = new GTK.Menu("Copy selected channel names (separated by space) to clipboard");
        
        /// <summary>
        /// Selected channels
        /// </summary>
        public List<string> Selected
        {
            get
            {
                List<string> ul = new List<string>();
                TreeIter iter;
                TreePath[] path = this.treeview8.Selection.GetSelectedRows();
                foreach (TreePath tree in path)
                {
                    this.treeview8.Model.GetIter(out iter, tree);
                    string user = (string)this.treeview8.Model.GetValue(iter, 0);
                    ul.Add(user);
                }
                return ul;
            }
        }

        /// <summary>
        /// Selected channels and data for these channels
        /// </summary>
        public List<string> SelectedData
        {
            get
            {
                List<string> ul = new List<string>();
                TreeIter iter;
                TreePath[] path = this.treeview8.Selection.GetSelectedRows();
                foreach (TreePath tree in path)
                {
                    this.treeview8.Model.GetIter(out iter, tree);
                    string user = (string)this.treeview8.Model.GetValue(iter, 0);
                    string topic = (string)this.treeview8.Model.GetValue(iter, 2);
                    ul.Add(user + " topic: " + topic);
                }
                return ul;
            }
        }

        /// <summary>
        /// Creates a new instance of this list
        /// </summary>
        /// <param name="nw"></param>
        public Channels(Network nw)
        {
            network = nw;
            this.Build();
            this.Init();
        }

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget MainWindow
            this.Name = "MainWindow";
            this.Title = "Channel list";
            this.TypeHint = Gdk.WindowTypeHint.Normal;
            this.Icon = Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.WindowPosition = WindowPosition.Center;
            // Container child MainWindow.Gtk.Container+ContainerChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.treeview8 = new global::Gtk.TreeView();
            this.treeview8.CanFocus = true;
            this.treeview8.Name = "treeview8";
            this.DeleteEvent += new DeleteEventHandler(destroy);
            this.GtkScrolledWindow.Add(this.treeview8);
            this.Add(this.GtkScrolledWindow);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 800;
            this.DefaultHeight = 520;
        }
        
        [GLib.ConnectBefore]
        private void Menu2(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 3)
            {
                e.RetVal = true;
                Menu(sender, null);
            }
        }

        private void Init()
        {
            Gtk.TreeViewColumn name = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn size = new Gtk.TreeViewColumn();
            Gtk.TreeViewColumn topic_item = new Gtk.TreeViewColumn();
            Gtk.CellRendererText c1 = new Gtk.CellRendererText();
            Gtk.CellRendererText c2 = new Gtk.CellRendererText();
            Gtk.CellRendererText c3 = new Gtk.CellRendererText();
            name.Title = "Name";
            size.Title = "Users";
            topic_item.Title = "Channel topic";
            name.PackStart(c1, true);
            size.PackStart(c2, true);
            topic_item.PackStart(c3, true);
            name.AddAttribute    (c1, "text", 0);
            size.AddAttribute    (c2, "text", 1);
            topic_item.AddAttribute    (c3, "text", 2);
            treeview8.PopupMenu += new PopupMenuHandler(Menu);
            this.treeview8.Model = data;
            treeview8.AppendColumn(name);
            treeview8.AppendColumn(size);
            treeview8.AppendColumn(topic_item);
            Reload();
            this.treeview8.Selection.Mode = SelectionMode.Multiple;
            this.treeview8.ButtonPressEvent += new ButtonPressEventHandler(Menu2);
        }
        
        private void destroy(object o, Gtk.DeleteEventArgs e)
        {
            Hide();
            e.RetVal = true;
        }
        
        [GLib.ConnectBefore]
        private void Menu(object sender, Gtk.PopupMenuArgs e)
        {
            try
            {
                Gtk.Menu xx = new Menu();
                Gtk.MenuItem join = new MenuItem(joinToolStripMenuItem.Text);
                join.Activated += new EventHandler(joinToolStripMenuItem_Click);
                xx.Append(join);
                Gtk.MenuItem knock = new MenuItem(knockToolStripMenuItem.Text);
                knock.Activated += new EventHandler(knockToolStripMenuItem_Click);
                xx.Append(knock);
                Gtk.MenuItem download = new MenuItem(downloadListFromServerToolStripMenuItem.Text);
                download.Activated += new EventHandler(downloadListFromServerToolStripMenuItem_Click);
                xx.Append(download);
                Gtk.MenuItem re = new MenuItem(refreshToolStripMenuItem.Text);
                re.Activated += new EventHandler(refreshToolStripMenuItem_Click);
                Gtk.MenuItem copy = new MenuItem(listtoclipboard.Text);
                copy.Activated += new EventHandler(ToClipboard);
                xx.Append(re);
                xx.Append(new Gtk.SeparatorMenuItem());
                xx.Append(copy);
                xx.ShowAll();
                xx.Popup();
            } catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void ToClipboard(object sender, EventArgs e)
        {
            string list = "";
            foreach (string item in SelectedData)
            {
                list += item + Environment.NewLine;
            }
            System.Windows.Forms.Clipboard.SetText(list);
        }

        private void Reload()
        {
            data.Clear();
            channels = 0;
            lock (network.ChannelList)
            {
                if (network.ChannelList.Count > 0)
                {
                    channelData.Clear();
                    channelData.AddRange(network.ChannelList);
                }
            }
            if (channelData.Count > 0)
            {
                foreach (Network.ChannelData info in channelData)
                {
                    data.AppendValues (info.ChannelName, info.UserCount, info.ChannelTopic);
                }
                Loaded = true;
            } else
            {
                Loaded = false;
                data.AppendValues("No channels were loaded so far, download list first", 0, "");
            }
            Title = "Channels on " + network.ServerName + " [" + channelData.Count.ToString() + "]";
        }

        private void Channels_Close(object sender, Gtk.DestroyEventArgs e)
        {
            try
            {
                e.RetVal = true;
                Hide();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private bool timerl_Tick()
        {
            try
            {
                if (network.DownloadingList)
                {
                    return true;
                }
                if (channels != network.ChannelList.Count)
                {
                    Reload();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return false;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void downloadListFromServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                network.SuppressData = true;
                network.Transfer("LIST");
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void joinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Loaded)
                {
                    foreach (string item in Selected)
                    {
                        if (item != "")
                        {
                            network.Join(item);
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void knockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Loaded)
                {
                    foreach (string item in Selected)
                    {
                        network.Transfer("KNOCK " + item);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}

