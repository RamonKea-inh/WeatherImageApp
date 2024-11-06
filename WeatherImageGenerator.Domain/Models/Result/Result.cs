public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, T? data, string? error = null, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T data) => new(true, data);
    public static Result<T> Failure(string error, string errorCode = "ERROR") =>
        new(false, default, error, errorCode);
}
