using System;

namespace InfernalInkSteelSuite.Domain
{
    public class ShopSettings
    {
        public int Id { get; set; } = -1;
        public string ShopName { get; set; } = string.Empty;
        public string LogoPath { get; set; } = string.Empty;
        public string AccentColor { get; set; } = string.Empty;
        public string SidebarArtworkPath { get; set; } = string.Empty;
        [Obsolete("This property is no longer used and will be removed in a future version.")]
        public string LoginHeadline { get; set; } = string.Empty;
        public string SpecialMessageText { get; set; } = string.Empty;
        public string LoginBackgroundPath { get; set; } = string.Empty;
        public string LoginHeadlineFontFamily { get; set; } = string.Empty;
        public string LoginTaglineFontFamily { get; set; } = string.Empty;
        public string LoginTextColor { get; set; } = string.Empty;
        public double TattooPerHour { get; set; } = 0.0;
        public double PiercingSingle { get; set; } = 0.0;
        public double PiercingMulti { get; set; } = 0.0;
        public bool EnableAutomaticHolidayThemes { get; set; } = false;
        public bool IsSpecialMessageEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
