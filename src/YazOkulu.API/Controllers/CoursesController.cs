using Microsoft.AspNetCore.Mvc;
using YazOkulu.Application.DTOs.Courses;
using YazOkulu.Application.Services.Interfaces;

namespace YazOkulu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
    {
        _courseService = courseService;
        _logger = logger;
    }

    /// <summary>
    /// Tüm dersleri listeler
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll([FromQuery] int? studentId = null)
    {
        var courses = await _courseService.GetAllCoursesAsync(studentId);
        return Ok(courses);
    }

    /// <summary>
    /// Belirli bir dersin detaylarını getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetById(int id, [FromQuery] int? studentId = null)
    {
        var course = await _courseService.GetCourseByIdAsync(id, studentId);
        if (course == null)
            return NotFound(new { message = "Ders bulunamadı" });

        return Ok(course);
    }

    /// <summary>
    /// Dersi günceller
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDto>> Update(int id, [FromBody] UpdateCourseDto updateDto)
    {
        var course = await _courseService.UpdateCourseAsync(id, updateDto);
        if (course == null)
            return NotFound(new { message = "Ders bulunamadı" });

        _logger.LogInformation("Ders güncellendi: {CourseId}", id);
        return Ok(course);
    }
}
