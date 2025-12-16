namespace Northwind.Services.EntityFramework.Entities;

public class Employee
{
    public int Id { get; private set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Country { get; set; } = default!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
