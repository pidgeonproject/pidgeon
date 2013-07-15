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
        /// Register a new plugin
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool RegisterPlugin(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    System.Reflection.Assembly library = System.Reflection.Assembly.LoadFrom(path);
                    if (library == null)
                    {
                        Core.DebugLog("Unable to load " + path + " because the file can't be read");
                        return false;
                    }
                    Type[] types = library.GetTypes();
                    Type type = typeof(Extension); // = library.GetType("Client.RestrictedModule");
                    Type pluginInfo = null;
                    foreach (Type curr in types)
                    {
                        if (curr.BaseType == type)
                        {
                            pluginInfo = curr;
                            break;
                        }
                    }
                    if (pluginInfo == null)
                    {
                        Core.DebugLog("Unable to load " + path + " because the library contains no module");
                        return false;
                    }
                    Extension _plugin = (Extension)Activator.CreateInstance(pluginInfo);
                    lock (Extensions)
                    {
                        if (Extensions.Contains(_plugin))
                        {
                            Core.DebugLog("Unable to load extension because the handle is already known to core");
                            return false;
                        }
                        bool problem = false;
                        foreach (Extension x in Core.Extensions)
                        {
                            if (x.Name == _plugin.Name)
                            {
                                Core.Ringlog("CORE: unable to load the extension, because the extension with same name is already loaded");
                                _plugin._Status = Extension.Status.Terminated;
                                problem = true;
                                break;
                            }
                        }
                        if (problem)
                        {
                            if (Core.Extensions.Contains(_plugin))
                            {
                                Core.Extensions.Remove(_plugin);
                            }
                        }
                        Core.Ringlog("CORE: everything is fine, registering " + _plugin.Name);
                        Extensions.Add(_plugin);
                    }
                    if (_plugin.Hook_OnRegister())
                    {
                        _plugin.Load();
                        _plugin._Status = Extension.Status.Active;
                        Core.Ringlog("CORE: finished loading of module " + _plugin.Name);
                        if (Core.SystemForm != null)
                        {
                            Core.SystemForm.main.scrollback.InsertText("Loaded plugin " + _plugin.Name + " (v. " + _plugin.Version + ")", Client.ContentLine.MessageStyle.System, false);
                        }
                        return true;
                    }
                    else
                    {
                        Core.Ringlog("CORE: failed to run OnRegister for " + _plugin.Name);
                    }
                    return false;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
            return false;
        }

        /// <summary>
        /// Register a new plugin
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        public static bool RegisterPlugin(Extension plugin)
        {
            if (plugin._Status == Extension.Status.Loading)
            {
                lock (Extensions)
                {
                    if (Extensions.Contains(plugin))
                    {
                        return false;
                    }
                    Extensions.Add(plugin);
                }
                if (plugin.Hook_OnRegister())
                {
                    plugin.Load();
                    if (Core.SystemForm != null)
                    {
                        Core.SystemForm.main.scrollback.InsertText("Loaded plugin " + plugin.Name + " (v. " + plugin.Version + ")", Client.ContentLine.MessageStyle.System, false);
                    }
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
