using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using sp_backend.DTO;
using sp_backend.Interfaces;
using sp_backend.Models;
using sp_backend.Services;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Services;
using sp_backend_March4.Services.NotificationCreator;
using System.Text;
using WeatherApi; // Ensure correct namespace
using WeatherApi.Interfaces;
using WeatherApi.Services;


LoadDotEnv();

var builder = WebApplication.CreateBuilder(args);
var backendUrls = builder.Configuration["BACKEND_URLS"] ?? "http://0.0.0.0:5038";
builder.WebHost.UseUrls(backendUrls);
//builder.WebHost.UseUrls("http://0.0.0.0:8000");

// 🔹 Add Database Context
var spApiConnectionString = builder.Configuration.GetConnectionString("spApiDb");
if (string.IsNullOrWhiteSpace(spApiConnectionString))
{
    throw new InvalidOperationException("ConnectionStrings__spApiDb is required. Add it to the root .env file or environment variables.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(spApiConnectionString));

// 🔹 Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 🔹 Register Services
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<ISubEquipmentService, SubEquipmentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INonavailabilityService, NonavailabilityService>();
builder.Services.AddScoped<IMissionService, MissionService>();
builder.Services.AddScoped<IEquipmentStockService, EquipmentStockService>();
builder.Services.AddHostedService<MissionStatusUpdater>();
builder.Services.AddHostedService<EquipmentStatusUpdater>();
builder.Services.AddHostedService<TrainingStatusUpdater>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<ITrainingService, TrainingService>();
builder.Services.AddScoped<IAccountTrainingService, AccountTrainingService>();
builder.Services.AddScoped<IMessageAgentService, MessageAgentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHostedService<NotificationCreator>();

builder.Services.AddScoped<IRequestMaintenanceService, RequestMaintenanceService>();
builder.Services.AddScoped<AuthService>(); // Register AuthService

// 🔹 Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Validate JWT key length (to prevent HS256 key size error)
if (string.IsNullOrEmpty(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException("JWT Key must be at least 32 bytes (256 bits) for HS256.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated: " + context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

// 🔹 Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// 🔹 Add Controllers
builder.Services.AddControllers();

// 🔹 Add Swagger with JWT Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Special Units Platform API", Version = "v1" });

    // Add JWT Authentication support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 🔹 Enable Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Special Units Platform API v1");
    });
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 🔹 Seed SuperAdmin
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();

    dbContext.Database.EnsureCreated();

    if (!dbContext.Accounts.Any(a => a.Role == "SuperAdmin"))
    {
        var superAdminDto = new AccountDTO
        {
            Username = "superadmin",
            Name = "Super Admin",
            Type = "admin",
            Badge = "",
            Password = "SuperPass123!",
            Role = "SuperAdmin"
        };
        dbContext.Accounts.Add(superAdminDto.ToEntity(authService.HashPassword(superAdminDto.Password)));
        await dbContext.SaveChangesAsync();
    }
}

app.Run();

static void LoadDotEnv()
{
    foreach (var startPath in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
    {
        var directory = new DirectoryInfo(startPath);
        while (directory != null)
        {
            var envPath = Path.Combine(directory.FullName, ".env");
            if (File.Exists(envPath))
            {
                foreach (var rawLine in File.ReadAllLines(envPath))
                {
                    var line = rawLine.Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    var separatorIndex = line.IndexOf('=');
                    if (separatorIndex <= 0)
                    {
                        continue;
                    }

                    var key = line[..separatorIndex].Trim();
                    var value = line[(separatorIndex + 1)..].Trim().Trim('"').Trim('\'');
                    if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
                    {
                        Environment.SetEnvironmentVariable(key, value);
                    }
                }

                return;
            }

            directory = directory.Parent;
        }
    }
}
