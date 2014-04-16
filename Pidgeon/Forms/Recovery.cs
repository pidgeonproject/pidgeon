using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using Gtk;

namespace Client.Forms
{
    public partial class Recovery : Gtk.Window
    {
        public Recovery () : base(Gtk.WindowType.Toplevel)
        {
            this.Build ();
        }
    }
}

