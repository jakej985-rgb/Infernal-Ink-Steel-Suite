using System;

namespace InfernalInkSteelSuite.Api.Models
{
    public interface ISyncEntity
    {
        Guid SyncId { get; set; }
        DateTime LastModifiedUtc { get; set; }
        string LastModifiedBy { get; set; }
        bool IsDeleted { get; set; }
        byte[]? RowVersion { get; set; }
    }
}
