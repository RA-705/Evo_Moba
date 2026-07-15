using LiteNetLib.Utils;

namespace Evo.Client
{
    public struct EvoVector3 : INetSerializable
    {
        public float x, y, z;

        public EvoVector3(float x = 0, float y = 0, float z = 0)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(x);
            writer.Put(y);
            writer.Put(z);
        }

        public void Deserialize(NetDataReader reader)
        {
            x = reader.GetFloat();
            y = reader.GetFloat();
            z = reader.GetFloat();
        }

        public static implicit operator UnityEngine.Vector3(EvoVector3 v) => new UnityEngine.Vector3(v.x, v.y, v.z);
        public static implicit operator EvoVector3(UnityEngine.Vector3 v) => new EvoVector3(v.x, v.y, v.z);
    }

    public struct CastTarget : INetSerializable
    {
        public TargetType Type;
        public int EntityId;
        public EvoVector3 Position;
        public EvoVector3 Direction;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Type);
            writer.Put(EntityId);
            Position.Serialize(writer);
            Direction.Serialize(writer);
        }

        public void Deserialize(NetDataReader reader)
        {
            Type = (TargetType)reader.GetByte();
            EntityId = reader.GetInt();
            Position = new EvoVector3(); Position.Deserialize(reader);
            Direction = new EvoVector3(); Direction.Deserialize(reader);
        }
    }

    public enum TargetType : byte { None = 0, Entity = 1, Position = 2, Direction = 3 }
}
