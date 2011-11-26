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
        public static ClientManager[] player_list;

        public static bool ContainsPlayer(int number)
        {
            int i = 0;
            while (i < player_list.Length)
            {
                if (player_list[i] != null && player_list[i].id == number)
                    return true;
                i++;
            }
            return false;
        }
        public static void SendToAll(PacketHandler p)
        {
            for (int i = 0; i < GridServer.player_list.Length; i++)
            {
                if (GridServer.player_list[i] != null)
                {
                    GridServer.player_list[i].WaitToRead = true;
                    GridServer.player_list[i].SendPacket(p, GridServer.player_list[i].id, ref GridServer.player_list[i], false, false);
                }
            }
        }
        public static void SendToPlayer(PacketHandler p, int id)
        {
            int i = 0;
            while (i < player_list.Length)
            {
                if (player_list[i] != null && player_list[i].id == id)
                {
                    player_list[i].SendPacket(p, id, ref player_list[i], false, false);
                }
                i++;
            }
        }
        public static void SendToAll(PacketHandler p, int id)
        {

        }
        public static int PlayerCount()
        {
            int i = 0;
            int count = 0;
            while (i < player_list.Length)
            {
                if (player_list[i] != null)
                    count++;
                i++;
            }
            return count;
        }

        #endregion Player data networking code
        public GridServer()
        {
            Config.InitializeSettings();
            DoSetupGCMode();
            player_list = new ClientManager[(int)Config.Configuration["MaxPlayers"] + 1];
            World.LoadWorld();
            SetupModules();
        }
        public void DoSetupGCMode()
        {
            switch ((string)Config.Configuration["GCMode"])
            {
                case "Batch":
                    System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;
                    break;
                case "Interactive":
                    System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Interactive;
                    break;
                case "LowLatency":
                    System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;
                    break;
            }
        }
        public void SetupModules()
        {
            #region Module Container Loading
            List<ComposablePartCatalog> compose = new List<ComposablePartCatalog>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(assembly);
            DirectoryCatalog directoryCatalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "LibOpenCraft.*.dll");
            #region Make Catalog
            compose.Add(assemblyCatalog);
            compose.Add(directoryCatalog);
            AggregateCatalog catalog = new AggregateCatalog(compose);
            #endregion Make Catalog
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
            ModuleHandler.StartCoreModules();
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
