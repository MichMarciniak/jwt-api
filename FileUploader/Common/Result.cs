namespace FileUploader.Common;

public record Result<T>(
    T? Value,
    bool IsSuccess,
    Error Error
    )
{
    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static Result<T> Failure(Error error) => new (default, false, error);
}