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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Gtk;

namespace Client.Graphics
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class TextBox : Gtk.Bin
	{
		public List<string> history = null;
        public int position = 0;
        public string prevtext = "";
        public string original = "";
        public Window parent = null;
        public bool restore = false;

		public Gtk.TextView richTextBox1
		{
			get
			{
				return richTextBox;
			}
		}
		
		public TextBox ()
		{
			this.Build ();
		}

		/// <summary>
		/// Deprecated
		/// </summary>
		[Obsolete]
		public void Init()
		{

		}

		public void setFocus()
		{
			this.GrabFocus ();
		}
	}
}

