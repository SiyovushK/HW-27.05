using Domain.DTOs.Category;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface ICategoryService
{
    Task<Response<GetCategoryDto>> CreateAsync(CreateCategoryDto request);
    Task<Response<GetCategoryDto>> UpdateAsync(int id, UpdateCategoryDto request);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<GetCategoryDto>> GetByIdAsync(int id);
    Task<PagedResponse<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter);
}