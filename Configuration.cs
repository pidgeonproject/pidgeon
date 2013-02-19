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

        /// <summary>
        /// Prefix for all commands
        /// </summary>
        public static readonly string CommandPrefix = "/";

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
        /// Enable this to make bars change the max size when reached
        /// </summary>
        public static bool DynamicBars = true;

        /// <summary>
        /// Real name
        /// </summary>
        public static string user = "My name is hidden, dude";

        /// <summary>
        /// Format of ttimestamp
        /// </summary>
        public static string format_date = "($1) ";
        /// <summary>
        /// Format of nick
        /// </summary>
        public static string format_nick = "<$1> ";

        public static bool FriendlyWho = true;

        /// <summary>
        /// If true a client will ask before executing generated kick bans
        /// </summary>
        public static bool ConfirmAll = true;

        /// <summary>
        /// If timestamp is displayed
        /// </summary>
        public static bool chat_timestamp = true;
        /// <summary>
        /// Timestamp mask
        /// </summary>
        public static string timestamp_mask = "HH:mm:ss";

        /// <summary>
        /// Default kick
        /// </summary>
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
        public static readonly Core.Platform CurrentPlatform = Core.Platform.Windowsx86;
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

        public static List<char> Separators = new List<char> { '<', ' ', '>', ',', '!', '[', ']', '{', '}', (char)1, (char)2};

        /// <summary>
        /// Network Scan
        /// </summary>
        public static bool NetworkSniff = true;

        public static bool DisplaySizeOfBuffer = true;

        /// <summary>
        /// If updates are allowed
        /// </summary>
        public static bool CheckUpdate = true;

        /// <summary>
        /// Updater
        /// </summary>
        public static string UpdaterUrl = "http://pidgeonclient.org/updater/index.php?this=" + System.Web.HttpUtility.UrlEncode( System.Windows.Forms.Application.ProductVersion) ;

        /// <summary>
        /// If flood protection is enabled
        /// </summary>
        public static bool flood_prot = false;
        /// <summary>
        /// If ctcp protection is enabled
        /// </summary>
        public static bool ctcp_prot = false;
        /// <summary>
        /// If notice protection is enabled
        /// </summary>
        public static bool notice_prot = false;

        /// <summary>
        /// Reload time for scrollback - this is deprecated
        /// </summary>
        public static int reload_time = 100;
        /// <summary>
        /// Time of message queue
        /// </summary>
        public static int mq = 1200;
        
        /// <summary>
        /// Directory path
        /// </summary>
        public static string logs_dir = System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "log";
        /// <summary>
        /// Name
        /// </summary>
        public static string logs_name = "$1_yyMMdd";

        public static bool slapsdef = true;
        public static bool pokesdef = true;

        public static bool Retrieve_Sv = true;

        public static List<Core.Shortcut> ShortcutKeylist = new List<Core.Shortcut>();

        /// <summary>
        /// Enable XML logs
        /// </summary>
        public static bool logs_xml = true;
        /// <summary>
        /// Enable TXT logs
        /// </summary>
        public static bool logs_txt = true;
        /// <summary>
        /// Enable html logs
        /// </summary>
        public static bool logs_html = true;

        /// <summary>
        /// Enable the notice in tray
        /// </summary>
        public static bool Notice = true;

        public static string Nick2 = "";

        /// <summary>
        /// Last used nick
        /// </summary>
        public static string LastNick = "";
        /// <summary>
        /// Last used host
        /// </summary>
        public static string LastHost = "";
        /// <summary>
        /// Last used port
        /// </summary>
        public static string LastPort = "";

        /// <summary>
        /// List of loaded skins
        /// </summary>
        public static List<Skin> SL = new List<Skin>();

        /// <summary>
        /// Highlighter list
        /// </summary>
        public static List<Network.Highlighter> HighlighterList = new List<Network.Highlighter>();

        /// <summary>
        /// backlog size
        /// </summary>
        public static int Depth = 20000;

        public static bool SearchRegex = true;

        public static bool Debugging = false;

        /// <summary>
        /// Position of textbox scroll
        /// </summary>
        public static int x1 = 0;
        /// <summary>
        /// Position of sidebar scroll
        /// </summary>
        public static int x4 = 0;

        /// <summary>
        /// If values that are being parsed are hidden
        /// </summary>
        public static bool HidingParsed = true;

        /// <summary>
        /// Require confirmation from user before exiting
        /// </summary>
        public static bool ShutdownCheck = true;

        /// <summary>
        /// Position of window
        /// </summary>
        public static int window_size = 0;

        private static Dictionary<string, string> ExtensionConfig = new Dictionary<string, string>();

        public static Dictionary<string, string> Extensions
        {
            get
            {
                lock (ExtensionConfig)
                {
                    Dictionary<string, string> conf = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> xx in ExtensionConfig)
                    {
                        conf.Add(xx.Key, xx.Value);
                    }
                    return conf;
                }
            }
        }

        public static void SetConfig(string key, string value)
        {
            lock (ExtensionConfig)
            {
                if (ExtensionConfig.ContainsKey(key))
                {
                    ExtensionConfig[key] = value;
                }
                else
                {
                    ExtensionConfig.Add(key, value);
                }
            }
        }

        public static void RemoveConfig(string key)
        { 
            lock (ExtensionConfig)
            {
                if (ExtensionConfig.ContainsKey(key))
                {
                    ExtensionConfig.Remove(key);
                }
             }
        }

        public static void SetConfig(string key, bool value)
        {
            SetConfig(key, value.ToString());
        }

        public static void SetConfig(string key, long value)
        {
            SetConfig(key, value.ToString());
        }

        public static void SetConfig(string key, int value)
        {
            SetConfig(key, value.ToString());
        }

        public static string GetConfig(string key)
        {
            lock (ExtensionConfig)
            {
                if (ExtensionConfig.ContainsKey(key))
                {
                    return ExtensionConfig[key];
                }
            }
            return null;
        }

        public static bool GetConfig(string key, bool Default)
        {
            lock (ExtensionConfig)
            {
                if (ExtensionConfig.ContainsKey(key))
                {
                    return bool.Parse(ExtensionConfig[key]);
                }
            }
            return Default;
        }

        public static long GetConfig(string key, long Default)
        {
            lock (ExtensionConfig)
            {
                if (ExtensionConfig.ContainsKey(key))
                {
                    return long.Parse(ExtensionConfig[key]);
                }
            }
            return Default;
        }

        public static int scrollback_plimit = 800;

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
