//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

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
        /// <summary>
        /// This is a part of text in a line
        /// </summary>
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
            /// Color
            /// </summary>
            public Color TextColor = Color.White;
            /// <summary>
            /// Color
            /// </summary>
            public Color Backcolor = Color.Black;
            /// <summary>
            /// Linked
            /// </summary>
            public bool Linked = false;
            /// <summary>
            /// Bold
            /// </summary>
            public bool Bold = false;
            /// <summary>
            /// Italic
            /// </summary>
            public bool Italic = false;
            /// <summary>
            /// Underline
            /// </summary>
            public bool Underline = false;
            /// <summary>
            /// Link
            /// </summary>
            public string Link = null;
            /// <summary>
            /// Whether pop a menu
            /// </summary>
            public bool menu = false;

            /// <summary>
            /// Creates a new content
            /// </summary>
            /// <param name="_text">Text of this part</param>
            /// <param name="color">Color of a text</param>
            public ContentText(string _text, Color color)
            {
                text = _text;
                TextColor = color;
            }
        }

        /// <summary>
        /// Line of text
        /// </summary>
        public class Line
        {
            /// <summary>
            /// Parts
            /// </summary>
            public List<ContentText> text = new List<ContentText>();
            /// <summary>
            /// Color
            /// </summary>
            public Color foreColor = Color.Blue;

            /// <summary>
            /// Insert data
            /// </summary>
            /// <param name="line"></param>
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

            /// <summary>
            /// Insert data
            /// </summary>
            /// <param name="line"></param>
            public void insertData(string line)
            {
                lock (text)
                {
                    text.Add(new ContentText(line, foreColor));
                }
            }

            /// <summary>
            /// Convert to string
            /// </summary>
            /// <returns></returns>
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

            /// <summary>
            /// Initializes a new instance of the <see cref="Client.RichTBox.Line"/> class.
            /// </summary>
            /// <param name="Text">Text.</param>
            /// <param name="SBAB">SBA.</param>
            public Line(string Text, RichTBox SBAB)
            {
                if (SBAB == null)
                {
                    throw new Exception("SBAB must not be null");
                }
                CreateLine(Text, SBAB, SBAB.ForeColor);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Client.RichTBox.Line"/> class.
            /// </summary>
            /// <param name="Text">Text.</param>
            /// <param name="SBAB">SBA.</param>
            /// <param name="color">Color.</param>
            public Line(string Text, RichTBox SBAB, Color color)
            {
                CreateLine(Text, SBAB, color);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Client.RichTBox.Line"/> class.
            /// </summary>
            /// <param name="SBAB">SBA.</param>
            public Line(RichTBox SBAB)
            {
                text = new List<ContentText>();
                foreColor = SBAB.foreColor;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Client.RichTBox.Line"/> class.
            /// </summary>
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
