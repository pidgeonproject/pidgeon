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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Pidgeon
{
    /// <summary>
    /// Window request, this class represent a request to create a new window and is usually done by a thread which can't control windows
    /// </summary>
    public class WindowRequest
    {
        /// <summary>
        /// Window handle
        /// </summary>
        public Graphics.Window window = null;
        /// <summary>
        /// Name
        /// </summary>
        public string name = null;
        /// <summary>
        /// Focus
        /// </summary>
        public bool focus = false;
        /// <summary>
        /// new window has a user list
        /// </summary>
        public bool hasUserList = true;
        /// <summary>
        /// Text
        /// </summary>
        public bool hasTextBox = true;
        public object Parent = null;
    }

    public class WindowsManager
    {
        /// <summary>
        /// Request db
        /// </summary>
        private static List<WindowRequest> WindowRequests = new List<WindowRequest>();
        /// <summary>
        /// Displayed window
        /// </summary>
        [NonSerialized]
        public static Graphics.Window CurrentWindow = null;
        /// <summary>
        /// Windows
        /// 
        /// Every window has unique string identifier, in order to prevent collisions, there are window groups,
        /// each group can belong to any object and this group has separate string identifiers.
        /// </summary>
        public static Dictionary<object, Dictionary<string, Graphics.Window>> Windows = new Dictionary<object, Dictionary<string, Pidgeon.Graphics.Window>>();
        private static object Root = new object();

        public static Graphics.Window GetWindow(string name, object parent = null)
        {
            if (parent == null)
            {
                parent = Root;
            }
            lock (Windows)
            {
                if (!Windows.ContainsKey (parent))
                {
                    return null;
                }
                lock (Windows[parent])
                {
                    if (!Windows [parent].ContainsKey (name))
                    {
                        return null;
                    }
                }
            }
            return Windows [parent] [name];
        }

        public static bool ShowChat(Graphics.Window chat)
        {
            if (chat != null)
            {
                CurrentWindow = chat;
                Core.SystemForm.SwitchWindow (CurrentWindow);
                CurrentWindow.Redraw ();
                if (CurrentWindow.IsChannel)
                {
                    if (Core.SelectedNetwork != null)
                    {
                        Core.SelectedNetwork.RenderedChannel = Core.SelectedNetwork.GetChannel (CurrentWindow.WindowName);
                    }
                }
                Core.SystemForm.SetChannel (chat.WindowName);
                if (CurrentWindow.Making == false)
                {
                    CurrentWindow.textbox.setFocus ();
                }
                Core.SystemForm.Chat = chat;
                CurrentWindow.Making = false;
                Core.SystemForm.UpdateStatus ();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Request window to be shown
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ShowChat(string name, object parent = null)
        {
            if (parent == null)
            {
                parent = Root;
            }
            lock (Windows)
            {
                if (!Windows.ContainsKey (parent))
                {
                    return false;
                }
                lock (Windows[parent])
                {
                    if (Windows [parent].ContainsKey (name))
                    {
                        ShowChat(Windows[parent][name]);
                    }
                }
            }
            return true;
        }

        private static void ClearWins()
        {
            lock (Windows)
            {
                foreach (var dict in Windows.Values)
                {
                    lock (dict)
                    {
                        foreach (var window in dict.Values)
                        {
                            window._Destroy ();
                        }
                        dict.Clear ();
                    }
                }
                Windows.Clear();
            }
            CurrentWindow = null;

        }

        /// <summary>
        /// Unregisters the object.
        /// </summary>
        /// <returns><c>true</c>, if object was unregistered, <c>false</c> otherwise.</returns>
        /// <param name="Object">Object.</param>
        public static bool UnregisterObject(object Object)
        {
            lock (Windows)
            {
                if (Windows.ContainsKey (Object))
                {
                    Windows [Object].Clear();
                    Windows.Remove(Object);
                    return true;
                }
            }
            return false;
        }

        public static void PendingRequests()
        {
            // if there are waiting window requests we process them here
            lock (WindowRequests)
            {
                foreach (WindowRequest item in WindowRequests)
                {
                    item.window.CreateChat(item.hasUserList, item.hasTextBox, item.focus);
                    if (item.focus)
                    {
                        WindowsManager.ShowChat(item.name, item.Parent);
                    }
                }
                WindowRequests.Clear();
            }
        }

        /// <summary>
        /// Create window
        /// </summary>
        /// <param name="name">Identifier of window, should be unique on network level, otherwise you won't be able to locate it</param>
        /// <param name="focus">Whether new window should be immediately focused</param>
        /// <param name="network">Network the window belongs to</param>
        /// <param name="channelw">If true a window will be flagged as channel</param>
        /// <param name="id"></param>
        /// <param name="hasUserList"></param>
        /// <param name="hasTextBox"></param>
        /// <returns></returns>
        public static Graphics.Window CreateChat(string name, bool focus, Network network, bool channelw = false, string id = null, bool hasUserList = true, 
                                                 bool hasTextBox = true, object parent = null)
        {
            // in case there is no parent object for this window, let's put it into root
            if (parent == null)
            {
                parent = Root;
            }
            WindowRequest request = new WindowRequest();
            if (id == null)
            {
                id = name;
            }
            request.name = name;
            request.window = new Graphics.Window();
            request.focus = focus;
            request.window._Network = network;
            request.window.WindowName = name;
            request.hasUserList = hasUserList;
            request.hasTextBox = hasTextBox;
            request.Parent = parent;

            if (network != null && !name.Contains("!"))
            { 
                RegisterWindow(parent, network.SystemWindowID + id, request.window);
            }
            else
            {
                RegisterWindow(parent, id, request.window);
            }

            if (channelw == true)
            {
                request.window.IsChannel = true;
            }

            lock (WindowRequests)
            {
                // Create a request to create this window
                WindowRequests.Add(request);
                Graphics.PidgeonList.Updated = true;
            }
            return request.window;
        }

        private static void RegisterWindow(object parent, string name, Graphics.Window window)
        {
            if (!Windows.ContainsKey (parent))
            {
                Windows.Add (parent, new Dictionary<string, Pidgeon.Graphics.Window> ());
            }
            Windows [parent].Add (name, window);
        }

        public static void UnregisterWindow(string name, object parent = null)
        {
            if (parent == null)
            {
                parent = Root;
            }
            lock (Windows)
            {
                if (!Windows.ContainsKey (parent))
                {
                    return;
                }
                lock (Windows[parent])
                {
                    if (Windows [parent].ContainsKey (name))
                    {
                        Windows [parent].Remove (name);
                    }
                }
            }
        }
    }
}

