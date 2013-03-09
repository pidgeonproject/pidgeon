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
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public partial class ProtocolSv
    {
        public List<string> ProcessedData = new List<string>();
        public void ProcessBuffer(string network)
        {
            int id = 0;
            string NetworkID = buffer.Networks[network];
            if (buffer.LastMQID[NetworkID] < 1)
            {
                return;
            }

            if (ProcessedData.Contains(NetworkID))
            {
                return;
            }

            ProcessedData.Add(NetworkID);
            int LastMQID =buffer.LastMQID[NetworkID];
            string last = LastMQID.ToString();
            Core._Main.progress = id;
            Core._Main.DisplayingProgress = true;
            SuppressChanges = true;
            Core._Main.ProgressMax = buffer.LastMQID[NetworkID];
            Network server = null;
            server = retrieveNetwork(network);
            if (server == null)
            {
                throw new Exception("ERROR #2: There is no network object " + network);
            }
            lock (buffer.data[NetworkID])
            {
                string blah = null;
                foreach (Datagram line in buffer.data[NetworkID])
                {
                    id++;
                    Core._Main.progress = id;
                    Core._Main.Status("Retrieving local cache from " + network + ", got " + id + " packets from total of " + last + " datagrams");
                    if (id >= LastMQID)
                    {
                        Core._Main.Status("");
                        Core._Main.DisplayingProgress = false;
                        Core._Main.progress = 0;
                        SuppressChanges = false;
                    }
                    long date = 0;
                    if (!long.TryParse(line.Parameters["time"], out date))
                    {
                        Core.DebugLog("Warning: " + line.Parameters["time"] + " is not a correct time");
                    }
                    if (line._InnerText == blah)
                    {
                         Core.DebugLog("");
                    }

                    ProcessorIRC processor = new ProcessorIRC(server, line._InnerText, ref pong, date, false);
                    processor.Result();
                    blah = line._InnerText;
                }
            }
        }
    }
}
