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
            public List<Scrollback.ContentLine> lines = null;
            public string Name = null;
            public string topic = null;
            public bool Changed = true;

            public Window()
            { 
                
            }

            public Window(Client.Window owner)
            {
                Name = owner.name;
                lines = owner.scrollback.Data;
            }
        }

        [Serializable]
        public class NetworkInfo
        {
            public string mode = "+i";
            public string Nick = null;
            public string NetworkID = null;
            public string Server = null;
            private int lastMQID = 0;
            public int LastMQID
            {
                get
                {
                    return lastMQID;
                }
            }
            public List<Buffer.Window> windows = new List<Window>();

            public NetworkInfo()
            { 
                
            }

            public Buffer.Window getW(string window)
            {
                lock (windows)
                {
                    foreach (Buffer.Window xx in windows)
                    {
                        if (xx.Name == window)
                        {
                            return xx;
                        }
                    }
                }
                return null;
            }

            public void recoverWindowText(Client.Window target, string source)
            {
                Buffer.Window Source = getW(source);
                if (Source == null)
                {
                    Core.DebugLog("Failed to recover " + source);
                    return;
                }
                target.scrollback.SetText(Source.lines);
            }

            public NetworkInfo(string nick)
            {
                Nick = nick;
            }

            public void MQID(string text)
            {
                int mqid = int.Parse(text);
                if (lastMQID < mqid)
                {
                    lastMQID = mqid;
                }
            }
        }

        public Dictionary<string, string> Networks = new Dictionary<string, string>();
        public Dictionary<string, NetworkInfo> networkInfo = new Dictionary<string,NetworkInfo>();
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

        public void Make(string network, string network_id)
        {
            if (!Networks.ContainsKey(network))
            {
                Networks.Add(network, network_id);
                networkInfo.Add(network_id, new NetworkInfo());
            }
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
            lock (networkInfo)
            {
                networkInfo.Clear();
            }
            Modified = true;
            Directory.Delete(Root, true);
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
            networkInfo.Clear();
            lock (protocol.NetworkList)
            {
                foreach (Network network in protocol.NetworkList)
                {
                    NetworkInfo info = new NetworkInfo(network.Nickname);
                    info.NetworkID = protocol.sBuffer.getUID(network.server);
                    info.Server = network.server;
                    info.windows.Add(new Buffer.Window(network.system));
                    lock (network.Channels)
                    {
                        foreach (Channel xx in network.Channels)
                        {
                            Client.Window window = xx.retrieveWindow();
                            if (window != null)
                            {
                                info.windows.Add(new Buffer.Window(window));
                            }
                        }

                        //foreach (User user in network.PrivateChat)
                        //{ 
                        //    network
                        //}
                    }
                    networkInfo.Add(info.NetworkID, info);
                }
            }
            WriteDisk();
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

        public void WriteDisk()
        {
            try
            {
                if (!Directory.Exists(Root))
                {
                    Directory.CreateDirectory(Root);
                }
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
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}
