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

namespace Pidgeon.Protocols
{
    /// <summary>
    /// Protocol
    /// </summary>
    public class ProtocolIrc : libirc.Protocols.ProtocolIrc
    {
        private Thread ThreadMain = null;
        public Network NetworkMeta = null;

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

        public override void TrafficLog(string text, bool incoming)
        {
            if(!incoming)
            {
                Core.TrafficScanner.Insert(this.Server, " << " + text);
                return;
            }
            Core.TrafficScanner.Insert(this.Server, " >> " + text);
        }

        public override void DebugLog(string Text, int Verbosity = 1)
        {
            Core.DebugLog(Text, Verbosity);
        }

        private void Exec()
        {
            try
            {
                this.Start();
            } catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        protected override void DisconnectExec(string reason, Exception ex)
        {
            Core.SystemForm.Status("Disconnected from server " + Server);
            this.NetworkMeta.SystemWindow.scrollback.InsertText("Disconnected: " + reason, Pidgeon.ContentLine.MessageStyle.System);
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
                if(cm.StartsWith(" ", StringComparison.Ordinal) != true && cm.Contains(" "))
                {
                    // uppercase
                    string first_word = cm.Substring(0, cm.IndexOf(" ", StringComparison.Ordinal)).ToUpper();
                    string rest = cm.Substring(first_word.Length);
                    Transfer(first_word + rest);
                    return true;
                }
                Transfer(cm.ToUpper());
            } catch (Exception ex)
            {
                Core.HandleException(ex);
            }
            return false;
        }

        public override Thread Open()
        {
            ThreadMain = new Thread(Exec);
            Core.SystemForm.Chat.scrollback.InsertText(messages.get("loading-server", Core.SelectedLanguage, new List<string> { this.Server }),
                                                       Pidgeon.ContentLine.MessageStyle.System);
            Core.SystemForm.Status("Connecting to server " + Server + " port " + Port.ToString());
            ThreadMain.Start();
            Core.SystemThreads.Add(ThreadMain);
            return ThreadMain;
        }
    }
}
