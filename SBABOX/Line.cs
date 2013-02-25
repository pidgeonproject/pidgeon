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

        public class Line
        {
            public class GraphicsInfo
            {
                public class Info
                {
                    public string String;
                    public string String2;
                    public int Width;
                    public RectangleF size;
                    public int Width2;
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

            ~Line()
            {
                owner = null;
                Buffer = null;
            }

            public void Merge(Line line)
            {
                lock (text)
                {
                    text.AddRange(line.text);
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

            private void CreateLine(string Text, SBABox SBAB, Color color)
            {
                text.Add(new ContentText(Text, SBAB, SBAB.ForeColor));
                owner = SBAB;
                Buffer.Valid = false;
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
    }
}
