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
    [ExportMetadata("Name", "EntityAction")]
    class EntityAction : CoreEventModule
    {
        string name = "";
        public EntityAction()
            : base(PacketType.EntityAction)
        {

        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.Animation, new ModuleCallback(OnEntityAction));
            base.RunModuleCache();
        }

        public void OnEntityAction(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            AnimationPacket p = new AnimationPacket(PacketType.EntityAction);
            p.EntityID = _pReader.ReadInt();
            p.Animation = _pReader.ReadByte();
            p.BuildPacket();

            EntityMetadataPacket e_packet = new EntityMetadataPacket(PacketType.EntityMetadata);

            /*
             * /// <summary>
    /// Metadata has a massively quirky packing format, 
    /// the details of which are not quite all understood. However, 
    /// successful parsing of metadata is possible, according to the following rules.
    /// A metadata field is a byte. The top three bits of the byte, 
    /// when masked off and interpreted as a three-bit number from 0 to 7, 
    /// indicate the type of the field. The lower five bits are some sort of unknown identifier. 
    /// The type of the byte indicates the size and type of a payload which follows the initial byte of the field.
    /// The metadata format consists of at least one field, 
    /// followed by either another field or the magic number 0x7F. 0x7F terminates a metadata stream.
    /// Thus, the example metadata stream 0x00 0x01 0x7f is decoded as a field of type 0, id 0, 
    /// with a payload of byte 0x00.
    /// </summary>
             * */
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.EntityAction);
        }
    }
}
