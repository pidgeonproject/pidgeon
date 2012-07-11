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
    public partial class Recovery : Form
    {
        public Recovery()
        {
            InitializeComponent();
        }

        private void __LP_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Recovery_Load(object sender, EventArgs e)
        {
            textBox1.Lines = ("YAY, we are terribly sorry, but pidgeon just crashed. If you want to prevent this from happening in future, please visit www.pidgeonclient.org/bugzilla and report this:\n\n"
                                + "Stack trace:\n" + Core.recovery_exception.StackTrace + "\n\n"
                                + "Target\n" + Core.recovery_exception.TargetSite + "\n\n"
                                + Core.recovery_exception.Message + "\n\n"
                                + Core.recovery_exception.Source + "\n\n").Split('\n');
        }

        private void bShutdown_Click(object sender, EventArgs e)
        {
            Core._KernelThread.Abort();
            Core.IgnoreErrors = true;
            foreach (Protocol server in Core.Connections)
            {
                server.Exit();
            }
            Application.ExitThread();
        }

        private void bContinue_Click(object sender, EventArgs e)
        {
            Core.blocked = false;
            Close();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            BringToFront();
            Show();
            timer.Enabled = false;
        }
    }
}
