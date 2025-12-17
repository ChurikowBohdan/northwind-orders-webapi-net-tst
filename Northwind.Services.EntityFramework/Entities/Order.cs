using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Order
{
    public int OrderID { get; private set; }

    public string CustomerID { get; set; } = default!;

    public int EmployeeID { get; set; }

    [Column("ShipVia")]
    public int ShipperID { get; set; }

    public DateTime OrderDate { get; init; }

    public DateTime RequiredDate { get; init; }

    public DateTime? ShippedDate { get; init; }

    public double Freight { get; init; }

    public string ShipName { get; init; } = default!;

    public string ShipAddress { get; init; } = default!;

    public string ShipCity { get; init; } = default!;

    public string? ShipRegion { get; init; }

    public string ShipPostalCode { get; init; } = default!;

    public string ShipCountry { get; init; } = default!;

    public Customer Customer { get; set; } = null!;

    public Employee Employee { get; set; } = null!;

    public Shipper Shipper { get; set; } = null!;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
