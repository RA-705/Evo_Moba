namespace Evo.Foundation.Functional;

public readonly record struct Option<T>
{
    private readonly T _value;
    private readonly bool _hasValue;

    private Option(T value)
    {
        _value = value;
        _hasValue = true;
    }

    public bool IsSome => _hasValue;
    public bool IsNone => !_hasValue;

    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) =>
        IsSome ? onSome(_value) : onNone();

    public T UnwrapOr(T defaultValue) =>
        IsSome ? _value : defaultValue;

    public static Option<T> Some(T value) => new(value);
    public static Option<T> None() => default;
}
