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
using System.Text;

namespace Client
{
    public class Skin
    {
        public static List<Skin> LoadedSkins = new List<Skin>();
        public System.Xml.XmlDocument data = new System.Xml.XmlDocument();
        public string localfont;
        public float fontsize;
        public System.Drawing.Color joincolor;
        public System.Drawing.Color kickcolor;
        public System.Drawing.Color miscelancscolor;
        public System.Drawing.Color highlightcolor;
        public System.Drawing.Color selfcolor;
        public System.Drawing.Color changenickcolor;
        public System.Drawing.Color fontcolor;
        public System.Drawing.Color backgroundcolor;
        public System.Drawing.Color othercolor;
        public System.Drawing.Color colortalk;
        public System.Drawing.Color link;
        public List<System.Drawing.Color> mrcl = new List<System.Drawing.Color>();
        public bool italic;

        public Skin()
        {
            // defaults
            mrcl.Add(System.Drawing.Color.White);
            mrcl.Add(System.Drawing.Color.Gray);
            mrcl.Add(System.Drawing.Color.Blue);
            mrcl.Add(System.Drawing.Color.Green);
            mrcl.Add(System.Drawing.Color.Red);
            mrcl.Add(System.Drawing.Color.SandyBrown);
            mrcl.Add(System.Drawing.Color.Purple);
            mrcl.Add(System.Drawing.Color.Orange);
            mrcl.Add(System.Drawing.Color.Yellow);
            mrcl.Add(System.Drawing.Color.LightGreen);
            mrcl.Add(System.Drawing.Color.Cornsilk);
            mrcl.Add(System.Drawing.Color.Cyan);
            mrcl.Add(System.Drawing.Color.LightCoral);
            mrcl.Add(System.Drawing.Color.LightPink);
            mrcl.Add(System.Drawing.Color.Gray);
            mrcl.Add(System.Drawing.Color.LightGoldenrodYellow);
            fontcolor = System.Drawing.Color.White;
            joincolor = System.Drawing.Color.LightBlue;
            highlightcolor = System.Drawing.Color.LightPink;
            miscelancscolor = System.Drawing.Color.LightGreen;
            selfcolor = System.Drawing.Color.LightGray;
            changenickcolor = System.Drawing.Color.LightSteelBlue;
            link = System.Drawing.Color.LightBlue;
            colortalk = System.Drawing.Color.Yellow;
            kickcolor = System.Drawing.Color.White;
            fontsize = 11;
            localfont = "Arial";
            backgroundcolor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// LoadSkin
        /// </summary>
        /// <param name="name"></param>
        public static void LoadSkin(string name)
        {
            
        }
    }
}
