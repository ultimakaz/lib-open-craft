using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.MajongProtocol
{
    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "Animation")]
    class Animation : CoreEventModule
    {
        string name = "";
        public Animation()
            : base(PacketType.Animation)
        {

        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.Animation, new ModuleCallback(OnAnimation));
            base.RunModuleCache();
        }

        public void OnAnimation(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            AnimationPacket p = new AnimationPacket(PacketType.Animation);
            p.EntityID = _pReader.ReadInt();
            p.Animation = _pReader.ReadByte();
            p.BuildPacket();
            int index_me = Chunk.GetIndex((int)_client._player.position.X, (int)_client._player.position.Y, (int)_client._player.position.Z);
            ClientManager[] player = GridServer.player_list;
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i] == null)
                {

                }
                else
                {
                    int index_remote = Chunk.GetIndex((int)player[i]._player.position.X, (int)player[i]._player.position.Y, (int)player[i]._player.position.Z);
                    if (index_remote - 5 < index_me && index_remote + 5 > index_me && _client.id != player[i].id)
                    {
                        player[i].SendPacket(p, player[i].id, ref player[i]);
                    }
                }
            }
            _client.SendPacket(p, _client.id, ref _client);
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.Animation);
        }
    }
}
