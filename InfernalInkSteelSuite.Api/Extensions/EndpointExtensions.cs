using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Api.Dtos;
using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Api.Services;
using InfernalInkSteelSuite.Repositories.Services;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Domain;
using Microsoft.EntityFrameworkCore;
using SharedHasher = InfernalInkSteelSuite.Repositories.Services.PasswordHasher;

namespace InfernalInkSteelSuite.Api.Extensions;

public static class EndpointExtensions
{
    public static void MapInfernalEndpoints(this IEndpointRouteBuilder app)
    {
        // ---- Auth ----
        var auth = app.MapGroup("/auth").AllowAnonymous();
        auth.MapPost("/login", async (LoginRequest request, AppDbContext db, SharedHasher hasher, TokenService tokenService) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null || !hasher.VerifyPassword(user.PasswordHash, request.Password))
                return Results.Unauthorized();

            if (!Enum.TryParse<UserRole>(user.Role, true, out var roleEnum))
                roleEnum = UserRole.Artist;

            return Results.Ok(new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName ?? user.Username,
                Role = roleEnum,
                Token = tokenService.GenerateToken(user)
            });
        }).RequireRateLimiting("LoginLimiter");

        // ---- Health ----
        app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

        // ---- Users ----
        var users = app.MapGroup("/api/users").RequireAuthorization("IsAdmin");
        
        users.MapGet("/", async (AppDbContext db) =>
        {
            var data = await db.Users.ToListAsync();
            return Results.Ok(data.Select(u =>
            {
                Enum.TryParse<UserRole>(u.Role, true, out var roleEnum);
                return new { u.Id, u.Username, DisplayName = u.DisplayName ?? u.Username, Role = roleEnum, u.IsActive };
            }));
        });

        users.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound();
            Enum.TryParse<UserRole>(user.Role, true, out var roleEnum);
            return Results.Ok(new { user.Id, user.Username, DisplayName = user.Username, Role = roleEnum, user.IsActive });
        });

        users.MapPost("/", async (UserCreateDto newUser, SharedHasher hasher, AppDbContext db) =>
        {
            var user = new User
            {
                Username = newUser.Username,
                PasswordHash = hasher.HashPassword(newUser.Password),
                DisplayName = newUser.DisplayName,
                Role = newUser.Role.ToString(),
                IsActive = true
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Username, user.DisplayName, newUser.Role, user.IsActive });
        });

        users.MapPut("/{id:int}", async (int id, UserUpdateDto updateDto, AppDbContext db) =>
        {
            var existing = await db.Users.FindAsync(id);
            if (existing is null) return Results.NotFound();
            existing.Username = updateDto.Username;
            existing.DisplayName = updateDto.DisplayName;
            existing.Role = updateDto.Role.ToString();
            existing.IsActive = updateDto.IsActive;
            await db.SaveChangesAsync();
            return Results.Ok(new { existing.Id, existing.Username, existing.DisplayName, updateDto.Role, existing.IsActive });
        });

        users.MapPut("/{id:int}/password", async (int id, UserUpdatePasswordDto passwordDto, SharedHasher hasher, AppDbContext db) =>
        {
            var existing = await db.Users.FindAsync(id);
            if (existing is null) return Results.NotFound();
            existing.PasswordHash = hasher.HashPassword(passwordDto.NewPassword);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        users.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var existing = await db.Users.FindAsync(id);
            if (existing is null) return Results.NotFound();
            db.Users.Remove(existing);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });


    }
}
