using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.AutoMapper;
using Infrastructure.Extensions;
using Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(InfrastructureProfile));
// builder.Services.AddHostedService<SendEmail>();
builder.Services.AddInfrastructure();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddAuthenticationConfiguration(builder.Configuration);
builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseNpgsql(connection));
builder.Services.AddMemoryCache();

// builder.Services.AddHangfire(opt => opt.UsePostgreSqlStorage(connection));
// builder.Services.AddHangfireServer();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

var app = builder.Build();

await app.Services.ApplyMigrationsAndSeedAsync();

// app.Services.RegisterRecurringJobs();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();