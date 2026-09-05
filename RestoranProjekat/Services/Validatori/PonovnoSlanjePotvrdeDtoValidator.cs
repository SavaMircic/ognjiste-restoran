using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class PonovnoSlanjePotvrdeDtoValidator : AbstractValidator<PonovnoSlanjePotvrdeDto>
{
    public PonovnoSlanjePotvrdeDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
