using Evo.Foundation.Results;

namespace Evo.Foundation.Common;

public static class Guard
{
    public static Result AgainstNull(object? value, string errorMessage = "Value cannot be null.") =>
        value is null ? Result.Failure(errorMessage) : Result.Success();

    public static Result AgainstNullOrEmpty(string? value, string errorMessage = "String cannot be null or empty.") =>
        string.IsNullOrEmpty(value) ? Result.Failure(errorMessage) : Result.Success();

    public static Result AgainstOutOfRange(int value, int min, int max, string errorMessage = "Value is out of range.") =>
        value < min || value > max ? Result.Failure(errorMessage) : Result.Success();
}
