using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class PotvrdaEmailDtoValidator : AbstractValidator<PotvrdaEmailDto>
{
    public PotvrdaEmailDtoValidator()
    {
        RuleFor(x => x.KorisnikId).NotEmpty().WithMessage("KorisnikId je obavezan.");
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token je obavezan.");
    }
}
