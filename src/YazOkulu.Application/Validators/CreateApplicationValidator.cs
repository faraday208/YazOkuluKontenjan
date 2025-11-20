using FluentValidation;
using YazOkulu.Application.DTOs.Applications;

namespace YazOkulu.Application.Validators;

public class CreateApplicationValidator : AbstractValidator<CreateApplicationDto>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Geçerli bir ders seçiniz");
    }
}
