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
        private Pango.FontDescription DefaultFont = new Pango.FontDescription();

        private Line CurrentLine = null;
                    
        private List<Line> Lines = new List<Line>();

        /// <summary>
        /// Remove a line at specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="redraw"></param>
        public void RemoveLine(int index, bool redraw = false)
        {
            lock (Lines)
            {
                if (Lines.Count > (index - 2))
                {
                    Lines.RemoveAt(index);
                }
            }
            if (redraw)
            {
                Redraw();
            }
        }

        private void DrawLineToBuffer(Line line, TextBuffer tb)
        {
            Gtk.TextIter iter = tb.EndIter;
            lock (line.text)
            {
                foreach (ContentText text in line.text)
                {
                    DrawPart(text, tb);
                }
            }
            iter = tb.EndIter;
            tb.Insert(ref iter, Environment.NewLine);
        }

        private void DrawLine(Line line)
        {
            DrawLineToBuffer(line, richTextBox.Buffer);
        }

        private void DrawPart(Line line)
        {
            lock (line.text)
            {
                foreach (ContentText text in line.text)
                {
                    DrawPart(text);
                }
            }
        }

        private void DrawPart(ContentText text, TextBuffer tb = null)
        {
            if (tb == null)
            {
                tb = richTextBox.Buffer;
            }

            Gtk.TextIter iter = tb.EndIter;

            TextTag format = new TextTag(null);

            if (text.Bold)
            {
                format.Weight = Pango.Weight.Bold;
                format.Underline = Pango.Underline.Double;
            }
            
            if (text.Underline)
            {
                format.Underline = Pango.Underline.Single;
            }

            if (text.Italic)
            {
                format.Style = Pango.Style.Italic;
                format.BackgroundGdk = Core.FromColor(text.TextColor);
                format.ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.backgroundcolor);
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
            if (!text.Italic)
            {
                format.ForegroundGdk = Core.FromColor(text.TextColor);
            }
            tb.TagTable.Add(format);
            format.FontDesc = DefaultFont;
            format.SizePoints = Configuration.CurrentSkin.fontsize;
            tb.InsertWithTags(ref iter, text.Text, format);
        }

        private void DrawNewline()
        {
            TextIter endi = richTextBox.Buffer.EndIter;
            richTextBox.Buffer.Insert(ref endi, Environment.NewLine);
        }

        private void DrawLineToHead(Line line)
        {
            Gtk.TextIter iter = richTextBox.Buffer.StartIter;
            lock (line.text)
            {
                foreach (ContentText text in line.text)
                {
                    TextTag format = new TextTag(null);

                    if (text.Bold)
                    {
                        format.Weight = Pango.Weight.Bold;
                        format.Underline = Pango.Underline.Double;
                    }

                    if (text.Underline)
                    {
                        format.Underline = Pango.Underline.Single;
                    }

                    if (text.Italic)
                    {
                        format.Style = Pango.Style.Italic;
                        format.BackgroundGdk = Core.FromColor(text.TextColor);
                        format.ForegroundGdk = Core.FromColor(Configuration.CurrentSkin.backgroundcolor);
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
                    if (!text.Italic)
                    {
                        format.ForegroundGdk = Core.FromColor(text.TextColor);
                    }
                    richTextBox.Buffer.TagTable.Add(format);
                    format.FontDesc = DefaultFont;
                    format.SizePoints = Configuration.CurrentSkin.fontsize;
                    richTextBox.Buffer.InsertWithTags(ref iter, text.Text, format);
                }
            }
            richTextBox.Buffer.Insert(ref iter, Environment.NewLine);
        }

        private void LinkRm(object sender, TextEventArgs e)
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
        
        private void LinkHandler(object sender, TextEventArgs e)
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

        /// <summary>
        /// Remove all text from box
        /// </summary>
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
            Core.Profiler profiler = null;
            if (Configuration.Kernel.Profiler)
            {
                profiler = new Core.Profiler("Redraw()");
            }
            lock (richTextBox)
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
            if (Configuration.Kernel.Profiler)
            {
                profiler.Done();
            }
        }
    }
}
