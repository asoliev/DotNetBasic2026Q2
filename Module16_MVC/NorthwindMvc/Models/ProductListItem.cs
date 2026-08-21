namespace NorthwindMvc.Models;

public sealed record ProductListItem(
    int ProductID,
    string ProductName,
    string SupplierName,
    string? CategoryName,
    string? QuantityPerUnit,
    decimal? UnitPrice,
    short? UnitsInStock,
    short? UnitsOnOrder,
    short? ReorderLevel,
    bool Discontinued);
