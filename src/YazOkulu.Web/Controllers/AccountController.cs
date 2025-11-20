using Microsoft.AspNetCore.Mvc;
using YazOkulu.Application.DTOs.Auth;

namespace YazOkulu.Web.Controllers;

public class CheckPhoneResponseDto
{
    public bool IsRegistered { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CheckPhone(string phoneNumber)
    {
        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var response = await client.GetAsync($"/api/auth/check-phone/{Uri.EscapeDataString(phoneNumber)}");
        var result = await response.Content.ReadFromJsonAsync<CheckPhoneResponseDto>();

        if (result?.IsRegistered == true)
        {
            // Kayıtlı → OTP gönder
            return await RequestOtp(phoneNumber);
        }
        else
        {
            // Kayıtsız → Kayıt formuna yönlendir
            TempData["PhoneNumber"] = phoneNumber;
            return RedirectToAction("Register");
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.PhoneNumber = TempData["PhoneNumber"];
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterStudentDto dto)
    {
        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        if (result?.Success == true)
        {
            TempData["SuccessMessage"] = result.Message;
            // Kayıt başarılı, OTP gönder
            return await RequestOtp(dto.PhoneNumber);
        }

        TempData["ErrorMessage"] = result?.Message ?? "Kayıt başarısız";
        TempData["PhoneNumber"] = dto.PhoneNumber;
        return RedirectToAction("Register");
    }

    private async Task<IActionResult> RequestOtp(string phoneNumber)
    {
        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var dto = new RequestOtpDto { PhoneNumber = phoneNumber };

        var response = await client.PostAsJsonAsync("/api/auth/request-otp", dto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        if (result?.Success == true)
        {
            TempData["PhoneNumber"] = phoneNumber;
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("VerifyOtp");
        }

        TempData["ErrorMessage"] = result?.Message ?? "Kod gönderilirken hata oluştu";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult VerifyOtp()
    {
        ViewBag.PhoneNumber = TempData["PhoneNumber"];
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOtp(string phoneNumber, string code)
    {
        var client = _httpClientFactory.CreateClient("YazOkuluAPI");
        var dto = new VerifyOtpDto { PhoneNumber = phoneNumber, Code = code };

        var response = await client.PostAsJsonAsync("/api/auth/verify-otp", dto);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        if (result?.Success == true && result.Student != null)
        {
            HttpContext.Session.SetInt32("StudentId", result.Student.Id);
            HttpContext.Session.SetString("StudentName", $"{result.Student.FirstName} {result.Student.LastName}");
            return RedirectToAction("Index", "Courses");
        }

        TempData["ErrorMessage"] = result?.Message ?? "Doğrulama başarısız";
        TempData["PhoneNumber"] = phoneNumber;
        return RedirectToAction("VerifyOtp");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
