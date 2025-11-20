using YazOkulu.Domain.Enums;

namespace YazOkulu.Application.DTOs.Applications;

public class ApplicationDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentPhone { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; }
    public string StatusText { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
}
