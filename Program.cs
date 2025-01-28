using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using WeatherApi; // Ensure this is the correct namespace where MappingProfile is defined
using WeatherApi.Interfaces; // For IEquipmentService, ISubEquipmentService
using WeatherApi.Services; // For EquipmentService, SubEquipmentService

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("spApiDb"))); // Ensure the correct connection string is used

// Fix ambiguity with AutoMapper by specifying the correct assembly
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly); // This tells AutoMapper to look for profiles in the specified assembly

// Add your services (e.g., IEquipmentService, ISubEquipmentService)
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ISubEquipmentService, SubEquipmentService>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API v1");
    });
}

app.UseRouting();
app.UseAuthorization(); // Ensure this is added if you're using authentication
app.MapControllers();
app.Run();
