using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class OdgovorNaPorukuDtoValidator : AbstractValidator<OdgovorNaPorukuDto>
{
    public OdgovorNaPorukuDtoValidator()
    {
        RuleFor(x => x.Odgovor).NotEmpty().WithMessage("Odgovor je obavezan.").MaximumLength(2000);
    }
}
