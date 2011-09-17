using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Reflection;

namespace LibOpenCraft
{
    public class GridServer
    {
        private CompositionContainer _container;

        #region Player data networking code
        public static Dictionary<int, ClientManager> players = new Dictionary<int, ClientManager>();

        public static int NewPlayer(TcpClient client)
        {
            int count = GridServer.InvokeCountPlayer();
            GridServer.InvokeAddPlayer(count, new ClientManager(client));
            players[count].id = count;
            return players[count].id;
        }
        public static int InvokeCountPlayer()
        {
            Assembly _Assemblies = Assembly.GetAssembly(GridServer.players.GetType());
            Type _Type = GridServer.players.GetType();

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
            return (int)_MethodInfo.GetValue(GridServer.players, null);
        }
        public static bool InvokeContainsKeyPlayer(int id)
        {
            Assembly _Assemblies = Assembly.GetAssembly(GridServer.players.GetType());
            Type _Type = GridServer.players.GetType();

            // Get the desired method we want from the target type.
            MethodInfo _MethodInfo = null;
            try
            {
                _MethodInfo = _Type.GetMethod("ContainsKey");
            }
            catch (Exception ex)
            {
                return false;
            }
            return (bool)_MethodInfo.Invoke(GridServer.players, new object[1] { ((object)id) });
        }
        public static void InvokeAddPlayer(int id, ClientManager cm)
        {
            Assembly _Assemblies = Assembly.GetAssembly(GridServer.players.GetType());
            Type _Type = GridServer.players.GetType();

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
            _MethodInfo.Invoke(GridServer.players, new object[2] { ((object)id), (object)cm });
        }
        public static void InvokeRemovePlayer(int id)
        {
            Assembly _Assemblies = Assembly.GetAssembly(GridServer.players.GetType());
            Type _Type = GridServer.players.GetType();

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
            _MethodInfo.Invoke(GridServer.players, new object[1] { ((object)id) });
        }

        public static ClientManager InvokeGetPlayer(int id)
        {
            if (InvokeContainsKeyPlayer(id))
            {
                return GridServer.players[id];
            }
            else
                return null;
        }

        #endregion Player data networking code
        public GridServer()
        {
            Config.InitializeSettings();
            SetupModules();
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "World\\" + "level.dat"))
            {
                World.LoadWorld();
            }
            else
            {
                World.GenerateWorld();
            }
        }
        public void SetupModules()
        {
            #region Module Container Loading

            AggregateCatalog catalog = new AggregateCatalog();

            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            DirectoryCatalog directoryCatalog = new DirectoryCatalog(".", "LibOpenCraft.*.dll");

            catalog.Catalogs.Add(assemblyCatalog);
            catalog.Catalogs.Add(directoryCatalog);

            _container = new CompositionContainer(catalog, true);

            try
            {

            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                StringBuilder error = new StringBuilder("Error(s) encountered loading extension modules. You may have an incompatible or out of date extension .dll in the current folder.");
                foreach (Exception loaderEx in ex.LoaderExceptions)
                    error.Append("\n " + loaderEx.Message);
            }

            #endregion Module Container Loading

            #region Module Loading
            Console.WriteLine("Loading Modules Initiated....");
            SetupCoreModules();

            SetupCoreEventModules();
            Console.WriteLine("Done loading modules.");
            #endregion Module Loading
        }
        public void SetupCoreModules()
        {
            IEnumerable<Lazy<object, object>> exportEnumerable = _container.GetExports(typeof(CoreModule), null, null);
            foreach (Lazy<object, object> lazyExport in exportEnumerable)
            {
                IDictionary<string, object> metadata = (IDictionary<string, object>)lazyExport.Metadata;
                object nameObj;
                if (metadata.TryGetValue("Name", out nameObj))
                {
                    string name = (string)nameObj;
                    Console.WriteLine("Adding " + name + " Event Module.");
                    ModuleHandler.AddCoreModule(name, (CoreModule)lazyExport.Value);
                }
            }
            Console.WriteLine("Starting Core Modules.....");
            ModuleHandler.StartCoreEventModules();
            Console.WriteLine("Core Modules Started.....");
        }
        public void SetupCoreEventModules()
        {
            Console.WriteLine("Loading modules...");
            IEnumerable<Lazy<object, object>> exportEnumerable = _container.GetExports(typeof(CoreEventModule), null, null);
            foreach (Lazy<object, object> lazyExport in exportEnumerable)
            {
                IDictionary<string, object> metadata = (IDictionary<string, object>)lazyExport.Metadata;
                object nameObj;
                if (metadata.TryGetValue("Name", out nameObj))
                {
                    string name = (string)nameObj;
                    Console.WriteLine("Adding " + name + " Core Event Module.");
                    ModuleHandler.AddCoreEventModule(name, (CoreEventModule)lazyExport.Value);
                }
            }
            Console.WriteLine("Starting Core Event Modules.....");
            ModuleHandler.StartCoreEventModules();
            Console.WriteLine("Core Event Modules Started.....");
        }
    }
}
