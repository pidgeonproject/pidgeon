

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
    public partial class Connection : Form
    {
        public Connection()
        {
            InitializeComponent();
        }

        private void Connection_Load(object sender, EventArgs e)
        {
            label1.Text = messages.get("connection.1", Core.SelectedLanguage);
            Text  = messages.get("connection", Core.SelectedLanguage);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
