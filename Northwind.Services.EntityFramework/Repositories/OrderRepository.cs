using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFramework.Entities;
using Northwind.Services.Repositories;
using RepositoryOrder = Northwind.Services.Repositories.Order;

namespace Northwind.Services.EntityFramework.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly NorthwindContext dBcontext;

    public OrderRepository(NorthwindContext context)
    {
        this.dBcontext = context;
    }

    public async Task<RepositoryOrder> GetOrderAsync(long orderId)
    {
        var order = await this.dBcontext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Supplier)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.OrderID == orderId);

        if (order == null)
        {
            throw new OrderNotFoundException($"Order with id {orderId} was not found.");
        }

        return MapToRepositoryOrder(order, includeDetails: true);
    }

    public async Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(skip);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

        var orders = await this.dBcontext.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .OrderBy(o => o.OrderID)
            .Skip(skip)
            .Take(count)
            .ToListAsync();

        return orders
            .Select(o => MapToRepositoryOrder(o, includeDetails: false))
            .ToList();
    }

    public async Task<long> AddOrderAsync(RepositoryOrder order)
    {
        try
        {
            var entity = MapToEntity(order);

            this.dBcontext.Orders.Add(entity);
            await this.dBcontext.SaveChangesAsync();

            return entity.OrderID;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Failed to add order.", ex);
        }
    }

    public async Task RemoveOrderAsync(long orderId)
    {
        var order = await this.dBcontext.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderID == orderId);

        if (order == null)
        {
            throw new OrderNotFoundException($"Order with id {orderId} was not found.");
        }

        this.dBcontext.OrderDetails.RemoveRange(order.OrderDetails);
        this.dBcontext.Orders.Remove(order);

        await this.dBcontext.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(RepositoryOrder order)
    {
        var entity = await this.dBcontext.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderID == order.Id);

        if (entity == null)
        {
            throw new OrderNotFoundException($"Order with id {order.Id} was not found.");
        }

        UpdateEntity(entity, order);

        await this.dBcontext.SaveChangesAsync();
    }

    private static RepositoryOrder MapToRepositoryOrder(Entities.Order order, bool includeDetails)
    {
        var result = new RepositoryOrder(order.OrderID)
        {
            Customer = new Services.Repositories.Customer(
                new CustomerCode(order.CustomerID))
            {
                CompanyName = order.Customer.CompanyName,
            },
            Employee = new Services.Repositories.Employee(order.Employee.EmployeeID)
            {
                FirstName = order.Employee.FirstName,
                LastName = order.Employee.LastName,
                Country = order.Employee.Country,
            },
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Shipper = new Services.Repositories.Shipper(order.Shipper.ShipperID)
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

        if (!includeDetails)
        {
            return result;
        }

        foreach (var detail in order.OrderDetails)
        {
            result.OrderDetails.Add(MapToRepositoryOrderDetail(result, detail));
        }

        return result;
    }

    private static Services.Repositories.OrderDetail MapToRepositoryOrderDetail(RepositoryOrder order, Entities.OrderDetail detail)
    {
        return new Services.Repositories.OrderDetail(order)
        {
            Product = new Services.Repositories.Product(detail.Product.ProductID)
            {
                ProductName = detail.Product.ProductName,
                SupplierId = detail.Product.SupplierID,
                Supplier = detail.Product.Supplier.CompanyName,
                CategoryId = detail.Product.CategoryID,
                Category = detail.Product.Category.CategoryName,
            },
            UnitPrice = detail.UnitPrice,
            Quantity = detail.Quantity,
            Discount = detail.Discount,
        };
    }

    private static Entities.Order MapToEntity(RepositoryOrder order)
    {
        var entity = new Entities.Order
        {
            CustomerID = order.Customer.Code.Code,
            EmployeeID = (int)order.Employee.Id,
            ShipperID = (int)order.Shipper.Id,
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShipAddress = order.ShippingAddress.Address,
            ShipCity = order.ShippingAddress.City,
            ShipRegion = order.ShippingAddress.Region,
            ShipPostalCode = order.ShippingAddress.PostalCode,
            ShipCountry = order.ShippingAddress.Country,
        };

        foreach (var detail in order.OrderDetails)
        {
            entity.OrderDetails.Add(new Entities.OrderDetail
            {
                ProductID = (int)detail.Product.Id,
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                Discount = detail.Discount,
            });
        }

        return entity;
    }

    private static void UpdateEntity(Entities.Order entity, RepositoryOrder order)
    {
        entity.CustomerID = order.Customer.Code.Code;
        entity.EmployeeID = (int)order.Employee.Id;
        entity.ShipperID = (int)order.Shipper.Id;
        entity.OrderDate = order.OrderDate;
        entity.RequiredDate = order.RequiredDate;
        entity.ShippedDate = order.ShippedDate;
        entity.Freight = order.Freight;
        entity.ShipName = order.ShipName;
        entity.ShipAddress = order.ShippingAddress.Address;
        entity.ShipCity = order.ShippingAddress.City;
        entity.ShipRegion = order.ShippingAddress.Region;
        entity.ShipPostalCode = order.ShippingAddress.PostalCode;
        entity.ShipCountry = order.ShippingAddress.Country;

        entity.OrderDetails.Clear();

        foreach (var detail in order.OrderDetails)
        {
            entity.OrderDetails.Add(new Entities.OrderDetail
            {
                OrderID = entity.OrderID,
                ProductID = (int)detail.Product.Id,
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                Discount = detail.Discount,
            });
        }
    }
}
