namespace YazOkulu.Domain.Entities;

public class Student : BaseEntity
{
    public string PhoneNumber { get; set; } = string.Empty; // GSM numarası (zorunlu, unique)
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty; // Öğrenci numarası
    public string Department { get; set; } = string.Empty; // Bölüm

    // Navigation properties
    public virtual ICollection<CourseApplication> Applications { get; set; } = new List<CourseApplication>();
}
