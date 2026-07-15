using System.Collections.Generic;
using System.Linq;
using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Match;

public struct RespawnComponent : IComponent
{
    public float Timer;
    public int TeamId;
}

public sealed class AutoRespawnSystem : ISystem
{
    private readonly float _respawnDelay;

    public AutoRespawnSystem(float respawnDelay)
    {
        _respawnDelay = respawnDelay;
    }

    public void OnTick(World world, float deltaTime)
    {
        var deadEntities = new List<EntityId>();
        var respawnedEntities = new List<EntityId>();

        foreach (var id in world.GetEntityIds<HealthComponent>())
        {
            if (!world.TryGetComponent<HealthComponent>(id, out var health) ||
                !world.TryGetComponent<TeamComponent>(id, out var team))
                continue;

            if (health.Current <= 0f)
            {
                var respawns = world.GetEntityIds<RespawnComponent>();
                if (respawns.Contains(id))
                    continue;

                world.AddComponent(new Entity(id), new RespawnComponent
                {
                    Timer = _respawnDelay,
                    TeamId = team.TeamId
                });
                continue;
            }
        }

        foreach (var id in world.GetEntityIds<RespawnComponent>())
        {
            ref var respawn = ref world.GetComponent<RespawnComponent>(id);

            respawn.Timer -= deltaTime;
            if (respawn.Timer <= 0f)
            {
                world.RemoveComponent<RespawnComponent>(id);

                float baseX = respawn.TeamId == 0 ? 3f : 57f;
                float baseZ = 3f;

                ref var pos = ref world.GetComponent<PositionComponent>(id);
                pos.Value = new EvoVector3(baseX, 0, baseZ);

                ref var health = ref world.GetComponent<HealthComponent>(id);
                health.Current = health.Max;

                respawnedEntities.Add(id);
            }
        }
    }
}
