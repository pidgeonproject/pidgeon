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
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;


namespace Client
{
    public class TclAPI
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Tcl_Obj
        {
            int dummy;
        }
        // [return: MarshalAs(UnmanagedType.I4)]
        public unsafe delegate int Tcl_ObjCmdProc(IntPtr clientData,
            IntPtr interp,
            Int32 objc,
            byte** argv);
        [DllImport("tcl84.DLL")]
        public static extern IntPtr Tcl_CreateInterp();
        [DllImport("tcl84.DLL")]
        public static extern int Tcl_DeleteInterp(IntPtr interp);
        [DllImport("tcl84.Dll")]
        public static extern int Tcl_Eval(IntPtr interp, [In] byte[] script);
        [DllImport("tcl84.Dll")]
        public static extern int Tcl_EvalObjEx(IntPtr interp, IntPtr obj, int flags);
        [DllImport("tcl84.Dll")]
        public static extern IntPtr Tcl_GetObjResult(IntPtr interp);
        [DllImport("tcl84.Dll")]
        public static extern String Tcl_GetStringResult(IntPtr interp);
        [DllImport("tcl84.Dll")]
        public static extern String Tcl_GetStringFromObj(IntPtr tclObj, ref int length);
        [DllImport("tcl84.Dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Tcl_CreateCommand(IntPtr interp, String cmdName, Tcl_ObjCmdProc proc, IntPtr clientData, IntPtr deleteProc);
        [DllImport("tcl84.Dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Tcl_CreateObjCommand(IntPtr interp, IntPtr cmdName, Tcl_ObjCmdProc proc, IntPtr clientData, IntPtr deleteProc);
        [DllImport("tcl84.Dll")]
        public static extern IntPtr Tcl_NewStringObj(String bytes, int length);
        [DllImport("tcl84.Dll")]
        public static extern IntPtr Tcl_NewIntObj(int value);
        [DllImport("tcl84.Dll")]
        public static extern IntPtr Tcl_NewDoubleObj(double value);
        [DllImport("tcl84.Dll")]
        public static extern IntPtr Tcl_NewBooleanObj(int value);
        [DllImport("tcl84.Dll")]
        public static extern int Tcl_SetObjResult(IntPtr interp, IntPtr objPtr);
    }

    public class TclInterpreter
    {
        private class TclCallbackDescription
        {
            public MethodInfo methodInfo;
            public object callbackKeeper;

        }
        private IntPtr interp;
        private ArrayList delegates = new ArrayList();
        public TclInterpreter()
        {
            interp = TclAPI.Tcl_CreateInterp();
            if (interp == IntPtr.Zero)
            {
                throw new SystemException("can not initialize Tcl interpreter");
            }
        }
        ~TclInterpreter()
        {
            TclAPI.Tcl_DeleteInterp(interp);
        }
        /// <summary>
        /// Evaluate Skript in Tcl Interpreter.
        /// The Returned value is 0 than OK and 1 or
        /// unequal 0 than Error.
        /// The Result get be retrived with property Result
        /// </summary>
        /// <param name="script">tcl script</param>
        /// <returns>Tcl Return code. 0 == Tcl_OK</returns>
        public int evalScript(string script)
        {
            // if (hused) gch.Free();
            Encoder d = Encoding.UTF8.GetEncoder();
            char[] schars = new char[script.Length];
            script.CopyTo(0, schars, 0, script.Length);
            int ilen = d.GetByteCount(schars, 0, script.Length, true);
            byte[] sbytes = new byte[ilen + 1];
            d.GetBytes(schars, 0, script.Length, sbytes, 0, true);
            sbytes[ilen] = 0;
            GCHandle gch = GCHandle.Alloc(sbytes, GCHandleType.Pinned);
            // hused = true;
            int ret = TclAPI.Tcl_Eval(interp, sbytes);
            gch.Free();
            return ret;
            // return TclAPI.Tcl_EvalObjEx(interp,getTclObject(script),0);
        }
        /// <summary>
        /// Return the result of script evaluation.
        /// </summary>
        public string Result
        {
            get
            {
                /*
                IntPtr obj = TclAPI.Tcl_GetObjResult(interp);
                if (obj == IntPtr.Zero) 
                {
                    return "NULL";
                } 
                else 
                {
                    int len = 0;
                    string ret = TclAPI.Tcl_GetStringFromObj(obj,ref len);
                    return ret;
                }
                */
                return TclAPI.Tcl_GetStringResult(interp);
            }
        }
        /// <summary>
        /// Only for internal callback useage.
        /// Set Result of interpreter.
        /// </summary>
        /// <param name="result">object</param>
        public void setResult(object result)
        {
            if (result == null) return;
            IntPtr tclObj = getTclObject(result);
            TclAPI.Tcl_SetObjResult(interp, tclObj);
        }
        /// <summary>
        /// Lowlevel API for callback registration as Tcl command.
        /// The delegate is resposible for marshalling parameters
        /// requires unsafe delegate
        /// </summary>
        /// <param name="name">name of new Tcl command</param>
        /// <param name="proc">callback delagate</param>
        public void registerProc(string name, TclAPI.Tcl_ObjCmdProc proc)
        {
            // prevent cleaning delegates
            delegates.Add(proc);
            TclAPI.Tcl_CreateCommand(interp, name, proc, IntPtr.Zero, IntPtr.Zero);
        }
        public Hashtable objCommandHash = new Hashtable();
        /// <summary>
        /// API for Callbackregistration as Tcl command
        /// The wrapper is resposible for marhaling tcl parameters to
        /// Net parameters.
        /// </summary>
        /// <param name="name">name of new Tcl command</param>
        /// <param name="keeper">object that holds the callback</param>
        /// <param name="methodInfo">MethodInfo of callback method</param>
        public void registerTclCommand(string name, object keeper, MethodInfo methodInfo)
        {
            if (methodInfo == null)
            {
                new SystemException("methodInfo ist null");
            }
            TclCallbackDescription cd = new TclCallbackDescription();
            cd.callbackKeeper = keeper;
            cd.methodInfo = methodInfo;
            objCommandHash[name] = cd;
            unsafe
            {
                registerProc(name, new TclAPI.Tcl_ObjCmdProc(this.invokeTclCommandMarshall));
            }
        }
        /// <summary>
        /// This method scan all object methods for TclMethod Attribute
        /// and all these methods will be registered as tcl commands
        /// </summary>
        /// <param name="obj">obj with methods</param>
        public void registerTclCommandsInObject(object obj)
        {
            foreach (MethodInfo mi in obj.GetType().GetMethods())
            {
                foreach (TclMethod vm in mi.GetCustomAttributes(typeof(TclMethod), false))
                {
                    registerTclCommand(vm.name, obj, mi);
                }
            }
        }
        /// <summary>
        /// It is suitable only to register static methods as tcl commands
        /// All methods must be marked with Attribute TclMethod
        /// </summary>
        /// <param name="type">holds tcl commands callbacks</param>
        public void registerTclCommandsForType(Type type)
        {
            foreach (MethodInfo mi in type.GetMethods())
            {
                if (mi.IsStatic)
                {
                    foreach (TclMethod vm in mi.GetCustomAttributes(typeof(TclMethod), false))
                    {
                        registerTclCommand(vm.name, null, mi);
                    }
                }
            }
        }
        #region object marshaling NET - Tcl
        private unsafe string stringFromArgv(byte** argv, int index)
        {
            Decoder dec = System.Text.Encoding.UTF8.GetDecoder();
            int plen = 0;
            for (plen = 0; argv[index][plen] != '\0'; plen++) ;
            byte[] barr = new byte[plen];
            for (int i = 0; i < plen; i++)
            {
                barr[i] = argv[index][i];
            }
            int len = dec.GetCharCount(barr, 0, plen);
            char[] input = new char[len];
            dec.GetChars(barr, 0, plen, input, 0);
            return new String(input, 0, len);
        }
        private object stringToType(string input, Type outtype)
        {
            if (outtype == typeof(string))
            {
                return input;
            }
            MethodInfo pm = outtype.GetMethod("Parse", new Type[] { typeof(string) });
            if (pm != null)
            {
                return pm.Invoke(null, new object[] { input });
            }
            throw new FormatException("can not convert string to " + outtype.Name);
        }
        private IntPtr getTclObject(object obj)
        {
            if (obj is string)
            {
                return TclAPI.Tcl_NewStringObj((string)obj, ((string)obj).Length);
            }
            else if (obj is Int32)
            {
                return TclAPI.Tcl_NewIntObj((int)obj);
            }
            else if (obj is Boolean)
            {
                return TclAPI.Tcl_NewBooleanObj((bool)obj ? 1 : 0);
            }
            else if (obj is Double)
            {
                return TclAPI.Tcl_NewDoubleObj((double)obj);
            }
            // Noch listen
            string srep = obj.ToString();
            return TclAPI.Tcl_NewStringObj(srep, srep.Length);
        }
        #endregion
        private unsafe int invokeForObject(object obj, MethodInfo mi, int argc, int startIndex, byte** argv)
        {
            try
            {
                ParameterInfo[] parameters = mi.GetParameters();
                if (parameters.Length != argc - startIndex - 1)
                {
                    setResult("Expect " + parameters.Length.ToString() + " parameters by got " + (argc - 1).ToString());
                    return 1;
                }
                object[] inparam = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    inparam[i] = stringToType(stringFromArgv(argv, i + 1 + startIndex), parameters[i].ParameterType);
                }
                setResult(mi.Invoke(obj, inparam));
            }
            catch (FormatException e)
            {
                setResult(e.Message);
                return 1;
            }
            return 0;
        }
        private unsafe int invokeTclCommandMarshall(IntPtr clientData, IntPtr interp, int argc, byte** argv)
        {
            string name = stringFromArgv(argv, 0);
            TclCallbackDescription cd = objCommandHash[name] as TclCallbackDescription;
            if (cd != null)
            {
                return invokeForObject(cd.callbackKeeper, cd.methodInfo, argc, 0, argv);
            }
            setResult("no associated object for method: " + name);
            return 1;
        }
        public unsafe Int32 testProc(IntPtr clientData, IntPtr interp, Int32 argc, byte** argv)
        {
            setResult("Test OK");
            return 0;
        }
        #region object interop
        /// <summary>
        /// To enable NET object marshalling in Tcl you must call with method.
        /// It create 4 new commands in Tcl (objinfoke, objinfo, objproperty, objfield)
        /// Than you must register NET object in Tcl as Name with
        /// procedure registerObject
        /// </summary>
        public void setObjectInterop()
        {
            unsafe
            {
                registerProc("objinvoke", new TclAPI.Tcl_ObjCmdProc(this.tclCommandObjectInvoke));
                registerProc("objinfo", new TclAPI.Tcl_ObjCmdProc(this.tclCommandObjectInfo));
                registerProc("objproperty", new TclAPI.Tcl_ObjCmdProc(this.tclCommandObjectProperty));
                registerProc("objfield", new TclAPI.Tcl_ObjCmdProc(this.tclCommandObjectField));
            }
        }
        /// <summary>
        /// Tcl Command: objfield objname fieldName $value
        /// Get or Set a field (instance varible) of NET object
        /// </summary>
        /// <param name="clientData"></param>
        /// <param name="interp"></param>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <returns></returns>
        private unsafe int tclCommandObjectField(IntPtr clientData, IntPtr interp, int argc, byte** argv)
        {
            if (argc < 3 || argc > 4)
            {
                setResult("expect : objfield obj fieldname ?value?");
                return 1;
            }
            object obj = getObject(stringFromArgv(argv, 1));
            if (obj == null) return 1;
            string propName = stringFromArgv(argv, 2);
            FieldInfo fi = obj.GetType().GetField(propName);
            if (fi == null)
            {
                setResult("field " + propName + " unknown for object");
                return 1;
            }
            if (argc == 3)
            {
                setResult(fi.GetValue(obj));
            }
            else if (argc == 4)
            {
                fi.SetValue(obj, stringToType(stringFromArgv(argv, 3), fi.FieldType));
            }
            return 0;
        }
        /// <summary>
        /// Tcl command objproperty objname propertyName ?value?
        /// get or set the property of object
        /// </summary>
        /// <param name="clientData"></param>
        /// <param name="interp"></param>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <returns></returns>
        private unsafe int tclCommandObjectProperty(IntPtr clientData, IntPtr interp, int argc, byte** argv)
        {
            if (argc < 3 || argc > 4)
            {
                setResult("expect : objattribute obj attributename ?value?");
                return 1;
            }
            object obj = getObject(stringFromArgv(argv, 1));
            if (obj == null) return 1;
            string propName = stringFromArgv(argv, 2);
            PropertyInfo pi = obj.GetType().GetProperty(propName);
            if (pi == null)
            {
                setResult("method " + propName + " unknown for object");
                return 1;
            }
            if (argc == 3)
            {
                if (pi.CanRead)
                {
                    setResult(pi.GetValue(obj, null));
                }
                else
                {
                    setResult("Can not read property: " + propName);
                    return 1;
                }
            }
            else if (argc == 4)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(obj, stringToType(stringFromArgv(argv, 3), pi.PropertyType), null);
                }
                else
                {
                    setResult("Can not write property: " + propName);
                    return 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// Tcl command objinvoke methodname ?parmaters? ...
        /// </summary>
        /// <param name="clientData"></param>
        /// <param name="interp"></param>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <returns></returns>
        private unsafe int tclCommandObjectInvoke(IntPtr clientData, IntPtr interp, int argc, byte** argv)
        {
            if (argc <= 2)
            {
                setResult("expect more arguments");
                return 1;
            }
            object obj = getObject(stringFromArgv(argv, 1));
            if (obj == null) return 1;
            string methodName = stringFromArgv(argv, 2);
            MethodInfo mi = obj.GetType().GetMethod(methodName);
            if (mi == null)
            {
                setResult("method " + methodName + " unknown for object");
                return 1;
            }
            return invokeForObject(obj, mi, argc, 2, argv);

        }
        /// <summary>
        /// Tcl command objinfo.
        /// objinfo objname type|methods|methodparam|properties|propertytype|fields|fieldtype ?name?
        /// objinfo - give alle registered object names
        /// </summary>
        /// <param name="clientData"></param>
        /// <param name="interp"></param>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <returns></returns>
        private unsafe int tclCommandObjectInfo(IntPtr clientData, IntPtr interp, int argc, byte** argv)
        {
            StringBuilder sb;
            if (argc == 1)
            {
                sb = new StringBuilder();
                foreach (string name in objtable.Keys)
                {
                    sb.Append(name);
                    sb.Append(' ');
                }
                setResult(sb.ToString());
                return 0;
            }
            if (argc <= 2)
            {
                setResult("expect: objinfo objname type|methods|methodparam|properties|propertytype|fields|fieldtype ?name?");
                return 1;
            }
            object obj = getObject(stringFromArgv(argv, 1));
            if (obj == null) return 1;
            string subcommand = stringFromArgv(argv, 2);
            switch (subcommand)
            {
                case "type":
                    setResult(obj.GetType().Name);
                    return 0;
                case "methods":
                    sb = new StringBuilder();
                    foreach (MethodInfo mi in obj.GetType().GetMethods())
                    {
                        sb.Append(mi.Name);
                        sb.Append(' ');
                    }
                    setResult(sb.ToString());
                    return 0;
                case "methodparams":
                    if (argc == 4)
                    {
                        MethodInfo mi = obj.GetType().GetMethod(stringFromArgv(argv, 3));
                        if (mi != null)
                        {
                            sb = new StringBuilder();
                            foreach (ParameterInfo pi in mi.GetParameters())
                            {
                                sb.Append(pi.ParameterType.Name);
                                sb.Append(' ');
                            }
                            setResult(sb.ToString());
                            return 0;
                        }
                        else
                        {
                            setResult("method unknown");
                            return 1;
                        }
                    }
                    setResult("await method name");
                    return 1;
                case "properties":
                    sb = new StringBuilder();
                    foreach (PropertyInfo mi in obj.GetType().GetProperties())
                    {
                        sb.Append(mi.Name);
                        sb.Append(' ');
                    }
                    setResult(sb.ToString());
                    return 0;
                case "propertytype":
                    if (argc == 4)
                    {
                        PropertyInfo pi = obj.GetType().GetProperty(stringFromArgv(argv, 3));
                        if (pi != null)
                        {
                            setResult(pi.PropertyType.Name);
                            return 0;
                        }
                        else
                        {
                            setResult("property unknown");
                            return 1;
                        }
                    }
                    setResult("await property name");
                    return 1;
                case "fields":
                    sb = new StringBuilder();
                    foreach (FieldInfo mi in obj.GetType().GetFields())
                    {
                        sb.Append(mi.Name);
                        sb.Append(' ');
                    }
                    setResult(sb.ToString());
                    return 0;
                case "fieldtype":
                    if (argc == 4)
                    {
                        FieldInfo fi = obj.GetType().GetField(stringFromArgv(argv, 3));
                        if (fi != null)
                        {
                            setResult(fi.FieldType.Name);
                            return 0;
                        }
                        else
                        {
                            setResult("field unknown");
                            return 1;
                        }
                    }
                    setResult("await field name");
                    return 1;
            }
            setResult("unknown subcommand " + subcommand);
            return 1;
        }

        public Hashtable objtable = new Hashtable();

        /// <summary>
        /// All NET objects that should be accessible in Tcl
        /// must be registered in TclWrapper.
        /// Each Object will be named as string
        /// You should unregister the objects if you do not use
        /// them because of blocking garbage collector
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        public void registerObject(object obj, string name)
        {
            objtable[name] = obj;
        }

        /// <summary>
        /// unregister all unused objects to let garbage collector work
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void unregisterObjectWithName(string name)
        {
            objtable.Remove(name);
        }
        public object getObject(string name)
        {
            if (objtable.ContainsKey(name))
            {
                return objtable[name];
            }
            setResult("net object with name " + name + " is not registered");
            return null;
        }
        #endregion
    }

    /// <summary>
    /// Method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TclMethod : Attribute
    {
        /// <summary>
        /// Name
        /// </summary>
        public string name;

        public TclMethod() { }

        public TclMethod(string name)
        {
            this.name = name;
        }
    }

    /// <summary>
    /// This is a class for tcl scripting
    /// </summary>
    public static class ScriptingCore
    {
        private static bool working;
        private static System.Threading.Thread thread;

        private static void Exec()
        {
            try
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
                if (Core.IgnoreErrors)
                {
                    return;
                }
            }
            catch (Exception er)
            {
                Core.handleException(er);
            }
        }

        private static void GenerateInterp(TclInterpreter interp)
        {
            interp.registerObject(Configuration.ChannelModes.aggressive_bans, "configuration_aggressive_bans");
            interp.registerObject(Configuration.ChannelModes.aggressive_channel, "configuration_aggressive_channel");
            interp.registerObject(Configuration.ChannelModes.aggressive_exception, "configuration_aggressive_exception");
            interp.registerObject(Configuration.ChannelModes.aggressive_invites, "configuration_aggressive_invites");
            interp.registerObject(Configuration.ChannelModes.aggressive_mode, "configuration_aggressive_mode");
            interp.registerObject(Configuration.Scrollback.chat_timestamp, "configuration_chat_timestamp");
            interp.registerObject(Configuration.irc.ConfirmAll, "configuration_confirm");
            interp.registerObject(Configuration.CurrentSkin, "configuration_skin_name");
            interp.registerObject(Configuration.irc.DefaultReason, "configuration_kickban_reason");
            interp.registerObject(Configuration.Services.Depth, "configuration_depth");
            interp.registerObject(Configuration.irc.DisplayCtcp, "configuration_display_ctcp");
            interp.registerObject(Configuration.Scrollback.format_date, "configuration_dateformat");
            interp.registerObject(Configuration.Scrollback.format_nick, "configuration_nickformat");
            interp.registerObject(Configuration.Kernel.HidingParsed, "configuration_hiding_parsed");
            interp.registerObject(Configuration.UserData.ident, "configuration_ident");
            interp.registerObject(Configuration.UserData.LastHost, "configuration_lasthost");
            interp.registerObject(Configuration.UserData.LastNick, "configuration_lastnick");
            interp.registerObject(Configuration.UserData.LastPort, "configuration_lastport");
            interp.registerObject(Configuration.irc.mq, "configuration_mq");
            interp.registerObject(Configuration.UserData.nick, "configuration_nick");
            interp.registerObject(Configuration.Kernel.Notice, "configuration_notice");
            interp.registerObject(Configuration.UserData.quit, "configuration_quit");
            interp.registerObject(Configuration.Scrollback.timestamp_mask, "configuration_timemask");
            interp.registerObject(Configuration.UserData.user, "configuration_user");
            interp.registerObject(Configuration.Version, "configuration_version");
            
        }

        private static void Load()
        {
            working = true;
            thread = new System.Threading.Thread(Exec);
            thread.Name = "ScriptingCore";
            Core.SystemThreads.Add(thread);
            thread.Start();
        }
    }
}
