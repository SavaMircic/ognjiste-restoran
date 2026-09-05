using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class ZaboravljenaLozinkaDtoValidator : AbstractValidator<ZaboravljenaLozinkaDto>
{
    public ZaboravljenaLozinkaDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
