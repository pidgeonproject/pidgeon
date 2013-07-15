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
using System.Drawing;
using Gtk;

namespace Client.Graphics
{
    /// <summary>
    /// Text box
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class TextBox : Gtk.Bin
    {
        /// <summary>
        /// History
        /// </summary>
        public List<string> history = null;
        /// <summary>
        /// Position
        /// </summary>
        public int position = 0;
        /// <summary>
        /// Previous text
        /// </summary>
        public string prevtext = "";
        private string original = "";
        /// <summary>
        /// Parent
        /// </summary>
        public Window parent = null;
        private bool restore = false;
        private global::Gtk.ScrolledWindow GtkScrolledWindow;
        /// <summary>
        /// Pointer
        /// </summary>
        public global::Gtk.TextView richTextBox;
        private bool destroyed = false;
        private string buffer = null;
        private bool Processing = false;

        /// <summary>
        /// This is a buffer containing the current content of input box
        /// </summary>
        public string Buffer
        {
            get
            {
                if (!Processing)
                {
                    return richTextBox.Buffer.Text;
                }
                return buffer;
            }
            set
            {
                if (!Processing)
                {
                    richTextBox.Buffer.Text = value;
                }
                buffer = value;
            }
        }

        /// <summary>
        /// This will return true in case object was requested to be disposed
        /// you should never work with objects that return true here
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return destroyed;
            }
        }

        /// <summary>
        /// This is a pointer to internal text box for compatibility reasons
        /// </summary>
        public Gtk.TextView richTextBox1
        {
            get
            {
                return richTextBox;
            }
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public TextBox()
        {

        }

        private void Build()
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
            this.richTextBox.WrapMode = WrapMode.Word;
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.AcceptsTab = true;
            this.GtkScrolledWindow.Add(this.richTextBox);
            this.Add(this.GtkScrolledWindow);
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
        }

        private void InitStyle()
        {
            richTextBox.ModifyBase(StateType.Normal, Core.FromColor(Configuration.CurrentSkin.backgroundcolor));
            richTextBox.ModifyText(StateType.Normal, Core.FromColor(Configuration.CurrentSkin.colordefault));
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                prevtext = richTextBox1.Buffer.Text;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        [GLib.ConnectBefore]
        private void _Enter(object sender, KeyPressEventArgs e)
        {
            try
            {
                Hooks._Sys.Poke();
                if (Forms.Main.ShortcutHandle(sender, e))
                {
                    e.RetVal = true;
                    return;
                }

                bool control = false;

                if (e.Event.State == Gdk.ModifierType.ShiftMask ||
                e.Event.State == Gdk.ModifierType.ControlMask ||
                e.Event.State == Gdk.ModifierType.Mod1Mask ||
                e.Event.State == Gdk.ModifierType.SuperMask)
                {
                    switch (e.Event.Key)
                    {
                        case Gdk.Key.KP_Enter:
                        case Gdk.Key.ISO_Enter:
                        case Gdk.Key.Up:
                        case Gdk.Key.Down:
                            return;
                    }
                    if (e.Event.KeyValue == 65293)
                    {
                        return;
                    }
                    if (e.Event.State == Gdk.ModifierType.ControlMask)
                    {
                        control = true;
                    }
                }

                // enter
                if (e.Event.KeyValue == 65293)
                {
                    Processing = true;
                    List<string> input = new List<string>();
                    if (richTextBox1.Buffer.Text.Contains("\n"))
                    {
                        input.AddRange(richTextBox1.Buffer.Text.Split('\n'));
                        foreach (var line in input)
                        {
                            string Line = line;
                            if (Line.Contains("\r"))
                            {
                                Line = Line.Replace("\r", "");
                            }
                            Parser.parse(Line, parent);
                            if (Line != "")
                            {
                                lock (history)
                                {
                                    while (history.Count > Configuration.Window.history)
                                    {
                                        history.RemoveAt(0);
                                    }
                                    history.Add(Line);
                                }
                            }
                        }
                        original = "";
                        richTextBox1.Buffer.Text = "";
                        position = history.Count;
                        Processing = false;
                        e.RetVal = true;
                        return;
                    }

                    richTextBox1.Buffer.Text = richTextBox1.Buffer.Text.Replace("\n", "");

                    if (richTextBox1.Buffer.Text != "")
                    {
                        Parser.parse(richTextBox1.Buffer.Text);
                        history.Add(richTextBox1.Buffer.Text);
                    }
                    original = "";
                    richTextBox1.Buffer.Text = "";
                    if (buffer != null)
                    {
                        richTextBox.Buffer.Text = buffer;
                        buffer = null;
                    }
                    position = history.Count;
                    e.RetVal = true;
                    return;
                }

                if (!(e.Event.Key == Gdk.Key.Tab))
                { restore = false; }
                switch (e.Event.Key)
                {
                    case Gdk.Key.Down:
                        if (position == history.Count)
                        {
                            return;
                        }
                        int max = position + 1;
                        if (history.Count <= max)
                        {
                            position = history.Count;
                            richTextBox1.Buffer.Text = original.Replace("\n", "");
                            return;
                        }
                        position++;
                        if (position >= history.Count)
                        {
                            position = history.Count - 1;
                        }
                        richTextBox1.Buffer.Text = history[position];
                        e.RetVal = true;
                        break;
                    case Gdk.Key.b:
                    case Gdk.Key.B:
                        if (control)
                        {
                            richTextBox1.Buffer.Text += (((char)002).ToString());
                            e.RetVal = true;
                            return;
                        }
                        break;
                    case Gdk.Key.U:
                    case Gdk.Key.u:
                        if (control)
                        {
                            richTextBox1.Buffer.Text += (((char)001).ToString());
                            e.RetVal = true;
                            return;
                        }
                        break;
                    case Gdk.Key.R:
                    case Gdk.Key.r:
                        if (control)
                        {
                            richTextBox1.Buffer.Text += (((char)0016).ToString());
                            e.RetVal = true;
                            return;
                        }
                        break;
                    case Gdk.Key.k:
                    case Gdk.Key.K:
                        if (control)
                        {
                            richTextBox1.Buffer.Text += (((char)003).ToString());
                            e.RetVal = true;
                            return;
                        }
                        break;
                    case Gdk.Key.Up:
                        if (position < 1)
                        {
                            return;
                        }
                        if (history.Count == position)
                        {
                            original = richTextBox1.Buffer.Text;
                        }
                        position = position - 1;
                        richTextBox1.Buffer.Text = history[position];
                        e.RetVal = true;
                        return;
                    case Gdk.Key.Tab:
                        int caret = richTextBox1.Buffer.CursorPosition;
                        string data = richTextBox1.Buffer.Text;
                        Hooks._Scrollback.TextTab(ref restore, ref data, ref prevtext, ref caret);
                        richTextBox1.Buffer.Text = data;
                        restore = true;
                        richTextBox1.Buffer.Text = prevtext;
                        TextIter position2 = richTextBox.Buffer.GetIterAtOffset(caret);
                        richTextBox.Buffer.PlaceCursor(position2);
                        e.RetVal = true;
                        break;
                }
            }
            catch (Exception fail)
            {
                Processing = false;
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Destroy
        /// </summary>
        public void _Destroy()
        {
            if (IsDestroyed)
            {
                return;
            }
            destroyed = true;
            history.Clear();
            parent = null;
            this.Destroy();
        }

        /// <summary>
        /// Init
        /// </summary>
        public void Init()
        {
            this.Build();
            this.InitStyle();
            richTextBox.Buffer.Changed += new EventHandler(richTextBox1_TextChanged);
            richTextBox.KeyPressEvent += new KeyPressEventHandler(_Enter);
            if (history == null)
            {
                history = new List<string>();
            }
        }

        /// <summary>
        /// Focus text
        /// </summary>
        public void setFocus()
        {
            this.richTextBox.GrabFocus();
        }
    }
}

