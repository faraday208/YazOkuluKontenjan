using Microsoft.Extensions.Logging;
using YazOkulu.Application.Services.Interfaces;

namespace YazOkulu.Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendOtpAsync(string phoneNumber, string code)
    {
        // MOCK: Gerçek SMS entegrasyonu yapılacaksa buraya eklenebilir
        // Örnek: Twilio, Vonage, Netgsm, İletimerkezi, vb.

        _logger.LogInformation("SMS gönderiliyor - Telefon: {PhoneNumber}, Kod: {Code}", phoneNumber, code);

        // Simüle et
        await Task.Delay(100);

        _logger.LogInformation("SMS başarıyla gönderildi - Telefon: {PhoneNumber}", phoneNumber);

        return true;
    }
}
