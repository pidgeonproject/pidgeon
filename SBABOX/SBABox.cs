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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Client
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class SBABox : Gtk.Bin
    {
        public class ContentText
        {
            /// <summary>
            /// Text of this part
            /// </summary>
            public string Text
            {
                get
                {
                    return text;
                }
            }
            private string text = null;
            /// <summary>
            /// Box which own this
            /// </summary>
            private SBABox Owner = null;
            public Color TextColor = Color.White;
            public Color Backcolor = Color.Black;
            public bool Linked = false;
            public bool Bold = false;
            public bool Italic = false;
            public bool Underline = false;
            public string Link = null;
            public bool menu = false;

            /// <summary>
            /// Creates a new content
            /// </summary>
            /// <param name="text">Text of this part</param>
            /// <param name="SBAB">Owner</param>
            /// <param name="color">Color of a text</param>
            public ContentText(string _text, SBABox SBAB, Color color)
            {
                text = _text;
                Owner = SBAB;
                TextColor = color;
            }

            ~ContentText()
            {
                Owner = null;
            }
        }

        /// <summary>
        /// Content of box in unformatted way
        /// </summary>
        private string text = null;
        private int MinimalWidth = 220;
        private bool initializationComplete = false;
        public bool WrapAll = true;
        private int RecursiveMax = 0;
        private bool isDisposing = false;
        private BufferedGraphicsContext backbufferContext = null;
        private BufferedGraphics backbufferGraphics = null;
        /// <summary>
        /// If this is false, this control will not reload itself on change of content
        /// </summary>
        public bool ReloadingEnabled = true;
        /// <summary>
        /// Current position of carret
        /// </summary>
        private int currentY = 0;
        /// <summary>
        /// Current position of carret
        /// </summary>
        private int currentX = 0;
        /// <summary>
        /// Current position of scroll bar
        /// </summary>
        private int scrollValue = 0;
        /// <summary>
        /// Defines whether long lines should be trimmed and continued on next line
        /// </summary>
        public bool Wrap = true;
        /// <summary>
        /// Number of lines that were rendered
        /// </summary>
        private int RenderedLineTotalCount = 0;
        private System.Drawing.Graphics drawingGraphics = null;
        public Scrollback scrollback = null;
        private List<Line> LineDB = new List<Line>();
        private bool Rendering = false;
        private Font GlobalFont = null;
        /// <summary>
        /// Information about all links
        /// </summary>
        private List<Link> LinkInfo = new List<Link>();
        private int spacing = 2;
		private Color foreColor = Color.Black;
		private Color backColor = Color.White;
		private global::Gtk.VBox vbox1;
		private global::Gtk.HBox hbox1;
		private global::Gtk.DrawingArea pt;
		private global::Gtk.VScrollbar vScrollBar1;
		private global::Gtk.HScrollbar hsBar;
		
		public int Height
		{
			get
			{
				return this.SizeRequest ().Height;
			}
			set
			{
				this.SetSizeRequest (Width, value);
			}
		}
		
		public int Width
		{
			get
			{
				return this.SizeRequest ().Width;
			}
			set
			{
				this.SetSizeRequest (value, Height);
			}
		}	

        /// <summary>
        /// Spacing
        /// </summary>
        public int Spacing
        {
            get
            {
                return spacing;
            }
            set
            {
                spacing = value;
                RedrawText();
            }
        }

        

        /// <summary>
        /// Color of text
        /// </summary>
        public Color ForeColor
        {
            get
            {
                return foreColor;
            }
            set
            {
                foreColor = value;
                Redraw();
            }
        }

        public Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
				backColor = value;
                //pt.BackColor = value;
                Redraw();
            }
        }

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Client.SBABoxWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Client.SBABoxWidget";
			// Container child Client.SBABoxWidget.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.pt = new global::Gtk.DrawingArea ();
			this.pt.Name = "pt";
			this.hbox1.Add (this.pt);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.pt]));
			w1.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vScrollBar1 = new global::Gtk.VScrollbar (null);
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Adjustment.Upper = 100D;
			this.vScrollBar1.Adjustment.PageIncrement = 10D;
			this.vScrollBar1.Adjustment.PageSize = 10D;
			this.vScrollBar1.Adjustment.StepIncrement = 1D;
			this.hbox1.Add (this.vScrollBar1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vScrollBar1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.vbox1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox1]));
			w3.Position = 0;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hsBar = new global::Gtk.HScrollbar (null);
			this.hsBar.Name = "hsBar";
			this.hsBar.Adjustment.Upper = 100D;
			this.hsBar.Adjustment.PageIncrement = 10D;
			this.hsBar.Adjustment.PageSize = 10D;
			this.hsBar.Adjustment.StepIncrement = 1D;
			this.vbox1.Add (this.hsBar);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hsBar]));
			w4.Position = 1;
			w4.Expand = false;
			w4.Fill = false;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
		}

        /// <summary>
        /// Return a text with no formatting, this can be used in case you want to copy content of this box
        /// </summary>
        public string Text
        {
            get
            {
                lock (LineDB)
                {
                    text = "";
                    text = Hooks._Scrollback.BeforeParser(text);
                    foreach (Line _l in LineDB)
                    {
                        string line = _l.ToString();
                        if (line != null)
                        {
                            text = Hooks._Scrollback.BeforeInsertLine(text, line + "\n");
                            text += line + "\n";
                        }
                    }
                }
                return text;
            }
        }

        private void Redraw()
        {
            if (!isDisposing)
            {
                //Lock();
                RedrawText();
                //pt.Refresh();
            }
        }

        public bool SearchDown(System.Text.RegularExpressions.Regex find)
        {
            if (!find.IsMatch(Text))
            {
                return false;
            }
            int line = 0;

            return false;
        }

        public bool SearchDown(string find)
        {
            if (!Text.Contains(find))
            {
                return false;
            }
            int line = 0;

            return false;
        }

        public bool SearchUp(System.Text.RegularExpressions.Regex find)
        {
            if (!find.IsMatch(Text))
            {
                return false;
            }
            int line = 0;

            return false;
        }

        public bool SearchUp(string find)
        {
            if (!Text.Contains(find))
            {
                return false;
            }
            int line = 0;

            return false;
        }

        /// <summary>
        /// Reload the scroll bars and their values
        /// </summary>
        private void UpdateBars()
        {
            scrollValue = RenderedLineTotalCount - (int)((float)pt.SizeRequest ().Height / (_Font.Size + spacing));

            if (scrollValue < 0)
            {
                vScrollBar1.Sensitive = false;
            }

            if (scrollValue > 0)
            {
                lock (vScrollBar1)
                {
                    try
                    {
                        if (vScrollBar1.Value > scrollValue)
                        {
                            vScrollBar1.Value = scrollValue;
                        }
                        vScrollBar1.Adjustment.Upper = scrollValue;
                        vScrollBar1.Sensitive = true;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (vScrollBar1.Adjustment.Upper > 0)
                        {
                            vScrollBar1.Value = vScrollBar1.Adjustment.Upper - 1;
                        }
                        else
                        {
                            vScrollBar1.Value = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This will remove all link data
        /// </summary>
        /// <returns></returns>
        public bool ClearLink()
        {
            lock (LinkInfo)
            {
                foreach (Link il in LinkInfo)
                {
                    il.LinkedText.Linked = false;
                    il.Dispose();
                }
                LinkInfo.Clear();
            }
            return true;
        }

        public SBABox()
        {
			Build ();
        }

        protected void Dispose(bool disposing)
        {
            try
            {
                lock (this)
                {
                    isDisposing = true;
                    if (disposing)
                    {
                        // We must dispose of backbufferGraphics before we dispose of backbufferContext or we will get an exception.
                        if (backbufferGraphics != null)
                        {
                            backbufferGraphics.Dispose();
                        }
                        if (backbufferContext != null)
                        {
                            try
                            {
                                backbufferContext.Dispose();
                            }
                            catch (Exception)
                            {
                                backbufferContext = null;
                            }
                        }
                    }
                }
                base.Dispose();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void RecreateBuffers()
        {
            try
            {
                // Check initialization has completed so we know backbufferContext has been assigned.
                // Check that we aren't disposing or this could be invalid.
                if (!initializationComplete || isDisposing)
                    return;

                // We recreate the buffer with a width and height of the control. The "+ 1"
                // guarantees we never have a buffer with a width or height of 0.
                backbufferContext.MaximumBuffer = new Size(pt.SizeRequest ().Width + 1, pt.SizeRequest ().Height + 1);

                // Dispose of old backbufferGraphics (if one has been created already)

                if (backbufferGraphics != null)
                {
                    lock (backbufferGraphics)
                    {
                        backbufferGraphics.Dispose();
                    }
                }

                // Create new backbufferGrpahics that matches the current size of buffer.
                //backbufferGraphics = backbufferContext.Allocate(pt.CreateGraphics(),
                new Rectangle(0, 0, Math.Max(pt.SizeRequest ().Width, 1), Math.Max(pt.SizeRequest ().Height, 1));

                // Assign the Graphics object on backbufferGraphics to "drawingGraphics" for easy reference elsewhere.
                drawingGraphics = backbufferGraphics.Graphics;

                // This is a good place to assign drawingGraphics.SmoothingMode if you want a better anti-aliasing technique.

                // Invalidate the control so a repaint gets called somewhere down the line.
                //pt.Invalidate();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Remove all text of this item
        /// </summary>
        public void RemoveText(bool Redraw = true)
        {
            currentY = 0;
            lock (LineDB)
            {
                LineDB.Clear();
            }
            if (Redraw)
            {
                RedrawText();
            }
        }

        private void Lock()
        {
            while (Rendering)
            {
                System.Threading.Thread.Sleep(10);
            }
        }

        public void ScrollToBottom()
        {
            lock (vScrollBar1)
            {
                if (vScrollBar1.Adjustment.Upper > 1)
                {
                    double previous = 0;
					while (previous < vScrollBar1.Adjustment.Upper)
                    {
						previous = vScrollBar1.Adjustment.Upper;
						ScrolltoX(vScrollBar1.Adjustment.Upper);
						vScrollBar1.Value = vScrollBar1.Adjustment.Upper;
                        Redraw();
                    }
                }
            }
        }

		private void ScrolltoX(double x)
		{
			currentY = ((int)_Font.Size + spacing) * Convert.ToInt32 (x);
		}

        private void ScrolltoX(int x)
        {
			currentY = ((int)_Font.Size + spacing) * Convert.ToInt32 (x);
        }

        /// <summary>
        /// Creates an instance of ContentText but won't insert it to this SBABox
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public ContentText CreateText(string text, Color color)
        {
            return new ContentText(text, this, color);
        }

        private void SBABox_Load(object sender, EventArgs e)
        {
            try
            {
                backbufferContext = BufferedGraphicsManager.Current;
                initializationComplete = true;
                vScrollBar1.Adjustment.Lower = 0;
                vScrollBar1.Value = 0;
                vScrollBar1.Adjustment.Upper = 0;
                vScrollBar1.Sensitive = false;
                if (Wrap)
                {
                    hsBar.Visible = false;
                }

                RecreateBuffers();

                //this.SetStyle(
                //ControlStyles.UserPaint |
                //ControlStyles.AllPaintingInWmPaint |
                //ControlStyles.DoubleBuffer, true);

                Redraw();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
