using System;
using System.Collections.Generic;

namespace Client
{
    public class RestrictedModule : Extension
    {
        public Gtk.MenuItem item = null;
        public Gtk.SeparatorMenuItem separator = null;
        private Forms.Main _m = null;


        public override void Initialise()
        {
            Name = "Help";
            Description = "This plugin enable you to open #pidgeon channel from help menu";
            Version = "1.2.0";
            base.Initialise();
        }

        public override bool Hook_Unload()
        {
            try
            {
                if (_m == null)
                {
                    Core.DebugLog("No handle to main");
                    return false;
                }

                _m.HelpMenu.Remove(separator);
                _m.HelpMenu.Remove(item);
                return true;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
                return false;
            }
        }

        public override void Hook_Initialise(Forms.Main main)
        {
            _m = main;
            item = new Gtk.MenuItem("#pidgeon");
            item.Activated += new EventHandler(pidgeonToolStripMenuItem_Click);
            separator = new Gtk.SeparatorMenuItem();
            main.HelpMenu.Append(separator);
            main.HelpMenu.Append(item);
            separator.Show();
            item.Show();
            Core.DebugLog("Registered #pidgeon in menu");
        }

        private void pidgeonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Protocol network in Core.Connections)
            {
                if (network.GetType() == typeof(ProtocolSv))
                {
                    ProtocolSv sv = (ProtocolSv)network;
                    foreach (Network server in sv.NetworkList)
                    {
                        if (server.ServerName == "irc.tm-irc.org")
                        {
                            server.Join("#pidgeon");
                            return;
                        }
                    }
                }

                if (network.Server == "irc.tm-irc.org")
                {
                    network.Join("#pidgeon");
                    return;
                }
            }
            Core.connectIRC("irc.tm-irc.org");
            foreach (Protocol network in Core.Connections)
            {
                if (network.Server == "irc.tm-irc.org")
                {
                    network.Join("#pidgeon");
                    return;
                }
            }
        }
    }
}
