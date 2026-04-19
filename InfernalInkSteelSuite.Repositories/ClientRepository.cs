using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfernalInkSteelSuite.Repositories
{
    public class ClientRepository(AppDbContext dbContext) : IClientRepository
    {
        private readonly AppDbContext _db = dbContext;

        public Client? Get(int id) => _db.Clients.Find(id);

        public List<Client> GetAll() => _db.Clients.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();

        public void Insert(Client client)
        {
            _db.Clients.Add(client);
            _db.SaveChanges();
        }

        public void Update(Client client)
        {
            _db.Clients.Update(client);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var client = _db.Clients.Find(id);
            if (client != null)
            {
                _db.Clients.Remove(client);
                _db.SaveChanges();
            }
        }

        public string? GetClientNameById(int clientId)
        {
            var client = _db.Clients.Find(clientId);
            return client?.FullName;
        }

        public int? GetClientIdByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            
            var nameParts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2) return null;

            string firstName = nameParts[0];
            string lastName = nameParts[^1];
            string? middleName = nameParts.Length > 2 ? string.Join(" ", nameParts, 1, nameParts.Length - 2) : null;

            IQueryable<Client> query = _db.Clients;
            
            if (!string.IsNullOrEmpty(middleName))
            {
                return query
                    .Where(c => c.FirstName.ToLower() == firstName.ToLower() && 
                                c.LastName.ToLower() == lastName.ToLower() && 
                                (c.MiddleName ?? "").ToLower() == middleName.ToLower())
                    .OrderByDescending(c => c.Id)
                    .Select(c => (int?)c.Id)
                    .FirstOrDefault();
            }

            return query
                .Where(c => c.FirstName.ToLower() == firstName.ToLower() && 
                            c.LastName.ToLower() == lastName.ToLower())
                .OrderBy(c => string.IsNullOrEmpty(c.MiddleName) ? 0 : 1)
                .ThenByDescending(c => c.Id)
                .Select(c => (int?)c.Id)
                .FirstOrDefault();
        }

        public int? GetClientIdByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return _db.Clients
                .Where(c => c.Email.ToLower() == email.Trim().ToLower())
                .Select(c => (int?)c.Id)
                .FirstOrDefault();
        }

        public int? GetClientIdByPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;
            return _db.Clients
                .Where(c => c.Phone == phone.Trim())
                .Select(c => (int?)c.Id)
                .FirstOrDefault();
        }

        // Async methods for API
        public async Task<List<Client>> GetAllAsync() => 
            await _db.Clients.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToListAsync();

        public async Task<Client?> GetByIdAsync(int id) => await _db.Clients.FindAsync(id);

        public async Task<Client> AddAsync(Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
            return client;
        }

        public async Task UpdateAsync(Client client)
        {
            _db.Clients.Update(client);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client != null)
            {
                _db.Clients.Remove(client);
                await _db.SaveChangesAsync();
            }
        }
    }
}
