namespace Northwind.Services.EntityFramework.Entities;

public class Customer
{
    public string CustomerID { get; private set; }

    public string CompanyName { get; set; } = default!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
