using Domain.DTOs.Auth;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<Response<TokenDTO>> Login(LoginDTO loginDto);
    Task<Response<string>> Register(RegisterDTO registerDto);
    Task<Response<string>> RequestResetPassword(RequestResetPasswordDTO requestResetPassword);
    Task<Response<string>> ResetPassword(ResetPasswordDTO resetPasswordDto);
}