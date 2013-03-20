using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class NetworkDB : Form
    {
        public NetworkDB()
        {
            InitializeComponent();
        }

        private void NetworkDB_Load(object sender, EventArgs e)
        {
            try
            {
                messages.Localize(this);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
