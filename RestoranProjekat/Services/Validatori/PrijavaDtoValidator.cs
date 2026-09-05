using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class PrijavaDtoValidator : AbstractValidator<PrijavaDto>
{
    public PrijavaDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email je obavezan.").EmailAddress();
        RuleFor(x => x.Lozinka).NotEmpty().WithMessage("Lozinka je obavezna.");
    }
}
