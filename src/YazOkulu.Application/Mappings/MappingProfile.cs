using AutoMapper;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.DTOs.Auth;
using YazOkulu.Application.DTOs.Courses;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Enums;

namespace YazOkulu.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Student mappings
        CreateMap<Student, StudentInfoDto>();

        // Course mappings
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.AvailableQuota, opt => opt.MapFrom(src => src.AvailableQuota))
            .ForMember(dest => dest.IsQuotaFull, opt => opt.MapFrom(src => src.IsQuotaFull))
            .ForMember(dest => dest.HasApplied, opt => opt.Ignore());

        // CourseApplication mappings
        CreateMap<CourseApplication, ApplicationDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
            .ForMember(dest => dest.StudentPhone, opt => opt.MapFrom(src => src.Student.PhoneNumber))
            .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.Code))
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => GetStatusText(src.Status)));
    }

    private static string GetStatusText(ApplicationStatus status)
    {
        return status switch
        {
            ApplicationStatus.Pending => "Beklemede",
            ApplicationStatus.Approved => "Onaylandı",
            ApplicationStatus.Rejected => "Reddedildi",
            _ => "Bilinmiyor"
        };
    }
}
