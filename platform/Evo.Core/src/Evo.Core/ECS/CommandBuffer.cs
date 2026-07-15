namespace Evo.Core.ECS;

public sealed class CommandBuffer
{
    private readonly List<Action<World>> _actions = new();

    public void Destroy(Entity entity) =>
        _actions.Add(w => w.Destroy(entity));

    public void AddComponent<T>(Entity entity, T component) where T : struct, IComponent =>
        _actions.Add(w => w.AddComponent(entity, component));

    public void RemoveComponent<T>(EntityId id) where T : struct, IComponent =>
        _actions.Add(w => w.RemoveComponent<T>(id));

    public void RemoveComponent<T>(Entity entity) where T : struct, IComponent =>
        _actions.Add(w => w.RemoveComponent<T>(entity.Id));

    internal void Execute(World world)
    {
        foreach (var action in _actions)
            action(world);
        _actions.Clear();
    }

    internal void Clear() => _actions.Clear();
}
