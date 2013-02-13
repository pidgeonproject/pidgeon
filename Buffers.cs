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
using System.Windows.Forms;
using System.Drawing;
using System.Text;

namespace Client
{
    class Buffers
    {
        static public RectangleF MeasureString(Graphics graphics, string text, Font font, StringFormat format)
        {
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0, 10000, 10000);
            CharacterRange[] ranges = { new System.Drawing.CharacterRange(0, text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            if (regions.Length > 0)
            {
                rect = regions[0].GetBounds(graphics);
            }

            return rect;
        }

        static public int MeasureDisplayStringWidth(RectangleF rect)
        {
            return (int)(rect.Right + 1.0f);
        }
    }
}
