using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class BlokirajKorisnikaDtoValidator : AbstractValidator<BlokirajKorisnikaDto>
{
    public BlokirajKorisnikaDtoValidator()
    {
        RuleFor(x => x.Razlog).NotEmpty().WithMessage("Razlog je obavezan.").MaximumLength(500);
        RuleFor(x => x.DatumDo)
            .GreaterThan(DateTime.UtcNow).WithMessage("Datum do kojeg traje blokada mora biti u budućnosti.")
            .When(x => x.DatumDo.HasValue);
    }
}
