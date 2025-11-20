using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YazOkulu.Domain.Entities;

namespace YazOkulu.Infrastructure.Data.Configurations;

public class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("OtpCodes");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Code)
            .IsRequired()
            .HasMaxLength(6);

        builder.Property(o => o.ExpiresAt)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(o => o.PhoneNumber);
        builder.HasIndex(o => o.ExpiresAt);

        // Ignore computed properties
        builder.Ignore(o => o.IsExpired);
        builder.Ignore(o => o.IsValid);
    }
}
