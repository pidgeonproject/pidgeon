using System;
using System.Collections.Generic;
using System.Text;
using Client;

namespace Client
{
    class PidgeonNotice : Client.Extension
    {
        Graphics.Window collector = null;
        Gtk.MenuItem menu;

        public override void  Hook_Initialise(Forms.Main main)
        {
            menu = new Gtk.MenuItem("Display notifications");
            collector = new Graphics.Window();
            main.CreateChat(collector, null, false);
            menu.Activated += new EventHandler(Display);
            collector.WindowName = "Notifications";
            main.ToolsMenu.Append(menu);
            menu.Show();
        }

        public override bool Hook_NotificationDisplay(string text, ContentLine.MessageStyle InputStyle, ref bool WriteLog, long Date, ref bool SuppressPing)
        {
            collector.scrollback.InsertText(text, InputStyle, false, Date, true);
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
                    Core._Main.SwitchWindow(collector);
                    return;
                }
                collector.Visible = true;
                Core._Main.SwitchWindow(collector);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public override void Initialise()
        {
            Name = "Notification collector";
            Version = "1.0.0";
            Description = "This allows you to collect all notification you get";
            base.Initialise();
        }
    }
}
