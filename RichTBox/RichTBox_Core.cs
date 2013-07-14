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
using System.ComponentModel;
using System.Data;
using System.Text;
using Gtk;

namespace Client
{
    /// <summary>
    /// Rich text box
    /// </summary>
    public partial class RichTBox : Gtk.Bin
    {
        /// <summary>
        /// Pointer
        /// </summary>
        public Scrollback scrollback = null;
        private System.Drawing.Font font;
        private string text = null;
        private System.Drawing.Color foreColor;
        private System.Drawing.Color backColor;

        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TextView richTextBox;

        /// <summary>
        /// Pointer
        /// </summary>
        public Gtk.TextView textView
        {
            get
            {
                return richTextBox;
            }
        }

        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
        }

        /// <summary>
        /// Background
        /// </summary>
        public System.Drawing.Color BackColor
        {
            set
            {
                backColor = value;
                colors();
            }
            get
            {
                return backColor;
            }
        }

        /// <summary>
        /// Foreground
        /// </summary>
        public System.Drawing.Color ForeColor
        {
            set
            {
                foreColor = value;
                colors();
            }
            get
            {
                return foreColor;
            }
        }

        private void colors()
        {
            richTextBox.ModifyBase(StateType.Normal, Core.FromColor(BackColor));
            richTextBox.ModifyText(StateType.Normal, Core.FromColor(ForeColor));
        }

        private void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Graphics.TextBox
            global::Stetic.BinContainer.Attach(this);
            this.Name = "Client.Graphics.TextBox";
            // Container child Client.Graphics.TextBox.Gtk.Container+ContainerChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.richTextBox = new global::Gtk.TextView();
            this.richTextBox.CanFocus = true;
            this.richTextBox.DoubleBuffered = true;
            this.richTextBox.Editable = false;
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.AcceptsTab = true;
            this.richTextBox.SizeAllocated += new SizeAllocatedHandler(Scroll2);
            richTextBox.WrapMode = WrapMode.Word;
            this.GtkScrolledWindow.Add(this.richTextBox);
            this.Add(this.GtkScrolledWindow);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Hide();
        }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        public RichTBox()
        {
            this.Build();
            ForeColor = Configuration.CurrentSkin.colordefault;
            BackColor = Configuration.CurrentSkin.backgroundcolor;
            font = new System.Drawing.Font(Configuration.CurrentSkin.localfont, Configuration.CurrentSkin.fontsize);
            DefaultFont.Family = Configuration.CurrentSkin.localfont;
            //DefaultFont.Size = Configuration.CurrentSkin.fontsize;
        }

        /// <summary>
        /// Insert a line to top of box
        /// </summary>
        /// <param name="line"></param>
        /// <param name="drawLine"></param>
        public void InsertLineToStart(Line line, bool drawLine = true)
        {
            if (line == null)
            {
                throw new Exception("You can't insert null to text box");
            }
            lock (Lines)
            {
                Lines.Insert(0, line);
            }
            if (drawLine)
            {
                DrawLineToHead(line);
            }
        }

        /// <summary>
        /// Insert a line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="drawLine"></param>
        public void InsertLine(Line line, bool drawLine = true)
        {
            if (line == null)
            {
                throw new Exception("You can't insert null to text box");
            }
            lock (Lines)
            {
                Lines.Add(line);
            }
            if (drawLine)
            {
                DrawLine(line);
            }
        }

        /// <summary>
        /// Insert a part
        /// </summary>
        /// <param name="line"></param>
        /// <param name="drawLine"></param>
        public void InsertPart(Line line, bool drawLine = true)
        {
            if (Core._KernelThread != System.Threading.Thread.CurrentThread)
            {
                throw new Exception("You can't call this function from different thread");
            }

            if (line == null)
            {
                throw new Exception("You can't insert null to text box");
            }

            if (CurrentLine != null)
            {
                DrawNewline();
            }

            CurrentLine = line;
            if (drawLine)
            {
                DrawPart(line);
            }
        }

        /// <summary>
        /// Insert a part
        /// </summary>
        /// <param name="text"></param>
        /// <param name="drawLine"></param>
        public void InsertPart(ContentText text, bool drawLine = true)
        {
            if (Core._KernelThread != System.Threading.Thread.CurrentThread)
            {
                throw new Exception("You can't call this function from different thread");
            }

            if (text == null)
            {
                throw new Exception("You can't insert null to text box");
            }

            if (CurrentLine == null)
            {
                CurrentLine = new Line(this);
            }

            CurrentLine.insertData(text);

            if (drawLine)
            {
                DrawPart(text);
            }
        }

        /// <summary>
        /// Reload text
        /// </summary>
        public void RedrawText()
        {
            Redraw();
        }

        private void Scroll2(object sender, SizeAllocatedArgs e)
        {
            if (Core._KernelThread != System.Threading.Thread.CurrentThread)
            {
                throw new Exception("You can't call this function from different thread");
            }
            if (scrollback == null || scrollback.IsDestroyed)
            {
                Core.DebugLog("Called ScrollToBottom on NULL scrollback");
                return;
            }
            if (scrollback.ScrollingEnabled)
            {
                richTextBox.ScrollToIter(richTextBox.Buffer.EndIter, 0, false, 0, 0);
            }
        }
    }
}
