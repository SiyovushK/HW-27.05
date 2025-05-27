using Domain.DTOs.Product;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task<Response<GetProductDto>> CreateAsync(CreateProductDto request, int userId);
    Task<Response<string>> DeleteAsync(int id, int userId, string userRole);
    Task<PagedResponse<List<GetProductDto>>> GetAllAsync(ProductFilter filter);
    Task<Response<GetProductDto>> GetByIdAsync(int id);
    Task<Response<GetProductDto>> UpdateAsync(int id, UpdateProductDto request, int userId, string userRole);
    Task<Response<GetProductDto>> PromoteProductAsync(int id, PromoteProductDto request, int userId, string userRole);
}
