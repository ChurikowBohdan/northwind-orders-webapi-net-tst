namespace Northwind.Services.EntityFramework.Entities;

public class Shipper
{
    public int Id { get; private set; }

    public string CompanyName { get; set; } = default!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
