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

using Pidgeon.Protocols;
using System;
using Gtk;

namespace Pidgeon.Forms
{
    /// <summary>
    /// Creates a new form
    /// </summary>
    public partial class Connection : PidgeonGtkToolkit.PidgeonForm
    {
        /// <summary>
        /// New connection gtk dialog
        /// </summary>
        public Connection()
        {
            this.Build ();
            this.LC("ConnectionWindow");
            messages.Localize(this);
            this.entry4.Visibility = false;
            this.DeleteEvent += new DeleteEventHandler(Unshow);
            entry3.Text = Definitions.StandardIrcPort.ToString();
            entry2.Text = Configuration.UserData.ident;
            entry1.Text = Configuration.UserData.nick;
            combobox1.Clear();
            CellRendererText cell = new CellRendererText();
            combobox1.PackStart(cell, false);
            combobox1.AddAttribute(cell, "text", 0);
            ListStore store = new ListStore(typeof(string));
            combobox1.Model = store;
            ListStore store2 = new ListStore(typeof(string));
            TreeIter iter = store.AppendValues("irc");
            store.AppendValues("pidgeon services");

            if (Configuration.Kernel.Debugging)
            {
                store.AppendValues("quassel");
                store.AppendValues("dcc");
            }
            
            comboboxentry1.Model = store2;
            combobox1.SetActiveIter(iter);
            button1.Clicked += new EventHandler(bConnect_Click);
            checkbutton1.Active = Configuration.UserData.LastSSL;
            if (!string.IsNullOrEmpty(Configuration.UserData.LastPort))
            {
                entry3.Text = Configuration.UserData.LastPort;
            }
            if (!string.IsNullOrEmpty(Configuration.UserData.LastNick))
            {
                entry1.Text = Configuration.UserData.LastNick;
            }
            if (!string.IsNullOrEmpty(Configuration.UserData.LastHost))
            {
                TreeIter iter2 = store2.AppendValues(Configuration.UserData.LastHost);
                this.comboboxentry1.SetActiveIter(iter2);
            }
            lock (Configuration.UserData.History)
            {
                foreach (string nw in Configuration.UserData.History)
                {
                    store2.AppendValues(nw);
                }
                if (!Configuration.UserData.History.Contains(comboboxentry1.ActiveText.ToLower()))
                {
                    Configuration.UserData.History.Add(comboboxentry1.ActiveText);
                }
            }
        }

        private void Unshow(object main, Gtk.DeleteEventArgs closing)
        {
            try
            {
                this.Hide();
                closing.RetVal = true;
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }
  
        private void Change(object sender, EventArgs x)
        {
            int port;
            if (!int.TryParse(this.entry3.Text, out port))
            {
                port = 6667;
            }
            bool st = false;
            switch (port)
            {
                case Definitions.StandardIrcPort:
                case Definitions.StandardIrcSSLPort:
                case Definitions.StandardServicesPort:
                case Definitions.StandardServicesSSLPort:
                    st = true;
                    break;
            }
            if (!st)
            {
                return;
            }
            switch (combobox1.ActiveText)
            {
                case "irc":
                    if (!this.checkbutton1.Active)
                    {
                        this.entry3.Text = Definitions.StandardIrcPort.ToString();
                    } else
                    {
                        this.entry3.Text = Definitions.StandardIrcSSLPort.ToString();
                    }
                    break;
                case "pidgeon services":
                    if (!this.checkbutton1.Active)
                    {
                        this.entry3.Text = Definitions.StandardServicesPort.ToString();
                    } else
                    {
                        this.entry3.Text = Definitions.StandardServicesSSLPort.ToString();
                    }
                    break;
            }
        }
        
        private void bConnect_Click(object sender, EventArgs e)
        {
            try
            {
                int port = 6667;
                Configuration.UserData.LastSSL = checkbutton1.Active;
                if (string.IsNullOrEmpty(entry2.Text))
                {
                    PidgeonGtkToolkit.MessageBox.Show(this, MessageType.Warning, ButtonsType.Ok, messages.get("newconnection-2", Core.SelectedLanguage), "Missing params");
                    return;
                }
                if (string.IsNullOrEmpty(entry3.Text) || !int.TryParse(entry3.Text, out port))
                {
                    PidgeonGtkToolkit.MessageBox.Show(this, MessageType.Warning, ButtonsType.Ok, messages.get("newconnection-3", Core.SelectedLanguage), "Missing params");
                    return;
                }
                if (string.IsNullOrEmpty(entry1.Text))
                {
                    PidgeonGtkToolkit.MessageBox.Show(this, MessageType.Warning, ButtonsType.Ok, messages.get("newconnection-1", Core.SelectedLanguage), "Missing params");
                    return;
                }
                if (Uri.CheckHostName(comboboxentry1.ActiveText) == UriHostNameType.Unknown)
                {
                    PidgeonGtkToolkit.MessageBox.Show(this, MessageType.Warning, ButtonsType.Ok, messages.get("newconnection-4", Core.SelectedLanguage), "Missing params");
                    return;
                }
                Configuration.UserData.nick = entry1.Text;
                Configuration.UserData.ident = entry2.Text;
                Configuration.UserData.LastHost = comboboxentry1.ActiveText;
                lock (Configuration.UserData.History)
                {
                    if (!Configuration.UserData.History.Contains(comboboxentry1.ActiveText.ToLower()))
                    {
                        Configuration.UserData.History.Add(comboboxentry1.ActiveText);
                    }
                    ((ListStore)comboboxentry1.Model).AppendValues(comboboxentry1.ActiveText);
                }
                Configuration.UserData.LastPort = entry3.Text;
                Configuration.UserData.LastNick = entry1.Text;
                switch (combobox1.ActiveText)
                {
                    case "irc":
                        Core.ConnectIRC(comboboxentry1.ActiveText, port, entry4.Text, checkbutton1.Active);
                        break;
                    case "quassel":
                        Core.ConnectQl(comboboxentry1.ActiveText, port, entry4.Text, checkbutton1.Active);
                        break;
                    case "pidgeon services":
                        Core.ConnectPS(comboboxentry1.ActiveText, port, entry4.Text, checkbutton1.Active);
                        break;
                    case "dcc":
                        Core.ConnectDcc(comboboxentry1.ActiveText, port, entry4.Text, ProtocolDCC.DCC.Chat, false, comboboxentry1.ActiveText, checkbutton1.Active);
                        break;
                }
                Hide();
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }
    }
}

