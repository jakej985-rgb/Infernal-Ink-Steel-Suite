using System;

namespace InfernalInkSteelSuite.Domain
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "";
        public string Notes { get; set; } = "";
    }
}
