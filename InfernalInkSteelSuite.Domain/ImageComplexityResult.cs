namespace InfernalInkSteelSuite.Domain
{
    public class ImageComplexityResult
    {
        public int LineComplexity { get; set; }      // 1–5
        public int ShadingComplexity { get; set; }   // 1–5
        public int ColorComplexity { get; set; }     // 1–5
        public int Difficulty { get; set; }          // 1–5

        public double BlackCoverage { get; set; }    // 0–1
        public string StyleHint { get; set; }
        public string Notes { get; set; }
    }
}
