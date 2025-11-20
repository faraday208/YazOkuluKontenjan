using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YazOkulu.Domain.Entities;

namespace YazOkulu.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(s => s.PhoneNumber)
            .IsUnique();

        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .HasMaxLength(200);

        builder.Property(s => s.StudentNumber)
            .HasMaxLength(50);

        builder.Property(s => s.Department)
            .HasMaxLength(200);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasMany(s => s.Applications)
            .WithOne(a => a.Student)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
