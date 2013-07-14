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
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    /// <summary>
    /// Provides information about repository
    /// </summary>
    public static class RevisionProvider
    {
        /// <summary>
        /// Return a hash and revision ifo
        /// </summary>
        /// <returns></returns>
        public static string GetHash()
        {
            try
            {
                using (var stream = Assembly.GetExecutingAssembly()
                                            .GetManifestResourceStream(
                                            "Client" + "." + "version.txt"))
                using (var reader = new StreamReader(stream))
                {
                    string result = reader.ReadLine();
                    if (!reader.EndOfStream)
                    {
                        result += " [" + reader.ReadLine() + "]";
                    }
                    return result;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return "";
        }
    }
}
