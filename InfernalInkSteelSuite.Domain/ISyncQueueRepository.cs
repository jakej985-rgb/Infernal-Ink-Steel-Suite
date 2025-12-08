using System.Collections.Generic;
using System.Threading.Tasks;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public interface ISyncQueueRepository
    {
        void Enqueue(SyncQueueItem item);
        List<SyncQueueItem> GetPendingItems();
        void UpdateStatus(int id, string status, string? errorMessage = null);
        void MarkAsSynced(int id);
        Task EnqueueAsync(SyncQueueItem item);
    }
}
