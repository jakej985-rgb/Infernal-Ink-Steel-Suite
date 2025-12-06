using InfernalInkSteelSuite.Domain;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
{
    public interface IClientRepository
    {
        Client? Get(int id);
        List<Client> GetAll();
        void Insert(Client client);
        void Update(Client client);
        void Delete(int id);
        string? GetClientNameById(int clientId);
        int? GetClientIdByName(string name);
        int? GetClientIdByEmail(string email);
        int? GetClientIdByPhone(string phone);

        // Async methods for API
        Task<List<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<Client> AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(int id);
    }
}
