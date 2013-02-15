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
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] parameters)
        {
            AppDomain.CurrentDomain.UnhandledException
                += delegate(object sender, UnhandledExceptionEventArgs args)
                {
                    Exception exception = (Exception)args.ExceptionObject;
                    Core.handleException(exception, true);
                    Environment.Exit(1);
                };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Core.startup = parameters;
                if (Core.Load())
                {
                    Core._Main = new Main();
                    Core.network = null;
                    Application.Run(Core._Main);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                Application.Exit();
            }
        }
    }
}
