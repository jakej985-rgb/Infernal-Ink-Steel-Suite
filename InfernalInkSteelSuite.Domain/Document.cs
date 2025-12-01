using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfernalInkSteelSuite.Domain
{
    public class Document : ISyncEntity
    {
        public int Id { get; set; }

        // Sync Properties
        public Guid SyncId { get; set; } = Guid.NewGuid();
        public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;
        public string LastModifiedBy { get; set; } = "";
        public bool IsDeleted { get; set; }
        public byte[]? RowVersion { get; set; }

        public int UploadedByUserId { get; set; }
        public int ClientId { get; set; }

        [NotMapped]
        public Guid? ClientSyncId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public virtual Client Client { get; set; } = null!;
        public virtual User UploadedByUser { get; set; } = null!;
    }
}
