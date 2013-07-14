//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Recovery : Form
    {
        /// <summary>
        /// Creates new
        /// </summary>
        public Recovery()
        {
            InitializeComponent();
        }

        private void Recovery_Load(object sender, EventArgs e)
        {
            try
            {
                string ring = "";
                string inner = "";
                if (Core.recovery_exception.InnerException != null)
                {
                    inner = Core.recovery_exception.InnerException.ToString() + "\n\n";
                }
                foreach (string line in Core.RingBuffer)
                {
                    ring += line + "\n";
                }
                if (Core.RecoveryIsFatal)
                {
                    textBox1.Lines = ("YAY, we are terribly sorry, but pidgeon just crashed. This is unrecoverable exception, the application has to be terminated now. If you want to prevent this from happening in future, please visit www.pidgeonclient.org/bugzilla and report this:\n\n"
                                        + "Stack trace:\n" + Core.recovery_exception.StackTrace + "\n\n"
                                        + "Target\n" + Core.recovery_exception.TargetSite + "\n\n"
                                        + Core.recovery_exception.Message + "\n\n"
                                        + inner
                                        + ring + "\n\n"
                                        + Core.recovery_exception.Source + "\n\n").Split('\n');
                    bContinue.Enabled = false;
                    bShutdown.Enabled = false;
                }
                else
                {

                    textBox1.Lines = ("YAY, we are terribly sorry, but pidgeon just crashed. If you want to prevent this from happening in future, please visit www.pidgeonclient.org/bugzilla and report this:\n\n"
                                            + "Stack trace:\n" + Core.recovery_exception.StackTrace + "\n\n"
                                            + "Target\n" + Core.recovery_exception.TargetSite + "\n\n"
                                            + Core.recovery_exception.Message + "\n\n"
                                            + inner
                                            + ring + "\n\n"
                                            + Core.recovery_exception.Source + "\n\n").Split('\n');
                }
            }
            catch (Exception)
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private void bShutdown_Click(object sender, EventArgs e)
        {
            try
            {
                Core.IgnoreErrors = true;
                Core._Configuration.ConfigSave();
                Core._KernelThread.Abort();
                Core.Quit();
            }
            catch (Exception)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void bContinue_Click(object sender, EventArgs e)
        {
            Core.IsBlocked = false;
            Hide();
            Close();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            BringToFront();
            Show();
            timer.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
