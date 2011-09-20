using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LibOpenCraft
{
    public delegate void ModuleCallback(ref PacketReader packet_reader, PacketType p_type, ref ClientManager cm);

    public delegate PacketHandler ModuleAddonCallback(PacketType p_type, string CustomPacketType, ref PacketReader packet_reader, PacketHandler p, ref ClientManager cm);

    public static class ModuleHandler
    {
        #region Dictionarys
        internal static Dictionary<string, CoreEventModule> _CoreEventModules = new Dictionary<string, CoreEventModule>();
        internal static Dictionary<string, CoreModule> _CoreModules = new Dictionary<string, CoreModule>();
        internal static Dictionary<PacketType, ModuleCallback> Eventmodules = new Dictionary<PacketType, ModuleCallback>();
        #endregion Dictionarys

        public static bool CoreEventModulesStarted = false;
        public static bool CoreModulesStarted = false;

        #region Module Addon Callback After Packets Sent
        internal static Dictionary<string, ModuleAddonCallback> ModuleAddons = new Dictionary<string, ModuleAddonCallback>();
        private static int count_i = 0;
        /// <summary>
        /// returns a object with 2 object arrays
        /// first one is ModuleAddonCallback
        /// second one is a string its the key in the dictionary
        /// 
        /// if getKeyOnly is left as true it will only return the key 
        /// if not it will return the key and the delegate 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static object[] InvokeGetModuleAddon(int i, bool getKeyOnly)
        {
            if (getKeyOnly == true)
            {
                /*Assembly _Assemblies = Assembly.GetAssembly(ModuleHandler.ModuleAddons.Keys.GetType());
                Type _Type = ModuleHandler.ModuleAddons.Keys.GetType();
                // Get the desired method we want from the target type.
                MethodInfo _MethodInfo = null;
                try
                {
                    _MethodInfo = _Type.GetMethod("ElementAt");
                }
                catch (Exception ex)
                {
                    return new object[0];
                }
                string key = (string)_MethodInfo.Invoke(ModuleHandler.ModuleAddons, new object[] {ModuleHandler.ModuleAddons, (object)i });
                */
                return new object[1] { ModuleHandler.ModuleAddons.Keys.ElementAt(i) };
            }
            else
            {
                /*Assembly _Assemblies = Assembly.GetAssembly(ModuleHandler.ModuleAddons.GetType());
                Type _Type = ModuleHandler.ModuleAddons.GetType();
                // Get the desired method we want from the target type.
                MethodInfo _MethodInfo = null;
                try
                {
                    _MethodInfo = _Type.GetMethod("ElementAt");
                }
                catch (Exception ex)
                {
                    return new object[0];
                }
                ModuleAddonCallback MAC_Temp = (ModuleAddonCallback)_MethodInfo.Invoke(ModuleHandler.ModuleAddons, new object[] {ModuleHandler.ModuleAddons, (object)i });

                _Assemblies = Assembly.GetAssembly(ModuleHandler.ModuleAddons.Keys.GetType());
                _Type = ModuleHandler.ModuleAddons.Keys.GetType();
                // Get the desired method we want from the target type.
                _MethodInfo = null;
                try
                {
                    _MethodInfo = _Type.GetMethod("ElementAt");
                }
                catch (Exception ex)
                {
                    return new object[0];
                }*/
                //(string)_MethodInfo.Invoke(ModuleHandler.ModuleAddons, new object[] { ModuleHandler.ModuleAddons, (object)i });
                return new object[2] { ModuleHandler.ModuleAddons.Keys.ElementAt(i), ModuleHandler.ModuleAddons.Values.ElementAt(i) };
            }
        }

        public static int InvokeGetModuleAddonCount()
        {
            Assembly _Assemblies = Assembly.GetAssembly(ModuleHandler.ModuleAddons.GetType());
            Type _Type = ModuleHandler.ModuleAddons.GetType();

            // Get the desired method we want from the target type.
            PropertyInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetProperty("Count");
            }
            catch (Exception ex)
            {
                return -1;
            }
            return (int)_MethodInfo.GetValue(ModuleHandler.ModuleAddons, null);
        }
        

        public static void InvokeAddModuleAddon(PacketType p, ModuleAddonCallback m)
        {
            Assembly _Assemblies = Assembly.GetAssembly(ModuleHandler.ModuleAddons.GetType());
            Type _Type = ModuleHandler.ModuleAddons.GetType();

            // Get the desired method we want from the target type.
            MethodInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetMethod("Add");
            }
            catch (Exception ex)
            {
                return;
            }
            _MethodInfo.Invoke(ModuleHandler.ModuleAddons, new object[2] { (p.ToString() + "_" + count_i), (object)m });
            count_i++;
        }
        public static void InvokeRemoveModuleAddon(string p)
        {
            Assembly _Assemblies = Assembly.GetAssembly(ModuleHandler.ModuleAddons.GetType());
            Type _Type = ModuleHandler.ModuleAddons.GetType();

            // Get the desired method we want from the target type.
            MethodInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetMethod("Remove");
            }
            catch (Exception ex)
            {
                return;
            }
            _MethodInfo.Invoke(ModuleHandler.ModuleAddons, new object[1] { p });
            count_i--;
        }
        #endregion Module Addon Callback

        #region event module add/remove
        public static bool AddEventModule(PacketType ptype, ModuleCallback m)
        {

            if (InvokeEventModuleContains(ptype) == true)
                return false;
            else
            {
                InvokeAddEventModule(ptype, m);
                return true;
            }
        }

        public static bool RemoveEventModule(PacketType ptype)
        {
            if (ModuleHandler.Eventmodules.ContainsKey(ptype) == true)
            {
                InvokeRemoveEventModule(ptype);
                return true;
            }
            else
                return false;
        }
        public static bool InvokeEventModuleContains(PacketType ptype)
        {
            Assembly _Assemblies = Assembly.GetAssembly(ModuleHandler.Eventmodules.GetType());
            Type _Type = ModuleHandler.Eventmodules.GetType();

            // Get the desired method we want from the target type.
            MethodInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetMethod("ContainsKey");
            }
            catch (Exception ex)
            {
                return true;
            }
            return (bool)_MethodInfo.Invoke(ModuleHandler.Eventmodules, new object[1] { ptype });
        }
        public static void InvokeAddEventModule(PacketType ptype, ModuleCallback m)
        {
            Assembly _Assemblies = Assembly.GetAssembly(ModuleHandler.Eventmodules.GetType());
            Type _Type = ModuleHandler.Eventmodules.GetType();

            // Get the desired method we want from the target type.
            MethodInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetMethod("Add");
            }
            catch (Exception ex)
            {
                return;
            }
            _MethodInfo.Invoke(ModuleHandler.Eventmodules, new object[2] { ptype, (object)m });
        }
        public static void InvokeRemoveEventModule(PacketType ptype)
        {
            Assembly _Assemblies = Assembly.GetAssembly(ModuleHandler.Eventmodules.GetType());
            Type _Type = ModuleHandler.Eventmodules.GetType();

            // Get the desired method we want from the target type.
            MethodInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetMethod("Remove");
            }
            catch (Exception ex)
            {
                return;
            }
            _MethodInfo.Invoke(ModuleHandler.Eventmodules, new object[1] { ptype });
        }

        #endregion  event module add/remove

        #region Core Event Modules
        public static bool AddCoreEventModule(string name, CoreEventModule m)
        {
            if (ModuleHandler._CoreEventModules.ContainsKey(name))
            {
                return false;
            }
            else
            {
                ModuleHandler._CoreEventModules.Add(name, m);
                return true;
            }
        }
        public static bool RemoveCoreEventModule(string name)
        {
            if (ModuleHandler._CoreEventModules.ContainsKey(name))
            {
                ModuleHandler._CoreEventModules[name].Stop();
                ModuleHandler._CoreEventModules.Remove(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void StartCoreEventModule(string key)
        {
            if (ModuleHandler._CoreEventModules.ContainsKey(key) && ModuleHandler._CoreEventModules[key].started == false)
            {
                ModuleHandler._CoreEventModules[key].Start();
            }
            else if(ModuleHandler._CoreEventModules.ContainsKey(key) && ModuleHandler._CoreEventModules[key].started == false)
            {
                ModuleHandler._CoreEventModules[key].Stop();
                ModuleHandler._CoreEventModules[key].Start();
            }
        }

        public static void StopCoreEventModule(string key)
        {
            if (ModuleHandler._CoreEventModules.ContainsKey(key) && ModuleHandler._CoreEventModules[key].started == true)
            {
                ModuleHandler._CoreEventModules[key].Stop();
            }
        }

        public static void StartCoreEventModules()
        {
            if (CoreModulesStarted == false)
            {
                CoreModulesStarted = true;
                int i = 0;
                for (; i < ModuleHandler._CoreEventModules.Count(); i++)
                {
                    string key = ModuleHandler._CoreEventModules.ElementAt(i).Key;
                    if (ModuleHandler._CoreEventModules[key].started == false)
                        ModuleHandler._CoreEventModules[key].Start();
                }
            }
            else
            {
                int i = 0;
                for (; i < ModuleHandler._CoreEventModules.Count(); i++)
                {
                    string key = ModuleHandler._CoreEventModules.ElementAt(i).Key;
                    if (ModuleHandler._CoreEventModules[key].started == true)
                        ModuleHandler._CoreEventModules[key].Stop();
                    ModuleHandler._CoreEventModules[key].Start();
                }
            }
        }
        public static void StopCoreEventModules()
        {
            if (CoreModulesStarted == true)
            {
                int i = 0;
                for (; i < ModuleHandler._CoreEventModules.Count(); i++)
                {
                    string key = ModuleHandler._CoreEventModules.ElementAt(i).Key;
                    if (ModuleHandler._CoreEventModules[key].started == true)
                        ModuleHandler._CoreEventModules[key].Stop();
                }
                CoreModulesStarted = false;
            }
            else
            {
                int i = 0;
                for (; i < ModuleHandler._CoreEventModules.Count(); i++)
                {
                    
                    string key = ModuleHandler._CoreEventModules.ElementAt(i).Key;
                    if (ModuleHandler._CoreEventModules[key].started == true)
                        ModuleHandler._CoreEventModules[key].Stop();
                    
                }
            }
        }
        #endregion

        #region Core Modules
        public static bool AddCoreModule(string name, CoreModule m)
        {
            if (ModuleHandler._CoreModules.ContainsKey(name))
            {
                return false;
            }
            else
            {
                ModuleHandler._CoreModules.Add(name, m);
                return true;
            }
        }
        public static bool RemoveCoreModule(string name)
        {
            if (ModuleHandler._CoreModules.ContainsKey(name))
            {
                ModuleHandler._CoreModules[name].Stop();
                ModuleHandler._CoreModules.Remove(name);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void StartCoreModule(string key)
        {
            if (ModuleHandler._CoreModules.ContainsKey(key) && ModuleHandler._CoreModules[key].started == false)
            {
                InvokeStartCoreModule(ModuleHandler._CoreModules[key]);
            }
            else if (ModuleHandler._CoreModules.ContainsKey(key) && ModuleHandler._CoreModules[key].started == false)
            {
                InvokeStopCoreModule(ModuleHandler._CoreModules[key]);
                InvokeStartCoreModule(ModuleHandler._CoreModules[key]);
            }
        }

        public static void StopCoreModule(string key)
        {
            if (ModuleHandler._CoreModules.ContainsKey(key) && ModuleHandler._CoreModules[key].started == true)
            {
                InvokeStopCoreModule(ModuleHandler._CoreModules[key]);
            }
        }

        public static void StartCoreModules()
        {
            if (CoreModulesStarted == false)
            {
                CoreModulesStarted = true;
                int i = 0;
                for (; i < ModuleHandler._CoreModules.Count(); i++)
                {
                    string key = ModuleHandler._CoreModules.ElementAt(i).Key;
                    if (ModuleHandler._CoreModules[key].started == false)
                        InvokeStartCoreModule(ModuleHandler._CoreModules[key]);
                }
            }
            else
            {
                int i = 0;
                for (; i < ModuleHandler._CoreModules.Count(); i++)
                {
                    string key = ModuleHandler._CoreModules.ElementAt(i).Key;
                    if (ModuleHandler._CoreModules[key].started == true)
                        InvokeStopCoreModule(ModuleHandler._CoreModules[key]);
                    InvokeStartCoreModule(ModuleHandler._CoreModules[key]);
                }
            }
        }
        public static void StopCoreModules()
        {
            if (CoreModulesStarted == true)
            {
                int i = 0;
                for (; i < ModuleHandler._CoreModules.Count(); i++)
                {
                    string key = ModuleHandler._CoreModules.ElementAt(i).Key;
                    if (ModuleHandler._CoreModules[key].started == true)
                        InvokeStopCoreModule(ModuleHandler._CoreModules[key]);
                }
                CoreModulesStarted = false;
            }
            else
            {
                int i = 0;
                for (; i < ModuleHandler._CoreModules.Count(); i++)
                {

                    string key = ModuleHandler._CoreModules.ElementAt(i).Key;
                    if (ModuleHandler._CoreModules[key].started == true)
                        InvokeStopCoreModule(ModuleHandler._CoreModules[key]);

                }
            }
        }
        public static void InvokeStartCoreModule(CoreModule cm)
        {
            Assembly _Assemblies = Assembly.GetAssembly(cm.GetType());
            Type _Type = cm.GetType();

            // Get the desired method we want from the target type.
            MethodInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetMethod("Start");
            }
            catch (Exception ex)
            {
                return;
            }
            _MethodInfo.Invoke(cm, null);
        }

        public static void InvokeStopCoreModule(CoreModule cm)
        {
            Assembly _Assemblies = Assembly.GetAssembly(cm.GetType());
            Type _Type = cm.GetType();

            // Get the desired method we want from the target type.
            MethodInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetMethod("Stop");
            }
            catch (Exception ex)
            {
                return;
            }
            _MethodInfo.Invoke(cm, null);
        }
        #endregion
    }

    public class CoreModule
    {
        protected Dictionary<string, ModuleAddonCallback> ModuleAddons = new Dictionary<string, ModuleAddonCallback>();
        public bool started;
        protected PacketType _ptype;
        public CoreModule()
        {
            started = false;
        }
        public virtual void Start()
        {
            started = true;
        }
        public virtual void Stop()
        {
            started = false;
        }
        public void RunModuleCache()
        {
            try
            {
                int count = ModuleHandler.InvokeGetModuleAddonCount();
                int i = 0;
                for (; i < count; i++)
                {
                    object[] obj_temp = ModuleHandler.InvokeGetModuleAddon(i, true);
                    string key = (string)obj_temp[0];
                    string[] split_key = key.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split_key[0] == _ptype.ToString())
                    {
                        obj_temp = ModuleHandler.InvokeGetModuleAddon(i, false);
                        key = (string)obj_temp[0];
                        ModuleAddonCallback mac = (ModuleAddonCallback)obj_temp[1];
                        if (ModuleAddons.ContainsKey(key))
                            ModuleAddons.Remove(key);
                        ModuleAddons.Add(key, mac);
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
        }
    }

    public class CoreEventModule
    {
        protected Dictionary<string, ModuleAddonCallback> ModuleAddons = new Dictionary<string, ModuleAddonCallback>();
        public bool started;
        PacketType _ptype;
        public CoreEventModule(PacketType ptype)
        {
            _ptype = ptype;
            started = false;
        }
        public virtual void Start()
        {
            started = true;
        }
        public virtual void Stop()
        {
            started = false;
        }
        public void RunModuleCache()
        {
            int count = ModuleHandler.InvokeGetModuleAddonCount();
            int i = 0;
            for (; i < count; i++)
            {
                object[] obj_temp = ModuleHandler.InvokeGetModuleAddon(i, true);
                string key = (string)obj_temp[0];
                string[] split_key = key.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (split_key[0] == _ptype.ToString())
                {
                    obj_temp = ModuleHandler.InvokeGetModuleAddon(i, false);
                    key = (string)obj_temp[0];
                    ModuleAddonCallback mac = (ModuleAddonCallback)obj_temp[1];
                    ModuleAddons.Add(key, mac);
                }
                else
                {

                }
            }
        }
    }

    public class PlayerModule
    {
        public PlayerModule()
        {

        }
        public void Start()
        {

        }
        public void Stop()
        {

        }
    }
}
