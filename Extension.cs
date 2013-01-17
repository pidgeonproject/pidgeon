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
using System.Linq;
using System.Text;

namespace Client
{
    public class Extension
    {
        public string Name = "Unknown extension";
        public string Version = "1.0";
        public string Description = "Author of this extension is stupid";
        public Version Required = new Version(1, 0, 8, 0);
        public Status _status;
        public bool RequiresReboot = false;

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
                _status = Status.Loading;
                Initialise();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
                Core.DebugLog("CORE: Terminating the extension " + Name);
                _status = Status.Terminated;
            }
        }

        public void Exit()
        {
            try
            {
                Core.DebugLog("CORE: Unloading " + Name);
                _status = Status.Terminating;
                lock (Core.Extensions)
                {
                    if (Core.Extensions.Contains(this))
                    {
                        Core.Extensions.Remove(this);
                        Core.DebugLog("CORE: Unloaded " + Name);
                    }
                }
                _status = Status.Terminated;
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

        public virtual void Hook_Network(Network network)
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
                    _status = Status.Stopped;
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
