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
        var order = await _context.Orders.FirstAsync(x => x.Id == orderId);

        if (order == null)
        {
            return null;
        }

        return new RepositoryOrder(order.Id)
        {
            Customer = new Services.Repositories.Customer()
            {
                
            },

        };
    }

    public Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        throw new NotImplementedException();
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
