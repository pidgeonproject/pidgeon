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
using System.Text;

namespace Client
{
    /// <summary>
    /// This class is handling all commands
    /// </summary>
    public static partial class Commands
    {
        private static partial class Generic
        {
            public static void If(string parameter)
            {
               if (parameter.Contains("\""))
               {
                   string text = parameter.Substring(parameter.IndexOf("\"", StringComparison.Ordinal) + 1);
                   if (text.Contains("\""))
                   {
                       string command = text.Substring(text.IndexOf("\"", StringComparison.Ordinal) + 1);
                       while (command.StartsWith(" ", StringComparison.Ordinal))
                       {
                           command=command.Substring(1);
                       }
                       text = text.Substring(0, text.IndexOf("\"", StringComparison.Ordinal));
                       if (Core.EvaluateText(text) == "true")
                       {
                           Parser.parse(command);
                       }
                       return;
                   }
               }
               Core.SystemForm.Chat.scrollback.InsertText("Invalid expression", ContentLine.MessageStyle.System);
            }
        }
    }
}
