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
    public class Configuration
    {
        public static bool Window_Maximized = true;
        public static int Window_Left = 0;

        public static string CommandPrefix = "/";

        public static int FontSize=2;

        public static string ident = "pidgeon";
        public static string quit = "Pidgeon client";
        public static string nick = "PidgeonUser";

        public static System.Drawing.Color color = System.Drawing.Color.Gray;
        public static System.Drawing.Color background = System.Drawing.Color.Black;

        public static string user = "My name is hidden, dude";

        public enum Priority
        {
            High,
            Normal,
            Low
        }
    }
}
