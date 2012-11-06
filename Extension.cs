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
        public Status _status;
        public bool RequiresReboot = false;

        public static void Init()
        {
            //Core.RegisterPlugin(new Stage.extension_freenode());
        }

        public Extension()
        {
            _status = Status.Loading;
            Initialise();
        }

        public virtual bool Hook_OnLoad()
        {
            return true;
        }

        public virtual bool Hook_OnRegister()
        {
            return true;
        }

        public virtual void Initialise()
        { 
            
        }

        public void Load()
        {
            try
            {
                if (!Hook_OnLoad())
                {
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
}
