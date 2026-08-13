using System.ComponentModel.DataAnnotations;

namespace NorthwindApi.Models;

public sealed class CategoryUpsertRequest
{
    [Required]
    [StringLength(15)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }
}