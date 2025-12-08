using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.DataProviders
{
    public sealed class LocalDataProvider : IDataProvider
    {
        public IClientRepository Clients { get; }
        public IAppointmentRepository Appointments { get; }
        public IUserRepository Users { get; }
        public IShopSettingsRepository ShopSettings { get; }
        public IDocumentRepository Documents { get; }
        public IQuoteRepository Quotes { get; }

        public bool IsOffline => false; // Local only is considered "always available" implementation

        public LocalDataProvider(string localDbPath, string uploadRoot)
        {
            string connString = $"Data Source={localDbPath}";

            Clients = new ClientRepository(connString);
            Appointments = new AppointmentRepository(connString);
            Users = new UserRepository(connString);
            ShopSettings = new ShopSettingsRepository(connString);
            Documents = new DocumentRepository(connString);
            Quotes = new QuoteRepository(connString);
        }
    }
}
