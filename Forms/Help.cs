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
    /// <summary>
    /// Help form
    /// </summary>
    public partial class Help : GTK.PidgeonForm
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public Help ()
        {
            try
            {
                this.Build ();
                messages.Localize (this);
                this.label1.Markup = "<span size='18000'>Pidgeon</span>";
                this.label4.Text = Configuration.Version + " build: " + RevisionProvider.GetHash();
            } catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Link(object sender, Gtk.ButtonPressEventArgs e)
        {
            if (e.Event.Button == 1)
            {
                Hyperlink.OpenLink("http://pidgeonclient.org/wiki/");
            }
        }
    }
}

