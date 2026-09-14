namespace ContosoDashboard.Services;

public sealed class LocalDocumentStorageService : IDocumentStorageService
{
    public const long MaxFileSizeBytes = 25 * 1024 * 1024;

    private static readonly IReadOnlyDictionary<string, string> SupportedFileTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = "application/pdf",
            [".doc"] = "application/msword",
            [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            [".xls"] = "application/vnd.ms-excel",
            [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            [".ppt"] = "application/vnd.ms-powerpoint",
            [".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation"
        };

    private readonly string _storageRoot;

    public LocalDocumentStorageService(IHostEnvironment environment)
    {
        _storageRoot = Path.Combine(environment.ContentRootPath, "AppData", "uploads");
    }

    public async Task<string> UploadAsync(
        Stream content,
        string originalFileName,
        string contentType,
        long length,
        int userId,
        int? projectId,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(originalFileName);
        if (!SupportedFileTypes.TryGetValue(extension, out var expectedContentType))
        {
            throw new InvalidDataException("This file type is not supported. Upload a PDF or Office document.");
        }

        if (length <= 0 || length > MaxFileSizeBytes)
        {
            throw new InvalidDataException("Files must be greater than zero and no larger than 25 MB.");
        }

        if (!string.IsNullOrWhiteSpace(contentType) && !contentType.Equals(expectedContentType, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("The file content type does not match its extension.");
        }

        var scope = projectId.HasValue ? $"project-{projectId.Value}" : "personal";
        var relativePath = Path.Combine(userId.ToString(), scope, $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}");
        var fullPath = GetSafeFullPath(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var output = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(output, cancellationToken);
        return relativePath;
    }

    public Task<Stream?> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafeFullPath(storageKey);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetSafeFullPath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string GetSafeFullPath(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey) || Path.IsPathRooted(storageKey))
        {
            throw new InvalidDataException("The document storage key is invalid.");
        }

        var root = Path.GetFullPath(_storageRoot);
        var fullPath = Path.GetFullPath(Path.Combine(root, storageKey));
        if (!fullPath.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("The document storage key is invalid.");
        }

        return fullPath;
    }
}