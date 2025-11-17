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
        int GetClientIdByName(string name);
    }
}
