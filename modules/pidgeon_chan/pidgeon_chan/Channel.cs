using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;

namespace Client
{
    public class RestrictedModule : Extension
    {
        public ToolStripMenuItem item = null;
        public ToolStripSeparator separator = null;
        private Main _m = null;


        public override void Initialise()
        {
            Name = "Help";
            Description = "This plugin enable you to open #pidgeon channel from help menu";
            Version = "1.0.6";
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
                lock (_m.helpToolStripMenuItem.DropDownItems)
                {
                    if (_m.helpToolStripMenuItem.DropDownItems.Contains(item))
                    {
                        _m.helpToolStripMenuItem.DropDownItems.Remove(item);
                    }
                    if (_m.helpToolStripMenuItem.DropDownItems.Contains(separator))
                    {
                        _m.helpToolStripMenuItem.DropDownItems.Remove(separator);
                    }
                }
                _m.menuStrip1.ResumeLayout(false);
                _m.menuStrip1.PerformLayout();
                return true;
            }
            catch (Exception fail)
            {
                Core.DebugLog(fail.ToString());
                return false;
            }
        }

        public override void Hook_Initialise(Main main)
        {
            _m = main;
            item = new ToolStripMenuItem("#pidgeon");
            item.Size = new System.Drawing.Size(200, 22);
            item.Click += new EventHandler(pidgeonToolStripMenuItem_Click);
            separator = new ToolStripSeparator();
            separator.Size = new System.Drawing.Size(200, 22);
            main.helpToolStripMenuItem.DropDownItems.Add(separator);
            main.helpToolStripMenuItem.DropDownItems.Add(item);
            _m.menuStrip1.ResumeLayout(false);
            _m.menuStrip1.PerformLayout();
        }

        private void pidgeonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Protocol network in Core.Connections)
            {
                if (network.ProtocolType == 2)
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
