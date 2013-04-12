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
using System.Text;

namespace Client
{
    public partial class Commands
    {
        public class CommandLink
        {
            public bool Overrides = false;
            public string Target = null;
            public CommandLink(string target, bool overrides = false)
            {
                Target = target;
                Overrides = overrides;
            }
        }

        public class Command
        {
            private Type type;
            public Type _Type
            {
                get
                {
                    return type;
                }
            }
            private Action<string> action = null;

            public virtual void Launch(string parameter = null)
            {
                if (action != null)
                {
                    action(parameter);

                }
            }

            public virtual void Register()
            {
                // this function is called when command is created
            }

            public virtual void Unregister()
            {
                // this function is called when command is deleted
            }

            public Command(Type _type, Action<string> _Action = null)
            {
                type = _type;
                action = _Action;
                Register();
            }

            public Command(string ManualPage, string Name, Type _type, Action<string> _Action)
            {
                type = _type;
                action = _Action;
                RegisterManual(Name, ManualPage);
                Register();
            }

            ~Command()
            {
                Unregister();
            }
        }

        private static Dictionary<string, string> ManualPages = new Dictionary<string, string>();

        public enum Type
        {
            System,
            SystemSv,
            Services,
            Network,
            Plugin,
            User,
        }

        public static SortedDictionary<string, Command> commands = new SortedDictionary<string, Command>();

        public static void RegisterManual(string key, string value)
        {
            lock (ManualPages)
            {
                if (ManualPages.ContainsKey(key))
                {
                    ManualPages[key] = value;
                }
                else
                {
                    ManualPages.Add(key, value);
                }
            }
        }

        public static void RegisterCommand(string Name, Command command)
        {
            Core.DebugLog("Registering a new command by extension: " + Name);
            lock (commands)
            {
                commands.Add(Name, command);
            }
        }

        public static void Initialise()
        {
            try
            {
                commands.Add("server", new Command(Type.System, Generic.server));
                RegisterManual("server", Client.Properties.Resources.Server);
                commands.Add("nick", new Command(Type.System, Generic.nick));
                RegisterManual("connect", Client.Properties.Resources.Connect);
                commands.Add("connect", new Command(Type.Services, Generic.connect));
                commands.Add("join", new Command(Type.SystemSv, Generic.join));
                commands.Add("part", new Command(Type.Network));
                commands.Add("quit", new Command(Type.System, Generic.quit));
                commands.Add("query", new Command(Type.SystemSv, Generic.query));
                commands.Add("map", new Command(Type.Network));
                commands.Add("me", new Command(Type.SystemSv, Generic.msg2));
                commands.Add("msg", new Command(Type.SystemSv, Generic.msg1));
                commands.Add("away", new Command(Type.Network));
                commands.Add("mode", new Command(Type.Network));
                commands.Add("help", new Command(Type.Network));
                commands.Add("info", new Command(Type.Network));
                commands.Add("invite", new Command(Type.Network));
                commands.Add("ison", new Command(Type.Network));
                commands.Add("kline", new Command(Type.Network));
                commands.Add("knock", new Command(Type.Network));
                commands.Add("kill", new Command(Type.Network));
                commands.Add("links", new Command(Type.Network));
                commands.Add("list", new Command(Type.Network));
                commands.Add("names", new Command(Type.Network));
                commands.Add("namesx", new Command(Type.Network));
                commands.Add("ping", new Command(Type.Network));
                commands.Add("rehash", new Command(Type.Network));
                commands.Add("restart", new Command(Type.Network));
                commands.Add("service", new Command(Type.Network));
                commands.Add("servlist", new Command(Type.Network));
                commands.Add("squit", new Command(Type.Network));
                commands.Add("setname", new Command(Type.Network));
                commands.Add("silence", new Command(Type.Network));
                commands.Add("stats", new Command(Type.Network));
                commands.Add("summon", new Command(Type.Network));
                commands.Add("topic", new Command(Type.Network));
                commands.Add("trace", new Command(Type.Network));
                commands.Add("user", new Command(Type.Network));
                commands.Add("userip", new Command(Type.Network));
                commands.Add("version", new Command(Type.Network));
                commands.Add("wallops", new Command(Type.Network));
                RegisterManual("oper", Client.Properties.Resources.Oper);
                commands.Add("oper", new Command(Type.Network));
                commands.Add("who", new Command(Type.Network));
                commands.Add("whois", new Command(Type.Network));
                commands.Add("whowas", new Command(Type.Network));
                commands.Add("gline", new Command(Type.Network));
                commands.Add("zline", new Command(Type.Network));
                commands.Add("nickserv", new Command(Type.Network));
                commands.Add("raw", new Command(Type.System, Generic.raw));
                commands.Add("chanserv", new Command(Type.Network));
                commands.Add("ctcp", new Command(Type.SystemSv, Generic.ctcp));
                RegisterManual("pidgeon.uptime", Client.Properties.Resources.PidgeonUptime);
                commands.Add("pidgeon.uptime", new Command(Type.System, Generic.up));
                commands.Add("service.quit", new Command(Type.Services, Generic.service_quit));
                commands.Add("service.gnick", new Command(Type.Services, Generic.service_gnick));
                commands.Add("pidgeon.service", new Command(Type.System, Generic.pidgeon_service));
                commands.Add("pidgeon.quit", new Command(Type.System, Generic.pidgeon_quit));
                commands.Add("pidgeon.sleep", new Command(Type.System, Generic.sleep));
                commands.Add("pidgeon.timer", new Command(Type.System, Generic.timer));
                commands.Add("pidgeon.rehash", new Command(Type.System, Generic.pidgeon_rehash));
                commands.Add("pidgeon.displaytimers", new Command(Type.System, Generic.displaytmdb));
                commands.Add("pidgeon.batch", new Command(Type.System, Generic.pidgeon_batch));
                commands.Add("pidgeon.memory.clean.ring", new Command(Type.System, Generic.clearring));
                commands.Add("pidgeon.memory.clean.gc", new Command(Type.System, Generic.free));
                commands.Add("pidgeon.memory.clean.traffic", new Command(Type.System, Generic.sniffer));
                commands.Add("pidgeon.ring.show", new Command(Type.System, Generic.ring_show));
                commands.Add("pidgeon.ring.file.overwrite", new Command(Type.System, Generic.forced_pidgeon_file));
                commands.Add("pidgeon.ring.file", new Command(Type.System, Generic.pidgeon_file));
                RegisterManual("pidgeon.man", Client.Properties.Resources.PidgeonMan);
                commands.Add("pidgeon.man", new Command(Type.System, Generic.man));
                commands.Add("pidgeon.module", new Command(Type.System, Generic.module));
                RegisterManual("pidgeon.module", Client.Properties.Resources.PidgeonModule);
                commands.Add("pidgeon.services.info", new Command(Type.System, Generic.services_cache));
                commands.Add("pidgeon.services.clear", new Command(Type.System, Generic.services_clear));
                commands.Add("pidgeon.services.flush", new Command(Type.System, Generic.services_flush));
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public static bool Proccess(string command)
        {
            string[] values = command.Split(' ');
            string parameter = command.Substring(values[0].Length);
            if (parameter.Length > 0)
            {
                parameter = parameter.Substring(1);
            }
            lock (commands)
            {
                if (commands.ContainsKey(values[0]))
                {
                    // network commands are handled by server
                    if (commands[values[0]]._Type == Type.Network)
                    {
                        return false;
                    }
                    commands[values[0]].Launch(parameter);
                    return true;
                }
            }
            return false;
        }
    }
}
