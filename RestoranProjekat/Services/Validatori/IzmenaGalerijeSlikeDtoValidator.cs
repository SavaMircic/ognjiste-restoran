using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaGalerijeSlikeDtoValidator : AbstractValidator<IzmenaGalerijeSlikeDto>
{
    public IzmenaGalerijeSlikeDtoValidator()
    {
        RuleFor(x => x.Naslov)
            .NotEmpty().WithMessage("Naslov je obavezan.")
            .MaximumLength(150);

        RuleFor(x => x.Opis).MaximumLength(500);

        RuleFor(x => x.Grupa)
            .NotEmpty().WithMessage("Oblast je obavezna.")
            .MaximumLength(100);

        RuleFor(x => x.Redosled)
            .InclusiveBetween(0, 999).WithMessage("Redosled mora biti između 0 i 999.");
    }
}
