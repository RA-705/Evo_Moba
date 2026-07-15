using Evo.Core.ECS;

namespace Evo.Gameplay;

public sealed class GameEntity
{
    private readonly Dictionary<Type, EntityComponent> _components = new();

    public EntityId Id { get; }
    public byte TeamId { get; private set; }
    public EntityId? OwnerId { get; private set; }

    public GameEntity(EntityId id) => Id = id;

    public void Initialize(byte teamId, EntityId? ownerId, List<EntityComponent> components)
    {
        TeamId = teamId;
        OwnerId = ownerId;
        _components.Clear();
        foreach (var component in components)
            _components[component.GetType()] = component;
    }

    public T GetComponent<T>() where T : EntityComponent =>
        (T)_components[typeof(T)];

    public bool TryGetComponent<T>(out T component) where T : EntityComponent
    {
        if (_components.TryGetValue(typeof(T), out var c))
        {
            component = (T)c;
            return true;
        }
        component = null!;
        return false;
    }
}
