using System.Collections.Generic;
using Evo.Core.ECS;

namespace Evo.MOBA.AI.MATE;

public enum MOBAEntityType : byte
{
    Hero,
    Creep,
    Tower,
    Nexus,
    Ward,
}

public readonly struct VisibleEntity
{
    public int EntityId { get; }
    public MOBAEntityType Type { get; }
    public int TeamId { get; }
    public float DistanceSq { get; }
    public float Health { get; }
    public float MaxHealth { get; }
    public float PosX { get; }
    public float PosZ { get; }

    public VisibleEntity(int entityId, MOBAEntityType type, int teamId, float distanceSq,
        float health, float maxHealth, float posX, float posZ)
    {
        EntityId = entityId;
        Type = type;
        TeamId = teamId;
        DistanceSq = distanceSq;
        Health = health;
        MaxHealth = maxHealth;
        PosX = posX;
        PosZ = posZ;
    }

    public float HealthRatio => MaxHealth > 0 ? Health / MaxHealth : 0f;
}

public readonly struct MOBAPerception
{
    public int SelfId { get; }
    public int SelfTeam { get; }
    public float SelfHP { get; }
    public float SelfMaxHP { get; }
    public float SelfMana { get; }
    public float SelfMaxMana { get; }
    public float SelfPosX { get; }
    public float SelfPosZ { get; }
    public float GameTime { get; }
    public bool IsRetreating { get; }
    public BotEmotion Emotion { get; }
    public float EmotionIntensity { get; }
    public float AggressionModifier { get; }
    public float CautionModifier { get; }
    public float ConfidenceModifier { get; }

    public List<VisibleEntity> VisibleEnemies { get; }
    public List<VisibleEntity> VisibleAllies { get; }

    public MOBAPerception(
        int selfId, int selfTeam, float selfHP, float selfMaxHP,
        float selfMana, float selfMaxMana, float selfPosX, float selfPosZ,
        float gameTime, bool isRetreating,
        BotEmotion emotion, float emotionIntensity,
        float aggressionModifier, float cautionModifier, float confidenceModifier,
        List<VisibleEntity> visibleEnemies, List<VisibleEntity> visibleAllies)
    {
        SelfId = selfId;
        SelfTeam = selfTeam;
        SelfHP = selfHP;
        SelfMaxHP = selfMaxHP;
        SelfMana = selfMana;
        SelfMaxMana = selfMaxMana;
        SelfPosX = selfPosX;
        SelfPosZ = selfPosZ;
        GameTime = gameTime;
        IsRetreating = isRetreating;
        Emotion = emotion;
        EmotionIntensity = emotionIntensity;
        AggressionModifier = aggressionModifier;
        CautionModifier = cautionModifier;
        ConfidenceModifier = confidenceModifier;
        VisibleEnemies = visibleEnemies;
        VisibleAllies = visibleAllies;
    }

    public float SelfHealthRatio => SelfMaxHP > 0 ? SelfHP / SelfMaxHP : 0f;
    public float SelfManaRatio => SelfMaxMana > 0 ? SelfMana / SelfMaxMana : 0f;

    public bool HasEnemyInRange(float rangeSq) =>
        VisibleEnemies.Exists(e => e.DistanceSq <= rangeSq);

    public VisibleEntity? GetClosestEnemy()
    {
        VisibleEntity? closest = null;
        float closestDist = float.MaxValue;
        foreach (var e in VisibleEnemies)
        {
            if (e.DistanceSq < closestDist)
            {
                closestDist = e.DistanceSq;
                closest = e;
            }
        }
        return closest;
    }

    public VisibleEntity? GetWeakestEnemy()
    {
        VisibleEntity? weakest = null;
        float lowestHP = float.MaxValue;
        foreach (var e in VisibleEnemies)
        {
            if (e.Health < lowestHP)
            {
                lowestHP = e.Health;
                weakest = e;
            }
        }
        return weakest;
    }

    public int CountEnemiesInRange(float rangeSq)
    {
        int count = 0;
        foreach (var e in VisibleEnemies)
            if (e.DistanceSq <= rangeSq) count++;
        return count;
    }

    public int CountAlliesInRange(float rangeSq)
    {
        int count = 0;
        foreach (var a in VisibleAllies)
            if (a.DistanceSq <= rangeSq) count++;
        return count;
    }

    public bool IsOutnumbered()
    {
        int enemyCount = VisibleEnemies.Count;
        int allyCount = VisibleAllies.Count + 1;
        return enemyCount > allyCount;
    }
}
