using Evo.Shared.Math;

namespace Evo.MOBA.Navigation;

public static class AStarPathfinder
{
    private static readonly (int dx, int dz, int cost)[] Neighbors =
    {
        (0, -1, 10), (1, 0, 10), (0, 1, 10), (-1, 0, 10),
        (-1, -1, 14), (1, -1, 14), (1, 1, 14), (-1, 1, 14),
    };

    public static List<EvoVector3> FindPath(NavGrid grid, EvoVector3 startWorldPos, EvoVector3 targetWorldPos)
    {
        var (startX, startZ) = grid.WorldToGrid(startWorldPos);
        var (targetX, targetZ) = grid.WorldToGrid(targetWorldPos);

        if (!IsInBounds(grid, startX, startZ) || !IsInBounds(grid, targetX, targetZ))
            return new List<EvoVector3>(0);

        ref var startNode = ref grid.GetNode(startX, startZ);
        ref var targetNode = ref grid.GetNode(targetX, targetZ);

        if (!startNode.IsWalkable || !targetNode.IsWalkable)
            return new List<EvoVector3>(0);

        var openSet = new List<int>(grid.Width * grid.Depth);
        var closedSet = new bool[grid.Width * grid.Depth];

        for (var i = 0; i < grid.Width * grid.Depth; i++)
        {
            grid.GetNode(i % grid.Width, i / grid.Width).GCost = int.MaxValue;
            grid.GetNode(i % grid.Width, i / grid.Width).ParentIndex = -1;
        }

        var startIndex = startZ * grid.Width + startX;
        startNode.GCost = 0;
        startNode.HCost = Heuristic(startX, startZ, targetX, targetZ);
        openSet.Add(startIndex);

        while (openSet.Count > 0)
        {
            var currentIndex = GetLowestFCost(openSet, grid);
            var currentZ = currentIndex / grid.Width;
            var currentX = currentIndex % grid.Width;

            if (currentX == targetX && currentZ == targetZ)
                return ReconstructPath(grid, currentIndex);

            openSet.Remove(currentIndex);
            closedSet[currentIndex] = true;

            foreach (var (dx, dz, moveCost) in Neighbors)
            {
                var nx = currentX + dx;
                var nz = currentZ + dz;
                var neighborIndex = nz * grid.Width + nx;

                if (!IsInBounds(grid, nx, nz) || closedSet[neighborIndex])
                    continue;

                ref var neighbor = ref grid.GetNode(nx, nz);
                if (!neighbor.IsWalkable)
                    continue;

                var newGCost = grid.GetNode(currentX, currentZ).GCost + moveCost;

                if (newGCost < neighbor.GCost)
                {
                    neighbor.GCost = newGCost;
                    neighbor.HCost = Heuristic(nx, nz, targetX, targetZ);
                    neighbor.ParentIndex = currentIndex;

                    if (!openSet.Contains(neighborIndex))
                        openSet.Add(neighborIndex);
                }
            }
        }

        return new List<EvoVector3>(0);
    }

    private static int GetLowestFCost(List<int> openSet, NavGrid grid)
    {
        var best = openSet[0];
        var bestF = grid.GetNode(best % grid.Width, best / grid.Width).FCost;

        for (var i = 1; i < openSet.Count; i++)
        {
            var idx = openSet[i];
            var f = grid.GetNode(idx % grid.Width, idx / grid.Width).FCost;

            if (f < bestF)
            {
                bestF = f;
                best = idx;
            }
        }

        return best;
    }

    private static int Heuristic(int x1, int z1, int x2, int z2)
    {
        var dx = Math.Abs(x1 - x2);
        var dz = Math.Abs(z1 - z2);
        return Math.Min(dx, dz) * 14 + Math.Abs(dx - dz) * 10;
    }

    private static List<EvoVector3> ReconstructPath(NavGrid grid, int currentIndex)
    {
        var path = new List<EvoVector3>();
        var idx = currentIndex;

        while (idx != -1)
        {
            var x = idx % grid.Width;
            var z = idx / grid.Width;
            path.Add(grid.GridToWorld(x, z));
            idx = grid.GetNode(x, z).ParentIndex;
        }

        path.Reverse();
        return path;
    }

    private static bool IsInBounds(NavGrid grid, int x, int z) =>
        x >= 0 && x < grid.Width && z >= 0 && z < grid.Depth;
}
