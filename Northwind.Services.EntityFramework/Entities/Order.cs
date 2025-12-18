using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFramework.Entities;

public class Order
{
    public int OrderID { get; private set; }

    public string CustomerID { get; set; } = default!;

    public int EmployeeID { get; set; }

    [Column("ShipVia")]
    public int ShipperID { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime RequiredDate { get; set; }

    public DateTime? ShippedDate { get; set; }

    public double Freight { get; set; }

    public string ShipName { get; set; } = default!;

    public string ShipAddress { get; set; } = default!;

    public string ShipCity { get; set; } = default!;

    public string? ShipRegion { get; set; }

    public string ShipPostalCode { get; set; } = default!;

    public string ShipCountry { get; set; } = default!;

    public Customer Customer { get; set; } = null!;

    public Employee Employee { get; set; } = null!;

    public Shipper Shipper { get; set; } = null!;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
