namespace Northwind.Services.EntityFramework.Entities;

public class Category
{
    public int Id { get; private set; }

    public string Name { get; set; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
