using System;
using System.IO;

namespace InfernalInkSteelSuite.Domain
{
    public class Document
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClientId { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string DisplayName => string.IsNullOrEmpty(Title) ? "[Untitled]" : Title;
        public string FileName => Path.GetFileName(FilePath);
        public bool Exists => File.Exists(FilePath);
    }
}
