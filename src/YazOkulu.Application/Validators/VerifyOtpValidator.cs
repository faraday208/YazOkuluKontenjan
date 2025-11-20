using FluentValidation;
using YazOkulu.Application.DTOs.Auth;

namespace YazOkulu.Application.Validators;

public class VerifyOtpValidator : AbstractValidator<VerifyOtpDto>
{
    public VerifyOtpValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş olamaz");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Doğrulama kodu boş olamaz")
            .Length(6).WithMessage("Doğrulama kodu 6 haneli olmalıdır")
            .Matches(@"^[0-9]{6}$").WithMessage("Doğrulama kodu sadece rakamlardan oluşmalıdır");
    }
}
