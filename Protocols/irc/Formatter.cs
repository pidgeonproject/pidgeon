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

namespace Client.Protocols.irc
{
    /// <summary>
    /// This class allow you to format the mode to better look
    /// </summary>
    public class Formatter
    {
        private int ParametersPerOneLine = 2;
        private int ModesPerOneLine = 20;
        public string Prefix = "";
        private string buffer = null;
        /// <summary>
        /// If this is true the produced string will remove the modes
        /// </summary>
        public bool Removing = false;
        private List<SimpleMode> Mode = new List<SimpleMode>();

        public Formatter(int _ParametersPerOneLine, int _ModesPerOneLine)
        {
            ParametersPerOneLine = _ParametersPerOneLine;
            ModesPerOneLine = _ModesPerOneLine;
        }

        public void InsertModes(List<SimpleMode> mode)
        {
            lock (Mode)
            {
                Mode.AddRange(mode);
            }
        }

        public void RewriteBuffer(string data)
        {
            lock (buffer)
            {
                buffer = data;
            }
        }

        public void Format()
        {
            string modes = "+";
            if (Removing)
            {
                modes = "-";
            }
            string parameters = " ";
            int CurrentMode = 1;
            buffer = "";
            int CurrentLine = 1;
            int CurrentPm = 1;
            lock (Mode)
            {
                foreach (SimpleMode xx in Mode)
                {
                    if (CurrentMode > ModesPerOneLine || CurrentPm > ParametersPerOneLine)
                    {
                        buffer += Prefix + modes + parameters + "\n";
                        CurrentLine++;
                        if (Removing)
                        {
                            modes = "-";
                        }
                        else
                        {
                            modes = "+";
                        }
                        parameters = " ";
                        CurrentMode = 1;
                        CurrentPm = 1;
                    }
                    modes += xx.Mode.ToString();
                    CurrentMode++;
                    if (xx.ContainsParameter)
                    {
                        parameters += xx.Parameter + " ";
                        CurrentPm++;
                    }
                }
            }
        }

        public override string ToString()
        {
            lock (buffer)
            {
                Format();
                return buffer;
            }
        }
    }
}
