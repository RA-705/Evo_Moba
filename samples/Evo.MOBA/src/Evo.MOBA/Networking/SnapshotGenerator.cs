using Evo.Core.ECS;
using Evo.MOBA.AI.MATE;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Networking;

namespace Evo.MOBA.Networking;

public static class SnapshotGenerator
{
    public static WorldSnapshot GenerateSnapshot(World world, uint currentTick)
    {
        var snapshot = new WorldSnapshot();
        snapshot.TickNumber = currentTick;
        snapshot.EntityCount = 0;

        var count = 0;
        var entities = snapshot.Entities;

        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<PositionComponent>(id, out var pos) ||
                !world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            var health = world.GetComponent<HealthComponent>(id);

            uint flags = 0;
            if (health.Current <= 0f)
                flags |= EntityStateFlags.Dead;
            if (world.TryGetComponent<MateBrainComponent>(id, out var brain) && brain.IsRetreating)
                flags |= EntityStateFlags.Fleeing;
            if (world.TryGetComponent<AttackComponent>(id, out var attack) && attack.CurrentTargetId != Entity.Null.Id)
                flags |= EntityStateFlags.Attacking;

            entities[count] = new EntitySnapshot
            {
                EntityId = id.Value,
                PosX = pos.Value.X,
                PosY = pos.Value.Y,
                PosZ = pos.Value.Z,
                CurrentHealth = health.Current,
                TeamId = team.TeamId,
                StateFlags = flags,
            };

            count++;
        }

        snapshot.EntityCount = count;
        return snapshot;
    }
}
