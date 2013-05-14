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
using System.Threading;
using System.Text;

namespace Client
{
    /// <summary>
    /// Extension
    /// </summary>
    public class Extension
    {
        /// <summary>
        /// Name of extension
        /// </summary>
        public string Name = "Unknown extension";
        /// <summary>
        /// Version
        /// </summary>
        public string Version = "1.0";
        /// <summary>
        /// Description of extension
        /// </summary>
        public string Description = "There is no description provided";
        /// <summary>
        /// Minimal version of pidgeon required for this extension
        /// </summary>
        public Version Required = new Version(1, 2, 0);
        /// <summary>
        /// Status of extension
        /// </summary>
        public Status _Status = Status.Loading;
        /// <summary>
        /// If this extensions require pidgeon to be restarted in case it's loaded after core
        /// </summary>
        public bool RequiresReboot = false;
        /// <summary>
        /// List of threads associated with this extension
        /// </summary>
        public List<Thread> _Threads = new List<Thread>();

        /// <summary>
        /// This function is called on start of extension
        /// </summary>
        public static void Init()
        {
            return;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Extension()
        {
            try
            {
                _Status = Status.Loading;
                if (Required > System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)
                {
                    Core.DebugLog("CORE: Terminating the extension " + Name + " which requires pidgeon " + Required.ToString());
                    _Status = Status.Terminated;
                    return;
                }
                Initialise();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
                Core.DebugLog("CORE: Terminating the extension " + Name);
                _Status = Status.Terminated;
            }
        }

        /// <summary>
        /// Get the configuration value for extension
        /// </summary>
        /// <param name="key"></param>
        /// <param name="Default">Default value that should be returned in case that this key doesn't exist</param>
        /// <returns></returns>
        public string GetConfig(string key, string Default)
        {
            return Configuration.GetConfig(Name + "." + key, Default);
        }

        /// <summary>
        /// Get the configuration value for extension
        /// </summary>
        /// <param name="key"></param>
        /// <param name="Default">Default value that should be returned in case that this key doesn't exist</param>
        /// <returns></returns>
        public bool GetConfig(string key, bool Default)
        {
            return Configuration.GetConfig(Name + "." + key, Default);
        }

        /// <summary>
        /// Change the configuration option for this extension
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetConfig(string key, string value)
        {
            Configuration.SetConfig(Name + "." + key, value);
            Core._Configuration.ConfigSave();
        }

        /// <summary>
        /// Change the configuration option for this extension
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetConfig(string key, bool value)
        {
            Configuration.SetConfig(Name + "." + key, value);
            Core._Configuration.ConfigSave();
        }

        /// <summary>
        /// Change the configuration option for this extension
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetConfig(string key, long value)
        {
            Configuration.SetConfig(Name + "." + key, value);
            Core._Configuration.ConfigSave();
        }

        /// <summary>
        /// Change the configuration option for this extension
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetConfig(string key, int value)
        {
            Configuration.SetConfig(Name + "." + key, value);
            Core._Configuration.ConfigSave();
        }

        /// <summary>
        /// Delete config from memory
        /// </summary>
        /// <param name="key"></param>
        public void RemoveConfig(string key)
        {
            Configuration.RemoveConfig(Name + "." + key);
            Core._Configuration.ConfigSave();
        }

        /// <summary>
        /// Terminate this extension
        /// </summary>
        public void Exit()
        {
            try
            {
                Core.DebugLog("CORE: Unloading " + Name);
                _Status = Status.Terminating;
                if (!Hook_Unload())
                {
                    Core.DebugLog("CORE: Failed to unload " + Name + " forcefully removing from memory");
                    lock (_Threads)
                    {
                        foreach (Thread thread in _Threads)
                        {
                            try
                            {
                                if (thread.ThreadState == ThreadState.WaitSleepJoin || thread.ThreadState == ThreadState.Running)
                                {
                                    thread.Abort();
                                }
                            }
                            catch (Exception fail)
                            {
                                Core.DebugLog("CORE: Failed to unload " + Name + " error " + fail.ToString());
                            }
                        }
                    }
                }
                lock (Core.Extensions)
                {
                    if (Core.Extensions.Contains(this))
                    {
                        Core.Extensions.Remove(this);
                        Core.DebugLog("CORE: Unloaded " + Name);
                    }
                }
                _Status = Status.Terminated;
                Core.DebugLog("CORE: Terminated " + Name);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        /// <summary>
        /// This hook is called on load of extension
        /// </summary>
        /// <returns></returns>
        public virtual bool Hook_OnLoad()
        {
            return true;
        }

        /// <summary>
        /// This hook is started when extension is unloaded
        /// </summary>
        /// <returns></returns>
        public virtual bool Hook_Unload()
        {
            return true;
        }

        /// <summary>
        /// This hook is started when user part from a channel
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public virtual bool Hook_UserPart(Network network, User user, Channel channel, string message, bool updated)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when a network object is created
        /// </summary>
        /// <param name="network"></param>
        public virtual void Hook_Network(Network network)
        {
            return;
        }

        /// <summary>
        /// This hook is started after connection to a network
        /// </summary>
        /// <param name="network"></param>
        public virtual void Hook_AfterConnect(Network network)
        {
            
        }

        /// <summary>
        /// This hook is started before you connect to a protocol
        /// </summary>
        /// <param name="protocol"></param>
        public virtual void Hook_BeforeConnect(Protocol protocol)
        {
            return;
        }

        /// <summary>
        /// This hook is started when user talk in a channel
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public virtual bool Hook_UserTalk(Network network, User user, Channel channel, string message, bool updated)
        {
            return true;
        }

        /// <summary>
        /// This hook is started before pidgeon try to join a channel, return false will abort the action
        /// </summary>
        /// <param name="network"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public virtual bool Hook_BeforeJoin(Network network, string Channel)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when user quit
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="window"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public virtual bool Hook_UserQuit(Network network, User user, string message, Graphics.Window window, bool updated)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when user join to a channel you are in
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="updated"></param>
        /// <returns></returns>
        public virtual bool Hook_UserJoin(Network network, User user, Channel channel, bool updated)
        {
            return true;
        }

        /// <summary>
        /// This hook is started when main form is loaded
        /// </summary>
        /// <param name="main"></param>
        public virtual void Hook_Initialise(Client.Forms.Main main)
        {
            return;
        }

        /// <summary>
        /// This hook is started when tab key is pressed in a text box
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="text"></param>
        /// <param name="caret"></param>
        /// <param name="restore"></param>
        public virtual void Hook_InputOnTab(ref string prev, ref string text, ref int caret, ref bool restore)
        {
            return;
        }

        /// <summary>
        /// This hook is called before the Note is displayed
        /// </summary>
        /// <param name="name">Caption</param>
        /// <param name="text">Data</param>
        /// <returns></returns>
        public virtual bool Hook_BeforeNote(ref string name, ref string text)
        {
            return true;
        }

        /// <summary>
        /// This hook is called before the extension is loaded
        /// </summary>
        /// <returns></returns>
        public virtual bool Hook_OnRegister()
        {
            return true;
        }

        /// <summary>
        /// This function is called when network send us a network info
        /// </summary>
        /// <param name="network"></param>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        public virtual void Hook_NetworkInfo(Network network, string command, string parameters, string value)
        {
            
        }

        /// <summary>
        /// Called on notification display
        /// </summary>
        /// <param name="text"></param>
        /// <param name="InputStyle"></param>
        /// <param name="WriteLog"></param>
        /// <param name="Date"></param>
        /// <param name="SuppressPing"></param>
        /// <returns>if false, the notification is not displayed</returns>
        public virtual bool Hook_NotificationDisplay(string text, Client.ContentLine.MessageStyle InputStyle, ref bool WriteLog, long Date, ref bool SuppressPing)
        {
            return true;
        }

        /// <summary>
        /// Topic is being changed, this event happen before the topic is changed and if false is returned
        /// the topic change is ignored
        /// </summary>
        /// <param name="network"></param>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public virtual bool Hook_Topic(Network network, string userline, Channel channel, string topic)
        {
            return true;
        }

        /// <summary>
        /// This hook is part of contructor, you can override this with constructor of extension
        /// </summary>
        public virtual void Initialise()
        {
            return;
        }

        /// <summary>
        /// Load
        /// </summary>
        public void Load()
        {
            try
            {
                if (!Hook_OnLoad())
                {
                    Core.DebugLog("Unable to load " + Name + " the OnLoad hook returned invalid value");
                    _Status = Status.Stopped;
                    return;
                }
            }
            catch (Exception f)
            {
                Core.handleException(f);
            }
        }

        /// <summary>
        /// Status
        /// </summary>
        public enum Status
        { 
            /// <summary>
            /// Extension is ok
            /// </summary>
            Active,
            /// <summary>
            /// Extensions is loading
            /// </summary>
            Loading,
            /// <summary>
            /// Terminating
            /// </summary>
            Terminating,
            /// <summary>
            /// Terminated
            /// </summary>
            Terminated,
            /// <summary>
            /// Stopped
            /// </summary>
            Stopped
        }
    }

    [Serializable()]
    class TrustedExtension : Extension
    { 
        
    }
}
