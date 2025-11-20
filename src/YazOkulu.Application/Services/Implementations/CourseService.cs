using AutoMapper;
using Microsoft.Extensions.Logging;
using YazOkulu.Application.DTOs.Courses;
using YazOkulu.Application.Services.Interfaces;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Interfaces;

namespace YazOkulu.Application.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<CourseApplication> _applicationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CourseService> _logger;

    public CourseService(
        IRepository<Course> courseRepository,
        IRepository<CourseApplication> applicationRepository,
        IMapper mapper,
        ILogger<CourseService> logger)
    {
        _courseRepository = courseRepository;
        _applicationRepository = applicationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(int? studentId = null)
    {
        var courses = await _courseRepository.GetAllAsync();
        var courseDtos = _mapper.Map<List<CourseDto>>(courses);

        // Tüm başvuruları getir (bekleyen başvuru sayısı için)
        var allApplications = await _applicationRepository.GetAllAsync();

        // Her ders için bekleyen başvuru sayısını hesapla
        foreach (var courseDto in courseDtos)
        {
            courseDto.PendingApplicationsCount = allApplications
                .Count(a => a.CourseId == courseDto.Id && a.Status == Domain.Enums.ApplicationStatus.Pending);
        }

        // Öğrenci giriş yapmışsa, başvuru durumlarını kontrol et
        if (studentId.HasValue)
        {
            var studentApplications = allApplications.Where(a => a.StudentId == studentId.Value).ToList();
            var appliedCourseIds = studentApplications.Select(a => a.CourseId).ToHashSet();

            foreach (var courseDto in courseDtos)
            {
                courseDto.HasApplied = appliedCourseIds.Contains(courseDto.Id);

                // Başvuru durumunu set et
                var application = studentApplications.FirstOrDefault(a => a.CourseId == courseDto.Id);
                if (application != null)
                {
                    courseDto.ApplicationStatus = application.Status;
                    courseDto.ApplicationStatusText = application.Status switch
                    {
                        Domain.Enums.ApplicationStatus.Pending => "Beklemede",
                        Domain.Enums.ApplicationStatus.Approved => "Onaylandı",
                        Domain.Enums.ApplicationStatus.Rejected => "Reddedildi",
                        _ => "Bilinmiyor"
                    };
                }
            }
        }

        return courseDtos.OrderBy(c => c.Code);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int courseId, int? studentId = null)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
            return null;

        var courseDto = _mapper.Map<CourseDto>(course);

        if (studentId.HasValue)
        {
            var hasApplied = await _applicationRepository.ExistsAsync(a =>
                a.StudentId == studentId.Value && a.CourseId == courseId);
            courseDto.HasApplied = hasApplied;
        }

        return courseDto;
    }

    public async Task<CourseDto?> UpdateCourseAsync(int courseId, UpdateCourseDto updateDto)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            _logger.LogWarning("Ders bulunamadı. CourseId: {CourseId}", courseId);
            return null;
        }

        // Update course properties
        course.Code = updateDto.Code;
        course.Name = updateDto.Name;
        course.Quota = updateDto.Quota;
        course.Department = updateDto.Department;
        course.Faculty = updateDto.Faculty;
        course.Instructor = updateDto.Instructor;
        course.Credits = updateDto.Credits;

        await _courseRepository.UpdateAsync(course);

        _logger.LogInformation("Ders güncellendi. CourseId: {CourseId}, Code: {Code}", courseId, course.Code);

        return _mapper.Map<CourseDto>(course);
    }
}
