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
		private string StatusBox = "";
        public Graphics.Window main = null;
        public Graphics.PidgeonList ChannelList = null;
        public Graphics.Window Chat = null;
        private Connection fConnection;
        private Preferences fPrefs;
        private bool UpdatedStatus = true;
        SearchItem searchbox = new SearchItem();
        bool done = false;
        public int progress = 0;
        public bool DisplayingProgress = false;
        public int ProgressMax = 0;

        public class _WindowRequest
        {
            public Window window;
            public string name;
            public bool focus;
            public bool writable;
            public Protocol owner;
        }

		
		public Main () : base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

