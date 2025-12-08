using System;

namespace InfernalInkSteelSuite.Domain
{
    public class SyncQueueItem
    {
        public int Id { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Action { get; set; } = string.Empty; // Create, Update, Delete
        public string? PayloadJson { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SyncedAt { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Synced, Failed
        public int RetryCount { get; set; }
        public string? LastErrorMessage { get; set; }
    }
}
