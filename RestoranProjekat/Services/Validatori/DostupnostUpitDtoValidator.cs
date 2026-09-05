using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class DostupnostUpitDtoValidator : AbstractValidator<DostupnostUpitDto>
{
    public DostupnostUpitDtoValidator()
    {
        RuleFor(x => x.DatumVreme).GreaterThan(DateTime.UtcNow).WithMessage("Termin mora biti u budućnosti.");
        RuleFor(x => x.BrojGostiju).GreaterThan(0).WithMessage("Broj gostiju mora biti veći od nule.");
    }
}
