namespace Evo.Gameplay.Systems;

public static class TargetSystem
{
    public static GameEntity? GetNearestEnemy(GameEntity seeker, IEnumerable<GameEntity> targets, float maxRange)
    {
        if (!seeker.TryGetComponent<TransformComponent>(out var seekerPos))
            return null;

        GameEntity? nearest = null;
        var nearestDistSq = float.MaxValue;
        var rangeSq = maxRange * maxRange;

        foreach (var target in targets)
        {
            if (target.TeamId == seeker.TeamId)
                continue;

            if (!IsAlive(target))
                continue;

            if (!target.TryGetComponent<TransformComponent>(out var targetPos))
                continue;

            var dx = targetPos.X - seekerPos.X;
            var dz = targetPos.Z - seekerPos.Z;
            var distSq = dx * dx + dz * dz;

            if (distSq > rangeSq || distSq >= nearestDistSq)
                continue;

            nearest = target;
            nearestDistSq = distSq;
        }

        return nearest;
    }

    private static bool IsAlive(GameEntity entity)
    {
        if (!entity.TryGetComponent<HealthComponent>(out var health))
            return true;

        return health.CurrentHealth > 0f;
    }
}
