using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class StoUlazDtoValidator : AbstractValidator<StoUlazDto>
{
    public StoUlazDtoValidator()
    {
        RuleFor(x => x.BrojStola).GreaterThan(0).WithMessage("Broj stola mora biti veći od nule.");
        RuleFor(x => x.Kapacitet).GreaterThan(0).WithMessage("Kapacitet mora biti veći od nule.");
    }
}
