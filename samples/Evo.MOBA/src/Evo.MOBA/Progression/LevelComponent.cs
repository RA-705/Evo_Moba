using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;

namespace Evo.MOBA.Progression;

public struct LevelComponent : IComponent
{
    public int Level;
    public float CurrentXp;
    public float XpToNext;
}

public struct XpGrantComponent : IComponent
{
    public float Reward;
}

public sealed class XpSystem : ISystem
{
    private const float BaseXpToLevel = 100f;
    private const float XpScaling = 1.5f;

    public void OnTick(World world, float deltaTime)
    {
        foreach (var deadId in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<HealthComponent>(deadId, out var health))
               continue;

            if (health.Current > 0f) continue;
            if (!world.TryGetComponent<TeamComponent>(deadId, out var deadTeam)) continue;
            if (!world.TryGetComponent<XpGrantComponent>(deadId, out var xpGrant)) continue;

            foreach (var allyId in world.GetEntityIds<LevelComponent>())
            {
                if (!world.TryGetComponent<TeamComponent>(allyId, out var allyTeam)) continue;
                if (allyTeam.TeamId != deadTeam.TeamId) continue;

                ref var level = ref world.GetComponent<LevelComponent>(allyId);
                level.CurrentXp += xpGrant.Reward;

                while (level.CurrentXp >= level.XpToNext && level.Level < 18)
                {
                    level.CurrentXp -= level.XpToNext;
                    level.Level++;
                    level.XpToNext = BaseXpToLevel * MathF.Pow(XpScaling, level.Level - 1);
                    ApplyLevelUpStats(world, allyId, level.Level);
                }
            }
        }
    }

    private static void ApplyLevelUpStats(World world, EntityId id, int newLevel)
    {
        if (!world.TryGetComponent<HealthComponent>(id, out var health))
            return;

        float hpGain = 20 + newLevel * 5f;
        health.Max += hpGain;
        health.Current += hpGain;
        world.SetComponent(id, health);

        if (!world.TryGetComponent<AttackComponent>(id, out var attack))
            return;

        attack.Damage += 2 + newLevel * 0.5f;
        world.SetComponent(id, attack);
    }
}
