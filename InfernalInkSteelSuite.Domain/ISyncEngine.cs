using System;
using System.Threading.Tasks;

namespace InfernalInkSteelSuite.Domain
{
    public interface ISyncEngine
    {
        void Start();
        void Stop();
        Task ForceSyncAsync();
        bool IsSyncing { get; }
        event EventHandler<SyncStatusEventArgs> SyncStatusChanged;
    }

    public class SyncStatusEventArgs : EventArgs
    {
        public bool IsSyncing { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public DateTime LastSyncTime { get; set; }
        public bool IsOnline { get; set; }
    }
}
