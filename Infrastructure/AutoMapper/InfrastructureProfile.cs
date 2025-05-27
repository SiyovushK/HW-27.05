using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Category;
using Domain.DTOs.Order;
using Domain.DTOs.Product;
using Domain.DTOs.User;
using Domain.Entities;

namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        CreateMap<Product, GetProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        CreateMap<Order, GetOrderDto>();
        CreateMap<CreateOrderDto, Order>();
        CreateMap<UpdateOrderDto, Order>();

        CreateMap<Category, GetCategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();
        
        CreateMap<User, GetUserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
        CreateMap<UpdateUserDto, User>();
    }
}