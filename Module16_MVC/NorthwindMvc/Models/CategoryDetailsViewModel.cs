namespace NorthwindMvc.Models;

public sealed record CategoryDetailsViewModel
{
    public int CategoryID { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? PictureDataUrl { get; init; }
}