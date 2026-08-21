namespace NorthwindApi.Services.Common;

public sealed record ServiceResult<T>(
    ServiceResultStatus Status,
    T? Value = default,
    string? Message = null,
    IReadOnlyDictionary<string, string[]>? Errors = null)
{
    public static ServiceResult<T> Success(T value) => new(ServiceResultStatus.Success, value);

    public static ServiceResult<T> NotFound() => new(ServiceResultStatus.NotFound);

    public static ServiceResult<T> Conflict(string message) => new(ServiceResultStatus.Conflict, default, message);

    public static ServiceResult<T> ValidationFailed(IReadOnlyDictionary<string, string[]> errors)
        => new(ServiceResultStatus.ValidationFailed, default, null, new Dictionary<string, string[]>(errors));
}
