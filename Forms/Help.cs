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
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Help : Form
    {
        public Help()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(linkLabel1.Text);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void Help_Load(object sender, EventArgs e)
        {
            try
            {
                label2.Text = Configuration.Version + " build: " + RevisionProvider.GetHash();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
