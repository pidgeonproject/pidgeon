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
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Data;
using System.Xml.Serialization;

namespace Pidgeon
{
    /// <summary>
    /// Helper class that is used for writing of IRC logs to a file
    /// </summary>
    public static class Scrollback_LogWriter
    {
        private static string ValidPath(string path)
        {
            return path.Replace("?", "1_").Replace("|", "2_").Replace(":", "3_").Replace("\\", "4_").Replace("/", "5_").Replace("*", "6_");
        }

        /// <summary>
        /// Return a file path without special symbols not supported in windows or linux
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string ValidPath(Graphics.Window owner, string directory)
        {
            if (directory != null)
            {
                return directory;
            }
            if (owner == null)
            {
                throw new PidgeonException("You can't enable logging for a window that has no parent");
            }
            directory = owner.WindowName.Replace("?", "1_").Replace("|", "2_").Replace(":", "3_").Replace("\\", "4_").Replace("/", "5_").Replace("*", "6_");
            return directory;
        }

        private static string GetFileName(Graphics.Window owner, string directory, DateTime time)
        {
            string name = Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + ValidPath(owner._Network.ServerName) + 
                Path.DirectorySeparatorChar + ValidPath(owner.WindowName) + Path.DirectorySeparatorChar + 
                time.ToString(Configuration.Logs.logs_name).Replace("$1", ValidPath(owner, directory));
            return name;
        }

        /// <summary>
        /// Write message to a file
        /// </summary>
        /// <param name="text"></param>
        /// <param name="InputStyle"></param>
        /// <param name="owner"></param>
        /// <param name="directory"></param>
        /// <param name="time"></param>
        public static void Log(string text, Pidgeon.ContentLine.MessageStyle InputStyle, Graphics.Window owner, string directory, DateTime time)
        {
            try
            {
                if (!Directory.Exists(Configuration.Logs.logs_dir))
                    Directory.CreateDirectory(Configuration.Logs.logs_dir);

                if (!Directory.Exists(Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + ValidPath(owner._Network.ServerName)))
                    System.IO.Directory.CreateDirectory(Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + ValidPath(owner._Network.ServerName));

                if (!Directory.Exists(Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + ValidPath(owner._Network.ServerName) + Path.DirectorySeparatorChar + ValidPath(owner, directory)))
                    Directory.CreateDirectory(Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + ValidPath(owner._Network.ServerName) + Path.DirectorySeparatorChar + ValidPath(owner, directory));

                if (Configuration.Logs.logs_txt)
                {
                    string stamp = "";
                    if (Configuration.Scrollback.chat_timestamp)
                        stamp = Configuration.Scrollback.format_date.Replace("$1", time.ToString(Configuration.Scrollback.timestamp_mask));
                    Writer.InsertText(stamp + Protocol.DecodeText(Core.RemoveSpecial(text)) + "\n", GetFileName(owner, directory, time) + ".txt");
                }
                if (Configuration.Logs.logs_html)
                {
                    string stamp = "";
                    if (Configuration.Scrollback.chat_timestamp)
                        stamp = Configuration.Scrollback.format_date.Replace("$1", time.ToString(Configuration.Scrollback.timestamp_mask));
                    Writer.InsertText("<font size=\"" + Configuration.CurrentSkin.FontSize.ToString() + "px\" face=" + Configuration.CurrentSkin.LocalFont + ">" +
                        System.Web.HttpUtility.HtmlEncode(stamp + Protocol.DecodeText(Core.RemoveSpecial(text))) + 
                        "</font><br>\n", GetFileName(owner, directory, time) + ".html");
                }
                if (Configuration.Logs.logs_xml)
                {
                    Writer.InsertText("<line time=\"" + time.ToBinary().ToString() + "\" style=\"" + InputStyle.ToString() + "\">" +
                        System.Web.HttpUtility.HtmlEncode(Protocol.DecodeText(Core.RemoveSpecial(text))) + "</line>\n", 
                        GetFileName(owner, directory, time) + ".xml");
                }
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }
    }
}
