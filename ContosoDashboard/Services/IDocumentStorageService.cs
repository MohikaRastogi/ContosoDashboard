namespace ContosoDashboard.Services;

public interface IDocumentStorageService
{
    Task<string> UploadAsync(Stream content, string originalFileName, string contentType, long length, int userId, int? projectId, CancellationToken cancellationToken = default);
    Task<Stream?> DownloadAsync(string storageKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}