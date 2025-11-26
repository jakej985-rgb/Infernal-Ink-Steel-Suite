namespace InfernalInkSteelSuite.Domain
{
    public class ImageComplexityResult
    {
        public int LineComplexity { get; set; }      // 1–5
        public int ShadingComplexity { get; set; }   // 1–5
        public int ColorComplexity { get; set; }     // 1–5
        public int SuggestedDifficulty { get; set; }          // 1–5

        public double SolidDarkFill { get; set; }    // 0–1
        public string StyleHint { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
