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
        private List<Line> Lines = new List<Line>();

        public void RemoveLine(int index)
        {
            lock (Lines)
            {
                if (Lines.Count > (index - 2))
                {
                    Lines.RemoveAt(index);
                }
            }
        }

        private void DrawLine(Line line)
        { 
            Gtk.TextIter iter = richTextBox.Buffer.EndIter;
            lock (line.text)
            {
                foreach (ContentText text in line.text)
                {
                    TextTag format = new TextTag(null);
                    
                    if (text.Bold)
                    {
                        format.Weight = Pango.Weight.Bold;
                    }

                    if (text.Underline)
                    {
                        format.Underline = Pango.Underline.Single;
                    }

                    if (text.Italic)
                    { 
                        
                    }

                    if (text.Link != null)
                    {
                        format.TextEvent += new TextEventHandler(LinkHandler);
                    }
                    else
                    {
                        format.TextEvent += new TextEventHandler(LinkRm);
                    }

                    format.Data.Add("link", text.Link);
                    format.Data.Add("identifier", text.Text);
                    format.Font = Configuration.CurrentSkin.localfont;
                    format.ForegroundGdk = Core.fromColor(text.TextColor);
                    richTextBox.Buffer.TagTable.Add(format);
                    richTextBox.Buffer.InsertWithTags(ref iter, text.Text, format);
                    iter = richTextBox.Buffer.EndIter;
                }
            }
            richTextBox.Buffer.Insert(ref iter, Environment.NewLine);
        }

        public void LinkRm(object sender, TextEventArgs e)
        {
            try
            {
                if (e.Event.Type == Gdk.EventType.MotionNotify)
                {
					if (scrollback != null)
                    {
                        scrollback.SelectedLink = null;
                    }
                    if (Configuration.ChangingMouse)
                    {
                        textView.GetWindow(TextWindowType.Text).Cursor = new Gdk.Cursor(Gdk.CursorType.Arrow);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
		
        public void LinkHandler(object sender, TextEventArgs e)
        {
            try
            {
                string user = (string)((TextTag)sender).Data["identifier"];
                string link = (string)((TextTag)sender).Data["link"];
                if (e.Event.Type == Gdk.EventType.ButtonPress)
                {
                    if (scrollback != null)
                    {
                        scrollback.SelectedLink = link;
                        scrollback.SelectedUser = user;
                    }
                }

                if (e.Event.Type == Gdk.EventType.MotionNotify)
                {
                    if (Configuration.ChangingMouse)
                    {
                        textView.GetWindow(TextWindowType.Text).Cursor = new Gdk.Cursor(Gdk.CursorType.Hand1);
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void RemoveText()
        {
            lock (Lines)
            {
                Lines.Clear();
            }
            richTextBox.Buffer.Text = "";
        }

        private void Redraw()
        {
            richTextBox.Buffer.Text = "";
            lock (Lines)
            {
                foreach (Line curr in Lines)
                {
                    DrawLine(curr);
                }
            }
        }
    }
}
