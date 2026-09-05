using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class ZabraniKomentarisanjeDtoValidator : AbstractValidator<ZabraniKomentarisanjeDto>
{
    public ZabraniKomentarisanjeDtoValidator()
    {
        RuleFor(x => x.BrojDana).InclusiveBetween(1, 365).WithMessage("Broj dana mora biti između 1 i 365.");

        RuleFor(x => x.Razlog)
            .NotEmpty().WithMessage("Razlog je obavezan.")
            .MaximumLength(500);
    }
}
