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
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Data;

namespace Client
{
    public class Skin
    {
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
        public System.Drawing.Color colorh;
        public System.Drawing.Color colordefault;
        public System.Drawing.Color colorv;
        public System.Drawing.Color colorq;
        public System.Drawing.Color colora;
        public System.Drawing.Color coloro;
        public System.Drawing.Color backgroundcolor;
        public System.Drawing.Color othercolor;
        public System.Drawing.Color colortalk;
        public string name = "Default";
        public System.Drawing.Color link;
        public List<System.Drawing.Color> mrcl = new List<System.Drawing.Color>();
        public bool italic;

        public Color colorFromXmlCode(XmlNode code)
        {
            Color color = Color.Black;
            try
            {
                if (code.Attributes.Count > 3)
                {
                    color = Color.FromArgb(int.Parse(code.Value));
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return color;
        }

        public Skin(string path)
        {
            Core.DebugLog("Loading skin " + path);
            Defaults();
            name = path;
            if (File.Exists(path))
            {
                XmlDocument configuration = new XmlDocument();
                configuration.Load(path);
                foreach (XmlNode curr in configuration.ChildNodes[0].ChildNodes)
                {
                    switch (curr.Name)
                    { 
                        case "fontcolor":
                            fontcolor = colorFromXmlCode(curr);
                            break;
                        case "link":
                            link = colorFromXmlCode(curr);
                            break;
                        case "joincolor":
                            joincolor = colorFromXmlCode(curr);
                            break;
                        case "miscelancscolor":
                            miscelancscolor = colorFromXmlCode(curr);
                            break;
                        case "highlightcolor":
                            highlightcolor = colorFromXmlCode(curr);
                            break;
                        case "selfcolor":
                            selfcolor = colorFromXmlCode(curr);
                            break;
                        case "changenickcolor":
                            changenickcolor = colorFromXmlCode(curr);
                            break;
                        case "backgroundcolor":
                            backgroundcolor = colorFromXmlCode(curr);
                            break;
                        case "othercolor":
                            othercolor = colorFromXmlCode(curr);
                            break;
                        case "colortalk":
                            colortalk = colorFromXmlCode(curr);
                            break;
                        case "fontname":
                            localfont = curr.Value;
                            break;
                    }

                }
            }
        }

        private void Defaults()
        {
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
            colorq = System.Drawing.Color.AliceBlue;
            colora = System.Drawing.Color.Azure;
            coloro = System.Drawing.Color.Yellow;
            colorh = System.Drawing.Color.Cyan;
            colorv = System.Drawing.Color.LightGreen;
            colordefault = System.Drawing.Color.White;
            fontsize = 11;
            localfont = "Arial";
            backgroundcolor = System.Drawing.Color.Black;
        }

        public Skin()
        {
            Defaults();
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
