using Evo.Core.ECS;
using Evo.MOBA.Abilities;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.AI.MATE;

public sealed class MateAbilitySystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<MOBAComponent>())
        {
            if (!world.TryGetComponent<MOBAComponent>(id, out var moba) ||
                !world.TryGetComponent<AbilitySlotComponent>(id, out var slots) ||
                !world.TryGetComponent<AttackComponent>(id, out var attack))
                continue;

            if (attack.CurrentTargetId == Entity.Null.Id)
                continue;

            AbilityData? availableData = null;

            if (slots.Q.CurrentCooldown <= 0f && AbilityRegistry.Database.TryGetValue(slots.Q.DataId, out var qData))
                availableData = qData;
            else if (slots.W.CurrentCooldown <= 0f && AbilityRegistry.Database.TryGetValue(slots.W.DataId, out var wData))
                availableData = wData;
            else if (slots.E.CurrentCooldown <= 0f && AbilityRegistry.Database.TryGetValue(slots.E.DataId, out var eData))
                availableData = eData;
            else if (slots.R.CurrentCooldown <= 0f && AbilityRegistry.Database.TryGetValue(slots.R.DataId, out var rData))
                availableData = rData;

            if (availableData is null)
                continue;

            if (!world.TryGetComponent<PositionComponent>(id, out var casterPos) ||
                !world.TryGetComponent<PositionComponent>(attack.CurrentTargetId, out var targetPos))
                continue;

            var dist = EvoVector3.Distance(casterPos.Value, targetPos.Value);
            if (dist > availableData.Value.Range)
                continue;

            world.AddComponent(
                new Entity(id),
                new PendingCastComponent
                {
                    AbilityDataId = availableData.Value.Id,
                    Target = CastTarget.Unit(attack.CurrentTargetId),
                });
        }
    }
}
