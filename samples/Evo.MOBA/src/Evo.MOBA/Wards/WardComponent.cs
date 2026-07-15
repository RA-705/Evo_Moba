using Evo.Core.ECS;
using Evo.MOBA.AI.MATE;
using Evo.MOBA.Combat;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.Wards;

public struct WardComponent : IComponent
{
    public int TeamId;
    public float Duration;
    public float SightRadius;
}

public enum WardType
{
    Stealth,
    Sentry,
}

public struct PlacedWardComponent : IComponent
{
    public WardType Type;
    public int TeamId;
    public float RemainingTime;
}

public sealed class WardSystem : ISystem
{
    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<PlacedWardComponent>())
        {
            ref var ward = ref world.GetComponent<PlacedWardComponent>(id);
            ward.RemainingTime -= deltaTime;

            if (ward.RemainingTime <= 0f)
            {
                world.Destroy(new Entity(id));
            }
        }
    }

    public static Entity PlaceWard(World world, int teamId, WardType type, EvoVector3 position)
    {
        float duration = type == WardType.Sentry ? 60f : 120f;
        float sightRadius = type == WardType.Sentry ? 12f : 10f;

        var entity = world.Create();
        world.AddComponent(entity, new PositionComponent { Value = position });
        world.AddComponent(entity, new TeamComponent { TeamId = teamId });
        world.AddComponent(entity, new PlacedWardComponent
        {
            Type = type, TeamId = teamId, RemainingTime = duration,
        });

        if (world.TryGetComponent<VisionComponent>(entity.Id, out var vision))
        {
            vision.SightRange = sightRadius;
            vision.TeamId = teamId;
        }
        else
        {
            world.AddComponent(new Entity(entity.Id), new VisionComponent { SightRange = sightRadius, TeamId = teamId });
        }

        return entity;
    }
}
