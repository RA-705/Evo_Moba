using Evo.Core.ECS;
using Evo.Shared.Math;

namespace Evo.Core.Spatial;

public sealed class SpatialGrid
{
    private readonly float _cellSize;
    private readonly Dictionary<(int cx, int cz), HashSet<EntityId>> _cells = new();
    private readonly Dictionary<EntityId, EvoVector3> _positions = new();
    private readonly Dictionary<EntityId, (int cx, int cz)> _entityCells = new();

    public SpatialGrid(float cellSize = 10f)
    {
        if (cellSize <= 0)
            throw new ArgumentException("Cell size must be positive.", nameof(cellSize));
        _cellSize = cellSize;
    }

    public float CellSize => _cellSize;
    public int EntityCount => _entityCells.Count;

    public void Add(EntityId id, EvoVector3 position)
    {
        var cell = ToCell(position);
        GetOrCreateCell(cell).Add(id);
        _entityCells[id] = cell;
        _positions[id] = position;
    }

    public void Remove(EntityId id)
    {
        if (!_entityCells.TryGetValue(id, out var cell))
            return;

        RemoveFromCell(cell, id);
        _entityCells.Remove(id);
        _positions.Remove(id);
    }

    public void Move(EntityId id, EvoVector3 position)
    {
        if (!_entityCells.TryGetValue(id, out var oldCell))
        {
            Add(id, position);
            return;
        }

        var newCell = ToCell(position);
        if (oldCell != newCell)
        {
            RemoveFromCell(oldCell, id);
            GetOrCreateCell(newCell).Add(id);
            _entityCells[id] = newCell;
        }

        _positions[id] = position;
    }

    public EvoVector3? GetPosition(EntityId id) =>
        _positions.TryGetValue(id, out var pos) ? pos : null;

    public List<EntityId> QueryRadius(EvoVector3 center, float radius)
    {
        var result = new List<EntityId>();
        var radiusSq = radius * radius;
        var cellRadius = (int)MathF.Ceiling(radius / _cellSize);
        var centerCell = ToCell(center);

        for (var dz = -cellRadius; dz <= cellRadius; dz++)
        {
            for (var dx = -cellRadius; dx <= cellRadius; dx++)
            {
                var cell = (centerCell.cx + dx, centerCell.cz + dz);
                if (!_cells.TryGetValue(cell, out var set))
                    continue;

                foreach (var id in set)
                {
                    if (_positions.TryGetValue(id, out var pos))
                    {
                        var offX = pos.X - center.X;
                        var offZ = pos.Z - center.Z;
                        if (offX * offX + offZ * offZ <= radiusSq)
                            result.Add(id);
                    }
                }
            }
        }

        return result;
    }

    public List<EntityId> QueryBounds(EvoVector3 min, EvoVector3 max)
    {
        var result = new List<EntityId>();
        var minCell = ToCell(min);
        var maxCell = ToCell(max);

        for (var cz = minCell.cz; cz <= maxCell.cz; cz++)
        {
            for (var cx = minCell.cx; cx <= maxCell.cx; cx++)
            {
                var cell = (cx, cz);
                if (!_cells.TryGetValue(cell, out var set))
                    continue;

                foreach (var id in set)
                {
                    if (_positions.TryGetValue(id, out var pos) &&
                        pos.X >= min.X && pos.X <= max.X &&
                        pos.Z >= min.Z && pos.Z <= max.Z)
                    {
                        result.Add(id);
                    }
                }
            }
        }

        return result;
    }

    public void Clear()
    {
        _cells.Clear();
        _positions.Clear();
        _entityCells.Clear();
    }

    private HashSet<EntityId> GetOrCreateCell((int cx, int cz) cell)
    {
        if (!_cells.TryGetValue(cell, out var set))
        {
            set = new HashSet<EntityId>();
            _cells[cell] = set;
        }
        return set;
    }

    private void RemoveFromCell((int cx, int cz) cell, EntityId id)
    {
        if (_cells.TryGetValue(cell, out var set))
        {
            set.Remove(id);
            if (set.Count == 0)
                _cells.Remove(cell);
        }
    }

    private (int cx, int cz) ToCell(EvoVector3 pos) =>
        ((int)MathF.Floor(pos.X / _cellSize), (int)MathF.Floor(pos.Z / _cellSize));
}
