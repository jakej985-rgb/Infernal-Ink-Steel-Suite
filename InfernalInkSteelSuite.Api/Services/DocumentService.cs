using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.Extensions.Options;

namespace InfernalInkSteelSuite.Api.Services;

public class DocumentService(IDocumentRepository repository, IConfiguration config)
{
    private readonly IDocumentRepository _repository = repository;
    private readonly string _rootPath = config["FileStorage:RootPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    public async Task<Document> UploadDocumentAsync(int clientId, int uploadedByUserId, string title, IFormFile file)
    {
        var clientDir = Path.Combine(_rootPath, "Clients", clientId.ToString());
        if (!Directory.Exists(clientDir))
        {
            Directory.CreateDirectory(clientDir);
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(clientDir, fileName);
        var relativePath = Path.Combine("Clients", clientId.ToString(), fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var document = new Document
        {
            ClientId = clientId,
            UploadedByUserId = uploadedByUserId,
            Title = string.IsNullOrWhiteSpace(title) ? file.FileName : title,
            FilePath = relativePath,
            CreatedAt = DateTime.UtcNow
        };

        _repository.Insert(document);
        return document;
    }

    public List<Document> GetAllDocuments()
    {
        return _repository.GetAll();
    }

    public (Stream? fileStream, string contentType, string fileName) GetFile(int documentId)
    {
        var doc = _repository.Get(documentId);
        if (doc == null) return (null, "", "");

        var fullPath = Path.Combine(_rootPath, doc.FilePath);
        if (!File.Exists(fullPath)) return (null, "", "");

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        var contentType = "application/octet-stream"; // Can be improved with a MIME type map

        return (stream, contentType, Path.GetFileName(doc.FilePath));
    }

    public Document? GetMetadata(int id)
    {
        return _repository.Get(id);
    }

    public List<Document> GetDocumentsForClient(int clientId)
    {
        // The repository method 'GetDocuments' is filtered by userId and role, or returns all.
        // We need a specific method to get by ClientId, but for now we can filter in memory or add a repo method.
        // Adding a repo method is better, but let's check if 'GetAll' is efficient enough or if we should just modify the repo.
        // Given the instructions, I should probably stick to what I have or extend slightly.
        // The current repo doesn't have GetByClientId. I'll add a helper here or modify repo.
        // Since I can't easily modify the interface across projects without recompiling everything carefully,
        // and 'GetAll' might be heavy, I will use raw SQL here or just filter GetAll if the list is small.
        // Actually, let's look at the Repo again. It uses raw SQL.
        // I will add a new method to the Repo interface and implementation?
        // Or just filter GetAll() for now to be safe with existing code?
        // Wait, 'GetDocuments' takes userId. Maybe that's what we need if documents belong to a user?
        // No, documents belong to a Client (ClientId).

        // Let's filter GetAll() for now to avoid breaking changes if I don't need to.
        return [.. _repository.GetAll().Where(d => d.ClientId == clientId)];
    }

    public void DeleteDocument(int id)
    {
        var doc = _repository.Get(id);
        if (doc != null)
        {
            var fullPath = Path.Combine(_rootPath, doc.FilePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            _repository.Delete(id);
        }
    }
}
