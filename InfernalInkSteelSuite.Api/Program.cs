using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Api.Extensions;
using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Repositories.Services;
using SharedHasher = InfernalInkSteelSuite.Repositories.Services.PasswordHasher;
using InfernalInkSteelSuite.Domain;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// (H8) Modularized Service Registration
builder.Services.AddInfernalCore(builder.Configuration);
builder.Services.AddInfernalAuth(builder.Configuration, builder.Environment);
builder.Services.AddInfernalCors(builder.Configuration);

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// (C3) Swagger restricted to Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();

// (H8) Modularized Endpoint Mapping
app.MapInfernalEndpoints();
app.MapControllers();

// Database initialization
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    var hasher = services.GetRequiredService<SharedHasher>();
    using var mutex = new System.Threading.Mutex(false, "Global\\InfernalInkSteelSuiteDbMigration");
    var hasHandle = false;
    try
    {
        hasHandle = mutex.WaitOne(TimeSpan.FromSeconds(30), false);
        if (!hasHandle) throw new TimeoutException("Timeout waiting for exclusive access to DB migration.");
        db.Database.Migrate();
    }
    finally
    {
        if (hasHandle) mutex.ReleaseMutex();
    }

    // (C2) Seeding restricted to Development
    if (app.Environment.IsDevelopment() && !db.Users.Any())
    {
        app.Logger.LogWarning("Seeding default users with WEAK passwords — change immediately in production.");
        db.Users.AddRange(
            new User
            {
                Username = "admin",
                PasswordHash = hasher.HashPassword("admin123"),
                DisplayName = "Shop Admin",
                Role = UserRole.Admin.ToString()
            },
            new User
            {
                Username = "artist1",
                PasswordHash = hasher.HashPassword("artist123"),
                DisplayName = "Artist One",
                Role = UserRole.Artist.ToString()
            }
        );
        db.SaveChanges();
    }
}

app.Run();
