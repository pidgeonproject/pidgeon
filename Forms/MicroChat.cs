using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Gtk;

namespace Client.Forms
{
	public partial class MicroChat : Gtk.Window
	{
		public static MicroChat mc = null;
		public Scrollback scrollback_mc = null;
		
		public MicroChat () : base(Gtk.WindowType.Toplevel)
		{
			//this.Toplevel = true;
			scrollback_mc = new Scrollback();
            scrollback_mc.owner = null;
            scrollback_mc.Create();
            //scrollback_mc.contextMenuStrip1.Items.Clear();
            //scrollback_mc.contextMenuStrip1.Items.Add(scrollback_mc.scrollToolStripMenuItem);
            //scrollback_mc.contextMenuStrip1.Items.Add(transparencyToolStripMenuItem);
		}
		
		
	}
}

