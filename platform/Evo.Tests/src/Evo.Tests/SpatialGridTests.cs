using Evo.Core.ECS;
using Evo.Core.Spatial;
using Evo.Shared.Math;

namespace Evo.Tests;

public class SpatialGridTests
{
    [Fact]
    public void Add_EntityIsQueryable()
    {
        var grid = new SpatialGrid(10f);
        var id = EntityId.Next();

        grid.Add(id, new EvoVector3(5, 0, 5));

        var nearby = grid.QueryRadius(new EvoVector3(5, 0, 5), 1f);
        Assert.Contains(id, nearby);
    }

    [Fact]
    public void Remove_EntityIsGone()
    {
        var grid = new SpatialGrid(10f);
        var id = EntityId.Next();

        grid.Add(id, new EvoVector3(5, 0, 5));
        grid.Remove(id);

        var nearby = grid.QueryRadius(new EvoVector3(5, 0, 5), 1f);
        Assert.DoesNotContain(id, nearby);
    }

    [Fact]
    public void QueryRadius_ReturnsEntitiesWithinRange()
    {
        var grid = new SpatialGrid(10f);
        var near = EntityId.Next();
        var far = EntityId.Next();

        grid.Add(near, new EvoVector3(0, 0, 0));
        grid.Add(far, new EvoVector3(50, 0, 50));

        var nearby = grid.QueryRadius(new EvoVector3(0, 0, 0), 10f);
        Assert.Contains(near, nearby);
        Assert.DoesNotContain(far, nearby);
    }

    [Fact]
    public void QueryRadius_ExcludesOutOfRange()
    {
        var grid = new SpatialGrid(5f);
        var id = EntityId.Next();

        grid.Add(id, new EvoVector3(100, 0, 100));

        var nearby = grid.QueryRadius(new EvoVector3(0, 0, 0), 1f);
        Assert.Empty(nearby);
    }

    [Fact]
    public void Move_UpdatesPosition()
    {
        var grid = new SpatialGrid(10f);
        var id = EntityId.Next();

        grid.Add(id, new EvoVector3(0, 0, 0));
        grid.Move(id, new EvoVector3(50, 0, 50));

        var atOrigin = grid.QueryRadius(new EvoVector3(0, 0, 0), 1f);
        var atDest = grid.QueryRadius(new EvoVector3(50, 0, 50), 1f);

        Assert.DoesNotContain(id, atOrigin);
        Assert.Contains(id, atDest);
    }

    [Fact]
    public void Move_ToSameCell_DoesNotChangeCell()
    {
        var grid = new SpatialGrid(10f);
        var id = EntityId.Next();

        grid.Add(id, new EvoVector3(1, 0, 1));
        grid.Move(id, new EvoVector3(2, 0, 2));

        var nearby = grid.QueryRadius(new EvoVector3(2, 0, 2), 5f);
        Assert.Contains(id, nearby);
    }

    [Fact]
    public void QueryBounds_ReturnsEntitiesInArea()
    {
        var grid = new SpatialGrid(10f);
        var inside = EntityId.Next();
        var outside = EntityId.Next();

        grid.Add(inside, new EvoVector3(5, 0, 5));
        grid.Add(outside, new EvoVector3(50, 0, 50));

        var result = grid.QueryBounds(new EvoVector3(0, 0, 0), new EvoVector3(10, 0, 10));
        Assert.Contains(inside, result);
        Assert.DoesNotContain(outside, result);
    }

    [Fact]
    public void Clear_RemovesAll()
    {
        var grid = new SpatialGrid(10f);
        grid.Add(EntityId.Next(), new EvoVector3(1, 0, 1));
        grid.Add(EntityId.Next(), new EvoVector3(2, 0, 2));

        grid.Clear();

        Assert.Equal(0, grid.EntityCount);
    }

    [Fact]
    public void Add_MultipleEntities_SameCell()
    {
        var grid = new SpatialGrid(10f);
        var a = EntityId.Next();
        var b = EntityId.Next();

        grid.Add(a, new EvoVector3(1, 0, 1));
        grid.Add(b, new EvoVector3(2, 0, 2));

        var nearby = grid.QueryRadius(new EvoVector3(0, 0, 0), 5f);
        Assert.Equal(2, nearby.Count);
    }

    [Fact]
    public void GetPosition_ReturnsStoredPosition()
    {
        var grid = new SpatialGrid(10f);
        var id = EntityId.Next();
        var expected = new EvoVector3(15, 0, 25);

        grid.Add(id, expected);

        var actual = grid.GetPosition(id);
        Assert.NotNull(actual);
        Assert.Equal(expected.X, actual.Value.X);
        Assert.Equal(expected.Z, actual.Value.Z);
    }

    [Fact]
    public void Remove_NonExistent_DoesNotThrow()
    {
        var grid = new SpatialGrid(10f);
        grid.Remove(EntityId.Next());
    }

    [Fact]
    public void Constructor_NegativeCellSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => new SpatialGrid(-1));
    }

    [Fact]
    public void Move_NonExistent_Adds()
    {
        var grid = new SpatialGrid(10f);
        var id = EntityId.Next();

        grid.Move(id, new EvoVector3(5, 0, 5));

        Assert.Equal(1, grid.EntityCount);
    }
}
