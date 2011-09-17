using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using zlib;

namespace LibOpenCraft
{
    public enum PacketType
    {
        KeepAlive = 0x00,
        LoginRequest = 0x01,
        Handshake = 0x02,
        ChatMessage = 0x03,
        TimeUpdate = 0x04,
        EntityEquipment = 0x05,
        SpwanPosition = 0x06,
        UseEntity = 0x07,
        UpdateHealth = 0x08,
        Respawn = 0x09,
        Player = 0x0A,
        PlayerPosition = 0x0B,
        PlayerLook = 0x0C,
        PlayerPositionLook = 0x0D,
        PlayerDigging = 0x0E,
        PlayerBlockPlacement = 0x0F,
        HoldingChange = 0x10,
        UseBed = 0x11,
        Animation = 0x12,
        EntityAction = 0x13,
        NamedEntitySpawn = 0x14,
        PickupSpawn = 0x15,
        CollectItem = 0x16,
        AddObject_Vehicle = 0x17,
        MobSpawn = 0x18,
        EntityPainting = 0x19,
        ExperienceOrb = 0x1A,
        StanceUpdate = 0x1B, // not used i dont think.
        EntityVelocity = 0x1C,
        DestroyEntity = 0x1D, // when player disconnects.
        Entity = 0x1E,
        EntityRelativeMove = 0x1F, // when a player moves or entity.
        EntityLook = 0x20,
        EntityLookRelativeMove = 0x21, //when a player moves and looks
        EntityTeleport = 0x22, // when a player teleports usually with /tp [from name] [to name]
        EntityStatus = 0x26,
        AttachEntity = 0x27,
        EntityMetadata = 0x28,
        EntityEffect = 0x29,
        RemoveEntityEffect = 0x2A,
        Experience = 0x2B,
        PreChunk = 0x32,
        MapChunk = 0x33,
        MultiBlockChange = 0x34,
        BlockChange = 0x35,
        BlockAction = 0x36,
        Explosion = 0x3C,
        SoundEffect = 0x3D,
        New_InvalidState = 0x46,
        Thunderbolt = 0x47,
        OpenWindow = 0x64,
        CloseWindow = 0x65,
        SetSlot = 0x67,
        WindowItems = 0x68,
        UpdateProgressBar = 0x69,
        Transaction = 0x6A,
        CreativeInventoryAction = 0x6B,
        UpdateSign = 0x82,
        ItemData = 0x83,
        IncrementStatistic = 0xC8,
        PlayerListItem = 0xC9,
        ServerListPing = 0xFE,
        ServerListPingBack = 0xFF,
        Disconnect_Kick = 0xFF,
    };
}
