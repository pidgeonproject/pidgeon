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
            public List<ContentText> text = new List<ContentText>();
            private SBABox owner = null;

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
            }

            public void insertData(string line)
            {
                lock (text)
                {
                    text.Add(new ContentText(line, owner, owner.ForeColor));
                }
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
            }

            public void Merge(Line line)
            {
                lock (text)
                {
                    text.AddRange(line.text);
                }
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
    }
}
