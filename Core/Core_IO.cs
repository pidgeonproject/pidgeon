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
using System.Net;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Client
{
    public partial class Core
    {
        /// <summary>
        /// Handler of storage
        /// </summary>
        public class IO
        {
            /// <summary>
            /// Line in a file
            /// </summary>
            public class FileLine
            {
                /// <summary>
                /// Name of file
                /// </summary>
                public string filename;
                /// <summary>
                /// Text
                /// </summary>
                public string line;

                /// <summary>
                /// Creates a new instance of this class
                /// </summary>
                /// <param name="File"></param>
                /// <param name="Line"></param>
                public FileLine(string File, string Line)
                {
                    line = Line;
                    filename = File;
                }
            }

            private static List<FileLine> processing = new List<FileLine>();
            private static List<FileLine> data = new List<FileLine>();

            /// <summary>
            /// Write all data to disk
            /// </summary>
            public static void Save()
            {
                lock (processing)
                {
                    if (processing.Count > 0)
                    {
                        data.AddRange(processing);
                        processing.Clear();
                    }
                }
                if (data.Count > 0)
                {
                    foreach (FileLine xx in data)
                    {
                        File.AppendAllText(xx.filename, xx.line);
                    }
                }
                data.Clear();
            }

            /// <summary>
            /// Load
            /// </summary>
            public static void Load()
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            Save();
                            System.Threading.Thread.Sleep(2000);
                        }
                        catch (ThreadAbortException)
                        {
                            Save();
                            return;
                        }
                        catch (Exception e1)
                        {
                            Core.handleException(e1);
                        }
                        Thread.Sleep(2000);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }

            /// <summary>
            /// Insert a line to a file
            /// </summary>
            /// <param name="line">Text to be inserted to this file</param>
            /// <param name="file">File</param>
            public static void InsertText(string line, string file)
            {
                lock (processing)
                {
                    processing.Add(new FileLine(file, line));
                }
            }
        }

        /// <summary>
        /// Retrieve a size in memory of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long GetSizeOfObject(object obj)
        {
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            var size = ms.Length;
            ms.Dispose();
            return size;
        }

        /// <summary>
        /// Convert a Color to Gdk version
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Gdk.Color FromColor(System.Drawing.Color color)
        {
            Gdk.Color xx = new Gdk.Color(color.R, color.G, color.B);
            return xx;
        }
    }
}
