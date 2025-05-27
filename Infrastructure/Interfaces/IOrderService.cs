using Domain.DTOs.Order;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IOrderService
{
    Task<Response<GetOrderDto>> CreateAsync(CreateOrderDto request);
    Task<Response<string>> DeleteAsync(int id, int userId);
    Task<PagedResponse<List<GetOrderDto>>> GetAllAsync(OrderFilter filter);
    Task<Response<GetOrderDto>> GetByIdAsync(int id, int userId);
    Task<Response<GetOrderDto>> UpdateAsync(int id, UpdateOrderDto request, int userId);
}
