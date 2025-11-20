using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.DTOs.Common;

namespace YazOkulu.Application.Services.Interfaces;

public interface IApplicationService
{
    Task<ApiResponse<ApplicationDto>> CreateApplicationAsync(int studentId, CreateApplicationDto dto);
    Task<IEnumerable<ApplicationDto>> GetStudentApplicationsAsync(int studentId);
    Task<IEnumerable<ApplicationDto>> GetCourseApplicationsAsync(int courseId);
    Task<ApiResponse<ApplicationDto>> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusDto dto);
}
