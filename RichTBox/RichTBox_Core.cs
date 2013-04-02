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
        public Scrollback scrollback = null;
        private Font font;
        private double textSize = 2;
        private string text = null;
        private System.Drawing.Color foreColor;
        private System.Drawing.Color backColor;

        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        private global::Gtk.TextView richTextBox;

        public Gtk.TextView textView
        {
            get
            {
                return richTextBox;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
        }

        public System.Drawing.Color BackColor
        {
            set
            {
                backColor = value;
                colors();
            }
            get
            {
                return backColor;
            }
        }

        public System.Drawing.Color ForeColor
        {
            set
            {
                foreColor = value;
                colors();
            }
            get
            {
                return foreColor;
            }
        }

        private void colors()
        {
            richTextBox.ModifyBase(StateType.Normal, Core.fromColor(BackColor));
            richTextBox.ModifyText(StateType.Normal, Core.fromColor(ForeColor));
        }

        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget Client.Graphics.TextBox
            global::Stetic.BinContainer.Attach(this);
            this.Name = "Client.Graphics.TextBox";
            // Container child Client.Graphics.TextBox.Gtk.Container+ContainerChild
            this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
            this.GtkScrolledWindow.Name = "GtkScrolledWindow";
            this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
            // Container child GtkScrolledWindow.Gtk.Container+ContainerChild
            this.richTextBox = new global::Gtk.TextView();
            this.richTextBox.CanFocus = true;
            this.richTextBox.DoubleBuffered = true;
            this.richTextBox.Editable = false;
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.AcceptsTab = true;
            richTextBox.WrapMode = WrapMode.Word;
            this.GtkScrolledWindow.Add(this.richTextBox);
            this.Add(this.GtkScrolledWindow);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.Hide();
        }

        public RichTBox()
        {
            this.Build();
            ForeColor = Configuration.CurrentSkin.colordefault;
            BackColor = Configuration.CurrentSkin.backgroundcolor;
        }

        public void InsertLine(Line line)
        {
            if (line == null)
            {
                throw new Exception("You can't insert null to text box");
            }
            lock (Lines)
            {
                Lines.Add(line);
            }
            DrawLine(line);
        }

        [Obsolete]
        public void RedrawText()
        {
            Redraw();
        }

        public void ScrollToBottom()
        {
            TextIter iter = richTextBox.Buffer.EndIter;
            richTextBox.ScrollToIter(iter, 0, true, 0, 0);
        }
    }
}
