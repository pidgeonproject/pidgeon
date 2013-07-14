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

namespace Client
{
    class LineLogs
    {
        private static string validpath(string path)
        {
            return path.Replace("?", "1_").Replace("|", "2_").Replace(":", "3_").Replace("\\", "4_").Replace("/", "5_").Replace("*", "6_");
        }

        /// <summary>
        /// Return a file path without special symbols not supported in windows or linux
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string validpath(Graphics.Window owner, string directory)
        {
            if (directory != null)
            {
                return directory;
            }
            if (owner == null)
            {
                throw new Exception("You can't enable logging for a window that has no parent");
            }
            directory = owner.WindowName.Replace("?", "1_").Replace("|", "2_").Replace(":", "3_").Replace("\\", "4_").Replace("/", "5_").Replace("*", "6_");
            return directory;
        }

        private static string _getFileName(Graphics.Window owner, string directory, DateTime time)
        {
            string name = Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + validpath(owner._Network.ServerName) + 
                Path.DirectorySeparatorChar + validpath(owner.WindowName) + Path.DirectorySeparatorChar + 
                time.ToString(Configuration.Logs.logs_name).Replace("$1", validpath(owner, directory));
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
        public static void Log(string text, Client.ContentLine.MessageStyle InputStyle, Graphics.Window owner, string directory, DateTime time)
        {
            try
            {
                if (!Directory.Exists(Configuration.Logs.logs_dir))
                {
                    Directory.CreateDirectory(Configuration.Logs.logs_dir);
                }
                if (!Directory.Exists(Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + owner._Network.ServerName))
                {
                    System.IO.Directory.CreateDirectory(Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + owner._Network.ServerName);
                }
                if (!Directory.Exists(Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + owner._Network.ServerName + Path.DirectorySeparatorChar + validpath(owner, directory)))
                {
                    Directory.CreateDirectory(Configuration.Logs.logs_dir + Path.DirectorySeparatorChar + owner._Network.ServerName + Path.DirectorySeparatorChar + validpath(owner, directory));
                }
                if (Configuration.Logs.logs_txt)
                {
                    string stamp = "";
                    if (Configuration.Scrollback.chat_timestamp)
                    {
                        stamp = Configuration.Scrollback.format_date.Replace("$1", time.ToString(Configuration.Scrollback.timestamp_mask));
                    }
                    Core.IO.InsertText(stamp + Protocol.decode_text(Core.RemoveSpecial(text)) + "\n", _getFileName(owner, directory, time) + ".txt");
                }
                if (Configuration.Logs.logs_html)
                {
                    string stamp = "";
                    if (Configuration.Scrollback.chat_timestamp)
                    {
                        stamp = Configuration.Scrollback.format_date.Replace("$1", time.ToString(Configuration.Scrollback.timestamp_mask));
                    }
                    Core.IO.InsertText("<font size=\"" + Configuration.CurrentSkin.fontsize.ToString() + "px\" face=" + Configuration.CurrentSkin.localfont + ">" + System.Web.HttpUtility.HtmlEncode(stamp + Protocol.decode_text(Core.RemoveSpecial(text))) + "</font><br>\n", _getFileName(owner, directory, time) + ".html");
                }
                if (Configuration.Logs.logs_xml)
                {
                    Core.IO.InsertText("<line time=\"" + time.ToBinary().ToString() + "\" style=\"" + InputStyle.ToString() + "\">" + System.Web.HttpUtility.HtmlEncode(Protocol.decode_text(Core.RemoveSpecial(text))) + "</line>\n", _getFileName(owner, directory, time) + ".xml");
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
