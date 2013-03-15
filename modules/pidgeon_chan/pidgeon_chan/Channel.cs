using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;

namespace Client
{
    public class RestrictedModule : Extension
    {
        public override void Initialise()
        {
            Name = "Help";
            Description = "This plugin enable you to open #pidgeon channel from help menu";
            Version = "1.0.6";
            base.Initialise();
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
                        if (server. == "irc.tm-irc.org")
                        {
                            server._Protocol.Join("#pidgeon", server);
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
