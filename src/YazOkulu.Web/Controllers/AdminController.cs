using Microsoft.AspNetCore.Mvc;
using YazOkulu.Application.DTOs.Applications;
using YazOkulu.Application.DTOs.Common;
using YazOkulu.Application.DTOs.Courses;
using YazOkulu.Domain.Enums;

namespace YazOkulu.Web.Controllers;

public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var response = await client.PostAsJsonAsync("/api/admin/login", new { username, password });

        if (response.IsSuccessStatusCode)
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = "Kullanıcı adı veya şifre hatalı";
        return View();
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToAction("Login");

        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var response = await client.GetAsync("/api/courses");
        var courses = await response.Content.ReadFromJsonAsync<List<CourseDto>>();

        return View(courses ?? new List<CourseDto>());
    }

    public async Task<IActionResult> Applications(int courseId)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToAction("Login");

        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var response = await client.GetAsync($"/api/courses/{courseId}/applications");
        var applications = await response.Content.ReadFromJsonAsync<List<ApplicationDto>>();

        ViewBag.CourseId = courseId;
        return View(applications ?? new List<ApplicationDto>());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, int courseId, ApplicationStatus status)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToAction("Login");

        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var dto = new UpdateApplicationStatusDto { Status = status };

        var response = await client.PutAsJsonAsync($"/api/course-applications/{id}/status", dto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ApplicationDto>>();

        if (result?.Success == true)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result?.Message ?? "Güncelleme başarısız";
        }

        return RedirectToAction("Applications", new { courseId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToAction("Login");

        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var response = await client.GetAsync($"/api/courses/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Ders bulunamadı";
            return RedirectToAction("Index");
        }

        var course = await response.Content.ReadFromJsonAsync<CourseDto>();
        return View(course);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CourseDto model)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToAction("Login");

        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var updateDto = new UpdateCourseDto
        {
            Code = model.Code,
            Name = model.Name,
            Quota = model.Quota,
            Department = model.Department,
            Faculty = model.Faculty,
            Instructor = model.Instructor,
            Credits = model.Credits
        };

        var response = await client.PutAsJsonAsync($"/api/courses/{id}", updateDto);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Ders başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = "Ders güncellenemedi";
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
