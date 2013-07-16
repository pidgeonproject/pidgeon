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

using System.IO;
using System.Threading;
using System;
using System.Collections.Generic;

namespace Client
{
    public partial class Core
    {
        /// <summary>
        /// Return a value of variable if it exist
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="window"></param>
        /// <returns></returns>
        public static string EvaluateVariable(string variable, Graphics.Window window)
        {
            switch(variable)
            {
                case "$PidgeonVersion":
                    return System.Windows.Forms.Application.ProductVersion.ToString();
                case "$PidgeonUptime":
                    TimeSpan uptime = DateTime.Now - Core.LoadTime;
                    return uptime.ToString();
                case "$Connected":
                    if (window._Network != null)
                    {
                        return window._Network.IsConnected.ToString();
                    }
                    return "false";
                case "$ServerHost":
                    if (window._Network != null)
                    {
                        return window._Network.ServerName;
                    }
                    return null;
            }
            return null; 
        }

        /// <summary>
        /// This function will take a text and make a condition from it. Returns either text "True" or "False" or returns the same text
        /// as you provided to it when it's not understood
        /// </summary>
        /// <param name="text"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static string EvaluateText(string text, Graphics.Window w = null)
        {
            Core.DebugLog("expression: " + text);
            if (w == null)
            {
                w = SystemForm.Chat;
            }
            string[] parts = text.Split(' ');
            string BufferA = null;
            string BufferB = null;
            int Status = 0;
            int Operation = 0;
            foreach (string part in parts)
            {
                if (Status == 3)
                {
                    break;
                }
                if (part == "")
                {
                    continue;
                }
                switch (Status)
                {
                    case 0:
                        if (part.StartsWith("$"))
                        {
                            BufferA = EvaluateVariable(part, w);
                        } else
                        {
                            BufferA = part;
                        }
                        Status = 1;
                        continue;
                    case 1:
                        switch (part)
                        {
                            case "!=":
                                Operation = 1;
                                Status = 2;
                                continue;
                            case "==":
                                Operation = 2;
                                Status = 2;
                                continue;
                        }
                        throw new Exception("Unable to parse the statement, expected operator (==, !=, >, <)");
                    case 2:
                       if (part.StartsWith("$"))
                        {
                            BufferB = EvaluateVariable(part, w);
                        } else
                        {
                            BufferB = part;
                        }
                        Status = 3;
                        continue;
                }
            }

            if (Status == 3)
            {
                switch (Operation)
                {
                    case 1:
                        return (BufferA != BufferB).ToString().ToLower();
                    case 2:
                        Core.DebugLog("Evaluating " + BufferA + " == " + BufferB);
                        return (BufferA == BufferB).ToString().ToLower();
                }
            }

            return text;
        }
    }
}
