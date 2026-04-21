using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.StaticFiles;

namespace InfernalInkSteelSuite.Api.Services;

public class DocumentService(IDocumentRepository repository, IConfiguration config)
{
    private readonly IDocumentRepository _repository = repository;
    private readonly string _rootPath = config["FileStorage:RootPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    public async Task<Document> UploadDocumentAsync(int clientId, int uploadedByUserId, string title, IFormFile file)
    {
        var clientDir = Path.Combine(_rootPath, "Clients", clientId.ToString());
        if (!Directory.Exists(clientDir))
        {
            Directory.CreateDirectory(clientDir);
        }

        // L5: Sanitize filename to prevent path traversal attacks
        var safeOriginalName = Path.GetFileName(file.FileName);
        var fileName = $"{Guid.NewGuid()}_{safeOriginalName}";
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
            Title = string.IsNullOrWhiteSpace(title) ? safeOriginalName : title,
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

        // L4: Proper MIME type detection instead of always returning octet-stream
        var fileName = Path.GetFileName(doc.FilePath);
        if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return (stream, contentType, fileName);
    }

    public Document? GetMetadata(int id)
    {
        return _repository.Get(id);
    }

    public List<Document> GetDocumentsForClient(int clientId)
    {
        // H7: Use efficient DB query via repository instead of loading all documents
        return _repository.GetByClientId(clientId);
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
