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

using System.IO;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Client
{
    public partial class Core
    {	
        public class IO
        {
            public class FileLine
            {
                public string filename;
                public string line;
                public FileLine(string File, string Line)
                {
                    line = Line;
                    filename = File;
                }
            }

            public static List<FileLine> processing = new List<FileLine>();
            public static List<FileLine> data = new List<FileLine>();

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

            public static void InsertText(string line, string file)
            {
                lock (processing)
                {
                    processing.Add(new FileLine(file, line));
                }
            }
        }
		
		public static Gdk.Color fromColor(System.Drawing.Color color)
		{
			Gdk.Color xx = new Gdk.Color(color.R, color.G, color.B);
			return xx;
		}
    }
}
