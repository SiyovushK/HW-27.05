using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(DataContext context) : IBaseRepository<Category, int>
{
    public async Task<int> AddAsync(Category entity)
    {
        await context.Categories.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Category entity)
    {
        context.Categories.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public Task<IQueryable<Category>> GetAll()
    {
        return Task.FromResult(context.Categories.AsQueryable());
    }

    public async Task<Category?> GetByAsync(int id)
    {
        return await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> UpdateAsync(Category entity)
    {
        context.Categories.Update(entity);
        return await context.SaveChangesAsync();
    }
}