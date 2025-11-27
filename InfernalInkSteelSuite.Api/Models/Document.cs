namespace InfernalInkSteelSuite.Api.Models;

public class Document
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public int UploadedByUserId { get; set; }
    public User UploadedBy { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
