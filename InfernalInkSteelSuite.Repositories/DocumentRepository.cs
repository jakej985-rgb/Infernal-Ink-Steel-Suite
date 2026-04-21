using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfernalInkSteelSuite.Repositories
{
    public class DocumentRepository(AppDbContext dbContext) : IDocumentRepository
    {
        private readonly AppDbContext _db = dbContext;

        public Document? Get(int id) => _db.Documents.Find(id);

        public List<Document> GetAll() => _db.Documents.OrderByDescending(d => d.CreatedAt).ToList();

        public List<Document> GetDocuments(int userId, string role, int maxDocuments, out bool truncated)
        {
            truncated = false;
            if (maxDocuments <= 0) return new List<Document>();

            var query = _db.Documents.OrderByDescending(d => d.CreatedAt).AsQueryable();

            if (role != "Admin" && role != "Manager")
            {
                query = query.Where(d => d.UploadedByUserId == userId);
            }

            var results = query.Take(maxDocuments + 1).ToList();
            if (results.Count > maxDocuments)
            {
                truncated = true;
                return results.Take(maxDocuments).ToList();
            }

            return results;
        }

        public void Insert(Document document)
        {
            _db.Documents.Add(document);
            _db.SaveChanges();
        }

        public void Update(Document document)
        {
            _db.Documents.Update(document);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var doc = _db.Documents.Find(id);
            if (doc != null)
            {
                _db.Documents.Remove(doc);
                _db.SaveChanges();
            }
        }

        public List<Document> GetByClientId(int clientId)
        {
            return _db.Documents
                .Where(d => d.ClientId == clientId)
                .OrderByDescending(d => d.CreatedAt)
                .ToList();
        }
    }
}
