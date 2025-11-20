using Microsoft.AspNetCore.Mvc;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.DTOs.Common;
using YazOkulu.Application.Services.Interfaces;

namespace YazOkulu.API.Controllers;

[ApiController]
[Route("api")]
public class AdminController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<AdminController> _logger;

    // HARDCODED ADMIN CREDENTIALS (Production'da kesinlikle değiştirilmeli!)
    private const string AdminUsername = "admin";
    private const string AdminPassword = "Admin123!";

    public AdminController(IApplicationService applicationService, ILogger<AdminController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// Admin giriş kontrolü (basit authentication)
    /// </summary>
    [HttpPost("admin/login")]
    public ActionResult<object> Login([FromBody] AdminLoginDto dto)
    {
        if (dto.Username == AdminUsername && dto.Password == AdminPassword)
        {
            return Ok(new
            {
                success = true,
                message = "Giriş başarılı",
                token = "ADMIN_TOKEN_" + Guid.NewGuid().ToString("N")
            });
        }

        return Unauthorized(new { success = false, message = "Kullanıcı adı veya şifre hatalı" });
    }

    /// <summary>
    /// Belirli bir derse yapılan tüm başvuruları listeler (Admin)
    /// </summary>
    [HttpGet("courses/{courseId}/applications")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetCourseApplications(int courseId)
    {
        var applications = await _applicationService.GetCourseApplicationsAsync(courseId);
        return Ok(applications);
    }

    /// <summary>
    /// Başvuru durumunu günceller (Onayla/Reddet) (Admin)
    /// </summary>
    [HttpPut("course-applications/{id}/status")]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> UpdateApplicationStatus(
        int id,
        [FromBody] UpdateApplicationStatusDto dto)
    {
        var result = await _applicationService.UpdateApplicationStatusAsync(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public class AdminLoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
