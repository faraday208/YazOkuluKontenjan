using FluentValidation;
using YazOkulu.Application.DTOs.Auth;

namespace YazOkulu.Application.Validators;

public class RequestOtpValidator : AbstractValidator<RequestOtpDto>
{
    public RequestOtpValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz")
            .Matches(@"^(\+90|0)?[0-9]{10}$").WithMessage("Geçerli bir telefon numarası giriniz (örn: 05XXXXXXXXX)");
    }
}
