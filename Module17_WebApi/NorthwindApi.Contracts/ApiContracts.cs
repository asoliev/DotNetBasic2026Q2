using System.ComponentModel.DataAnnotations;

namespace NorthwindApi.Contracts;

public sealed class CategoryUpsertRequest
{
    [Required]
    [StringLength(15)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }
}

public sealed class ProductUpsertRequest
{
    [Required]
    [StringLength(40)]
    public string ProductName { get; set; } = string.Empty;

    public int? SupplierId { get; set; }

    public int? CategoryId { get; set; }

    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? UnitPrice { get; set; }

    [Range(0, short.MaxValue)]
    public short? UnitsInStock { get; set; }

    [Range(0, short.MaxValue)]
    public short? UnitsOnOrder { get; set; }

    [Range(0, short.MaxValue)]
    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }
}

public sealed record CategoryDto(int CategoryId, string CategoryName, string? Description);

public sealed record ProductDto(
    int ProductId,
    string ProductName,
    int? SupplierId,
    int? CategoryId,
    string? QuantityPerUnit,
    decimal? UnitPrice,
    short? UnitsInStock,
    short? UnitsOnOrder,
    short? ReorderLevel,
    bool Discontinued);
