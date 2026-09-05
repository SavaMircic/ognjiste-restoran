using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class PromenaLozinkeDtoValidator : AbstractValidator<PromenaLozinkeDto>
{
    public PromenaLozinkeDtoValidator()
    {
        RuleFor(x => x.StaraLozinka).NotEmpty().WithMessage("Unesite trenutnu lozinku.");
        RuleFor(x => x.NovaLozinka)
            .NotEmpty().WithMessage("Nova lozinka je obavezna.")
            .MinimumLength(8).WithMessage("Nova lozinka mora imati bar 8 karaktera.")
            .Matches("[A-Z]").WithMessage("Nova lozinka mora sadržati bar jedno veliko slovo.")
            .Matches("[0-9]").WithMessage("Nova lozinka mora sadržati bar jedan broj.")
            .NotEqual(x => x.StaraLozinka).WithMessage("Nova lozinka mora biti različita od stare.");
    }
}
