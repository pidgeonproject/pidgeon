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
using Gtk;

namespace Client
{
    static class Program
    {
        public static string[] Parameters;
        private static void ExceptionForm(GLib.UnhandledExceptionArgs e)
        {
            Core.handleException((Exception)e.ExceptionObject, true);
            Environment.Exit(2);
        }
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] parameters)
        {
            try
            {
                Parameters = parameters;
                Application.Init();
                GLib.ExceptionManager.UnhandledException += new GLib.UnhandledExceptionHandler(ExceptionForm);
                if (Terminal.Parameters())
                {
                    if (Core.Load())
                    {
                        Core.SelectedNetwork = null;
                        Core.SetMain(new Forms.Main());
                        Core.SystemForm.Show();
                        Application.Run();
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                Application.Quit();
            }
            catch (AccessViolationException fail)
            {
                Core.handleException(fail, true);
            }
            catch (Exception fail)
            {
                Core.handleException(fail, true);
            }
        }
    }
}
