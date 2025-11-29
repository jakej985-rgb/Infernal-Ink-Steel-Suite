using System.ComponentModel.DataAnnotations.Schema;

namespace InfernalInkSteelSuite.Api.Models;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled,
    Blocked
}

public class Appointment : ISyncEntity
{
    public int Id { get; set; }

    // Sync Properties
    public Guid SyncId { get; set; } = Guid.NewGuid();
    public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;
    public string LastModifiedBy { get; set; } = "";
    public bool IsDeleted { get; set; }
    public byte[]? RowVersion { get; set; }

    public int ClientId { get; set; }

    [NotMapped]
    public Guid? ClientSyncId { get; set; }

    public Client Client { get; set; } = null!;

    public int ArtistId { get; set; }
    public User Artist { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public string ServiceType { get; set; } = null!;     // Tattoo / Piercing
    public string ServiceCategory { get; set; } = null!; // Touch-up, Consult, etc.
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public decimal? QuotedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public string? Notes { get; set; }
}
