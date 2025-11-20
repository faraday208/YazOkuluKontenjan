using Microsoft.AspNetCore.Mvc;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.DTOs.Common;
using YazOkulu.Application.Services.Interfaces;

namespace YazOkulu.API.Controllers;

[ApiController]
[Route("api/course-applications")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(IApplicationService applicationService, ILogger<ApplicationsController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// Derse başvuru yapar
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApplicationDto>>> CreateApplication(
        [FromQuery] int studentId,
        [FromBody] CreateApplicationDto dto)
    {
        var result = await _applicationService.CreateApplicationAsync(studentId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Öğrencinin tüm başvurularını getirir
    /// </summary>
    [HttpGet("~/api/me/applications")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetMyApplications([FromQuery] int studentId)
    {
        var applications = await _applicationService.GetStudentApplicationsAsync(studentId);
        return Ok(applications);
    }
}
