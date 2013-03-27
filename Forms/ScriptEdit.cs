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

namespace Client.Forms
{
	public partial class ScriptEdit : Gtk.Window
	{
		public Network network = null;
		
		public TextView textBox1
		{
			get
			{
				return textview1;
			}
		}

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide ();
			this.Destroy ();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
				string[] values = textview1.Buffer.Text.Split ('\n');
                foreach (string text in values)
                {
                    if (text != "")
                    {
                        if (text.StartsWith("#"))
                        {
                            continue;
                        }
                        if (text.StartsWith(Configuration.CommandPrefix))
                        {
                            Core.ProcessCommand(text);
                            continue;
                        }
                        network.Transfer(text, Configuration.Priority.High);
                    }
                }
                Hide();
				this.Destroy();
            }
            catch(Exception fail)
            {
                Core.handleException(fail);
            }
        }
		
		public ScriptEdit () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			messages.Localize(this);
		}
	}
}

