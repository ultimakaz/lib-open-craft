using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.Chat
{
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "LocalChat")]
    public class LocalChat : CoreModule
    {
        string name = "";
        public LocalChat()
            : base()
        {
           
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.ChatMessage, new ModuleCallback(OnChatMessage));
            base.RunModuleCache();
        }

        public void OnChatMessage(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            string message = _pReader.ReadString();
            ClientManager[] player = GridServer.player_list;
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i] == null)
                {

                }
                else
                {
                    if (player[i] == null)
                    {

                    }
                    else
                    {
                        PacketHandler ChatMessage = new PacketHandler(PacketType.ChatMessage);
                        ChatMessage.AddString(_client._player.name + ": " + message);
                        player[i].SendPacket(ChatMessage, player[i].id, ref player[i], false, false);
                    }
                }
            }
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.ChatMessage);
        }

    }
}
