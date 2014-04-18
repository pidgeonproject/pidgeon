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

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
