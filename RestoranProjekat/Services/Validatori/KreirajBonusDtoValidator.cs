using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class KreirajBonusDtoValidator : AbstractValidator<KreirajBonusDto>
{
    public KreirajBonusDtoValidator()
    {
        RuleFor(x => x.ZaposleniIds)
            .NotEmpty().WithMessage("Mora biti izabran bar jedan zaposleni.");

        RuleFor(x => x.ZaposleniIds)
            .Must(l => l.All(id => id > 0)).WithMessage("Svi id-jevi zaposlenih moraju biti pozitivni.")
            .Must(l => l.Distinct().Count() == l.Count).WithMessage("Isti zaposleni je naveden više puta.")
            .Must(l => l.Count <= 20).WithMessage("Grupni bonus može obuhvatiti najviše 20 zaposlenih.")
            .When(x => x.ZaposleniIds.Count > 0);

        RuleFor(x => x.Iznos).GreaterThan(0).WithMessage("Iznos bonusa mora biti veći od nule.");

        RuleFor(x => x.DatumPocetka)
            .NotEqual(default(DateOnly)).WithMessage("Datum početka je obavezan.");

        RuleFor(x => x.DatumKraja)
            .GreaterThanOrEqualTo(x => x.DatumPocetka)
            .WithMessage("Datum kraja ne može biti pre datuma početka.");

        RuleFor(x => x.Razlog)
            .NotEmpty().WithMessage("Razlog dodele bonusa je obavezan.")
            .MaximumLength(500);
    }
}
