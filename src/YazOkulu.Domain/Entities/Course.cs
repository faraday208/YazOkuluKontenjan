namespace YazOkulu.Domain.Entities;

public class Course : BaseEntity
{
    public string Code { get; set; } = string.Empty; // Ders kodu (ör: CS101)
    public string Name { get; set; } = string.Empty; // Ders adı
    public int Quota { get; set; } // Kontenjan
    public string Department { get; set; } = string.Empty; // Bölüm
    public string Faculty { get; set; } = string.Empty; // Fakülte
    public string Instructor { get; set; } = string.Empty; // Öğretim üyesi
    public int Credits { get; set; } // Kredi

    // Navigation properties
    public virtual ICollection<CourseApplication> Applications { get; set; } = new List<CourseApplication>();

    // Business logic helpers
    public int ApprovedApplicationsCount => Applications.Count(a => a.Status == Enums.ApplicationStatus.Approved);
    public bool IsQuotaFull => ApprovedApplicationsCount >= Quota;
    public int AvailableQuota => Math.Max(0, Quota - ApprovedApplicationsCount);
}
