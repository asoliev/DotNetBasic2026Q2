namespace AdoNetLibrary.Models;

public sealed class OrderFilter
{
    public int? Month { get; set; }

    public int? Year { get; set; }

    public OrderStatus? Status { get; set; }

    public int? ProductId { get; set; }
}
