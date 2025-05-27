using System.Security.Authentication.ExtendedProtection;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(DataContext context) : IBaseRepository<Product, int>
{
    public async Task<int> AddAsync(Product entity)
    {
        await context.Products.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Product entity)
    {
        context.Products.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public Task<IQueryable<Product>> GetAll()
    {
        return Task.FromResult(context.Products
            .Include(p => p.Category)
            .Include(p => p.User)
            .AsQueryable());
    }

    public async Task<Product?> GetByAsync(int id)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<int> UpdateAsync(Product entity)
    {
        context.Products.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetExpiredPromotionsAsync()
    {
        return await context.Products
            .Where(p => (p.IsTop || p.IsPremium) && 
                       p.PremiumOrTopExpiryDate.HasValue && 
                       p.PremiumOrTopExpiryDate <= DateTime.UtcNow)
            .Include(p => p.User)
            .ToListAsync();
    }
}