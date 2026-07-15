using Evo.Core.ECS;
using Evo.Shared.Math;

namespace Evo.MOBA.Systems;

public struct PathfollowComponent : IComponent
{
    public List<EvoVector3> Waypoints;
    public int CurrentWaypointIndex;
    public float StoppingDistance;
}

public struct VelocityComponent : IComponent
{
    public EvoVector3 Value;
}

public struct PositionComponent : IComponent
{
    public EvoVector3 Value;
}

public sealed class PathfollowSystem : ISystem
{
    private const float BaseSpeed = 5f;

    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<PositionComponent>())
        {
            if (!world.TryGetComponent<VelocityComponent>(id, out _) ||
                !world.TryGetComponent<PathfollowComponent>(id, out var pathfollow))
                continue;

            if (pathfollow.Waypoints is null || pathfollow.Waypoints.Count == 0 ||
                pathfollow.CurrentWaypointIndex >= pathfollow.Waypoints.Count)
                continue;

            ref var pos = ref world.GetComponent<PositionComponent>(id);
            ref var vel = ref world.GetComponent<VelocityComponent>(id);

            var target = pathfollow.Waypoints[pathfollow.CurrentWaypointIndex];
            var direction = (target - pos.Value).Normalized();
            vel.Value = direction * BaseSpeed;
            pos.Value += vel.Value * deltaTime;

            if (EvoVector3.Distance(pos.Value, target) < pathfollow.StoppingDistance)
            {
                pathfollow.CurrentWaypointIndex++;
                world.SetComponent(id, pathfollow);
            }
        }
    }
}
