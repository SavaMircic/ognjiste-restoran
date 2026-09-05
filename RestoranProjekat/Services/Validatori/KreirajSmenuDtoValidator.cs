using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KreirajSmenuDtoValidator : AbstractValidator<KreirajSmenuDto>
{
    public KreirajSmenuDtoValidator()
    {
        RuleFor(x => x.ZaposleniId).GreaterThan(0).WithMessage("Zaposleni je obavezan.");

        RuleFor(x => x.Datum)
            .NotEqual(default(DateOnly)).WithMessage("Datum smene je obavezan.");

        RuleFor(x => x.VremeKraja)
            .NotEqual(x => x.VremePocetka).WithMessage("Vreme kraja ne može biti isto kao vreme početka.");
    }
}
