using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YazOkulu.Domain.Entities;

namespace YazOkulu.Infrastructure.Data.Configurations;

public class CourseApplicationConfiguration : IEntityTypeConfiguration<CourseApplication>
{
    public void Configure(EntityTypeBuilder<CourseApplication> builder)
    {
        builder.ToTable("CourseApplications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.StudentId)
            .IsRequired();

        builder.Property(a => a.CourseId)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.AppliedAt)
            .IsRequired();

        builder.Property(a => a.ReviewNotes)
            .HasMaxLength(500);

        // Bir öğrenci bir derse sadece 1 kez başvurabilir
        builder.HasIndex(a => new { a.StudentId, a.CourseId })
            .IsUnique();

        builder.HasOne(a => a.Student)
            .WithMany(s => s.Applications)
            .HasForeignKey(a => a.StudentId);

        builder.HasOne(a => a.Course)
            .WithMany(c => c.Applications)
            .HasForeignKey(a => a.CourseId);
    }
}
