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
        /// <summary>
        /// Content of box in unformatted way
        /// </summary>
        private string text = null;
        public bool BufferOn = false;
        private bool initializationComplete = false;
        private bool isDisposing = false;
        private BufferedGraphicsContext backbufferContext = null;
        private BufferedGraphics backbufferGraphics = null;
        private int offset = 0;
        private int currentX = 0;
        private int scrolldown = 0;
        public bool Wrap = true;
        public int rendered = 0;
        private Graphics drawingGraphics = null;
        public Scrollback scrollback = null;
        private List<Line> LineDB = new List<Line>();
        private bool reloading = false;

        public class ContentText
        {
            /// <summary>
            /// Text of this part
            /// </summary>
            public string Text = null;
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
            public ContentText(string text, SBABox SBAB, Color color)
            {
                Text = text;
                Owner = SBAB;
                TextColor = color;
            }

            ~ContentText()
            {
                Owner = null;
            }
        }

        public List<Link> LinkInfo = new List<Link>();

        public class Link
        {
            public int X = 0;
            public int Y = 0;
            public int Width = 0;
            public int Height = 0;
            private SBABox Parent = null;
            public string _name = null;
            public string _text = null;
            public ContentText linkedtext = null;

            public Link(int x, int y, System.Drawing.Color normal, int width, int height, SBABox SBAB, string http, string label, ContentText text)
            {
                X = x;
                Y = y;
                Width = width;
                _name = label;
                _text = http;
                Parent = SBAB;
                linkedtext = text;
                Height = height;
            }

            ~Link()
            {
                Parent = null;
                linkedtext = null;
            }

            public void Dispose()
            {
                Parent = null;
                linkedtext = null;
            }
        }

        public class Line
        {
            public List<ContentText> text = new List<ContentText>();
            private SBABox owner = null;

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
                    if (data.Text != null)
                    {
                        part += data.Text;
                    }
                }
                return part;
            }

            ~Line()
            {
                owner = null;
            }

            public void Merge(Line line)
            {
                text.AddRange(line.text);
            }

            public Line(string Text, SBABox SBAB)
            {
                if (SBAB == null)
                {
                    throw new Exception("SBAB must not be null");
                }
                CreateLine(Text, SBAB, SBAB.ForeColor);
            }

            public Line(string Text, SBABox SBAB, Color color)
            {
                CreateLine(Text, SBAB, color);
            }

            private void CreateLine(string Text, SBABox SBAB, Color color)
            {
                text.Add(new ContentText(Text, SBAB, SBAB.ForeColor));
                owner = SBAB;
            }

            public Line(SBABox SBAB)
            {
                text = new List<ContentText>();
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
                lock (LineDB)
                {
                    text = "";
                    text = Hooks.BeforeParser(text);
                    foreach (Line _l in LineDB)
                    {
                        string line = _l.ToString();
                        if (line != null)
                        {
                            text = Hooks.BeforeInsertLine(text, line + "\n");
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
                pt.Refresh();
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

        public string maketext(int length)
        {
            int curr = 0;
            string text = "";
            while (curr < length)
            {
                curr++;
                text += "F";
            }
            return text;
        }

        protected void RepaintWindow(object sender, PaintEventArgs e)
        {
            if (isDisposing)
            {
                return;
            }
            if (backbufferGraphics != null)
            {
                backbufferGraphics.Render(e.Graphics);
            }
        }

        /// <summary>
        /// Redraw
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="line"></param>
        public void RedrawLine(ref Graphics graphics, ref float X, ref float Y, Line line)
        {
            bool wrappingnow = false;
            rendered++;
            Line extraline = null;
            lock (line.text)
            {
                foreach (ContentText part in line.text)
                {
                    if (wrappingnow)
                    {
                        extraline.insertData(part);
                        continue;
                    }
                    if ((Y + (int)Font.Size + 8) > 0 && (Y + (int)Font.Size + 8) < this.Height)
                    {
                        if (part.Text != "")
                        {
                            Brush brush = new SolidBrush(part.TextColor);
                            string TextOfThispart = part.Text;
                            TextOfThispart = Hooks.BeforeLinePartLoad(TextOfThispart);
                            StringFormat format = new StringFormat(StringFormat.GenericTypographic);
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
                            SizeF stringSize = graphics.MeasureString(part.Text, font, new Point(0, 0), format);
                            //stringSize.Width = Buffers.MeasureDisplayStringWidth(_t, part.text, font, format);
                            if (Wrap)
                            {
                                if (X + stringSize.Width > (this.Width - vScrollBar1.Width - 20))
                                {
                                    bool ls = part.Text.StartsWith(" ");
                                    bool es = part.Text.EndsWith(" ");
                                    string[] words = part.Text.Split(' ');
                                    string trimmed = "";
                                    foreach (string xx in words)
                                    {
                                        string value = xx;
                                        if (graphics.MeasureString(value, font, new Point(0, 0), format).Width > (this.Width - 20) - vScrollBar1.Width)
                                        {
                                            int size = xx.Length - 1;
                                            while (graphics.MeasureString(maketext(size), font, new Point(0, 0), format).Width > (this.Width - 20) - vScrollBar1.Width)
                                            {
                                                if (size == 0)
                                                {
                                                    Core.DebugLog("Invalid size on SBABox, error #1");
                                                    return;
                                                }
                                                size--;
                                            }
                                            string part1 = xx.Substring(0, size);
                                            string part2 = value.Substring(size);
                                            trimmed += part1;
                                            value = part2;
                                        }
                                        if (graphics.MeasureString(trimmed + value, font, new Point(0, 0), format).Width + X > (this.Width - 40) - vScrollBar1.Width)
                                        {
                                            if (trimmed != "" || graphics.MeasureString(value, font, new Point(0, 0), format).Width < pt.Width)
                                            {
                                                break;
                                            }
                                        }
                                        trimmed += value + " ";
                                    }
                                    if (!ls && trimmed.EndsWith(" ") && trimmed.Length > 0)
                                    {
                                        trimmed = trimmed.Substring(0, trimmed.Length - 1);
                                    }
                                    // we trimmed the value, now update the text temporarily
                                    TextOfThispart = trimmed;
                                    string remaining_data = part.Text.Substring(trimmed.Length);
                                    extraline = new Line("", this);
                                    ContentText _text = new ContentText(remaining_data, this, part.TextColor);
                                    _text.Underline = part.Underline;
                                    _text.Bold = part.Bold;
                                    _text.Link = part.Link;
                                    wrappingnow = true;
                                    extraline.insertData(_text);
                                }
                            }

                            if (part.Link != null)
                            {
                                if (!part.Linked)
                                {
                                    part.Linked = true;
                                    Pen pen = new Pen(part.TextColor);
                                    Link http = new Link((int)X, (int)Y, part.TextColor, (int)stringSize.Width, (int)stringSize.Height, this, part.Link, part.Text, part);
                                    lock (LinkInfo)
                                    {
                                        LinkInfo.Add(http);
                                    }
                                }
                            }
                            graphics.DrawString(TextOfThispart, font, brush, X, Y);

                            X = X + stringSize.Width;
                            if (Wrap)
                            {
                                hsBar.Visible = false;
                            }
                            else
                            {
                                if ((int)(X + stringSize.Width) > hsBar.Maximum)
                                {
                                    lock (hsBar)
                                    {
                                        if (hsBar.Value > (int)(X + stringSize.Width))
                                        {
                                            hsBar.Value = (int)(X + stringSize.Width);
                                        }
                                        hsBar.Maximum = (int)(X + stringSize.Width);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (wrappingnow)
            {
                Y = Y + Font.Size + 6;
                X = 0 - currentX;
                RedrawLine(ref graphics, ref X, ref Y, extraline);
            }
        }

        public void RedrawText()
        {
            try
            {
                reloading = true;
                rendered = 0;
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

                lock (LineDB)
                {
                    foreach (Line text in LineDB)
                    {
                        X = 0 - currentX;
                        RedrawLine(ref _t, ref X, ref Y, text);
                        Y = Y + Font.Size + 6;
                    }
                }
                scrolldown = rendered - ((int)((float)pt.Height / (Font.Size + 6)));
                if (scrolldown < 0)
                {
                    vScrollBar1.Enabled = false;
                }

                if (scrolldown > 0)
                {
                    lock (vScrollBar1)
                    {
                        if (vScrollBar1.Value > scrolldown)
                        {
                            vScrollBar1.Value = scrolldown;
                        }
                        vScrollBar1.Maximum = scrolldown;
                        vScrollBar1.Enabled = true;
                    }
                }
                pt.Invalidate();
                reloading = false;
            }
            catch (Exception fail)
            {
                reloading = false;
                Core.handleException(fail);
            }
        }

        public void Wheeled(Object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                int delta = (e.Delta / 16) * -1;
                if (delta < 0)
                {
                    if ((vScrollBar1.Value + delta) > vScrollBar1.Minimum)
                    {
                        vScrollBar1.Value += delta;
                    }
                    else
                    {
                        vScrollBar1.Value = vScrollBar1.Minimum;
                    }
                }

                if (delta > 0)
                {
                    if ((vScrollBar1.Value + delta) < vScrollBar1.Maximum)
                    {
                        vScrollBar1.Value += delta;
                    }
                    else
                    {
                        vScrollBar1.Value = vScrollBar1.Maximum;
                    }
                }
                ScrolltoX(vScrollBar1.Value);
                Redraw();
                return;
            }
        }

        public void ClickHandler(Object sender, MouseEventArgs e)
        {
            Link httparea = null;
            foreach (Link l in LinkInfo)
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
                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    scrollback.Click_L(httparea._text);
                    return;
                }
            }
            if (scrollback.owner != null)
            {
                scrollback.owner.textbox.richTextBox1.Focus();
            }
        }

        public bool ClearLink()
        {
            foreach (Link il in LinkInfo)
            {
                il.linkedtext.Linked = false;
                il.linkedtext = null;
                il.Dispose();
            }
            LinkInfo.Clear();
            return true;
        }

        public SBABox()
        {
            InitializeComponent();
        }

        public void deleteLine(int index)
        {
            if (LineDB.Count > index + 1)
            {
                lock (LineDB)
                {
                    LineDB.RemoveAt(index);
                }
                lock (vScrollBar1)
                {
                    if (vScrollBar1.Maximum > 2)
                    {
                        if (vScrollBar1.Value == vScrollBar1.Maximum)
                        {
                            vScrollBar1.Value--;
                        }
                        vScrollBar1.Maximum--;
                    }
                }
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
            {
                lock (backbufferGraphics)
                {
                    backbufferGraphics.Dispose();
                }
            }

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

        /// <summary>
        /// Remove all text of this item
        /// </summary>
        public void RemoveText()
        {
            offset = 0;
            lock (LineDB)
            {
                LineDB.Clear();
            }
        }

        private void Lock()
        {
            while (reloading)
            {
                System.Threading.Thread.Sleep(10);
            }
        }

        public void ScrollToBottom()
        {
            lock (vScrollBar1)
            {
                if (vScrollBar1.Maximum > 1)
                {
                    int previous = 0;
                    while (previous < vScrollBar1.Maximum)
                    {
                        previous = vScrollBar1.Maximum;
                        vScrollBar1.Value = vScrollBar1.Maximum;
                        ScrolltoX(vScrollBar1.Maximum);
                        Redraw();
                    }
                }
            }
        }

        private void ScrolltoX(int x)
        {
            offset = ((int)Font.Size + 6) * x;
        }

        public void InsertLine(Line line)
        {
            lock (LineDB)
            {
                LineDB.Add(line);
            }
        }

        public void InsertLine(string text, Color color)
        {
            lock (LineDB)
            {
                LineDB.Add(new Line(text, this, color));
            }
        }

        public void InsertLine(string text)
        {
            InsertLine(text, ForeColor);
        }

        public void InsertLine(List<ContentText> text)
        {
            Line line = new Line(this);
            lock (text)
            {
                line.text.AddRange(text);
            }
            InsertLine(line);
        }

        public ContentText CreateText(string text, Color color)
        {
            return new ContentText(text, this, color);
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
                hsBar.Visible = false;
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
            lock (vScrollBar1)
            {
                ScrolltoX(e.NewValue);
                Redraw();
            }
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            lock (hsBar)
            {
                currentX = hsBar.Value;
                Redraw();
            }
        }
    }
}
