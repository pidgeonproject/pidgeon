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
using System.Text;

namespace Client
{
    public partial class SBABox
    {
        public class Link
        {
            public int X = 0;
            public int Y = 0;
            public int Width = 0;
            public int Height = 0;
            private SBABox Parent = null;
            public string Name = null;
            /// <summary>
            /// URL
            /// </summary>
            public string Text = null;
            public ContentText LinkedText = null;

            public Link(int x, int y, System.Drawing.Color normal, int width, int height, SBABox SBAB, string http, string label, ContentText text)
            {
                X = x;
                Y = y;
                Width = width;
                Name = label;
                Text = http;
                Parent = SBAB;
                LinkedText = text;
                Height = height;
            }

            ~Link()
            {
                Dispose();
            }

            public void Dispose()
            {
                Parent = null;
                LinkedText = null;
                Name = null;
                Text = null;
            }
        }

        public Link getLink(int X, int Y)
        {
            foreach (Link l in LinkInfo)
            {
                if ((X >= l.X && X <= l.X + l.Width) && (Y <= l.Y + l.Height && Y >= l.Y))
                {
                    return l;
                }
            }
            return null;
        }

        public void MouseIcon(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (Configuration.ChangingMouse)
                {
                    if (getLink(e.X, e.Y) == null)
                    {
                        pt.Cursor = System.Windows.Forms.Cursors.Default;
                        return;
                    }
                    pt.Cursor = System.Windows.Forms.Cursors.Hand;
                    return;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
