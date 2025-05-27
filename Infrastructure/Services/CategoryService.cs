using System.Net;
using AutoMapper;
using Domain.DTOs.Category;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class CategoryService(
    IBaseRepository<Category, int> repository,
    IMapper mapper,
    IMemoryCacheService memoryCache) : ICategoryService
{
    private const string cacheKey = "categories";

    public async Task<Response<GetCategoryDto>> CreateAsync(CreateCategoryDto request)
    {
        var category = mapper.Map<Category>(request);

        var result = await repository.AddAsync(category);
        if (result == 0)
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category not added!");

        await memoryCache.DeleteData(cacheKey);

        var data = mapper.Map<GetCategoryDto>(category);
        return new Response<GetCategoryDto>(data);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var category = await repository.GetByAsync(id);
        if (category == null)
            return new Response<string>(HttpStatusCode.NotFound, $"Category with id {id} not found");

        var result = await repository.DeleteAsync(category);
        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Category not deleted!");

        await memoryCache.DeleteData(cacheKey);

        return new Response<string>("Category deleted successfully");
    }

    public async Task<PagedResponse<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter)
    {
        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var categoriesInCache = await memoryCache.GetData<List<GetCategoryDto>>(cacheKey);
        if (categoriesInCache == null)
        {
            var categories = await repository.GetAll();
            categoriesInCache = mapper.Map<List<GetCategoryDto>>(categories);
            await memoryCache.SetData(cacheKey, categoriesInCache, 2);
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var nameFilter = filter.Name.ToLower().Trim();
            categoriesInCache = categoriesInCache
                .Where(c => c.Name.ToLower().Trim().Contains(nameFilter))
                .ToList();
        }

        var totalRecords = categoriesInCache.Count;

        var data = categoriesInCache
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToList();

        return new PagedResponse<List<GetCategoryDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);
    }

    public async Task<Response<GetCategoryDto>> GetByIdAsync(int id)
    {
        var category = await repository.GetByAsync(id);
        if (category == null)
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, $"Category with id {id} not found");

        var data = mapper.Map<GetCategoryDto>(category);
        return new Response<GetCategoryDto>(data);
    }

    public async Task<Response<GetCategoryDto>> UpdateAsync(int id, UpdateCategoryDto request)
    {
        var category = await repository.GetByAsync(id);
        if (category == null)
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, $"Category with id {id} not found");

        category.Name = request.Name;
        category.Description = request.Description;

        var result = await repository.UpdateAsync(category);
        if (result == 0)
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category not updated!");

        await memoryCache.DeleteData(cacheKey);

        var data = mapper.Map<GetCategoryDto>(category);
        return new Response<GetCategoryDto>(data);
    }
}