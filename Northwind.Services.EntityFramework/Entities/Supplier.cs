namespace Northwind.Services.EntityFramework.Entities;

public class Supplier
{
    public int Id { get; private set; }

    public string CompanyName { get; init; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
