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
using System.Threading;
using System.Text;

namespace Client
{
    public class Timer
    {
        public class LastID
        {
            public int last_id = 0;
        }

        private static LastID last = new LastID();
        public int Time = 0;
        public string Command = null;
        public bool Running = false;
        public bool Permanent = false;
        private Thread th;

        public int ID
        {
            get
            {
                return id;
            }
        }
        private int id = 0;
        public Timer(int _Time, string _Command)
        {
            lock (last)
            {
                last.last_id++;
                id = last.last_id;
            }
            Command = _Command;
            Time = _Time;
            if (Time < 0)
            {
                Time = 0;
            }
            Core.Ringlog("Created timer " + ID.ToString() + " executing " + Command);
        }

        public void Execute()
        {
            if (!Running)
            {
                th = new Thread(Wait);
                th.Name = "Timer " + ID.ToString();
                Running = true;
                th.Start();
            }
        }


        /// <summary>
        /// Kill a timer
        /// </summary>
        /// <returns>True on succ</returns>
        public bool Kill()
        {
            try
            {
                if (!Running)
                {
                    Core.Ringlog("Unable to kill timer " + ID.ToString() + " because it's not running");
                    return false;
                }
                th.Abort();
                Running = false;
                return true;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
                return false;
            }
        }

        public void Wait()
        {
            try
            {
                System.Threading.Thread.Sleep(Time);
                Parser.parse(Command);
                Running = false;
                if (!Permanent)
                { 
                    lock (Core.TimerDB)
                    {
                        if (Core.TimerDB.Contains(this))
                        {
                            Core.TimerDB.Remove(this);
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Core.Ringlog("Timer " + ID + " was killed");
                Running = false;
                return;
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
                Running = false;
                return;
            }
        }
    }
}
