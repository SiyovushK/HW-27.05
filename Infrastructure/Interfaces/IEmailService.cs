using Domain.DTOs;

namespace Infrastructure.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailDTO emailDto);
}