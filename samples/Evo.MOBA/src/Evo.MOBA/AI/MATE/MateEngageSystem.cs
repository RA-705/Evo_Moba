using Evo.Core.ECS;
using Evo.MOBA.Combat;
using Evo.MOBA.Navigation;
using Evo.MOBA.Systems;
using Evo.Shared.Math;
using MateOS.Core;

namespace Evo.MOBA.AI.MATE;

public sealed class MateEngageSystem : ISystem
{
    private readonly NavGrid _navGrid;

    public MateEngageSystem(NavGrid navGrid) =>
        _navGrid = navGrid;

    public void OnTick(World world, float deltaTime)
    {
        foreach (var id in world.GetEntityIds<MOBAComponent>())
        {
            if (!world.TryGetComponent<MOBAComponent>(id, out var moba) ||
                !world.TryGetComponent<PositionComponent>(id, out var pos))
                continue;

            foreach (var intention in moba.Core.ExecuteIntentions())
            {
                switch (intention.Kind)
                {
                    case IntentionKind.Move:
                        var moveData = intention.GetMoveData();
                        ExecuteMove(world, id, pos.Value, moveData);
                        break;

                    case IntentionKind.Attack:
                        var attackData = intention.GetAttackData();
                        ExecuteAttack(world, id, attackData);
                        break;

                    case IntentionKind.Stop:
                        ExecuteStop(world, id);
                        break;
                }
            }

            world.SetComponent(id, moba);
        }
    }

    private void ExecuteMove(World world, EntityId id, EvoVector3 currentPos, IntentionMove moveData)
    {
        var targetPos = new EvoVector3(moveData.X, 0, moveData.Z);

        if (EvoVector3.Distance(currentPos, targetPos) < 0.5f)
            return;

        if (!world.TryGetComponent<PathfollowComponent>(id, out var path))
        {
            path = new PathfollowComponent();
        }

        var waypoints = AStarPathfinder.FindPath(_navGrid, currentPos, targetPos);
        path.Waypoints = waypoints;
        path.CurrentWaypointIndex = 0;
        path.StoppingDistance = 0.3f;
        world.SetComponent(id, path);

        if (world.TryGetComponent<AttackComponent>(id, out var attack))
        {
            attack.CurrentTargetId = Entity.Null.Id;
            world.SetComponent(id, attack);
        }
    }

    private void ExecuteAttack(World world, EntityId id, IntentionAttack attackData)
    {
        var found = EntityHelper.FindEntityId(world, attackData.TargetId);
        if (found == null) return;
        var targetId = found;

        if (!world.TryGetComponent<HealthComponent>(targetId, out _))
            return;

        if (world.TryGetComponent<AttackComponent>(id, out var attack))
        {
            attack.CurrentTargetId = targetId;
            world.SetComponent(id, attack);
        }

        if (!world.TryGetComponent<PositionComponent>(id, out var pos) ||
            !world.TryGetComponent<PositionComponent>(targetId, out var targetPos))
            return;

        if (!world.TryGetComponent<PathfollowComponent>(id, out var path))
        {
            path = new PathfollowComponent();
        }

        if (!world.TryGetComponent<AttackComponent>(id, out var atk))
            return;

        var dist = EvoVector3.Distance(pos.Value, targetPos.Value);
        if (dist > atk.Range * 0.9f)
        {
            var waypoints = AStarPathfinder.FindPath(_navGrid, pos.Value, targetPos.Value);
            path.Waypoints = waypoints;
            path.CurrentWaypointIndex = 0;
            path.StoppingDistance = atk.Range * 0.8f;
            world.SetComponent(id, path);
        }
        else
        {
            path.Waypoints?.Clear();
            path.CurrentWaypointIndex = 0;
            world.SetComponent(id, path);
        }
    }

    private void ExecuteStop(World world, EntityId id)
    {
        if (world.TryGetComponent<PathfollowComponent>(id, out var path))
        {
            path.Waypoints?.Clear();
            path.CurrentWaypointIndex = 0;
            world.SetComponent(id, path);
        }

        if (world.TryGetComponent<AttackComponent>(id, out var attack))
        {
            attack.CurrentTargetId = Entity.Null.Id;
            world.SetComponent(id, attack);
        }
    }
}
