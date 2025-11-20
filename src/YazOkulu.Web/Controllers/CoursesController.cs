using Microsoft.AspNetCore.Mvc;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.DTOs.Common;
using YazOkulu.Application.DTOs.Courses;

namespace YazOkulu.Web.Controllers;

public class CoursesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CoursesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var studentId = HttpContext.Session.GetInt32("StudentId");
        if (studentId == null)
            return RedirectToAction("Login", "Account");

        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var response = await client.GetAsync($"/api/courses?studentId={studentId}");
        var courses = await response.Content.ReadFromJsonAsync<List<CourseDto>>();

        ViewBag.StudentName = HttpContext.Session.GetString("StudentName");
        return View(courses ?? new List<CourseDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Apply(int courseId)
    {
        var studentId = HttpContext.Session.GetInt32("StudentId");
        if (studentId == null)
            return RedirectToAction("Login", "Account");

        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var dto = new CreateApplicationDto { CourseId = courseId };

        var response = await client.PostAsJsonAsync($"/api/course-applications?studentId={studentId}", dto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ApplicationDto>>();

        if (result?.Success == true)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result?.Message ?? "Başvuru yapılırken hata oluştu";
        }

        return RedirectToAction("Index");
    }
}
