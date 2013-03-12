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
            public List<Buffer.Window> windows = new List<Window>();

            public NetworkInfo()
            { 
                
            }

            public NetworkInfo(string nick)
            {
                Nick = nick;
            }

            public int LastMQID = 0;

            public void MQID(string text)
            {
                int mqid = int.Parse(text);
                if (LastMQID < mqid)
                {
                    LastMQID = mqid;
                }
            }
        }

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

        public void ReadDisk()
        {
            if (!Directory.Exists(Root))
            {
                Core.DebugLog("There is no folder for buffer of " + protocol.Server);
                Loaded = false;
                return;
            }
            if (!File.Exists(Root + "data"))
            {
                Core.DebugLog("There is no folder for buffer of " + protocol.Server);
                Loaded = false;
                return;
            }
            Data = new List<string>();
            lock (Data)
            {
                Data.AddRange(File.ReadAllLines(Root + "data"));
            }
        }

        public static string SerializeLine(List<Scrollback.ContentLine> line)
        {
            XmlSerializer xs = new XmlSerializer(line.GetType());
            StringWriter data = new StringWriter();
            xs.Serialize(data, line);
            return data.ToString();
        }

        public static List<Scrollback.ContentLine> DeserializeLine(string text)
        {
            XmlSerializer xs = new XmlSerializer(typeof(List<Scrollback.ContentLine>));
            StringReader data = new StringReader(text);
            List<Scrollback.ContentLine> line = (List<Scrollback.ContentLine>)xs.Deserialize(data);
            return line;
        }

        public static string SerializeNetwork(NetworkInfo line)
        {
            XmlSerializer xs = new XmlSerializer(line.GetType());
            StringWriter data = new StringWriter();
            xs.Serialize(data, line);
            return data.ToString();
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

        public static NetworkInfo DeserializeNetwork(string text)
        {
            XmlSerializer xs = new XmlSerializer(typeof(NetworkInfo));
            StringReader data = new StringReader(text);
            NetworkInfo info = (NetworkInfo)xs.Deserialize(data);
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
                    info.NetworkID = protocol.buffer.getUID(network.server);
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
                        files.Add(Root + network.Key);
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
