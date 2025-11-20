using AutoMapper;
using Microsoft.Extensions.Logging;
using YazOkulu.Application.DTOs.Auth;
using YazOkulu.Application.Services.Interfaces;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Enums;
using YazOkulu.Domain.Interfaces;

namespace YazOkulu.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<OtpCode> _otpRepository;
    private readonly ISmsService _smsService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IRepository<Student> studentRepository,
        IRepository<OtpCode> otpRepository,
        ISmsService smsService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _studentRepository = studentRepository;
        _otpRepository = otpRepository;
        _smsService = smsService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CheckPhoneResponseDto> CheckPhoneNumberAsync(string phoneNumber)
    {
        var normalizedPhone = NormalizePhoneNumber(phoneNumber);
        var student = await _studentRepository.FirstOrDefaultAsync(s => s.PhoneNumber == normalizedPhone);

        return new CheckPhoneResponseDto
        {
            IsRegistered = student != null,
            Message = student != null ? "Telefon numarası kayıtlı" : "Telefon numarası kayıtlı değil"
        };
    }

    public async Task<AuthResponseDto> RegisterStudentAsync(RegisterStudentDto dto)
    {
        try
        {
            var normalizedPhone = NormalizePhoneNumber(dto.PhoneNumber);

            // Telefon numarası zaten kayıtlı mı kontrol et
            var existingStudent = await _studentRepository.FirstOrDefaultAsync(s => s.PhoneNumber == normalizedPhone);
            if (existingStudent != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Bu telefon numarası zaten kayıtlı"
                };
            }

            // Öğrenci numarası zaten kayıtlı mı kontrol et
            if (!string.IsNullOrWhiteSpace(dto.StudentNumber))
            {
                var existingStudentNumber = await _studentRepository.FirstOrDefaultAsync(s => s.StudentNumber == dto.StudentNumber);
                if (existingStudentNumber != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Bu öğrenci numarası zaten kayıtlı"
                    };
                }
            }

            // Yeni öğrenci oluştur
            var student = new Student
            {
                PhoneNumber = normalizedPhone,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                StudentNumber = dto.StudentNumber,
                Department = dto.Department
            };

            await _studentRepository.AddAsync(student);
            _logger.LogInformation("Yeni öğrenci kaydedildi - Telefon: {PhoneNumber}, Ad: {FirstName} {LastName}",
                normalizedPhone, dto.FirstName, dto.LastName);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Kayıt başarılı. Şimdi doğrulama kodu gönderilecek."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Öğrenci kaydı sırasında hata oluştu");
            return new AuthResponseDto
            {
                Success = false,
                Message = "Kayıt sırasında bir hata oluştu"
            };
        }
    }

    public async Task<AuthResponseDto> RequestOtpAsync(RequestOtpDto dto)
    {
        try
        {
            // Telefon numarasını normalize et
            var phoneNumber = NormalizePhoneNumber(dto.PhoneNumber);

            // 6 haneli random kod oluştur
            var code = GenerateOtpCode();

            // Eski aktif kodları expire et
            var existingCodes = await _otpRepository.FindAsync(o =>
                o.PhoneNumber == phoneNumber &&
                o.Status == OtpStatus.Active);

            foreach (var existingCode in existingCodes)
            {
                existingCode.Status = OtpStatus.Expired;
                await _otpRepository.UpdateAsync(existingCode);
            }

            // Yeni OTP kodu oluştur
            var otpCode = new OtpCode
            {
                PhoneNumber = phoneNumber,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Status = OtpStatus.Active
            };

            await _otpRepository.AddAsync(otpCode);

            // SMS gönder
            await _smsService.SendOtpAsync(phoneNumber, code);

            _logger.LogInformation("OTP kodu gönderildi - Telefon: {PhoneNumber}", phoneNumber);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Doğrulama kodu telefonunuza gönderildi. (5 dakika geçerli)"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OTP kodu gönderilirken hata oluştu");
            return new AuthResponseDto
            {
                Success = false,
                Message = "Kod gönderilirken bir hata oluştu. Lütfen tekrar deneyiniz."
            };
        }
    }

    public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto dto)
    {
        try
        {
            var phoneNumber = NormalizePhoneNumber(dto.PhoneNumber);

            // OTP kodunu bul
            var otpCode = await _otpRepository.FirstOrDefaultAsync(o =>
                o.PhoneNumber == phoneNumber &&
                o.Code == dto.Code &&
                o.Status == OtpStatus.Active);

            if (otpCode == null)
            {
                _logger.LogWarning("Geçersiz OTP kodu - Telefon: {PhoneNumber}", phoneNumber);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Geçersiz doğrulama kodu"
                };
            }

            if (otpCode.IsExpired)
            {
                otpCode.Status = OtpStatus.Expired;
                await _otpRepository.UpdateAsync(otpCode);

                _logger.LogWarning("Süresi dolmuş OTP kodu - Telefon: {PhoneNumber}", phoneNumber);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Doğrulama kodunun süresi dolmuş. Yeni kod talep ediniz."
                };
            }

            // OTP kodunu kullanıldı olarak işaretle
            otpCode.Status = OtpStatus.Used;
            otpCode.UsedAt = DateTime.UtcNow;
            await _otpRepository.UpdateAsync(otpCode);

            // Öğrenciyi bul
            var student = await _studentRepository.FirstOrDefaultAsync(s => s.PhoneNumber == phoneNumber);

            if (student == null)
            {
                _logger.LogWarning("Kayıtlı olmayan telefon numarası - Telefon: {PhoneNumber}", phoneNumber);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Bu telefon numarası kayıtlı değil. Lütfen önce kayıt olun."
                };
            }

            _logger.LogInformation("OTP doğrulandı - ÖğrenciId: {StudentId}", student.Id);

            // Token oluştur (basit session token - production'da JWT kullan)
            var token = GenerateSessionToken(student.Id);

            return new AuthResponseDto
            {
                Success = true,
                Message = "Giriş başarılı",
                Token = token,
                Student = _mapper.Map<StudentInfoDto>(student)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OTP doğrulama hatası");
            return new AuthResponseDto
            {
                Success = false,
                Message = "Doğrulama sırasında bir hata oluştu"
            };
        }
    }

    private static string NormalizePhoneNumber(string phoneNumber)
    {
        // Boşlukları ve özel karakterleri temizle
        phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        // +90 ile başlıyorsa kaldır
        if (phoneNumber.StartsWith("+90"))
            phoneNumber = phoneNumber.Substring(3);

        // 0 ile başlıyorsa kaldır
        if (phoneNumber.StartsWith("0"))
            phoneNumber = phoneNumber.Substring(1);

        return phoneNumber;
    }

    private static string GenerateOtpCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private static string GenerateSessionToken(int studentId)
    {
        // Basit token - Production'da JWT kullan
        return $"STU_{studentId}_{Guid.NewGuid():N}";
    }
}
