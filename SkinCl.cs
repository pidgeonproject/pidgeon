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
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Data;

namespace Pidgeon
{
    /// <summary>
    /// Skin
    /// </summary>
    public class Skin : IDisposable
    {
        /// <summary>
        /// Icon of at symbol used for PM
        /// </summary>
        public Gdk.Pixbuf Icon_ShadowAt = Gdk.Pixbuf.LoadFromResource("Pidgeon.Resources.at-s.png");
        /// <summary>
        /// Icon #
        /// </summary>
        public Gdk.Pixbuf Icon_ShadowHash = Gdk.Pixbuf.LoadFromResource("Pidgeon.Resources.hash-s.png");
        /// <summary>
        /// Icon !
        /// </summary>
        public Gdk.Pixbuf Icon_ShadowMark = Gdk.Pixbuf.LoadFromResource("Pidgeon.Resources.exclamation-mark-s.png");
        /// <summary>
        /// Icon @
        /// </summary>
        public Gdk.Pixbuf Icon_At = Gdk.Pixbuf.LoadFromResource("Pidgeon.Resources.at.png");
        /// <summary>
        /// Icon #
        /// </summary>
        public Gdk.Pixbuf Icon_Hash = Gdk.Pixbuf.LoadFromResource("Pidgeon.Resources.icon_hash.png");
        /// <summary>
        /// Icon !
        /// </summary>
        public Gdk.Pixbuf Icon_ExclamationMark = Gdk.Pixbuf.LoadFromResource("Pidgeon.Resources.exclamation mark.png");
        /// <summary>
        /// Local font name or family
        /// </summary>
        public string LocalFont = "Arial";
        /// <summary>
        /// This is a string that /me is prefixed with
        /// </summary>
        public string Message2 = ">>>>>>";
        /// <summary>
        /// Font size
        /// </summary>
        public int FontSize = 9000;
        /// <summary>
        /// Size of channel list
        /// </summary>
        public int ChannelListFontSize = 12000;
        /// <summary>
        /// Size of user list
        /// </summary>
        public int UserListFontSize = 9000;
        /// <summary>
        /// Join color
        /// </summary>
        public System.Drawing.Color JoinColor;
        /// <summary>
        /// Kick color
        /// </summary>
        public System.Drawing.Color KickColor;
        /// <summary>
        /// Channel modes and actions
        /// </summary>
        public System.Drawing.Color MiscelancsColor;
        /// <summary>
        /// Hl
        /// </summary>
        public System.Drawing.Color HighlightColor;
        /// <summary>
        /// Nick change color
        /// </summary>
        public System.Drawing.Color changenickcolor;
        /// <summary>
        /// This is a defaul regular color for text
        /// </summary>
        public System.Drawing.Color FontColor;
        /// <summary>
        /// +h
        /// </summary>
        public System.Drawing.Color ColorH;
        /// <summary>
        /// Default color for system menus
        /// </summary>
        public System.Drawing.Color ColorDefault;
        /// <summary>
        /// +v
        /// </summary>
        public System.Drawing.Color ColorV;
        /// <summary>
        /// +q
        /// </summary>
        public System.Drawing.Color ColorQ;
        /// <summary>
        /// +a
        /// </summary>
        public System.Drawing.Color ColorA;
        /// <summary>
        /// +o
        /// </summary>
        public System.Drawing.Color ColorO;
        /// <summary>
        /// Bg
        /// </summary>
        public System.Drawing.Color BackgroundColor;
        /// <summary>
        /// Color
        /// </summary>
        public System.Drawing.Color ColorTalk;
        /// <summary>
        /// Color
        /// </summary>
        public System.Drawing.Color ColorAway;
        /// <summary>
        /// Skin
        /// </summary>
        public string Name = "Default";
        /// <summary>
        /// Link
        /// </summary>
        public System.Drawing.Color LinkColor;
        /// <summary>
        /// mIRC colors
        /// </summary>
        public List<System.Drawing.Color> mrcl = new List<System.Drawing.Color>();
        private bool disposed = false;

        private static Color colorFromXmlCode(XmlNode code)
        {
            Color color = Color.Black;
            try
            {
                color = System.Drawing.ColorTranslator.FromHtml(code.InnerText);
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
            return color;
        }

        /// <summary>
        /// Change a current skin to skin of another name
        /// </summary>
        /// <param name="path"></param>
        public static void ReloadSkin(string path)
        {
            lock (Configuration.SL)
            {
                foreach (Skin skin in Configuration.SL)
                {
                    if (skin.Name == path)
                    {
                        Configuration.CurrentSkin = skin;
                        return;
                    }
                }
            }
            Core.DebugLog("No such a skin is loaded: " + path);
        }

        /// <summary>
        /// Releases all resources used by this class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this class
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Icon_ShadowHash.Dispose();
                    Icon_ShadowMark.Dispose();
                    Icon_At.Dispose();
                    Icon_ExclamationMark.Dispose();
                    Icon_Hash.Dispose();
                    Icon_ShadowAt.Dispose();
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Creates a new skin
        /// </summary>
        /// <param name="path"></param>
        public Skin(string path)
        {
            Core.DebugLog("Loading skin " + path);
            Defaults();
            string _p = path;
            Name = path;
            if (Name.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                Name = Name.Substring(Name.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) + 1);
            }
            if (File.Exists(_p))
            {
                XmlDocument configuration = new XmlDocument();
                configuration.Load(_p);
                mrcl.Clear();
                foreach (XmlNode curr in configuration.ChildNodes[0].ChildNodes)
                {
                    switch (curr.Name.ToLower())
                    { 
                        case "fontcolor":
                            FontColor = colorFromXmlCode(curr);
                            break;
                        case "link":
                            LinkColor = colorFromXmlCode(curr);
                            break;
                        case "colordefault":
                            ColorDefault = colorFromXmlCode(curr);
                            break;
                        case "joincolor":
                            JoinColor = colorFromXmlCode(curr);
                            break;
                        case "miscelancscolor":
                            MiscelancsColor = colorFromXmlCode(curr);
                            break;
                        case "highlightcolor":
                            HighlightColor = colorFromXmlCode(curr);
                            break;
                        case "changenickcolor":
                            changenickcolor = colorFromXmlCode(curr);
                            break;
                        case "backgroundcolor":
                            BackgroundColor = colorFromXmlCode(curr);
                            break;
                        case "colortalk":
                            ColorTalk = colorFromXmlCode(curr);
                            break;
                        case "fontname":
                            LocalFont = curr.InnerText;
                            break;
                        case "fontsize":
                        case "size":
                            FontSize = int.Parse(curr.InnerText);
                            break;
                        case "coloraway":
                            this.ColorAway = colorFromXmlCode(curr);
                            break;
                        case "colorq":
                            this.ColorQ = colorFromXmlCode(curr);
                            break;
                        case "colorv":
                            this.ColorV = colorFromXmlCode(curr);
                            break;
                        case "colora":
                            this.ColorA = colorFromXmlCode(curr);
                            break;
                        case "colorh":
                            this.ColorH = colorFromXmlCode(curr);
                            break;
                        case "coloro":
                            this.ColorO = colorFromXmlCode(curr);
                            break;
                        case "mirc":
                            this.mrcl.Add(colorFromXmlCode(curr));
                            break;
                        case "message":
                            this.Message2 = curr.InnerText;
                            break;
                        case "channelsize":
                            this.ChannelListFontSize = int.Parse(curr.InnerText);
                            break;
                        case "usersize":
                            this.UserListFontSize = int.Parse(curr.InnerText);
                            break;
                        case "icon_at":
                            this.Icon_At = new Gdk.Pixbuf(Core.SystemBinRootPath + curr.InnerText);
                            break;
                        case "icon_exclamationmark":
                            this.Icon_ExclamationMark = new Gdk.Pixbuf(Core.SystemBinRootPath + curr.InnerText);
                            break;
                        case "icon_shadowmark":
                            this.Icon_ShadowMark = new Gdk.Pixbuf(Core.SystemBinRootPath + curr.InnerText);
                            break;
                        case "icon_shadowhash":
                            this.Icon_ShadowHash = new Gdk.Pixbuf(Core.SystemBinRootPath + curr.InnerText);
                            break;
                        case "icon_shadowat":
                            this.Icon_ShadowAt = new Gdk.Pixbuf(Core.SystemBinRootPath + curr.InnerText);
                            break;
                        case "icon_hash":
                            this.Icon_Hash = new Gdk.Pixbuf(Core.SystemBinRootPath + curr.InnerText);
                            break;
                    }
                }
            }
            if (mrcl.Count < 15)
            {
                throw new PidgeonException("The skin doesn't contain all required mirc colors");
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
            FontColor = System.Drawing.Color.White;
            JoinColor = System.Drawing.Color.LightBlue;
            HighlightColor = System.Drawing.Color.LightPink;
            MiscelancsColor = System.Drawing.Color.LightGreen;
            changenickcolor = System.Drawing.Color.LightSteelBlue;
            LinkColor = System.Drawing.Color.LightBlue;
            ColorTalk = System.Drawing.Color.Yellow;
            KickColor = System.Drawing.Color.White;
            ColorQ = System.Drawing.Color.LightBlue;
            ColorA = System.Drawing.Color.LightCyan;
            ColorO = System.Drawing.Color.Yellow;
            ColorH = System.Drawing.Color.Cyan;
            ColorV = System.Drawing.Color.LightGreen;
            ColorDefault = System.Drawing.Color.White;
            ColorAway = System.Drawing.Color.Gray;
            BackgroundColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Creates a new skin
        /// </summary>
        public Skin()
        {
            Defaults();
        }

        /// <summary>
        /// Save a skin
        /// </summary>
        public void Save()
        {
            
        }

        /// <summary>
        /// LoadSkin
        /// </summary>
        /// <param name="name"></param>
        public static void LoadSkin(string name)
        {
            Skin skin = new Skin(name);
            lock (Configuration.SL)
            {
                Configuration.SL.Add(skin);
            }
        }
    }
}
