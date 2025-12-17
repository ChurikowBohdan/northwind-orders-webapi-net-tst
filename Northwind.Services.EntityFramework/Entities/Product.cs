namespace Northwind.Services.EntityFramework.Entities;

public class Product
{
    public int ProductID { get; private init; }

    public int SupplierID { get; init; }

    public int CategoryID { get; init; }

    public string ProductName { get; init; } = default!;

    public Supplier Supplier { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
