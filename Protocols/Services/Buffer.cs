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
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;

namespace Client.Services
{
    /// <summary>
    /// Stores info and packets to local disk so that they can be retrieved instead of downloading them
    /// </summary>
    public class Buffer
    {
        /// <summary>
        /// Window
        /// </summary>
        [Serializable]
        public class Window
        {
            /// <summary>
            /// Whether it is channel
            /// </summary>
            public bool isChannel = false;
            /// <summary>
            /// Private message
            /// </summary>
            public bool isPM = false;
            /// <summary>
            /// History
            /// </summary>
            public List<string> history = new List<string>();
            /// <summary>
            /// Data
            /// </summary>
            public List<Client.ContentLine> lines = null;
            /// <summary>
            /// Name
            /// </summary>
            public string Name = null;
            /// <summary>
            /// topic
            /// </summary>
            public string topic = null;
            /// <summary>
            /// Changed
            /// </summary>
            public bool Changed = true;
            /// <summary>
            /// Color
            /// </summary>
            public int menucolor = 0;
            /// <summary>
            /// Sort
            /// </summary>
            public bool SortNeeded = false;

            /// <summary>
            /// Creates a new instance of window
            /// </summary>
            public Window()
            {

            }

            /// <summary>
            /// Creates a new instance of window
            /// </summary>
            /// <param name="owner"></param>
            public Window(Graphics.Window owner)
            {
                Name = owner.WindowName;
                isChannel = owner.isChannel;
                isPM = owner.isPM;
                lines = new List<Client.ContentLine>();
                if (owner.textbox != null)
                {
                    history.AddRange(owner.textbox.history);
                }
                lines.AddRange(owner.scrollback.Data);
                menucolor = owner.MenuColor.ToArgb();
                SortNeeded = owner.scrollback.SortNeeded;
            }
        }

        /// <summary>
        /// Information data
        /// </summary>
        [Serializable]
        public class ChannelInfo
        {
            /// <summary>
            /// Name of a channel including the special prefix
            /// </summary>
            public string Name;
            /// <summary>
            /// Topic
            /// </summary>
            public string Topic = null;
            /// <summary>
            /// Whether channel is in proccess of dispose
            /// </summary>
            public bool dispose = false;
            /// <summary>
            /// User who set a topic
            /// </summary>
            public string TopicUser = "<Unknown user>";
            /// <summary>
            /// Date when a topic was set
            /// </summary>
            public int TopicDate = 0;
            /// <summary>
            /// Invites
            /// </summary>
            public List<Invite> Invites = null;
            /// <summary>
            /// List of bans set
            /// </summary>
            public List<SimpleBan> Bans = null;
            /// <summary>
            /// Exception list 
            /// </summary>
            public List<ChannelBanException> Exceptions = null;
            /// <summary>
            /// If channel output is temporarily hidden
            /// </summary>
            public bool temporary_hide = false;
            /// <summary>
            /// If true the channel is processing the /who data
            /// </summary>
            public bool parsing_who = false;
            /// <summary>
            /// If true the channel is processing ban data
            /// </summary>
            public bool parsing_bans = false;
            /// <summary>
            /// If true the channel is processing exception data
            /// </summary>
            public bool parsing_xe = false;
            /// <summary>
            /// If true the channel is processing whois data
            /// </summary>
            public bool parsing_wh = false;
            /// <summary>
            /// Whether window needs to be redraw
            /// </summary>
            public bool Redraw = false;
            /// <summary>
            /// If true the window is considered usable
            /// </summary>
            public bool ChannelWork = false;
            /// <summary>
            /// Current mode
            /// </summary>
            public string mode = "";
        }

        /// <summary>
        /// Information about a network
        /// </summary>
        [Serializable]
        public class NetworkInfo
        {
            /// <summary>
            /// Mode
            /// </summary>
            public string mode = "+i";
            /// <summary>
            /// Current nick
            /// </summary>
            public string Nick = null;
            /// <summary>
            /// Network MQID
            /// </summary>
            public string NetworkID = null;
            /// <summary>
            /// Server
            /// </summary>
            public string Server = null;
            /// <summary>
            /// Last mqid
            /// </summary>
            public int lastMQID = 0;
            /// <summary>
            /// List of parsed data
            /// </summary>
            public List<int> MQ = new List<int>();
            /// <summary>
            /// User modes
            /// </summary>
            public List<char> UModes = new List<char>();
            /// <summary>
            /// Channel user symbols (oper and such)
            /// </summary>
            public List<char> UChars = new List<char>();
            /// <summary>
            /// Channel user modes
            /// </summary>
            public List<char> CUModes = new List<char>();
            /// <summary>
            /// Channel modes
            /// </summary>
            public List<char> CModes = new List<char>();
            /// <summary>
            /// Special channel modes with parameter as a string
            /// </summary>
            public List<char> SModes = new List<char>();
            /// <summary>
            /// Special channel modes with parameter as a number
            /// </summary>
            public List<char> XModes = new List<char>();
            /// <summary>
            /// Special channel user modes with parameters as a string
            /// </summary>
            public List<char> PModes = new List<char>();
            /// <summary>
            /// List of channels
            /// </summary>
            public List<Network.ChannelData> ChannelList = new List<Network.ChannelData>();
            /// <summary>
            /// Info
            /// </summary>
            public List<Description> Descriptions = new List<Description>();
            /// <summary>
            /// IRCD
            /// </summary>
            public string Version = null;
            /// <summary>
            /// List of windows in system
            /// </summary>
            public List<Buffer.Window> _windows = new List<Buffer.Window>();
            /// <summary>
            /// List of channels in system
            /// </summary>
            public List<Buffer.ChannelInfo> _channels = new List<Buffer.ChannelInfo>();
            /// <summary>
            /// Private wins
            /// </summary>
            public List<string> PrivateWins = new List<string>();

            /// <summary>
            /// Description
            /// </summary>
            [Serializable]
            public class Description
            {
                /// <summary>
                /// Char
                /// </summary>
                public char Char;
                /// <summary>
                /// Text
                /// </summary>
                public string String = null;

                /// <summary>
                /// Creates a new instance for xml
                /// </summary>
                public Description()
                {

                }

                /// <summary>
                /// Creates a new instance with data
                /// </summary>
                /// <param name="c"></param>
                /// <param name="s"></param>
                public Description(char c, string s)
                {
                    Char = c;
                    String = s;
                }
            }

            /// <summary>
            /// Range
            /// </summary>
            public class Range
            {
                /// <summary>
                /// First
                /// </summary>
                public int X;
                /// <summary>
                /// Last
                /// </summary>
                public int Y;

                /// <summary>
                /// Creates a new instance of range
                /// </summary>
                /// <param name="x">X</param>
                /// <param name="y">Y</param>
                public Range(int x, int y)
                {
                    Y = y;
                    X = x;
                }
            }

            /// <summary>
            /// Creates a new instance (for xml serialization only)
            /// </summary>
            public NetworkInfo()
            {

            }

            /// <summary>
            /// retrieve a window info
            /// </summary>
            /// <param name="window"></param>
            /// <returns></returns>
            public Buffer.Window getW(string window)
            {
                lock (_windows)
                {
                    foreach (Buffer.Window xx in _windows)
                    {
                        if (xx.Name == window)
                        {
                            return xx;
                        }
                    }
                }
                return null;
            }

            /// <summary>
            /// Whether this mqid was parsed
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public bool containsMQID(int id)
            {
                return MQ.Contains(id);
            }

            /// <summary>
            /// Retrieve a range
            /// </summary>
            /// <param name="startindex"></param>
            /// <returns></returns>
            public Range getRange(int startindex = 0)
            {
                Range range = null;

                int index = startindex;

                lock (MQ)
                {
                    while (index < lastMQID && containsMQID(index))
                    {
                        index++;
                    }

                    if (index >= lastMQID)
                    {
                        return null;
                    }

                    int Y = index;

                    while (Y < lastMQID && !containsMQID(index))
                    {
                        Y++;
                    }

                    range = new Range(index, Y);
                }

                return range;
            }

            /// <summary>
            /// Recover a window
            /// </summary>
            /// <param name="target"></param>
            /// <param name="source"></param>
            public void recoverWindowText(Graphics.Window target, string source)
            {
                Buffer.Window Source = getW(source);
                if (Source == null)
                {
                    Core.DebugLog("Failed to recover " + source);
                    return;
                }
                if (Source.lines == null)
                {
                    throw new Exception("This window doesn't contain any lines");
                }
                target.scrollback.SetText(Source.lines);
                Source.lines.Clear();
                Source.lines = null;
                if (Source.menucolor != 0)
                {
                    target.MenuColor = System.Drawing.Color.FromArgb(Source.menucolor);
                }
                if (target.textbox != null)
                {
                    if (target.textbox.history == null)
                    {
                        target.textbox.history = new List<string>();
                    }
                    target.textbox.history.Clear();
                    target.textbox.history.AddRange(Source.history);
                    target.textbox.position = Source.history.Count;
                }
                Source.history.Clear();
                target.isPM = Source.isPM;
                target.isChannel = Source.isChannel;
                // once we recover a window we don't longer need it, so remove it from memory
                if (_windows.Contains(Source))
                {
                    _windows.Remove(Source);
                }
                target.scrollback.SortNeeded = Source.SortNeeded;
            }

            /// <summary>
            /// Channel from string
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public ChannelInfo getChannel(string name)
            {
                lock (_channels)
                {
                    foreach (ChannelInfo ch in _channels)
                    {
                        if (ch.Name == name)
                        {
                            return ch;
                        }
                    }
                }
                return null;
            }

            /// <summary>
            /// Creates a new instance
            /// </summary>
            /// <param name="nick"></param>
            public NetworkInfo(string nick)
            {
                Nick = nick;
            }

            /// <summary>
            /// Creates a new instance
            /// </summary>
            /// <param name="network"></param>
            public NetworkInfo(Network network)
            {
                Nick = network.Nickname;
            }

            /// <summary>
            /// Stores an ID in a list
            /// </summary>
            /// <param name="text"></param>
            public void MQID(string text)
            {
                int mqid = int.Parse(text);
                lock (MQ)
                {
                    if (!containsMQID(mqid))
                    {
                        MQ.Add(mqid);
                    }
                }
                if (lastMQID < mqid)
                {
                    lastMQID = mqid;
                }
            }
        }

        /// <summary>
        /// Network / mqid
        /// </summary>
        public Dictionary<string, string> Networks = new Dictionary<string, string>();
        /// <summary>
        /// Network data
        /// </summary>
        public Dictionary<string, NetworkInfo> networkInfo = new Dictionary<string, NetworkInfo>();
        /// <summary>
        /// Protocol this buffer is connected to
        /// </summary>
        public ProtocolSv protocol = null;
        /// <summary>
        /// Whether the buffer is loaded
        /// </summary>
        public bool Loaded = false;
        /// <summary>
        /// Folder
        /// </summary>
        public string Root = null;
        /// <summary>
        /// Index
        /// </summary>
        private List<string> Data = null;
        /// <summary>
        /// Whether the buffer was modified
        /// </summary>
        public bool Modified = true;
        private bool destroyed = false;
        /// <summary>
        /// This will return true in case object was requested to be disposed
        /// you should never work with objects that return true here
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return destroyed;
            }
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="_s"></param>
        public Buffer(ProtocolSv _s)
        {
            Root = Core.PermanentTemp + "buffer_" + _s.Server + Path.DirectorySeparatorChar;
            protocol = _s;
        }

        /// <summary>
        /// Get uid
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public string getUID(string server)
        {
            if (Networks.ContainsKey(server))
            {
                return Networks[server];
            }
            return null;
        }

        /// <summary>
        /// Make a buffer for new network
        /// </summary>
        /// <param name="network"></param>
        /// <param name="network_id"></param>
        /// <param name="_network"></param>
        public void Make(string network, string network_id, Network _network = null)
        {
            if (Networks.ContainsKey(network))
            {
                networkInfo.Remove(Networks[network]);
                Networks.Remove(network);
            }
            Networks.Add(network, network_id);
            if (_network == null)
            {
                networkInfo.Add(network_id, new NetworkInfo());
                return;
            }
            networkInfo.Add(network_id, new NetworkInfo(_network));
        }

        /// <summary>
        /// read disk
        /// </summary>
        public void ReadDisk()
        {
            try
            {
                lock (networkInfo)
                {
                    networkInfo.Clear();
                    if (!Directory.Exists(Root))
                    {
                        Core.DebugLog("There is no folder for buffer of " + protocol.Server);
                        Loaded = false;
                        return;
                    }
                    if (!File.Exists(Root + "data"))
                    {
                        Core.DebugLog("There is no main for buffer of " + protocol.Server);
                        Loaded = false;
                        return;
                    }
                    Data = new List<string>();
                    lock (Data)
                    {
                        Data.AddRange(File.ReadAllLines(Root + "data"));
                    }
                    foreach (string network in Data)
                    {
                        if (network != "")
                        {
                            Core.SystemForm.Status("Reading disk cache for " + network);
                            NetworkInfo info = DeserializeNetwork(Root + network);
                            networkInfo.Add(network, info);
                            Networks.Add(info.Server, network);
                        }
                    }
                }
                Modified = false;
                Loaded = true;
            }
            catch (Exception fail)
            {
                Core.DebugLog("Failed to load buffer, invalidating it " + fail.ToString());
                Loaded = false;
                Modified = true;
                networkInfo.Clear();
            }
        }

        /// <summary>
        /// Remove all data
        /// </summary>
        public void Clear()
        {
            ClearData();
        }

        private void ClearData()
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, true);
            }
        }

        private static void SerializeNetwork(NetworkInfo line, string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(NetworkInfo));
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            StreamWriter writer = File.AppendText(file);
            xs.Serialize(writer, line);
            writer.Close();
        }

        private static NetworkInfo DeserializeNetwork(string file)
        {
            XmlDocument document = new XmlDocument();
            TextReader sr = new StreamReader(file);
            document.Load(sr);
            XmlNodeReader reader = new XmlNodeReader(document.DocumentElement);
            XmlSerializer xs = new XmlSerializer(typeof(NetworkInfo));
            NetworkInfo info = (NetworkInfo)xs.Deserialize(reader);
            reader.Close();
            sr.Close();
            return info;
        }

        /// <summary>
        /// Write all data to disk
        /// </summary>
        public void Snapshot()
        {
            lock (protocol.NetworkList)
            {
                List<Network> unknown = new List<Network>();
                foreach (Network network in protocol.NetworkList)
                {
                    string uid = protocol.sBuffer.getUID(network.ServerName);
                    if (uid == null)
                    {
                        unknown.Add(network);
                        continue;
                    }
                    lock (networkInfo)
                    {
                        if (networkInfo.ContainsKey(uid))
                        {
                            networkInfo[uid]._windows.Clear();
                            networkInfo[uid]._channels.Clear();
                            networkInfo[uid].PrivateWins.Clear();
                            networkInfo[uid]._windows.Add(new Buffer.Window(network.SystemWindow));
                            networkInfo[uid].CModes = network.CModes;
                            networkInfo[uid].CUModes = network.CUModes;
                            networkInfo[uid].PModes = network.PModes;
                            networkInfo[uid].mode = network.usermode.ToString();
                            networkInfo[uid].Version = network.IrcdVersion;
                            networkInfo[uid].SModes = network.SModes;
                            networkInfo[uid].ChannelList = new List<Network.ChannelData>();
                            lock (network.ChannelList)
                            {
                                networkInfo[uid].ChannelList.AddRange(network.ChannelList);
                            }
                            lock (network.Descriptions)
                            {
                                networkInfo[uid].Descriptions.Clear();
                                foreach (KeyValuePair<char, string> description in network.Descriptions)
                                {
                                    networkInfo[uid].Descriptions.Add(new NetworkInfo.Description(description.Key, description.Value));
                                }
                            }
                            networkInfo[uid].XModes = network.XModes;
                            networkInfo[uid].UChars = network.UChars;
                            lock (network.Channels)
                            {
                                foreach (Channel xx in network.Channels)
                                {
                                    Graphics.Window window = xx.RetrieveWindow();
                                    if (window != null)
                                    {
                                        networkInfo[uid]._windows.Add(new Buffer.Window(window));
                                    }
                                    ChannelInfo info = new ChannelInfo();
                                    info.Bans = xx.Bans;
                                    info.dispose = xx.dispose;
                                    info.Exceptions = xx.Exceptions;
                                    info.Invites = xx.Invites;
                                    info.Name = xx.Name;
                                    info.parsing_bans = xx.parsing_bans;
                                    info.parsing_wh = xx.parsing_wh;
                                    info.parsing_who = xx.parsing_who;
                                    info.parsing_xe = xx.parsing_xe;
                                    info.Redraw = xx.Redraw;
                                    info.mode = xx.ChannelMode.ToString();
                                    info.temporary_hide = xx.temporary_hide;
                                    info.Topic = xx.Topic;
                                    info.TopicDate = xx.TopicDate;
                                    info.ChannelWork = xx.ChannelWork;
                                    info.TopicUser = xx.TopicUser;
                                    networkInfo[uid]._channels.Add(info);
                                }

                                foreach (KeyValuePair<User, Graphics.Window> wn in network.PrivateWins)
                                {
                                    Buffer.Window window = new Buffer.Window(wn.Value);
                                    if (!networkInfo[uid].PrivateWins.Contains(window.Name))
                                    {
                                        networkInfo[uid].PrivateWins.Add(window.Name);
                                        networkInfo[uid]._windows.Add(window);
                                    }
                                    else
                                    {
                                        Core.DebugLog("ERROR: Multiple same private windows detected of " + window.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            WriteDisk();
        }

        private void Init(object network)
        {
            try
            {
                // we wait here for a while so that we don't colide with post processing functions
                System.Threading.Thread.Sleep(20000);
                if (IsDestroyed)
                {
                    return;
                }
                NetworkInfo nw = (NetworkInfo)network;
                nw.MQ.Sort();
                NetworkInfo.Range range = nw.getRange();
                while (range != null && !IsDestroyed)
                {
                    ProtocolSv.Datagram request = new ProtocolSv.Datagram("BACKLOGRANGE");
                    request.Parameters.Add("network", nw.Server);
                    request.Parameters.Add("from", range.X.ToString());
                    request.Parameters.Add("to", range.Y.ToString());
                    protocol.Deliver(request);
                    range = nw.getRange(range.Y + 1);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// Destroy
        /// </summary>
        public void Destroy()
        {
            destroyed = true;
            foreach (NetworkInfo nw in networkInfo.Values)
            {
                nw._windows.Clear();
                nw._channels.Clear();
                nw.ChannelList.Clear();
                nw.MQ.Clear();
            }
            Networks.Clear();
            networkInfo.Clear();
        }

        /// <summary>
        /// Retrieve a data of network
        /// </summary>
        /// <param name="network"></param>
        public void retrieveData(string network)
        {
            lock (networkInfo)
            {
                if (networkInfo.ContainsKey(Networks[network]))
                {
                    System.Threading.Thread th = new System.Threading.Thread(Init);
                    th.Name = "Services buffer";
                    lock (Core.SystemThreads)
                    {
                        Core.SystemThreads.Add(th);
                    }
                    th.Start(networkInfo[Networks[network]]);
                }
            }
        }

        /// <summary>
        /// Write list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="file"></param>
        public static void ListFile(List<string> list, string file)
        {
            StringBuilder data = new StringBuilder("");
            foreach (string line in list)
            {
                data.Append(line + Environment.NewLine);
            }
            File.WriteAllText(file, data.ToString());
        }

        /// <summary>
        /// Display information to current scrollback
        /// </summary>
        public void PrintInfo()
        {
            if (Core.SystemForm.Chat != null)
            {
                Core.SystemForm.Chat.scrollback.InsertText("Information about cache:", Client.ContentLine.MessageStyle.System, false);
                lock (networkInfo)
                {
                    foreach (KeyValuePair<string, NetworkInfo> xx in networkInfo)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText("Network: " + xx.Value.Server + " MQID: " + xx.Value.lastMQID.ToString(), Client.ContentLine.MessageStyle.System, false);
                    }
                }
            }
        }

        /// <summary>
        /// Write all data to disk
        /// </summary>
        public void WriteDisk()
        {
            try
            {
                if (!Directory.Exists(Root))
                {
                    Directory.CreateDirectory(Root);
                }
                Core.DebugLog("Saving cache for " + protocol.Server);
                List<string> files = new List<string>();
                lock (networkInfo)
                {
                    foreach (KeyValuePair<string, NetworkInfo> network in networkInfo)
                    {
                        SerializeNetwork(network.Value, Root + network.Key);
                        files.Add(network.Key);
                    }
                }

                ListFile(files, Root + "data");
                Modified = false;
                Core.DebugLog("Done");
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
