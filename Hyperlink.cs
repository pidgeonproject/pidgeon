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
using System.Threading;
using System.Text;

namespace Client
{
    /// <summary>
    /// Helper class
    /// </summary>
    public class Hyperlink
    {
        private static List<string> Links = new List<string>();

        /// <summary>
        /// Open link
        /// </summary>
        /// <param name="ln">Link</param>
        public static void OpenLink(string ln)
        {
            lock (Links)
            {
                Links.Add(ln);
            }
        }

        /// <summary>
        /// Function that handles the queue of links
        /// </summary>
        private static void Exec()
        {
            try
            {
                while (true)
                {
                    if (Links.Count > 0)
                    {
                        List<string> list = new List<string>();
                        lock (Links)
                        {
                            list.AddRange(Links);
                            Links.Clear();
                        }
                        foreach (string ln in list)
                        {
                            System.Diagnostics.Process.Start(ln);
                        }
                    }
                    Thread.Sleep(200);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns></returns>
        public static int Initialize()
        {
            Thread link = new Thread(Exec);
            link.Start();
            return 0;
        }
    }
}
