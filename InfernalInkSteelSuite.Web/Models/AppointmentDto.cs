namespace InfernalInkSteelSuite.Web.Models
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ArtistId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ServiceType { get; set; } = "";
        public string ServiceCategory { get; set; } = "";
        public string Status { get; set; } = ""; // Simplified from Enum for now, or match ApiClient
        public decimal? QuotedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? Notes { get; set; }
        public ClientDto? Client { get; set; }
        public string ArtistName { get; set; } = "";
    }

}
