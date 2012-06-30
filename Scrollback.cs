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
using System.Linq;
using System.Threading;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;



namespace Client
{
    public partial class Scrollback : UserControl
    {
        private List<ContentLine> Line = new List<ContentLine>();
        public TextBox Last;
        public class ContentLine
        {
            public ContentLine(MessageStyle _style, string Text)
            {
                style = _style;
                time = DateTime.Now;
                text = Text;
            }
            public DateTime time;
            public string text;
            public MessageStyle style;
        }
        public bool Modified;

        public void Recreate(object sender, EventArgs e)
        {
            Data.Width = this.Width;
            Data.Height = this.Height;
        }


        public Scrollback()
        {
            InitializeComponent();
        }

        public enum MessageStyle
        {
            System,
            Message,
            Action,
            User,
            Channel,
            Kick,
            Join,
            Part,
        }


        public bool InsertText(string text, MessageStyle _style)
        {
            lock(Line)
            {
                Line.Add(new ContentLine(_style, text));
            }
            Modified = true;
            return false;
        }

        private void Scrollback_Load(object sender, EventArgs e)
        {
            Reload();
            Recreate(null, null);
        }

        public void Reload()
        {
            Modified = false;
            string text = "<html><head></head><body STYLE=\"background-color: " + Configuration.CurrentSkin.backgroundcolor.Name + "\"><script>javascript:var s = function() { window.scrollBy(0,100000); setTimeout(s, 10); }; s();</script>";
            lock (Line)
            {
                foreach (ContentLine _c in Line)
                {
                    text += "<font size=\"" + Configuration.CurrentSkin.fontsize.ToString() + "px\" color=\"" + Configuration.CurrentSkin.fontcolor.Name + "\" face=" + Configuration.CurrentSkin.localfont + ">[" + _c.time.ToShortTimeString() + "] " +  System.Web.HttpUtility.HtmlEncode(_c.text) + "</font><br>";
                }
            }
            text += "</body>";

            Data.DocumentText = text;
            //Data.Refresh();

            //Data.Document.Window.ScrollTo(0, Data.Document.Body.ScrollRectangle.Height);
            

            
        }

        private void Data_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void refresh_Tick(object sender, EventArgs e)
        {
            if (Modified)
            {
                Reload();
            }
        }

    }
}
