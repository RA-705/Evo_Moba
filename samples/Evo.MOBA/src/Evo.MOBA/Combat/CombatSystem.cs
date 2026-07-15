using Evo.Core.ECS;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Combat;

public struct HealthComponent : IComponent
{
    public float Current;
    public float Max;
}

public struct ManaComponent : IComponent
{
    public float Current;
    public float Max;
}

public struct TeamComponent : IComponent
{
    public int TeamId;
}

public struct AttackComponent : IComponent
{
    public float Damage;
    public float Range;
    public float CooldownTime;
    public float TimeUntilNextAttack;
    public EntityId CurrentTargetId;
}

public sealed class MeleeAttackSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<AttackComponent>())
        {
            if (!world.TryGetComponent<PositionComponent>(id, out _) ||
                !world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            ref var attack = ref world.GetComponent<AttackComponent>(id);
            attack.TimeUntilNextAttack -= deltaTime;

            if (attack.CurrentTargetId == Entity.Null.Id)
                continue;

            if (!world.TryGetComponent<HealthComponent>(attack.CurrentTargetId, out var targetHealth))
            {
                attack.CurrentTargetId = Entity.Null.Id;
                continue;
            }

            if (!world.TryGetComponent<TeamComponent>(attack.CurrentTargetId, out var targetTeam) ||
                targetTeam.TeamId == team.TeamId)
                continue;

            if (!world.TryGetComponent<PositionComponent>(attack.CurrentTargetId, out var targetPos))
                continue;

            var distance = EvoVector3.Distance(
                world.GetComponent<PositionComponent>(id).Value, targetPos.Value);

            if (distance <= attack.Range && attack.TimeUntilNextAttack <= 0f)
            {
                DamageFormulas.ApplyDamage(world, attack.CurrentTargetId, attack.Damage, DamageType.Physical);
                attack.TimeUntilNextAttack = attack.CooldownTime;
            }
        }
    }
}
