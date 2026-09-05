using FluentValidation;
using Services.DTO;

namespace Services.Validatori;

public class ResetLozinkeDtoValidator : AbstractValidator<ResetLozinkeDto>
{
    public ResetLozinkeDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty().WithMessage("Token iz mejla nedostaje.");
        RuleFor(x => x.NovaLozinka)
            .NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Lozinka mora sadržati bar jedno veliko slovo.")
            .Matches("[0-9]").WithMessage("Lozinka mora sadržati bar jedan broj.");
    }
}
