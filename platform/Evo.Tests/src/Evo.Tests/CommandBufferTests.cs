using Evo.Core.ECS;

namespace Evo.Tests;

public class CommandBufferTests
{
    [Fact]
    public void Destroy_RemovesEntityAfterFlush()
    {
        var w = new World();
        var e = w.Create();
        w.AddComponent(e, new TestPos());

        w.Commands.Destroy(e);
        Assert.True(w.TryGetComponent<TestPos>(e.Id, out _));

        w.Tick(0);

        Assert.False(w.TryGetComponent<TestPos>(e.Id, out _));
    }

    [Fact]
    public void AddComponent_AppliesAfterFlush()
    {
        var w = new World();
        var e = w.Create();

        w.Commands.AddComponent(e, new TestPos { X = 5, Y = 10 });
        Assert.False(w.TryGetComponent<TestPos>(e.Id, out _));

        w.Tick(0);

        Assert.True(w.TryGetComponent<TestPos>(e.Id, out _));
        Assert.Equal(5, w.GetComponent<TestPos>(e).X);
    }

    [Fact]
    public void RemoveComponent_AppliesAfterFlush()
    {
        var w = new World();
        var e = w.Create();
        w.AddComponent(e, new TestPos());

        w.Commands.RemoveComponent<TestPos>(e);
        w.Tick(0);

        Assert.False(w.TryGetComponent<TestPos>(e.Id, out _));
    }

    [Fact]
    public void DestroyDuringIteration_DoesNotCrash()
    {
        var w = new World();
        var entities = new List<Entity>();

        for (int i = 0; i < 5; i++)
        {
            var e = w.Create();
            w.AddComponent(e, new TestPos());
            entities.Add(e);
        }

        var destroyed = 0;
        w.AddSystem(new LambdaSystem(world =>
        {
            foreach (var id in world.GetEntityIds<TestPos>().ToList())
            {
                world.Commands.Destroy(new Entity(id));
                destroyed++;
            }
        }));

        w.Tick(0);

        Assert.Equal(5, destroyed);
        Assert.Empty(w.GetEntityIds<TestPos>());
    }

    [Fact]
    public void MultipleCommands_AllFlushed()
    {
        var w = new World();
        var a = w.Create();
        var b = w.Create();

        w.Commands.AddComponent(a, new TestPos { X = 1, Y = 2 });
        w.Commands.AddComponent(b, new TestPos { X = 3, Y = 4 });

        w.Tick(0);

        Assert.Equal(1, w.GetComponent<TestPos>(a).X);
        Assert.Equal(3, w.GetComponent<TestPos>(b).X);
    }

    private sealed class LambdaSystem : ISystem
    {
        private readonly Action<World> _action;
        public LambdaSystem(Action<World> action) => _action = action;
        public void OnTick(World world, float dt) => _action(world);
    }
}
