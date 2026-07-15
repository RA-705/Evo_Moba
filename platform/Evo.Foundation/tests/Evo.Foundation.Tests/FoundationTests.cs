using Evo.Foundation.Common;
using Evo.Foundation.Functional;
using Evo.Foundation.Results;

namespace Evo.Foundation.Tests;

public sealed class FoundationTests
{
    // ── Result<T> ──────────────────────────────────────────────────────

    [Fact]
    public void Result_T_Success_ContainsValue()
    {
        var r = Result<int>.Success(42);

        Assert.True(r.IsSuccess);
        Assert.Equal(42, r.Match(v => v, _ => -1));
    }

    [Fact]
    public void Result_T_Failure_ReturnsErrorCode_NoException()
    {
        var r = Result<int>.Failure("ERR_SIM_LAG");

        Assert.True(r.IsError);
        Assert.Equal("ERR_SIM_LAG", r.Error);
    }

    [Fact]
    public void Result_T_Match_InvokesCorrectBranch()
    {
        var ok = Result<string>.Success("ok");
        var fail = Result<string>.Failure("nope");

        Assert.Equal("ok", ok.Match(v => v, e => e));
        Assert.Equal("nope", fail.Match(v => v, e => e));
    }

    // ── Option<T> ──────────────────────────────────────────────────────

    [Fact]
    public void Option_Some_Match_ReturnsTransformedValue()
    {
        var opt = Option<int>.Some(7);

        var result = opt.Match(v => v * 2, () => -1);

        Assert.Equal(14, result);
    }

    [Fact]
    public void Option_None_Match_InvokesNoneBranch()
    {
        var opt = Option<int>.None();

        var result = opt.Match(v => v * 2, () => -1);

        Assert.Equal(-1, result);
    }

    [Fact]
    public void Option_None_IsNotSome()
    {
        Assert.False(Option<int>.None().IsSome);
        Assert.True(Option<int>.None().IsNone);
    }

    // ── Guard ──────────────────────────────────────────────────────────

    [Fact]
    public void Guard_AgainstNull_NullValue_ReturnsFailure_NoThrow()
    {
        string? simParam = null;

        var result = Guard.AgainstNull(simParam, "Simulation tick rate cannot be null.");

        Assert.True(result.IsError);
    }

    [Fact]
    public void Guard_AgainstNull_ValidValue_ReturnsSuccess()
    {
        var result = Guard.AgainstNull("valid");

        Assert.True(result.IsSuccess);
    }
}
