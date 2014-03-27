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

using System.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Pidgeon.Protocols
{
    /// <summary>
    /// Protocol
    /// </summary>
    public class ProtocolIrc : libirc.Protocols.ProtocolIrc
    {
        WindowsManager WindowManager = new WindowsManager();

        /// <summary>
        /// Return back the system chars to previous look
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string DecodeText(string text)
        {
            return text.Replace("%####%", "%");
        }

        /// <summary>
        /// Escape system char
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Escaped text</returns>
        public static string EncodeText(string text)
        {
            return text.Replace("%", "%####%");
        }

        /// <summary>
        /// Command
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public bool Command(string cm, Network network = null)
        {
            try
            {
                if (cm.StartsWith(" ", StringComparison.Ordinal) != true && cm.Contains(" "))
                {
                    // uppercase
                    string first_word = cm.Substring(0, cm.IndexOf(" ", StringComparison.Ordinal)).ToUpper();
                    string rest = cm.Substring(first_word.Length);
                    Transfer(first_word + rest);
                    return true;
                }
                Transfer(cm.ToUpper());
            }
            catch (Exception ex)
            {
                Core.HandleException(ex);
            }
            return false;
        }
    }
}
