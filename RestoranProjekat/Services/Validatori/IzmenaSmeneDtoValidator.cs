using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaSmeneDtoValidator : AbstractValidator<IzmenaSmeneDto>
{
    public IzmenaSmeneDtoValidator()
    {
        RuleFor(x => x.Datum)
            .NotEqual(default(DateOnly)).WithMessage("Datum smene je obavezan.");

        RuleFor(x => x.VremeKraja)
            .NotEqual(x => x.VremePocetka).WithMessage("Vreme kraja ne može biti isto kao vreme početka.");
    }
}
