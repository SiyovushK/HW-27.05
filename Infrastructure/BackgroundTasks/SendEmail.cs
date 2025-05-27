using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.BackgroundTasks;

public class SendEmail(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User, int>>();

            var users = await dataContext.Users.ToListAsync(stoppingToken);


            foreach (var user in users)
            {
                var emailDto = new EmailDTO()
                {
                    To = user.Email,
                    Subject = "Registration info",
                    Body = $"Hi {user.Username}! Your registration has been successfull."
                };

                await emailService.SendEmailAsync(emailDto);
            }
    
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }

    }
}