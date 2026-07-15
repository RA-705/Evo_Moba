using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Navigation;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.AI.MATE;

public sealed class MateSurvivalSystem : ISystem
{
    private readonly NavGrid _navGrid;

    public MateSurvivalSystem(NavGrid navGrid) =>
        _navGrid = navGrid;

    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<MateBrainComponent>())
        {
            if (!world.TryGetComponent<MateBrainComponent>(id, out var brain) ||
                !world.TryGetComponent<HealthComponent>(id, out var health) ||
                !world.TryGetComponent<PositionComponent>(id, out var pos) ||
                !world.TryGetComponent<PathfollowComponent>(id, out var path))
                continue;

            if (brain.IsRetreating)
            {
                if (EvoVector3.Distance(pos.Value, brain.RetreatTarget) < 1f)
                {
                    brain.IsRetreating = false;
                    world.SetComponent(id, brain);
                }
                continue;
            }

            var healthPercent = health.Current / health.Max;
            if (healthPercent > brain.RetreatHealthThreshold)
                continue;

            ref var attack = ref world.GetComponent<AttackComponent>(id);
            attack.CurrentTargetId = Entity.Null.Id;

            var gridCenter = new EvoVector3(
                _navGrid.Width * _navGrid.CellSize * 0.5f, 0f,
                _navGrid.Depth * _navGrid.CellSize * 0.5f);

            var waypoints = AStarPathfinder.FindPath(_navGrid, pos.Value, gridCenter);
            path.Waypoints = waypoints;
            path.CurrentWaypointIndex = 0;
            world.SetComponent(id, path);

            brain.IsRetreating = true;
            brain.RetreatTarget = gridCenter;
            brain.ReactionTimer = brain.ReactionTime * 1.5f;
            world.SetComponent(id, brain);
        }
    }
}
