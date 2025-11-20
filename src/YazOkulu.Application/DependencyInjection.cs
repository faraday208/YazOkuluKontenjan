using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using YazOkulu.Application.Services.Implementations;
using YazOkulu.Application.Services.Interfaces;

namespace YazOkulu.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IApplicationService, ApplicationService>();

        return services;
    }
}
