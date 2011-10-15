using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using LibOpenCraft.ServerPackets;



namespace LibOpenCraft.Bukkit.Plugin
{
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "Bukkit Plugin Loader")]
    public class BukkitLoader : CoreModule
    {
        string name = "";
        List<string> bukkitfiles;
        public BukkitLoader()
            : base()
        {
            bukkitfiles = new List<string>();
            string[] files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            foreach (string str in files)
            {
                if (str.Contains(".jar"))
                {
                    System.Diagnostics.Process.Start("ikvmc.exe -target:library " + str);
                    bukkitfiles.Add(str.Replace(".jar", ".dll"));
                }
            }

        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnBlockChange);
            base.RunModuleCache();
        }

        public PacketHandler OnBlockChange(PacketType p_type, string CustomPacketType, ref PacketReader packet_reader, PacketHandler _p, ref ClientManager cm)
        {
            return _p;
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.ChatMessage);
        }
    }
}
