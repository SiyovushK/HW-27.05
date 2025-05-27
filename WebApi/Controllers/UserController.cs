using Domain.DTOs.User;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<Response<GetUserDto>> CreateAsync(CreateUserDto request)
    {
        return await userService.CreateAsync(request);
    }

    [HttpPut]
    [Authorize]
    public async Task<Response<GetUserDto>> UpdateAsync(int id, UpdateUserDto request)
    {
        return await userService.UpdateAsync(id, request);
    }

    [HttpDelete]
    [Authorize]
    public async Task<Response<string>> DeleteAsync(int id)
    {
        return await userService.DeleteAsync(id);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<PagedResponse<List<GetUserDto>>> GetAllAsync([FromQuery] UserFilter filter)
    {
        return await userService.GetAllAsync(filter);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<Response<GetUserDto>> GetByIdAsync(int id)
    {
        return await userService.GetByIdAsync(id);
    }
}