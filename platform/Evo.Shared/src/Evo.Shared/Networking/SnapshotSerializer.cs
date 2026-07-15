namespace Evo.Shared.Networking;

public static class SnapshotSerializer
{
    public static byte[] Serialize(in WorldSnapshot snapshot)
    {
        using var ms = new MemoryStream(8192);
        using var writer = new BinaryWriter(ms);

        writer.Write(snapshot.TickNumber);
        writer.Write(snapshot.EntityCount);

        var entities = snapshot.Entities;
        for (var i = 0; i < snapshot.EntityCount; i++)
        {
            var e = entities[i];
            writer.Write(e.EntityId);
            writer.Write(e.PosX);
            writer.Write(e.PosY);
            writer.Write(e.PosZ);
            writer.Write(e.CurrentHealth);
            writer.Write(e.TeamId);
            writer.Write(e.StateFlags);
        }

        return ms.ToArray();
    }

    public static WorldSnapshot Deserialize(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);

        var snapshot = new WorldSnapshot();
        snapshot.TickNumber = reader.ReadUInt32();
        snapshot.EntityCount = reader.ReadInt32();

        var entities = snapshot.Entities;
        for (var i = 0; i < snapshot.EntityCount; i++)
        {
            entities[i] = new EntitySnapshot
            {
                EntityId = reader.ReadInt32(),
                PosX = reader.ReadSingle(),
                PosY = reader.ReadSingle(),
                PosZ = reader.ReadSingle(),
                CurrentHealth = reader.ReadSingle(),
                TeamId = reader.ReadInt32(),
                StateFlags = reader.ReadUInt32(),
            };
        }

        return snapshot;
    }
}
