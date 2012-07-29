using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class SBABox : UserControl
    {
        private string text = "";
        public bool BufferOn = false;
        private bool initializationComplete;
        private bool isDisposing;
        private BufferedGraphicsContext backbufferContext;
        private BufferedGraphics backbufferGraphics;
        private int offset = 0;
        private Graphics drawingGraphics;
        public Scrollback scrollback;

        public class ContentText
        {
            public string text;
            private SBABox owner;
            public Color textc;
            public Color backc;
            public string link;
            public bool menu = false;
            public ContentText(string Text, SBABox SBAB, Color color)
            {
                text = Text;
                owner = SBAB;
                textc = color;
            }
        }

        public List<Link> link = new List<Link>();

        public class Link
        {
            private int X;
            private int Y;
            private int Width;
            private int Height;
            private SBABox parent;
            Label area;
            public Link(int x, int y, int width, int height, SBABox SBAB, string http)
            {
                X = x;
                Y = y;
                Width = width;
                area = new Label();
                area.BackColor = Color.Thistle;
                area.Top = X;
                area.Left = Y;
                area.Width = width;
                area.Height = height;
                area.Name = http;
                parent = SBAB;
                parent.Controls.Add(area);
                area.BringToFront();
                area.CreateControl();
                area.MouseDown += new MouseEventHandler(parent.ClickL);
                area.ContextMenu = parent.ContextMenu;
                lock (parent.Controls)
                {
                    parent.Controls.Add(area);
                }
                Height = height;
            }
            public void Dispose()
            {
                lock (parent.Controls)
                {
                    if (parent.Controls.Contains(area))
                    {
                        parent.Controls.Remove(area);
                        area.MouseDown -= new MouseEventHandler(parent.ClickL);
                        area.Dispose();
                    }
                }
            }
        }

        public class Line
        {
            public List<ContentText> text = new List<ContentText>();
            private SBABox owner;

            public void insertData(ContentText line)
            {
                if (line == null)
                {
                    return;
                }
                    text.Add(line);
            }
            public void insertData(string line)
            {
                text.Add(new ContentText(line, owner, owner.ForeColor));
            }
            public override string ToString()
            {
                string part = "";
                foreach (ContentText data in text)
                {
                    part += data;
                }
                return part;
            }

            public void Merge(Line line)
            {
                text.AddRange(line.text);
            }

            public Line(string Text, SBABox SBAB)
            {
                text.Add(new ContentText(Text, SBAB, SBAB.ForeColor));
                owner = SBAB;
            }
            public Line()
            { 
                text = new List<ContentText>();
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                RedrawText();
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                pt.BackColor = value;
                buffer.BackColor = value;
                RedrawText();
            }
        }
        public string Text
        {
            get
            {
                lock (text)
                {
                    text = "";
                    foreach (Line _l in lines)
                    {
                        text += _l.ToString() + "\n";
                    }
                }
                return text;
            }
        }

        private List<Line> lines = new List<Line>();

        private void Redraw()
        {
            RedrawText();
            pt.Refresh();
        }

        protected void RepaintWindow(object sender, PaintEventArgs e)
        {
            if (!isDisposing && backbufferGraphics != null)
                backbufferGraphics.Render(e.Graphics);
        }

        public void RedrawText()
        {
            //
                int X = 0;
                int Y = 0 - offset;
                
                Graphics _t;
                if (drawingGraphics == null)
                {
                    return;
                }
                _t = drawingGraphics;
                ClearLink();


                _t.Clear(BackColor);
                foreach (Line text in lines)
                {
                    X = 0;
                    foreach (ContentText part in text.text)
                    {
                        if ((Y + (int)Font.Size + 8) > 0 && (Y + (int)Font.Size + 8) < this.Height)
                        {
                            SizeF stringSize = _t.MeasureString(part.text, this.Font);
                            if (part.link != null)
                            {
                                Pen pen = new Pen(part.textc);
                                _t.DrawLine(pen, X, Y + (int)stringSize.Height,X + (int)stringSize.Width,  Y + (int)stringSize.Height);
                                Link http = new Link(X,Y, (int) stringSize.Width, (int) stringSize.Height, this, part.link);
                                lock (link)
                                {
                                    link.Add(http);
                                }
                            }
                            Brush _b = new SolidBrush(part.textc);
                            _t.DrawString(part.text, this.Font, _b, X, Y);
                            X = X + (int)stringSize.Width + 2;
                        }
                    }
                    Y = Y + (int)Font.Size + 6;
                }
                vScrollBar1.Maximum = lines.Count;
                if (lines.Count > 0)
                {
                    vScrollBar1.Enabled = true;
                }
                pt.Invalidate();
        }

        public void ClickR(Object sender, MouseEventArgs e)
        {
            
        }

        public void ClickL(Object sender, EventArgs e)
        {
            
        }

        public bool ClearLink()
        {
            foreach (Link il in link)
            {
                il.Dispose();
            }
            link.Clear();
            return true;
        }

        public SBABox()
        {
            InitializeComponent();
        }

        public void deleteLine(int index)
        {
            if (lines.Count > index + 1)
            {
                lock (lines)
                {
                    lines.RemoveAt(index);
                }
                vScrollBar1.Maximum -= 1;
            }
        }

        protected override void Dispose(bool disposing)
        {
            isDisposing = true;
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                // We must dispose of backbufferGraphics before we dispose of backbufferContext or we will get an exception.
                if (backbufferGraphics != null)
                    backbufferGraphics.Dispose();
                if (backbufferContext != null)
                    backbufferContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private void RecreateBuffers()
        {
            // Check initialization has completed so we know backbufferContext has been assigned.
            // Check that we aren't disposing or this could be invalid.
            if (!initializationComplete || isDisposing)
                return;

            // We recreate the buffer with a width and height of the control. The "+ 1"
            // guarantees we never have a buffer with a width or height of 0.
            backbufferContext.MaximumBuffer = new Size(pt.Width + 1, pt.Height + 1);

            // Dispose of old backbufferGraphics (if one has been created already)
            if (backbufferGraphics != null)
                backbufferGraphics.Dispose();

            // Create new backbufferGrpahics that matches the current size of buffer.
            backbufferGraphics = backbufferContext.Allocate(pt.CreateGraphics(),
            new Rectangle(0, 0, Math.Max(pt.Width, 1), Math.Max(pt.Height, 1)));

            // Assign the Graphics object on backbufferGraphics to "drawingGraphics" for easy reference elsewhere.
            drawingGraphics = backbufferGraphics.Graphics;

            // This is a good place to assign drawingGraphics.SmoothingMode if you want a better anti-aliasing technique.

            // Invalidate the control so a repaint gets called somewhere down the line.
            pt.Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecreateBuffers();
            Redraw();
        }

        public void Remove()
        {
            lock (lines)
            {
                lines.Clear();
            }
        }

        public void ScrollToBottom()
        {
            if (((int)Font.Size + 6) * lines.Count > pt.Height)
            {
                offset = ((int)Font.Size + 6) * lines.Count - pt.Height;
                if (vScrollBar1.Maximum > 0)
                {
                    vScrollBar1.Value = vScrollBar1.Maximum;
                }
            }
        }

        private void Scrolled(int x)
        {
                offset = ((int)Font.Size + 6) * x;
        }

        public void insertLine(Line line)
        {
            lock (lines)
            {
                lines.Add(line);
            }
        }

        private void SBABox_Load(object sender, EventArgs e)
        {
            buffer.Visible = false;
            backbufferContext = BufferedGraphicsManager.Current;
            initializationComplete = true;
            vScrollBar1.Minimum = 0;
            vScrollBar1.Value = 0;
            vScrollBar1.Maximum = 0;
            vScrollBar1.Enabled = false;

            RecreateBuffers();

            this.SetStyle(  
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer, true);

            Redraw();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Scrolled(e.NewValue);
            Redraw();
        }

        private void buffer_Click(object sender, EventArgs e)
        {

        }
    }
}
