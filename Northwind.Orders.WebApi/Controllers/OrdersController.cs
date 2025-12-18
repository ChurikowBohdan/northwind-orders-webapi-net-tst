using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Repositories;
using ModelsAddOrder = Northwind.Orders.WebApi.Models.AddOrder;
using ModelsBriefOrder = Northwind.Orders.WebApi.Models.BriefOrder;
using ModelsFullOrder = Northwind.Orders.WebApi.Models.FullOrder;

namespace Northwind.Orders.WebApi.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    [HttpGet("{orderId:long}")]
    public async Task<ActionResult<ModelsFullOrder>> GetOrderAsync(long orderId)
    {
        try
        {
            _logger.LogTrace("Get order {OrderId}", orderId);

            var order = await _orderRepository.GetOrderAsync(orderId);

            return Ok(MapToFullOrder(order));
        }
        catch (OrderNotFoundException ex)
        {
            _logger.LogError(ex, "Order {OrderId} not found", orderId);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", orderId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModelsBriefOrder>>> GetOrdersAsync(int? skip, int? count)
    {
        try
        {
            var skipValue = skip ?? 0;
            var countValue = count ?? 10;

            if (skipValue < 0 || countValue <= 0)
            {
                _logger.LogError(
                    "Invalid skip or count value: skip={Skip}, count={Count}",
                    skipValue,
                    countValue);

                return BadRequest();
            }

            _logger.LogTrace("Get orders skip={Skip} count={Count}", skipValue, countValue);

            var orders = await _orderRepository.GetOrdersAsync(skipValue, countValue);

            return Ok(orders.Select(MapToBriefOrder));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }


    [HttpPost]
    public async Task<ActionResult<ModelsAddOrder>> AddOrderAsync(ModelsBriefOrder order)
    {
        try
        {
            _logger.LogTrace("Add order");

            var repositoryOrder = MapToRepositoryOrder(order);
            var id = await _orderRepository.AddOrderAsync(repositoryOrder);

            return Ok(new ModelsAddOrder { OrderId = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding order");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{orderId:long}")]
    public async Task<ActionResult> RemoveOrderAsync(long orderId)
    {
        try
        {
            _logger.LogTrace("Remove order {OrderId}", orderId);

            await _orderRepository.RemoveOrderAsync(orderId);
            return NoContent();
        }
        catch (OrderNotFoundException ex)
        {
            _logger.LogError(ex, "Order {OrderId} not found", orderId);
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing order {OrderId}", orderId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{orderId:long}")]
    public async Task<ActionResult> UpdateOrderAsync(long orderId, ModelsBriefOrder order)
    {
        try
        {
            this._logger.LogTrace("Update order {OrderId}", orderId);

            order.Id = orderId;
            var repositoryOrder = MapToRepositoryOrder(order);

            await this._orderRepository.UpdateOrderAsync(repositoryOrder);
            return this.NoContent();
        }
        catch (OrderNotFoundException ex)
        {
            this._logger.LogError(ex, "Order {OrderId} not found", orderId);
            return this.NotFound();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error updating order {OrderId}", orderId);
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private static ModelsBriefOrder MapToBriefOrder(Order order)
    {
        return new ModelsBriefOrder
        {
            Id = order.Id,
            CustomerId = order.Customer.Code.Code,
            EmployeeId = order.Employee.Id,
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            ShipperId = order.Shipper.Id,
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShipAddress = order.ShippingAddress.Address,
            ShipCity = order.ShippingAddress.City,
            ShipRegion = order.ShippingAddress.Region,
            ShipPostalCode = order.ShippingAddress.PostalCode,
            ShipCountry = order.ShippingAddress.Country,
            OrderDetails = new List<Models.BriefOrderDetail>(),
        };
    }

    private static ModelsFullOrder MapToFullOrder(Order order)
    {
        return new ModelsFullOrder
        {
            Id = order.Id,
            Customer = new Models.Customer
            {
                Code = order.Customer.Code.Code,
                CompanyName = order.Customer.CompanyName,
            },
            Employee = new Models.Employee
            {
                Id = order.Employee.Id,
                FirstName = order.Employee.FirstName,
                LastName = order.Employee.LastName,
                Country = order.Employee.Country,
            },
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Shipper = new Models.Shipper
            {
                Id = order.Shipper.Id,
                CompanyName = order.Shipper.CompanyName,
            },
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShippingAddress = new Models.ShippingAddress
            {
                Address = order.ShippingAddress.Address,
                City = order.ShippingAddress.City,
                Region = order.ShippingAddress.Region,
                PostalCode = order.ShippingAddress.PostalCode,
                Country = order.ShippingAddress.Country,
            },
            OrderDetails = order.OrderDetails.Select(d => new Models.FullOrderDetail
            {
                ProductId = d.Product.Id,
                ProductName = d.Product.ProductName,
                CategoryId = d.Product.CategoryId,
                CategoryName = d.Product.Category,
                SupplierId = d.Product.SupplierId,
                SupplierCompanyName = d.Product.Supplier,
                UnitPrice = d.UnitPrice,
                Quantity = d.Quantity,
                Discount = d.Discount,
            }).ToList(),
        };
    }

    private static Order MapToRepositoryOrder(ModelsBriefOrder order)
    {
        var result = new Order(order.Id)
        {
            Customer = new Customer(new CustomerCode(order.CustomerId)),
            Employee = new Employee(order.EmployeeId),
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Shipper = new Shipper(order.ShipperId),
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
            result.OrderDetails.Add(new OrderDetail(result)
            {
                Product = new Product(detail.ProductId),
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                Discount = detail.Discount,
            });
        }

        return result;
    }
}

