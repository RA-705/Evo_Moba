using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class WorldSnapshot
{
    public int Tick { get; set; }
    public List<EntitySnapshot> Entities { get; set; } = new();
    public GameState GameState { get; set; }
    
    public static WorldSnapshot Deserialize(byte[] data)
    {
        using var reader = new BinaryReader(new MemoryStream(data));
        var snapshot = new WorldSnapshot
        {
            Tick = reader.ReadInt32()
        };
        
        int entityCount = reader.ReadInt32();
        for (int i = 0; i < entityCount; i++)
        {
            snapshot.Entities.Add(EntitySnapshot.Read(reader));
        }
        
        snapshot.GameState = GameState.Read(reader);
        
        return snapshot;
    }
}

[System.Serializable]
public class EntitySnapshot
{
    public int EntityId { get; set; }
    public string EntityType { get; set; }
    public Vector3 Position { get; set; }
    public float Rotation { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Team { get; set; }
    public Dictionary<string, object> CustomData { get; set; } = new();
    
    public static EntitySnapshot Read(BinaryReader reader)
    {
        return new EntitySnapshot
        {
            EntityId = reader.ReadInt32(),
            EntityType = reader.ReadString(),
            Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
            Rotation = reader.ReadSingle(),
            Health = reader.ReadInt32(),
            MaxHealth = reader.ReadInt32(),
            Team = reader.ReadInt32()
        };
    }
}

[System.Serializable]
public class GameState
{
    public int GameTime { get; set; }
    public int BlueTeamGold { get; set; }
    public int RedTeamGold { get; set; }
    public bool GameOver { get; set; }
    public int WinningTeam { get; set; }
    
    public static GameState Read(BinaryReader reader)
    {
        return new GameState
        {
            GameTime = reader.ReadInt32(),
            BlueTeamGold = reader.ReadInt32(),
            RedTeamGold = reader.ReadInt32(),
            GameOver = reader.ReadBoolean(),
            WinningTeam = reader.ReadInt32()
        };
    }
}
