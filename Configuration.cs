/***************************************************************************
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
        public static int Window_Left = 0;

        public static string CommandPrefix = "/";

        public static string ident = "pidgeon";
        public static string quit = "Pidgeon client";
        public static string nick = "PidgeonUser";

        public static string user = "My name is hidden, dude";

        public static string LastHostName = "";

        public static string format_date = "($1) ";
        public static string format_nick = "<$1> ";

        public static bool chat_timestamp = true;
        public static string timestamp_mask = "HH:mm:ss";

        public static string DefaultReason = "Removed from the channel";

        public static bool aggressive_mode = true;
        public static bool aggressive_whois = true;
        public static bool aggressive_bans = true;
        public static bool aggressive_exception = false;
        public static bool aggressive_invites = false;
        public static bool aggressive_channel = true;

        public static Core.Platform CurrentPlatform = Core.Platform.Windowsx86;
        public static string Version = "Pidgeon v. " + System.Windows.Forms.Application.ProductVersion;

        public static Skin CurrentSkin = new Skin();

        public static bool DisplayCtcp = true;

        public static string UpdaterUrl = "http://pidgeonclient.org/updater/index.php?this=" + System.Web.HttpUtility.UrlEncode( System.Windows.Forms.Application.ProductVersion) ;

        public static bool flood_prot = false;
        public static bool ctcp_prot = false;
        public static bool notice_prot = false;
        
        public static string logs_dir = "log";
        public static string logs_name = "$1_YYYYmmdd";

        public static bool logs_xml = true;
        public static bool logs_txt = true;
        public static bool logs_html = true;

        public static int x1 = 0;
        public static int x4 = 0;

        public static int window_size = 0;

        public enum Priority
        {
            High = 8,
            Normal = 2,
            Low = 1
        }
    }
}
