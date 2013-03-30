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
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;

namespace Client.Services
{
    public class Buffer
    {
        [Serializable]
        public class Window
        {
            public bool isChannel = false;
            public bool isPM = false;
            public List<string> history = new List<string>();
            public List<Client.ContentLine> lines = null;
            public string Name = null;
            public string topic = null;
            public bool Changed = true;
			public int menucolor = 0;

            public Window()
            {
				
            }

            public Window(Graphics.Window owner)
            {
                Name = owner.name;
                isChannel = owner.isChannel;
                lines = new List<Client.ContentLine>();
                if (owner.textbox != null)
                {
                    history.AddRange(owner.textbox.history);
                }
                lines.AddRange(owner.scrollback.Data);
				menucolor = owner.MenuColor.ToArgb();
            }
        }

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
            public List<Except> Exceptions = null;
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
            public string mode = "";
        }

        [Serializable]
        public class NetworkInfo
        {
            /// <summary>
            /// Mode
            /// </summary>
            public string mode = "+i";
            public string Nick = null;
            public string NetworkID = null;
            public string Server = null;
            public int lastMQID = 0;
            public List<int> MQ = new List<int>();
            /// <summary>
            /// User modes
            /// </summary>
            public List<char> UModes = new List<char> { 'i', 'w', 'o', 'Q', 'r', 'A' };
            /// <summary>
            /// Channel user symbols (oper and such)
            /// </summary>
            public List<char> UChars = new List<char> { '~', '&', '@', '%', '+' };
            /// <summary>
            /// Channel user modes
            /// </summary>
            public List<char> CUModes = new List<char> { 'q', 'a', 'o', 'h', 'v' };
            /// <summary>
            /// Channel modes
            /// </summary>
            public List<char> CModes = new List<char> { 'n', 'r', 't', 'm' };
            /// <summary>
            /// Special channel modes with parameter as a string
            /// </summary>
            public List<char> SModes = new List<char> { 'k', 'L' };
            /// <summary>
            /// Special channel modes with parameter as a number
            /// </summary>
            public List<char> XModes = new List<char> { 'l' };
            /// <summary>
            /// Special channel user modes with parameters as a string
            /// </summary>
            public List<char> PModes = new List<char> { 'b', 'I', 'e' };
            /// <summary>
            /// List of channels
            /// </summary>
            public List<Network.ChannelData> ChannelList = new List<Network.ChannelData>();

            public List<Description> Descriptions = new List<Description>();
            public List<Buffer.Window> _windows = new List<Buffer.Window>();
            public List<Buffer.ChannelInfo> _channels = new List<Buffer.ChannelInfo>();
            public List<string> PrivateWins = new List<string>();

            [Serializable]
            public class Description
            {
                public char Char;
                public string String;

                public Description()
                {

                }

                public Description(char c, string s)
                {
                    Char = c;
                    String = s;
                }
            }

            public class Range
            {
                public int X;
                public int Y;
                public Range(int x, int y)
                {
                    Y = y;
                    X = x;
                }
            }

            public NetworkInfo()
            {

            }

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

            public bool containsMQID(int id)
            {
                lock (MQ)
                {
                    if (MQ.Contains(id))
                    {
                        return true;
                    }
                }
                return false;
            }

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
				System.Drawing.Color test;
                if (Source.menucolor != 0)
				{
					target.MenuColor = System.Drawing.Color.FromArgb(Source.menucolor);
				}
                if (target.textbox != null)
                {
                    target.textbox.history = new List<string>();
                    target.textbox.history.AddRange(Source.history);
                    target.textbox.position = Source.history.Count;
                }
                target.isChannel = Source.isChannel;
            }

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

            public NetworkInfo(string nick)
            {
                Nick = nick;
            }

            public NetworkInfo(Network network)
            {
                Nick = network.Nickname;
            }

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

        public Dictionary<string, string> Networks = new Dictionary<string, string>();
        public Dictionary<string, NetworkInfo> networkInfo = new Dictionary<string, NetworkInfo>();
        public ProtocolSv protocol = null;
        public bool Loaded = false;
        public string Root = null;
        private List<string> Data = null;
        public bool Modified = true;

        public Buffer(ProtocolSv _s)
        {
            Root = Core.PermanentTemp + "buffer_" + _s.Server + Path.DirectorySeparatorChar;
            protocol = _s;
        }

        public string getUID(string server)
        {
            if (Networks.ContainsKey(server))
            {
                return Networks[server];
            }
            return null;
        }

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
                            Core._Main.Status("Reading disk cache for " + network);
                            string content = File.ReadAllText(Root + network);
                            NetworkInfo info = DeserializeNetwork(content);
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

        public void Clear()
        {
            ClearData();
        }

        public void ClearData()
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, true);
            }
        }

        public static string SerializeNetwork(NetworkInfo line)
        {
            XmlSerializer xs = new XmlSerializer(typeof(NetworkInfo));
            StringWriter data = new StringWriter();
            xs.Serialize(data, line);
            return data.ToString();
        }

        public static NetworkInfo DeserializeNetwork(string text)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(text);
            XmlNodeReader reader = new XmlNodeReader(document.DocumentElement);
            XmlSerializer xs = new XmlSerializer(typeof(NetworkInfo));
            NetworkInfo info = (NetworkInfo)xs.Deserialize(reader);
            return info;
        }

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
                                    Graphics.Window window = xx.retrieveWindow();
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

        public void Init(object network)
        {
            try
            {
                NetworkInfo nw = (NetworkInfo)network;
                NetworkInfo.Range range = nw.getRange();
                while (range != null)
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

        public void retrieveData(string network)
        {
            lock (networkInfo)
            {
                if (networkInfo.ContainsKey(Networks[network]))
                {
                    System.Threading.Thread th = new System.Threading.Thread(Init);
                    th.Start(networkInfo[Networks[network]]);
                }
            }
        }

        public static void ListFile(List<string> list, string file)
        {
            StringBuilder data = new StringBuilder("");
            foreach (string line in list)
            {
                data.Append(line + Environment.NewLine);
            }
            File.WriteAllText(file, data.ToString());
        }

        public void PrintInfo()
        {
            if (Core._Main.Chat != null)
            {
                Core._Main.Chat.scrollback.InsertText("Information about cache:", Client.ContentLine.MessageStyle.System, false);
                lock (networkInfo)
                {
                    foreach (KeyValuePair<string, NetworkInfo> xx in networkInfo)
                    {
                        Core._Main.Chat.scrollback.InsertText("Network: " + xx.Value.Server + " MQID: " + xx.Value.lastMQID.ToString(), Client.ContentLine.MessageStyle.System, false);
                    }
                }
            }
        }

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
                        string xx = SerializeNetwork(network.Value);
                        File.WriteAllText(Root + network.Key, xx);
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
