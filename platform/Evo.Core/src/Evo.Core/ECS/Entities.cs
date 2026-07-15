using Evo.Foundation.Common;

namespace Evo.Core.ECS;

public sealed class EntityId : StrongId
{
    private static int _nextId;

    internal EntityId(int value) : base(value) { }

    public static EntityId Next() => new(Interlocked.Increment(ref _nextId));
}

public readonly struct Entity : IEquatable<Entity>
{
    public EntityId Id { get; }

    public Entity(EntityId id) => Id = id;

    public static Entity Null => new(new EntityId(0));

    public override bool Equals(object? obj) =>
        obj is Entity other && Id == other.Id;

    public bool Equals(Entity other) => Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity left, Entity right) => left.Equals(right);
    public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
}

public interface IComponent { }
