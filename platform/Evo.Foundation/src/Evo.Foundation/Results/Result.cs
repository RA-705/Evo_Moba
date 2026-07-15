namespace Evo.Foundation.Results;

public readonly record struct Result
{
    private readonly string? _error;

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        _error = error;
    }

    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;
    public string? Error => IsError ? _error : null;

    public T Match<T>(Func<T> onSuccess, Func<string, T> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(_error!);

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error ?? "Unknown error");
}

public readonly record struct Result<T>
{
    private readonly T? _value;
    private readonly string? _error;

    private Result(T value)
    {
        _value = value;
        _error = null;
        IsSuccess = true;
    }

    private Result(string error)
    {
        _value = default;
        _error = error;
        IsSuccess = false;
    }

    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;
    public string? Error => IsError ? _error : null;

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(_error!);

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error ?? "Unknown error");

    public static implicit operator Result<T>(Result result) =>
        result.IsSuccess ? Success(default!) : Failure(result.Error!);
}
