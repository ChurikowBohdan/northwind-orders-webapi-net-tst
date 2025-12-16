namespace Northwind.Services.EntityFramework.Entities;

public class Order
{
    public long Id { get; private set; }

    public string CustomerId { get; set; } = default!;

    public int EmployeeId { get; set; }

    public int ShiperId { get; set; }

    public DateTime OrderDate { get; init; }

    public DateTime RequiredDate { get; init; }

    public DateTime? ShippedDate { get; init; }

    public double Freight { get; init; }

    public string ShipName { get; init; } = default!;

    public string ShipAddress { get; init; } = default!;

    public string ShipCity { get; init; } = default!;

    public string ShipRegion { get; init; } = default!;

    public string ShipPostalCode { get; init; } = default!;

    public string ShipCountry { get; init; } = default!;

    public Customer Customer { get; set; } = null!;

    public Employee Employee { get; set; } = null!;

    public Shipper Shipper { get; set; } = null!;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
