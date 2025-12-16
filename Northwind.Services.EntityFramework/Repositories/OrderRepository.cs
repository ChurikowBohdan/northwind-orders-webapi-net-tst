using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFramework.Entities;
using Northwind.Services.Repositories;
using RepositoryOrder = Northwind.Services.Repositories.Order;

namespace Northwind.Services.EntityFramework.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly NorthwindContext _context;

    public OrderRepository(NorthwindContext context)
    {
        this._context = context;
    }

    public async Task<RepositoryOrder> GetOrderAsync(long orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Supplier)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            throw new OrderNotFoundException($"Order with id {orderId} was not found.");
        }

        var result = new RepositoryOrder(order.Id)
        {
            Customer = new Services.Repositories.Customer(new CustomerCode(order.CustomerId))
            {
                CompanyName = order.Customer.CompanyName,
            },
            Employee = new Services.Repositories.Employee(order.Employee.Id)
            {
                FirstName = order.Employee.FirstName,
                LastName = order.Employee.LastName,
                Country = order.Employee.Country,
            },
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Shipper = new Services.Repositories.Shipper(order.Shipper.Id)
            {
                CompanyName = order.Shipper.CompanyName,
            },
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShippingAddress = new ShippingAddress(
                order.ShipAddress,
                order.ShipCity,
                order.ShipRegion,
                order.ShipPostalCode,
                order.ShipCountry),
        };

        foreach (var detail in order.OrderDetails)
        {
            result.OrderDetails.Add(new Services.Repositories.OrderDetail(result)
            {
                Product = new Services.Repositories.Product(detail.Product.Id)
                {
                    ProductName = detail.Product.ProductName,
                    SupplierId = detail.Product.SupplierId,
                    Supplier = detail.Product.Supplier.CompanyName,
                    CategoryId = detail.Product.CategoryId,
                    Category = detail.Product.Category.Name,
                },
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                Discount = detail.Discount,
            });
        }

        return result;
    }

    public async Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip));
        }

        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .OrderBy(o => o.Id)
            .Skip(skip)
            .Take(count)
            .ToListAsync();

        var result = new List<RepositoryOrder>(orders.Count);

        foreach (var order in orders)
        {
            var repositoryOrder = new RepositoryOrder(order.Id)
            {
                Customer = new Services.Repositories.Customer(new CustomerCode(order.CustomerId))
                {
                    CompanyName = order.Customer.CompanyName,
                },
                Employee = new Services.Repositories.Employee(order.Employee.Id)
                {
                    FirstName = order.Employee.FirstName,
                    LastName = order.Employee.LastName,
                    Country = order.Employee.Country,
                },
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                Shipper = new Services.Repositories.Shipper(order.Shipper.Id)
                {
                    CompanyName = order.Shipper.CompanyName,
                },
                Freight = order.Freight,
                ShipName = order.ShipName,
                ShippingAddress = new ShippingAddress(
                    order.ShipAddress,
                    order.ShipCity,
                    order.ShipRegion,
                    order.ShipPostalCode,
                    order.ShipCountry)
            };

            result.Add(repositoryOrder);
        }

        return result;
    }

    public Task<long> AddOrderAsync(RepositoryOrder order)
    {
        throw new NotImplementedException();
    }

    public Task RemoveOrderAsync(long orderId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateOrderAsync(RepositoryOrder order)
    {
        throw new NotImplementedException();
    }
}
