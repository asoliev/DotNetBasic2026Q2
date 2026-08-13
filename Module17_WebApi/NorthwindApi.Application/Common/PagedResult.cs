namespace NorthwindApi.Services.Common;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalItems, int PageNumber, int PageSize)
{
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
}