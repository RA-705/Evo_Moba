using BenchmarkDotNet.Attributes;
using Evo.Core.ECS;
using Evo.Shared.Math;

namespace Evo.Benchmarks;

public struct PositionComponent : IComponent
{
    public EvoVector3 Value;
}

public struct VelocityComponent : IComponent
{
    public EvoVector3 Value;
}

[MemoryDiagnoser]
public class EcsBenchmarks
{
    private World _world = null!;
    private Entity[] _entities = null!;

    [Params(100, 1000, 10_000)]
    public int EntityCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _world = new World();
        _world.AddSystem(new BenchmarkSystem());

        _entities = new Entity[EntityCount];
        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i] = _world.Create();
            _world.AddComponent(_entities[i], new PositionComponent { Value = new EvoVector3(i, 0, i) });
            _world.AddComponent(_entities[i], new VelocityComponent { Value = new EvoVector3(1, 0, 0) });
        }
    }

    [Benchmark]
    public void TickWorld() => _world.Tick(1f / 60f);

    [Benchmark]
    public void IterateComponents()
    {
        foreach (var id in _world.GetEntityIds<PositionComponent>())
        {
            if (_world.TryGetComponent<PositionComponent>(id, out var pos))
            {
                var _ = pos.Value.X;
            }
        }
    }

    private sealed class BenchmarkSystem : ISystem
    {
        public void OnTick(World world, float deltaTime)
        {
            foreach (var id in world.GetEntityIds<PositionComponent>())
            {
                if (!world.TryGetComponent<VelocityComponent>(id, out var vel))
                    continue;

                ref var pos = ref world.GetComponent<PositionComponent>(id);
                pos.Value = new EvoVector3(
                    pos.Value.X + vel.Value.X * deltaTime,
                    pos.Value.Y + vel.Value.Y * deltaTime,
                    pos.Value.Z + vel.Value.Z * deltaTime);
            }
        }
    }
}
