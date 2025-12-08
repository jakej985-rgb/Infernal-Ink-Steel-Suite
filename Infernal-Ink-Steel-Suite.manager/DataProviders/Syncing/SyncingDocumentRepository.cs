using System.Collections.Generic;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Text.Json;

namespace InfernalInkSteelSuite.DataProviders.Syncing
{
    public class SyncingDocumentRepository : IDocumentRepository
    {
        private readonly IDocumentRepository _inner;
        private readonly ISyncQueueRepository _syncQueue;

        public SyncingDocumentRepository(IDocumentRepository inner, ISyncQueueRepository syncQueue)
        {
            _inner = inner;
            _syncQueue = syncQueue;
        }

        public Document? Get(int id) => _inner.Get(id);
        public List<Document> GetAll() => _inner.GetAll();
        public List<Document> GetDocuments(int userId, string role, int maxDocuments, out bool truncated) => _inner.GetDocuments(userId, role, maxDocuments, out truncated);

        public void Insert(Document document)
        {
            _inner.Insert(document);
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Document",
                EntityId = document.Id,
                Action = "Create",
                PayloadJson = JsonSerializer.Serialize(document)
            });
        }

        public void Update(Document document)
        {
            _inner.Update(document);
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Document",
                EntityId = document.Id,
                Action = "Update",
                PayloadJson = JsonSerializer.Serialize(document)
            });
        }

        public void Delete(int id)
        {
            _inner.Delete(id);
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Document",
                EntityId = id,
                Action = "Delete"
            });
        }
    }
}
