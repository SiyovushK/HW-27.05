using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seeder;

public static class DefaultUser
{
    public static async Task SeedAsync(DataContext context, IPasswordHasher<User> passwordHasher)
    {
        var user = new User
        {
            Username = "Admin User",
            Email = "kurbonovs397@gmail.com",
            Role = Roles.Admin
        };
        
        user.PasswordHash = passwordHasher.HashPassword(user, "admin");
        
        var existingUser = await context.Users.FirstOrDefaultAsync(c => c.Email == user.Email);
        if (existingUser != null)
        {
            return;
        }

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}