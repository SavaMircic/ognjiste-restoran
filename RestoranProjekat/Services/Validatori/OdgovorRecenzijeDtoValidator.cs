using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class OdgovorRecenzijeDtoValidator : AbstractValidator<OdgovorRecenzijeDto>
{
    public OdgovorRecenzijeDtoValidator()
    {
        RuleFor(x => x.OdgovorRestorana).NotEmpty().WithMessage("Odgovor je obavezan.").MaximumLength(1000);
    }
}
