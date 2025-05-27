using System.Net;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class ProductService(
    IBaseRepository<Product, int> repository,
    IBaseRepository<Category, int> categoryRepository,
    IMapper mapper,
    IMemoryCacheService memoryCache) : IProductService
{
    public async Task<Response<GetProductDto>> CreateAsync(CreateProductDto request, int userId)
    {
        var category = await categoryRepository.GetByAsync(request.CategoryId);
        if (category == null)
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Category not found!");

        var product = mapper.Map<Product>(request);
        product.UserId = userId;

        var result = await repository.AddAsync(product);
        if (result == 0)
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product not added!");

        await memoryCache.DeleteData("products");

        var data = mapper.Map<GetProductDto>(product);
        return new Response<GetProductDto>(data);
    }

    public async Task<Response<string>> DeleteAsync(int id, int userId, string userRole)
    {
        var product = await repository.GetByAsync(id);
        if (product == null)
            return new Response<string>(HttpStatusCode.NotFound, $"Product with id {id} not found");

        if (product.UserId != userId)
            return new Response<string>(HttpStatusCode.Forbidden, "You can only delete your own products");

        var result = await repository.DeleteAsync(product);
        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Product not deleted!");

        await memoryCache.DeleteData("products");

        return new Response<string>("Product deleted successfully");
    }

    public async Task<PagedResponse<List<GetProductDto>>> GetAllAsync(ProductFilter filter)
    {
        const string cacheKey = "products";

        var productsInCache = await memoryCache.GetData<List<GetProductDto>>(cacheKey);

        if (productsInCache == null)
        {
            var products = await repository.GetAll();

            if (products == null)
                return new PagedResponse<List<GetProductDto>>(HttpStatusCode.NotFound, "No products available");

            productsInCache = mapper.Map<List<GetProductDto>>(products);

            await memoryCache.SetData(cacheKey, productsInCache, 2);
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
            productsInCache = productsInCache
                .Where(p => p.Name.ToLower().Trim().Contains(filter.Name.ToLower().Trim()))
                .ToList();

        if (filter.CategoryId != null)
            productsInCache = productsInCache
                .Where(p => p.CategoryId == filter.CategoryId.Value)
                .ToList();

        if (filter.UserId != null)
            productsInCache = productsInCache
                .Where(p => p.UserId == filter.UserId.Value)
                .ToList();

        if (filter.IsTop != null)
            productsInCache = productsInCache
                .Where(p => p.IsTop == filter.IsTop.Value)
                .ToList();

        if (filter.IsPremium != null)
            productsInCache = productsInCache
                .Where(p => p.IsPremium == filter.IsPremium.Value)
                .ToList();

        if (filter.PriceFrom != null)
            productsInCache = productsInCache
                .Where(p => p.Price >= filter.PriceFrom.Value)
                .ToList();

        if (filter.PriceTo != null)
            productsInCache = productsInCache
                .Where(p => p.Price <= filter.PriceTo.Value)
                .ToList();

        var totalRecords = productsInCache.Count;

        var data = productsInCache
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        return new PagedResponse<List<GetProductDto>>(data, filter.PageNumber, filter.PageSize, totalRecords);
    }

    public async Task<Response<GetProductDto>> GetByIdAsync(int id)
    {
        var product = await repository.GetByAsync(id);
        if (product == null)
            return new Response<GetProductDto>(HttpStatusCode.NotFound, $"Product with id {id} not found");

        var data = mapper.Map<GetProductDto>(product);
        return new Response<GetProductDto>(data);
    }

    public async Task<Response<GetProductDto>> UpdateAsync(int id, UpdateProductDto request, int userId, string userRole)
    {
        var product = await repository.GetByAsync(id);
        if (product == null)
            return new Response<GetProductDto>(HttpStatusCode.NotFound, $"Product with id {id} not found");

        if (product.UserId != userId)
            return new Response<GetProductDto>(HttpStatusCode.Forbidden, "You can only edit your own products");

        var category = await categoryRepository.GetByAsync(request.CategoryId);
        if (category == null)
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Category not found!");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        var result = await repository.UpdateAsync(product);
        if (result == 0)
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product not updated!");

        await memoryCache.DeleteData("products");

        var data = mapper.Map<GetProductDto>(product);
        return new Response<GetProductDto>(data);
    }

    public async Task<Response<GetProductDto>> PromoteProductAsync(int id, PromoteProductDto request, int userId, string userRole)
    {
        var product = await repository.GetByAsync(id);
        if (product == null)
            return new Response<GetProductDto>(HttpStatusCode.NotFound, $"Product with id {id} not found");

        if (product.UserId != userId)
            return new Response<GetProductDto>(HttpStatusCode.Forbidden, "You can only promote your own products");

        if (request.SetTop) product.IsTop = true;
        if (request.SetPremium) product.IsPremium = true;

        if (request.SetTop || request.SetPremium)
            product.PremiumOrTopExpiryDate = DateTime.UtcNow.AddDays(request.DurationInDays);

        var result = await repository.UpdateAsync(product);
        if (result == 0)
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product promotion failed!");

        await memoryCache.DeleteData("products");

        var data = mapper.Map<GetProductDto>(product);
        return new Response<GetProductDto>(data);
    }
}
