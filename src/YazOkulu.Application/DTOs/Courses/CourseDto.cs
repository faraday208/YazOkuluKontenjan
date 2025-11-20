namespace YazOkulu.Application.DTOs.Courses;

public class CourseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quota { get; set; }
    public int AvailableQuota { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int Credits { get; set; }
    public bool IsQuotaFull { get; set; }
    public bool HasApplied { get; set; } // Öğrencinin daha önce başvurup başvurmadığı
    public int PendingApplicationsCount { get; set; } // Bekleyen başvuru sayısı
    public YazOkulu.Domain.Enums.ApplicationStatus? ApplicationStatus { get; set; } // Başvuru durumu (varsa)
    public string? ApplicationStatusText { get; set; } // Başvuru durumu metni (varsa)
}
