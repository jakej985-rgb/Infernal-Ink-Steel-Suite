namespace InfernalInkSteelSuite.Api.Models;

public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled,
    Blocked
}

public class Appointment
{
    public int Id { get; set; }

    public int ClientId { get; set; }
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
