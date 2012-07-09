<<<<<<< HEAD
﻿/***************************************************************************
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) version 3.                                           *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.         *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class Configuration
    {
        public static bool Window_Maximized = true;
        /// <summary>
        /// Window left position
        /// </summary>
        public static int Window_Left = 0;

        public static string CommandPrefix = "/";

        /// <summary>
        /// Ident
        /// </summary>
        public static string ident = "pidgeon";
        /// <summary>
        /// Message that is delivered to server when client exit
        /// </summary>
        public static string quit = "Pidgeon client http://pidgeonclient.org/";
        /// <summary>
        /// Nick
        /// </summary>
        public static string nick = "PidgeonUser";

        /// <summary>
        /// Real name
        /// </summary>
        public static string user = "My name is hidden, dude";

        public static string LastHostName = "";

        /// <summary>
        /// Format of ttimestamp
        /// </summary>
        public static string format_date = "($1) ";
        /// <summary>
        /// Format of nick
        /// </summary>
        public static string format_nick = "<$1> ";

        public static bool chat_timestamp = true;
        /// <summary>
        /// Timestamp mask
        /// </summary>
        public static string timestamp_mask = "HH:mm:ss";

        public static string DefaultReason = "Removed from the channel";

        public static bool aggressive_mode = true;
        public static bool aggressive_whois = true;
        public static bool aggressive_bans = true;
        public static bool aggressive_exception = false;
        public static bool aggressive_invites = false;
        public static bool aggressive_channel = true;

        /// <summary>
        /// Platform
        /// </summary>
        public static Core.Platform CurrentPlatform = Core.Platform.Windowsx86;
        /// <summary>
        /// Version
        /// </summary>
        public static string Version = "Pidgeon v. " + System.Windows.Forms.Application.ProductVersion;

        /// <summary>
        /// Used skin
        /// </summary>
        public static Skin CurrentSkin = new Skin();

        /// <summary>
        /// Default bans
        /// </summary>
        public static TypeOfBan DefaultBans = TypeOfBan.Host;

        /// <summary>
        /// If true, ctcp messages will be printed to window
        /// </summary>
        public static bool DisplayCtcp = true;

        /// <summary>
        /// Updater
        /// </summary>
        public static string UpdaterUrl = "http://pidgeonclient.org/updater/index.php?this=" + System.Web.HttpUtility.UrlEncode( System.Windows.Forms.Application.ProductVersion) ;

        public static bool flood_prot = false;
        public static bool ctcp_prot = false;
        public static bool notice_prot = false;

        public static bool parse_hide = false;

        public static int reload_time = 100;
        public static int mq = 1200;
        
        public static string logs_dir = System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "log";
        public static string logs_name = "$1_yyMMdd";

        public static bool slapsdef = true;
        public static bool pokesdef = true;

        public static bool logs_xml = true;
        public static bool logs_txt = true;
        public static bool logs_html = true;

        public static int x1 = 0;
        public static int x4 = 0;

        public static int window_size = 0;

        public enum TypeOfBan
        { 
            Nick,
            Ident,
            Host,
            NickHost
        }

        public enum Priority
        {
            High = 8,
            Normal = 2,
            Low = 1
        }
    }
}
