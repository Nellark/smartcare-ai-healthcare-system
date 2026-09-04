namespace SmartCare.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<T> : Result
{
    public T Value { get; }

    protected Result(T value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static new Result<T> Failure(Error error) => new(default(T)!, false, error);
}

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("NullValue", "Null value provided");
    public static readonly Error InvalidInput = new("InvalidInput", "Invalid input provided");
    public static readonly Error NotFound = new("NotFound", "Entity not found");
    public static readonly Error Unauthorized = new("Unauthorized", "Unauthorized access");
}
