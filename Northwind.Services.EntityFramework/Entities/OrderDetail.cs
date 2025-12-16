namespace Northwind.Services.EntityFramework.Entities;

public class OrderDetail
{
    public int OrderId { get; set; }

    public int ProductId { get; init; }

    public double UnitPrice { get; init; }

    public long Quantity { get; init; }

    public double Discount { get; init; }

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
