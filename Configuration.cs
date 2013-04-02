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

// This file contains all configuration of pidgeon, functions to save and load it are in core not here

using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    public class Configuration
    {
        // Static config
        // ===========================================================
        /// <summary>
        /// Changing this to false will disable changing of mouse pointer in SBA box
        /// </summary>
        public static readonly bool ChangingMouse = true;
        /// <summary>
        /// Prefix for all commands
        /// </summary>
        public static readonly string CommandPrefix = "/";
        /// <summary>
        /// Platform
        /// </summary>
        public static readonly Core.Platform CurrentPlatform = Core.Platform.Windowsx64;
        /// <summary>
        /// Version
        /// </summary>
        public static string Version = "Pidgeon v. " + System.Windows.Forms.Application.ProductVersion;


        // Dynamic configuration
        // ===========================================================
        /// <summary>
        /// Configuration for window
        /// </summary>
        public class Window
        {
            public static bool Window_Maximized = true;
            /// <summary>
            /// Window left position
            /// </summary>
            public static int Window_Left = 0;
            /// <summary>
            /// Position of textbox scroll
            /// </summary>
            public static int x1 = 0;
            /// <summary>
            /// Position of sidebar scroll
            /// </summary>
            public static int x4 = 0;
            /// <summary>
            /// Position of window
            /// </summary>
            public static int window_size = 0;
            /// <summary>
            /// Maximal size of history
            /// </summary>
            public static int history = 200;
            /// <summary>
            /// Interval for lock of userlist in case that user is working with it
            /// </summary>
            public static int LockWork = 8000;
        }

        /// <summary>
        /// Configuration of IRC
        /// </summary>
        public class irc
        {
            /// <summary>
            /// If true the system will convert whois data to some format
            /// </summary>
            public static bool FriendlyWhois = true;
            /// <summary>
            /// If true a client will ask before executing generated kick bans
            /// </summary>
            public static bool ConfirmAll = true;
            /// <summary>
            /// Default bans
            /// </summary>
            public static TypeOfBan DefaultBans = TypeOfBan.Host;
            /// <summary>
            /// If true, ctcp messages will be printed to window
            /// </summary>
            public static bool DisplayCtcp = true;
            /// <summary>
            /// Default kick
            /// </summary>
            public static string DefaultReason = "Removed from the channel";
            /// <summary>
            /// Time of message queue
            /// </summary>
            public static int mq = 1200;
        }

        /// <summary>
        /// Personal default configuration for all connections
        /// </summary>
        public class UserData
        {
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
            /// <summary>
            /// This is a nick that is used in case the current nick is in use
            /// </summary>
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
            /// Change this to false to disable links to open a browser
            /// </summary>
            public static bool OpenLinkInBrowser = true;
        }

        public class Colors
        {
            /// <summary>
            /// If this is true the link color will be overriden by color defined by color code
            /// </summary>
            public static bool ChangeLinks = false;
        }

        public class Kernel
        {
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
            /// Enable the notice in tray
            /// </summary>
            public static bool Notice = true;
            /// <summary>
            /// Updater
            /// </summary>
            public static string UpdaterUrl = "http://pidgeonclient.org/updater/index.php?this=" + System.Web.HttpUtility.UrlEncode(System.Windows.Forms.Application.ProductVersion);

            public static int MaximalRingLogSize = 20000;
            public static bool SearchRegex = true;

            public static bool Debugging = false;
            /// <summary>
            /// If values that are being parsed are hidden
            /// </summary>
            public static bool HidingParsed = true;
            /// <summary>
            /// Require confirmation from user before exiting
            /// </summary>
            public static bool ShutdownCheck = true;
        }

        public class ChannelModes
        {
            /// <summary>
            /// If this is true the /mode run on every new channel in order to retrieve its mode no matter if server offer it or not
            /// </summary>
            public static bool aggressive_mode = true;
            /// <summary>
            /// If this is true the /whois is run on every user in a new channel in order to get all information about every user in that channel
            /// </summary>
            public static bool aggressive_whois = false;
            /// <summary>
            /// If this is true the /mode +b is run on every new channel
            /// </summary>
            public static bool aggressive_bans = true;
            /// <summary>
            /// If this is true the /mode +e is run on every new channel
            /// </summary>
            public static bool aggressive_exception = false;
            /// <summary>
            /// If this is true the /mode +I is run on every new channel
            /// </summary>
            public static bool aggressive_invites = false;
            /// <summary>
            /// If this is true the /who is run on every new channel in order to get all information about every user in that channel
            /// </summary>
            public static bool aggressive_channel = true;
        }

        public class Logs
        {
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
            /// Directory path
            /// </summary>
            public static string logs_dir = System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "log";
            /// <summary>
            /// Name
            /// </summary>
            public static string logs_name = "$1_yyMMdd";

            public static ServiceLogs ServicesLogs = ServiceLogs.none;

            public enum ServiceLogs
            { 
                full,
                incremental,
                none
            }
        }

        public class Scrollback
        {
            public static int DynamicSize = 600;
            /// <summary>
            /// Enable this to make bars change the max size when reached
            /// </summary>
            public static bool DynamicBars = true;
            /// <summary>
            /// Size
            /// </summary>
            public static int scrollback_plimit = 200;
            /// <summary>
            /// If timestamp is displayed
            /// </summary>
            public static bool chat_timestamp = true;
            /// <summary>
            /// Timestamp mask
            /// </summary>
            public static string timestamp_mask = "HH:mm:ss";
            /// <summary>
            /// Format of ttimestamp
            /// </summary>
            public static string format_date = "($1) ";
            /// <summary>
            /// Format of nick in a scrollback
            /// </summary>
            public static string format_nick = "<$1> ";
        }

        public class Services
        {
            /// <summary>
            /// backlog size
            /// </summary>
            public static int Depth = 600000;

            public static bool Retrieve_Sv = true;
            /// <summary>
            /// Services can store cached traffic to local temp
            /// </summary>
            public static bool UsingCache = true;
        }

        public class Parser
        {
            public static List<string> Protocols = new List<string> { "http://", "ftp://", "https://", "irc://", "ssh://" };
            public static List<char> Separators = new List<char> { '<', ' ', '>', '!', '[', ']', '(', '{', '}', ')', (char)1, (char)2, (char)3 };
        }

        /// <summary>
        /// Used skin
        /// </summary>
        public static Skin CurrentSkin = new Skin();

        /// <summary>
        /// Reload time for scrollback - this is deprecated
        /// </summary>
        public static int reload_time = 100;
        

        /// <summary>
        /// Shortcuts enabled
        /// </summary>
        public static List<Core.Shortcut> ShortcutKeylist = new List<Core.Shortcut>();

        /// <summary>
        /// List of loaded skins
        /// </summary>
        public static List<Skin> SL = new List<Skin>();

        /// <summary>
        /// Highlighter list
        /// </summary>
        public static List<Network.Highlighter> HighlighterList = new List<Network.Highlighter>();
        
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
