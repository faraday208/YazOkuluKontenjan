using YazOkulu.Application.DTOs.Auth;

namespace YazOkulu.Application.Services.Interfaces;

public interface IAuthService
{
    Task<CheckPhoneResponseDto> CheckPhoneNumberAsync(string phoneNumber);
    Task<AuthResponseDto> RegisterStudentAsync(RegisterStudentDto dto);
    Task<AuthResponseDto> RequestOtpAsync(RequestOtpDto dto);
    Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
}
