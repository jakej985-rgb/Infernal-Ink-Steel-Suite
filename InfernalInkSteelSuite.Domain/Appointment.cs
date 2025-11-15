using System;

namespace InfernalInkSteelSuite.Domain
{
    public class Appointment
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public DateTime DateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string ServiceType { get; set; }
        public string ServiceCategory { get; set; }
        public string PriceType { get; set; }
        public double PriceCharged { get; set; }
        public string Notes { get; set; }
        public string ClientName { get; set; }
        public string Color { get; set; }
        public string Status { get; set; }
    }
}
