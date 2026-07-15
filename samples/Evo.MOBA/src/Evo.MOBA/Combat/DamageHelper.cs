using Evo.Core.ECS;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Combat;

public static class DamageHelper
{
    public static void ApplyAoE(World world, EvoVector3 center, float radius, float damage, int casterTeamId, EntityId casterId)
    {
        foreach (var targetId in world.GetEntityIds<HealthComponent>())
        {
            if (targetId == casterId)
                continue;

            if (!world.TryGetComponent<TeamComponent>(targetId, out var team) ||
                team.TeamId == casterTeamId)
                continue;

            if (!world.TryGetComponent<PositionComponent>(targetId, out var pos))
                continue;

            if (EvoVector3.Distance(center, pos.Value) > radius)
                continue;

            DamageFormulas.ApplyDamage(world, targetId, casterId, damage, DamageType.Magic);
        }
    }
}
