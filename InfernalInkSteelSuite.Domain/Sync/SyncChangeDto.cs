using System;

namespace InfernalInkSteelSuite.Domain.Sync
{
    public class SyncChangeDto<T>
    {
        public string EntityName { get; set; } = "";
        public Guid EntityId { get; set; }
        public string Operation { get; set; } = ""; // "Create" | "Update" | "Delete"
        public T Payload { get; set; } = default!;
        public DateTime ClientTimestampUtc { get; set; }
    }
}
