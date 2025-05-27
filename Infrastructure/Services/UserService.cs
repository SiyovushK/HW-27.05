using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.User;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Infrastructure.Services;

public class UserService(
    IBaseRepository<User, int> repository,
    IMapper mapper,
    IPasswordHasher<User> passwordHasher,
    IMemoryCacheService memoryCache,
    IEmailService emailService) : IUserService
{
    public async Task<Response<GetUserDto>> CreateAsync(CreateUserDto request)
    {
        var user = mapper.Map<User>(request);

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        var result = await repository.AddAsync(user);

        if (result == 0)
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, "User not added!");

        await memoryCache.DeleteData("users");

        var emailDto = new EmailDTO()
        {
            To = user.Email,
            Subject = "Account info",
            Body = $"Hi {user.Username}! Your registration has been successfull."
        };

        await emailService.SendEmailAsync(emailDto);

        var data = mapper.Map<GetUserDto>(user);

        return new Response<GetUserDto>(data);
    }

    public async Task<Response<string>> DeleteAsync(int Id)
    {
        var user = await repository.GetByAsync(Id);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, $"User with id {Id} not found");

        var result = await repository.DeleteAsync(user);
        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "User not deleted!");

        await memoryCache.DeleteData("users");

        var emailDto = new EmailDTO()
        {
            To = user.Email,
            Subject = "Account info",
            Body = $"Hi {user.Username}! Your account has been successfully deleted."
        };

        await emailService.SendEmailAsync(emailDto);

        return new Response<string>("User deleted successfully");
    }

    public async Task<PagedResponse<List<GetUserDto>>> GetAllAsync(UserFilter filter)
    {
        if (filter.PageNumber <= 0) filter.PageNumber = 1;
        if (filter.PageSize <= 0) filter.PageSize = 10;

        const string cacheKey = "users";

        var usersInCache = await memoryCache.GetData<List<GetUserDto>>(cacheKey);

        if (usersInCache == null)
        {
            var users = await repository.GetAll();

            if (users == null || !users.Any())
            {
                return new PagedResponse<List<GetUserDto>>(HttpStatusCode.NotFound, "No users available");
            }

            usersInCache = users.Select(u => new GetUserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            }).ToList();

            await memoryCache.SetData(cacheKey, usersInCache, 2);
        }

        if (!string.IsNullOrWhiteSpace(filter.UserName))
            usersInCache = usersInCache.Where(u => u.Username.ToLower().Trim().Contains(filter.UserName.ToLower().Trim())).ToList();

        if (!string.IsNullOrWhiteSpace(filter.Email))
            usersInCache = usersInCache.Where(u => u.Email.ToLower().Trim().Contains(filter.Email.ToLower().Trim())).ToList();

        var totalRecords = usersInCache.Count;

        var data = usersInCache   
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        return new PagedResponse<List<GetUserDto>>(data, filter.PageNumber, filter.PageSize, totalRecords);
    }

    public async Task<Response<GetUserDto>> GetByIdAsync(int Id)
    {
        var user = await repository.GetByAsync(Id);
        if (user == null)
            return new Response<GetUserDto>(HttpStatusCode.NotFound, $"User with id {Id} not found");

        var data = mapper.Map<GetUserDto>(user);
        return new Response<GetUserDto>(data);
    }

    public async Task<Response<GetUserDto>> UpdateAsync(int Id, UpdateUserDto request)
    {
        var user = await repository.GetByAsync(Id);
        if (user == null)
            return new Response<GetUserDto>(HttpStatusCode.NotFound, $"User with id {Id} not found");

        user.Username = request.Username;
        user.Email = request.Email;

        var result = await repository.UpdateAsync(user);
        if (result == 0)
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, "User not updated!");

        await memoryCache.DeleteData("users");    

        var emailDto = new EmailDTO()
        {
            To = user.Email,
            Subject = "Account info",
            Body = $"Hi {user.Username}! Your account has been updated successfully."
        };

        await emailService.SendEmailAsync(emailDto);

        var data = mapper.Map<GetUserDto>(user);

        return new Response<GetUserDto>(data);
    }
}
