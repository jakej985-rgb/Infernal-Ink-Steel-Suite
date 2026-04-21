using InfernalInkSteelSuite.Domain;
using System;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.Repositories
{
    public interface IDocumentRepository
    {
        Document? Get(int id);
        List<Document> GetAll();
        List<Document> GetDocuments(int userId, string role, int maxDocuments, out bool truncated);
        void Insert(Document document);
        void Update(Document document);
        void Delete(int id);
        List<Document> GetByClientId(int clientId);
    }
}
