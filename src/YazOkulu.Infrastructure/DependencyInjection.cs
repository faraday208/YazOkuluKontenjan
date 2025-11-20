using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YazOkulu.Application.Services.Interfaces;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Interfaces;
using YazOkulu.Infrastructure.Data;
using YazOkulu.Infrastructure.Repositories;
using YazOkulu.Infrastructure.Services;

namespace YazOkulu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IRepository<Course>, CourseRepository>(); // Specialized repository for Course

        // Services
        services.AddScoped<ISmsService, SmsService>();

        return services;
    }
}
