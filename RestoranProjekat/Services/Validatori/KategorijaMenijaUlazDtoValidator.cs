using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KategorijaMenijaUlazDtoValidator : AbstractValidator<KategorijaMenijaUlazDto>
{
    public KategorijaMenijaUlazDtoValidator()
    {
        RuleFor(x => x.Naziv).NotEmpty().WithMessage("Naziv je obavezan.").MaximumLength(100);
        RuleFor(x => x.Opis).MaximumLength(500);
        RuleFor(x => x.Redosled).GreaterThanOrEqualTo(0).WithMessage("Redosled ne može biti negativan.");
        RuleFor(x => x.Odrediste).IsInEnum().WithMessage("Odredište mora biti Kuhinja ili Sank.");
    }
}
