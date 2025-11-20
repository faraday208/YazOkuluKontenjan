using YazOkulu.Application.DTOs.Auth;

namespace YazOkulu.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RequestOtpAsync(RequestOtpDto dto);
    Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
}
