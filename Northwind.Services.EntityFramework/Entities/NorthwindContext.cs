using Microsoft.EntityFrameworkCore;
using Northwind.Services.Repositories;

namespace Northwind.Services.EntityFramework.Entities;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Shipper> Shippers { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

 protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // =========================
    // Customers
    // =========================
    modelBuilder.Entity<Customer>(entity =>
    {
        entity.ToTable("Customers");

        entity.HasKey(c => c.Id);

        entity.Property(c => c.Id)
              .HasColumnName("CustomerID");

        entity.Property(c => c.CompanyName)
              .HasColumnName("CompanyName");

        entity.HasMany(c => c.Orders)
              .WithOne(o => o.Customer)
              .HasForeignKey(o => o.CustomerId);
    });

    // =========================
    // Employees
    // =========================
    modelBuilder.Entity<Employee>(entity =>
    {
        entity.ToTable("Employees");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
              .HasColumnName("EmployeeID");

        entity.Property(e => e.FirstName)
              .HasColumnName("FirstName");

        entity.Property(e => e.LastName)
              .HasColumnName("LastName");

        entity.Property(e => e.Country)
              .HasColumnName("Country");

        entity.HasMany(e => e.Orders)
              .WithOne(o => o.Employee)
              .HasForeignKey(o => o.EmployeeId);
    });

    // =========================
    // Shippers
    // =========================
    modelBuilder.Entity<Shipper>(entity =>
    {
        entity.ToTable("Shippers");

        entity.HasKey(s => s.Id);

        entity.Property(s => s.Id)
              .HasColumnName("ShipperID");

        entity.Property(s => s.CompanyName)
              .HasColumnName("CompanyName");

        entity.HasMany(s => s.Orders)
              .WithOne(o => o.Shipper)
              .HasForeignKey(o => o.ShipperId);
    });

    // =========================
    // Orders
    // =========================
    modelBuilder.Entity<Order>(entity =>
    {
        entity.ToTable("Orders");

        entity.HasKey(o => o.Id);

        entity.Property(o => o.Id)
              .HasColumnName("OrderID");

        entity.Property(o => o.CustomerId)
              .HasColumnName("CustomerID");

        entity.Property(o => o.EmployeeId)
              .HasColumnName("EmployeeID");

        entity.Property(o => o.ShipperId)
              .HasColumnName("ShipVia");

        entity.Property(o => o.OrderDate)
              .HasColumnName("OrderDate");

        entity.Property(o => o.RequiredDate)
              .HasColumnName("RequiredDate");

        entity.Property(o => o.ShippedDate)
              .HasColumnName("ShippedDate");

        entity.Property(o => o.Freight)
              .HasColumnName("Freight");

        entity.Property(o => o.ShipName)
              .HasColumnName("ShipName");

        entity.Property(o => o.ShipAddress)
              .HasColumnName("ShipAddress");

        entity.Property(o => o.ShipCity)
              .HasColumnName("ShipCity");

        entity.Property(o => o.ShipRegion)
              .HasColumnName("ShipRegion");

        entity.Property(o => o.ShipPostalCode)
              .HasColumnName("ShipPostalCode");

        entity.Property(o => o.ShipCountry)
              .HasColumnName("ShipCountry");

        entity.HasMany(o => o.OrderDetails)
              .WithOne(od => od.Order)
              .HasForeignKey(od => od.OrderId);
    });

    // =========================
    // OrderDetails
    // =========================
    modelBuilder.Entity<OrderDetail>(entity =>
    {
        entity.ToTable("OrderDetails");

        entity.HasKey(od => new { od.OrderId, od.ProductId });

        entity.Property(od => od.OrderId)
              .HasColumnName("OrderID");

        entity.Property(od => od.ProductId)
              .HasColumnName("ProductID");

        entity.Property(od => od.UnitPrice)
              .HasColumnName("UnitPrice");

        entity.Property(od => od.Quantity)
              .HasColumnName("Quantity");

        entity.Property(od => od.Discount)
              .HasColumnName("Discount");
    });

    // =========================
    // Products
    // =========================
    modelBuilder.Entity<Product>(entity =>
    {
        entity.ToTable("Products");

        entity.HasKey(p => p.Id);

        entity.Property(p => p.Id)
              .HasColumnName("ProductID");

        entity.Property(p => p.ProductName)
              .HasColumnName("ProductName");

        entity.Property(p => p.SupplierId)
              .HasColumnName("SupplierID");

        entity.Property(p => p.CategoryId)
              .HasColumnName("CategoryID");

        entity.HasMany(p => p.OrderDetails)
              .WithOne(od => od.Product)
              .HasForeignKey(od => od.ProductId);

        entity.HasOne(p => p.Supplier)
              .WithMany(s => s.Products)
              .HasForeignKey(p => p.SupplierId);

        entity.HasOne(p => p.Category)
              .WithMany(c => c.Products)
              .HasForeignKey(p => p.CategoryId);
    });

    // =========================
    // Suppliers
    // =========================
    modelBuilder.Entity<Supplier>(entity =>
    {
        entity.ToTable("Suppliers");

        entity.HasKey(s => s.Id);

        entity.Property(s => s.Id)
              .HasColumnName("SupplierID");

        entity.Property(s => s.CompanyName)
              .HasColumnName("CompanyName");
    });

    // =========================
    // Categories
    // =========================
    modelBuilder.Entity<Category>(entity =>
    {
        entity.ToTable("Categories");

        entity.HasKey(c => c.Id);

        entity.Property(c => c.Id)
              .HasColumnName("CategoryID");

        entity.Property(c => c.Name)
              .HasColumnName("CategoryName");
    });
}
}
