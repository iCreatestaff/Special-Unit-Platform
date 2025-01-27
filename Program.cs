using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // Add controllers
builder.Services.AddEndpointsApiExplorer(); // Enable API explorer for minimal APIs

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Weather API",
        Version = "v1",
        Description = "A simple API to manage weather-related data."
    });
});

// Configure Entity Framework and connection string
builder.Services.AddDbContext<WeatherApi.Models.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("spApiDb")));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Developer exception page
    app.UseSwagger(); // Enable Swagger
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API v1");
    });
}

app.UseHttpsRedirection(); // Redirect HTTP traffic to HTTPS

app.UseAuthorization(); // Authorization middleware

app.MapControllers(); // Map controllers to endpoints

app.Run(); // Run the application
