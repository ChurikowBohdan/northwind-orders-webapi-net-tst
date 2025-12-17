namespace Northwind.Services.EntityFramework.Entities;

public class Supplier
{
    public int SupplierID { get; private set; }

    public string CompanyName { get; init; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
