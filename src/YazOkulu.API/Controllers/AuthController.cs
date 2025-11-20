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
    /// Telefon numarasının kayıtlı olup olmadığını kontrol eder
    /// </summary>
    [HttpGet("check-phone/{phoneNumber}")]
    public async Task<ActionResult<CheckPhoneResponseDto>> CheckPhone(string phoneNumber)
    {
        var result = await _authService.CheckPhoneNumberAsync(phoneNumber);
        return Ok(result);
    }

    /// <summary>
    /// Yeni öğrenci kaydı oluşturur
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterStudentDto dto)
    {
        var result = await _authService.RegisterStudentAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
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
