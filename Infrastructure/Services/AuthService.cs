using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.DTOs;
using Domain.DTOs.Auth;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService(
        IAuthRepository<User> _userRepository,
        IPasswordHasher<User> _passwordHasher,
        IConfiguration _config,
        IEmailService _emailService
    ) : IAuthService
{
    public async Task<Response<TokenDTO>> Login(LoginDTO loginDto)
    {
        var user = await _userRepository.FirstOrDefaultAsync(c => c.Email.ToLower() == loginDto.Email.ToLower());
        if (user == null)
        {
            return new Response<TokenDTO>(HttpStatusCode.BadRequest, "Incorrect email or password");
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return new Response<TokenDTO>(HttpStatusCode.BadRequest, "Incorrect email or password");
        }

        var token = GenerateJwt(user);
        return new Response<TokenDTO>(new TokenDTO { Token = token });
    }

    public async Task<Response<string>> Register(RegisterDTO registerDto)
    {
        var existingUser = await _userRepository.AnyAsync(c => c.Email.ToLower() == registerDto.Email.ToLower());
        if (existingUser)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "User with this email already exists.");
        }

        var user = new User
        {
            Username = registerDto.FullName,
            Email = registerDto.Email,  
            Role = Roles.User
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new Response<string>("User registered successfully");
    }

    public async Task<Response<string>> RequestResetPassword(RequestResetPasswordDTO requestResetPassword)
    {
        var user = await _userRepository.FirstOrDefaultAsync(c => c.Email == requestResetPassword.Email);
        if (user == null)
        {
            return new Response<string>("If the email address is valid, a password reset link has been sent.");
        }

        var resetToken = Guid.NewGuid().ToString();
        var tokenExpiry = DateTime.UtcNow.AddHours(1);

        user.ResetToken = resetToken;
        user.ResetTokenExpiry = tokenExpiry;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        var emailDto = new EmailDTO()
        {
            To = requestResetPassword.Email,
            Subject = "Password Reset Request",
            Body = $"Hello {user.Username},\n\n" +
                   $"You have requested a password reset. Please use the following token to reset your password:\n\n" +
                   $"<b>{resetToken}</b>\n\n" +
                   $"This token is valid for 1 hour. If you did not request this, please ignore this email.\n\n" +
                   $"Thank you."
        };

        var emailSent = await _emailService.SendEmailAsync(emailDto);

        return emailSent
            ? new Response<string>("If the email address is valid, a password reset link has been sent.")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to send reset password email.");
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDTO resetPasswordDto)
    {
        var user = await _userRepository.FirstOrDefaultAsync(c => c.Email.ToLower() == resetPasswordDto.Email.ToLower());
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Invalid token or email.");
        }

        if (string.IsNullOrEmpty(user.ResetToken) || user.ResetToken != resetPasswordDto.Token ||
            user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Invalid or expired token.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, resetPasswordDto.NewPassword);

        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return new Response<string>("Password has been reset successfully.");
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        if (!string.IsNullOrEmpty(user.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role));
        }

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}