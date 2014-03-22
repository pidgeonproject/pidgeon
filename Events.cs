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
    public static class Events
    {
        /// <summary>
        /// System events
        /// </summary>
        public static class _System
        {
            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void AfterCoreHandler(object sender, EventArgs e);
            /// <summary>
            /// This event happens after the core is loaded
            /// </summary>
            public static event AfterCoreHandler AfterCore;

            /// <summary>
            /// This function is called by a related handler and should not be called anywhere else
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
                    Core.HandleException(fail);
                }
            }

            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void BeforeNoteHandler(object sender, Extension.BeforeNoteArgs e);
            /// <summary>
            /// This event happens before the note is displayed
            /// </summary>
            public static event BeforeNoteHandler BeforeNote;

            /// <summary>
            /// This function is called by a related handler and should not be called anywhere else
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
                    Core.HandleException(fail);
                }
            }

            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void InitialiseHandler(object sender, Extension.SystemInitialiseArgs e);
            /// <summary>
            /// This event happens when the application is initialised
            /// </summary>
            public static event InitialiseHandler Initialise;

            /// <summary>
            /// This function is called by a related handler and should not be called anywhere else
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
                    Core.HandleException(fail);
                }
            }

            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void BeforeOptionsHandler(object sender, Extension.BeforeOptionsArgs e);
            /// <summary>
            /// This event happens before the options dialog is shown
            /// </summary>
            public static event BeforeOptionsHandler BeforeOptions;

            /// <summary>
            /// This function is called by a related handler and should not be called anywhere else
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
                    Core.HandleException(fail);
                }
            }
        }

        /// <summary>
        /// Windows
        /// </summary>
        public static class _Window
        {
            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void BeforeUserMenuHandler(object sender, Extension.BeforeUserMenuArgs e);
            /// <summary>
            /// Event that happens before the user menu
            /// </summary>
            public static event BeforeUserMenuHandler BeforeUserMenu;

            /// <summary>
            /// This function is called by a related handler and should not be called anywhere else
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
                    Core.HandleException(fail);
                }
            }

            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void AfterUserMenuHandler(object sender, Extension.BeforeUserMenuArgs e);
            /// <summary>
            /// This even happens right before the user menu is displayed
            /// </summary>
            public static event AfterUserMenuHandler AfterUserMenu;

            /// <summary>
            /// This function is called by a related handler and should not be called anywhere else
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
                    Core.HandleException(fail);
                }
            }
        }

        /// <summary>
        /// Protocol
        /// </summary>
        public static class _Protocol
        {
            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void BeforeConnectHandler(object sender, Extension.NetworkInfo e);
            /// <summary>
            /// Event to happen before you connect to some protocol
            /// </summary>
            public static event BeforeConnectHandler BeforeConnect;

            /// <summary>
            /// This function is called by a related handler and should not be called anywhere else
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
                    Core.HandleException(fail);
                }
            }
        }

        /// <summary>
        /// Scrollback
        /// </summary>
        public static class _Scrollback
        {
            /// <summary>
            /// Handler for this event
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void LinkMouseHoverHandler(object sender, Extension.MouseHoverArgs e);
            /// <summary>
            /// This event is triggered when you mouse over a link
            /// </summary>
            public static event LinkMouseHoverHandler OnMouseHover;

            /// <summary>
            /// This function is called by a related handler and should not be called anywhere else
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public static void TriggerMouseHover(object sender, Extension.MouseHoverArgs e)
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
                    Core.HandleException(fail);
                }
            }
        }
    }
}
