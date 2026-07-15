namespace Evo.Foundation.Common;

public abstract class StrongId
{
    public int Value { get; }

    protected StrongId(int value) => Value = value;

    public override bool Equals(object? obj) =>
        obj is StrongId other && GetType() == other.GetType() && Value == other.Value;

    public override int GetHashCode() => HashCode.Combine(GetType(), Value);

    public override string ToString() => $"{GetType().Name}: {Value}";

    public static bool operator ==(StrongId? left, StrongId? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(StrongId? left, StrongId? right) =>
        !(left == right);
}

/*
Example — thread-safe sequential ID via per-type static counter:

public sealed class ExampleEntityId : StrongId
{
    private static int _nextId;

    private ExampleEntityId(int value) : base(value) { }

    public static ExampleEntityId Next() => new(Interlocked.Increment(ref _nextId));
}
*/
