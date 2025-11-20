using YazOkulu.Application.DTOs.Courses;

namespace YazOkulu.Application.Services.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync(int? studentId = null);
    Task<CourseDto?> GetCourseByIdAsync(int courseId, int? studentId = null);
    Task<CourseDto?> UpdateCourseAsync(int courseId, UpdateCourseDto updateDto);
}
