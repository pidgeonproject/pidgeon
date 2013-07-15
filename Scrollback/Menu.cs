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
using System.Threading;
using System.Text;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Client
{
    public partial class Scrollback
    {
        /// <summary>
        /// The toggle advanced layout tool strip menu item.
        /// </summary>
        public GTK.Menu toggleAdvancedLayoutToolStripMenuItem = new GTK.Menu("Toggle advanced layout");
        /// <summary>
        /// The toggle simple layout tool strip menu item.
        /// </summary>
        public GTK.Menu toggleSimpleLayoutToolStripMenuItem = new GTK.Menu("Toggle simple view");
        /// <summary>
        /// The ban tool strip menu item.
        /// </summary>
        public GTK.Menu banToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The list all channels tool strip menu item.
        /// </summary>
        public GTK.Menu listAllChannelsToolStripMenuItem = new GTK.Menu("List all channels on this network");
        /// <summary>
        /// The retrieve topic tool strip menu item.
        /// </summary>
        public GTK.Menu retrieveTopicToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The channel tool strip menu item.
        /// </summary>
        public GTK.Menu channelToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The mode1b2 tool strip menu item.
        /// </summary>
        public GTK.Menu mode1b2ToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The mode1e2 tool strip menu item.
        /// </summary>
        public GTK.Menu mode1e2ToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The mode1 i2 tool strip menu item.
        /// </summary>
        public GTK.Menu mode1I2ToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The mode1q2 tool strip menu item.
        /// </summary>
        public GTK.Menu mode1q2ToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The kick tool strip menu item.
        /// </summary>
        public GTK.Menu kickToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The whois tool strip menu item.
        /// </summary>
        public GTK.Menu whoisToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The whowas tool strip menu item.
        /// </summary>
        public GTK.Menu whowasToolStripMenuItem = new GTK.Menu();
        /// <summary>
        /// The tool strip menu item1.
        /// </summary>
        public GTK.Menu toolStripMenuItem1 = new GTK.Menu();
        /// <summary>
        /// The tool strip menu item2.
        /// </summary>
        public GTK.Menu toolStripMenuItem2 = new GTK.Menu();
        /// <summary>
        /// The copy entire window to clip board tool strip menu item.
        /// </summary>
        public GTK.Menu copyEntireWindowToClipBoardToolStripMenuItem = new GTK.Menu("Copy entire window to clipboard");
        /// <summary>
        /// The copy text to clip board tool strip menu item.
        /// </summary>
        public GTK.Menu copyTextToClipBoardToolStripMenuItem = new GTK.Menu("Copy text to clipboard");
        /// <summary>
        /// The scroll tool strip menu item.
        /// </summary>
        public GTK.Menu scrollToolStripMenuItem = new GTK.Menu("Scroll");
        /// <summary>
        /// The open link in browser tool strip menu item.
        /// </summary>
        public GTK.Menu openLinkInBrowserToolStripMenuItem = new GTK.Menu("Open link in a new browser");
        /// <summary>
        /// The copy link to clipboard tool strip menu item.
        /// </summary>
        public GTK.Menu copyLinkToClipboardToolStripMenuItem = new GTK.Menu("Copy link to clipboard");
        /// <summary>
        /// Join menu
        /// </summary>
        public GTK.Menu joinToolStripMenuItem = new GTK.Menu("Join");
        /// <summary>
        /// Currently selected user
        /// </summary>
        public string SelectedUser = null;

        private bool CreatingMenu = false;
        
        /// <summary>
        /// View type
        /// </summary>
        public enum ViewType
        {
            /// <summary>
            /// Channel
            /// </summary>
            Channel,
            /// <summary>
            /// User
            /// </summary>
            User,
            /// <summary>
            /// link
            /// </summary>
            Link
        }

        [GLib.ConnectBefore]
        private void CreateMenu_rt(object o, Gtk.PopulatePopupArgs e)
        {
            try
            {
                CreateMenu(o, e);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        [GLib.ConnectBefore]
        private void CreateMenu_simple(object o, Gtk.PopulatePopupArgs e)
        {
            try
            {
                CreateMenu(o, e);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void CreateMenu(object o, Gtk.PopulatePopupArgs e)
        {
            CreatingMenu = true;
            Gtk.SeparatorMenuItem separator1 = new Gtk.SeparatorMenuItem();
            separator1.Show();
            e.Menu.Append(separator1);
            e.Menu.Append(new Gtk.SeparatorMenuItem());

            listAllChannelsToolStripMenuItem.Visible = (owner != null && owner._Network != null);

            // channels
            if (joinToolStripMenuItem.Visible)
            {
                Gtk.MenuItem join = new Gtk.MenuItem(joinToolStripMenuItem.Text);
                join.Show();
                join.Activated += new EventHandler(joinToolStripMenuItem_Click);
                e.Menu.Append(join);
                Gtk.SeparatorMenuItem separator6 = new Gtk.SeparatorMenuItem();
                separator6.Show();
                e.Menu.Append(separator6);
            }

            if (openLinkInBrowserToolStripMenuItem.Visible)
            {
                Gtk.MenuItem copylink = new Gtk.MenuItem(copyLinkToClipboardToolStripMenuItem.Text);
                copylink.Show();
                copylink.Activated += new EventHandler(copyLinkToClipboardToolStripMenuItem_Click);
                e.Menu.Append(copylink);
            }

            if (openLinkInBrowserToolStripMenuItem.Visible)
            {
                Gtk.MenuItem open = new Gtk.MenuItem(openLinkInBrowserToolStripMenuItem.Text);
                open.Show();
                open.Activated += new EventHandler(openLinkInBrowserToolStripMenuItem_Click);
                e.Menu.Append(open);
                Gtk.SeparatorMenuItem separator8 = new Gtk.SeparatorMenuItem();
                separator8.Show();
                e.Menu.Append(separator8);
            }

            // whois items
            if (whoisToolStripMenuItem.Visible)
            {
                Gtk.MenuItem whois = new Gtk.MenuItem(whoisToolStripMenuItem.Text);
                whois.Show();
                whois.Activated += new EventHandler(whoisToolStripMenuItem_Click);
                e.Menu.Append(whois);

                Gtk.MenuItem whowas = new Gtk.MenuItem(whowasToolStripMenuItem.Text);
                whowas.Show();
                whowas.Activated += new EventHandler(whowasToolStripMenuItem_Click);
                e.Menu.Append(whowas);

                if (kickToolStripMenuItem.Visible)
                {
                    Gtk.MenuItem ku = new Gtk.MenuItem(kickToolStripMenuItem.Text);
                    ku.Show();
                    ku.Activated += new EventHandler(kickToolStripMenuItem_Click);
                    e.Menu.Append(ku);
                }

                Gtk.SeparatorMenuItem separator3 = new Gtk.SeparatorMenuItem();
                separator3.Show();
                e.Menu.Append(separator3);
            }

            if (mode1b2ToolStripMenuItem.Visible)
            {
                Gtk.MenuItem mode1 = new Gtk.MenuItem(mode1b2ToolStripMenuItem.Text);
                mode1.Show();
                mode1.Activated += new EventHandler(mode1b2ToolStripMenuItem_Click);
                e.Menu.Append(mode1);

                Gtk.MenuItem mode2 = new Gtk.MenuItem(mode1e2ToolStripMenuItem.Text);
                mode2.Show();
                mode2.Activated += new EventHandler(mode1e2ToolStripMenuItem_Click);
                e.Menu.Append(mode2);

                Gtk.MenuItem mode3 = new Gtk.MenuItem(mode1I2ToolStripMenuItem.Text);
                mode3.Show();
                mode3.Activated += new EventHandler(mode1I2ToolStripMenuItem_Click);
                e.Menu.Append(mode3);

                Gtk.MenuItem mode4 = new Gtk.MenuItem(mode1q2ToolStripMenuItem.Text);
                mode4.Show();
                mode4.Activated += new EventHandler(mode1q2ToolStripMenuItem_Click);
                e.Menu.Append(mode4);

                Gtk.SeparatorMenuItem separator4 = new Gtk.SeparatorMenuItem();
                separator4.Show();
                e.Menu.Append(separator4);
            }

            Gtk.MenuItem clean = new Gtk.MenuItem("Clean");
            clean.Show();
            e.Menu.Append(clean);
            clean.Activated += new EventHandler(mrhToolStripMenuItem_Click);
            Gtk.CheckMenuItem scroll = new Gtk.CheckMenuItem(scrollToolStripMenuItem.Text);
            if (scrollToolStripMenuItem.Checked)
            {
                scroll.Active = true;
            }
            scroll.Show();
            scroll.Activated += new EventHandler(scrollToolStripMenuItem_Click);
            e.Menu.Append(scroll);
            Gtk.MenuItem refresh = new Gtk.MenuItem("Refresh");
            refresh.Show();
            refresh.Activated += new EventHandler(refreshToolStripMenuItem_Click);
            e.Menu.Append(refresh);
            Gtk.CheckMenuItem taly = new Gtk.CheckMenuItem(toggleAdvancedLayoutToolStripMenuItem.Text);
            taly.Activated += new EventHandler(toggleAdvancedLayoutToolStripMenuItem_Click);
            if (toggleAdvancedLayoutToolStripMenuItem.Checked)
            {
                taly.Active = true;
            }
            taly.Show();
            e.Menu.Append(taly);
            Gtk.CheckMenuItem tsly = new Gtk.CheckMenuItem("Toggle simple layout");
            tsly.Activated += new EventHandler(toggleSimpleLayoutToolStripMenuItem_Click);
            if (toggleSimpleLayoutToolStripMenuItem.Checked)
            {
                tsly.Active = true;
            }
            tsly.Show();
            e.Menu.Append(tsly);
            Gtk.SeparatorMenuItem separator2 = new Gtk.SeparatorMenuItem();
            separator2.Show();
            e.Menu.Append(separator2);
            if (channelToolStripMenuItem.Visible)
            {
                Gtk.MenuItem channel = new Gtk.MenuItem("Channel");
                channel.Show();
                e.Menu.Append(channel);
                channel.Activated += new EventHandler(channelToolStripMenuItem_Click);
            }
            if (listAllChannelsToolStripMenuItem.Visible)
            {
                Gtk.MenuItem list = new Gtk.MenuItem(listAllChannelsToolStripMenuItem.Text);
                list.Show();
                list.Activated += new EventHandler(listAllChannelsToolStripMenuItem_Click);
                e.Menu.Append(list);
            }
            if (retrieveTopicToolStripMenuItem.Visible)
            {
                Gtk.MenuItem retrieve = new Gtk.MenuItem("Retrieve topic");
                retrieve.Show();
                retrieve.Activated += new EventHandler(retrieveTopicToolStripMenuItem_Click);
                e.Menu.Append(retrieve);
            }
            Gtk.MenuItem copy = new Gtk.MenuItem(copyTextToClipBoardToolStripMenuItem.Text);
            copy.Activated += new EventHandler(copyTextToClipBoardToolStripMenuItem_Click);
            copy.Show();
            e.Menu.Append(copy);
            Gtk.MenuItem copy_x = new Gtk.MenuItem(copyEntireWindowToClipBoardToolStripMenuItem.Text);
            copy_x.Activated += new EventHandler(copyEntireWindowToClipBoardToolStripMenuItem_Click);
            copy_x.Show();
            e.Menu.Append(copy_x);
            CreatingMenu = false;
        }

        private void Click_R(string adds, string data)
        {
            try
            {
                if (data.StartsWith("http"))
                {
                    ViewLn(data, ViewType.Link);
                }
                if (data.StartsWith("pidgeon://text"))
                {
                    ViewLn(adds, ViewType.User, adds);
                }
                if (data.StartsWith("pidgeon://join"))
                {
                    ViewLn(data, ViewType.Channel);
                }
                if (data.StartsWith("pidgeon://ident"))
                {
                    ViewLn("*!" + adds + "@*", ViewType.User);
                }
                if (data.StartsWith("pidgeon://user"))
                {
                    if (this.owner != null && this.owner.isChannel)
                    {
                        Channel channel = owner._Network.getChannel(owner.WindowName);
                        if (channel != null)
                        {
                            User user = channel.UserFromName(adds);
                            if (user != null)
                            {
                                if (user.Host != "")
                                {
                                    ViewLn("*@" + user.Host, ViewType.User, adds);
                                    return;
                                }
                            }
                        }
                    }
                    ViewLn(adds + "!*@*", ViewType.User, adds);
                }
                if (data.StartsWith("pidgeon://hostname"))
                {
                    ViewLn("*@" + adds, ViewType.User);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void ViewLn(string content, ViewType type, string name = "")
        {
            if (owner != null)
            {
                toolStripMenuItem1.Visible = true;
                Link = content;
                if (type == ViewType.Channel)
                {
                    if (owner.isChannel)
                    {
                        mode1b2ToolStripMenuItem.Visible = false;
                        mode1e2ToolStripMenuItem.Visible = false;
                        mode1I2ToolStripMenuItem.Visible = false;
                        mode1q2ToolStripMenuItem.Visible = false;
                        kickToolStripMenuItem.Visible = false;
                        whoisToolStripMenuItem.Visible = false;
                        whowasToolStripMenuItem.Visible = false;
                    }
                    toolStripMenuItem2.Visible = false;
                    openLinkInBrowserToolStripMenuItem.Visible = false;
                    copyLinkToClipboardToolStripMenuItem.Visible = false;
                    openLinkInBrowserToolStripMenuItem.Visible = false;
                    joinToolStripMenuItem.Visible = true;
                    return;
                }
                if (type == ViewType.User)
                {
                    if (owner.isChannel)
                    {
                        kickToolStripMenuItem.Visible = true;
                        whoisToolStripMenuItem.Visible = true;
                        whowasToolStripMenuItem.Visible = true;
                        mode1e2ToolStripMenuItem.Visible = true;
                        mode1I2ToolStripMenuItem.Visible = true;
                        mode1q2ToolStripMenuItem.Visible = true;
                        mode1q2ToolStripMenuItem.Text = "/mode " + owner.WindowName + " +q " + content;
                        mode1I2ToolStripMenuItem.Text = "/mode " + owner.WindowName + " +I " + content;
                        mode1e2ToolStripMenuItem.Text = "/mode " + owner.WindowName + " +e " + content;
                        mode1b2ToolStripMenuItem.Text = "/mode " + owner.WindowName + " +b " + content;
                        kickToolStripMenuItem.Text = "/raw KICK " + owner.WindowName + " " + name + " :" + Configuration.irc.DefaultReason;
                        whoisToolStripMenuItem.Text = "/whois " + name;
                        whowasToolStripMenuItem.Text = "/whowas " + name;
                        mode1b2ToolStripMenuItem.Visible = true;
                        if (name == "")
                        {
                            kickToolStripMenuItem.Visible = false;
                            whoisToolStripMenuItem.Visible = false;
                            whowasToolStripMenuItem.Visible = false;
                        }
                    }
                    toolStripMenuItem2.Visible = true;
                    toolStripMenuItem1.Visible = true;
                    copyLinkToClipboardToolStripMenuItem.Visible = false;
                    openLinkInBrowserToolStripMenuItem.Visible = false;
                    joinToolStripMenuItem.Visible = false;
                }
                if (type == ViewType.Link)
                {
                    if (owner.isChannel)
                    {
                        mode1b2ToolStripMenuItem.Visible = false;
                        mode1e2ToolStripMenuItem.Visible = false;
                        mode1I2ToolStripMenuItem.Visible = false;
                        mode1q2ToolStripMenuItem.Visible = false;
                        kickToolStripMenuItem.Visible = false;
                        whoisToolStripMenuItem.Visible = false;
                        whowasToolStripMenuItem.Visible = false;
                    }
                    openLinkInBrowserToolStripMenuItem.Visible = false;
                    copyLinkToClipboardToolStripMenuItem.Visible = true;
                    openLinkInBrowserToolStripMenuItem.Visible = true;
                    joinToolStripMenuItem.Visible = false;
                    toolStripMenuItem2.Visible = false;
                }
            }
        }

        [GLib.ConnectBefore]
        private void Click(object sender, Gtk.ButtonPressEventArgs e)
        {
            try
            {
                if (e.Event.Button == 3)
                {
                    if (SelectedLink != null && SelectedUser != null)
                    {
                        Click_R(SelectedUser, SelectedLink);
                    }
                }
                if (e.Event.Button == 1)
                {
                    if (SelectedLink != null)
                    {
                        Click_L(SelectedLink);
                    }
                }
                if (owner != null)
                {
                    owner.textbox.setFocus();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Click_L(string http)
        {
            if (http.StartsWith("https://") ||
                http.StartsWith("http://") ||
                http.StartsWith("ftp://"))
            {
                try
                {
                    if (Configuration.UserData.OpenLinkInBrowser)
                    {
                        Hyperlink.OpenLink(http);
                    }
                    // we don't need to continue with execution of this parser because it's not possible for http to start with any other value
                    return;
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to open " + http, "Link", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            if (http.StartsWith("irc://"))
            {
                if (owner != null)
                {
                    if (owner._Network != null && owner._Network.ParentSv != null)
                    {
                        Core.ParseLink(http, owner._Network.ParentSv);
                        return;
                    }
                }
                Core.ParseLink(http);
                return;
            }
            if (http.StartsWith("pidgeon://"))
            {
                string command = http.Substring("pidgeon://".Length);
                if (command.EndsWith("/"))
                {
                    command = command.Substring(0, command.Length - 1);
                }
                if (command.StartsWith("user/#"))
                {
                    string nick = command.Substring(6);
                    if (owner != null && owner._Network != null)
                    {
                        if (owner.isChannel || owner.isPM)
                        {
                            owner.textbox.richTextBox.Buffer.Text += nick + ": ";
                            owner.textbox.setFocus();
                        }
                    }
                    return;
                }
                if (command.StartsWith("join/#"))
                {
                    Parser.parse("/join " + command.Substring(5));
                    return;
                }
                if (command.StartsWith("ident/#"))
                {
                    string nick = command.Substring(7);
                    if (owner != null && owner._Network != null)
                    {
                        if (owner.isChannel)
                        {
                            owner.textbox.richTextBox1.Buffer.Text += nick;
                            owner.textbox.setFocus();
                        }
                    }
                    return;
                }
                if (command.StartsWith("hostname/#"))
                {
                    string nick = command.Substring(10);
                    if (owner != null && owner._Network != null)
                    {
                        if (owner.isChannel)
                        {
                            owner.textbox.richTextBox1.Buffer.Text += nick;
                            owner.textbox.setFocus();
                        }
                    }
                    return;
                }
                if (command.StartsWith("text/#"))
                {
                    string nick = command.Substring(6);
                    if (owner != null && owner._Network != null)
                    {
                        if (owner.isChannel)
                        {
                            owner.textbox.richTextBox1.Buffer.Text += nick;
                            owner.textbox.setFocus();
                        }
                    }
                    return;
                }
                if (command.StartsWith("join#"))
                {
                    Parser.parse("/join " + command.Substring(4));
                    return;
                }
            }
        }

        private void channelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Core.SelectedNetwork != null)
                {
                    if (Core.SelectedNetwork.RenderedChannel != null)
                    {
                        Forms.Channel_Info info = new Forms.Channel_Info(Core.SelectedNetwork.RenderedChannel);
                        info.Show();
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void mrhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if ((GTK.MessageBox.Show(null, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, "Do you really want to clear this window", "Warning").result == Gtk.ResponseType.Yes))
                {
                    lock (ContentLines)
                    {
                        ContentLines.Clear();
                        lastDate = DateTime.MinValue;
                        Reload(true);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void scrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                scrollToolStripMenuItem.Checked = !scrollToolStripMenuItem.Checked;
                ScrollingEnabled = scrollToolStripMenuItem.Checked;
                if (ScrollingEnabled)
                {
                    Reload();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                lock (ContentLines)
                {
                    ContentLines.Sort();
                }
                SortNeeded = false;
                Reload(true);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void retrieveTopicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner.isChannel)
                {
                    owner._Network._Protocol.Transfer("TOPIC " + owner.WindowName);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void toggleSimpleLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (CreatingMenu)
                {
                    return;
                }
                Switch(false);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void toggleAdvancedLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (CreatingMenu)
                {
                    return;
                }
                Switch(true);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void mode1b2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner != null)
                {
                    owner.textbox.richTextBox1.Buffer.Text += mode1b2ToolStripMenuItem.Text;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void mode1q2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner != null)
                {
                    owner.textbox.richTextBox1.Buffer.Text += mode1q2ToolStripMenuItem.Text;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void mode1I2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner != null)
                {
                    owner.textbox.richTextBox1.Buffer.Text += mode1I2ToolStripMenuItem.Text;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void mode1e2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner != null)
                {
                    owner.textbox.richTextBox1.Buffer.Text += mode1e2ToolStripMenuItem.Text;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void openLinkInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Link.StartsWith("http"))
                {
                    Hyperlink.OpenLink(Link);
                }
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
                if (Link.StartsWith("#"))
                {
                    Parser.parse("/join " + Link);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void whoisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner != null)
                {
                    Parser.parse(whoisToolStripMenuItem.Text);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void whowasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner != null)
                {
                    Parser.parse(whowasToolStripMenuItem.Text);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void kickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner != null)
                {
                    if (Configuration.irc.ConfirmAll)
                    {
                        //if (MessageBox.Show(messages.get("window-confirm", Core.SelectedLanguage, new List<string> { "\n\n" + kickToolStripMenuItem.Text }), "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    Parser.parse(kickToolStripMenuItem.Text);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void copyTextToClipBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string text = null;
                if (simple)
                {
                    text = simpleview.Buffer.Text;
                    Clipboard.SetText(text);
                    return;
                }
                text = RT.Text;
                if (text != null)
                {
                    Clipboard.SetText(RT.Text);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void copyEntireWindowToClipBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(Text);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void listAllChannelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (owner != null && owner._Network != null)
                {
                    owner._Network.DisplayChannelWindow();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void copyLinkToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(Link);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
