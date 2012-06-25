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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockingJay
{
    class Core
    {
        public static DateTime LoadTime;
        public static string ConfigFile = "configuration.dat";
        public static string SelectedLanguage = "en";

        public static bool Load()
        {
            ConfigurationLoad();
            LoadTime = DateTime.Now;
            messages.data.Add("en", new messages.container("en"));
            return true;
        }

        public static bool ConfigurationLoad()
        {
            // Check if config file is present
            if (File.Exists(ConfigFile))
            { 

            }
            return false;
        }

        public static int handleException(Exception _exception)
        {
            Console.WriteLine(_exception.InnerException);
            return 0;
        }
    }
}
