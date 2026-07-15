using System;
using LiteNetLib.Utils;
using Evo.Client;

namespace Evo.Client.ServerPackets
{
    public class MatchStateUpdate : INetSerializable
    {
        public int State { get; set; }
        public int RemainingTime { get; set; }
        public int BlueScore { get; set; }
        public int RedScore { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Client.ClientPackets.PacketType.MatchStateUpdate);
            writer.Put(State);
            writer.Put(RemainingTime);
            writer.Put(BlueScore);
            writer.Put(RedScore);
        }

        public void Deserialize(NetDataReader reader)
        {
            State = reader.GetInt();
            RemainingTime = reader.GetInt();
            BlueScore = reader.GetInt();
            RedScore = reader.GetInt();
        }
    }

    public class EntitySnapshot : INetSerializable
    {
        public int EntityId { get; set; }
        public EvoVector3 Position { get; set; }
        public float Rotation { get; set; } // Y-axis only
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public float Mana { get; set; }
        public float MaxMana { get; set; }
        public bool IsAlive { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
        public int Experience { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Client.ClientPackets.PacketType.EntitySnapshot);
            writer.Put(EntityId);
            Position.Serialize(writer);
            writer.Put(Rotation);
            writer.Put(Health);
            writer.Put(MaxHealth);
            writer.Put(Mana);
            writer.Put(MaxMana);
            writer.Put(IsAlive);
            writer.Put(Level);
            writer.Put(Gold);
            writer.Put(Experience);
        }

        public void Deserialize(NetDataReader reader)
        {
            EntityId = reader.GetInt();
            Position = new EvoVector3(); Position.Deserialize(reader);
            Rotation = reader.GetFloat();
            Health = reader.GetFloat();
            MaxHealth = reader.GetFloat();
            Mana = reader.GetFloat();
            MaxMana = reader.GetFloat();
            IsAlive = reader.GetBool();
            Level = reader.GetInt();
            Gold = reader.GetInt();
            Experience = reader.GetInt();
        }
    }

    public class PlayerAssigned : INetSerializable
    {
        public int PlayerId { get; set; }
        public int HeroId { get; set; }
        public int TeamId { get; set; }
        public int SkinId { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Client.ClientPackets.PacketType.PlayerAssigned);
            writer.Put(PlayerId);
            writer.Put(HeroId);
            writer.Put(TeamId);
            writer.Put(SkinId);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetInt();
            HeroId = reader.GetInt();
            TeamId = reader.GetInt();
            SkinId = reader.GetInt();
        }
    }

    public class MatchStart : INetSerializable
    {
        public int MatchId { get; set; }
        public int MapId { get; set; }
        public int[] BlueTeamHeroes { get; set; }
        public int[] RedTeamHeroes { get; set; }
        public int TickRate { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Client.ClientPackets.PacketType.MatchStart);
            writer.Put(MatchId);
            writer.Put(MapId);
            writer.PutArray(BlueTeamHeroes);
            writer.PutArray(RedTeamHeroes);
            writer.Put(TickRate);
        }

        public void Deserialize(NetDataReader reader)
        {
            MatchId = reader.GetInt();
            MapId = reader.GetInt();
            BlueTeamHeroes = reader.GetIntArray();
            RedTeamHeroes = reader.GetIntArray();
            TickRate = reader.GetInt();
        }
    }

    public class MatchEnd : INetSerializable
    {
        public int WinningTeam { get; set; } // 0 = Blue, 1 = Red
        public int BlueScore { get; set; }
        public int RedScore { get; set; }
        public int DurationSeconds { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Client.ClientPackets.PacketType.MatchEnd);
            writer.Put(WinningTeam);
            writer.Put(BlueScore);
            writer.Put(RedScore);
            writer.Put(DurationSeconds);
        }

        public void Deserialize(NetDataReader reader)
        {
            WinningTeam = reader.GetInt();
            BlueScore = reader.GetInt();
            RedScore = reader.GetInt();
            DurationSeconds = reader.GetInt();
        }
    }

    public class AbilityCast : INetSerializable
    {
        public int CasterEntityId { get; set; }
        public int AbilityId { get; set; }
        public CastTarget Target { get; set; }
        public int CastTick { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Client.ClientPackets.PacketType.AbilityCast);
            writer.Put(CasterEntityId);
            writer.Put(AbilityId);
            Target.Serialize(writer);
            writer.Put(CastTick);
        }

        public void Deserialize(NetDataReader reader)
        {
            CasterEntityId = reader.GetInt();
            AbilityId = reader.GetInt();
            Target = new CastTarget(); Target.Deserialize(reader);
            CastTick = reader.GetInt();
        }
    }

    public class DamageDealt : INetSerializable
    {
        public int SourceEntityId { get; set; }
        public int TargetEntityId { get; set; }
        public float Damage { get; set; }
        public bool IsCritical { get; set; }
        public DamageType DamageType { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Client.ClientPackets.PacketType.DamageDealt);
            writer.Put(SourceEntityId);
            writer.Put(TargetEntityId);
            writer.Put(Damage);
            writer.Put(IsCritical);
            writer.Put((byte)DamageType);
        }

        public void Deserialize(NetDataReader reader)
        {
            SourceEntityId = reader.GetInt();
            TargetEntityId = reader.GetInt();
            Damage = reader.GetFloat();
            IsCritical = reader.GetBool();
            DamageType = (DamageType)reader.GetByte();
        }
    }

    public enum DamageType : byte
    {
        Physical = 0,
        Magic = 1,
        True = 2,
        Heal = 3
    }

    public class ChatMessage : INetSerializable
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string Message { get; set; }
        public ChatChannel Channel { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Client.ClientPackets.PacketType.ChatMessage);
            writer.Put(PlayerId);
            writer.Put(PlayerName);
            writer.Put(Message);
            writer.Put((byte)Channel);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerId = reader.GetInt();
            PlayerName = reader.GetString();
            Message = reader.GetString();
            Channel = (ChatChannel)reader.GetByte();
        }
    }

    public enum ChatChannel : byte
    {
        All = 0,
        Team = 1,
        Whisper = 2,
        System = 3
    }
}