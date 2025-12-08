using System.Collections.Generic;
using System.Threading.Tasks;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Text.Json;

namespace InfernalInkSteelSuite.DataProviders.Syncing
{
    public class SyncingClientRepository : IClientRepository
    {
        private readonly IClientRepository _inner;
        private readonly ISyncQueueRepository _syncQueue;

        public SyncingClientRepository(IClientRepository inner, ISyncQueueRepository syncQueue)
        {
            _inner = inner;
            _syncQueue = syncQueue;
        }

        public Client? Get(int id) => _inner.Get(id);
        public List<Client> GetAll() => _inner.GetAll();
        public string? GetClientNameById(int clientId) => _inner.GetClientNameById(clientId);
        public int? GetClientIdByName(string name) => _inner.GetClientIdByName(name);
        public int? GetClientIdByEmail(string email) => _inner.GetClientIdByEmail(email);
        public int? GetClientIdByPhone(string phone) => _inner.GetClientIdByPhone(phone);

        public async Task<List<Client>> GetAllAsync() => await _inner.GetAllAsync();
        public async Task<Client?> GetByIdAsync(int id) => await _inner.GetByIdAsync(id);


        public void Insert(Client client)
        {
            _inner.Insert(client);
            // After insert, client.Id is populated
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Client",
                EntityId = client.Id,
                Action = "Create",
                PayloadJson = JsonSerializer.Serialize(client)
            });
        }

        public void Update(Client client)
        {
            _inner.Update(client);
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Client",
                EntityId = client.Id,
                Action = "Update",
                PayloadJson = JsonSerializer.Serialize(client)
            });
        }

        public void Delete(int id)
        {
            _inner.Delete(id);
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "Client",
                EntityId = id,
                Action = "Delete"
            });
        }

        public async Task<Client> AddAsync(Client client)
        {
            var result = await _inner.AddAsync(client);
            await _syncQueue.EnqueueAsync(new SyncQueueItem
            {
                EntityType = "Client",
                EntityId = result.Id,
                Action = "Create",
                PayloadJson = JsonSerializer.Serialize(result)
            });
            return result;
        }

        public async Task UpdateAsync(Client client)
        {
            await _inner.UpdateAsync(client);
            await _syncQueue.EnqueueAsync(new SyncQueueItem
            {
                EntityType = "Client",
                EntityId = client.Id,
                Action = "Update",
                PayloadJson = JsonSerializer.Serialize(client)
            });
        }

        public async Task DeleteAsync(int id)
        {
            await _inner.DeleteAsync(id);
            await _syncQueue.EnqueueAsync(new SyncQueueItem
            {
                EntityType = "Client",
                EntityId = id,
                Action = "Delete"
            });
        }
    }
}
