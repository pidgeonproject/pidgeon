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
using System.Linq;
using System.Text;

namespace Client.GTK
{
    /// <summary>
    /// Menu
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// Enabled
        /// </summary>
        public bool Enabled = false;
        /// <summary>
        /// Checked
        /// </summary>
        public bool Checked = false;
        /// <summary>
        /// Visible
        /// </summary>
        public bool Visible = false;
        /// <summary>
        /// Text
        /// </summary>
        public string Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client.GTK.Menu"/> class.
        /// </summary>
        public Menu()
        { 
            Text = null;
        }

        /// <summary>
        /// Creates menu
        /// </summary>
        /// <param name="id"></param>
        public Menu(string id)
        {
            Text = id;
        }
    }

    /// <summary>
    /// Form
    /// </summary>
    public class PidgeonForm : Gtk.Window
    {
        /// <summary>
        /// Configuration
        /// </summary>
        public class Info
        {
            /// <summary>
            /// X 
            /// </summary>
            public int X = 0;
            /// <summary>
            /// Y
            /// </summary>
            public int Y = 0;
            /// <summary>
            /// Height
            /// </summary>
            public int Height = 0;
            /// <summary>
            /// Width
            /// </summary>
            public int Width = 0;
        }

        /// <summary>
        /// Information about windows
        /// </summary>
        public static Dictionary<string, Info> WindowInfo = new Dictionary<string, Info>();

        /// <summary>
        /// Timer that handles the resize events
        /// </summary>
        public GLib.TimeoutHandler ChangeTimer = null;

        /// <summary>
        /// ID of form
        /// </summary>
        public string ID = null;

        /// <summary>
        /// Height
        /// </summary>
        public int Height
        {
            get
            {
                int height;
                int width;
                this.GetSize(out width, out height);
                return height;
            }
            set
            {
                this.SetSizeRequest(Width, value);
            }
        }

        /// <summary>
        /// Width
        /// </summary>
        public int Width
        {
            get
            {
                int height;
                int width;
                this.GetSize(out width, out height);
                return width;
            }
            set
            {
                this.SetSizeRequest(value, Height);
            }
        }   
        
        /// <summary>
        /// Position X
        /// </summary>
        public int Left
        {
            get
            {
                int x;
                int y;
                this.GetPosition(out x, out y);
                return x;
            }
        }
        
        /// <summary>
        /// Position
        /// </summary>
        public int Top
        {
            get
            {
                int x;
                int y;
                this.GetPosition(out x, out y);
                return y;
            }
        }
        
        /// <summary>
        /// Enabled
        /// </summary>
        public bool Enabled
        {
            set
            {
                this.Sensitive = value;
            }
            get
            {
                return this.Sensitive;
            }
        }

        /// <summary>
        /// Creates a new window
        /// </summary>
        public PidgeonForm() : base(Gtk.WindowType.Toplevel)
        {
            
        }

        /// <summary>
        /// Save the current location of window
        /// </summary>
        ~PidgeonForm()
        {
            OnResize(null, null);
        }

        /// <summary>
        /// Event to happen on resize
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        public virtual void OnResize(object o, EventArgs args)
        {
            try
            {
                if (Configuration.Window.RememberPosition)
                {
                    if (ID == null)
                    {
                        return;
                    }
                    if (!WindowInfo.ContainsKey(ID))
                    {
                        Core.DebugLog("Invalid key for window: " + ID);
                        return;
                    }
                    Info info = WindowInfo[ID];
                    info.X = this.Left;
                    info.Y = this.Top;
                    info.Width = this.Width;
                    info.Height = this.Height;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private bool ResizeHandler()
        {
            OnResize(null, null);
            return true;
        }

        /// <summary>
        /// Load conf
        /// </summary>
        /// <param name="id"></param>
        public void LC(string id)
        {
            ID = id;
            if (Configuration.Window.RememberPosition)
            {
                lock (WindowInfo)
                {
                    if (!WindowInfo.ContainsKey(id))
                    {
                        Info info = new Info();
                        WindowInfo.Add(id, info);
                        info.X = this.Left;
                        info.Y = this.Top;
                        info.Width = this.Width;
                        info.Height = this.Height;
                    }
                    else
                    {
                        this.Move(WindowInfo[id].X, WindowInfo[id].Y);
                        this.Resize(WindowInfo[id].Width, WindowInfo[id].Height);
                    }
                }
                // **FIXME**
                this.ChangeTimer = new GLib.TimeoutHandler(ResizeHandler);
                this.ConfigureEvent += new Gtk.ConfigureEventHandler(OnResize);
                GLib.Timeout.Add(8000, this.ChangeTimer);
            }
        }

        /// <summary>
        /// Creates a new window
        /// </summary>
        public PidgeonForm(string id) : base(Gtk.WindowType.Toplevel)
        {
            LC(id);
        }
    }
}
