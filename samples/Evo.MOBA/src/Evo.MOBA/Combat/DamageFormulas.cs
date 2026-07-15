using Evo.Core.ECS;

namespace Evo.MOBA.Combat;

public struct ArmorComponent : IComponent
{
    public float BaseArmor;
    public float BonusArmor;
    public float TotalArmor => BaseArmor + BonusArmor;
}

public struct MagicResistComponent : IComponent
{
    public float BaseResist;
    public float BonusResist;
    public float TotalResist => BaseResist + BonusResist;
}

public enum DamageType
{
    Physical,
    Magic,
    True,
}

public static class DamageFormulas
{
    public static float CalculatePhysicalDamage(float rawDamage, float armor)
    {
        float reduction = armor / (armor + 100f);
        return rawDamage * (1f - reduction);
    }

    public static float CalculateMagicDamage(float rawDamage, float magicResist)
    {
        float reduction = magicResist / (magicResist + 100f);
        return rawDamage * (1f - reduction);
    }

    public static float ApplyDamage(World world, EntityId target, float rawDamage, DamageType damageType)
    {
        return ApplyDamage(world, target, Entity.Null.Id, rawDamage, damageType);
    }

    public static float ApplyDamage(World world, EntityId target, EntityId source, float rawDamage, DamageType damageType)
    {
        float finalDamage = damageType switch
        {
            DamageType.Physical when world.TryGetComponent<ArmorComponent>(target, out var armor)
                => CalculatePhysicalDamage(rawDamage, armor.TotalArmor),
            DamageType.Magic when world.TryGetComponent<MagicResistComponent>(target, out var mr)
                => CalculateMagicDamage(rawDamage, mr.TotalResist),
            _ => rawDamage,
        };

        if (world.TryGetComponent<HealthComponent>(target, out var health))
        {
            health.Current -= finalDamage;
            world.SetComponent(target, health);
        }

        var evt = world.Create();
        world.AddComponent(evt, new DamageDealtEventComponent
        {
            SourceId = source,
            TargetId = target,
            RawDamage = rawDamage,
            FinalDamage = finalDamage,
            DamageType = damageType,
        });

        return finalDamage;
    }
}
