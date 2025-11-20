using YazOkulu.Domain.Enums;

namespace YazOkulu.Application.DTOs.Applications;

public class UpdateApplicationStatusDto
{
    public ApplicationStatus Status { get; set; }
    public string? ReviewNotes { get; set; }
}
