using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class IzmenaKolicineDtoValidator : AbstractValidator<IzmenaKolicineDto>
{
    public IzmenaKolicineDtoValidator()
    {
        RuleFor(x => x.Kolicina).GreaterThan(0).WithMessage("Količina mora biti veća od nule.");
    }
}
