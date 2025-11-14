using System;

namespace InfernalInkSteelSuite.Domain
{
    public class ShopSettings
    {
        public int Id { get; set; } = -1;
        public string ShopName { get; set; }
        public string LogoPath { get; set; }
        public string AccentColor { get; set; }
        public string SidebarArtworkPath { get; set; }
        public string LoginHeadline { get; set; }
        public string LoginTagline { get; set; }
        public string LoginBackgroundPath { get; set; }
        public string LoginHeadlineFontFamily { get; set; }
        public string LoginTaglineFontFamily { get; set; }
        public string LoginTextColor { get; set; }
        public double TattooPerHour { get; set; } = 0.0;
        public double PiercingSingle { get; set; } = 0.0;
        public double PiercingMulti { get; set; } = 0.0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
