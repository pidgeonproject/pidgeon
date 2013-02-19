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
    public partial class ScriptEdit : Form
    {
        public Network network = null;
        public ScriptEdit()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string text in textBox1.Lines)
                {
                    if (text != "")
                    {
                        network.Transfer(text, Configuration.Priority.High);
                    }
                }
            }
            catch(Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
