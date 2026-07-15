using System.Collections.Generic;
using Evo.Core.ECS;
using Evo.MOBA.Navigation;
using Evo.MOBA.Systems;
using Evo.Shared.Math;

namespace Evo.MOBA.AI.MATE;

public struct VisionComponent : IComponent
{
    public float SightRange;
    public int TeamId;
}

public sealed class FogOfWarSystem : ISystem
{
    private readonly NavGrid _grid;
    private readonly Dictionary<int, HashSet<EntityId>> _teamVisible = new();
    private readonly List<EntityId> _visibleBuffer = new();

    public IReadOnlyDictionary<int, HashSet<EntityId>> TeamVisible => _teamVisible;

    public FogOfWarSystem(NavGrid grid)
    {
        _grid = grid;
    }

    public bool IsVisibleToTeam(int teamId, EntityId target)
    {
        return _teamVisible.TryGetValue(teamId, out var set) && set.Contains(target);
    }

    public void OnTick(World world, float deltaTime)
    {
        _teamVisible.Clear();

        foreach (var viewerId in world.GetEntityIds<VisionComponent>())
        {
            if (!world.TryGetComponent<VisionComponent>(viewerId, out var vision) ||
                !world.TryGetComponent<PositionComponent>(viewerId, out var viewerPos))
                continue;

            var viewerTeam = vision.TeamId;
            if (!_teamVisible.ContainsKey(viewerTeam))
                _teamVisible[viewerTeam] = new HashSet<EntityId>();

            var visibleSet = _teamVisible[viewerTeam];
            var visRangeSq = vision.SightRange * vision.SightRange;

            _visibleBuffer.Clear();

            foreach (var targetId in world.GetEntityIds<PositionComponent>())
            {
                if (targetId == viewerId)
                    continue;

                if (!world.TryGetComponent<PositionComponent>(targetId, out var targetPos))
                    continue;

                var dx = targetPos.Value.X - viewerPos.Value.X;
                var dz = targetPos.Value.Z - viewerPos.Value.Z;
                if (dx * dx + dz * dz > visRangeSq)
                    continue;

                if (!HasLineOfSight(viewerPos.Value, targetPos.Value))
                    continue;

                _visibleBuffer.Add(targetId);
            }

            foreach (var id in _visibleBuffer)
                visibleSet.Add(id);
        }
    }

    private bool HasLineOfSight(EvoVector3 from, EvoVector3 to)
    {
        var (x0, z0) = _grid.WorldToGrid(from);
        var (x1, z1) = _grid.WorldToGrid(to);

        int dx = System.Math.Abs(x1 - x0);
        int dz = System.Math.Abs(z1 - z0);
        int sx = x0 < x1 ? 1 : -1;
        int sz = z0 < z1 ? 1 : -1;
        int err = dx - dz;

        int cx = x0, cz = z0;

        while (cx != x1 || cz != z1)
        {
            if (!_grid.GetNode(cx, cz).IsWalkable)
                return false;

            int e2 = 2 * err;
            if (e2 > -dz) { err -= dz; cx += sx; }
            if (e2 < dx) { err += dx; cz += sz; }
        }

        return true;
    }
}
