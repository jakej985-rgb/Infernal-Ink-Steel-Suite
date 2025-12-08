using System.Collections.Generic;

namespace InfernalInkSteelSuite.Domain
{
    public interface IDataProvider
    {
        IClientRepository Clients { get; }
        IAppointmentRepository Appointments { get; }
        IUserRepository Users { get; }
        IShopSettingsRepository ShopSettings { get; }
        IDocumentRepository Documents { get; }
        IQuoteRepository Quotes { get; }

        bool IsOffline { get; }
    }
}
