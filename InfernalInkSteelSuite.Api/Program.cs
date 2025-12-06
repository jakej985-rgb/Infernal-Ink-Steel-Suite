using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Api.Dtos;
using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Api.Services;
using InfernalInkSteelSuite.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

using InfernalInkSteelSuite.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IShopSettingsRepository>(sp => new ShopSettingsRepository(connectionString));
builder.Services.AddScoped<IAppointmentRepository>(sp => new AppointmentRepository(connectionString));
builder.Services.AddScoped<IClientRepository>(sp => new ClientRepository(connectionString));
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["JWT_KEY"] ?? builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured.");
        }
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsArtist", policy => policy.RequireRole(UserRole.Artist.ToString(), UserRole.Admin.ToString()))
    .AddPolicy("IsAdmin", policy => policy.RequireRole(UserRole.Admin.ToString()));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapControllers();

// ---- Auth (temporary simple login) ----
app.MapPost("/auth/login", async (LoginRequest request, AppDbContext db, PasswordHasher hasher, TokenService tokenService) =>
{
    var user = await db.Users
        .FirstOrDefaultAsync(u => u.Username == request.Username);

    if (user is null || !hasher.VerifyPassword(user.PasswordHash, request.Password))
        return Results.Unauthorized();

    // Map string Role to Enum for response
    if (!Enum.TryParse<UserRole>(user.Role, true, out var roleEnum))
    {
        roleEnum = UserRole.Artist; // Default fallback
    }

    var response = new LoginResponse
    {
        UserId = user.Id,
        Username = user.Username,
        DisplayName = user.DisplayName ?? user.Username, // Domain.User has DisplayName? No, it has Username. Wait, Domain.User doesn't have DisplayName!
        Role = roleEnum,
        Token = tokenService.GenerateToken(user)
    };

    return Results.Ok(response);
}).AllowAnonymous();

// Simple health check
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

// ---- Clients ----
app.MapGet("/clients", async (AppDbContext db) =>
    await db.Clients.ToListAsync())
    .RequireAuthorization("IsArtist");

app.MapGet("/clients/{id:int}", async (int id, AppDbContext db) =>
    await db.Clients.FindAsync(id) is { } client
        ? Results.Ok(client)
        : Results.NotFound())
    .RequireAuthorization("IsArtist");

app.MapPost("/clients", async (Client client, AppDbContext db) =>
{
    db.Clients.Add(client);
    await db.SaveChangesAsync();
    return Results.Created($"/clients/{client.Id}", client);
}).RequireAuthorization("IsArtist");

app.MapPut("/clients/{id:int}", async (int id, Client update, AppDbContext db) =>
{
    var existing = await db.Clients.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.FirstName = update.FirstName;
    existing.LastName = update.LastName;
    existing.Phone = update.Phone;
    existing.Email = update.Email;

    await db.SaveChangesAsync();
    return Results.Ok(existing);
}).RequireAuthorization("IsArtist");

app.MapDelete("/clients/{id:int}", async (int id, AppDbContext db) =>
{
    var existing = await db.Clients.FindAsync(id);
    if (existing is null) return Results.NotFound();

    db.Clients.Remove(existing);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization("IsAdmin");

// ---- Users ----
app.MapGet("/users", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users.Select(u =>
    {
        Enum.TryParse<UserRole>(u.Role, true, out var roleEnum);
        return new { u.Id, u.Username, DisplayName = u.Username, Role = roleEnum, u.IsActive };
    }));
}).RequireAuthorization("IsAdmin");

app.MapGet("/users/{id:int}", async (int id, AppDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user is null) return Results.NotFound();

    Enum.TryParse<UserRole>(user.Role, true, out var roleEnum);
    return Results.Ok(new { user.Id, user.Username, DisplayName = user.Username, Role = roleEnum, user.IsActive });
}).RequireAuthorization("IsAdmin");

app.MapPost("/users", async (UserCreateDto newUser, PasswordHasher hasher, AppDbContext db) =>
{
    var user = new User
    {
        Username = newUser.Username,
        PasswordHash = hasher.HashPassword(newUser.Password),
        // Domain.User doesn't have DisplayName, use Username or ignore
        Role = newUser.Role.ToString(),
        IsActive = true
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, DisplayName = user.Username, newUser.Role, user.IsActive });
}).RequireAuthorization("IsAdmin");

app.MapPut("/users/{id:int}", async (int id, UserUpdateDto updateDto, AppDbContext db) =>
{
    var existing = await db.Users.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.Username = updateDto.Username;
    // existing.DisplayName = updateDto.DisplayName; // Domain.User doesn't have DisplayName
    existing.Role = updateDto.Role.ToString();
    existing.IsActive = updateDto.IsActive;

    await db.SaveChangesAsync();
    return Results.Ok(new { existing.Id, existing.Username, DisplayName = existing.Username, updateDto.Role, existing.IsActive });
}).RequireAuthorization("IsAdmin");

app.MapPut("/users/{id:int}/password", async (int id, UserUpdatePasswordDto passwordDto, PasswordHasher hasher, AppDbContext db) =>
{
    var existing = await db.Users.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.PasswordHash = hasher.HashPassword(passwordDto.NewPassword);

    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization("IsAdmin");


app.MapDelete("/users/{id:int}", async (int id, AppDbContext db) =>
{
    var existing = await db.Users.FindAsync(id);
    if (existing is null) return Results.NotFound();

    db.Users.Remove(existing);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization("IsAdmin");


// ---- Appointments (simple listing by date/artist) ----
app.MapGet("/appointments", async (DateTime? date, int? artistId, AppDbContext db, HttpContext httpContext) =>
{
    var query = db.Appointments
        .Include(a => a.Client)
        .Include(a => a.Artist)
        .AsQueryable();

    // Domain.Appointment uses DateTime (Start)
    if (date.HasValue)
    {
        var dayStart = date.Value.Date;
        var dayEnd = dayStart.AddDays(1);
        query = query.Where(a => a.DateTime >= dayStart && a.DateTime < dayEnd);
    }

    var user = httpContext.User;
    if (user.IsInRole(UserRole.Artist.ToString()))
    {
        var userId = int.Parse(user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        query = query.Where(a => a.UserId == userId); // Domain uses UserId
    }
    else if (artistId.HasValue)
    {
        query = query.Where(a => a.UserId == artistId.Value);
    }


    var appointments = await query.ToListAsync();

    var results = appointments.Select(a =>
    {
        Enum.TryParse<AppointmentStatus>(a.Status, true, out var statusEnum);
        return new AppointmentDto(
            a.Id,
            a.ClientId,
            a.ArtistId, // Alias
            a.StartTime, // Alias
            a.EndTime,   // Alias
            a.ServiceType,
            a.ServiceCategory,
            statusEnum,
            a.QuotedPrice,
            a.FinalPrice,
            a.Notes,
            new ClientDto(a.Client.Id, a.Client.FirstName, a.Client.LastName, a.Client.Phone, a.Client.Email),
            a.Artist.Username // Domain.User doesn't have DisplayName
        );
    }).ToList();

    return Results.Ok(results);
}).RequireAuthorization("IsArtist");

app.MapPost("/appointments", async (Appointment appt, AppDbContext db) =>
{
    db.Appointments.Add(appt);
    await db.SaveChangesAsync();
    return Results.Created($"/appointments/{appt.Id}", appt);
}).RequireAuthorization("IsArtist");

app.MapPut("/appointments/{id:int}", async (int id, Appointment update, AppDbContext db) =>
{
    var existing = await db.Appointments.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.StartTime = update.StartTime; // Alias updates DateTime
    existing.EndTime = update.EndTime;     // Alias updates DurationMinutes
    existing.ServiceType = update.ServiceType;
    existing.ServiceCategory = update.ServiceCategory;
    existing.Status = update.Status;
    existing.QuotedPrice = update.QuotedPrice;
    existing.FinalPrice = update.FinalPrice;
    existing.Notes = update.Notes;
    existing.ClientId = update.ClientId;
    existing.ArtistId = update.ArtistId; // Alias updates UserId

    await db.SaveChangesAsync();
    return Results.Ok(existing);
}).RequireAuthorization("IsArtist");

app.MapDelete("/appointments/{id:int}", async (int id, AppDbContext db) =>
{
    var existing = await db.Appointments.FindAsync(id);
    if (existing is null) return Results.NotFound();

    db.Appointments.Remove(existing);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization("IsAdmin");

// ---- Documents: list by client ----
app.MapGet("/documents/by-client/{clientId:int}", async (int clientId, AppDbContext db) =>
{
    var docs = await db.Documents
        .Where(d => d.ClientId == clientId)
        .OrderByDescending(d => d.CreatedAt)
        .ToListAsync();

    return Results.Ok(docs);
}).RequireAuthorization("IsArtist");


// ---- Documents: upload ----
app.MapPost("/documents", async (
    int clientId,
    string? title,
    IFormFile file,
    AppDbContext db,
    IConfiguration config,
    HttpContext httpContext) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest("No file uploaded.");

    var rootPath = config["FileStorage:RootPath"] ??
                   Path.Combine(AppContext.BaseDirectory, "Uploads");

    var clientDir = Path.Combine(rootPath, clientId.ToString());
    Directory.CreateDirectory(clientDir);

    var safeFileName = Path.GetFileName(file.FileName);
    var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(safeFileName)}";
    var fullPath = Path.Combine(clientDir, uniqueName);

    await using (var stream = File.Create(fullPath))
    {
        await file.CopyToAsync(stream);
    }

    var userId = int.Parse(httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    var doc = new Document
    {
        ClientId = clientId,
        UploadedByUserId = userId,
        Title = string.IsNullOrWhiteSpace(title) ? safeFileName : title,
        FilePath = fullPath,
        CreatedAt = DateTime.UtcNow
    };

    db.Documents.Add(doc);
    await db.SaveChangesAsync();

    return Results.Created($"/documents/{doc.Id}", doc);
}).RequireAuthorization("IsArtist");


// ---- Documents: download ----
app.MapGet("/documents/{id:int}/download", async (int id, AppDbContext db) =>
{
    var doc = await db.Documents.FindAsync(id);
    if (doc is null)
        return Results.NotFound();

    if (!File.Exists(doc.FilePath))
        return Results.NotFound("File not found on disk.");

    var stream = File.OpenRead(doc.FilePath);
    var fileName = Path.GetFileName(doc.FilePath);

    // Simple generic content-type for now
    return Results.File(stream, "application/octet-stream", fileName);
}).RequireAuthorization("IsArtist");


// ---- Database migration & startup ----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    var hasher = services.GetRequiredService<PasswordHasher>();
    db.Database.Migrate();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User
            {
                Username = "admin",
                PasswordHash = hasher.HashPassword("admin123"),
                // DisplayName = "Shop Admin", // Domain.User doesn't have DisplayName
                Role = UserRole.Admin.ToString()
            },
            new User
            {
                Username = "artist1",
                PasswordHash = hasher.HashPassword("artist123"),
                // DisplayName = "Artist One",
                Role = UserRole.Artist.ToString()
            }
        );

        db.SaveChanges();
    }
}

app.Run();
