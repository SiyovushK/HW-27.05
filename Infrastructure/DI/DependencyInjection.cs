using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Repositories;

namespace Infrastructure.DI;

public static class DependencyInjection
{
   public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBaseRepository<Category, int>, CategoryRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBaseRepository<Product, int>, ProductRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBaseRepository<User, int>, UserRepository>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IBaseRepository<Order, int>, OrderRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBaseRepository<Product, int>, ProductRepository>();
        services.AddScoped(typeof(IAuthRepository<>), typeof(AuthRepository<>));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMemoryCacheService, MemoryCacheService>();        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
    }
}