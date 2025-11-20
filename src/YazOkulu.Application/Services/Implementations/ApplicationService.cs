using AutoMapper;
using Microsoft.Extensions.Logging;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.DTOs.Common;
using YazOkulu.Application.Services.Interfaces;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Enums;
using YazOkulu.Domain.Interfaces;

namespace YazOkulu.Application.Services.Implementations;

public class ApplicationService : IApplicationService
{
    private readonly IRepository<CourseApplication> _applicationRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Student> _studentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(
        IRepository<CourseApplication> applicationRepository,
        IRepository<Course> courseRepository,
        IRepository<Student> studentRepository,
        IMapper mapper,
        ILogger<ApplicationService> logger)
    {
        _applicationRepository = applicationRepository;
        _courseRepository = courseRepository;
        _studentRepository = studentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<ApplicationDto>> CreateApplicationAsync(int studentId, CreateApplicationDto dto)
    {
        try
        {
            // Öğrenci var mı kontrol et
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
            {
                return ApiResponse<ApplicationDto>.ErrorResponse("Öğrenci bulunamadı");
            }

            // Ders var mı kontrol et
            var course = await _courseRepository.GetByIdAsync(dto.CourseId);
            if (course == null)
            {
                return ApiResponse<ApplicationDto>.ErrorResponse("Ders bulunamadı");
            }

            // Daha önce başvurmuş mu kontrol et
            var existingApplication = await _applicationRepository.ExistsAsync(a =>
                a.StudentId == studentId && a.CourseId == dto.CourseId);

            if (existingApplication)
            {
                return ApiResponse<ApplicationDto>.ErrorResponse("Bu derse zaten başvurdunuz");
            }

            // Kontenjan dolu mu kontrol et
            if (course.IsQuotaFull)
            {
                return ApiResponse<ApplicationDto>.ErrorResponse("Bu dersin kontenjanı dolmuştur");
            }

            // Başvuru oluştur
            var application = new CourseApplication
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                Status = ApplicationStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            var createdApplication = await _applicationRepository.AddAsync(application);

            // Navigation properties'i manuel yükle
            createdApplication.Student = (await _studentRepository.GetByIdAsync(studentId))!;
            createdApplication.Course = (await _courseRepository.GetByIdAsync(dto.CourseId))!;

            var applicationDto = _mapper.Map<ApplicationDto>(createdApplication);

            _logger.LogInformation("Başvuru oluşturuldu - ÖğrenciId: {StudentId}, DersId: {CourseId}",
                studentId, dto.CourseId);

            return ApiResponse<ApplicationDto>.SuccessResponse(applicationDto, "Başvurunuz alınmıştır");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Başvuru oluşturma hatası - ÖğrenciId: {StudentId}, DersId: {CourseId}",
                studentId, dto.CourseId);
            return ApiResponse<ApplicationDto>.ErrorResponse("Başvuru oluşturulurken bir hata oluştu");
        }
    }

    public async Task<IEnumerable<ApplicationDto>> GetStudentApplicationsAsync(int studentId)
    {
        var applications = (await _applicationRepository.FindAsync(a => a.StudentId == studentId)).ToList();

        // Navigation properties'i manuel yükle
        foreach (var app in applications)
        {
            app.Student = (await _studentRepository.GetByIdAsync(app.StudentId))!;
            app.Course = (await _courseRepository.GetByIdAsync(app.CourseId))!;
        }

        return _mapper.Map<List<ApplicationDto>>(applications.OrderByDescending(a => a.AppliedAt));
    }

    public async Task<IEnumerable<ApplicationDto>> GetCourseApplicationsAsync(int courseId)
    {
        var applications = (await _applicationRepository.FindAsync(a => a.CourseId == courseId)).ToList();

        // Navigation properties'i manuel yükle
        foreach (var app in applications)
        {
            app.Student = (await _studentRepository.GetByIdAsync(app.StudentId))!;
            app.Course = (await _courseRepository.GetByIdAsync(app.CourseId))!;
        }

        return _mapper.Map<List<ApplicationDto>>(applications.OrderByDescending(a => a.AppliedAt));
    }

    public async Task<ApiResponse<ApplicationDto>> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusDto dto)
    {
        try
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);

            if (application == null)
            {
                return ApiResponse<ApplicationDto>.ErrorResponse("Başvuru bulunamadı");
            }

            // Navigation properties'i yükle
            application.Student = (await _studentRepository.GetByIdAsync(application.StudentId))!;
            application.Course = (await _courseRepository.GetByIdAsync(application.CourseId))!;

            // Kontenjan kontrolü (Onaylama durumunda)
            if (dto.Status == ApplicationStatus.Approved)
            {
                var course = application.Course;
                if (course.IsQuotaFull)
                {
                    return ApiResponse<ApplicationDto>.ErrorResponse("Bu dersin kontenjanı dolmuştur");
                }
            }

            // Başvuru durumunu güncelle
            application.Status = dto.Status;
            application.ReviewedAt = DateTime.UtcNow;
            application.ReviewNotes = dto.ReviewNotes;

            await _applicationRepository.UpdateAsync(application);

            var applicationDto = _mapper.Map<ApplicationDto>(application);

            _logger.LogInformation("Başvuru durumu güncellendi - BaşvuruId: {ApplicationId}, Durum: {Status}",
                applicationId, dto.Status);

            return ApiResponse<ApplicationDto>.SuccessResponse(applicationDto, "Başvuru durumu güncellendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Başvuru durumu güncelleme hatası - BaşvuruId: {ApplicationId}", applicationId);
            return ApiResponse<ApplicationDto>.ErrorResponse("Durum güncellenirken bir hata oluştu");
        }
    }
}
