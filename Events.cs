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

using System;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// This is similar to hooks but is more flexible
    /// </summary>
    class Events
    {
        /// <summary>
        /// System events
        /// </summary>
        public class _System
        {
            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void AfterCoreHandler(object sender, EventArgs e);
            public static event AfterCoreHandler AfterCore;

            public static void TriggerAfterCore(object sender, EventArgs e)
            {
                try
                {
                    if (AfterCore != null)
                    {
                        AfterCore(sender, e);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }

            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void BeforeNoteHandler(object sender, Extension.BeforeNoteArgs e);
            public static event BeforeNoteHandler BeforeNote;

            public static void TriggerBeforeNote(object sender, Extension.BeforeNoteArgs e)
            {
                try
                {
                    if (BeforeNote != null)
                    {
                        BeforeNote(sender, e);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }

            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void InitialiseHandler(object sender, Extension.SystemInitialiseArgs e);
            public static event InitialiseHandler Initialise;

            public static void TriggerInitialise(object sender, Extension.SystemInitialiseArgs e)
            {
                try
                {
                    if (Initialise != null)
                    {
                        Initialise(sender, e);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }

            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void BeforeOptionsHandler(object sender, Extension.BeforeOptionsArgs e);
            public static event BeforeOptionsHandler BeforeOptions;

            public static void TriggerBeforeOptions(object sender, Extension.BeforeOptionsArgs e)
            {
                try
                {
                    if (BeforeOptions != null)
                    {
                        BeforeOptions(sender, e);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }
        }

        public class _Window
        {
            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void BeforeUserMenuHandler(object sender, Extension.BeforeUserMenuArgs e);
            public static event BeforeUserMenuHandler BeforeUserMenu;

            public static void TriggerBeforeUserMenu(object sender, Extension.BeforeUserMenuArgs e)
            {
                try
                {
                    if (BeforeUserMenu != null)
                    {
                        BeforeUserMenu(sender, e);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }

            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void AfterUserMenuHandler(object sender, Extension.BeforeUserMenuArgs e);
            public static event AfterUserMenuHandler AfterUserMenu;

            public static void TriggerAfterUserMenu(object sender, Extension.BeforeUserMenuArgs e)
            {
                try
                {
                    if (AfterUserMenu != null)
                    {
                        AfterUserMenu(sender, e);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }
        }

        public class _Protocol
        {
            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void BeforeConnectHandler(object sender, Extension.NetworkInfo e);
            public static event BeforeConnectHandler BeforeConnect;

            public static void TriggerBeforeConnect(object sender, Extension.NetworkInfo e)
            {
                try
                {
                    if (BeforeConnect != null)
                    {
                        BeforeConnect(sender, e);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }
        }

        public class _Scrollback
        {
            public delegate void LinkMouseHoverHandler(object sender, Extension.MouseHoverArgs e);
            public static event LinkMouseHoverHandler OnMouseHover;

            public static void MouseHover(object sender, Extension.MouseHoverArgs e)
            {
                try
                {
                    if (OnMouseHover != null)
                    {
                        OnMouseHover(sender, e);
                    }
                }
                catch (Exception fail)
                {
                    Core.handleException(fail);
                }
            }
        }
    }
}
