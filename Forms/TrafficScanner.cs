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
    public partial class TrafficScanner : Form
    {
        bool modified = true;
        List<string> text = new List<string>();
        public TrafficScanner()
        {
            InitializeComponent();
        }

        public void Stop(object O, FormClosingEventArgs L)
        {
            Visible = false;
            L.Cancel = true;
        }

        public bool insert(string server, string data)
        {
            modified = true;
            if (!Configuration.NetworkSniff)
            {
                return true;
            }
            text.Add(server + " " + data);
            return true;
        }

        private void refresh_Tick(object sender, EventArgs e)
        {
            if (Visible && modified)
            {
                modified = false;
                textBox1.Lines = text.ToArray<string>();
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }
        }
    }
}
