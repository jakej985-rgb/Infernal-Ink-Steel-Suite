using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Api.Services;
using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Repositories.Services;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using SharedHasher = InfernalInkSteelSuite.Repositories.Services.PasswordHasher;
using InfernalInkSteelSuite.Domain;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace InfernalInkSteelSuite.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfernalCore(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Fix M12: Resolve relative paths to AppContext.BaseDirectory
        if (connectionString.StartsWith("Data Source=../"))
        {
            var relativePath = connectionString.Substring("Data Source=".Length);
            var absolutePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath));
            connectionString = $"Data Source={absolutePath}";
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddSingleton<SharedHasher>();
        services.AddScoped<TokenService>();
        services.AddScoped<ISyncService, SyncService>();
        
        services.AddScoped<IShopSettingsRepository, ShopSettingsRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<DocumentService>();
        services.AddScoped<QuoteService>();
        services.AddScoped<StatsService>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserProvider, WebUserProvider>();
        
        // ASP.NET Core UI / API support
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("LoginLimiter", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 5;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });
        });

        return services;
    }

    public static IServiceCollection AddInfernalAuth(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var jwtKey = configuration["JWT_KEY"] ?? configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("CRITICAL: JWT_KEY environment variable (or Jwt:Key in appsettings) is missing. Authentication cannot be configured.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("IsArtist", policy => policy.RequireRole(UserRole.Artist.ToString(), UserRole.Admin.ToString()))
            .AddPolicy("IsAdmin", policy => policy.RequireRole(UserRole.Admin.ToString()));

        return services;
    }

    public static IServiceCollection AddInfernalCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:5001"];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
