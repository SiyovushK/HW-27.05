using Domain.DTOs.Auth;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<Response<string>> Register(RegisterDTO registerDto)
    {
        return await authService.Register(registerDto);
    }
    
    [HttpPost("login")]
    public async Task<Response<TokenDTO>> Login(LoginDTO loginDto)
    {
        return await authService.Login(loginDto);
    }
    
    [HttpPost("reset-password")]
    public async Task<Response<string>> ResetPassword(ResetPasswordDTO resetPasswordDto)
    {
        return await authService.ResetPassword(resetPasswordDto);
    }
    
    [HttpPost("request-reset-password")]
    public async Task<Response<string>> RequestResetPassword(RequestResetPasswordDTO resetPasswordDto)
    {
        return await authService.RequestResetPassword(resetPasswordDto);
    }
}