using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YazOkulu.Domain.Entities;

namespace YazOkulu.Infrastructure.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.Code)
            .IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Quota)
            .IsRequired();

        builder.Property(c => c.Department)
            .HasMaxLength(200);

        builder.Property(c => c.Faculty)
            .HasMaxLength(200);

        builder.Property(c => c.Instructor)
            .HasMaxLength(200);

        builder.Property(c => c.Credits)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasMany(c => c.Applications)
            .WithOne(a => a.Course)
            .HasForeignKey(a => a.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore computed properties
        builder.Ignore(c => c.ApprovedApplicationsCount);
        builder.Ignore(c => c.IsQuotaFull);
        builder.Ignore(c => c.AvailableQuota);
    }
}
