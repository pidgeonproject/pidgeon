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
    /// <summary>
    /// List of networks
    /// </summary>
    public partial class NetworkDB
    {
        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Forms.NetworkDB
            this.Name = "Client.Forms.NetworkDB";
            this.Title = "NetworkDB";
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 456;
            this.DefaultHeight = 336;
            this.Show();
        }
    }
}
