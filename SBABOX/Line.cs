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
        public class Line
        {
            public class GraphicsInfo
            {
                public class Info
                {
                    public string String = null;
                    public string String2 = null;
                    /// <summary>
                    /// Cached size
                    /// </summary>
                    public int Width = 0;
                    public RectangleF size;
                    public int Width2 = 0;
                    public Line extraLine = null;
                    public RectangleF size2;
                    public bool Oversized = false;

                    public void Remove()
                    {
                        extraLine = null;
                    }

                    ~Info()
                    {
                        Remove();
                    }
                }

                public bool Valid = false;
                public Dictionary<ContentText, Info> Buffer = new Dictionary<ContentText, Info>();
                public int Y = 0;

                ~GraphicsInfo()
                {
                    Buffer = null;
                }

                public void Invalidate()
                {
                    lock (Buffer)
                    {
                        Valid = false;
                        Buffer.Clear();
                    }
                }
            }

            public List<ContentText> text = new List<ContentText>();
            private SBABox owner = null;
            public GraphicsInfo Buffer = new GraphicsInfo();

            public void insertData(ContentText line)
            {
                if (line == null)
                {
                    return;
                }
                lock (text)
                {
                    text.Add(line);
                }
                Buffer.Invalidate();
            }

            public void insertData(string line)
            {
                lock (text)
                {
                    text.Add(new ContentText(line, owner, owner.ForeColor));
                }
                Buffer.Invalidate();
            }

            public override string ToString()
            {
                string part = "";
                lock (text)
                {
                    foreach (ContentText data in text)
                    {
                        if (data.Text != null)
                        {
                            part += data.Text;
                        }
                    }
                }
                return part;
            }

            /// <summary>
            /// When we remove the line we should free the memory allocated for it
            /// </summary>
            ~Line()
            {
                owner = null;
                Buffer = null;
            }

            /// <summary>
            /// Merge with a different instance of line, context of another line will be appended to this one
            /// </summary>
            /// <param name="line"></param>
            public void Merge(Line line)
            {
                lock (line.text)
                {
                    lock (text)
                    {
                        text.AddRange(line.text);
                    }
                }
                Buffer.Invalidate();
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

            private void CreateLine(string Text, SBABox SBAB, Color color)
            {
                text.Add(new ContentText(Text, SBAB, SBAB.ForeColor));
                owner = SBAB;
                Buffer.Valid = false;
            }
        }

        public void InvalidateCaches()
        {
            lock (LineDB)
            {
                foreach (Line l in LineDB)
                {
                    l.Buffer.Invalidate();
                }
            }
        }

        public void RemoveLine(int Index, bool Redraw = false)
        {
            lock (LineDB)
            {
                if (LineDB.Count > 0)
                {
                    if (Index < 0 || Index > (LineDB.Count - 1))
                    {
                        throw new Exception("Index must be between 0 and " + (LineDB.Count - 1).ToString());
                    }
                    LineDB.RemoveAt(Index);

                    if (Redraw)
                    {
                        RedrawText();
                    }
                }
            }
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
                    /*lock (vScrollBar1)
                    {
                        if (vScrollBar1.Maximum > 2)
                        {
                            if (vScrollBar1.Value == vScrollBar1.Maximum)
                            {
                                vScrollBar1.Value--;
                            }
                            vScrollBar1.Maximum--;
                        }
                    }*/
                }
            }
            if (redraw)
            {
                Redraw();
            }
        }
    }
}
