namespace Evo.Core.ECS;

public interface ISystem
{
    void OnTick(World world, float deltaTime);
}

public interface IComponentPool
{
    void Remove(EntityId id);
    IEnumerable<EntityId> EntityIds { get; }
}

public sealed class ComponentPool<T> : IComponentPool where T : struct, IComponent
{
    private sealed class Box
    {
        public T Value;
    }

    private readonly Dictionary<EntityId, Box> _components = new();

    public void Add(EntityId id, T component)
    {
        if (_components.TryGetValue(id, out var existing))
            existing.Value = component;
        else
            _components[id] = new Box { Value = component };
    }

    public bool Has(EntityId id) =>
        _components.ContainsKey(id);

    public void Remove(EntityId id) =>
        _components.Remove(id);

    public IEnumerable<EntityId> EntityIds => _components.Keys;

    public ref T Get(EntityId id)
    {
        if (!_components.TryGetValue(id, out var box))
            throw new KeyNotFoundException($"Component '{typeof(T).Name}' not found for entity {id}.");

        return ref box.Value;
    }
}

public sealed class World : ITickable
{
    private readonly Dictionary<Type, IComponentPool> _pools = new();
    private readonly List<ISystem> _systems = new();
    private readonly CommandBuffer _commandBuffer = new();

    public Entity Create()
    {
        var id = EntityId.Next();
        return new Entity(id);
    }

    public void Destroy(Entity entity)
    {
        foreach (var pool in _pools.Values)
            pool.Remove(entity.Id);
    }

    public void AddComponent<T>(Entity entity, T component) where T : struct, IComponent
    {
        if (!_pools.TryGetValue(typeof(T), out var pool))
        {
            pool = new ComponentPool<T>();
            _pools[typeof(T)] = pool;
        }

        ((ComponentPool<T>)pool).Add(entity.Id, component);
    }

    public ref T GetComponent<T>(Entity entity) where T : struct, IComponent
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
            return ref ((ComponentPool<T>)pool).Get(entity.Id);

        throw new InvalidOperationException($"Component of type {typeof(T).Name} not found on entity.");
    }

    public ref T GetComponent<T>(EntityId id) where T : struct, IComponent
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
            return ref ((ComponentPool<T>)pool).Get(id);

        throw new InvalidOperationException($"Component of type {typeof(T).Name} not found on entity.");
    }

    public bool TryGetComponent<T>(EntityId id, out T component) where T : struct, IComponent
    {
        if (_pools.TryGetValue(typeof(T), out var pool) && ((ComponentPool<T>)pool).Has(id))
        {
            component = ((ComponentPool<T>)pool).Get(id);
            return true;
        }

        component = default;
        return false;
    }

    public void SetComponent<T>(EntityId id, T component) where T : struct, IComponent
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            ((ComponentPool<T>)pool).Add(id, component);
        }
    }

    public void RemoveComponent<T>(EntityId id) where T : struct, IComponent
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            pool.Remove(id);
        }
    }

    public IEnumerable<EntityId> GetEntityIds<T>() where T : struct, IComponent
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
            return pool.EntityIds;

        return Enumerable.Empty<EntityId>();
    }

    public IEnumerable<EntityId> Query<T1, T2>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
    {
        if (!_pools.TryGetValue(typeof(T1), out var p1) ||
            !_pools.TryGetValue(typeof(T2), out var p2))
            yield break;

        var pool1 = (ComponentPool<T1>)p1;
        var pool2 = (ComponentPool<T2>)p2;

        foreach (var id in pool1.EntityIds)
            if (pool2.Has(id))
                yield return id;
    }

    public IEnumerable<EntityId> Query<T1, T2, T3>()
        where T1 : struct, IComponent
        where T2 : struct, IComponent
        where T3 : struct, IComponent
    {
        if (!_pools.TryGetValue(typeof(T1), out var p1) ||
            !_pools.TryGetValue(typeof(T2), out var p2) ||
            !_pools.TryGetValue(typeof(T3), out var p3))
            yield break;

        var pool1 = (ComponentPool<T1>)p1;
        var pool2 = (ComponentPool<T2>)p2;
        var pool3 = (ComponentPool<T3>)p3;

        foreach (var id in pool1.EntityIds)
            if (pool2.Has(id) && pool3.Has(id))
                yield return id;
    }

    public CommandBuffer Commands => _commandBuffer;

    public void AddSystem(ISystem system) =>
        _systems.Add(system);

    public void Tick(float deltaTime)
    {
        try
        {
            for (var i = 0; i < _systems.Count; i++)
            {
                try
                {
                    _systems[i].OnTick(this, deltaTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[World] System '{_systems[i].GetType().Name}' failed: {ex.Message}");
                    Console.Out.Flush();
                }
            }

            _commandBuffer.Execute(this);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[World] Tick failed: {ex.Message}");
            Console.Out.Flush();
        }
    }
}
