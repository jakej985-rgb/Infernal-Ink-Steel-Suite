namespace InfernalInkSteelSuite.Api.Models;

public class Client : ISyncEntity
{
    public int Id { get; set; }

    // Sync Properties
    public Guid SyncId { get; set; } = Guid.NewGuid();
    public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;
    public string LastModifiedBy { get; set; } = "";
    public bool IsDeleted { get; set; }
    public byte[]? RowVersion { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
