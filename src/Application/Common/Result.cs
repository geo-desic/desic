using System.Diagnostics.Contracts;

namespace Desic.Application.Common;

// source: https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/
public readonly struct Result<T>
{
    private enum ResultState
    {
        Null,
        Failure,
        Success
    }

    private readonly ResultState _state;

    public T Value { get; }
    public Error Error { get; }

    public bool IsSuccess => _state == ResultState.Success;
    public bool IsFailure => _state == ResultState.Failure;
    public bool IsNull => _state == ResultState.Null;

    public Result(T value)
    {
        Value = value;
        Error = null!;
        _state = ResultState.Success;
    }

    public Result(Error error)
    {
        Value = default!;
        Error = error;
        _state = ResultState.Failure;
    }

    [Pure]
    public TR Match<TR>(Func<T, TR> onSuccess, Func<Error, TR> onFailure, Func<TR>? onNull = null)
    {
        if (IsSuccess) return onSuccess(Value);
        if (IsFailure) return onFailure(Error);
        if (onNull is not null) return onNull();
        throw new InvalidOperationException("Result is null, but no onNull function was provided.");
    }

    public static implicit operator Result<T>(T? value) => value is not null ? new Result<T>(value) : new Result<T>();
    public static implicit operator Result<T>(Error error) => new(error);
}
