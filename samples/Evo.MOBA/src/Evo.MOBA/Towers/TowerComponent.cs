using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Towers;

public struct TowerComponent : IComponent
{
    public float AttackRange;
    public float Damage;
    public float AttackCooldown;
    public float Timer;
    public int TeamId;
    public EntityId CurrentTarget;
}

public enum TowerTargetPriority
{
    ClosestCreep,
    LowestHpCreep,
    ClosestHero,
}

public sealed class TowerAttackSystem : ISystem
{
    private const float Range = 10f;

    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<TowerComponent>())
        {
            ref var tower = ref world.GetComponent<TowerComponent>(id);
            tower.Timer -= deltaTime;

            if (!world.TryGetComponent<PositionComponent>(id, out var towerPos))
                continue;

            if (tower.CurrentTarget is null ||
                tower.CurrentTarget == Entity.Null.Id ||
                !TargetStillValid(world, tower.CurrentTarget, tower.TeamId, towerPos.Value))
            {
                tower.CurrentTarget = AcquireTarget(world, tower.TeamId, towerPos.Value);
            }

            if (tower.CurrentTarget is not null &&
                tower.CurrentTarget != Entity.Null.Id &&
                tower.Timer <= 0f)
            {
                DamageFormulas.ApplyDamage(world, tower.CurrentTarget, tower.Damage, DamageType.Physical);
                tower.Timer = tower.AttackCooldown;
            }
        }
    }

    private static bool TargetStillValid(World world, EntityId target, int towerTeam, EvoVector3 towerPos)
    {
        if (target is null) return false;
        if (!world.TryGetComponent<HealthComponent>(target, out var health) || health.Current <= 0f)
            return false;

        if (!world.TryGetComponent<TeamComponent>(target, out var team) || team.TeamId == towerTeam)
            return false;

        if (!world.TryGetComponent<PositionComponent>(target, out var pos))
            return false;

        return EvoVector3.Distance(towerPos, pos.Value) <= Range;
    }

    private static EntityId AcquireTarget(World world, int towerTeam, EvoVector3 towerPos)
    {
        EntityId bestTarget = Entity.Null.Id;
        float bestScore = float.MaxValue;

        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<TeamComponent>(id, out var team) || team.TeamId == towerTeam)
                continue;

            if (!world.TryGetComponent<HealthComponent>(id, out var health) || health.Current <= 0f)
                continue;

            if (!world.TryGetComponent<PositionComponent>(id, out var pos))
                continue;

            var dist = EvoVector3.Distance(towerPos, pos.Value);
            if (dist > Range) continue;

            if (dist < bestScore)
            {
                bestScore = dist;
                bestTarget = id;
            }
        }

        return bestTarget;
    }
}

public sealed class TowerFactory
{
    public static Entity CreateTower(World world, int teamId, EvoVector3 position)
    {
        var entity = world.Create();

        world.AddComponent(entity, new PositionComponent { Value = position });
        world.AddComponent(entity, new TeamComponent { TeamId = teamId });
        world.AddComponent(entity, new HealthComponent { Current = 500, Max = 500 });
        world.AddComponent(entity, new ArmorComponent { BaseArmor = 20 });
        world.AddComponent(entity, new TowerComponent
        {
            AttackRange = 10f,
            Damage = 25,
            AttackCooldown = 1.5f,
            Timer = 0f,
            TeamId = teamId,
            CurrentTarget = Entity.Null.Id,
        });

        return entity;
    }
}
