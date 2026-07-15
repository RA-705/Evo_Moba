using Evo.Core;
using Evo.Core.ECS;
using Evo.Core.Prefabs;
using Evo.Core.Spatial;
using Evo.Quickstart;

class MovementSystem : ISystem
{
    public void OnTick(World world, float dt)
    {
        foreach (var id in world.GetEntityIds<Position>())
        {
            if (!world.TryGetComponent<Velocity>(id, out var vel))
                continue;

            ref var pos = ref world.GetComponent<Position>(id);
            pos.Value += vel.Value * dt;
        }
    }
}

class SpatialSystem : ISystem
{
    private readonly SpatialGrid _grid;
    private int _tick;

    public SpatialSystem(SpatialGrid grid) => _grid = grid;

    public void OnTick(World world, float dt)
    {
        foreach (var id in world.GetEntityIds<Position>())
        {
            var pos = world.GetComponent<Position>(id);
            _grid.Move(id, pos.Value);
        }

        if (++_tick % 10 != 0)
            return;

        var origin = new Evo.Shared.Math.EvoVector3(0, 0, 0);
        var nearby = _grid.QueryRadius(origin, 5f);
        Console.WriteLine($"  Entities near origin: {string.Join(", ", nearby.Select(e => e.Value))}");
    }
}

class StatusSystem : ISystem
{
    private int _tick;

    public void OnTick(World world, float dt)
    {
        if (++_tick % 10 != 0)
            return;

        foreach (var id in world.GetEntityIds<Position>())
        {
            var pos = world.GetComponent<Position>(id);
            Console.WriteLine($"  [{id.Value}] @ {pos.Value:F2}");
        }
        Console.WriteLine();
    }
}

class Program
{
    static void Main()
    {
        var world = new World();
        var grid = new SpatialGrid(10f);
        world.AddSystem(new MovementSystem());
        world.AddSystem(new SpatialSystem(grid));
        world.AddSystem(new StatusSystem());

        var prefabPath = Path.Combine(AppContext.BaseDirectory, "prefabs.json");
        var registry = PrefabRegistry.Load(prefabPath);

        Console.WriteLine("=== EVO ECS Quickstart with Spatial Grid ===");
        Console.WriteLine($"Loaded {registry.All.Count} prefabs\n");

        world.Spawn("Walker", registry);
        world.Spawn("Faller", registry);
        world.Spawn("DiagMover", registry);

        Console.WriteLine("Running 60 ticks at 10 tps...\n");
        var loop = new FixedTickLoop(10);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(6));
        loop.Start(world, cts.Token);

        Console.WriteLine("=== Done ===");
    }
}
