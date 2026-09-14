using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public int UploaderId { get; set; }

    public int? ProjectId { get; set; }

    [Required]
    [MaxLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(1024)]
    public string StoredFilePath { get; set; } = string.Empty;

    [Required]
    public int FileSizeBytes { get; set; }

    [Required]
    [MaxLength(255)]
    public string ContentType { get; set; } = string.Empty;

    public DateTime UploadedUtc { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    [ForeignKey(nameof(UploaderId))]
    public virtual User Uploader { get; set; } = null!;

    [ForeignKey(nameof(ProjectId))]
    public virtual Project? Project { get; set; }

    public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
}