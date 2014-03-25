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

namespace Pidgeon
{
    /// <summary>
    /// Provides an interface to write logs to terminal as well as system window
    /// </summary>
    public class Syslog
    {
        /// <summary>
        /// Insert text in to debug log
        /// </summary>
        /// <param name="data">Text to insert</param>
        /// <param name="verbosity">Verbosity (default is 1)</param>
        public static void DebugLog(string data, int verbosity)
        {
            System.Diagnostics.Debug.Print(data);
            if (Configuration.Kernel.Debugging)
            {
                if (Core.CoreState != Core.State.Terminating && Core.SystemForm != null && !Core.IsBlocked)
                {
                    if (Core.SystemForm.main != null)
                    {
                        Core.SystemForm.main.scrollback.InsertText("DEBUG: " + data, Pidgeon.ContentLine.MessageStyle.System, false);
                    }
                }
            }
            Core.Ringlog("DEBUG: " + data);
        }
    }
}

