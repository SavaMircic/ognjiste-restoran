using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KorekcijaKolicineDtoValidator : AbstractValidator<KorekcijaKolicineDto>
{
    public KorekcijaKolicineDtoValidator()
    {
        RuleFor(x => x.Razlog).NotEmpty().WithMessage("Razlog korekcije je obavezan.").MaximumLength(500);

        RuleFor(x => x)
            .Must(x => x.NovaKolicina.HasValue ^ x.Delta.HasValue)
            .WithMessage("Pošaljite tačno jedno: NovaKolicina (stanje posle popisa) ili Delta (promena).");

        RuleFor(x => x.NovaKolicina)
            .GreaterThanOrEqualTo(0).WithMessage("Nova količina ne može biti negativna.")
            .When(x => x.NovaKolicina.HasValue);

        RuleFor(x => x.Delta)
            .NotEqual(0).WithMessage("Delta ne može biti nula.")
            .When(x => x.Delta.HasValue);
    }
}
