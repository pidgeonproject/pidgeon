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
using System.Text.RegularExpressions;
using System.Text;

namespace Client
{
    class Ignoring
    {
        public class Ignore
        {
            /// <summary>
            /// If this item should be ignored
            /// </summary>
            public bool Enabled = true;
            /// <summary>
            /// What to ignore
            /// </summary>
            public string Text = null;
            /// <summary>
            /// Regex
            /// </summary>
            public Regex regex = null;
            /// <summary>
            /// If true there is no regex
            /// </summary>
            public bool Simple = false;
            /// <summary>
            /// Type
            /// </summary>
            public Type type = Type.User;

            public Ignore(bool enabled, bool simple, string data, Type _Type)
            {
                Enabled = enabled;
                if (!simple)
                {
                    regex = new Regex(data);
                }
                type = _Type;
                Text = data;
                Simple = simple;
            }

            public enum Type
            {
                Everything,
                User
            }
        }

        public static List<Ignore> IgnoreList = new List<Ignore>();

        public static bool Matches(string text, User user = null)
        {
            // we need to walk through list of all ignores to find out
            foreach (Ignore x in IgnoreList)
            {
                // first of all we need to know if it's enabled, otherwise there is no point in checking it
                if (x.Enabled)
                {
                    // if it's simple match we only check the text
                    if (x.Simple)
                    {
                        // if user is null we can skip it
                        if (user != null && x.type == Ignore.Type.User)
                        {
                            if (user.ToString().Contains(x.Text))
                            {
                                return true;
                            }
                        }
                        if (x.type == Ignore.Type.Everything)
                        {
                            if (text.Contains(x.Text))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (user != null && x.type == Ignore.Type.User)
                        {
                            if (x.regex.IsMatch(user.ToString()))
                            {
                                return true;
                            }
                        }
                        if (x.type == Ignore.Type.Everything)
                        {
                            if (x.regex.IsMatch(text))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
