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
using System.Text;
using Gtk;

namespace Client
{
    public partial class RichTBox : Gtk.Bin
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
            public ContentText(string _text, Color color)
            {
                text = _text;
                TextColor = color;
            }
        }

        public class Line
        {
            public List<ContentText> text = new List<ContentText>();
            public Color foreColor = Color.Blue;

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
                    text.Add(new ContentText(line, foreColor));
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
            }

            public Line(string Text, RichTBox SBAB)
            {
                if (SBAB == null)
                {
                    throw new Exception("SBAB must not be null");
                }
                CreateLine(Text, SBAB, SBAB.ForeColor);
            }

            public Line(string Text, RichTBox SBAB, Color color)
            {
                CreateLine(Text, SBAB, color);
            }

            public Line(RichTBox SBAB)
            {
                text = new List<ContentText>();
                foreColor = SBAB.foreColor;
            }

            public Line()
            {
                text = new List<ContentText>();
                foreColor = Configuration.CurrentSkin.colordefault;
            }

            private void CreateLine(string Text, RichTBox SBAB, Color color)
            {
                text.Add(new ContentText(Text, SBAB.ForeColor));
                foreColor = SBAB.foreColor;
            }
        }
    }
}
