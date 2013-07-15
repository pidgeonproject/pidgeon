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
using System.Threading;
using System.Text;

namespace Client
{
    /// <summary>
    /// Extension
    /// </summary>
    public partial class Extension
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
        /// This hook is part of contructor, you can override this with constructor of extension
        /// </summary>
        public virtual void Initialise()
        {
            if (Name == "Unknown extension")
            {
                _Status = Status.Stopped;
                throw new Exception("Extension had no Initialise() function and was terminated");
            }
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
