using InfernalInkSteelSuite.Api.Data;
using InfernalInkSteelSuite.Api.Dtos;
using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// ---- Auth (temporary simple login) ----
app.MapPost("/auth/login", async (LoginRequest request, AppDbContext db) =>
{
    var user = await db.Users
        .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password);

    if (user is null)
        return Results.Unauthorized();

    var response = new LoginResponse
    {
        UserId = user.Id,
        Username = user.Username,
        DisplayName = user.DisplayName,
        Role = user.Role,
        Token = Guid.NewGuid().ToString() // placeholder
    };

    return Results.Ok(response);
});

// Simple health check
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// ---- Clients ----
app.MapGet("/clients", async (AppDbContext db) =>
    await db.Clients.ToListAsync());

app.MapGet("/clients/{id:int}", async (int id, AppDbContext db) =>
    await db.Clients.FindAsync(id) is { } client
        ? Results.Ok(client)
        : Results.NotFound());

app.MapPost("/clients", async (Client client, AppDbContext db) =>
{
    db.Clients.Add(client);
    await db.SaveChangesAsync();
    return Results.Created($"/clients/{client.Id}", client);
});

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
});

app.MapDelete("/clients/{id:int}", async (int id, AppDbContext db) =>
{
    var existing = await db.Clients.FindAsync(id);
    if (existing is null) return Results.NotFound();

    db.Clients.Remove(existing);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// ---- Users ----
app.MapGet("/users", async (AppDbContext db) =>
    await db.Users.Select(u => new { u.Id, u.Username, u.DisplayName, u.Role, u.IsActive }).ToListAsync());

app.MapGet("/users/{id:int}", async (int id, AppDbContext db) =>
    await db.Users.FindAsync(id) is { } user
        ? Results.Ok(new { user.Id, user.Username, user.DisplayName, user.Role, user.IsActive })
        : Results.NotFound());

app.MapPost("/users", async (UserCreateDto newUser, PasswordHasher hasher, AppDbContext db) =>
{
    var user = new User
    {
        Username = newUser.Username,
        PasswordHash = hasher.HashPassword(newUser.Password),
        DisplayName = newUser.DisplayName,
        Role = newUser.Role,
        IsActive = true
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", new { user.Id, user.Username, user.DisplayName, user.Role, user.IsActive });
});

app.MapPut("/users/{id:int}", async (int id, UserUpdateDto updateDto, AppDbContext db) =>
{
    var existing = await db.Users.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.Username = updateDto.Username;
    existing.DisplayName = updateDto.DisplayName;
    existing.Role = updateDto.Role;
    existing.IsActive = updateDto.IsActive;

    await db.SaveChangesAsync();
    return Results.Ok(new { existing.Id, existing.Username, existing.DisplayName, existing.Role, existing.IsActive });
});

app.MapPut("/users/{id:int}/password", async (int id, UserUpdatePasswordDto passwordDto, PasswordHasher hasher, AppDbContext db) =>
{
    var existing = await db.Users.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.PasswordHash = hasher.HashPassword(passwordDto.NewPassword);

    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.MapDelete("/users/{id:int}", async (int id, AppDbContext db) =>
{
    var existing = await db.Users.FindAsync(id);
    if (existing is null) return Results.NotFound();

    db.Users.Remove(existing);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


// ---- Appointments (simple listing by date/artist) ----
app.MapGet("/appointments", async (DateTime? date, int? artistId, AppDbContext db) =>
{
    var query = db.Appointments
        .Include(a => a.Client)
        .Include(a => a.Artist)
        .AsQueryable();

    if (date.HasValue)
    {
        var dayStart = date.Value.Date;
        var dayEnd = dayStart.AddDays(1);
        query = query.Where(a => a.StartTime >= dayStart && a.StartTime < dayEnd);
    }

    if (artistId.HasValue)
        query = query.Where(a => a.ArtistId == artistId.Value);

    var results = await query
        .Select(a => new AppointmentDto(
            a.Id,
            a.ClientId,
            a.ArtistId,
            a.StartTime,
            a.EndTime,
            a.ServiceType,
            a.ServiceCategory,
            a.Status,
            a.QuotedPrice,
            a.FinalPrice,
            a.Notes,
            new ClientDto(a.Client.Id, a.Client.FirstName, a.Client.LastName, a.Client.Phone, a.Client.Email),
            a.Artist.DisplayName
        ))
        .ToListAsync();

    return Results.Ok(results);
});

app.MapPost("/appointments", async (Appointment appt, AppDbContext db) =>
{
    db.Appointments.Add(appt);
    await db.SaveChangesAsync();
    return Results.Created($"/appointments/{appt.Id}", appt);
});

app.MapPut("/appointments/{id:int}", async (int id, Appointment update, AppDbContext db) =>
{
    var existing = await db.Appointments.FindAsync(id);
    if (existing is null) return Results.NotFound();

    existing.StartTime = update.StartTime;
    existing.EndTime = update.EndTime;
    existing.ServiceType = update.ServiceType;
    existing.ServiceCategory = update.ServiceCategory;
    existing.Status = update.Status;
    existing.QuotedPrice = update.QuotedPrice;
    existing.FinalPrice = update.FinalPrice;
    existing.Notes = update.Notes;
    existing.ClientId = update.ClientId;
    existing.ArtistId = update.ArtistId;

    await db.SaveChangesAsync();
    return Results.Ok(existing);
});

app.MapDelete("/appointments/{id:int}", async (int id, AppDbContext db) =>
{
    var existing = await db.Appointments.FindAsync(id);
    if (existing is null) return Results.NotFound();

    db.Appointments.Remove(existing);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// ---- Documents: list by client ----
app.MapGet("/documents/by-client/{clientId:int}", async (int clientId, AppDbContext db) =>
{
    var docs = await db.Documents
        .Where(d => d.ClientId == clientId)
        .OrderByDescending(d => d.CreatedAt)
        .ToListAsync();

    return Results.Ok(docs);
});


// ---- Documents: upload ----
app.MapPost("/documents", async (
    int clientId,
    int uploadedByUserId,
    string? title,
    IFormFile file,
    AppDbContext db,
    IConfiguration config) =>
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

    var doc = new Document
    {
        ClientId = clientId,
        UploadedByUserId = uploadedByUserId,
        Title = string.IsNullOrWhiteSpace(title) ? safeFileName : title,
        FilePath = fullPath,
        CreatedAt = DateTime.UtcNow
    };

    db.Documents.Add(doc);
    await db.SaveChangesAsync();

    return Results.Created($"/documents/{doc.Id}", doc);
});


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
});


// ---- Database migration & startup ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User
            {
                Username = "admin",
                PasswordHash = "admin123", // TODO: replace with real hashing
                DisplayName = "Shop Admin",
                Role = UserRole.Admin
            },
            new User
            {
                Username = "artist1",
                PasswordHash = "artist123",
                DisplayName = "Artist One",
                Role = UserRole.Artist
            }
        );

        db.SaveChanges();
    }
}

app.Run();
