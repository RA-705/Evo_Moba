using Evo.Core.ECS;

namespace Evo.Tests;

struct TestPos : IComponent
{
    public float X;
    public float Y;
}

struct TestVel : IComponent
{
    public float Vx;
    public float Vy;
}

sealed class TestSystem : ISystem
{
    public int TickCount;

    public void OnTick(World world, float deltaTime)
    {
        TickCount++;
    }
}

sealed class MoveSystem : ISystem
{
    public void OnTick(World world, float dt)
    {
        foreach (var id in world.GetEntityIds<TestPos>())
        {
            if (!world.TryGetComponent<TestVel>(id, out var vel))
                continue;

            ref var pos = ref world.GetComponent<TestPos>(id);
            pos.X += vel.Vx * dt;
            pos.Y += vel.Vy * dt;
        }
    }
}

public class WorldTests
{
    [Fact]
    public void Create_ReturnsUniqueEntities()
    {
        var w = new World();
        var a = w.Create();
        var b = w.Create();

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void AddComponent_StoresAndRetrieves()
    {
        var w = new World();
        var e = w.Create();

        w.AddComponent(e, new TestPos { X = 10, Y = 20 });

        var pos = w.GetComponent<TestPos>(e);
        Assert.Equal(10, pos.X);
        Assert.Equal(20, pos.Y);
    }

    [Fact]
    public void GetComponent_ReturnsRef()
    {
        var w = new World();
        var e = w.Create();
        w.AddComponent(e, new TestPos { X = 1, Y = 2 });

        ref var pos = ref w.GetComponent<TestPos>(e);
        pos.X = 99;

        var read = w.GetComponent<TestPos>(e);
        Assert.Equal(99, read.X);
    }

    [Fact]
    public void TryGetComponent_ReturnsTrueWhenExists()
    {
        var w = new World();
        var e = w.Create();
        w.AddComponent(e, new TestPos());

        var found = w.TryGetComponent<TestPos>(e.Id, out var val);
        Assert.True(found);
    }

    [Fact]
    public void TryGetComponent_ReturnsFalseWhenMissing()
    {
        var w = new World();
        var e = w.Create();

        var found = w.TryGetComponent<TestPos>(e.Id, out _);
        Assert.False(found);
    }

    [Fact]
    public void RemoveComponent_RemovesFromPool()
    {
        var w = new World();
        var e = w.Create();
        w.AddComponent(e, new TestPos());
        w.RemoveComponent<TestPos>(e.Id);

        var found = w.TryGetComponent<TestPos>(e.Id, out _);
        Assert.False(found);
    }

    [Fact]
    public void Destroy_Entity_RemovesAllComponents()
    {
        var w = new World();
        var e = w.Create();
        w.AddComponent(e, new TestPos());
        w.AddComponent(e, new TestVel());

        w.Destroy(e);

        Assert.False(w.TryGetComponent<TestPos>(e.Id, out _));
        Assert.False(w.TryGetComponent<TestVel>(e.Id, out _));
    }

    [Fact]
    public void SetComponent_UpdatesExisting()
    {
        var w = new World();
        var e = w.Create();
        w.AddComponent(e, new TestPos { X = 1, Y = 2 });

        w.SetComponent(e.Id, new TestPos { X = 10, Y = 20 });

        var pos = w.GetComponent<TestPos>(e);
        Assert.Equal(10, pos.X);
    }

    [Fact]
    public void AddSystem_CallsOnTick()
    {
        var w = new World();
        var sys = new TestSystem();
        w.AddSystem(sys);

        w.Tick(1f / 60f);

        Assert.Equal(1, sys.TickCount);
    }

    [Fact]
    public void Tick_RunsAllSystems()
    {
        var w = new World();
        var a = new TestSystem();
        var b = new TestSystem();
        w.AddSystem(a);
        w.AddSystem(b);

        w.Tick(1f / 60f);

        Assert.Equal(1, a.TickCount);
        Assert.Equal(1, b.TickCount);
    }

    [Fact]
    public void MovementSystem_UpdatesPosition()
    {
        var w = new World();
        w.AddSystem(new MoveSystem());

        var e = w.Create();
        w.AddComponent(e, new TestPos { X = 0, Y = 0 });
        w.AddComponent(e, new TestVel { Vx = 10, Vy = 0 });

        w.Tick(1f);

        var pos = w.GetComponent<TestPos>(e);
        Assert.Equal(10, pos.X, 4);
        Assert.Equal(0, pos.Y, 4);
    }

    [Fact]
    public void GetEntityIds_ReturnsOnlyEntityIdsWithComponent()
    {
        var w = new World();
        var a = w.Create(); w.AddComponent(a, new TestPos());
        var b = w.Create(); w.AddComponent(b, new TestPos());
        var c = w.Create(); w.AddComponent(c, new TestPos());

        var ids = w.GetEntityIds<TestPos>().ToArray();
        Assert.Equal(3, ids.Length);
    }

    [Fact]
    public void MultipleTicks_AccumulateCorrectly()
    {
        var w = new World();
        w.AddSystem(new MoveSystem());

        var e = w.Create();
        w.AddComponent(e, new TestPos { X = 0, Y = 0 });
        w.AddComponent(e, new TestVel { Vx = 1, Vy = 0 });

        for (int i = 0; i < 10; i++)
            w.Tick(0.1f);

        var pos = w.GetComponent<TestPos>(e);
        Assert.Equal(1f, pos.X, 4);
    }
}
