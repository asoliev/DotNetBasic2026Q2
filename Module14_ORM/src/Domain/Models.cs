namespace Module14_ORM.Domain;

public enum OrderStatus
{
    NotStarted = 0,
    Loading = 1,
    InProgress = 2,
    Arrived = 3,
    Unloading = 4,
    Cancelled = 5,
    Done = 6,
}

public sealed class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Weight { get; set; }

    public decimal Height { get; set; }

    public decimal Width { get; set; }

    public decimal Length { get; set; }
}

public sealed class Order
{
    public int Id { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }
}

public sealed record OrderFilter(int? Month = null, OrderStatus? Status = null, int? Year = null, int? ProductId = null)
{
    public bool HasCriteria => Month.HasValue || Status.HasValue || Year.HasValue || ProductId.HasValue;
}