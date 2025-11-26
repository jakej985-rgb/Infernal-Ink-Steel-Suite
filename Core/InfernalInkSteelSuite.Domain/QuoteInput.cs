namespace InfernalInkSteelSuite.Domain
{
    public class QuoteInput
    {
        // Client & Tattoo
        public int? ClientId { get; set; }
        public string Placement { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public bool IsCoverUp { get; set; }

        // Size
        public double Width { get; set; }   // in cm or inches
        public double Height { get; set; }
        public int CoverageLevel { get; set; } // 1–5

        // Complexity sliders (manual or from photo)
        public int LineComplexity { get; set; }      // 1–5
        public int ShadingComplexity { get; set; }   // 1–5
        public int ColorComplexity { get; set; }     // 1–5
        public int Difficulty { get; set; }          // 1–5

        // Artist
        public int ArtistId { get; set; }
    }
}
