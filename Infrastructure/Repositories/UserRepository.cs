using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(DataContext context) : IBaseRepository<User, int>
{
    public async Task<int> AddAsync(User entity)
    {
        await context.Users.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(User entity)
    {
        context.Users.Remove(entity);
        return await context.SaveChangesAsync();
    }

    public Task<IQueryable<User>> GetAll()
    {
        var users = context.Users.AsQueryable();
        return Task.FromResult(users);
    }

    public async Task<User?> GetByAsync(int id)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<int> UpdateAsync(User entity)
    {
        context.Users.Update(entity);
        return await context.SaveChangesAsync();
    }
}
