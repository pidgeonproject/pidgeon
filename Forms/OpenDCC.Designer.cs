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

namespace Client.Forms
{
    public partial class OpenDCC
    {
        private global::Gtk.VBox vbox2;
        private global::Gtk.Label label1;
        private global::Gtk.Table table1;
        private global::Gtk.Label label3;
        private global::Gtk.Label label4;
        private global::Gtk.Label label5;
        private global::Gtk.Label label6;
        private global::Gtk.Label label7;
        private global::Gtk.Label label8;
        private global::Gtk.Button button9;
        private bool Connected = false;

        /// <summary>
        /// Build
        /// </summary>
        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget MainWindow
            this.Icon = global::Gdk.Pixbuf.LoadFromResource("Client.Resources.pigeon_clip_art_hight.ico");
            this.Name = "MainWindow";
            if (ListenerMode)
            {
                this.Title = "Open DCC";
            }
            else
            {
                this.Title = "Incoming DCC";
            }
            this.TypeHint = ((global::Gdk.WindowTypeHint)(5));
            this.WindowPosition = Gtk.WindowPosition.Center;
            // Container child MainWindow.Gtk.Container+ContainerChild
            this.vbox2 = new global::Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.label1 = new global::Gtk.Label();
            this.label1.Name = "label1";
            if (ListenerMode)
            {
                this.label1.LabelProp = "You want to initate a DCC connection to " + username + ". This protocol will reveal your public IP address. Are you sure you want to process this request?";
            }
            else
            {
                this.label1.LabelProp = "User " + username + " want to connect with you using DCC protocol. This protocol will reveal your public IP address. Are you sure you want to process this request?";
            }
            this.vbox2.Add(this.label1);
            global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.label1]));
            w1.Position = 0;
            w1.Expand = false;
            w1.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.table1 = new global::Gtk.Table(((uint)(3)), ((uint)(2)), false);
            this.table1.Name = "table1";
            this.table1.RowSpacing = ((uint)(6));
            this.table1.ColumnSpacing = ((uint)(6));
            // Container child table1.Gtk.Table+TableChild
            this.label3 = new global::Gtk.Label();
            this.label3.Name = "label3";
            this.label3.LabelProp = "Type:";
            this.table1.Add(this.label3);
            global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1[this.label3]));
            w2.XOptions = ((global::Gtk.AttachOptions)(4));
            w2.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label4 = new global::Gtk.Label();
            this.label4.Name = "label4";
            switch (type)
            {
                case ProtocolDCC.DCC.Chat:
                    this.label4.LabelProp = "Chat";
                    break;
                case ProtocolDCC.DCC.File:
                    this.label4.LabelProp = "File";
                    break;
                case ProtocolDCC.DCC.SecureChat:
                    this.label4.LabelProp = "Secure chat";
                    break;
                case ProtocolDCC.DCC.SecureFile:
                    this.label4.LabelProp = "Secure file";
                    break;
            }
            
            this.table1.Add(this.label4);
            global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1[this.label4]));
            w3.LeftAttach = ((uint)(1));
            w3.RightAttach = ((uint)(2));
            w3.XOptions = ((global::Gtk.AttachOptions)(4));
            w3.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label5 = new global::Gtk.Label();
            this.label5.Name = "label5";
            this.label5.LabelProp = "Port";
            this.table1.Add(this.label5);
            global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1[this.label5]));
            w4.TopAttach = ((uint)(1));
            w4.BottomAttach = ((uint)(2));
            w4.XOptions = ((global::Gtk.AttachOptions)(4));
            w4.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label6 = new global::Gtk.Label();
            this.label6.Name = "label6";
            this.label6.LabelProp = Port.ToString();
            this.table1.Add(this.label6);
            global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1[this.label6]));
            w5.TopAttach = ((uint)(1));
            w5.BottomAttach = ((uint)(2));
            w5.LeftAttach = ((uint)(1));
            w5.RightAttach = ((uint)(2));
            w5.XOptions = ((global::Gtk.AttachOptions)(4));
            w5.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label7 = new global::Gtk.Label();
            this.label7.Name = "label7";
            this.label7.LabelProp = "IP:";
            this.table1.Add(this.label7);
            global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1[this.label7]));
            w6.TopAttach = ((uint)(2));
            w6.BottomAttach = ((uint)(3));
            w6.XOptions = ((global::Gtk.AttachOptions)(4));
            w6.YOptions = ((global::Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.label8 = new global::Gtk.Label();
            this.label8.Name = "label8";
            if (!ListenerMode)
            {
                this.label8.LabelProp = Server;
            }
            else
            {
                this.label8.LabelProp = Core.GetIP();
            }
            this.table1.Add(this.label8);
            global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1[this.label8]));
            w7.TopAttach = ((uint)(2));
            w7.BottomAttach = ((uint)(3));
            w7.LeftAttach = ((uint)(1));
            w7.RightAttach = ((uint)(2));
            w7.XOptions = ((global::Gtk.AttachOptions)(4));
            w7.YOptions = ((global::Gtk.AttachOptions)(4));
            this.vbox2.Add(this.table1);
            global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.table1]));
            w8.Position = 1;
            w8.Expand = false;
            w8.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.button9 = new global::Gtk.Button();
            this.button9.CanFocus = true;
            this.button9.Name = "button9";
            this.button9.UseUnderline = true;
            this.button9.Label = "Connect";
            this.vbox2.Add(this.button9);
            global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.button9]));
            w9.Position = 2;
            w9.Expand = false;
            w9.Fill = false;
            this.Add(this.vbox2);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 296;
            this.Show();
            this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
        }

        private void OnDeleteEvent(Object o, Gtk.DeleteEventArgs e)
        {
            if (!Connected && ListenerMode)
            {
                lock (Core.LockedPorts)
                {
                    if (Core.LockedPorts.Contains(Port))
                    {
                        Core.LockedPorts.Remove(Port);
                    }
                }
            }
        }
    }
}
