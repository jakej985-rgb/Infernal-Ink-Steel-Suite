namespace InfernalInkSteelSuite.Api.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int UploadedByUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual Client? Client { get; set; }
        public virtual User? UploadedByUser { get; set; }
    }
}
