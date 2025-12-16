namespace Northwind.Services.EntityFramework.Entities;

public class Product
{
    public int Id { get; private init; }

    public int SupplierId { get; init; }

    public int CategoryId { get; init; }

    public string ProductName { get; init; } = default!;

    public Supplier Supplier { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
