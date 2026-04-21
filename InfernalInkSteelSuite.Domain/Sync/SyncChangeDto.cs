using System;
using System.Text.Json.Serialization;

namespace InfernalInkSteelSuite.Domain.Sync
{
    public class SyncChangeDto<T>
    {
        public string EntityName { get; set; } = "";
        public Guid EntityId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SyncOperation Operation { get; set; } = SyncOperation.Update;
        public T Payload { get; set; } = default!;
        public DateTime ClientTimestampUtc { get; set; }
    }
}
