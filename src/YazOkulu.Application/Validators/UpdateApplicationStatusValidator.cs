using FluentValidation;
using YazOkulu.Application.DTOs.Applications;

namespace YazOkulu.Application.Validators;

public class UpdateApplicationStatusValidator : AbstractValidator<UpdateApplicationStatusDto>
{
    public UpdateApplicationStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Geçerli bir durum seçiniz");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(500).WithMessage("İnceleme notu en fazla 500 karakter olabilir");
    }
}
