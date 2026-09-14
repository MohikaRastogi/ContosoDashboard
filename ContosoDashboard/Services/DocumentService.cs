using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<DocumentUploadResult> CreateAsync(DocumentUploadRequest request, CancellationToken cancellationToken = default);
    Task<List<Document>> GetUserDocumentsAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<Document>> GetProjectDocumentsAsync(int projectId, int userId, CancellationToken cancellationToken = default);
}

public sealed class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IDocumentStorageService _storage;

    public DocumentService(ApplicationDbContext context, IDocumentStorageService storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<DocumentUploadResult> CreateAsync(DocumentUploadRequest request, CancellationToken cancellationToken = default)
    {
        var title = request.Title?.Trim();
        var category = request.Category?.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            return DocumentUploadResult.Failure("A document title is required.");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            return DocumentUploadResult.Failure("A document category is required.");
        }

        if (title.Length > 255 || category.Length > 100)
        {
            return DocumentUploadResult.Failure("The title or category is too long.");
        }

        if (request.ProjectId.HasValue && !await CanAccessProjectAsync(request.ProjectId.Value, request.UploaderId, cancellationToken))
        {
            return DocumentUploadResult.Failure("You do not have access to the selected project.");
        }

        string storageKey;
        try
        {
            storageKey = await _storage.UploadAsync(
                request.Content,
                request.OriginalFileName,
                request.ContentType,
                request.FileSizeBytes,
                request.UploaderId,
                request.ProjectId,
                cancellationToken);
        }
        catch (InvalidDataException ex)
        {
            return DocumentUploadResult.Failure(ex.Message);
        }

        var document = new Document
        {
            Title = title,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Category = category,
            UploaderId = request.UploaderId,
            ProjectId = request.ProjectId,
            StoredFileName = Path.GetFileName(storageKey),
            StoredFilePath = storageKey,
            FileSizeBytes = checked((int)request.FileSizeBytes),
            ContentType = request.ContentType,
            UploadedUtc = DateTime.UtcNow
        };

        try
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _storage.DeleteAsync(storageKey, cancellationToken);
            throw;
        }

        return DocumentUploadResult.Success(document);
    }

    public async Task<List<Document>> GetUserDocumentsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .AsNoTracking()
            .Include(d => d.Project)
            .Include(d => d.Uploader)
            .Where(d => !d.IsDeleted && (d.UploaderId == userId ||
                (d.ProjectId.HasValue && _context.ProjectMembers.Any(pm => pm.ProjectId == d.ProjectId && pm.UserId == userId)) ||
                (d.ProjectId.HasValue && _context.Projects.Any(p => p.ProjectId == d.ProjectId && p.ProjectManagerId == userId)) ||
                d.Shares.Any(s => s.SharedWithUserId == userId && s.IsActive)))
            .OrderByDescending(d => d.UploadedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Document>> GetProjectDocumentsAsync(int projectId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await CanAccessProjectAsync(projectId, userId, cancellationToken))
        {
            return new List<Document>();
        }

        return await _context.Documents
            .AsNoTracking()
            .Include(d => d.Uploader)
            .Where(d => d.ProjectId == projectId && !d.IsDeleted)
            .OrderByDescending(d => d.UploadedUtc)
            .ToListAsync(cancellationToken);
    }

    private async Task<bool> CanAccessProjectAsync(int projectId, int userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (user?.Role == UserRole.Administrator)
        {
            return true;
        }

        return await _context.Projects.AnyAsync(p => p.ProjectId == projectId &&
            (p.ProjectManagerId == userId || p.ProjectMembers.Any(pm => pm.UserId == userId)), cancellationToken);
    }
}

public sealed record DocumentUploadRequest(
    int UploaderId,
    string Title,
    string Category,
    string? Description,
    int? ProjectId,
    Stream Content,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes);

public sealed class DocumentUploadResult
{
    private DocumentUploadResult(bool succeeded, string? error, Document? document)
    {
        Succeeded = succeeded;
        Error = error;
        Document = document;
    }

    public bool Succeeded { get; }
    public string? Error { get; }
    public Document? Document { get; }

    public static DocumentUploadResult Success(Document document) => new(true, null, document);
    public static DocumentUploadResult Failure(string error) => new(false, error, null);
}