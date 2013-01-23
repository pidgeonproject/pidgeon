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
            Core.Ringlog("Created timer " + ID + " executing " + Command);
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
