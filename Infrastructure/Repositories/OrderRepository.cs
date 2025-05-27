using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository(DataContext context) : IBaseRepository<Order, int>
{
    public async Task<int> AddAsync(Order entity)
    {
        await context.Orders.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Order entity)
    {
        context.Orders.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public Task<IQueryable<Order>> GetAll()
    {
        return Task.FromResult(context.Orders
            .Include(o => o.User)
            .Include(o => o.Products)
            .AsQueryable());
    }

    public async Task<Order?> GetByAsync(int id)
    {
        return await context.Orders
            .Include(o => o.User)
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<int> UpdateAsync(Order entity)
    {
        context.Orders.Update(entity);
        return await context.SaveChangesAsync();
    }
}