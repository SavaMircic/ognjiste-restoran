using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class DodajURecepturuDtoValidator : AbstractValidator<DodajURecepturuDto>
{
    public DodajURecepturuDtoValidator()
    {
        RuleFor(x => x.NamirnicaId).GreaterThan(0).WithMessage("Namirnica je obavezna.");
        RuleFor(x => x.Kolicina).GreaterThan(0).WithMessage("Količina mora biti veća od nule.");
    }
}
