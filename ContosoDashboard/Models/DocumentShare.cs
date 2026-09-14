using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class DocumentShare
{
    [Key]
    public int ShareId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    [Required]
    public int SharedByUserId { get; set; }

    [Required]
    public int SharedWithUserId { get; set; }

    public DateTime SharedUtc { get; set; } = DateTime.UtcNow;

    public bool NotificationSent { get; set; }

    public bool IsActive { get; set; } = true;

    [ForeignKey(nameof(DocumentId))]
    public virtual Document Document { get; set; } = null!;

    [ForeignKey(nameof(SharedByUserId))]
    public virtual User SharedByUser { get; set; } = null!;

    [ForeignKey(nameof(SharedWithUserId))]
    public virtual User SharedWithUser { get; set; } = null!;
}