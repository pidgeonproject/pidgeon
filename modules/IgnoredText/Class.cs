using System;
using System.Collections.Generic;
using System.Text;
using Client;

namespace Client
{
    class IgnoredText : Client.Extension
    {
        Graphics.Window collector = null;
        Gtk.MenuItem menu;

        public override void Initialise()
        {
            Name = "Ignored text";
            Version = "1.0.0";
            Description = "This allows you to display all ignored text in a separate window";
            base.Initialise();
        }

        public override void Hook_Initialise(Forms.Main main)
        {
            menu = new Gtk.MenuItem("Display ignored text");
            collector = new Graphics.Window();
            collector.CreateChat(null, false, false, false);
            menu.Activated += new EventHandler(Display);
            collector.WindowName = "Ignored";
            main.ToolsMenu.Append(menu);
            menu.Show();
        }

        public override bool Hook_BeforeIgnore(Extension.MessageArgs _IgnoreArgs)
        {
            if (_IgnoreArgs.user == null)
            {
                collector.scrollback.InsertText("{" + _IgnoreArgs.window.WindowName + "} " + _IgnoreArgs.text, ContentLine.MessageStyle.Message, false, _IgnoreArgs.date, true);
                return true;
            }
            collector.scrollback.InsertText("{" + _IgnoreArgs.window.WindowName + "} " + _IgnoreArgs.window._Network._Protocol.PRIVMSG(_IgnoreArgs.user.Nick, _IgnoreArgs.text), ContentLine.MessageStyle.Message, false, _IgnoreArgs.date, true);
            return true;
        }

        private void Display(object s, EventArgs e)
        {
            try
            {
                if (Core.network != null)
                {
                    Core.network.RenderedChannel = null;
                    Core.network._Protocol.Current = collector;
                    Core.SystemForm.SwitchWindow(collector);
                    return;
                }
                collector.Visible = true;
                Core.SystemForm.SwitchWindow(collector);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
