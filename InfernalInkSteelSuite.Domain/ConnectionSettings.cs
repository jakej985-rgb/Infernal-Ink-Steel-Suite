using System;

namespace InfernalInkSteelSuite.Domain
{
    public sealed class ConnectionSettings
    {
        public DataMode Mode { get; set; } = DataMode.LocalOnly;

        public string ServerBaseUrl { get; set; } = string.Empty;
        public bool UseHttps { get; set; } = true;

        public int SyncIntervalMinutes { get; set; } = 60;

        public string LocalDbPath { get; set; } = string.Empty;
        public string LocalUploadRoot { get; set; } = string.Empty;

        public DateTime? LastSuccessfulSyncUtc { get; set; }
    }
}
