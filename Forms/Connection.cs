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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Connection : Form
    {
        public Connection()
        {
            InitializeComponent();
        }

        private void Connection_Load(object sender, EventArgs e)
        {
            label5.Text = messages.get("nconnection-start-protocol", Core.SelectedLanguage);
            label6.Text = messages.get("nconnection-p", Core.SelectedLanguage);
            label4.Text = messages.get("nconnection-start-server", Core.SelectedLanguage);
            label3.Text = messages.get("nconnection-start-port", Core.SelectedLanguage);
            label2.Text = messages.get("nconnection-ident", Core.SelectedLanguage);
            label1.Text = messages.get("nconnection-name", Core.SelectedLanguage);
            textBox2.Text = "6667";
            textBox1.Text = Configuration.ident;
            _Nickname.Text = Configuration.nick;
            comboBox1.Items.Add("irc");
            comboBox1.SelectedItem = "irc";
            comboBox2.Text = Configuration.LastHostName;
            comboBox1.Items.Add("quassel");
            comboBox1.Items.Add("pidgeon service");
            Text  = messages.get("connection", Core.SelectedLanguage);
            bConnect.Text = messages.get("bconnect", Core.SelectedLanguage);
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            int port;
            if (textBox1.Text == "")
            {
                MessageBox.Show(messages.get("nconnection-2", Core.SelectedLanguage), "Missing params", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox2.Text == "" || !int.TryParse(textBox2.Text, out port))
            {
                MessageBox.Show(messages.get("nconnection-3", Core.SelectedLanguage), "Missing params", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_Nickname.Text == "")
            {
                MessageBox.Show(messages.get("nconnection-1", Core.SelectedLanguage), "Missing params", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Configuration.nick = _Nickname.Text;
            Configuration.ident = textBox1.Text;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Core.connectIRC(comboBox2.Text, port);
                    break;
                case 1:
                    Core.connectQl(comboBox2.Text, port);
                    break;
                case 2:
                    Core.connectPS(comboBox2.Text, port, textBox3.Text);
                    break;
            }
            Close();
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
