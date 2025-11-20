namespace YazOkulu.Application.DTOs.Courses;

public class UpdateCourseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quota { get; set; }
    public string Department { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public int Credits { get; set; }
}
