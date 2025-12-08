using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.DataProviders.Syncing;

namespace InfernalInkSteelSuite.DataProviders
{
    public class SyncingDataProvider : IDataProvider
    {
        private readonly IDataProvider _localProvider;
        private readonly ISyncQueueRepository _syncQueue;

        public IClientRepository Clients { get; }
        public IAppointmentRepository Appointments { get; }
        public IUserRepository Users { get; }
        public IShopSettingsRepository ShopSettings { get; }
        public IDocumentRepository Documents { get; }
        public IQuoteRepository Quotes { get; }

        public bool IsOffline => false; // It connects to server eventually

        public IDataProvider LocalProvider => _localProvider;
        public ISyncQueueRepository SyncQueue => _syncQueue;

        public SyncingDataProvider(IDataProvider localProvider, ISyncQueueRepository syncQueueRepository)
        {
            _localProvider = localProvider;
            _syncQueue = syncQueueRepository;

            Clients = new SyncingClientRepository(_localProvider.Clients, _syncQueue);
            Appointments = new SyncingAppointmentRepository(_localProvider.Appointments, _syncQueue);
            Users = new SyncingUserRepository(_localProvider.Users, _syncQueue);
            ShopSettings = new SyncingShopSettingsRepository(_localProvider.ShopSettings, _syncQueue);
            Documents = new SyncingDocumentRepository(_localProvider.Documents, _syncQueue);
            Quotes = new SyncingQuoteRepository(_localProvider.Quotes, _syncQueue);
        }
    }
}
