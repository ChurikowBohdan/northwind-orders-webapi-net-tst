using Microsoft.EntityFrameworkCore;

namespace Northwind.Services.EntityFramework.Entities;

[PrimaryKey(nameof(OrderID), nameof(ProductID))]
public class OrderDetail
{
    public int OrderID { get; set; }

    public int ProductID { get; init; }

    public double UnitPrice { get; init; }

    public long Quantity { get; init; }

    public double Discount { get; init; }

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
