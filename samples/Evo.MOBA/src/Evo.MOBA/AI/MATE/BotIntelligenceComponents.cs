using System;
using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;

namespace Evo.MOBA.AI.MATE;

public enum BotEmotion : byte
{
    Neutral,
    Confident,
    Frustrated,
    Desperate,
    Tilted,
    Calm,
}

public enum BotPersonalityType : byte
{
    Aggressive,
    Cautious,
    Strategic,
    Troll,
    Supportive,
    Assassin,
}

public struct BotIntelligenceComponent : IComponent
{
    public BotPersonalityType Personality;
    public BotEmotion CurrentEmotion;
    public float EmotionIntensity;
    public float LastKillTime;
    public float LastDeathTime;
    public int ConsecutiveKills;
    public int ConsecutiveDeaths;
    public float AggressionModifier;
    public float CautionModifier;
    public float ConfidenceModifier;
}

public struct PlayerPatternTracker : IComponent
{
    public Dictionary<int, PlayerPattern> Patterns;
}

public struct PlayerPattern
{
    public int PlayerId;
    public float RetreatHpThreshold;
    public float AggressionLevel;
    public float PreferredLane;
    public float ReactionTime;
    public int TimesKilledByBot;
    public int TimesKilledBot;
    public float LastSeenTime;
    public float[] PositionHistory;
}

public struct MetaAdaptationComponent : IComponent
{
    public int EnemyPhysicalDamageCount;
    public int EnemyMagicDamageCount;
    public int EnemyTankCount;
    public int EnemyAssassinCount;
    public float PhysicalDefensePriority;
    public float MagicDefensePriority;
    public float LastMetaAnalysisTime;
}

public struct TeamCoordinationComponent : IComponent
{
    public int AssignedRole;
    public int CoordinatedWithId;
    public float LastCoordinationTime;
    public bool IsFlanking;
    public bool IsCovering;
    public float CoordinationCooldown;
}
