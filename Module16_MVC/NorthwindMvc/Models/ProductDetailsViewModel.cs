namespace NorthwindMvc.Models;

public sealed class ProductDetailsViewModel
{
    public int ProductID { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public string SupplierName { get; init; } = string.Empty;

    public string? CategoryName { get; init; }

    public string? QuantityPerUnit { get; init; }

    public decimal? UnitPrice { get; init; }

    public short? UnitsInStock { get; init; }

    public short? UnitsOnOrder { get; init; }

    public short? ReorderLevel { get; init; }

    public bool Discontinued { get; init; }
}