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
            ChatMessagePacket ChatMessage = new ChatMessagePacket(PacketType.ChatMessage);
            ChatMessage.MessageRecieved = message;
            ChatMessage.MessageSent = "";
            if (message[0] == '/' && (bool)Config.Configuration["EnableEmbeddedChatCommands"])
            {
                string command = message.Substring(1, message.Length - 1).ToLower();
                switch (command)
                {
                    case "save":
                        ChatMessage.MessageSent = _client._player.name + ": " + "Saving World....";
                        World.SaveWorld();
                        ChatMessage.MessageSent = _client._player.name + ": " + "World Saved!";
                        break;
                    case "set -b 1":
                        break;
                    case "set -b 2":
                        break;
                    case "set -b 3":
                        break;
                    case "set -b 4":
                        break;
                    default:
                        ChatMessage.MessageSent = "There is no such command please try again!";
                        break;
                }

                string[] data = command.Split((" ").ToCharArray());
                // command.Split((" ").ToCharArray());
                switch (data[0])
                {
                    case "tp":
                        bool UniqueUserNames = true;

                        PlayerClass player1 = null, player2 = null;

                        foreach (ClientManager _user1 in GridServer.player_list)
                        {
                            if ((_user1 != null) && (_user1._player.name.ToLower().StartsWith(data[1])))
                            {
                                if (player1 == null)
                                {
                                    player1 = _user1._player;
                                }
                                else
                                {
                                    UniqueUserNames = false;
                                    break;
                                }
                            }
                            if ((_user1 != null) && (_user1._player.name.ToLower().StartsWith(data[2])))
                            {
                                if (player2 == null)
                                {
                                    player2 = _user1._player;
                                }
                                else
                                {
                                    UniqueUserNames = false;
                                    break;
                                }
                            }
                        }

                        if (player1 != null & player2 != null & UniqueUserNames == true)
                        {
                            player2.position = player1.position;

                            PlayerPositionAndLookPacket tp_packet = new PlayerPositionAndLookPacket(PacketType.PlayerPositionLook);

                            tp_packet.OnGround = player2.onGround;
                            tp_packet.Pitch = player2.Pitch;
                            tp_packet.Stance = player2.stance;
                            tp_packet.X = player2.position.X;
                            tp_packet.Y = player2.position.Y;
                            tp_packet.Z = player2.position.Z;
                            tp_packet.Yaw = player2.Yaw;

                            tp_packet.BuildPacket();

                            _client.SendPacket((PacketHandler)tp_packet, _client.id, ref _client, false, false);
                            ChatMessage.MessageSent = player1.name + " teleported to " + player2.name;

                        }
                        else
                        {
                            ChatMessage.MessageSent = _client._player.name + ": " + "Cannot find user/wrong syntax";
                            GridServer.SendToPlayer((PacketHandler)ChatMessage, _client.id);                
                        }
                        break;

                    case "kick":
                        foreach (ClientManager _user1 in GridServer.player_list)
                        {
                            if ((_user1 != null) && (_user1._player.name.ToLower().StartsWith(data[1])) && _user1._player.Rank <= RankLevel.Moderator)
                            {
                                DisconnectKick dk_packet = new DisconnectKick(PacketType.Disconnect_Kick);
                                dk_packet.Reason = "You have been kicked by " + _client._player.name + ", bitch!";
                                dk_packet.BuildPacket();
                                ClientManager test = _user1;
                                _client.SendPacket((PacketHandler)dk_packet, _user1.id, ref test, false, false);
                            }
                        }
                       break;
                    default:
                       //ChatMessage.MessageSent = "There is no such command please try again!";
                       break;

                }

                try
                {
                    int i = 0;
                    for (; i < base.ModuleAddons.Count; i++)
                    {
                        ChatMessage = (ChatMessagePacket)base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)ChatMessage, ref _client);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source + " Method:" + e.TargetSite + " Data:" + e.Data);
                }
                if (ChatMessage.MessageSent == null || ChatMessage.MessageSent == "")
                {
                    ChatMessage.BuildPacket();
                    _client.SendPacket((PacketHandler)ChatMessage, _client.id, ref _client, false, false);
                }
                else
                {
                    ChatMessage.BuildPacket();
                    _client.SendPacket((PacketHandler)ChatMessage, _client.id, ref _client, false, false);
                }
            }
            else
            {
                ChatMessage.MessageSent = _client._player.name + ": " + message;
                ChatMessage.BuildPacket();
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
                            player[i].SendPacket((PacketHandler)ChatMessage, player[i].id, ref player[i], false, false);
                        }
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
