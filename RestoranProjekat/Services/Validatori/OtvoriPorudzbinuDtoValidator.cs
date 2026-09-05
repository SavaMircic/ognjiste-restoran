using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class OtvoriPorudzbinuDtoValidator : AbstractValidator<OtvoriPorudzbinuDto>
{
    public OtvoriPorudzbinuDtoValidator()
    {
        RuleFor(x => x.StoId).GreaterThan(0).WithMessage("Sto je obavezan.");
    }
}
