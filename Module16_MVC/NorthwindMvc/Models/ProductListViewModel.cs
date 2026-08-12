using Microsoft.AspNetCore.Mvc.Rendering;

namespace NorthwindMvc.Models;

public sealed class ProductListViewModel
{
    public IReadOnlyList<ProductListItem> Products { get; init; } = [];

    public int RowLimit { get; init; }

    public IReadOnlyList<SelectListItem> RowLimitOptions { get; init; } = [];
}