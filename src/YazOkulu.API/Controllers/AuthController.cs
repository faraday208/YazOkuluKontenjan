using Microsoft.AspNetCore.Mvc;
using YazOkulu.Application.DTOs.Auth;
using YazOkulu.Application.Services.Interfaces;

namespace YazOkulu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// GSM numarasına SMS ile OTP kodu gönderir
    /// </summary>
    [HttpPost("request-otp")]
    public async Task<ActionResult<AuthResponseDto>> RequestOtp([FromBody] RequestOtpDto dto)
    {
        var result = await _authService.RequestOtpAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// OTP kodunu doğrular ve giriş işlemini tamamlar
    /// </summary>
    [HttpPost("verify-otp")]
    public async Task<ActionResult<AuthResponseDto>> VerifyOtp([FromBody] VerifyOtpDto dto)
    {
        var result = await _authService.VerifyOtpAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
