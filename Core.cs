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

using System.IO;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Client
{
    public class Core
    {
        public enum Platform
        {
            Linuxx64,
            Linuxx86,
            Windowsx64,
            Windowsx86,
            MacOSx64,
            MacOSx86,
        }

        /// <summary>
        /// Thread of core
        /// </summary>
        public static Thread _KernelThread;
        /// <summary>
        /// Exact time of system load
        /// </summary>
        public static DateTime LoadTime;
        /// <summary>
        /// Configuration path
        /// </summary>
        public static string ConfigFile = System.Windows.Forms.Application.LocalUserAppDataPath + Path.DirectorySeparatorChar + "configuration.dat";
        /// <summary>
        /// Language used in system
        /// </summary>
        public static string SelectedLanguage = "en";
        /// <summary>
        /// List of active networks in system
        /// </summary>
        public static List<Protocol> Connections = new List<Protocol>();
        /// <summary>
        /// Thread for IO logs
        /// </summary>
        public static Thread Thread_logs = null;
        /// <summary>
        /// Thread for update system
        /// </summary>
        public static Thread ThUp = null;
        /// <summary>
        /// Module layers
        /// </summary>
        public static List<Domain> domains = new List<Domain>();
        /// <summary>
        /// Pointer to exception class during recovery
        /// </summary>
        public static Exception recovery_exception = null;
        /// <summary>
        /// Recovery thread
        /// </summary>
        public static Thread _RecoveryThread = null;
        /// <summary>
        /// Timers
        /// </summary>
        public static List<Timer> TimerDB = new List<Timer>();
        /// <summary>
        /// Notification box
        /// </summary>
        private static Notification notification = null;
        /// <summary>
        /// Threads currently allocated in kernel
        /// </summary>
        public static List<Thread> SystemThreads = new List<Thread>();
        /// <summary>
        /// Ring log
        /// </summary>
        private static List<string> Ring = new List<string>();
        /// <summary>
        /// Selected network
        /// </summary>
        public static Network network = null;
        /// <summary>
        /// Main
        /// </summary>
        public static Main _Main = null;
        /// <summary>
        /// Wheter notification is waiting
        /// </summary>
        public static bool notification_waiting = false;
        /// <summary>
        /// Data of notification (text)
        /// </summary>
        private static string notification_data = "";
        /// <summary>
        /// Caption
        /// </summary>
        private static string notification_caption = "";
        /// <summary>
        /// This is index of last random number, you should never write or read this value, except for its own function
        /// </summary>
        private static int randomuq = 0;
        /// <summary>
        /// Path to skin
        /// </summary>
        public static string SkinPath = "Skin";
        /// <summary>
        /// Packet scan
        /// </summary>
        public static TrafficScanner trafficscanner;
        /// <summary>
        /// System is blocked - if this is set to true, all subsystems and kernel are supposed to freeze
        /// </summary>
        public static bool blocked = false;
        /// <summary>
        /// Ignore errors - all exceptions and debug logs are ignored and pidgeon is flagged as unstable
        /// </summary>
        public static bool IgnoreErrors = false;
        /// <summary>
        /// If this is true the recovery window will not allow to ignore
        /// </summary>
        public static bool recovery_fatal = false;
        /// <summary>
        /// Parameters that were retrieved in console (deprecated)
        /// </summary>
        public static string[] startup = null;
        /// <summary>
        /// Cache of current params
        /// </summary>
        private static List<string> startup_params = new List<string>();
        /// <summary>
        /// Extensions loaded in core
        /// </summary>
        public static List<Extension> Extensions = new List<Extension>();

        /// <summary>
        /// Parameters that were retrieved in console
        /// </summary>
        public static List<string> Parameters
        {
            get
            {
                List<string> data = new List<string>();
                lock (startup)
                {
                    data.AddRange(startup_params);
                }
                return data;
            }
        }

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

        public class Domain
        {
            AppDomain domain;
            string name;
            public Domain(AppDomain _appDomain, string _name)
            {
                domain = _appDomain;
                name = _name;
            }
        }

        public class IO
        {
            public class FileLine
            {
                public string filename;
                public string line;
                public FileLine(string File, string Line)
                {
                    line = Line;
                    filename = File;
                }
            }

            public static List<FileLine> processing = new List<FileLine>();
            public static List<FileLine> data = new List<FileLine>();

            public static void Save()
            {
                lock (processing)
                {
                    if (processing.Count > 0)
                    {
                        data.AddRange(processing);
                        processing.Clear();
                    }
                }
                if (data.Count > 0)
                {
                    foreach (FileLine xx in data)
                    {
                        File.AppendAllText(xx.filename, xx.line);
                    }
                }
                data.Clear();
            }

            public static void Load()
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            Save();
                        }
                        catch (ThreadAbortException)
                        {
                            Save();
                            return;
                        }
                        catch (Exception e1)
                        {
                            Core.handleException(e1);
                        }
                        Thread.Sleep(2000);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }

            public static void InsertText(string line, string file)
            {
                lock (processing)
                {
                    processing.Add(new FileLine(file, line));
                }
            }
        }

        public class Shortcut
        {
            public bool control;
            public bool alt;
            public bool shift;
            public System.Windows.Forms.Keys keys;
            public string data;
            public Shortcut(System.Windows.Forms.Keys Value, bool Control = false, bool Alt = false, bool Shift = false, string Data = "")
            {
                control = Control;
                shift = Shift;
                alt = Alt;
                data = Data;
                keys = Value;
            }
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <returns></returns>
        public static bool Load()
        {
            try
            {
                _KernelThread = System.Threading.Thread.CurrentThread;
                LoadTime = DateTime.Now;
                Ringlog("Pidgeon " + Application.ProductVersion.ToString() + " loading core");
                foreach (string data in startup)
                {
                    startup_params.Add(data);
                }
                if (Application.LocalUserAppDataPath.EndsWith(Application.ProductVersion))
                {
                    ConfigFile = Application.LocalUserAppDataPath.Substring(0, Application.LocalUserAppDataPath.Length - Application.ProductVersion.Length) + "configuration.dat";
                }
                string is64 = " which is a 32 bit system";
                if (Environment.Is64BitOperatingSystem)
                {
                    is64 = " which is a 64 bit system";
                }
                Ringlog("This pidgeon is compiled for " + Configuration.CurrentPlatform.ToString() + " and running on " + Environment.OSVersion.ToString() + is64);
                DebugLog("Loading messages");
                messages.data.Add("en", new messages.container("en"));
                messages.data.Add("cs", new messages.container("cs"));
                trafficscanner = new TrafficScanner();
                if (!System.IO.File.Exists(Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "pidgeon.dat"))
                {
                    LoadSkin();
                    DebugLog("Loading configuration file");
                    ConfigurationLoad();
                    DebugLog("Running updater");
                    ThUp = new Thread(Updater.Run);
                    ThUp.Name = "pidgeon service";
                    ThUp.Start();
                    SystemThreads.Add(ThUp);
                    DebugLog("Loading log writer thread");
                    Thread_logs = new Thread(IO.Load);
                    Thread_logs.Name = "Logs";
                    SystemThreads.Add(Thread_logs);
                    Thread_logs.Start();
                    DebugLog("Loading commands");
                    Commands.Initialise();
                    MicroChat.mc = new MicroChat();
                    notification = new Notification();
                    DebugLog("Loading scripting core");
                    ScriptingCore.Load();
                    DebugLog("Loading extensions");
                    Extension.Init();
                    if (Directory.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "modules"))
                    {
                        foreach (string dll in Directory.GetFiles(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "modules", "*.pmod"))
                        {
                            DebugLog("Registering plugin " + dll);
                            RegisterPlugin(dll);
                        }
                    }
                    if (!File.Exists(ConfigFile))
                    {
                        Network.Highlighter simple = new Network.Highlighter();
                        simple.enabled = true;
                        simple.text = "$nick";
                        Configuration.HighlighterList.Add(simple);
                    }
                    Hooks._Sys.AfterCore();
                    return true;
                }
                Updater _finalisingupdater = new Updater();
                _finalisingupdater.update.Visible = false;
                _finalisingupdater.finalize = true;
                _finalisingupdater.lStatus.Text = messages.get("update2");
                System.Windows.Forms.Application.Run(_finalisingupdater);
            }
            catch (Exception panic)
            {
                Core.DebugLog("Failed to Core.Load: " + panic.Message + panic.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Connect to irc link
        /// </summary>
        /// <param name="text">link</param>
        /// <param name="services"></param>
        public static void ParseLink(string text, ProtocolSv services = null)
        {
            if (text.StartsWith("irc://"))
            {
                text = text.Substring("irc://".Length);
                string network = text;
                string channel = null;
                int PORT = 6667;
                if (network.Contains("#"))
                {
                    channel = network.Substring(network.IndexOf("#"));
                }
                network = network.Substring(0, network.IndexOf("#"));
                if (network.Contains(":"))
                {
                    string port = network.Substring(network.IndexOf(":") + 1);
                    network = network.Substring(0, network.IndexOf(port));
                    if (port.Contains("/"))
                    {
                        port = port.Substring(0, port.IndexOf("/"));
                    }
                    if (port.Contains("#"))
                    {
                        port = port.Substring(0, port.IndexOf("#"));
                    }
                    if (!int.TryParse(port, out PORT))
                    {
                        PORT = 6667;
                    }
                }
                ProtocolIrc server = null;
                foreach (Protocol protocol in Connections)
                {
                    if (typeof(ProtocolIrc) == protocol.GetType() && protocol.Server == network)
                    {
                        server = (ProtocolIrc)protocol;
                        break;
                    }
                }

                if (server == null)
                {
                    server = connectIRC(network, PORT);
                }

                if (channel != null)
                {
                    server._IRCNetwork.Join(channel);
                }
            }
        }

        /// <summary>
        /// Recover from crash
        /// </summary>
        public static void Recover()
        {
            Recovery x = new Recovery();
            System.Windows.Forms.Application.Run(x);
        }

        public static void killThread(Thread name)
        {
            if (name.ThreadState == System.Threading.ThreadState.Running || name.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
            {
                name.Abort();
                Core.DebugLog("Killed thread " + name.Name);
            }
            else
            {
                Core.DebugLog("Ignored request to abort thread in " + name.ThreadState.ToString() + name.Name);
            }

            if (Core.IgnoreErrors)
            {
                DebugLog("Not removing thread from thread queue " + name.Name + " because system is shutting down");
                return;
            }

            lock (SystemThreads)
            {
                if (SystemThreads.Contains(name))
                {
                    SystemThreads.Remove(name);
                }
            }
        }

        /// <summary>
        /// Remove special symbols
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveSpecial(string text)
        {
            return text.Replace("%/USER%", "")
                .Replace("%USER%", "")
                .Replace("%H%", "")
                .Replace("%/H%", "")
                .Replace("%D%", "")
                .Replace("%L%", "")
                .Replace("%/L%", "")
                .Replace("%/D%", "")
                .Replace(((char)001).ToString(), "")
                .Replace(((char)002).ToString(), "")
                .Replace(((char)003).ToString(), "")
                .Replace(((char)004).ToString(), "");
        }

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
        /// <param name="data"></param>
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
            Console.WriteLine(text);
        }

        /// <summary>
        /// Insert text in to debug log
        /// </summary>
        /// <param name="data">Text to insert</param>
        public static void DebugLog(string data)
        {
            try
            {
                System.Diagnostics.Debug.Print(data);
                if (Configuration.Kernel.Debugging)
                {
                    if (Core._Main != null && !Core.blocked)
                    {
                        if (Core._Main.main != null)
                        {
                            Core._Main.main.scrollback.InsertText("DEBUG: " + data, Scrollback.MessageStyle.System, false);
                        }
                    }
                }
                Ringlog("DEBUG: " + data);
            }
            catch (Exception er)
            {
                Core.handleException(er);
            }
        }

        public static void PrintRing(Window window, bool write = true)
        {
            lock (Ring)
            {
                foreach (string item in Ring)
                {
                    window.scrollback.InsertText(item, Scrollback.MessageStyle.System, write, 0, true);
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
            try
            {
                if (command.StartsWith(Configuration.CommandPrefix))
                {
                    command = command.Substring(1);
                }

                // in case that it's known command we return
                if (Commands.Proccess(command))
                {
                    return true;
                }

                string[] values = command.Split(' ');

                // if not we can try to pass it to server
                if (Core._Main.Chat._Protocol != null)
                {
                    if (_Main.Chat._Protocol.Connected)
                    {
                        Core._Main.Chat._Protocol.Command(command);
                        return false;
                    }
                }
                _Main.Chat.scrollback.InsertText(messages.get("invalid-command", SelectedLanguage), Scrollback.MessageStyle.System);
                return false;
            }
            catch (Exception f)
            {
                handleException(f);
            }
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
            DebugLog("Restoring file " + file + " from a backup");
            if (File.Exists(file))
            {
                File.Copy(file, backup);
                DebugLog("Stored previous version: " + backup);
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
            if (_KernelThread == Thread.CurrentThread)
            {
                if (notification_waiting)
                {
                    bool Focus = false;
                    notification.text.Text = notification_data;
                    notification.label1.Text = notification_caption;
                    notification_waiting = false;
                    if (Core._Main.Chat != null)
                    {
                        if (Core._Main.Chat.textbox.richTextBox1.Focused)
                        {
                            Focus = true;
                        }
                    }
                    if (!notification.Visible)
                    {
                        if (notification.Width < System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width && notification.Height < System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height)
                        {
                            notification.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - notification.Height;
                            notification.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - notification.Width;
                        }
                        notification.Show();
                        if (Focus)
                        {
                            Core._Main.Focus();
                            if (Core._Main.Chat != null)
                            {
                                Core._Main.Chat.textbox.Focus();
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
        /// <param name="caption">Text</param>
        public static void DisplayNote(string data, string caption)
        {
            if (Configuration.Kernel.Notice == false)
            {
                return;
            }
            data = Protocol.decode_text(Core.RemoveSpecial(data));
            notification_waiting = true;
            notification_data = data;
            notification_caption = caption;
            if (_KernelThread == Thread.CurrentThread)
            {
                DisplayNote();
            }
        }

        /// <summary>
        /// Backup a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static string retrieveRandom()
        {
            int random = 0;
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(_KernelThread, ref lockWasTaken);
                randomuq++;
                random = randomuq;
            }
            finally
            {
                if (lockWasTaken)
                {
                    Monitor.Exit(_KernelThread);
                }
            }
            return ":" + random.ToString() + "*";
        }

        public static void ProcessScript(string script, Network target)
        {
            try
            {
                Forms.ScriptEdit edit = new Forms.ScriptEdit();
                string[] text = script.Split('\n');
                foreach (string line in text)
                {
                    edit.textBox1.AppendText(line + Environment.NewLine);
                }
                edit.network = target;
                edit.Show();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Create a node
        /// </summary>
        /// <param name="config_key">Key</param>
        /// <param name="text">Text</param>
        /// <param name="xmlnode">Node</param>
        /// <param name="key">Key</param>
        /// <param name="_c">Config</param>
        /// <param name="conf">Node</param>
        /// <param name="nn">Config name</param>
        /// <returns></returns>
        private static bool makenode(string config_key, string text, XmlNode xmlnode, XmlAttribute key, XmlDocument _c, XmlNode conf, string nn = "confname")
        {
            key = _c.CreateAttribute(nn);
            xmlnode = _c.CreateElement("data");
            key.Value = config_key;
            xmlnode.Attributes.Append(key);
            xmlnode.InnerText = text;
            conf.AppendChild(xmlnode);
            return true;
        }

        public static bool ConfigSave()
        {
            try
            {
                System.Xml.XmlDocument config = new System.Xml.XmlDocument();
                XmlComment notice = config.CreateComment("This is a configuration file of pidgeon client, see http://pidgeonclient.org/wiki/Help:Configuration");
                System.Xml.XmlNode xmlnode = config.CreateElement("configuration.pidgeon");
                System.Xml.XmlNode curr = null;
                XmlAttribute confname = null;
                makenode("network.ident", Configuration.UserData.ident, curr, confname, config, xmlnode);
                makenode("quitmessage", Configuration.UserData.quit, curr, confname, config, xmlnode);
                makenode("network.nick", Configuration.UserData.nick, curr, confname, config, xmlnode);
                makenode("scrollback.showctcp", Configuration.irc.DisplayCtcp.ToString(), curr, confname, config, xmlnode);
                makenode("formats.user", Configuration.Scrollback.format_nick, curr, confname, config, xmlnode);
                makenode("formats.date", Configuration.Scrollback.format_date, curr, confname, config, xmlnode);
                makenode("irc.auto.whois", Configuration.ChannelModes.aggressive_whois.ToString(), curr, confname, config, xmlnode);
                makenode("irc.auto.mode", Configuration.ChannelModes.aggressive_mode.ToString(), curr, confname, config, xmlnode);
                makenode("irc.auto.channels", Configuration.ChannelModes.aggressive_channel.ToString(), curr, confname, config, xmlnode);
                makenode("irc.auto.bans", Configuration.ChannelModes.aggressive_bans.ToString(), curr, confname, config, xmlnode);
                makenode("irc.auto.exception", Configuration.ChannelModes.aggressive_exception.ToString(), curr, confname, config, xmlnode);
                makenode("irc.auto.invites", Configuration.ChannelModes.aggressive_invites.ToString(), curr, confname, config, xmlnode);

                makenode("location.maxi", Configuration.Window.Window_Maximized.ToString(), curr, confname, config, xmlnode);
                makenode("window.size", Configuration.Window.window_size.ToString(), curr, confname, config, xmlnode);
                makenode("location.x1", Configuration.Window.x1.ToString(), curr, confname, config, xmlnode);
                makenode("location.x4", Configuration.Window.x4.ToString(), curr, confname, config, xmlnode);
                makenode("logs.dir", Configuration.Logs.logs_dir, curr, confname, config, xmlnode);
                makenode("logs.type", Configuration.Logs.logs_name, curr, confname, config, xmlnode);
                makenode("ignore.ctcp", Configuration.irc.DisplayCtcp.ToString(), curr, confname, config, xmlnode);
                makenode("logs.html", Configuration.Logs.logs_html.ToString(), curr, confname, config, xmlnode);
                makenode("logs.xml", Configuration.Logs.logs_xml.ToString(), curr, confname, config, xmlnode);
                makenode("logs.txt", Configuration.Logs.logs_txt.ToString(), curr, confname, config, xmlnode);
                makenode("updater.check", Configuration.Kernel.CheckUpdate.ToString(), curr, confname, config, xmlnode);
                makenode("debugger", Configuration.Kernel.Debugging.ToString(), curr, confname, config, xmlnode);

                makenode("history.nick", Configuration.UserData.LastNick, curr, confname, config, xmlnode);
                makenode("scrollback_plimit", Configuration.Scrollback.scrollback_plimit.ToString(), curr, confname, config, xmlnode);
                makenode("history.host", Configuration.UserData.LastHost, curr, confname, config, xmlnode);
                makenode("history.port", Configuration.LastPort, curr, confname, config, xmlnode);
                makenode("confirm.all", Configuration.ConfirmAll.ToString(), curr, confname, config, xmlnode);
                makenode("notification.tray", Configuration.Kernel.Notice.ToString(), curr, confname, config, xmlnode);
                makenode("sniffer", Configuration.Kernel.NetworkSniff.ToString(), curr, confname, config, xmlnode);
                makenode("pidgeon.size", Configuration.Services.Depth.ToString(), curr, confname, config, xmlnode);
                makenode("skin", Configuration.CurrentSkin.name, curr, confname, config, xmlnode);
                makenode("default.reason", Configuration.irc.DefaultReason, curr, confname, config, xmlnode);
                makenode("network.n2", Configuration.UserData.Nick2, curr, confname, config, xmlnode);
                makenode("colors.changelinks", Configuration.Colors.ChangeLinks.ToString(), curr, confname, config, xmlnode);

                foreach (KeyValuePair<string, string> data in Configuration.Extensions)
                {
                    makenode("extension." + data.Key, data.Value, curr, confname, config, xmlnode);
                }

                string separators = "";
                foreach (char separator in Configuration.Parser.Separators)
                {
                    separators = separators + separator.ToString();
                }
                makenode("delimiters", separators, curr, confname, config, xmlnode);

                lock (Configuration.Parser.Protocols)
                {
                    foreach (string xx in Configuration.Parser.Protocols)
                    {
                        curr = config.CreateElement("protocol");
                        curr.InnerText = xx;
                        xmlnode.AppendChild(curr);
                    }
                }

                lock (Configuration.HighlighterList)
                {
                    foreach (Network.Highlighter high in Configuration.HighlighterList)
                    {
                        curr = config.CreateElement("list");
                        XmlAttribute highlightenabled = config.CreateAttribute("enabled");
                        XmlAttribute highlighttext = config.CreateAttribute("text");
                        XmlAttribute highlightsimple = config.CreateAttribute("simple");
                        highlightenabled.Value = "true";
                        highlightsimple.Value = high.simple.ToString();
                        highlighttext.Value = high.text;
                        curr.Attributes.Append(highlightsimple);
                        curr.Attributes.Append(highlighttext);
                        curr.Attributes.Append(highlightenabled);
                        xmlnode.AppendChild(curr);
                    }
                }
                config.AppendChild(xmlnode);

                lock (Ignoring.IgnoreList)
                {
                    foreach (Ignoring.Ignore ci in Ignoring.IgnoreList)
                    {
                        curr = config.CreateElement("ignore");
                        XmlAttribute enabled = config.CreateAttribute("enabled");
                        XmlAttribute simple = config.CreateAttribute("simple");
                        XmlAttribute text = config.CreateAttribute("text");
                        XmlAttribute type = config.CreateAttribute("type");
                        enabled.Value = ci.Enabled.ToString();
                        simple.Value = ci.Simple.ToString();
                        text.Value = ci.Text;
                        type.Value = ci.type.ToString();
                        curr.Attributes.Append(enabled);
                        curr.Attributes.Append(simple);
                        curr.Attributes.Append(text);
                        curr.Attributes.Append(type);
                        xmlnode.AppendChild(curr);
                    }
                }
                config.AppendChild(xmlnode);

                lock (Configuration.ShortcutKeylist)
                {
                    foreach (Shortcut RR in Configuration.ShortcutKeylist)
                    {
                        curr = config.CreateElement("shortcut");
                        XmlAttribute name = config.CreateAttribute("name");
                        XmlAttribute ctrl = config.CreateAttribute("ctrl");
                        XmlAttribute alt = config.CreateAttribute("alt");
                        XmlAttribute shift = config.CreateAttribute("shift");
                        curr.InnerText = RR.data;
                        name.Value = RR.keys.ToString();
                        shift.Value = RR.shift.ToString();
                        ctrl.Value = RR.control.ToString();
                        alt.Value = RR.alt.ToString();
                        curr.Attributes.Append(name);
                        curr.Attributes.Append(ctrl);
                        curr.Attributes.Append(alt);
                        curr.Attributes.Append(shift);
                        xmlnode.AppendChild(curr);
                    }
                }
                config.AppendChild(xmlnode);

                if (Backup(ConfigFile))
                {
                    config.Save(ConfigFile);
                }
                File.Delete(ConfigFile + "~");
            }
            catch (Exception t)
            {
                Core.handleException(t);
            }
            return false;
        }

        /// <summary>
        /// Convert string to key
        /// </summary>
        /// <param name="Key">Key</param>
        /// <returns></returns>
        public static System.Windows.Forms.Keys parseKey(string Key)
        {
            switch (Key.ToLower())
            {
                case "a":
                    return System.Windows.Forms.Keys.A;
                case "b":
                    return System.Windows.Forms.Keys.B;
                case "c":
                    return System.Windows.Forms.Keys.C;
                case "d":
                    return System.Windows.Forms.Keys.D;
                case "e":
                    return System.Windows.Forms.Keys.E;
                case "f":
                    return System.Windows.Forms.Keys.F;
                case "g":
                    return System.Windows.Forms.Keys.G;
                case "h":
                    return System.Windows.Forms.Keys.H;
                case "i":
                    return System.Windows.Forms.Keys.I;
                case "j":
                    return System.Windows.Forms.Keys.J;
                case "k":
                    return System.Windows.Forms.Keys.K;
                case "l":
                    return System.Windows.Forms.Keys.L;
                case "m":
                    return System.Windows.Forms.Keys.M;
                case "n":
                    return System.Windows.Forms.Keys.N;
                case "o":
                    return System.Windows.Forms.Keys.O;
                case "p":
                    return System.Windows.Forms.Keys.P;
                case "q":
                    return System.Windows.Forms.Keys.Q;
                case "r":
                    return System.Windows.Forms.Keys.R;
                case "s":
                    return System.Windows.Forms.Keys.S;
                case "t":
                    return System.Windows.Forms.Keys.T;
                case "u":
                    return System.Windows.Forms.Keys.U;
                case "v":
                    return System.Windows.Forms.Keys.V;
                case "x":
                    return System.Windows.Forms.Keys.X;
                case "w":
                    return System.Windows.Forms.Keys.W;
                case "y":
                    return System.Windows.Forms.Keys.Y;
                case "z":
                    return System.Windows.Forms.Keys.Z;
                case "1":
                    return System.Windows.Forms.Keys.D1;
                case "2":
                    return System.Windows.Forms.Keys.D2;
                case "3":
                    return System.Windows.Forms.Keys.D3;
                case "4":
                    return System.Windows.Forms.Keys.D4;
                case "5":
                    return System.Windows.Forms.Keys.D5;
                case "6":
                    return System.Windows.Forms.Keys.D6;
                case "7":
                    return System.Windows.Forms.Keys.D7;
                case "8":
                    return System.Windows.Forms.Keys.D8;
                case "9":
                    return System.Windows.Forms.Keys.D9;
                case "0":
                    return System.Windows.Forms.Keys.D0;
                case "f1":
                    return System.Windows.Forms.Keys.F1;
                case "f2":
                    return System.Windows.Forms.Keys.F2;
                case "f3":
                    return System.Windows.Forms.Keys.F3;
                case "home":
                    return System.Windows.Forms.Keys.Home;
                case "end":
                    return System.Windows.Forms.Keys.End;
                case "pageup":
                    return System.Windows.Forms.Keys.PageUp;
                case "delete":
                    return System.Windows.Forms.Keys.Delete;
            }
            return System.Windows.Forms.Keys.A;
        }

        public static bool LoadSkin()
        {
            Configuration.SL.Clear();
            Configuration.SL.Add(new Skin());
            DebugLog("Loading skins");
            if (Directory.Exists(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + SkinPath))
            {
                string[] skin = Directory.GetFiles(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + SkinPath);
                {
                    foreach (string file in skin)
                    {
                        if (file.EndsWith(".ps"))
                        {
                            Skin curr = new Skin(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + SkinPath
                                + Path.DirectorySeparatorChar + file);
                            if (curr == null)
                            {
                                continue;
                            }
                            Configuration.SL.Add(curr);
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Conf
        /// </summary>
        /// <returns></returns>
        public static bool ConfigurationLoad()
        {
            // Check if config file is present
            Restore(ConfigFile);
            bool Protocols = false;
            if (File.Exists(ConfigFile))
            {
                try
                {
                    XmlDocument configuration = new XmlDocument();
                    lock (Configuration.ShortcutKeylist)
                    {
                        Configuration.ShortcutKeylist = new List<Shortcut>();
                    }
                    lock (Configuration.HighlighterList)
                    {
                        Configuration.HighlighterList = new List<Network.Highlighter>();
                    }
                    configuration.Load(ConfigFile);

                    foreach (XmlNode curr in configuration.ChildNodes[0].ChildNodes)
                    {
                        if (curr.Attributes.Count > 0)
                        {
                            if (curr.Name.StartsWith("extension."))
                            {
                                Configuration.SetConfig(curr.Name.Substring(10), curr.InnerText);
                                continue;
                            }
                            if (curr.Name == "ignore")
                            {
                                if (curr.Attributes.Count > 3)
                                {
                                    Ignoring.Ignore.Type _t = Ignoring.Ignore.Type.Everything;
                                    if (curr.Attributes[3].Value == "User")
                                    {
                                        _t = Ignoring.Ignore.Type.User;
                                    }
                                    Ignoring.Ignore ignore = new Ignoring.Ignore(bool.Parse(curr.Attributes[0].Value), bool.Parse(curr.Attributes[1].Value), curr.Attributes[2].Value, _t);
                                    lock (Ignoring.IgnoreList)
                                    {
                                        Ignoring.IgnoreList.Add(ignore);
                                    }
                                }
                                continue;
                            }
                            if (curr.Name == "list")
                            {
                                if (curr.Attributes.Count > 2)
                                {
                                    Network.Highlighter list = new Network.Highlighter();
                                    list.simple = bool.Parse(curr.Attributes[0].Value);
                                    list.text = curr.Attributes[1].Value;
                                    list.enabled = bool.Parse(curr.Attributes[2].Value);
                                    lock (Configuration.HighlighterList)
                                    {
                                        Configuration.HighlighterList.Add(list);
                                    }
                                }
                                continue;
                            }

                            if (curr.Name == "protocol")
                            {
                                if (!Protocols)
                                {
                                    Configuration.Parser.Protocols.Clear();
                                    Protocols = true;
                                }
                                Configuration.Parser.Protocols.Add(curr.InnerText);
                                continue;
                            }

                            if (curr.Name == "shortcut")
                            {
                                if (curr.Attributes.Count > 2)
                                {
                                    Shortcut list = new Shortcut(parseKey(curr.Attributes[0].Value), bool.Parse(curr.Attributes[1].Value), bool.Parse(curr.Attributes[2].Value), bool.Parse(curr.Attributes[3].Value));
                                    list.data = curr.InnerText;
                                    Configuration.ShortcutKeylist.Add(list);
                                }
                                continue;
                            }
                            if (curr.Attributes[0].Name == "confname")
                            {
                                switch (curr.Attributes[0].Value)
                                {
                                    case "location.x1":
                                        Configuration.Window.x1 = int.Parse(curr.InnerText);
                                        break;
                                    case "window.size":
                                        Configuration.Window.window_size = int.Parse(curr.InnerText);
                                        break;
                                    case "location.x4":
                                        Configuration.Window.x4 = int.Parse(curr.InnerText);
                                        break;
                                    case "location.maxi":
                                        Configuration.Window.Window_Maximized = bool.Parse(curr.InnerText);
                                        break;
                                    case "timestamp.display":
                                        Configuration.Scrollback.chat_timestamp = bool.Parse(curr.InnerText);
                                        break;
                                    case "network.nick":
                                        Configuration.UserData.nick = curr.InnerText;
                                        break;
                                    case "network.n2":
                                        Configuration.UserData.Nick2 = curr.InnerText;
                                        break;
                                    case "network.ident":
                                        Configuration.UserData.ident = curr.InnerText;
                                        break;
                                    case "scrollback.showctcp":
                                        Configuration.irc.DisplayCtcp = bool.Parse(curr.InnerText);
                                        break;
                                    case "formats.user":
                                        Configuration.Scrollback.format_nick = curr.InnerText;
                                        break;
                                    case "formats.date":
                                        Configuration.Scrollback.format_date = curr.InnerText;
                                        break;
                                    case "formats.datetime":
                                        Configuration.Scrollback.timestamp_mask = curr.InnerText;
                                        break;
                                    case "irc.auto.whois":
                                        Configuration.ChannelModes.aggressive_whois = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.bans":
                                        Configuration.ChannelModes.aggressive_bans = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.exception":
                                        Configuration.ChannelModes.aggressive_exception = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.channels":
                                        Configuration.ChannelModes.aggressive_channel = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.invites":
                                        Configuration.ChannelModes.aggressive_invites = bool.Parse(curr.InnerText);
                                        break;
                                    case "irc.auto.mode":
                                        Configuration.ChannelModes.aggressive_mode = bool.Parse(curr.InnerText);
                                        break;
                                    case "network.reason":
                                        Configuration.irc.DefaultReason = curr.InnerText;
                                        break;
                                    case "logs.type":
                                        Configuration.Logs.logs_name = curr.InnerText;
                                        break;
                                    case "ignore.ctcp":
                                        Configuration.irc.DisplayCtcp = bool.Parse(curr.InnerText);
                                        break;
                                    case "logs.html":
                                        Configuration.Logs.logs_html = bool.Parse(curr.InnerText);
                                        break;
                                    case "logs.dir":
                                        Configuration.Logs.logs_dir = curr.InnerText;
                                        break;
                                    case "logs.xml":
                                        Configuration.Logs.logs_xml = bool.Parse(curr.InnerText);
                                        break;
                                    case "logs.txt":
                                        Configuration.Logs.logs_txt = bool.Parse(curr.InnerText);
                                        break;
                                    case "scrollback_plimit":
                                        Configuration.Scrollback.scrollback_plimit = int.Parse(curr.InnerText);
                                        break;
                                    case "notification.tray":
                                        Configuration.Kernel.Notice = bool.Parse(curr.InnerText);
                                        break;
                                    case "pidgeon.size":
                                        Configuration.Services.Depth = int.Parse(curr.InnerText);
                                        break;
                                    case "history.nick":
                                        Configuration.UserData.LastNick = curr.InnerText;
                                        break;
                                    case "sniffer":
                                        Configuration.Kernel.NetworkSniff = bool.Parse(curr.InnerText);
                                        break;
                                    case "history.host":
                                        Configuration.UserData.LastHost = curr.InnerText;
                                        break;
                                    case "updater.check":
                                        Configuration.Kernel.CheckUpdate = bool.Parse(curr.InnerText);
                                        break;
                                    case "history.port":
                                        Configuration.LastPort = curr.InnerText;
                                        break;
                                    case "delimiters":
                                        List<char> temp = new List<char>();
                                        foreach (char part in curr.InnerText)
                                        {
                                            temp.Add(part);
                                        }
                                        Configuration.Parser.Separators = temp;
                                        break;
                                    case "colors.changelinks":
                                        Configuration.Colors.ChangeLinks = bool.Parse(curr.InnerText);
                                        break;
                                    case "debugger":
                                        Configuration.Kernel.Debugging = bool.Parse(curr.InnerText);
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    handleException(f);
                    return false;
                }
            }
            return true;
        }

        public static bool connectXmpp(string server, int port, string password, bool secured = false)
        {
            ProtocolXmpp IM = new ProtocolXmpp();
            IM.Open();
            return false;
        }

        public static bool RegisterPlugin(TclInterpreter plugin)
        {
            return false;
        }

        public static bool RegisterPlugin(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    System.Reflection.Assembly library = System.Reflection.Assembly.LoadFrom(path);
                    if (library == null)
                    {
                        Core.DebugLog("Unable to load " + path + " because the file can't be read");
                        return false;
                    }
                    Type[] types = library.GetTypes();
                    Type type = library.GetType("Client.RestrictedModule");
                    Type pluginInfo = null;
                    foreach (Type curr in types)
                    {
                        if (curr.IsAssignableFrom(type))
                        {
                            pluginInfo = curr;
                            break;
                        }
                    }
                    if (pluginInfo == null)
                    {
                        Core.DebugLog("Unable to load " + path + " because the library contains no module");
                        return false;
                    }
                    Extension _plugin = (Extension)Activator.CreateInstance(pluginInfo);
                    lock (Extensions)
                    {
                        if (Extensions.Contains(_plugin))
                        {
                            Core.DebugLog("Unable to load extension because the handle is already known to core");
                            return false;
                        }
                        bool problem = false;
                        foreach (Extension x in Core.Extensions)
                        {
                            if (x.Name == _plugin.Name)
                            {
                                Core.Ringlog("CORE: unable to load the extension, because the extension with same name is already loaded");
                                _plugin._Status = Extension.Status.Terminated;
                                problem = true;
                                break;
                            }
                        }
                        if (problem)
                        {
                            if (Core.Extensions.Contains(_plugin))
                            {
                                Core.Extensions.Remove(_plugin);
                            }
                        }
                        Core.Ringlog("CORE: everything is fine, registering " + _plugin.Name);
                        Extensions.Add(_plugin);
                    }
                    if (_plugin.Hook_OnRegister())
                    {
                        _plugin.Load();
                        _plugin._Status = Extension.Status.Active;
                        Core.Ringlog("CORE: finished loading of module " + _plugin.Name);
                        if (Core._Main != null)
                        {
                            Core._Main.main.scrollback.InsertText("Loaded plugin " + _plugin.Name + " (v. " + _plugin.Version + ")", Scrollback.MessageStyle.System, false);
                        }
                        return true;
                    }
                    else
                    {
                        Core.Ringlog("CORE: failed to run OnRegister for " + _plugin.Name);
                    }
                    return false;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return false;
        }

        public static bool RegisterPlugin(Extension plugin)
        {
            if (plugin._Status == Extension.Status.Loading)
            {
                lock (Extensions)
                {
                    if (Extensions.Contains(plugin))
                    {
                        return false;
                    }
                    Extensions.Add(plugin);
                }
                if (plugin.Hook_OnRegister())
                {
                    plugin.Load();
                    if (Core._Main != null)
                    {
                        Core._Main.main.scrollback.InsertText("Loaded plugin " + plugin.Name + " (v. " + plugin.Version + ")", Scrollback.MessageStyle.System, false);
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        public static bool connectQl(string server, int port, string password = "xx", bool secured = false)
        {
            ProtocolQuassel _quassel = new ProtocolQuassel();
            _quassel.Port = port;
            _quassel.password = password;
            _quassel.Server = server;
            _quassel.Open();
            Connections.Add(_quassel);
            return false;
        }

        /// <summary>
        /// Connect to pidgeon server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool connectPS(string server, int port = 8222, string password = "xx", bool secured = false)
        {
            ProtocolSv protocol = new ProtocolSv();
            protocol.Server = server;
            protocol.nick = Configuration.UserData.nick;
            protocol.Port = port;
            protocol.password = password;
            Connections.Add(protocol);
            protocol.Open();
            return true;
        }


        /// <summary>
        /// Connect to IRC
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static ProtocolIrc connectIRC(string server, int port = 6667, string pw = "", bool secured = false)
        {
            ProtocolIrc protocol = new ProtocolIrc();
            Connections.Add(protocol);
            protocol.Server = server;
            protocol.Port = port;
            protocol.Password = pw;
            protocol._IRCNetwork = new Network(server, protocol);
            network = protocol._IRCNetwork;
            protocol._IRCNetwork._Protocol = protocol;
            protocol.Open();
            return protocol;
        }

        public static int handleException(Exception _exception, bool fatal = false)
        {
            if (IgnoreErrors)
            {
                Console.WriteLine("EXCEPTION: " + _exception.StackTrace);
                return -2;
            }
            if (fatal)
            {
                recovery_fatal = true;
            }
            DebugLog(_exception.Message + " at " + _exception.Source + " info: " + _exception.Data.ToString());
            blocked = true;
            recovery_exception = _exception;
            _RecoveryThread = new Thread(Recover);
            _RecoveryThread.Start();
            if (Thread.CurrentThread != _KernelThread)
            {
                DebugLog("Warning, the thread which raised the exception is not a core thread, identifier: " + Thread.CurrentThread.Name);
            }
            while (blocked || fatal)
            {
                Thread.Sleep(100);
            }
            return 0;
        }

        public static bool Quit()
        {
            try
            {
                if (!IgnoreErrors)
                {
                    IgnoreErrors = true;
                    _Main.Visible = false;
                    ConfigSave();
                    try
                    {
                        foreach (Protocol server in Connections)
                        {
                            server.Exit();
                        }
                    }
                    catch (Exception fail)
                    {
                        Core.handleException(fail);
                    }
                    foreach (Thread th in SystemThreads)
                    {
                        try
                        {
                            if (th.ThreadState != System.Threading.ThreadState.WaitSleepJoin && th.ThreadState != System.Threading.ThreadState.Running)
                            {
                                continue;
                            }
                            th.Abort();
                        }
                        catch (Exception fail)
                        {
                            Core.handleException(fail);
                        }
                    }
                    Thread.Sleep(800);
                    Environment.Exit(0);
                }
            }
            catch (Exception fail)
            {
                Console.WriteLine(fail.StackTrace.ToString());
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            return true;
        }
    }
}
