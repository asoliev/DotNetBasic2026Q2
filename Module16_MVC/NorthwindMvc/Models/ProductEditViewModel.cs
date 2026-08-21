using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NorthwindMvc.Models;

public sealed class ProductEditViewModel
{
    public int ProductID { get; set; }

    [Required]
    [StringLength(40, MinimumLength = 3)]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Select a supplier.")]
    public int? SupplierID { get; set; }

    [Required(ErrorMessage = "Select a category.")]
    public int? CategoryID { get; set; }

    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
    [Range(typeof(decimal), "0", "999999")]
    public decimal? UnitPrice { get; set; }

    [Range(typeof(short), "0", "32767")]
    public short? UnitsInStock { get; set; }

    [Range(typeof(short), "0", "32767")]
    public short? UnitsOnOrder { get; set; }

    [Range(typeof(short), "0", "32767")]
    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    public IEnumerable<SelectListItem> Suppliers { get; set; } = [];

    public IEnumerable<SelectListItem> Categories { get; set; } = [];
}
