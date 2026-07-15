using Evo.Shared.Math;

namespace Evo.MOBA.Navigation;

public struct GridNode
{
    public bool IsWalkable;
    public int GridX;
    public int GridZ;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;
    public int ParentIndex;
}

public sealed class NavGrid
{
    private readonly GridNode[] _nodes;
    public int Width { get; }
    public int Depth { get; }
    public float CellSize { get; }

    public NavGrid(int width, int depth, float cellSize)
    {
        Width = width;
        Depth = depth;
        CellSize = cellSize;
        _nodes = new GridNode[width * depth];

        for (var z = 0; z < depth; z++)
        {
            for (var x = 0; x < width; x++)
            {
                var i = z * width + x;
                _nodes[i].GridX = x;
                _nodes[i].GridZ = z;
                _nodes[i].IsWalkable = true;
            }
        }
    }

    public ref GridNode GetNode(int x, int z) =>
        ref _nodes[z * Width + x];

    public void SetWalkable(int x, int z, bool isWalkable) =>
        GetNode(x, z).IsWalkable = isWalkable;

    public EvoVector3 GridToWorld(int x, int z) =>
        new(x * CellSize + CellSize * 0.5f, 0f, z * CellSize + CellSize * 0.5f);

    public (int x, int z) WorldToGrid(EvoVector3 worldPos) =>
        ((int)(worldPos.X / CellSize), (int)(worldPos.Z / CellSize));
}
