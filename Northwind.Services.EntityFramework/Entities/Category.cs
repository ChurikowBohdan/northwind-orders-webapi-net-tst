namespace Northwind.Services.EntityFramework.Entities;

public class Category
{
    public int CategoryID { get; private set; }

    public string CategoryName { get; set; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
