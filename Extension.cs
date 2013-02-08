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
    public class Extension
    {
        public string Name = "Unknown extension";
        public string Version = "1.0";
        public string Description = "Author of this extension is stupid";
        public Version Required = new Version(1, 0, 9, 0);
        public Status _Status = Status.Loading;
        public bool RequiresReboot = false;
        public List<Thread> _Threads = new List<Thread>();

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

        public string GetConfig(string key, string Default)
        {
            return Configuration.GetConfig(Name + "." + key);
        }

        public bool GetConfig(string key, bool Default)
        {
            return Configuration.GetConfig(Name + "." + key, Default);
        }

        public void SetConfig(string key, string value)
        {
            Configuration.SetConfig(Name + "." + key, value);
            Core.ConfigSave();
        }

        public void SetConfig(string key, bool value)
        {
            Configuration.SetConfig(Name + "." + key, value);
            Core.ConfigSave();
        }

        public void SetConfig(string key, long value)
        {
            Configuration.SetConfig(Name + "." + key, value);
            Core.ConfigSave();
        }

        public void SetConfig(string key, int value)
        {
            Configuration.SetConfig(Name + "." + key, value);
            Core.ConfigSave();
        }

        public void RemoveConfig(string key)
        {
            Configuration.RemoveConfig(Name + "." + key);
            Core.ConfigSave();
        }

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

        public virtual bool Hook_Unload()
        {
            return true;
        }

        public virtual void Hook_Network(Network network)
        {
            return;
        }

        public virtual void Hook_BeforeConnect(Protocol protocol)
        {
            return;
        }

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

        public enum Status
        { 
            Active,
            Loading,
            Terminating,
            Terminated,
            Stopped
        }
    }

    [Serializable()]
    class TrustedExtension : Extension
    { 
        
    }
}
