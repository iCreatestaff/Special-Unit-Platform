using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using WeatherApi; // Ensure correct namespace
using WeatherApi.Interfaces; // For service interfaces
using WeatherApi.Services;
using sp_backend.Interfaces;
using sp_backend.Services; // For service implementations

var builder = WebApplication.CreateBuilder(args);
/*TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris"); // Change to your timezone
TimeZoneInfo.Local = localTimeZone; */

// 🔹 Add Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("spApiDb")));

// 🔹 Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 🔹 Register Services
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ISubEquipmentService, SubEquipmentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INonavailabilityService, NonavailabilityService>(); // Register NonAvailability service
builder.Services.AddScoped<IMissionService, MissionService>(); // ✅ Register Mission Service
builder.Services.AddHostedService<MissionStatusUpdater>();
builder.Services.AddHostedService<SubEquipmentStatusUpdater>();
// 🔹 Register Password Hasher
builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();

// 🔹 Add Controllers
builder.Services.AddControllers();  // Ensure controllers are added

// 🔹 Enable CORS (for frontend communication)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// 🔹 Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Special Units Platform API", Version = "v1" });
});

// Build and run the app
var app = builder.Build();

// 🔹 Enable Swagger Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Special Units Platform API v1");
    });
}

// 🔹 Enable Middleware
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthorization(); // 

app.MapControllers();

app.Run();
