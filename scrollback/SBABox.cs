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
        private int currentX = 0;
        public bool Wrap = true;
        private Graphics drawingGraphics;
        public Scrollback scrollback;

        public class ContentText
        {
            public string text;
            private SBABox owner;
            public Color textc;
            public Color backc;
            public bool linked = false;
            public bool Bold = false;
            public bool Italic = false;
            public bool Underline = false;
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
            public int X;
            public int Y;
            public int Width;
            public int Height;
            private SBABox parent;
            public string _name;
            public string _text;
            public ContentText linkedtext;
            //public LinkLabel area;
            public Link(int x, int y, System.Drawing.Color normal, int width, int height, SBABox SBAB, string http, string label, ContentText text)
            {
                X = x;
                Y = y;
                Width = width;
                _name = label;
                _text = http;
                //area = new LinkLabel();
                //area.BackColor = Color.Transparent;
                parent = SBAB;
                //area.Left = X;
                //area.Top = Y;
                linkedtext = text;
                //area.AutoSize = false;
                //area.LinkColor = Color.Transparent;
                //area.Font = parent.Font;
                //area.Text = label;
                //area.ActiveLinkColor = Color.Transparent;
                //area.VisitedLinkColor = Color.Transparent;
                //area.Width = width;
                //area.Height = height;
                
                //area.Name = http;
                //area.CreateControl();
                //area.ForeColor = normal;
                //area.MouseClick += new MouseEventHandler(parent.ClickHandler);
                //area.ContextMenu = parent.ContextMenu;

                Height = height;
            }
            public void Dispose()
            {
                linkedtext = null;
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
                owner = null;
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
                RedrawText();
            }
        }
        public override string Text
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

        public void RedrawLine(ref Graphics _t, ref float X, ref float Y, Line line)
        {
            bool wrappingnow = false;
            Line extraline = null;
            foreach (ContentText part in line.text)
            {
                if (part.text != "")
                {
                    if (wrappingnow)
                    {
                        extraline.insertData(part);
                        continue;
                    }
                    if ((Y + (int)Font.Size + 8) > 0 && (Y + (int)Font.Size + 8) < this.Height)
                    {
                        Brush _b = new SolidBrush(part.textc);
                        string TextOfThispart = part.text;
                        StringFormat format = new StringFormat(StringFormat.GenericTypographic);
                        format.Alignment = StringAlignment.Center;
                        format.Trimming = StringTrimming.None;
                        format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                        Font font = new Font(this.Font, FontStyle.Regular);

                        if (part.Bold)
                        {
                            font = new Font(font, FontStyle.Bold);
                        }

                        if (part.Underline)
                        {
                            font = new Font(font, FontStyle.Underline);
                        }

                        //SizeF stringSize = TextRenderer.Me
                        SizeF stringSize = _t.MeasureString(part.text, font, new Point(0, 0), format);
                        if (Wrap)
                        {
                            if (X + stringSize.Width > pt.Width)
                            {
                                bool ls = part.text.StartsWith(" ");
                                bool es = part.text.EndsWith(" ");
                                string[] words = part.text.Split(' ');
                                string trimmed = "";
                                foreach (string xx in words)
                                {
                                    if (_t.MeasureString(trimmed + xx, font, new Point(0, 0), format).Width + X > pt.Width)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        trimmed += xx + " ";
                                    }
                                }
                                if (!ls && trimmed.EndsWith(" ") && trimmed.Length > 0)
                                {
                                    trimmed = trimmed.Substring(0, trimmed.Length - 1);
                                }
                                // we trimmed the value, now update the text temporarily
                                TextOfThispart = trimmed;
                                string remaining_data = part.text.Substring(trimmed.Length);
                                extraline = new Line("", this);
                                ContentText _text = new ContentText(remaining_data, this, part.textc);
                                _text.Underline = part.Underline;
                                _text.Bold = part.Bold;
                                _text.link = part.link;
                                wrappingnow = true;
                                extraline.insertData(_text);
                            }
                        }

                        if (part.link != null)
                        {
                            if (!part.linked)
                            {
                                part.linked = true;
                                Pen pen = new Pen(part.textc);
                                Link http = new Link((int)X, (int)Y, part.textc, (int)stringSize.Width, (int)stringSize.Height, this, part.link, part.text, part);
                                lock (link)
                                {
                                    link.Add(http);
                                }
                            }
                        }
                        _t.DrawString(TextOfThispart, font, _b, X, Y);

                        X = X + stringSize.Width;
                        if (Wrap)
                        {
                            ScrollBar.Visible = false;
                        } else
                        if ((int)(X + stringSize.Width) > ScrollBar.Maximum)
                        {
                            ScrollBar.Maximum = (int)(X + stringSize.Width);
                        }
                    }
                }
            }
            if (wrappingnow)
            {
                Y = Y + Font.Size + 6;
                X = 0 - currentX;
                RedrawLine(ref _t, ref X,ref Y, extraline);
            }
        }

        public void RedrawText()
        {
            //
            float X = 0 - currentX;
            float Y = 0 - offset;

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
                X = 0 - currentX;
                RedrawLine(ref _t, ref X, ref Y, text);
                Y = Y + Font.Size + 6;
            }
            vScrollBar1.Maximum = lines.Count;
            if (lines.Count > 0)
            {
                vScrollBar1.Enabled = true;
            }
            pt.Invalidate();
        }

        public void ClickHandler(Object sender, MouseEventArgs e)
        {
            Link httparea = null;
            foreach (Link l in link)
            {
                if ((e.X >= l.X && e.X <= l.X + l.Width) && (e.Y <= l.Y + l.Height && e.Y >= l.Y))
                {
                    httparea = l;
                    break;
                }
            }
            if (httparea != null)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    scrollback.Click_R(httparea._name, httparea._text);
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    scrollback.Click_L(httparea._text);
                }
            }
        }

        public bool ClearLink()
        {
            foreach (Link il in link)
            {
                il.linkedtext.linked = false;
                il.linkedtext = null;
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
            backbufferContext = BufferedGraphicsManager.Current;
            initializationComplete = true;
            vScrollBar1.Minimum = 0;
            vScrollBar1.Value = 0;
            vScrollBar1.Maximum = 0;
            vScrollBar1.Enabled = false;
            if (Wrap)
            {
                ScrollBar.Visible = false;
            }

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

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            currentX = ScrollBar.Value;
            Redraw();
        }
    }
}
