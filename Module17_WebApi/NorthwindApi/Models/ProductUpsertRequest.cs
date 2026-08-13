using System.ComponentModel.DataAnnotations;

namespace NorthwindApi.Models;

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