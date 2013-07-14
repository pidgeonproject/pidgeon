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
using System.Drawing;
using Gtk;

namespace Client.Graphics
{
    public partial class PidgeonList : Gtk.Bin
    {
        /// <summary>
        /// Represent a request to open a new DCC connection
        /// </summary>
        public class RequestDCC
        {
            /// <summary>
            /// User
            /// </summary>
            public string User;
            /// <summary>
            /// Server
            /// </summary>
            public string Server;
            /// <summary>
            /// Port
            /// </summary>
            public int Port;
            /// <summary>
            /// Listener
            /// </summary>
            public bool Listener;
            /// <summary>
            /// SSL
            /// </summary>
            public bool SSL;
            /// <summary>
            /// Network
            /// </summary>
            public Network network;

            /// <summary>
            /// Creates a new instance of request
            /// </summary>
            /// <param name="user"></param>
            /// <param name="server"></param>
            /// <param name="port"></param>
            /// <param name="Ssl"></param>
            /// <param name="listener"></param>
            /// <param name="n"></param>
            public RequestDCC(string user, string server, int port, bool Ssl, bool listener, Network n)
            {
                User = user;
                Server = server;
                network = n;
                Port = port;
                Listener = listener;
                SSL = Ssl;
            }
        }

        /// <summary>
        /// List of waiting DCC
        /// </summary>
        public static List<RequestDCC> WaitingDCC = new List<RequestDCC>();

        private bool timer01_Tick()
        {
            try
            {
                if (Core.NotificationIsNowWaiting)
                {
                    Core.DisplayNote();
                }

                // there is no update needed so skip
                if (!Updated)
                {
                    return true;
                }

                Updated = false;

                tv.ColumnsAutosize();

                lock (queueDcc)
                {
                    foreach (ProtocolDCC dcc in queueDcc)
                    {
                        insertDcc(dcc);
                    }
                    queueDcc.Clear();
                }

                // we sort out all networks that are waiting to be inserted to list
                lock (queueNetwork)
                {
                    foreach (Network it in queueNetwork)
                    {
                        insertNetwork(it);
                    }
                    queueNetwork.Clear();
                }

                // we sort out all channels that are waiting to be inserted to list
                lock (queueChannels)
                {
                    foreach (Channel item in queueChannels)
                    {
                        insertChan(item);
                    }
                    queueChannels.Clear();
                }

                lock (queueQs)
                {
                    foreach (ProtocolQuassel item in queueQs)
                    {
                        insertQuassel(item);
                    }
                    queueQs.Clear();
                }

                // we sort out all services that are waiting to be inserted to list
                lock (queueProtocol)
                {
                    foreach (ProtocolSv item in queueProtocol)
                    {
                        insertService(item);
                    }
                    queueProtocol.Clear();
                }

                List<Channel> _channels = new List<Channel>();
                lock (ChannelList)
                {
                    foreach (var chan in ChannelList)
                    {
                        if (chan.Key.dispose)
                        {
                            if (chan.Key._Network.Channels.Contains(chan.Key))
                            {
                                chan.Key._Network.Channels.Remove(chan.Key);
                            }
                            Graphics.Window window = chan.Key.RetrieveWindow();
                            if (window != null)
                            {
                                window._Destroy();
                            }
                            _channels.Add(chan.Key);
                        }
                    }
                }

                foreach (var chan in _channels)
                {
                    ChannelList.Remove(chan);
                    chan.Destroy();
                }

                // if there are waiting window requests we process them here
                lock (Core.SystemForm.WindowRequests)
                {
                    foreach (Forms.Main._WindowRequest item in Core.SystemForm.WindowRequests)
                    {
                        item.window.CreateChat(item.owner, item.hasUserList, item.hasTextBox, item.focus);
                        if (item.owner != null && item.focus)
                        {
                            item.owner.ShowChat(item.name);
                        }
                    }
                    Core.SystemForm.WindowRequests.Clear();
                }

                lock (WaitingDCC)
                {
                    foreach (RequestDCC dcc in WaitingDCC)
                    {
                        Core.OpenDCC(dcc);
                    }
                    WaitingDCC.Clear();
                }

                lock (queueUsers)
                {
                    foreach (User user in queueUsers)
                    {
                        _insertUser(user);
                    }
                    queueUsers.Clear();
                }

                lock (ChannelList)
                {
                    foreach (var channel in ChannelList)
                    {
                        if (!channel.Key.IsDestroyed && channel.Key.Redraw)
                        {
                            channel.Key.RedrawUsers();
                        }
                    }
                }

                // we check if there are some data at all, if not we can safely remove
                // items in store and skip all checks
                if (IsEmpty)
                {
                    Values.Clear();
                    return true;
                }

                // check all destroyed windows
                TreeIter iter;
                if (Values.GetIterFirst(out iter))
                {
                    do
                    {
                        // in case the window is destroyed we need to remove the reference
                        var window = (Window)Values.GetValue(iter, 3);
                        if (window != null)
                        {
                            if (window.IsDestroyed)
                            {
                                Values.Remove(ref iter);
                            }
                        }
                    } while (Values.IterNext(ref iter));
                }

                ClearServer();
                ClearUser();
                ClearChan();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return true;
        }
    }
}
