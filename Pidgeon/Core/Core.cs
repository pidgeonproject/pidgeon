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

using System.IO;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pidgeon
{
    /// <summary>
    /// Basically most elementar stuff the application consist of and that doesn't belong to some other class
    /// </summary>
    public static partial class Core
    {
        /// <summary>
        /// Profiler
        /// </summary>
        public class Profiler
        {
            /// <summary>
            /// Time
            /// </summary>
            private Stopwatch time = new Stopwatch();
            /// <summary>
            /// Function that is being checked
            /// </summary>
            public string Function = null;

            /// <summary>
            /// Creates a new instance with name of function
            /// </summary>
            /// <param name="function"></param>
            public Profiler(string function)
            {
                Function = function;
                time.Start();
            }

            /// <summary>
            /// Called when profiler is supposed to be stopped
            /// </summary>
            public void Done()
            {
                time.Stop();
                if (time.ElapsedMilliseconds > Configuration.Kernel.Profiler_Minimal)
                {
                    Core.Ringlog("PROFILER: " + Function + ": " + time.ElapsedMilliseconds.ToString());
                }
            }
        }

        /// <summary>
        /// This represents a shortcut defined in pidgeon
        /// </summary>
        public class Shortcut
        {
            /// <summary>
            /// Control key needs to be pressed in order to execute this
            /// </summary>
            public bool control;
            /// <summary>
            /// Alt key needs to be pressed in order to execute this
            /// </summary>
            public bool alt;
            /// <summary>
            /// Shift key needs to be pressed in order to execute this
            /// </summary>
            public bool shift;
            /// <summary>
            /// Keycode
            /// </summary>
            public Gdk.Key keys;
            /// <summary>
            /// Execution command (it is supposed to be parsed as regular input)
            /// </summary>
            public string data;

            /// <summary>
            /// Creates a new instance of shortcut
            /// </summary>
            /// <param name="Value">Keycode</param>
            /// <param name="Control">Control needs to be pressed in order to execute this</param>
            /// <param name="Alt">Alt needs to be pressed in order to execute this</param>
            /// <param name="Shift">Shift needs to be pressed in order to execute this</param>
            /// <param name="Data"></param>
            public Shortcut(Gdk.Key Value, bool Control = false, bool Alt = false, bool Shift = false, string Data = "")
            {
                control = Control;
                shift = Shift;
                alt = Alt;
                data = Data;
                keys = Value;
            }
        }

        private static Process KernelProc = null;
        /// <summary>
        /// Thread which the core is running in
        /// </summary>
        private static Thread KernelThread = null;
        /// <summary>
        /// Thread in which the core is running in, the only thread which can control the GTK objects
        /// </summary>
        public static Thread _KernelThread
        {
            get
            {
                return KernelThread;
            }
        }
        /// <summary>
        /// Exact time of system load
        /// </summary>
        public static DateTime StartupTime;
        /// <summary>
        /// Configuration path
        /// </summary>
        public static string ConfigFile = null;
        /// <summary>
        /// Root dir
        /// </summary>
        public static string RootDirectory = null;
        /// <summary>
        /// Language used in system
        /// </summary>
        public static string SelectedLanguage = "en";
        /// <summary>
        /// Pointer to exception class during recovery
        /// </summary>
        public static Exception RecoveryException = null;
        /// <summary>
        /// Timers
        /// </summary>
        public static List<Timer> TimerDB = new List<Timer>();
        /// <summary>
        /// Notification box
        /// </summary>
        private static Forms.Notification NotificationWidget = null;
        private static object RandomLock = new object();
        /// <summary>
        /// Ring log
        /// </summary>
        private static List<string> Ring = new List<string>();
        /// <summary>
        /// If user is holding ctrl key
        /// </summary>
        public static bool HoldingCtrl = false;
        /// <summary>
        /// Container
        /// </summary>
        private static Network selectedNetwork = null;
        /// <summary>
        /// Selected network, if you are connected to a network and currently displayed window is attached to some,
        /// this is that network
        /// </summary>
        public static Network SelectedNetwork
        {
            get
            {
                return selectedNetwork;
            }
            set
            {
                selectedNetwork = value;
            }
        }
        /// <summary>
        /// List of ports that the dcc is using in this moment
        /// </summary>
        public static List<uint> LockedPorts = new List<uint>();
        /// <summary>
        /// This is a reference to system form, it should be changed only once
        /// </summary>
        private static Forms.Main systemForm = null;
        /// <summary>
        /// Gets the system form
        /// </summary>
        /// <value>
        /// The system form
        /// </value>
        public static Forms.Main SystemForm
        {
            get
            {
                return systemForm;
            }
        }
        /// <summary>
        /// Wheter notification is waiting
        /// </summary>
        public static bool NotificationIsNowWaiting = false;
        /// <summary>
        /// Data of notification (text)
        /// </summary>
        private static string NotificationData = "";
        /// <summary>
        /// Caption of notification
        /// </summary>
        private static string NotificationCaption = "";
        /// <summary>
        /// This is index of last unique number, you should never write or read this value, except for its own function
        /// </summary>
        private static int randomuq = 0;
        /// <summary>
        /// Path to skin
        /// </summary>
        public static string SkinPath = "skins";
        /// <summary>
        /// System binary root path
        /// </summary>
        /// <value>The system bin root path.</value>
        public static string SystemBinRootPath
        {
            get
            {
                return SystemRoot;
            }
        }
        /// <summary>
        /// This is a root path to binary file
        /// </summary>
        private static string SystemRoot = null;
        /// <summary>
        /// This is a reference to scanner window
        /// </summary>
        public static Forms.TrafficScanner TrafficScanner = null;
        /// <summary>
        /// System is blocked if this is set to true, all subsystems and kernel are supposed to freeze
        /// </summary>
        public static bool IsBlocked = false;
        /// <summary>
        /// Ignore errors - all exceptions and debug logs are ignored and pidgeon is flagged as unstable
        /// </summary>
        public static bool IgnoreErrors = false;
        /// <summary>
        /// If this is true the recovery window will not allow to ignore
        /// </summary>
        public static bool RecoveryIsFatal = false;
        /// <summary>
        /// Cache of current params
        /// </summary>
        private static List<string> startupParams = new List<string>();
        /// <summary>
        /// Parameters that were retrieved in console
        /// </summary>
        public static List<string> StartupParams
        {
            get
            {
                List<string> data = new List<string>();
                lock (startupParams)
                {
                    data.AddRange(startupParams);
                }
                return data;
            }
        }
        /// <summary>
        /// Status of core
        /// </summary>
        public static State CoreState = State.Loading;
        private static DateTime lastActivityTime;

        /// <summary>
        /// Time for which the user is idle
        /// </summary>
        public static TimeSpan IdleTime
        {
            get
            {
                return DateTime.Now - lastActivityTime;
            }
        }

        /// <summary>
        /// Location of temporary stuff which is not going to be deleted, if you are in need to store some
        /// permanent cache anywhere, this is a good place
        /// </summary>
        public static string PermanentTemp
        {
            get
            {
                return RootDirectory + "Temp" + System.IO.Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Retrieve a read only copy of system ring buffer
        /// </summary>
        public static List<string> RingBuffer
        {
            get
            {
                List<string> rb = new List<string>();
                lock (Ring)
                {
                    rb.AddRange(Ring);
                }
                return rb;
            }
        }

        /// <summary>
        /// Sets the reference to system form
        /// </summary>
        /// <param name='form'>
        /// Form.
        /// </param>
        public static void SetMain(Forms.Main form)
        {
            systemForm = form;
        }

        /// <summary>
        /// This is a first function that should be called after application start
        /// </summary>
        /// <returns></returns>
        public static bool Load(string[] parameters)
        {
            if (CoreState != State.Loading)
            {
                throw new PidgeonException("You can't load core multiple times");
            }
            try
            {
                KernelThread = Thread.CurrentThread;
                KernelThread.Name = "Pidgeon:Core";
                ThreadManager.RegisterThread(KernelThread);
                StartupTime = DateTime.Now;
                // turn on debugging until we load the config
                Configuration.Kernel.Debugging = true;
                SystemRoot = Application.StartupPath + Path.DirectorySeparatorChar;
                if (RootDirectory == null && Application.LocalUserAppDataPath.EndsWith(Application.ProductVersion, StringComparison.Ordinal))
                {
                    RootDirectory = Application.LocalUserAppDataPath.Substring(0, Application.LocalUserAppDataPath.Length - Application.ProductVersion.Length);
                    ConfigFile = RootDirectory + "configuration.dat";
                }
                else if (RootDirectory == null)
                {
                    RootDirectory = System.Windows.Forms.Application.LocalUserAppDataPath + Path.DirectorySeparatorChar;
                    ConfigFile = RootDirectory + "configuration.dat";
                }
                Ringlog("Pidgeon " + Application.ProductVersion.ToString() + " loading core");
                KernelProc = Process.GetCurrentProcess();
                if (Configuration.Kernel.Safe)
                {
                    Ringlog("Running in safe mode");
                }
                startupParams = new List<string>(parameters);
                Configuration.Logs.logs_dir = RootDirectory + "logs";
                Ringlog("Root path is " + RootDirectory);
                Ringlog("Config file: " + ConfigFile);
                Configuration.irc.CertificateDCC = RootDirectory + "certificate.p12";
                string is64 = " which is a 32 bit system";
                if (Environment.Is64BitOperatingSystem)
                {
                    is64 = " which is a 64 bit system";
                }
                Ringlog("This pidgeon is compiled for " + Configuration.CurrentPlatform.ToString() + " and running on " + Environment.OSVersion.ToString() + is64);
                Syslog.DebugLog("Loading messages");
                messages.Read(Configuration.Kernel.Safe);
                TrafficScanner = new Forms.TrafficScanner();
                if (!System.IO.File.Exists(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "pidgeon.dat"))
                {
                    LoadSkin();
                    Syslog.DebugLog("Loading configuration file");
                    Configuration.Kernel.Debugging = false;
                    Configuration.Load();
                    if (!Directory.Exists(PermanentTemp))
                    {
                        Directory.CreateDirectory(PermanentTemp);
                    }
                    if (Configuration.Kernel.Safe)
                    {
                        Syslog.DebugLog("Not running updater in safe mode");
                    }
                    else
                    {
                        Syslog.DebugLog("Running updater");
                        ThreadManager.ThUp = new Thread(Updater.Run);
                        ThreadManager.ThUp.Name = "Pidgeon:Updater";
                        ThreadManager.ThUp.Start();
                        ThreadManager.RegisterThread(ThreadManager.ThUp);
                    }
                    Syslog.DebugLog("Loading log writer thread");
                    ThreadManager.Thread_logs = new Thread(Writer.Load);
                    ThreadManager.Thread_logs.Name = "Pidgeon:Writer";
                    ThreadManager.RegisterThread(ThreadManager.Thread_logs);
                    ThreadManager.Thread_logs.Start();
                    Syslog.DebugLog("Loading commands");
                    Commands.Initialise();
                    NotificationWidget = new Forms.Notification();
                    //DebugLog("Loading scripting core");
                    //ScriptingCore.Load();
                    if (Configuration.Kernel.Safe)
                    {
                        Syslog.DebugLog("Skipping load of extensions");
                    }
                    else
                    {
                        Syslog.DebugLog("Loading extensions");
                        Extension.Init();
                        if (Directory.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "modules"))
                        {
                            foreach (string dll in Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "modules", "*.dll"))
                            {
                                Syslog.DebugLog("Registering plugin " + dll);
                                ExtensionPool.RegisterPlugin(dll);
                            }
                            foreach (string dll in Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "modules", "*.pmod"))
                            {
                                Syslog.DebugLog("Registering plugin " + dll);
                                ExtensionPool.RegisterPlugin(dll);
                            }
                        }
                    }
                    Syslog.DebugLog("Loading http");
                    Hyperlink.Initialize();
                    if (!File.Exists(ConfigFile))
                    {
                        Commands.RegisterAlias("j", "join", false);
                        if (Configuration.CurrentPlatform == Platform.Linuxx86 || Configuration.CurrentPlatform == Platform.Linuxx64)
                        {
                            Commands.RegisterAlias("grep", "pidgeon.term grep", false);
                            Commands.RegisterAlias("shell", "pidgeon.term2in", false);
                        }
                        Network.Highlighter simple = new Network.Highlighter();
                        simple.Enabled = true;
                        simple.Text = "$nick";
                        Configuration.HighlighterList.Add(simple);
                    }
                    ResetMainActivityTimer();
                    Hooks._Sys.AfterCore();
                    CoreState = State.Running;
                    return true;
                }
                Updater _finalisingupdater = new Updater();
                _finalisingupdater.update.Visible = false;
                _finalisingupdater.finalize = true;
                _finalisingupdater.lStatus.Text = messages.get("update2");
                CoreState = State.Killed;
                System.Windows.Forms.Application.Run(_finalisingupdater);
            }
            catch (Exception panic)
            {
                Syslog.DebugLog("@ Core.Load(): " + panic.Message + panic.StackTrace);
                Core.HandleException(panic, true);
            }
            return false;
        }

        /// <summary>
        /// This will reset the activity timer
        /// </summary>
        public static void ResetMainActivityTimer()
        {
            lastActivityTime = DateTime.Now;
        }

        public static bool ExtractIPv6(string hostname, out string ipv6)
        {
            ipv6 = hostname;
            if (!ipv6.Contains("["))
            {
                ipv6 = null;
                return false;
            }
            ipv6 = ipv6.Substring(ipv6.IndexOf("[") + 1);
            if (!ipv6.Contains("]"))
            {
                ipv6 = null;
                return false;
            }
            ipv6 = ipv6.Substring(0, ipv6.IndexOf("]"));
            return true;
        }

        /// <summary>
        /// Parse irc:// this function will connect you using currently active protocol or to services
        /// </summary>
        /// <param name="text">Link to be parsed</param>
        /// <param name="services"></param>
        public static void ParseLink(string text, Protocols.Services.ProtocolSv services = null)
        {
            Syslog.DebugLog("Parsing " + text);
            string ipv6;
            ExtractIPv6(text, out ipv6);
            if (text.StartsWith("ircs://", StringComparison.Ordinal) || text.StartsWith("irc://", StringComparison.Ordinal))
            {
                string channel = null;
                int PORT = 6667;
                bool ssl = false;
                if (text.StartsWith ("irc://", StringComparison.Ordinal))
                {
                    text = text.Substring ("irc://".Length);
                } else
                {
                    text = text.Substring ("ircs://".Length);
                    ssl = true;
                }
                string network = text;
                if (network.StartsWith("$", StringComparison.Ordinal))
                {
                    network = network.Substring(1);
                    ssl = true;
                }
                if (ssl)
                {
                    PORT = 6697;
                }
                if (network.Contains(":"))
                {
                    string port = network.Substring(network.IndexOf(":", StringComparison.Ordinal) + 1);
                    network = network.Substring(0, network.IndexOf(":", StringComparison.Ordinal));
                    if (port.Contains("/"))
                    {
                        port = port.Substring(0, port.IndexOf("/", StringComparison.Ordinal));
                    }
                    if (port.Contains("#"))
                    {
                        port = port.Substring(0, port.IndexOf("#", StringComparison.Ordinal));
                    }
                    if (!int.TryParse(port, out PORT))
                    {
                        PORT = 6667;
                    }
                }
                if (network.Contains("/"))
                {
                    channel = network.Substring(network.IndexOf("/", StringComparison.Ordinal) + 1);
                    if (!channel.StartsWith("#", StringComparison.Ordinal))
                    {
                        channel = "#" + channel;
                    }
                    network = network.Substring(0, network.IndexOf("/", StringComparison.Ordinal));
                }
                if (network.Contains("#"))
                {
                    channel = network.Substring(network.IndexOf("#", StringComparison.Ordinal));
                    network = network.Substring(0, network.IndexOf("#", StringComparison.Ordinal));
                }
                while (network.Contains("/"))
                {
                    network = network.Substring(0, network.Length - 1);
                }
                if (ipv6 != null)
                    network = ipv6;
                if (services != null)
                {
                    lock (services.NetworkList)
                    {
                        foreach (Network sv in services.NetworkList)
                        {
                            if (sv.ServerName == network)
                            {
                                sv.Join(channel);
                                return;
                            }
                        }
                    }
                    services.ConnectTo(network, PORT);
                    lock (services.NetworkList)
                    {
                        foreach (Network sv in services.NetworkList)
                        {
                            if (sv.ServerName == network)
                            {
                                sv.Join(channel);
                                return;
                            }
                        }
                    }
                    return;
                }
                Protocols.ProtocolIrc server = null;
                foreach (libirc.Protocol protocol in Connections.ConnectionList)
                {
                    if (typeof(Protocols.ProtocolIrc) == protocol.GetType() && protocol.Server == network)
                    {
                        server = (Protocols.ProtocolIrc)protocol;
                        break;
                    }
                }

                if (server == null)
                {
                    server = Connections.ConnectIRC(network, PORT, "", ssl);
                }

                if (channel != null)
                {
                    server.IRCNetwork.Join(channel);
                }
            }
        }

        /// <summary>
        /// Recover from crash
        /// </summary>
        private static void Recover()
        {
            Pidgeon.Recovery recoveryWindow = new Pidgeon.Recovery();
            System.Windows.Forms.Application.Run(recoveryWindow);
        }

        /// <summary>
        /// Remove special symbols
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveSpecial(string text)
        {
            if (text.Contains(Protocols.ProtocolIrc.ColorChar))
            {
                int number = 15;
                while (number > 0)
                {
                    text = text.Replace(Protocols.ProtocolIrc.ColorChar + number.ToString(), "");
                    if (number < 10)
                    {
                        text = text.Replace(Protocols.ProtocolIrc.ColorChar + "0" + number.ToString(), "");
                    }
                    number--;
                }
            }
            return text.Replace("%/USER%", "")
                       .Replace("%USER%", "")
                       .Replace("%H%", "")
                       .Replace("%/H%", "")
                       .Replace("%D%", "")
                       .Replace("%L%", "")
                       .Replace("%/L%", "")
                       .Replace("%/D%", "")
                       .Replace(Protocols.ProtocolIrc.UnderlineChar, "")
                       .Replace(Protocols.ProtocolIrc.BoldChar, "")
                       .Replace(Protocols.ProtocolIrc.ColorChar, "")
                       .Replace(((char)004).ToString(), "");
        }

        /// <summary>
        /// Remove the ring data
        /// </summary>
        public static void ClearRingBufferLog()
        {
            lock (Ring)
            {
                Ring.Clear();
            }
        }

        /// <summary>
        /// Insert a text to log
        /// </summary>
        /// <param name="data">Data</param>
        public static void Ringlog(string data)
        {
            string text = "[" + DateTime.Now.ToString() + "] " + data;
            lock (Ring)
            {
                Ring.Add(text);
                while (Ring.Count > Configuration.Kernel.MaximalRingLogSize)
                {
                    Ring.RemoveAt(0);
                }
            }
            if (Configuration.Kernel.Debugging)
            {
                Console.WriteLine(text);
            }
        }

        /// <summary>
        /// Insert text in to debug log
        /// </summary>
        /// <param name="data">Text to insert</param>
        [Obsolete]
        public static void DebugLog(string data)
        {
            Syslog.DebugLog(data, 1);
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Port"></param>
        /// <param name="User"></param>
        /// <param name="Listener"></param>
        /// <param name="SSL"></param>
        /// <param name="network"></param>
        public static void OpenDCC(string Server, int Port, string User, bool Listener, bool SSL, Network network)
        {
            if (KernelThread == Thread.CurrentThread)
            {
                new Forms.OpenDCC(Server, User, (uint)Port, Listener, SSL, network);
                return;
            }
            lock (Graphics.PidgeonList.WaitingDCC)
            {
                Graphics.PidgeonList.WaitingDCC.Add(new Graphics.PidgeonList.RequestDCC(User, Server, Port, SSL, Listener, network));
                Graphics.PidgeonList.Updated = true;
            }
        }

        /// <summary>
        /// Thread unsafe
        /// </summary>
        /// <param name="requestDCC"></param>
        public static void OpenDCC(Graphics.PidgeonList.RequestDCC requestDCC)
        {
            new Forms.OpenDCC(requestDCC.Server, requestDCC.User, (uint)requestDCC.Port, requestDCC.Listener, requestDCC.SSL, requestDCC.network);
            return;
        }

        /// <summary>
        /// Draw all text into a scrollback
        /// </summary>
        /// <param name="window">Window to which it should be printed</param>
        /// <param name="write">Whether it should be written to file</param>
        public static void PrintRing(Graphics.Window window, bool write = true)
        {
            lock (Ring)
            {
                foreach (string item in Ring)
                {
                    window.scrollback.InsertText(item, Pidgeon.ContentLine.MessageStyle.System, write, 0, true);
                }
            }
        }

        /// <summary>
        /// Parse a command from input box
        /// </summary>
        /// <param name="command">Command, example /connect</param>
        /// <returns></returns>
        public static bool ProcessCommand(string command)
        {
            if (command.StartsWith(Configuration.CommandPrefix, StringComparison.Ordinal))
            {
                command = command.Substring(1);
            }

            // in case that it's known command we return
            if (Commands.Process(command))
            {
                return true;
            }

            // if not we can try to pass it to server
            if (Core.SystemForm.Chat != null && !Core.SystemForm.Chat.IsDestroyed && Core.SystemForm.Chat._Network != null && Core.SystemForm.Chat._Network._Protocol != null)
            {
                if (SystemForm.Chat._Network._Protocol.IsConnected)
                {
                    Core.SystemForm.Chat._Network._Protocol.Command(command);
                    return false;
                }
            }
            SystemForm.Chat.scrollback.InsertText(messages.get("invalid-command", SelectedLanguage), Pidgeon.ContentLine.MessageStyle.System);
            return false;
        }

        /// <summary>
        /// Restore a file
        /// </summary>
        /// <param name="file">File path</param>
        /// <returns></returns>
        public static bool Restore(string file)
        {
            if (!File.Exists(file + "~"))
            {
                return false;
            }
            string backup = System.IO.Path.GetRandomFileName();
            Syslog.DebugLog("Restoring file " + file + " from a backup");
            if (File.Exists(file))
            {
                File.Copy(file, backup);
                Syslog.DebugLog("Stored previous version: " + backup);
            }
            File.Copy(file + "~", file, true);
            return true;
        }

        /// <summary>
        /// Finalize displaying of notice box or show the last displayed box
        /// </summary>
        public static void DisplayNote()
        {
            if (Configuration.Kernel.Notice == false)
            {
                return;
            }
            if (Configuration.Media.NotificationSound && SystemForm.Chat.Sounds)
            {
                System.Media.SystemSounds.Asterisk.Play();
            }
            if (_KernelThread == Thread.CurrentThread)
            {
                if (NotificationIsNowWaiting)
                {
                    bool Focus = false;
                    NotificationWidget.text.Text = NotificationData;
                    NotificationWidget.title.Markup = "<span size='18000'>" + NotificationCaption + "</span>";
                    NotificationIsNowWaiting = false;
                    if (Core.SystemForm.Chat != null && Core.SystemForm.Chat.textbox.richTextBox1.IsFocus)
                    {
                        Focus = true;
                    }
                    if (!NotificationWidget.Visible)
                    {
                        NotificationWidget.Relocate();
                        NotificationWidget.Show();
                        if (Focus)
                        {
                            Core.SystemForm.setFocus();
                            if (Core.SystemForm.Chat != null)
                            {
                                NotificationWidget.WillFocus = true;
                                Core.SystemForm.Chat.textbox.setFocus();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Show a notice box, if started from non kernel thread, then it's finalized in that
        /// </summary>
        /// <param name="data">Information</param>
        /// <param name="caption">Name</param>
        public static void DisplayNote(string data, string caption)
        {
            if (Configuration.Kernel.Notice == false)
            {
                return;
            }
            data = Protocol.DecodeText(Core.RemoveSpecial(data));
            NotificationIsNowWaiting = true;
            NotificationData = data;
            NotificationCaption = caption;
            if (_KernelThread == Thread.CurrentThread)
            {
                DisplayNote();
            }
        }

        /// <summary>
        /// Backup a file
        /// </summary>
        /// <param name="file">File name</param>
        /// <returns>true</returns>
        public static bool Backup(string file)
        {
            if (File.Exists(file + "~"))
            {
                string backup = System.IO.Path.GetRandomFileName();
                File.Copy(file + "~", backup);
            }
            if (File.Exists(file))
            {
                File.Copy(file, file + "~", true);
            }
            return true;
        }

        /// <summary>
        /// Get a random unique string (it's not possible to get 2 same strings from this function)
        /// </summary>
        /// <returns>Unique ID</returns>
        public static string RetrieveRandom()
        {
            int random = 0;
            lock (RandomLock)
            {
                randomuq++;
                random = randomuq;
            }
            return ":" + random.ToString() + "*";
        }

        /// <summary>
        /// Editor
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="target">Network</param>
        public static void ProcessScript(string script, Network target)
        {
            try
            {
                Forms.ScriptEdit edit = new Forms.ScriptEdit();
                edit.textBox1.Buffer.Text += script;
                edit.network = target;
                edit.Show();
            }
            catch (Exception fail)
            {
                Core.HandleException(fail);
            }
        }

        /// <summary>
        /// Remove a special symbols from html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string NormalizeHtml(string text)
        {
            return System.Web.HttpUtility.HtmlEncode(text);
        }

        /// <summary>
        /// Convert a key to upper case
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Gdk.Key ConvertKey(Gdk.Key key)
        {
            switch (key)
            {
                case Gdk.Key.a:
                    return Gdk.Key.A;
                case Gdk.Key.b:
                    return Gdk.Key.B;
                case Gdk.Key.c:
                    return Gdk.Key.C;
                case Gdk.Key.d:
                    return Gdk.Key.D;
                case Gdk.Key.e:
                    return Gdk.Key.E;
                case Gdk.Key.f:
                    return Gdk.Key.F;
                case Gdk.Key.g:
                    return Gdk.Key.G;
                case Gdk.Key.h:
                    return Gdk.Key.H;
                case Gdk.Key.i:
                    return Gdk.Key.I;
                case Gdk.Key.j:
                    return Gdk.Key.J;
                case Gdk.Key.k:
                    return Gdk.Key.K;
                case Gdk.Key.l:
                    return Gdk.Key.L;
                case Gdk.Key.m:
                    return Gdk.Key.M;
                case Gdk.Key.n:
                    return Gdk.Key.N;
                case Gdk.Key.o:
                    return Gdk.Key.O;
                case Gdk.Key.p:
                    return Gdk.Key.P;
                case Gdk.Key.q:
                    return Gdk.Key.Q;
                case Gdk.Key.r:
                    return Gdk.Key.R;
                case Gdk.Key.s:
                    return Gdk.Key.S;
                case Gdk.Key.t:
                    return Gdk.Key.T;
                case Gdk.Key.u:
                    return Gdk.Key.U;
                case Gdk.Key.v:
                    return Gdk.Key.V;
                case Gdk.Key.w:
                    return Gdk.Key.W;
                case Gdk.Key.x:
                    return Gdk.Key.X;
                case Gdk.Key.y:
                    return Gdk.Key.Y;
                case Gdk.Key.z:
                    return Gdk.Key.Z;
            }

            return key;
        }

        /// <summary>
        /// Convert string to key
        /// </summary>
        /// <param name="Key">Key</param>
        /// <returns></returns>
        public static Gdk.Key ParseKey(string Key)
        {
            switch (Key.ToLower())
            {
                case "a":
                    return Gdk.Key.A;
                case "b":
                    return Gdk.Key.B;
                case "c":
                    return Gdk.Key.C;
                case "d":
                    return Gdk.Key.D;
                case "e":
                    return Gdk.Key.E;
                case "f":
                    return Gdk.Key.F;
                case "g":
                    return Gdk.Key.G;
                case "h":
                    return Gdk.Key.H;
                case "i":
                    return Gdk.Key.I;
                case "j":
                    return Gdk.Key.J;
                case "k":
                    return Gdk.Key.K;
                case "l":
                    return Gdk.Key.L;
                case "m":
                    return Gdk.Key.M;
                case "n":
                    return Gdk.Key.N;
                case "o":
                    return Gdk.Key.O;
                case "p":
                    return Gdk.Key.P;
                case "q":
                    return Gdk.Key.Q;
                case "r":
                    return Gdk.Key.R;
                case "s":
                    return Gdk.Key.S;
                case "t":
                    return Gdk.Key.T;
                case "u":
                    return Gdk.Key.U;
                case "v":
                    return Gdk.Key.V;
                case "x":
                    return Gdk.Key.X;
                case "w":
                    return Gdk.Key.W;
                case "y":
                    return Gdk.Key.Y;
                case "z":
                    return Gdk.Key.Z;
                case "1":
                    return Gdk.Key.Key_1;
                case "2":
                    return Gdk.Key.Key_2;
                case "3":
                    return Gdk.Key.Key_3;
                case "4":
                    return Gdk.Key.Key_4;
                case "5":
                    return Gdk.Key.Key_5;
                case "6":
                    return Gdk.Key.Key_6;
                case "7":
                    return Gdk.Key.Key_7;
                case "8":
                    return Gdk.Key.Key_8;
                case "9":
                    return Gdk.Key.Key_9;
                case "0":
                    return Gdk.Key.Key_0;
                case "f1":
                    return Gdk.Key.F1;
                case "f2":
                    return Gdk.Key.F2;
                case "f3":
                    return Gdk.Key.F3;
                case "home":
                    return Gdk.Key.Home;
                case "end":
                    return Gdk.Key.End;
                case "pageup":
                    return Gdk.Key.Page_Up;
                case "delete":
                    return Gdk.Key.Delete;
            }
            return Gdk.Key.A;
        }

        /// <summary>
        /// Load skin
        /// </summary>
        /// <returns></returns>
        public static bool LoadSkin()
        {
            Configuration.SL.Clear();
            Configuration.SL.Add(new Skin());
            if (Configuration.Kernel.Safe)
            {
                Syslog.DebugLog("Skipping load of skins because of safe mode");
            }
            else
            {
                Syslog.DebugLog("Loading skins");
                if (Directory.Exists(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + SkinPath))
                {
                    string[] skin = Directory.GetFiles(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + SkinPath);
                    {
                        foreach (string file in skin)
                        {
                            if (file.EndsWith(".ps", StringComparison.Ordinal))
                            {
                                Skin curr = new Skin(file);
                                if (curr == null)
                                {
                                    continue;
                                }
                                Configuration.SL.Add(curr);
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the unused system port.
        /// </summary>
        /// <returns>The unused system port.</returns>
        public static uint GetUnusedSystemPort()
        {
            uint port = Configuration.irc.DefaultCTCPPort;
            lock (LockedPorts)
            {
                while (LockedPorts.Contains(port))
                {
                    port++;
                }
                LockedPorts.Add(port);
            }
            return port;
        }

        /// <summary>
        /// Return IP address of this machine
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            IPHostEntry host;
            string localIP = null;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        /// <summary>
        /// Convert a unix timestamp to human readable time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ConvertFromUNIXToString(string time)
        {
            try
            {
                if (time == null)
                {
                    return "Unable to read: NULL";
                }
                double unixtimestmp = double.Parse(time);
                return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixtimestmp).ToString();
            }
            catch (Exception)
            {
                return "Unable to read: " + time;
            }
        }

        /// <summary>
        /// Convert the xml collection to dictionary
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static Dictionary<string, string> XmlCollectionToDict(XmlAttributeCollection collection)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            int curr = 0;
            while (curr < collection.Count)
            {
                if (!data.ContainsKey(collection[curr].Name))
                {
                    data.Add(collection[curr].Name, collection[curr].Value);
                }
                curr++;
            }
            return data;
        }

        /// <summary>
        /// See the comments in recovery.cs for explanation how this work
        /// </summary>
        /// <param name="_exception"></param>
        /// <param name="ek"></param>
        /// <returns></returns>
        public static int HandleException(Exception _exception, ExceptionKind ek)
        {
            if (IgnoreErrors || ek == ExceptionKind.Safe)
            {
                Console.WriteLine("EXCEPTION: " + _exception.StackTrace);
                return 2;
            }
            if (ek == ExceptionKind.Critical)
            {
                RecoveryIsFatal = true;
            }
            Syslog.DebugLog(_exception.Message + " at " + _exception.Source + " info: " + _exception.Data.ToString());
            IsBlocked = true;
            RecoveryException = _exception;
            if (Configuration.Kernel.KernelDump)
            {
                Syslog.DebugLog("Generating report");
                string dump = "KERNEL DUMP\n\n";
                dump += "Time: " + DateTime.Now.ToString() + "\n";
                dump += "Version: " + Application.ProductVersion + RevisionProvider.GetHash() + "\n";
                dump += "Extensions: " + "\n";
                lock (ExtensionPool.Extensions)
                {
                    foreach (Extension xx in ExtensionPool.Extensions)
                    {
                        dump += "  " + xx.Name + " version: " + xx.Version + " status: " + xx._Status.ToString() + "\n";
                    }
                }
                dump += "Exception: " + _exception.Message + "\n";
                dump += "Source: " + _exception.Source + "\n";
                dump += "Status: " + CoreState.ToString() + "\n";
                dump += "Stack trace: " + _exception.StackTrace + "\n";
                if (_exception.InnerException != null)
                {
                    dump += "Inner: " + _exception.InnerException.ToString() + "\n";
                }
                dump += "Thread name: " + Thread.CurrentThread.Name + "\n";
                dump += "Is kernel: " + (Thread.CurrentThread != _KernelThread).ToString() + "\n";
                dump += "Ring log:\n";
                foreach (string line in RingBuffer)
                {
                    dump += line + "\n";
                }
                dump += "That's all folks";
                int current = 0;
                string file = RootDirectory + "dump__" + current.ToString();
                while (File.Exists(file))
                {
                    current++;
                    file = RootDirectory + "dump__" + current.ToString();
                }
                File.WriteAllText(file, dump);
            }
            ThreadManager.RecoveryThread = new Thread(Recover);
            ThreadManager.RecoveryThread.Start();
            if (Thread.CurrentThread != _KernelThread)
            {
                Syslog.DebugLog("Warning, the thread which raised the exception is not a core thread, identifier: " + Thread.CurrentThread.Name);
            }
            while (IsBlocked || RecoveryIsFatal)
            {
                Thread.Sleep(100);
            }
            return 0;
        }

        /// <summary>
        /// Convert a Color to Gdk version
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Gdk.Color FromColor(System.Drawing.Color color)
        {
            Gdk.Color xx = new Gdk.Color(color.R, color.G, color.B);
            return xx;
        }
        
        /// <summary>
        /// Recover from exception
        /// </summary>
        /// <param name="_exception"></param>
        /// <param name="fatal"></param>
        /// <returns></returns>
        public static int HandleException(Exception _exception, bool fatal = false)
        {
            if (fatal)
            {
                HandleException(_exception, ExceptionKind.Critical);
            }
            else
            {
                HandleException(_exception, ExceptionKind.Normal);
            }
            return 0;
        }

        /// <summary>
        /// Exit
        /// </summary>
        /// <returns>true</returns>
        public static bool Quit()
        {
            try
            {
                Syslog.DebugLog("User requested a shut down");
                if (CoreState == State.Terminating)
                {
                    Syslog.DebugLog("Multiple calls of Core.Quit() ignored");
                    return false;
                }
                CoreState = State.Terminating;
                if (!IgnoreErrors)
                {
                    IgnoreErrors = true;
                    if (Configuration.UserData.TrayIcon)
                    {
                        SystemForm.icon.Visible = false;
                        SystemForm.icon.Dispose();
                    }
                    SystemForm.Hide();
                    NotificationWidget.Hide();
                    Configuration.Save();
                    try
                    {
                        foreach (Protocol server in Connections.ConnectionList)
                        {
                            Core.Ringlog("Exiting network " + server.Server);
                            server.Exit();
                        }
                    }
                    catch (Exception fail)
                    {
                        Core.HandleException(fail);
                    }
                    foreach (Thread th in ThreadManager.Threads)
                    {
                        try
                        {
                            Core.Ringlog("CORE: Thread " + th.ManagedThreadId.ToString() + " (" + th.Name + ") needs to be terminated now");
                            Core.ThreadManager.KillThread(th);
                        }
                        catch (Exception fail)
                        {
                            Core.HandleException(fail);
                        }
                    }
                    Thread.Sleep(800);
                    Syslog.DebugLog("Exiting with code 0");
                    Environment.Exit(0);
                }
            }
            catch (Exception fail)
            {
                Console.WriteLine(fail.StackTrace.ToString());
                if (KernelProc != null)
                {
                    KernelProc.Kill();
                }
            }
            return true;
        }

        /// <summary>
        /// Type of exception
        /// </summary>
        public enum ExceptionKind
        {
            /// <summary>
            /// Normal
            /// </summary>
            Normal,
            /// <summary>
            /// Critical
            /// </summary>
            Critical,
            /// <summary>
            /// Safe
            /// </summary>
            Safe,
        }

        /// <summary>
        /// Platform
        /// </summary>
        public enum Platform
        {
            /// <summary>
            /// Linux
            /// </summary>
            Linuxx64,
            /// <summary>
            /// Linux
            /// </summary>
            Linuxx86,
            /// <summary>
            /// Windows
            /// </summary>
            Windowsx64,
            /// <summary>
            /// Windows
            /// </summary>
            Windowsx86,
            /// <summary>
            /// Mac OS
            /// </summary>
            MacOSx64,
            /// <summary>
            /// Max OS
            /// </summary>
            MacOSx86,
        }

        /// <summary>
        /// Which status the core is in
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Everything is OK
            /// </summary>
            Running,
            /// <summary>
            /// There is exception being handled
            /// </summary>
            Frozen,
            /// <summary>
            /// Terminating
            /// </summary>
            Terminating,
            /// <summary>
            /// Loading
            /// </summary>
            Loading,
            /// <summary>
            /// Killed
            /// </summary>
            Killed,
        }
    }

    /// <summary>
    /// This is an attribute that should be used for all new functions
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class Experimental : Attribute
    { }
}
