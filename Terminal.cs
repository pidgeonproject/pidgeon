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

namespace Client
{
    /// <summary>
    /// Terminal
    /// </summary>
    public class Terminal
    {
        private class Parameter
        {
            /// <summary>
            /// The parameter.
            /// </summary>
            public string parameter;
            /// <summary>
            /// Parm
            /// </summary>
            public List<string> parm;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="Client.Terminal.Parameter"/> class.
            /// </summary>
            /// <param name='_Parameter'>
            /// Name
            /// </param>
            /// <param name='Params'>
            /// Parameters.
            /// </param>
            public Parameter(string _Parameter, List<string> Params)
            {
                parameter = _Parameter;
                parm = Params;
            }
        }
        
        private static void ShowHelp()
        {
            Console.WriteLine("Usage: pidgeon [h] [link]\n"
                              + "\n"
                              + "This is a GUI irc client, you need to have an xserver in order to run it, bellow is a list of available options:\n"
                              + "\n"
                              + "Calling pidgeon irc://irc.tm-irc.org will connect to server tm-irc.org on port 6667, link has format [$]server:port $ is optional\n"
                              + "\nParameters:\n\n"
                              + "  --safe: Start pidgeon in safe mode\n"
                              + "  -h (--help): display this help\n"
                              + "\n"
                              + "for more information see http://pidgeonclient.org/wiki pidgeon is open source");
        }
        
        private static bool Process(List<Parameter> ls)
        {
            foreach (Parameter parameter in ls)
            {
                switch(parameter.parameter)
                {
                    case "help":
                        ShowHelp();
                        return true;
                    case "safe":
                        Configuration.Kernel.Safe = true;
                        return false;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Check the parameters of program, return true if we can continue
        /// </summary>
        public static bool Parameters()
        {
            List<string> args = new List<string>();
            foreach (string xx in Program.Parameters)
            {
                args.Add(xx);
            }
            
            List<Parameter> ParameterList = new List<Parameter>();
            
            if (args.Count > 0)
            {
                List<string> values = null;
                string id = null;
                string parsed = null;
                foreach (string data in args)
                {
                    bool Read = false;
                    switch (data)
                    {
                        case "--help":
                        case "-h":
                            parsed = id;
                            id = "help";
                            Read = true;
                            break;
                        case "--safe":
                            parsed = id;
                            id = "safe";
                            Read = true;
                            break;
                    }
                    
                    if (parsed != null)
                    {
                        Parameter text = new Parameter(parsed, values);
                        ParameterList.Add(text);
                        parsed = null;
                        values = null;
                    }
                    
                    if (Read)
                    {
                        continue;
                    }
                    
                    if (values == null)
                    {
                        values = new List<string>();
                    }
                    
                    values.Add(  data  );
                }
                
                if (id != null)
                {
                    ParameterList.Add(new Parameter(id, values));
                }
                
                if (Process(ParameterList))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

