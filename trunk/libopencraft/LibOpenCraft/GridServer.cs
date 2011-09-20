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

        public static List<Chunk> chunk_b = new List<Chunk>();
        public static Chunk[] chunks;
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
            player_list = new ClientManager[(int)Config.Configuration["MaxPlayers"] + 1];
            World.LoadWorld();
            SetupModules();
        }
        public void SetupModules()
        {
            #region Module Container Loading

            AggregateCatalog catalog = new AggregateCatalog();

            AssemblyCatalog assemblyCatalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            DirectoryCatalog directoryCatalog = new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "LibOpenCraft.*.dll");

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
