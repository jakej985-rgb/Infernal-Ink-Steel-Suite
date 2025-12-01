using InfernalInkSteelSuite.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfernalInkSteelSuite.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Document> Documents => Set<Document>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateLastModified();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateLastModified();
        return base.SaveChanges();
    }

    private void UpdateLastModified()
    {
        var entries = ChangeTracker.Entries<ISyncEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var currentUser = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                          ?? _httpContextAccessor?.HttpContext?.User?.Identity?.Name
                          ?? "System";

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            entity.LastModifiedUtc = DateTime.UtcNow;

            if (entry.State == EntityState.Added && entity.SyncId == Guid.Empty)
            {
                entity.SyncId = Guid.NewGuid();
            }

            entity.LastModifiedBy = currentUser;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasMaxLength(256);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Client)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Artist relationship might need adjustment if User doesn't have Appointments collection
        // Domain.User doesn't have Appointments collection, so WithMany() is empty
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Artist)
            .WithMany()
            .HasForeignKey(a => a.UserId) // Domain.Appointment uses UserId for Artist
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Client)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.ClientId);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.UploadedByUser)
            .WithMany(u => u.UploadedDocuments)
            .HasForeignKey(d => d.UploadedByUserId);

        modelBuilder.Entity<User>()
            .Property(u => u.HourlyRate)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<User>()
            .Property(u => u.CommissionRate)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Appointment>()
            .Property(a => a.PriceCharged)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Appointment>()
            .Property(a => a.QuotedPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Appointment>()
            .Property(a => a.FinalPrice)
            .HasColumnType("decimal(18,2)");
    }
}
