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
using System.Linq;
using System.Drawing;
using System.Text;

namespace Client
{
    public partial class SBABox
    {
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
            RecursiveMax++;
            if (RecursiveMax > 20)
            {
                Core.DebugLog("Recursive point reached");
                return;
            }
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
                                                    if (size <= 1)
                                                    {
                                                        if (Configuration.Kernel.Debugging)
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
                                        partInfo.extraLine = extraline;
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
                                    if (Configuration.Kernel.Debugging)
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

                    if (this.Width < MinimalWidth)
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
                        Rendering = false;
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
                            RecursiveMax = 0;
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
    }
}
