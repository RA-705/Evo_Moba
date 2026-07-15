using UnityEngine;
using LiteNetLib.Utils;
using Evo.Client;

namespace Evo.Client.ClientPackets
{
    public class HeroSelectRequest : INetSerializable
    {
        public int HeroId { get; set; }
        public int SkinId { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(HeroId);
            writer.Put(SkinId);
        }

        public void Deserialize(NetDataReader reader)
        {
            HeroId = reader.GetInt();
            SkinId = reader.GetInt();
        }
    }

    public class StartMatchRequest : INetSerializable
    {
        public void Serialize(NetDataWriter writer) { }
        public void Deserialize(NetDataReader reader) { }
    }

    public class PlayerInput : INetSerializable
    {
        public int Tick;
        public float MoveDirX, MoveDirY, MoveDirZ;
        public float LookDirX, LookDirY, LookDirZ;
        public byte ActionFlags;
        public int TargetEntityId;
        public float TargetPosX, TargetPosY, TargetPosZ;
        public int AbilityId;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Tick);
            writer.Put(MoveDirX); writer.Put(MoveDirY); writer.Put(MoveDirZ);
            writer.Put(LookDirX); writer.Put(LookDirY); writer.Put(LookDirZ);
            writer.Put(ActionFlags);
            writer.Put(TargetEntityId);
            writer.Put(TargetPosX); writer.Put(TargetPosY); writer.Put(TargetPosZ);
            writer.Put(AbilityId);
        }

        public void Deserialize(NetDataReader reader)
        {
            Tick = reader.GetInt();
            MoveDirX = reader.GetFloat(); MoveDirY = reader.GetFloat(); MoveDirZ = reader.GetFloat();
            LookDirX = reader.GetFloat(); LookDirY = reader.GetFloat(); LookDirZ = reader.GetFloat();
            ActionFlags = reader.GetByte();
            TargetEntityId = reader.GetInt();
            TargetPosX = reader.GetFloat(); TargetPosY = reader.GetFloat(); TargetPosZ = reader.GetFloat();
            AbilityId = reader.GetInt();
        }
    }

    public class CastAbility : INetSerializable
    {
        public int AbilityId { get; set; }
        public CastTarget Target { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(AbilityId);
            Target.Serialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            AbilityId = reader.GetInt();
            Target = new CastTarget(); Target.Deserialize(reader);
        }
    }

    public class ChatMessage : INetSerializable
    {
        public string Message { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Message);
        }

        public void Deserialize(NetDataReader reader)
        {
            Message = reader.GetString();
        }
    }

    public class PingRequest : INetSerializable
    {
        public long ClientTime { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ClientTime);
        }

        public void Deserialize(NetDataReader reader)
        {
            ClientTime = reader.GetLong();
        }
    }

    public enum PacketType : byte
    {
        StartMatchRequest = 1,
        HeroSelectRequest = 2,
        PlayerInput = 3,
        ChatMessage = 4,
        PingRequest = 5,
        
        MatchStateUpdate = 100,
        EntitySnapshot = 101,
        PlayerAssigned = 102,
        MatchStart = 103,
        MatchEnd = 104,
        AbilityCast = 105,
        DamageDealt = 106,
        ServerChatMessage = 107,
        PingResponse = 108
    }
}