using Evo.Core.ECS;

namespace Evo.MOBA.Abilities;

public enum TargetType { Self, Unit, Point, Direction }

public enum CastType { Instant, Channelled }

public readonly struct AbilityData
{
    public int Id { get; init; }
    public string Name { get; init; }
    public TargetType Target { get; init; }
    public CastType CastType { get; init; }
    public float Range { get; init; }
    public float CooldownTime { get; init; }
    public float CastTime { get; init; }
    public float Damage { get; init; }
    public float Radius { get; init; }
    
    // Extended fields from SkillDescriptionData
    public int HeroId { get; init; }
    public int SkillType { get; init; } // 0=passive, 1=active, etc.
    public int Effect { get; init; }
    public float ManaCost { get; init; }
    public string Description { get; init; }
    
    // Damage scaling
    public float PhysicalScaling { get; init; }
    public float MagicScaling { get; init; }
    public float BaseDamage { get; init; }
    public float SecondDamage { get; init; }
    
    // Crowd Control
    public float StunDuration { get; init; }
    public float SlowDuration { get; init; }
    public float SlowAmount { get; init; }
    public float KnockupDuration { get; init; }
    public float SilenceDuration { get; init; }
    
    // Buffs/Debuffs
    public float ShieldAmount { get; init; }
    public float HealAmount { get; init; }
    public float MoveSpeedBuff { get; init; }
    public float AttackSpeedBuff { get; init; }
    public float ArmorBuff { get; init; }
    public float MagicResistBuff { get; init; }
    
    // Area of Effect
    public bool IsAoE { get; init; }
    public bool IsProjectile { get; init; }
    public float ProjectileSpeed { get; init; }
}

public readonly struct AbilitySlot
{
    public int DataId { get; init; }
    public float CurrentCooldown { get; init; }
}

public struct AbilitySlotComponent : IComponent
{
    public AbilitySlot Q;
    public AbilitySlot W;
    public AbilitySlot E;
    public AbilitySlot R;

    public bool TryGetSlot(int dataId, ref AbilitySlot slot)
    {
        if (Q.DataId == dataId) { slot = Q; return true; }
        if (W.DataId == dataId) { slot = W; return true; }
        if (E.DataId == dataId) { slot = E; return true; }
        if (R.DataId == dataId) { slot = R; return true; }
        return false;
    }
}
