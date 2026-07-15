using Evo.Core.ECS;

namespace Evo.Tests;

struct PoolComp : IComponent
{
    public int Value;
}

public class ComponentPoolTests
{
    [Fact]
    public void Add_ComponentCanBeRetrieved()
    {
        var pool = new ComponentPool<PoolComp>();
        var id = EntityId.Next();

        pool.Add(id, new PoolComp { Value = 7 });

        Assert.True(pool.Has(id));
    }

    [Fact]
    public void Get_ReturnsRefToSameStorage()
    {
        var pool = new ComponentPool<PoolComp>();
        var id = EntityId.Next();

        pool.Add(id, new PoolComp { Value = 10 });

        ref var comp = ref pool.Get(id);
        comp.Value = 20;

        Assert.Equal(20, pool.Get(id).Value);
    }

    [Fact]
    public void Remove_ClearsComponent()
    {
        var pool = new ComponentPool<PoolComp>();
        var id = EntityId.Next();

        pool.Add(id, new PoolComp());
        pool.Remove(id);

        Assert.False(pool.Has(id));
    }

    [Fact]
    public void Has_ReturnsFalseForMissing()
    {
        var pool = new ComponentPool<PoolComp>();
        Assert.False(pool.Has(EntityId.Next()));
    }

    [Fact]
    public void EntityIds_ReturnsAddedIds()
    {
        var pool = new ComponentPool<PoolComp>();
        var a = EntityId.Next();
        var b = EntityId.Next();

        pool.Add(a, new PoolComp());
        pool.Add(b, new PoolComp());

        var ids = pool.EntityIds.ToHashSet();
        Assert.Contains(a, ids);
        Assert.Contains(b, ids);
        Assert.Equal(2, ids.Count);
    }

    [Fact]
    public void Add_OverwritesExisting()
    {
        var pool = new ComponentPool<PoolComp>();
        var id = EntityId.Next();

        pool.Add(id, new PoolComp { Value = 5 });
        pool.Add(id, new PoolComp { Value = 10 });

        Assert.Equal(10, pool.Get(id).Value);
    }

    [Fact]
    public void Remove_NonExistent_DoesNotThrow()
    {
        var pool = new ComponentPool<PoolComp>();
        pool.Remove(EntityId.Next());
    }
}
