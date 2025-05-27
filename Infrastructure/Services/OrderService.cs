using System.Net;
using AutoMapper;
using Domain.DTOs.Order;
using Domain.Entities;
using Domain.Enums;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class OrderService(
    IBaseRepository<Order, int> repository,
    IBaseRepository<Product, int> productRepository,
    IMapper mapper,
    IMemoryCacheService memoryCache) : IOrderService
{
    private const string сacheKey = "orders";

    public async Task<Response<GetOrderDto>> CreateAsync(CreateOrderDto request)
    {
        var product = await productRepository.GetByAsync(request.ProductId);
        if (product == null)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Product not found!");

        var order = new Order
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            OrderDate = DateTime.UtcNow,
            Status = Status.OnTheWay,
            UserId = request.UserId
        };

        var result = await repository.AddAsync(order);
        if (result == 0)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Order not created!");

        await memoryCache.DeleteData(сacheKey);

        var addedOrder = await repository.GetByAsync(order.Id);
        var data = mapper.Map<GetOrderDto>(addedOrder);
        return new Response<GetOrderDto>(data);
    }

    public async Task<Response<string>> DeleteAsync(int id, int userId)
    {
        var order = await repository.GetByAsync(id);
        if (order == null)
            return new Response<string>(HttpStatusCode.NotFound, $"Order with id {id} not found");

        if (order.UserId != userId)
            return new Response<string>(HttpStatusCode.Forbidden, "You can only delete your own orders");

        var result = await repository.DeleteAsync(order);
        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Order not deleted!");

        await memoryCache.DeleteData(сacheKey);

        return new Response<string>("Order deleted successfully");
    }

    public async Task<PagedResponse<List<GetOrderDto>>> GetAllAsync(OrderFilter filter)
    {
        var ordersInCache = await memoryCache.GetData<List<GetOrderDto>>(сacheKey);

        if (ordersInCache == null)
        {
            var orders = await repository.GetAll();
            if (orders == null)
                return new PagedResponse<List<GetOrderDto>>(HttpStatusCode.NotFound, "No orders found");

            ordersInCache = mapper.Map<List<GetOrderDto>>(orders);
            await memoryCache.SetData(сacheKey, ordersInCache, 2);
        }

        if (filter.UserId != null)
            ordersInCache = ordersInCache.Where(o => o.UserId == filter.UserId.Value).ToList();

        if (filter.ProductId != null)
            ordersInCache = ordersInCache.Where(o => o.ProductId == filter.ProductId.Value).ToList();

        if (filter.Status != null)
            ordersInCache = ordersInCache.Where(o => o.Status == filter.Status).ToList();

        if (filter.FromDate != null)
            ordersInCache = ordersInCache.Where(o => o.OrderDate >= filter.FromDate.Value).ToList();

        if (filter.ToDate != null)
            ordersInCache = ordersInCache.Where(o => o.OrderDate <= filter.ToDate.Value).ToList();

        var totalRecords = ordersInCache.Count;

        var data = ordersInCache
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        return new PagedResponse<List<GetOrderDto>>(data, filter.PageNumber, filter.PageSize, totalRecords);
    }

    public async Task<Response<GetOrderDto>> GetByIdAsync(int id, int userId)
    {
        var order = await repository.GetByAsync(id);
        if (order == null)
            return new Response<GetOrderDto>(HttpStatusCode.NotFound, $"Order with id {id} not found");

        if (order.UserId != userId)
            return new Response<GetOrderDto>(HttpStatusCode.Forbidden, "You can only view your own orders");

        var data = mapper.Map<GetOrderDto>(order);
        return new Response<GetOrderDto>(data);
    }

    public async Task<Response<GetOrderDto>> UpdateAsync(int id, UpdateOrderDto request, int userId)
    {
        var order = await repository.GetByAsync(id);
        if (order == null)
            return new Response<GetOrderDto>(HttpStatusCode.NotFound, $"Order with id {id} not found");

        if (order.UserId != userId)
            return new Response<GetOrderDto>(HttpStatusCode.Forbidden, "You can only edit your own orders");

        var product = await productRepository.GetByAsync(order.ProductId);
        if (product == null)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Related product not found!");

        order.Quantity = request.Quantity;
        order.Status = request.Status;

        var result = await repository.UpdateAsync(order);
        if (result == 0)
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Order not updated!");

        await memoryCache.DeleteData(сacheKey);

        var updatedOrder = await repository.GetByAsync(id);
        var data = mapper.Map<GetOrderDto>(updatedOrder);
        return new Response<GetOrderDto>(data);
    }
}