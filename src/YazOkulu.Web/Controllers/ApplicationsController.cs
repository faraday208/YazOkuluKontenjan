using Microsoft.AspNetCore.Mvc;
using YazOkulu.Application.DTOs.Applications;

namespace YazOkulu.Web.Controllers;

public class ApplicationsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApplicationsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var studentId = HttpContext.Session.GetInt32("StudentId");
        if (studentId == null)
            return RedirectToAction("Login", "Account");

        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var response = await client.GetAsync($"/api/me/applications?studentId={studentId}");
        var applications = await response.Content.ReadFromJsonAsync<List<ApplicationDto>>();

        ViewBag.StudentName = HttpContext.Session.GetString("StudentName");
        return View(applications ?? new List<ApplicationDto>());
    }
}
