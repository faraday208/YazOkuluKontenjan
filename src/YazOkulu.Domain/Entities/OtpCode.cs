using YazOkulu.Domain.Enums;

namespace YazOkulu.Domain.Entities;

public class OtpCode : BaseEntity
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // 6 haneli kod
    public DateTime ExpiresAt { get; set; } // Geçerlilik süresi (5 dakika)
    public OtpStatus Status { get; set; } = OtpStatus.Active;
    public DateTime? UsedAt { get; set; }

    // Business logic helpers
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsValid => Status == OtpStatus.Active && !IsExpired;
}
