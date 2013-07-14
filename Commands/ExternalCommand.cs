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
using System.Diagnostics;
using System.IO;

namespace Client
{
    /// <summary>
    /// This is a command that is launched from operating system
    /// </summary>
    class ExternalCommand
    {
        private string Temp = Path.GetTempFileName();
        private string Data = "";
        private string Command;
        private string Parameters;
        private string Text;

        /// <summary>
        /// Creates an external command
        /// </summary>
        /// <param name="Window">Window the input / output should be used of</param>
        /// <param name="command">Command</param>
        /// <param name="parameters">Parameters</param>
        public ExternalCommand(Graphics.Window Window, string command, string parameters)
        {
            Command = command;
            Parameters = parameters;
            Text = Window.scrollback.Text;
        }

        /// <summary>
        /// Start the command
        /// </summary>
        /// <returns></returns>
        public string Execute()
        {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Command,
                    Arguments = Parameters,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            Core.DebugLog("Starting a process " + Command);
            p.Start();
            Core.DebugLog("Writing standard input to " + Command);
            p.StandardInput.Write(Text);
            p.StandardInput.Close();
            Core.DebugLog("Reading standard error from " + Command);
            Data += p.StandardError.ReadToEnd();
            Core.DebugLog("Reading standard output from " + Command);
            Data += p.StandardOutput.ReadToEnd();
            return Data;
        }
    }
}
