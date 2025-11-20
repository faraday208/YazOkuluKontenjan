using YazOkulu.Domain.Enums;

namespace YazOkulu.Domain.Entities;

public class CourseApplication : BaseEntity
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }

    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
}
