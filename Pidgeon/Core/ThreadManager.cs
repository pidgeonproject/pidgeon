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

using System.IO;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pidgeon
{
    public static partial class Core
    {
        public static class ThreadManager
        {
            /// <summary>
            /// Thread for IO logs
            /// </summary>
            public static Thread Thread_logs = null;
            /// <summary>
            /// Thread for update system
            /// </summary>
            public static Thread ThUp = null;
            /// <summary>
            /// Recovery thread
            /// </summary>
            public static Thread RecoveryThread = null;
            private static List<Thread> ThreadPool = new List<Thread>();

            public static List<Thread> Threads
            {
                get
                {
                    List<Thread> rs = new List<Thread>();
                    lock (ThreadPool)
                    {
                         rs.AddRange(ThreadPool);
                    }
                    rs.AddRange(libirc.ThreadManager.ThreadList);
                    return rs;
                }
            }

            public static void KillThread(Thread thread)
            {
                if (thread == null)
                {
                    return;
                }
                if (thread == Core.KernelThread)
                {
                    Syslog.DebugLog("Refusing to kill kernel thread >:-|");
                    return;
                }
                if (thread != Thread.CurrentThread)
                {
                    if (thread.ThreadState == System.Threading.ThreadState.Running || thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                    {
                        thread.Abort();
                        Syslog.DebugLog("Killed thread " + thread.Name);
                    }
                    else
                    {
                        Syslog.DebugLog("Ignored request to abort thread in " + thread.ThreadState.ToString() + " " + thread.Name);
                    }
                }
                else
                {
                    Syslog.DebugLog("Ignored request to abort thread from within the same thread " + thread.Name);
                }
                UnregisterThread(thread);
            }

            public static void RegisterThread(Thread thread)
            {
                if (thread.Name == "")
                {
                    Syslog.DebugLog("No thread name provided for: " + thread.ManagedThreadId.ToString());
                }
                lock (ThreadPool)
                {
                    if (!ThreadPool.Contains(thread))
                    {
                        ThreadPool.Add(thread);
                    }
                }
            }

            public static void UnregisterThread(Thread thread)
            {
                lock (ThreadPool)
                {
                    if (ThreadPool.Contains(thread))
                    {
                        ThreadPool.Remove(thread);
                    }
                }
            }
        }
    }
}
