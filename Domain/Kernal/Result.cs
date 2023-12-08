namespace Domain.Kernal;

public readonly record struct Result<T> : IResult
{

    private readonly T? _value = default;
    private readonly List<Error>? _errors = null;

    public bool IsError { get; }

    private static readonly Error NoErrors = Error.Unexpected(
        code: "ErrorOr.NoErrors",
        description: "Error list cannot be retrieved from a successful Result.");

    public T Value => _value!;

    public static Result<T> From(List<Error> errors)
    {
        return errors;
    }

    public List<Error> Errors => IsError ? _errors! : [NoErrors];

    private Result(Error error)
    {
        _errors = [error];
        IsError = true;
    }

    private Result(List<Error> errors)
    {
        _errors = errors;
        IsError = true;
    }

    private Result(T value)
    {
        _value = value;
        IsError = false;
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Error error)
    {
        return new Result<T>(error);
    }

    public static implicit operator Result<T>(List<Error> errors)
    {
        return new Result<T>(errors);
    }

    public static implicit operator Result<T>(Error[] errors)
    {
        return new Result<T>([.. errors]);
    }

    public TResult Match<TResult>(Func<T, TResult> onValue, Func<List<Error>, TResult> onError)
    {
        if (IsError)
        {
            return onError(Errors);
        }

        return onValue(Value);
    }

    public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> onValue, Func<List<Error>, Task<TResult>> onError)
    {
        if (IsError)
        {
            return await onError(Errors).ConfigureAwait(false);
        }

        return await onValue(Value).ConfigureAwait(false);
    }

    public void Switch(Action<T> onValue, Action<List<Error>> onError)
    {
        if (IsError)
        {
            onError(Errors);
            return;
        }

        onValue(Value);
    }

    public async Task SwitchAsync(Func<T, Task> onValue, Func<List<Error>, Task> onError)
    {
        if (IsError)
        {
            await onError(Errors).ConfigureAwait(false);
            return;
        }

        await onValue(Value).ConfigureAwait(false);
    }

}

// Utilities Class
public static class Result
{
    public static Result<TValue> From<TValue>(TValue value)
    {
        return value;
    }

    public static Deleted Created => default;
    public static Deleted Success => default;
    public static Deleted Deleted => default;
    public static Deleted Updated => default;


}

public readonly record struct Success;
public readonly record struct Created;
public readonly record struct Deleted;
public readonly record struct Updated;
