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
        private bool initializationComplete = false;
        public bool WrapAll = true;
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
        private Graphics drawingGraphics = null;
        public Scrollback scrollback = null;
        private List<Line> LineDB = new List<Line>();
        private bool Rendering = false;
        private Font GlobalFont = null;
        /// <summary>
        /// Information about all links
        /// </summary>
        private List<Link> LinkInfo = new List<Link>();
        private int spacing = 2;
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
        /// Color of text
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                Redraw();
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
                Redraw();
            }
        }

        /// <summary>
        /// Return a text with no formatting, this can be used in case you want to copy content of this box
        /// </summary>
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

        public void ChangeSize(object sender, EventArgs e)
        {
            try
            {
                InvalidateCaches();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
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
        /// Redraw line
        /// </summary>
        /// <param name="graphics">Graphics object you are drawing to</param>
        /// <param name="X">carret X</param>
        /// <param name="Y">carret Y</param>
        /// <param name="line">line</param>
        private void RedrawLine(ref Graphics graphics, ref float X, ref float Y, Line line)
        {
            bool wrappingnow = false;
            // we increase the number of lines that are rendered - we do it now because this line will be either renedered or whole text won't be rendered
            RenderedLineTotalCount++;
            // this is a line which could be drawn in case that we have to split this line to more
            Line extraline = null;
            lock (line.text)
            {
                foreach (ContentText part in line.text)
                {
                    // we don't want to draw lines that will not be in visible area
                    bool RedrawLine = false;
                    // in case that we are now wrapping this former part of line is now belonging to new line
                    if (wrappingnow)
                    {
                        extraline.insertData(part);
                        continue;
                    }
                    int padding = (int)Font.Size + spacing;
                    if ((Y + padding) > 0 && (Y - padding < this.Height))
                    {
                        RedrawLine = true;
                    }
                    if (RedrawLine || WrapAll)
                    {
                        // in case that text is nothing or null we will not draw this part
                        if (part.Text != null && part.Text != "")
                        {
                            Brush brush = new SolidBrush(part.TextColor);
                            string TextOfThisPart = part.Text;
                            StringFormat format = new StringFormat(StringFormat.GenericTypographic);
                            format.Trimming = StringTrimming.None;
                            format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                            Font font = GlobalFont;

                            if (part.Bold && part.Underline)
                            {
                                font = new Font(font, FontStyle.Underline | FontStyle.Bold);
                            }
                            else
                            {
                                if (part.Bold)
                                {
                                    font = new Font(font, FontStyle.Bold);
                                }

                                if (part.Underline)
                                {
                                    font = new Font(font, FontStyle.Underline);
                                }
                            }

                            // we need to retrieve the buffer if there is some
                            Line.GraphicsInfo.Info partInfo;
                            int stringWidth = 0;
                            RectangleF stringSize;
                            
                            // if it's buffered, we get the data from a buffer instead of making shitton of calculations
                            if (line.Buffer.Valid)
                            {
                                if (!line.Buffer.Buffer.ContainsKey(part))
                                {
                                    throw new Exception("This SBABOX line has cache that contains corrupted data");
                                }
                            }

                            // in case there is no valid data we need to calculate them now and save them for later use
                            if (!line.Buffer.Valid)
                            {
                                stringSize = Buffers.MeasureString(graphics, part.Text, font, format);
                                stringWidth = Buffers.MeasureDisplayStringWidth(stringSize);
                                partInfo = new Line.GraphicsInfo.Info();
                                lock (line.Buffer.Buffer)
                                {
                                    if (!line.Buffer.Buffer.ContainsKey(part))
                                    {
                                        line.Buffer.Buffer.Add(part, partInfo);
                                    }
                                    else
                                    {
                                        line.Buffer.Buffer[part] = partInfo;
                                    }
                                }
                                partInfo.Width = stringWidth;
                                partInfo.size = stringSize;
                            }
                            else
                            {
                                lock (line.Buffer.Buffer)
                                {
                                    partInfo = line.Buffer.Buffer[part];
                                }
                                stringSize = partInfo.size;
                                stringWidth = partInfo.Width;
                            }
                            

                            // in case this line is about to be wrapped we need to split it
                            if (Wrap && !(line.Buffer.Valid && !partInfo.Oversized))
                            {
                                bool isOversized;
                                int width = this.Width - vScrollBar1.Width - 20;
                                if (!line.Buffer.Valid)
                                {
                                    isOversized = (X + stringWidth) > width;
                                    partInfo.Oversized = isOversized;
                                }
                                else
                                {
                                    isOversized = partInfo.Oversized;
                                }
                                
                                if (isOversized)
                                {
                                    if (!line.Buffer.Valid)
                                    {
                                        bool ls = part.Text.StartsWith(" ");
                                        bool es = part.Text.EndsWith(" ");
                                        string[] words = part.Text.Split(' ');
                                        string trimmed = "";
                                        foreach (string xx in words)
                                        {
                                            string value = xx;
                                            if (graphics.MeasureString(value, font, new Point(0, 0), format).Width > width)
                                            {
                                                int size = xx.Length - 1;
                                                while (graphics.MeasureString(value.Substring(0, size), font, new Point(0, 0), format).Width > width)
                                                {
                                                    if (size <= 0)
                                                    {
                                                        if (Configuration.Debugging)
                                                        {
                                                            throw new Exception("Invalid size");
                                                        }
                                                        Core.DebugLog("Invalid size on SBABox, error #1");
                                                        line.Buffer.Invalidate();
                                                        return;
                                                    }
                                                    if (size > 10)
                                                    {
                                                        size = size - 5;
                                                    }
                                                    else
                                                    {
                                                        size--;
                                                    }
                                                }
                                                string part1 = xx.Substring(0, size);
                                                string part2 = value.Substring(size);
                                                trimmed += part1;
                                                value = part2;
                                            }
                                            if (graphics.MeasureString(trimmed + value, font, new Point(0, 0), format).Width + X > width)
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
                                        TextOfThisPart = trimmed;
                                        string remaining_data = null;
                                        if (trimmed.Length < part.Text.Length)
                                        {
                                            remaining_data = part.Text.Substring(trimmed.Length);
                                        }
                                        else
                                        {
                                            remaining_data = part.Text;
                                        }
                                        ContentText _text = new ContentText(remaining_data, this, part.TextColor);
                                        _text.Underline = part.Underline;
                                        _text.Bold = part.Bold;
                                        _text.Link = part.Link;
                                        wrappingnow = true;
                                        extraline = new Line(this);
                                        extraline.insertData(_text);
                                        partInfo.String = TextOfThisPart;
                                        partInfo.String2 = remaining_data;
                                        //partInfo.extraLine = extraline;
                                    }
                                    else
                                    {
                                        TextOfThisPart = partInfo.String;
                                        //extraline = partInfo.extraLine;
                                        ContentText _text = new ContentText(partInfo.String2, this, part.TextColor);
                                        _text.Underline = part.Underline;
                                        _text.Bold = part.Bold;
                                        _text.Link = part.Link;
                                        wrappingnow = true;
                                        extraline = new Line(this);
                                        extraline.insertData(_text);
                                        wrappingnow = true;
                                    }
                                }
                            }

                            if (TextOfThisPart.Length < 1)
                            {
                                continue;
                            }

                            if (TextOfThisPart.Length != part.Text.Length)
                            {
                                if (line.Buffer.Valid)
                                {
                                    stringSize = partInfo.size2;
                                    stringWidth = partInfo.Width2;
                                }
                                else
                                {
                                    stringSize = Buffers.MeasureString(graphics, TextOfThisPart, font, format);
                                    stringWidth = Buffers.MeasureDisplayStringWidth(stringSize);
                                    partInfo.size2 = stringSize;
                                    partInfo.Width2 = stringWidth;
                                }
                            }

                            if (part.Link != null && RedrawLine)
                            {
                                if (!part.Linked)
                                {
                                    part.Linked = true;
                                    Pen pen = new Pen(part.TextColor);
                                    Link http = new Link((int)X, (int)Y, part.TextColor, stringWidth, (int)stringSize.Height, this, part.Link, TextOfThisPart, part);
                                    if (Configuration.Debugging)
                                    { 
                                        graphics.DrawLine(pen, new Point(http.X, http.Y), new Point(http.X + stringWidth, http.Y));
                                        graphics.DrawLine(pen, new Point(http.X, http.Y), new Point(http.X, http.Y + (int)stringSize.Height));
                                    }
                                    lock (LinkInfo)
                                    {
                                        LinkInfo.Add(http);
                                    }
                                }
                            }

                            if (RedrawLine)
                            {
                                graphics.DrawString(TextOfThisPart, font, brush, X, Y);
                            }

                            X = X + stringWidth;
                            if (!Wrap && RedrawLine)
                            {
                                if ((int)(X + stringWidth) > hsBar.Maximum)
                                {
                                    lock (hsBar)
                                    {
                                        if (hsBar.Value > (int)(X + stringWidth))
                                        {
                                            hsBar.Value = (int)(X + stringWidth);
                                        }
                                        hsBar.Maximum = (int)(X + stringWidth);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (wrappingnow)
            {
                Y = Y + Font.Size + spacing;
                X = 0 - currentX;
                RedrawLine(ref graphics, ref X, ref Y, extraline);
            }
            if (!line.Buffer.Valid)
            {
                line.Buffer.Valid = true;
            }
        }

        /// <summary>
        /// Reload the scroll bars and their values
        /// </summary>
        private void UpdateBars()
        {
            scrollValue = RenderedLineTotalCount - (int)((float)pt.Height / (Font.Size + spacing));

            if (scrollValue < 0)
            {
                vScrollBar1.Enabled = false;
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
                        vScrollBar1.Maximum = scrollValue;
                        vScrollBar1.Enabled = true;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (vScrollBar1.Maximum > 0)
                        {
                            vScrollBar1.Value = vScrollBar1.Maximum - 1;
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
        /// Redraw all text
        /// </summary>
        public void RedrawText()
        {
            try
            {
                lock (this)
                {
                    if (!ReloadingEnabled)
                    {
                        return;
                    }
                    Rendering = true;
                    RenderedLineTotalCount = 0;

                    float X = 0 - currentX;
                    float Y = 0 - currentY;

                    Graphics graphics = null;
                    if (drawingGraphics == null)
                    {
                        return;
                    }

                    graphics = drawingGraphics;

                    ClearLink();
                    lock (graphics)
                    {
                        graphics.Clear(BackColor);
                    }

                    GlobalFont = new Font(this.Font, FontStyle.Regular);

                    lock (LineDB)
                    {
                        foreach (Line text in LineDB)
                        {
                            X = 0 - currentX;
                            RedrawLine(ref graphics, ref X, ref Y, text);
                            Y = Y + Font.Size + spacing;
                        }
                    }

                    UpdateBars();

                    pt.Invalidate();
                    Rendering = false;
                }
            }
            catch (Exception fail)
            {
                Rendering = false;
                Core.handleException(fail);
            }
        }

        public void Wheeled(Object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta != 0)
                {
                    try
                    {
                        lock (vScrollBar1)
                        {
                            int delta = (e.Delta / 16) * -1;
                            if (delta < 0)
                            {
                                if ((vScrollBar1.Value + delta) > vScrollBar1.Minimum)
                                {
                                    vScrollBar1.Value = vScrollBar1.Value + delta;
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
                                    vScrollBar1.Value = vScrollBar1.Value + delta;
                                }
                                else
                                {
                                    vScrollBar1.Value = vScrollBar1.Maximum;
                                }
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        vScrollBar1.Value = vScrollBar1.Maximum;
                    }
                    ScrolltoX(vScrollBar1.Value);
                    Redraw();
                    return;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void ClickHandler(Object sender, MouseEventArgs e)
        {
            try
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
                        scrollback.Click_R(httparea.Name, httparea.Text);
                        return;
                    }

                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        scrollback.Click_L(httparea.Text);
                        return;
                    }
                }
                if (scrollback.owner != null)
                {
                    scrollback.owner.textbox.richTextBox1.Focus();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
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
                    il.linkedtext.Linked = false;
                    il.Dispose();
                }
                LinkInfo.Clear();
            }
            return true;
        }

        public SBABox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Remove a part of text at specific line
        /// </summary>
        /// <param name="index"></param>
        public void deleteLine(int index, bool redraw = false)
        {
            lock (LineDB)
            {
                if (LineDB.Count > index + 1)
                {
                    LineDB.RemoveAt(index);
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
            if (redraw)
            {
                Redraw();
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                lock (this)
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
                }
                base.Dispose(disposing);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
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
            try
            {
                base.OnResize(e);
                RecreateBuffers();
                Redraw();
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
                if (vScrollBar1.Maximum > 1)
                {
                    int previous = 0;
                    while (previous < vScrollBar1.Maximum)
                    {
                        previous = vScrollBar1.Maximum;
                        ScrolltoX(vScrollBar1.Maximum);
                        vScrollBar1.Value = vScrollBar1.Maximum;
                        Redraw();
                    }
                }
            }
        }

        private void ScrolltoX(int x)
        {
            currentY = ((int)Font.Size + spacing) * x;
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
            try
            {
                lock (vScrollBar1)
                {
                    ScrolltoX(e.NewValue);
                    Redraw();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                lock (hsBar)
                {
                    currentX = hsBar.Value;
                    Redraw();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
